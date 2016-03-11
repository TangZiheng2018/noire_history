using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Assimp;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.DirectInput;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using Device = SharpDX.Direct3D9.Device;
using DeviceType = SharpDX.Direct3D9.DeviceType;
using PrimitiveType = SharpDX.Direct3D9.PrimitiveType;

namespace Noire.AssimpView {
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

            _device = new Device(_direct3D, 0, DeviceType.Hardware, Handle, createFlags, pp);

            _commonVertexDeclaration = new VertexDeclaration(_device, new VertexElement[]
            {
                new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
               VertexElement.VertexDeclarationEnd,
            });

            _directInput = new DirectInput();
            _keyboard = new Keyboard(_directInput);
            _keyboard.Acquire();

            var material = new SharpDX.Direct3D9.Material();
            material.Ambient = new Color(255, 255, 255);
            material.Diffuse = new Color(255, 255, 255);
            material.Emissive = new Color(191, 191, 191);
            material.Specular = new Color(255, 255, 255);
            _defaultMaterial = material;
        }

        private void DisposeD3DContext() {
            _keyboard.Unacquire();
            _keyboard.Dispose();
            _directInput.Dispose();
            _commonVertexDeclaration.Dispose();
            _device.Dispose();
            _direct3D.Dispose();
        }

        private void Run() {
            using (var renderLoop = new RenderLoop(this)) {
                var filename = @"C:\Users\MIC\Desktop\ea55\17_3ds.3ds";
                var scene = _aiContext.ImportFile(filename);
                var clientSize = new Size2(ClientSize.Width, ClientSize.Height);
                var device = _device;
                while (renderLoop.NextFrame()) {
                    _keyboard.Poll();
                    ControlElements();
                    device.BeginScene();
                    device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.MidnightBlue, 1, 0);
                    UpdateAndRender(scene, clientSize);
                    device.EndScene();
                    device.Present();
                }
            }
        }

        private void ControlElements() {
            var state = _keyboard.GetCurrentState();
            var vecLookAt = _vecLookAt;
            var vecEye = _vecEye;
            var vecUp = _vecUp;

            var vecDirection = vecLookAt - vecEye;
            vecDirection.Normalize();
            var vecDirectionLeft = Vector3.Cross(vecDirection, vecUp);
            vecDirectionLeft.Normalize();
            if (state.IsPressed(Key.W)) {
                // 前进
                _vecEye += vecDirection * 50;
            }
            if (state.IsPressed(Key.S)) {
                // 后退
                _vecEye -= vecDirection * 50;
            }
            if (state.IsPressed(Key.A)) {
                // 左平移
                _vecEye += vecDirectionLeft * 50;
                _vecLookAt += vecDirectionLeft * 50;
            }
            if (state.IsPressed(Key.D)) {
                // 右平移
                _vecEye -= vecDirectionLeft * 50;
                _vecLookAt -= vecDirectionLeft * 50;
            }
            if (state.IsPressed(Key.Left)) {
                // 左旋转
                _vecLookAt += vecDirectionLeft * 50;
            }
            if (state.IsPressed(Key.Right)) {
                // 右旋转
                _vecLookAt -= vecDirectionLeft * 50;
            }
            if (state.IsPressed(Key.Up)) {
                // 上旋转
                _vecUp -= vecDirection;
                _vecLookAt += vecUp;
                _vecUp.Normalize();
            }
            if (state.IsPressed(Key.Down)) {
                // 下旋转
                _vecUp += vecDirection;
                _vecLookAt -= vecUp;
                _vecUp.Normalize();
            }

            ////var eye = _vecEye;
            ////var angles = _vecAngles;
            ////angles.X = (float)(Math.PI / 4);
            ////var mat = Matrix.RotationYawPitchRoll(angles.X, angles.Y, angles.Z);
            ////var lookingDirection4 = Multiply(mat, new Vector4(0, 1, 0, 1));
            ////var lookingDirection = new Vector3(lookingDirection4.X, lookingDirection4.Y, lookingDirection4.Z);
            //var matCameraRotation = Matrix.RotationYawPitchRoll(_vecAngles.X, _vecAngles.Y, _vecAngles.Z);
            //var vecNewLookingAt = Multiply(matCameraRotation, OriginalLookingDirection);
            //var vecNewUp = Multiply(matCameraRotation, OriginalUpDirection);
            //var vecNewLeft = Vector3.Cross(vecNewUp, vecNewLookingAt);
            //vecNewLookingAt.Normalize();
            //vecNewUp.Normalize();
            //vecNewLeft.Normalize();
            //if (state.IsPressed(Key.Left)) {
            //    _vecAngles.X += 0.1f;
            //}
            //if (state.IsPressed(Key.Right)) {
            //    _vecAngles.X -= 0.1f;
            //}
            //if (state.IsPressed(Key.Up)) {
            //    _vecAngles.Y += 0.1f;
            //}
            //if (state.IsPressed(Key.Down)) {
            //    _vecAngles.Y -= 0.1f;
            //}
            //if (state.IsPressed(Key.W)) {
            //    _vecEye += vecNewLookingAt * 10;
            //}
            //if (state.IsPressed(Key.S)) {
            //    _vecEye -= vecNewLookingAt * 10;
            //}
            //if (state.IsPressed(Key.A)) {
            //    _vecEye += vecNewLeft * 10;
            //}
            //if (state.IsPressed(Key.D)) {
            //    _vecEye -= vecNewLeft * 10;
            //}
        }

        private static Matrix PerspectiveWorkaround(float fov, float aspect, float near, float far) {
            var m = new Matrix();
            m.M11 = (float)(1 / Math.Tan(fov * 0.5)) / aspect;
            m.M22 = (float)(1 / Math.Tan(fov * 0.5));
            m.M33 = far / (far - near);
            m.M34 = 1;
            m.M44 = far * near / (near - far);
            return m;
        }

        private void SetCommonD3DStates(Size2 clientSize) {
            var device = _device;
            device.SetRenderState(RenderState.Lighting, true);
            device.SetRenderState(RenderState.CullMode, Cull.None);
            device.SetRenderState(RenderState.NormalizeNormals, true);
            device.SetRenderState(RenderState.SpecularEnable, true);
            //device.SetRenderState(RenderState.ZEnable, true);
            //device.SetRenderState(RenderState.ZWriteEnable, true);
            //device.SetRenderState(RenderState.ZFunc, Compare.Less);
            device.SetRenderState(RenderState.Ambient, new Color(92, 92, 92).ToBgra());

            //var matCameraRotation = Matrix.RotationYawPitchRoll(_vecAngles.X, _vecAngles.Y, _vecAngles.Z);
            ////var vecNewUp = Multiply(matCameraRotation, OriginalUpDirection);
            //var matView = Matrix.LookAtLH(_vecEye, OriginalLookingDirection, OriginalUpDirection);
            //matView *= matCameraRotation;
            //device.SetTransform(TransformState.View, matView);
            var matView = Matrix.LookAtLH(_vecEye, _vecLookAt, _vecUp);
            device.SetTransform(TransformState.View, matView);
            var fovDeg = 45f;
            var matProj = PerspectiveWorkaround(MathUtil.DegreesToRadians(fovDeg), (float)clientSize.Width / clientSize.Height, 0, 4000f);
            device.SetTransform(TransformState.Projection, matProj);
            var matWorld = Matrix.Identity;
            device.SetTransform(TransformState.World, matWorld);
            var light = new SharpDX.Direct3D9.Light();
            light.Type = LightType.Point;
            light.Position = Vector3.UnitZ * 4000;
            light.Range = 6000f;
            light.Attenuation0 = 1f;
            light.Diffuse = new Color(255, 255, 255);
            light.Ambient = new Color(200, 200, 200);
            light.Specular = new Color(255, 255, 255);
            device.SetLight(0, ref light);
            device.EnableLight(0, true);
        }

        private void UpdateAndRender(Scene scene, Size2 clientSize) {
            SetCommonD3DStates(clientSize);
            var device = _device;

            if (scene.HasMeshes) {
                foreach (var mesh in scene.Meshes) {
                    var vertices = mesh.Vertices;
                    var indices = mesh.GetShortIndices();
                    DataStream ptr;
                    using (var vertexBuffer = new VertexBuffer(device, mesh.VertexCount * Utilities.SizeOf<Vector3D>(), Usage.WriteOnly, VertexFormat.None, Pool.Managed)) {
                        using (var indexBuffer = new IndexBuffer(device, indices.Length * Utilities.SizeOf<ushort>(), Usage.WriteOnly, Pool.Managed, true)) {
                            device.VertexDeclaration = _commonVertexDeclaration;
                            ptr = vertexBuffer.Lock(0, 0, LockFlags.None);
                            ptr.WriteRange(vertices.ToArray());
                            vertexBuffer.Unlock();
                            ptr.Dispose();
                            ptr = indexBuffer.Lock(0, 0, LockFlags.None);
                            ptr.WriteRange(indices);
                            indexBuffer.Unlock();
                            ptr.Dispose();
                            device.SetStreamSource(0, vertexBuffer, 0, Utilities.SizeOf<Vector3D>());
                            device.Indices = indexBuffer;

                            if (scene.HasMaterials) {
                                var material = scene.Materials[mesh.MaterialIndex];
                                var dxMaterial = new SharpDX.Direct3D9.Material();
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
                                //dxMaterial.Ambient = new Color(0.3f, 0.3f, 0.3f);
                                //dxMaterial.Diffuse = new Color(0.2f, 0.2f, 0.2f);
                                //dxMaterial.Specular = new Color(0.7f, 0.7f, 0.7f);
                                //dxMaterial.Emissive = new Color(0f, 0f, 0f);
                                //dxMaterial.Power = 0.07f;
                                device.Material = dxMaterial;
                            } else {
                                device.Material = _defaultMaterial;
                            }
                            device.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, mesh.VertexCount, 0, indices.Length / 3);
                        }
                    }
                }
            }
        }

        private static Vector4 Multiply(Matrix left, Vector4 right) {
            return new Vector4(
                (left.M11 + left.M21 + left.M31 + left.M41) * right.X,
                (left.M12 + left.M22 + left.M32 + left.M42) * right.Y,
                (left.M13 + left.M23 + left.M33 + left.M43) * right.Z,
                (left.M14 + left.M24 + left.M34 + left.M44) * right.W
                );
        }

        private static Vector3 Multiply(Matrix left, Vector3 right) {
            var v4 = new Vector4(right, 1);
            v4 = Multiply(left, v4);
            return new Vector3(v4.X, v4.Y, v4.Z);
        }

        private AssimpContext _aiContext;
        private Direct3D _direct3D;
        private DirectInput _directInput;
        private Keyboard _keyboard;
        private Device _device;
        private VertexDeclaration _commonVertexDeclaration;
        private Vector3 _vecEye = new Vector3(0, -2000, 0);
        private Vector3 _vecLookAt = new Vector3(0, 0, 0);
        private Vector3 _vecUp = new Vector3(0, 0, 1);
        // 以视点为基准的（左手系）theta（XY平面内，X到Y）、phi（射线到XY平面）、alpha（射线-up平面内）
        private Vector3 _vecAngles = new Vector3(0, 0, 0);
        private SharpDX.Direct3D9.Material _defaultMaterial;

        private static readonly Vector3 OriginalLookingDirection = Vector3.UnitY;
        private static readonly Vector3 OriginalUpDirection = Vector3.UnitZ;

    }
}
