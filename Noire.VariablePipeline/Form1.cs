using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using SharpDX.DirectInput;
using Color = SharpDX.Color;
using Device = SharpDX.Direct3D9.Device;
using DeviceType = SharpDX.Direct3D9.DeviceType;
using PrimitiveType = SharpDX.Direct3D9.PrimitiveType;
using Effect = SharpDX.Direct3D9.Effect;
using Material = SharpDX.Direct3D9.Material;
using Assimp;

namespace Noire.VariablePipeline {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers() {
            Load += Form1_Load;
            Closed += Form1_Closed;
        }

        private void Form1_Closed(object sender, EventArgs e) {
            DisposeD3DContext();
            _aiContext.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e) {
            _aiContext = new AssimpContext();
            CreateD3DContext();
            Show();
            Focus();

            Run();
        }

        private void CreateD3DContext() {
            _direct3D = new Direct3D();

            var createFlags = CreateFlags.HardwareVertexProcessing;
            var clientSize = ClientSize;
            PresentParameters pp = new PresentParameters(clientSize.Width, clientSize.Height);
            pp.PresentationInterval = PresentInterval.Default;
            pp.SwapEffect = SwapEffect.Discard;
            pp.Windowed = true;
            pp.DeviceWindowHandle = Handle;
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = Format.D24X8;

            var device = new Device(_direct3D, 0, DeviceType.Hardware, Handle, createFlags, pp);
            _device = device;
            device.SetRenderState(RenderState.Lighting, false);

            _directInput = new DirectInput();
            _keyboard = new Keyboard(_directInput);
            _keyboard.Acquire();
        }

        private void DisposeD3DContext() {
            foreach (var meshData in _meshes) {
                meshData.Dispose();
            }
            _parallelLightEffect.Dispose();
            _pointLightEffect.Dispose();
            _keyboard.Unacquire();
            _keyboard.Dispose();
            _directInput.Dispose();
            _device.Dispose();
            _direct3D.Dispose();
        }

        private void Run() {
            using (var renderLoop = new RenderLoop(this)) {
                var filename = @"C:\Users\MIC\Documents\3dsMax\export\ball.3ds";
                //var filename = @"C:\Users\MIC\Desktop\ea55\17_3ds.3ds";
                //var filename = @"C:\Users\MIC\Documents\3dsMax\export\box_ground.3ds";
                InitializeEffect();
                var scene = _aiContext.ImportFile(filename);
                var clientSize = new Size2(ClientSize.Width, ClientSize.Height);
                var device = _device;
                SetDeviceStates(device);
                InitializeModelData(scene);
                //CombineIntoOneMesh(device);
                while (renderLoop.NextFrame()) {
                    _keyboard.Poll();
                    ControlElements();
                    device.BeginScene();
                    device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new Color(51, 51, 51, 255), 1, 0);
                    UpdateAndRender(scene, clientSize);
                    device.EndScene();
                    device.Present();
                }
            }
        }

        private void SetDeviceStates(Device device) {
            device.SetRenderState(RenderState.ZEnable, true);
            device.SetRenderState(RenderState.ZFunc, Compare.Less);
        }

        private void InitializeEffect() {
            var device = _device;
            _parallelLightEffect = Effect.FromFile(device, "ParallelLight.fx", ShaderFlags.None);
            _pointLightEffect = Effect.FromFile(device, "PointLight.fx", ShaderFlags.None);
            var material = new Material();
            material.Ambient = new Color(15, 15, 15);
            material.Diffuse = new Color(220, 220, 220);
            material.Emissive = new Color(191, 191, 191);
            material.Specular = new Color(255, 255, 255);
            _defaultMaterial = material;
            _effect = _pointLightEffect;
        }

        private void InitializeModelData(Scene scene) {
            MeshData meshData;
            var device = _device;
            List<float> f1 = new List<float>();
            List<uint> us1 = new List<uint>();
            if (scene.HasMeshes) {
                foreach (var mesh in scene.Meshes) {
                    if (!mesh.HasNormals || !mesh.HasFaces) {
                        throw new ApplicationException("Need faces and normals.");
                    }
                    meshData = new MeshData();
                    meshData.VertexDeclaration = new VertexDeclaration(device, new[] {
                        new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                        new VertexElement(0, 16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                        VertexElement.VertexDeclarationEnd
                    });
                    foreach (var face in mesh.Faces) {
                        for (var i = 0; i < 3; ++i) {
                            var index = face.Indices[i];
                            f1.AddRange(new float[] { mesh.Vertices[index].X, mesh.Vertices[index].Y, mesh.Vertices[index].Z, 1 });
                            f1.AddRange(new float[] { mesh.Normals[index].X, mesh.Normals[index].Y, mesh.Normals[index].Z, 1 });
                        }
                        us1.AddRange(new uint[] { (uint)face.Indices[0], (uint)face.Indices[1], (uint)face.Indices[2] });
                    }
                    meshData.Indices = us1.ToArray();
                    meshData.Data = f1.ToArray();
                    meshData.Stride = Utilities.SizeOf<float>() * 8;
                    meshData.Build(device);
                    f1.Clear();
                    us1.Clear();
                    if (scene.HasMaterials) {
                        var material = scene.Materials[mesh.MaterialIndex];
                        var dxMaterial = new Material();
                        if (material.HasColorAmbient) {
                            dxMaterial.Ambient = new Color(material.ColorAmbient.R, material.ColorAmbient.G, material.ColorAmbient.B, material.ColorAmbient.A);
                        } else {
                            dxMaterial.Ambient = _defaultMaterial.Ambient;
                        }
                        if (material.HasColorDiffuse) {
                            dxMaterial.Diffuse = new Color(material.ColorDiffuse.R, material.ColorDiffuse.G, material.ColorDiffuse.B, material.ColorDiffuse.A);
                        } else {
                            dxMaterial.Diffuse = _defaultMaterial.Diffuse;
                        }
                        if (material.HasColorEmissive) {
                            dxMaterial.Emissive = new Color(material.ColorEmissive.R, material.ColorEmissive.G, material.ColorEmissive.B, material.ColorEmissive.A);
                        } else {
                            dxMaterial.Emissive = _defaultMaterial.Emissive;
                        }
                        if (material.HasColorSpecular) {
                            dxMaterial.Specular = new Color(material.ColorSpecular.R, material.ColorSpecular.G, material.ColorSpecular.B, material.ColorSpecular.A);
                        } else {
                            dxMaterial.Specular = _defaultMaterial.Specular;
                        }
                        meshData.Material = dxMaterial;
                    } else {
                        meshData.Material = _defaultMaterial;
                    }
                    _meshes.Add(meshData);
                }
            }
        }

        private void CombineIntoOneMesh(Device device) {
            var meshData = new MeshData();
            meshData.VertexDeclaration = new VertexDeclaration(device, new[] {
                        new VertexElement(0,0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
                        new VertexElement(0,16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                        VertexElement.VertexDeclarationEnd
                    });
            meshData.Stride = Utilities.SizeOf<float>() * 8;
            meshData.Material = _defaultMaterial;
            List<uint> ui1 = new List<uint>();
            List<float> fl1 = new List<float>();
            uint baseIndex = 0;
            foreach (var mesh in _meshes) {
                fl1.AddRange(mesh.Data);
                ui1.AddRange(from index in mesh.Indices select (index + baseIndex));
                baseIndex += (uint)mesh.Indices.Length;
            }
            foreach (var mesh in _meshes) {
                mesh.Dispose();
            }
            meshData.Data = fl1.ToArray();
            meshData.Indices = ui1.ToArray();
            meshData.Build(device);
            _meshes.Clear();
            _meshes.Add(meshData);
        }

        private void ControlElements() {
            var state = _keyboard.GetCurrentState();
            //_camera2.React(state, _camera2.ViewMatrix);
            _camera.React(state);
            bool isEffectReset = false;
            if (!isEffectReset && state.IsPressed(Key.D1)) {
                _effect = _parallelLightEffect;
                isEffectReset = true;
            }
            if (!isEffectReset && state.IsPressed(Key.D2)) {
                _effect = _pointLightEffect;
                isEffectReset = true;
            }
        }

        private static Matrix PerspectiveWorkaround(float fov, float aspect, float near, float far) {
            var m = new Matrix();
            m.M11 = (float)(1 / Math.Tan(fov * 0.5)) / aspect;
            m.M22 = (float)(1 / Math.Tan(fov * 0.5));
            m.M33 = far / (far - near);
            m.M34 = 1;
            m.M43 = far * near / (near - far);
            return m;
        }

        private void UpdateAndRender(Scene scene, Size2 clientSize) {
            var device = _device;
            var effect = _effect;
            //var meshIndices = new int[] { 0, 1, 2, 3 };
            //var meshIndices = new int[] { 0 };
            var meshIndices = Range.Int32(0, _meshes.Count);
            MeshData mesh;
            foreach (var index in meshIndices) {
                mesh = _meshes[index];
                device.SetStreamSource(0, mesh.VertexBuffer, 0, mesh.Stride);
                device.Indices = mesh.IndexBuffer;
                device.VertexDeclaration = mesh.VertexDeclaration;
                if (effect == _parallelLightEffect) {
                    SetParallelLightEffectParams(clientSize, mesh.Material);
                } else if (effect == _pointLightEffect) {
                    SetPointLightEffectParams(clientSize, mesh.Material);
                } else {
                    throw new ArgumentOutOfRangeException(nameof(effect));
                }
                var numPasses = effect.Begin();
                for (var i = 0; i < numPasses; ++i) {
                    var sum = 0;
                    Parallel.ForEach(new[] { 0, 1, 2, 3 }, (value) => {
                        sum += value;
                    });
                    effect.BeginPass(i);
                    device.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, mesh.Data.Length / (mesh.Stride / Utilities.SizeOf<float>()), 0, mesh.Indices.Length / 3);
                    effect.EndPass();
                }
                effect.End();
            }
        }

        private void SetParallelLightEffectParams(Size2 clientSize, Material material) {
            var e = _parallelLightEffect;
            var tech = e.GetTechnique("SpecularLight");
            e.Technique = tech;
            Matrix matWorld = Matrix.Identity;
            Matrix matProj = PerspectiveWorkaround(MathUtil.DegreesToRadians(45f), (float)clientSize.Width / clientSize.Height, 0, 1000);
            Matrix matView = _camera.ViewMatrix;
            //Matrix matView = _camera2.ViewMatrix;
            //Matrix matView = Matrix.LookAtLH(Vector3.UnitX * 700 + Vector3.UnitY * 500, Vector3.UnitY * -1000, Vector3.UnitZ);
            e.SetValue(e.GetParameter(null, "matWorldViewProj"), matWorld * matView * matProj);
            e.SetValue(e.GetParameter(null, "matWorld"), matWorld);
            var lightPosition = new Vector4(20, 20, 20, 1);
            e.SetValue(e.GetParameter(null, "vecEye"), lightPosition);
            //e.SetValue(e.GetParameter(null, "vecEye"), new Vector4(_camera.Position, 1));
            var lightDirection = new Vector4(0, -1, -1, 0);
            lightDirection.Normalize();
            lightDirection.W = 1;
            e.SetValue(e.GetParameter(null, "vecLightDir"), lightDirection);
            e.SetValue(e.GetParameter(null, "vDiffuseColor"), material.Diffuse);
            e.SetValue(e.GetParameter(null, "vSpecularColor"), material.Specular);
            e.SetValue(e.GetParameter(null, "vAmbient"), material.Ambient);
        }

        private void SetPointLightEffectParams(Size2 clientSize, Material material) {
            var e = _pointLightEffect;
            var tech = e.GetTechnique("SpecularLight");
            e.Technique = tech;
            Matrix matWorld = Matrix.Identity;
            Matrix matProj = PerspectiveWorkaround(MathUtil.DegreesToRadians(45f), (float)clientSize.Width / clientSize.Height, 0, 1000);
            Matrix matView = _camera.ViewMatrix;
            //Matrix matView = _camera2.ViewMatrix;
            //Matrix matView = Matrix.LookAtLH(Vector3.UnitX * 700 + Vector3.UnitY * 500, Vector3.UnitY * -1000, Vector3.UnitZ);
            e.SetValue(e.GetParameter(null, "matWorldViewProj"), matWorld * matView * matProj);
            e.SetValue(e.GetParameter(null, "matWorld"), matWorld);
            var lightPosition = new Vector4(20, 20, 20, 1);
            //e.SetValue(e.GetParameter(null, "vecEye"), lightPosition);
            e.SetValue(e.GetParameter(null, "vecEye"), new Vector4(_camera.Position, 1));
            e.SetValue(e.GetParameter(null, "vecLightPos"), lightPosition);
            e.SetValue(e.GetParameter(null, "vDiffuseColor"), material.Diffuse);
            e.SetValue(e.GetParameter(null, "vSpecularColor"), material.Specular);
            e.SetValue(e.GetParameter(null, "vAmbient"), material.Ambient);
        }

        private Direct3D _direct3D;
        private DirectInput _directInput;
        private Keyboard _keyboard;
        private Device _device;
        private AssimpContext _aiContext;
        private SimpleCamera _camera = new SimpleCamera();
        private SimpleCamera2 _camera2 = new SimpleCamera2();
        private Effect _effect;
        private Effect _parallelLightEffect;
        private Effect _pointLightEffect;
        private List<MeshData> _meshes = new List<MeshData>();
        private Material _defaultMaterial;
        private float _rotationDegree;

        private static readonly Vector3 OriginalLookingDirection = Vector3.UnitY;
        private static readonly Vector3 OriginalUpDirection = Vector3.UnitZ;

        private class MeshData : IDisposable {

            public float[] Data { get; set; }
            public uint[] Indices { get; set; }
            public VertexDeclaration VertexDeclaration { get; set; }
            public VertexBuffer VertexBuffer => _vertexBuffer;
            public IndexBuffer IndexBuffer => _indexBuffer;
            public int Stride { get; set; }
            public Material Material { get; set; }

            public void Build(Device device) {
                DisposeBuffers();
                DataStream ptr;
                _vertexBuffer = new VertexBuffer(device, Data.Length * Utilities.SizeOf<float>(), Usage.WriteOnly, VertexFormat.None, Pool.Managed);
                _indexBuffer = new IndexBuffer(device, Indices.Length * Utilities.SizeOf<uint>(), Usage.WriteOnly, Pool.Managed, false);
                ptr = _vertexBuffer.Lock(0, 0, LockFlags.None);
                ptr.WriteRange(Data);
                _vertexBuffer.Unlock();
                ptr = _indexBuffer.Lock(0, 0, LockFlags.None);
                ptr.WriteRange(Indices);
                _indexBuffer.Unlock();
                ptr.Dispose();
            }

            public void Dispose() {
                if (VertexDeclaration != null) {
                    VertexDeclaration.Dispose();
                    VertexDeclaration = null;
                }
                DisposeBuffers();
            }

            private void DisposeBuffers() {
                if (_vertexBuffer != null) {
                    _vertexBuffer.Dispose();
                    _vertexBuffer = null;
                }
                if (_indexBuffer != null) {
                    _indexBuffer.Dispose();
                    _indexBuffer = null;
                }
            }

            private VertexBuffer _vertexBuffer;
            private IndexBuffer _indexBuffer;

        }

    }
}
