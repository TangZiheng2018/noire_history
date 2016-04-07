using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using Noire.Common;
using Noire.Common.Lighting;
using Noire.Common.Vertices;
using Noire.Graphics.D3D11;
using Noire.Graphics.D3D11.FX;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Material = Noire.Common.Material;

namespace Noire.Demo.D3D11 {

    public sealed class AssimpModel : GameComponent {

        public AssimpModel(string filename) {
            _filename = filename;
            _worldMatrix = Matrix.Identity;
            _scale = Vector3.One;
            _lightCount = 1;
        }

        public Vector3 Translation {
            get {
                return _translation;
            }
            set {
                _translation = value;
                UpdateWorldMatrix();
            }
        }

        public float RotationX {
            get {
                return _rotationX;
            }
            set {
                _rotationX = value;
                UpdateWorldMatrix();
            }
        }

        public float RotationY {
            get {
                return _rotationY;
            }
            set {
                _rotationY = value;
                UpdateWorldMatrix();
            }
        }

        public float RotationZ {
            get {
                return _rotationZ;
            }
            set {
                _rotationZ = value;
                UpdateWorldMatrix();
            }
        }

        public Vector3 Scale {
            get {
                return _scale;
            }
            set {
                _scale = value;
                UpdateWorldMatrix();
            }
        }

        public int LightCount {
            get { return _lightCount; }
            set { _lightCount = value; }
        }

        protected override void InitializeInternal() {
            base.InitializeInternal();

            _dirLights = new[] {
                new DirectionalLight {
                    Ambient = new Color(0.2f, 0.2f, 0.2f),
                    Diffuse = new Color(0.5f, 0.5f, 0.5f),
                    Specular = new Color(0.5f, 0.5f, 0.5f),
                    Direction = new Vector3(0.57735f, -0.57735f, 0.57735f)
                },
                new DirectionalLight {
                    Ambient = new Color(0, 0, 0),
                    Diffuse = new Color(0.2f, 0.2f, 0.2f),
                    Specular = new Color(0.25f, 0.25f, 0.25f),
                    Direction = new Vector3(-0.57735f, -0.57735f, 0.57735f)
                },
                new DirectionalLight {
                    Ambient = new Color(0, 0, 0),
                    Diffuse = new Color(0.2f, 0.2f, 0.2f),
                    Specular = new Color(0, 0, 0),
                    Direction = new Vector3(0, -0.707f, -0.707f)
                }
            };
            _modelMaterial = new Material() {
                Ambient = new Color(0.4f, 0.4f, 0.4f),
                Diffuse = new Color(0.8f, 0.8f, 0.8f),
                Specular = new Color(0.8f, 0.8f, 0.8f, 1.0f),
                Reflect = new Color(0.1f, 0.1f, 0.1f)
            };

            _assimpContext = new AssimpContext();
            _scene = _assimpContext.ImportFile(_filename);
            var scene = _scene;
            var vertices = new List<VertexPositionNormalTC>();
            var indices = new List<int>();
            var c = 0;
            foreach (var mesh in scene.Meshes) {
                foreach (var face in mesh.Faces) {
                    var faceIndices = face.Indices;
                    for (var i = 0; i < 3; ++i) {
                        vertices.Add(new VertexPositionNormalTC() {
                            Position = mesh.Vertices[faceIndices[i]].ToVector3(),
                            Normal = mesh.Normals[faceIndices[i]].ToVector3(),
                            TextureCoords = Vector2.Zero
                        });
                        indices.Add(c);
                        ++c;
                    }
                }
            }
            _indexCount = indices.Count;

            var device = D3DApp11.I.D3DDevice;
            var vbd = new BufferDescription(VertexPositionNormalTC.Stride * vertices.Count, ResourceUsage.Immutable,
               BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            _vb = new Buffer(device, DataStream.Create(vertices.ToArray(), false, false), vbd);
            var ibd = new BufferDescription(sizeof(int) * indices.Count, ResourceUsage.Immutable,
                BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            _ib = new Buffer(device, DataStream.Create(indices.ToArray(), false, false), ibd);
        }

        protected override void UpdateInternal(GameTime gameTime) {
            base.UpdateInternal(gameTime);
        }

        protected override void DrawInternal(GameTime gameTime) {
            base.DrawInternal(gameTime);

            var context = D3DApp11.I.ImmediateContext;
            var camera = D3DApp11.I.RenderTarget.Camera;
            var skybox = D3DApp11.I.Skybox;
            var basicFx = EffectManager11.Instance.GetEffect<BasicEffect11>();

            context.InputAssembler.InputLayout = InputLayouts.PositionNormalTC;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            Matrix view = camera.ViewMatrix;
            Matrix proj = camera.ProjectionMatrix;
            Matrix viewProj = camera.ViewProjectionMatrix;
            basicFx.SetEyePosW(camera.Position);
            basicFx.SetDirLights(_dirLights);
            basicFx.SetCubeMap(skybox.CubeMapSRV);

            EffectTechnique activeTexTech;
            EffectTechnique activeSkullTech;
            EffectTechnique activeReflectTech;
            switch (_lightCount) {
                case 1:
                    activeTexTech = basicFx.Light1TexTech;
                    activeSkullTech = basicFx.Light1ReflectTech;
                    activeReflectTech = basicFx.Light1TexReflectTech;
                    break;
                case 2:
                    activeTexTech = basicFx.Light2TexTech;
                    activeSkullTech = basicFx.Light2ReflectTech;
                    activeReflectTech = basicFx.Light2TexReflectTech;
                    break;
                case 3:
                    activeTexTech = basicFx.Light3TexTech;
                    activeSkullTech = basicFx.Light3ReflectTech;
                    activeReflectTech = basicFx.Light3TexReflectTech;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var world = _worldMatrix;
            var worldInvTranspose = MathF.InverseTranspose(world);
            var wvp = world * viewProj;
            var passCount = activeSkullTech.Description.PassCount;
            for (var p = 0; p < passCount; p++) {
                using (var pass = activeSkullTech.GetPassByIndex(p)) {
                    context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vb, VertexPositionNormalTC.Stride, 0));
                    context.InputAssembler.SetIndexBuffer(_ib, Format.R32_UInt, 0);
                    basicFx.SetWorld(world);
                    basicFx.SetWorldInvTranspose(worldInvTranspose);
                    basicFx.SetWorldViewProj(wvp);
                    basicFx.SetMaterial(_modelMaterial);
                    pass.Apply(context);
                    context.DrawIndexed(_indexCount, 0, 0);
                }
            }
        }

        protected override void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            if (disposing) {
                NoireUtilities.DisposeNonPublicDeclaredFields(this);
            }
            base.Dispose(disposing);
        }

        private void UpdateWorldMatrix() {
            _worldMatrix = Matrix.Translation(_translation) * Matrix.RotationX(_rotationX) * Matrix.RotationY(_rotationY) * Matrix.RotationZ(_rotationZ) * Matrix.Scaling(_scale);
        }

        private Buffer _vb;
        private Buffer _ib;
        private int _indexCount;
        private Material _modelMaterial;

        private readonly string _filename;
        private int _lightCount;
        private Matrix _worldMatrix;
        private Vector3 _translation;
        private float _rotationX;
        private float _rotationY;
        private float _rotationZ;
        private Vector3 _scale;
        private AssimpContext _assimpContext;
        private Scene _scene;

        private DirectionalLight[] _dirLights;

    }

}
