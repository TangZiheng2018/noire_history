using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
            pp.AutoDepthStencilFormat = Format.D16;

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
            _earthTexture.Dispose();
            //_parallelLightEffect.Dispose();
            _pointLightEffect.Dispose();
            _keyboard.Unacquire();
            _keyboard.Dispose();
            _directInput.Dispose();
            _device.Dispose();
            _direct3D.Dispose();
        }

        private void Run() {
            using (var renderLoop = new RenderLoop(this)) {
                //var filename = @"C:\Users\MIC\Documents\3dsMax\export\ball.3ds";
                //var filename = @"C:\Users\MIC\Desktop\ea55\17_3ds.3ds";
                var filename = @"C:\Users\MIC\Documents\3dsMax\export\box_ground.3ds";
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
            //_parallelLightEffect = Effect.FromFile(device, "ParallelLight.fx", ShaderFlags.None);
            _pointLightEffect = Effect.FromFile(device, "PointLight.fx", ShaderFlags.None);
            var material = new Material();
            material.Ambient = new Color(0, 0, 0);
            material.Diffuse = new Color(44, 44, 44);
            material.Emissive = new Color(191, 191, 191);
            material.Specular = new Color(255, 255, 255);
            _defaultMaterial = material;
            _effect = _pointLightEffect;

            _earthTexture = Texture.FromFile(device, "earth.jpg");
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
                        new VertexElement(0, 32, DeclarationType.Float2, DeclarationMethod.Default,  DeclarationUsage.TextureCoordinate, 0),
                        VertexElement.VertexDeclarationEnd
                    });
                    foreach (var face in mesh.Faces) {
                        for (var i = 0; i < 3; ++i) {
                            var index = face.Indices[i];
                            f1.AddRange(new float[] { mesh.Vertices[index].X, mesh.Vertices[index].Y, mesh.Vertices[index].Z, 1 });
                            f1.AddRange(new float[] { mesh.Normals[index].X, mesh.Normals[index].Y, mesh.Normals[index].Z, 1 });
                            // http://www.cnblogs.com/graphics/archive/2011/09/13/2174022.html, sphere only
                            // 上面代码有问题
                            // 不过这个例子说明了自动纹理映射的思想：法线投影！
                            float tu;
                            Vector2 v2 = new Vector2(mesh.Normals[index].X, mesh.Normals[index].Y);
                            v2.Normalize();
                            if (Math.Abs(v2.X) < float.Epsilon) {
                                tu = Math.Sign(v2.Y) > 0 ? 0f : 1;
                            } else {
                                tu = -(float)(Math.Atan2(v2.Y, v2.X) / Math.PI / 2) + 0.5f;
                            }
                            //var tu = (float)(Math.Asin(mesh.Normals[index].X) / Math.PI) + 0.5f;
                            float tv = (float)(Math.Acos(mesh.Normals[index].Z) / Math.PI);
                            f1.AddRange(new float[] { tu, tv });
                        }
                        us1.AddRange(new uint[] { (uint)face.Indices[0], (uint)face.Indices[1], (uint)face.Indices[2] });
                    }
                    meshData.Indices = us1.ToArray();
                    meshData.Data = f1.ToArray();
                    meshData.Stride = Utilities.SizeOf<float>() * 10;
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

        //private void CombineIntoOneMesh(Device device) {
        //    var meshData = new MeshData();
        //    meshData.VertexDeclaration = new VertexDeclaration(device, new[] {
        //                new VertexElement(0,0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
        //                new VertexElement(0,16, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
        //                VertexElement.VertexDeclarationEnd
        //            });
        //    meshData.Stride = Utilities.SizeOf<float>() * 8;
        //    meshData.Material = _defaultMaterial;
        //    List<uint> ui1 = new List<uint>();
        //    List<float> fl1 = new List<float>();
        //    uint baseIndex = 0;
        //    foreach (var mesh in _meshes) {
        //        fl1.AddRange(mesh.Data);
        //        ui1.AddRange(from index in mesh.Indices select (index + baseIndex));
        //        baseIndex += (uint)mesh.Indices.Length;
        //    }
        //    foreach (var mesh in _meshes) {
        //        mesh.Dispose();
        //    }
        //    meshData.Data = fl1.ToArray();
        //    meshData.Indices = ui1.ToArray();
        //    meshData.Build(device);
        //    _meshes.Clear();
        //    _meshes.Add(meshData);
        //}

        private void ControlElements() {
            var state = _keyboard.GetCurrentState();
            //_camera2.React(state, _camera2.ViewMatrix);
            _camera.React(state);
            bool isEffectReset = false;
            //if (!isEffectReset && state.IsPressed(Key.D1)) {
            //    _effect = _parallelLightEffect;
            //    isEffectReset = true;
            //}
            if (state.IsPressed(Key.Left)) {
                _rotationZ -= 5;
            }
            if (state.IsPressed(Key.Right)) {
                _rotationZ += 5;
            }
            if (state.IsPressed(Key.Up)) {
                _rotationX -= 5;
            }
            if (state.IsPressed(Key.Down)) {
                _rotationX += 5;
            }
            if (!isEffectReset && state.IsPressed(Key.D2)) {
                _effect = _pointLightEffect;
                isEffectReset = true;
            }
            if (state.IsPressed(Key.Add) || state.IsPressed(Key.Equals)) {
                _specularPower += 0.5f;
            }
            if (state.IsPressed(Key.Subtract) || state.IsPressed(Key.Minus)) {
                _specularPower = Math.Max(_specularPower -= 0.5f, 0f);
            }
            // O: mat. override on; I: off
            bool isMaterialOverridden = false;
            if (!isMaterialOverridden && state.IsPressed(Key.I)) {
                _useDefaultMaterialOverride = false;
                isMaterialOverridden = true;
            }
            if (!isMaterialOverridden && state.IsPressed(Key.O)) {
                _useDefaultMaterialOverride = true;
                isMaterialOverridden = true;
            }
            bool isShadowAmbientChanged = false;
            if (!isShadowAmbientChanged && state.IsPressed(Key.K)) {
                _shadowAmbient = false;
                isShadowAmbientChanged = true;
            }
            if (!isShadowAmbientChanged && state.IsPressed(Key.L)) {
                _shadowAmbient = true;
                isShadowAmbientChanged = true;
            }
            bool isUseTextureChanged = false;
            if (!isUseTextureChanged && state.IsPressed(Key.N)) {
                _useTexture = false;
                isUseTextureChanged = true;
            }
            if (!isUseTextureChanged && state.IsPressed(Key.M)) {
                _useTexture = true;
                isUseTextureChanged = true;
            }
        }

        private static Matrix PerspectiveLHWorkaround(float fov, float aspect, float znear, float zfar) {
            //var m = new Matrix();
            //m.M11 = (float)(1 / Math.Tan(fov * 0.5)) / aspect;
            //m.M22 = (float)(1 / Math.Tan(fov * 0.5));
            //m.M33 = far / (far - near);
            //m.M34 = 1;
            //m.M43 = far * near / (near - far);
            //return m;
            //var m = new Matrix();
            //var fCos = (float)Math.Cos(fov / 2);
            //var fSin = (float)Math.Sin(fov / 2);
            //var q = (far * fSin) / (far - near);
            //m.M11 = fCos * aspect;
            //m.M22 = fCos;
            //m.M33 = q;
            //m.M34 = fSin;
            //m.M43 = -q * near;
            //return m;
            float yScale = (float)(1.0f / Math.Tan(fov * 0.5f));
            float q = zfar / (zfar - znear);
            var result = new Matrix();
            result.M11 = yScale / aspect;
            result.M22 = yScale;
            result.M33 = q;
            result.M34 = 1.0f;
            result.M43 = -q * znear;
            return result;
        }

        private void UpdateAndRender(Scene scene, Size2 clientSize) {
            var device = _device;
            var effect = _effect;
            //var meshIndices = new int[] { 0, 1, 2, 3 };
            //var meshIndices = new int[] { 0 };
            var meshIndices = Range.ReversedInt32(_meshes.Count, _meshes.Count);
            MeshData mesh;
            if (!_matrixCalced) {
                var eye = new Vector3(0, -200, 0);
                var up = new Vector3(0, 0, 1);
                var lookAt = new Vector3(0, 0, 0);
                using (var fs = new FileStream("test.txt", FileMode.OpenOrCreate | FileMode.Truncate, FileAccess.Write)) {
                    using (var w = new StreamWriter(fs, Encoding.UTF8)) {
                        Matrix matWorld = Matrix.Identity;
                        Matrix matProj = PerspectiveLHWorkaround(MathUtil.DegreesToRadians(45f), (float)clientSize.Width / clientSize.Height, 1, 1000);
                        //Matrix matView = _camera.ViewMatrix;
                        Matrix matView = Matrix.LookAtLH(eye, lookAt, up);
                        Matrix matMVP = matWorld * matView * matProj;
                        Vector4 vec = new Vector4(0, 0, 0, 1);
                        Vector4 vec2;
                        foreach (var index in meshIndices) {
                            mesh = _meshes[index];
                            for (var i = 0; i < mesh.Data.Length; i += (mesh.Stride / Utilities.SizeOf<float>())) {
                                vec.X = mesh.Data[i];
                                vec.Y = mesh.Data[i + 1];
                                vec.Z = mesh.Data[i + 2];
                                vec2 = vec.RightMultiply(matMVP);
                                w.WriteLine($"V #{i / 2 / Utilities.SizeOf<float>()} Original: {vec}, Transformed: {vec2}");
                            }
                        }
                    }
                }
                _matrixCalced = true;
            }
            foreach (var index in meshIndices) {
                mesh = _meshes[index];
                Material material = _useDefaultMaterialOverride ? _defaultMaterial : mesh.Material;
                device.SetStreamSource(0, mesh.VertexBuffer, 0, mesh.Stride);
                device.Indices = mesh.IndexBuffer;
                device.VertexDeclaration = mesh.VertexDeclaration;
                /*if (effect == _parallelLightEffect) {
                    SetParallelLightEffectParams(clientSize, material);
                } else*/
                if (effect == _pointLightEffect) {
                    SetPointLightEffectParams(clientSize, material);
                } else {
                    throw new ArgumentOutOfRangeException(nameof(effect));
                }
                var numPasses = effect.Begin();
                for (var i = 0; i < numPasses; ++i) {
                    effect.BeginPass(i);
                    device.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, mesh.Data.Length / (mesh.Stride / Utilities.SizeOf<float>()), 0, mesh.Indices.Length / 3);
                    effect.EndPass();
                }
                effect.End();
            }
        }

        //private void SetParallelLightEffectParams(Size2 clientSize, Material material) {
        //    var e = _parallelLightEffect;
        //    var tech = e.GetTechnique("SpecularLight");
        //    e.Technique = tech;
        //    Matrix matWorld = Matrix.Identity;
        //    Matrix matProj = PerspectiveLHWorkaround(MathUtil.DegreesToRadians(45f), (float)clientSize.Width / clientSize.Height, 0, 1000);
        //    Matrix matView = _camera.ViewMatrix;
        //    //Matrix matView = _camera2.ViewMatrix;
        //    //Matrix matView = Matrix.LookAtLH(Vector3.UnitX * 700 + Vector3.UnitY * 500, Vector3.UnitY * -1000, Vector3.UnitZ);
        //    e.SetValue(e.GetParameter(null, "matWorldViewProj"), matWorld * matView * matProj);
        //    e.SetValue(e.GetParameter(null, "matWorld"), matWorld);
        //    var lightPosition = new Vector4(20, 20, 20, 1);
        //    e.SetValue(e.GetParameter(null, "vecEye"), lightPosition);
        //    //e.SetValue(e.GetParameter(null, "vecEye"), new Vector4(_camera.Position, 1));
        //    var lightDirection = new Vector4(0, -1, -1, 0);
        //    lightDirection.Normalize();
        //    lightDirection.W = 1;
        //    e.SetValue(e.GetParameter(null, "vecLightDir"), lightDirection);
        //    e.SetValue(e.GetParameter(null, "vDiffuseColor"), material.Diffuse);
        //    e.SetValue(e.GetParameter(null, "vSpecularColor"), material.Specular);
        //    e.SetValue(e.GetParameter(null, "vAmbient"), material.Ambient);
        //    e.SetValue(e.GetParameter(null, "fPower"), _specularPower);
        //    e.SetValue(e.GetParameter(null, "bShadowAmbient"), _shadowAmbient);
        //}

        private void SetPointLightEffectParams(Size2 clientSize, Material material) {
            var e = _pointLightEffect;
            var tech = e.GetTechnique("SpecularLight");
            e.Technique = tech;
            var eye = new Vector3(0, -200, 0);
            var up = new Vector3(0, 0, 1);
            var lookAt = new Vector3(0, 0, 0);

            Matrix matWorld = Matrix.RotationZ(MathUtil.DegreesToRadians(_rotationZ)) * Matrix.RotationX(MathUtil.DegreesToRadians(_rotationX));
            Matrix matProj = PerspectiveLHWorkaround(MathUtil.DegreesToRadians(45f), (float)clientSize.Width / clientSize.Height, 1, 1000);
            //Matrix matView = Matrix.LookAtLH(eye, lookAt, up);
            Matrix matView = _camera.ViewMatrix;
            //Matrix matView = _camera2.ViewMatrix;
            //Matrix matView = Matrix.LookAtLH(Vector3.UnitX * 700 + Vector3.UnitY * 500, Vector3.UnitY * -1000, Vector3.UnitZ);
            //e.SetValue(e.GetParameter(null, "matWorldViewProj"), matWorld * matView * matProj);
            e.SetValue(e.GetParameter(null, "matWorldViewProj"), matWorld * matView * matProj);
            e.SetValue(e.GetParameter(null, "matWorld"), matWorld);
            var m = matWorld;
            m.Invert();
            e.SetValue(e.GetParameter(null, "matWorldInv"), m);
            var lightPosition = new Vector3(300, 200, 0);
            //e.SetValue(e.GetParameter(null, "vecEye"), lightPosition);
            e.SetValue(e.GetParameter(null, "vecEye"), _camera.Position);
            e.SetValue(e.GetParameter(null, "vecLightPos"), lightPosition);
            e.SetValue(e.GetParameter(null, "vDiffuseColor"), material.Diffuse);
            e.SetValue(e.GetParameter(null, "vSpecularColor"), material.Specular);
            e.SetValue(e.GetParameter(null, "vAmbient"), material.Ambient);
            e.SetValue(e.GetParameter(null, "fPower"), _specularPower);
            e.SetValue(e.GetParameter(null, "bShadowAmbient"), _shadowAmbient);
            e.SetTexture(e.GetParameter(null, "texSphere"), _earthTexture);
            e.SetValue(e.GetParameter(null, "bUseTexture"), _useTexture);
        }

        private Direct3D _direct3D;
        private DirectInput _directInput;
        private Keyboard _keyboard;
        private Device _device;
        private AssimpContext _aiContext;
        private SimpleCamera _camera = new SimpleCamera();
        private SimpleCamera2 _camera2 = new SimpleCamera2();
        private Effect _effect;
        //private Effect _parallelLightEffect;
        private Effect _pointLightEffect;
        private List<MeshData> _meshes = new List<MeshData>();
        private Material _defaultMaterial;
        private float _rotationZ = 0;
        private float _rotationX = 0;
        private float _specularPower = 15f;
        private bool _useDefaultMaterialOverride = false;
        private bool _shadowAmbient = false;
        private bool _matrixCalced = false;
        private Texture _earthTexture = null;
        private bool _useTexture = false;

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
