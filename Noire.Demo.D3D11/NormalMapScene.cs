using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Common;
using Noire.Common.Vertices;
using Noire.Graphics.D3D11;
using Noire.Graphics.D3D11.FX;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;
using System.IO;
using Noire.Common.Lighting;

namespace Noire.Demo.D3D11 {
    public sealed class NormalMapScene : GameComponent {

        public NormalMapScene() {
            _lightCount = 1;
        }

        public int LightCount {
            get { return _lightCount; }
            set { _lightCount = value; }
        }

        protected override void InitializeInternal() {
            base.InitializeInternal();

            _gridWorld = Matrix.Identity;

            _boxWorld = Matrix.Scaling(3.0f, 1.0f, 3.0f) * Matrix.Translation(0, 0.5f, 0);
            _skullWorld = Matrix.Scaling(0.5f, 0.5f, 0.5f) * Matrix.Translation(0, 1.0f, 0);

            for (var i = 0; i < 5; i++) {
                _cylWorld[i * 2] = Matrix.Translation(-5.0f, 1.5f, -10.0f + i * 5.0f);
                _cylWorld[i * 2 + 1] = Matrix.Translation(5.0f, 1.5f, -10.0f + i * 5.0f);

                _sphereWorld[i * 2] = Matrix.Translation(-5.0f, 3.5f, -10.0f + i * 5.0f);
                _sphereWorld[i * 2 + 1] = Matrix.Translation(5.0f, 3.5f, -10.0f + i * 5.0f);
            }
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
            _gridMat = new Material {
                Ambient = new Color(0.8f, 0.8f, 0.8f),
                Diffuse = new Color(0.8f, 0.8f, 0.8f),
                Specular = new Color(0.8f, 0.8f, 0.8f, 16.0f),
                Reflect = Color.Black
            };
            _cylinderMat = new Material {
                Ambient = Color.White,
                Diffuse = Color.White,
                Specular = new Color(0.8f, 0.8f, 0.8f, 16.0f),
                Reflect = Color.Black
            };
            _sphereMat = new Material {
                Ambient = new Color(0.6f, 0.8f, 0.9f),
                Diffuse = new Color(0.6f, 0.8f, 0.9f),
                Specular = new Color(0.9f, 0.9f, 0.9f, 16.0f),
                Reflect = new Color(0.4f, 0.4f, 0.4f)
            };
            _boxMat = new Material {
                Ambient = Color.White,
                Diffuse = Color.White,
                Specular = new Color(0.8f, 0.8f, 0.8f, 16.0f),
                Reflect = Color.Black
            };
            _skullMat = new Material {
                Ambient = new Color(0.4f, 0.4f, 0.4f),
                Diffuse = new Color(0.8f, 0.8f, 0.8f),
                Specular = new Color(0.8f, 0.8f, 0.8f, 16.0f),
                Reflect = new Color(0.5f, 0.5f, 0.5f)
            };

            var device = D3DApp11.I.D3DDevice;

            _stoneTexSRV = TextureLoader.BitmapFromFile(device, NoireConfiguration.GetFullResourcePath("textures/floor.dds")).AsShaderResourceView();
            _brickTexSRV = TextureLoader.BitmapFromFile(device, NoireConfiguration.GetFullResourcePath("Textures/bricks.dds")).AsShaderResourceView();
            _stoneNormalTexSRV = TextureLoader.BitmapFromFile(device, NoireConfiguration.GetFullResourcePath("textures/floor_nmap.png")).AsShaderResourceView();
            _brickNormalTexSRV = TextureLoader.BitmapFromFile(device, NoireConfiguration.GetFullResourcePath("textures/bricks_nmap.png")).AsShaderResourceView();

            BuildShapeGeometryBuffers(device);
            BuildSkullGeometryBuffers(device);
        }

        protected override void DrawInternal(GameTime gameTime) {
            base.DrawInternal(gameTime);
            var context = D3DApp11.I.ImmediateContext;
            var camera = D3DApp11.I.Camera;
            var skybox = D3DApp11.I.Skybox;
            var basicFx = EffectManager11.Instance.GetEffect<BasicEffect11>();
            var normalMapFx = EffectManager11.Instance.GetEffect<NormalMapEffect11>();
            var displacementMapFx = EffectManager11.Instance.GetEffect<DisplacementMapEffect11>();
            var viewProj = camera.ViewProjectionMatrix;

            basicFx.SetDirLights(_dirLights);
            basicFx.SetEyePosW(camera.Position);
            basicFx.SetCubeMap(skybox.CubeMapSRV);

            normalMapFx.SetDirLights(_dirLights);
            normalMapFx.SetEyePosW(camera.Position);
            normalMapFx.SetCubeMap(skybox.CubeMapSRV);

            displacementMapFx.SetDirLights(_dirLights);
            displacementMapFx.SetEyePosW(camera.Position);
            displacementMapFx.SetCubeMap(skybox.CubeMapSRV);

            displacementMapFx.SetHeightScale(0.05f);
            displacementMapFx.SetMaxTessDistance(1.0f);
            displacementMapFx.SetMinTessDistance(25.0f);
            displacementMapFx.SetMinTessFactor(1.0f);
            displacementMapFx.SetMaxTessFactor(5.0f);

            var activeTech = displacementMapFx.Light3Tech;
            var activeSphereTech = basicFx.Light3ReflectTech;
            var activeSkullTech = basicFx.Light3ReflectTech;

            switch (_renderOptions) {
                case RenderOptions.Basic:
                    activeTech = basicFx.Light3TexTech;
                    context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                    break;
                case RenderOptions.NormalMap:
                    activeTech = normalMapFx.Light3TexTech;
                    context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                    break;
                case RenderOptions.DisplacementMap:
                    activeTech = displacementMapFx.Light3TexTech;
                    context.InputAssembler.PrimitiveTopology = PrimitiveTopology.PatchListWith3ControlPoints;
                    break;
            }
            var stride = VertPosNormTexTan.Stride;
            var offset = 0;

            context.InputAssembler.InputLayout = InputLayouts.PosNormTexTan;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_shapesVB, stride, offset));
            context.InputAssembler.SetIndexBuffer(_shapesIB, Format.R32_UInt, 0);

            for (var p = 0; p < activeTech.Description.PassCount; p++) {
                var world = _gridWorld;
                var wit = MathF.InverseTranspose(world);
                var wvp = world * viewProj;

                switch (_renderOptions) {
                    case RenderOptions.Basic:
                        basicFx.SetWorld(world);
                        basicFx.SetWorldInvTranspose(wit);
                        basicFx.SetWorldViewProj(wvp);
                        basicFx.SetTexTransform(Matrix.Scaling(8, 10, 1));
                        basicFx.SetMaterial(_gridMat);
                        basicFx.SetDiffuseMap(_stoneTexSRV);
                        break;
                    case RenderOptions.NormalMap:
                        normalMapFx.SetWorld(world);
                        normalMapFx.SetWorldInvTranspose(wit);
                        normalMapFx.SetWorldViewProj(wvp);
                        normalMapFx.SetTexTransform(Matrix.Scaling(8, 10, 1));
                        normalMapFx.SetMaterial(_gridMat);
                        normalMapFx.SetDiffuseMap(_stoneTexSRV);
                        normalMapFx.SetNormalMap(_stoneNormalTexSRV);
                        break;
                    case RenderOptions.DisplacementMap:
                        displacementMapFx.SetWorld(world);
                        displacementMapFx.SetWorldInvTranspose(wit);
                        displacementMapFx.SetViewProj(viewProj);
                        displacementMapFx.SetWorldViewProj(wvp);
                        displacementMapFx.SetTexTransform(Matrix.Scaling(8, 10, 1));
                        displacementMapFx.SetMaterial(_gridMat);
                        displacementMapFx.SetDiffuseMap(_stoneTexSRV);
                        displacementMapFx.SetNormalMap(_stoneNormalTexSRV);
                        break;
                }
                var pass = activeTech.GetPassByIndex(p);
                pass.Apply(context);
                context.DrawIndexed(_gridIndexCount, _gridIndexOffset, _gridVertexOffset);

                world = _boxWorld;
                wit = MathF.InverseTranspose(world);
                wvp = world * viewProj;

                switch (_renderOptions) {
                    case RenderOptions.Basic:
                        basicFx.SetWorld(world);
                        basicFx.SetWorldInvTranspose(wit);
                        basicFx.SetWorldViewProj(wvp);
                        basicFx.SetTexTransform(Matrix.Scaling(2, 1, 1));
                        basicFx.SetMaterial(_boxMat);
                        basicFx.SetDiffuseMap(_brickTexSRV);
                        break;
                    case RenderOptions.NormalMap:
                        normalMapFx.SetWorld(world);
                        normalMapFx.SetWorldInvTranspose(wit);
                        normalMapFx.SetWorldViewProj(wvp);
                        normalMapFx.SetTexTransform(Matrix.Scaling(2, 1, 1));
                        normalMapFx.SetMaterial(_boxMat);
                        normalMapFx.SetDiffuseMap(_brickTexSRV);
                        normalMapFx.SetNormalMap(_brickNormalTexSRV);
                        break;
                    case RenderOptions.DisplacementMap:
                        displacementMapFx.SetWorld(world);
                        displacementMapFx.SetWorldInvTranspose(wit);
                        displacementMapFx.SetViewProj(viewProj);
                        displacementMapFx.SetWorldViewProj(wvp);
                        displacementMapFx.SetTexTransform(Matrix.Scaling(2, 1, 1));
                        displacementMapFx.SetMaterial(_boxMat);
                        displacementMapFx.SetDiffuseMap(_brickTexSRV);
                        displacementMapFx.SetNormalMap(_brickNormalTexSRV);
                        break;
                }
                pass.Apply(context);
                context.DrawIndexed(_boxIndexCount, _boxIndexOffset, _boxVertexOffset);

                foreach (var matrix in _cylWorld) {
                    world = matrix;
                    wit = MathF.InverseTranspose(world);
                    wvp = world * viewProj;

                    switch (_renderOptions) {
                        case RenderOptions.Basic:
                            basicFx.SetWorld(world);
                            basicFx.SetWorldInvTranspose(wit);
                            basicFx.SetWorldViewProj(wvp);
                            basicFx.SetTexTransform(Matrix.Scaling(1, 2, 1));
                            basicFx.SetMaterial(_cylinderMat);
                            basicFx.SetDiffuseMap(_brickTexSRV);
                            break;
                        case RenderOptions.NormalMap:
                            normalMapFx.SetWorld(world);
                            normalMapFx.SetWorldInvTranspose(wit);
                            normalMapFx.SetWorldViewProj(wvp);
                            normalMapFx.SetTexTransform(Matrix.Scaling(1, 2, 1));
                            normalMapFx.SetMaterial(_cylinderMat);
                            normalMapFx.SetDiffuseMap(_brickTexSRV);
                            normalMapFx.SetNormalMap(_brickNormalTexSRV);
                            break;
                        case RenderOptions.DisplacementMap:
                            displacementMapFx.SetWorld(world);
                            displacementMapFx.SetWorldInvTranspose(wit);
                            displacementMapFx.SetViewProj(viewProj);
                            displacementMapFx.SetWorldViewProj(wvp);
                            displacementMapFx.SetTexTransform(Matrix.Scaling(1, 2, 1));
                            displacementMapFx.SetMaterial(_cylinderMat);
                            displacementMapFx.SetDiffuseMap(_brickTexSRV);
                            displacementMapFx.SetNormalMap(_brickNormalTexSRV);
                            break;
                    }
                    pass.Apply(context);
                    context.DrawIndexed(_cylinderIndexCount, _cylinderIndexOffset, _cylinderVertexOffset);
                }

            }
            context.HullShader.Set(null);
            context.DomainShader.Set(null);
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            for (var p = 0; p < activeSphereTech.Description.PassCount; p++) {
                foreach (var matrix in _sphereWorld) {
                    var world = matrix;
                    var wit = MathF.InverseTranspose(world);
                    var wvp = world * viewProj;

                    basicFx.SetWorld(world);
                    basicFx.SetWorldInvTranspose(wit);
                    basicFx.SetWorldViewProj(wvp);
                    basicFx.SetTexTransform(Matrix.Identity);
                    basicFx.SetMaterial(_sphereMat);

                    activeSphereTech.GetPassByIndex(p).Apply(context);
                    context.DrawIndexed(_sphereIndexCount, _sphereIndexOffset, _sphereVertexOffset);
                }
            }
            stride = VertPosNormTex.Stride;
            offset = 0;

            context.Rasterizer.State = null;

            context.InputAssembler.InputLayout = InputLayouts.PosNormTex;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_skullVB, stride, offset));
            context.InputAssembler.SetIndexBuffer(_skullIB, Format.R32_UInt, 0);

            for (var p = 0; p < activeSkullTech.Description.PassCount; p++) {
                var world = _skullWorld;
                var wit = MathF.InverseTranspose(world);
                var wvp = world * viewProj;

                basicFx.SetWorld(world);
                basicFx.SetWorldInvTranspose(wit);
                basicFx.SetWorldViewProj(wvp);
                basicFx.SetMaterial(_skullMat);

                activeSkullTech.GetPassByIndex(p).Apply(context);
                context.DrawIndexed(_skullIndexCount, 0, 0);
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

        private void BuildShapeGeometryBuffers(Device device) {
            var box = GeometryGenerator.CreateBox(1, 1, 1);
            var grid = GeometryGenerator.CreateGrid(20, 30, 60, 40);
            var sphere = GeometryGenerator.CreateSphere(0.5f, 20, 20);
            var cylinder = GeometryGenerator.CreateCylinder(0.5f, 0.3f, 3.0f, 20, 20);

            _boxVertexOffset = 0;
            _gridVertexOffset = box.Vertices.Count;
            _sphereVertexOffset = _gridVertexOffset + grid.Vertices.Count;
            _cylinderVertexOffset = _sphereVertexOffset + sphere.Vertices.Count;

            _boxIndexCount = box.Indices.Count;
            _gridIndexCount = grid.Indices.Count;
            _sphereIndexCount = sphere.Indices.Count;
            _cylinderIndexCount = cylinder.Indices.Count;

            _boxIndexOffset = 0;
            _gridIndexOffset = _boxIndexCount;
            _sphereIndexOffset = _gridIndexOffset + _gridIndexCount;
            _cylinderIndexOffset = _sphereIndexOffset + _sphereIndexCount;

            var totalVertexCount = box.Vertices.Count + grid.Vertices.Count + sphere.Vertices.Count + cylinder.Vertices.Count;
            var totalIndexCount = _boxIndexCount + _gridIndexCount + _sphereIndexCount + _cylinderIndexCount;

            var vertices = box.Vertices.Select(v => new VertPosNormTexTan(v.Position, v.Normal, v.TexCoords, v.TangentU)).ToList();
            vertices.AddRange(grid.Vertices.Select(v => new VertPosNormTexTan(v.Position, v.Normal, v.TexCoords, v.TangentU)));
            vertices.AddRange(sphere.Vertices.Select(v => new VertPosNormTexTan(v.Position, v.Normal, v.TexCoords, v.TangentU)));
            vertices.AddRange(cylinder.Vertices.Select(v => new VertPosNormTexTan(v.Position, v.Normal, v.TexCoords, v.TangentU)));

            var vbd = new BufferDescription(VertPosNormTexTan.Stride * totalVertexCount, ResourceUsage.Immutable, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            _shapesVB = new Buffer(device, DataStream.Create(vertices.ToArray(), false, false), vbd);

            var indices = new List<int>();
            indices.AddRange(box.Indices);
            indices.AddRange(grid.Indices);
            indices.AddRange(sphere.Indices);
            indices.AddRange(cylinder.Indices);

            var ibd = new BufferDescription(sizeof(int) * totalIndexCount, ResourceUsage.Immutable, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            _shapesIB = new Buffer(device, DataStream.Create(indices.ToArray(), false, false), ibd);
        }

        private void BuildSkullGeometryBuffers(Device device) {
            var vertices = new List<VertPosNormTex>();
            var indices = new List<int>();
            var vcount = 0;
            var tcount = 0;
            using (var reader = new StreamReader(NoireConfiguration.GetFullResourcePath("models/skull.txt"))) {


                var input = reader.ReadLine();
                if (input != null)
                    // VertexCount: X
                    vcount = Convert.ToInt32(input.Split(new[] { ':' })[1].Trim());

                input = reader.ReadLine();
                if (input != null)
                    //TriangleCount: X
                    tcount = Convert.ToInt32(input.Split(new[] { ':' })[1].Trim());

                // skip ahead to the vertex data
                do {
                    input = reader.ReadLine();
                } while (input != null && !input.StartsWith("{"));
                // Get the vertices  
                for (int i = 0; i < vcount; i++) {
                    input = reader.ReadLine();
                    if (input != null) {
                        var vals = input.Split(new[] { ' ' });
                        vertices.Add(
                            new VertPosNormTex(
                                new Vector3(
                                    Convert.ToSingle(vals[0].Trim(), CultureInfo.InvariantCulture),
                                    Convert.ToSingle(vals[1].Trim(), CultureInfo.InvariantCulture),
                                    Convert.ToSingle(vals[2].Trim(), CultureInfo.InvariantCulture)),
                                new Vector3(
                                    Convert.ToSingle(vals[3].Trim(), CultureInfo.InvariantCulture),
                                    Convert.ToSingle(vals[4].Trim(), CultureInfo.InvariantCulture),
                                    Convert.ToSingle(vals[5].Trim(), CultureInfo.InvariantCulture)),
                                    new Vector2()
                            )
                        );
                    }
                }
                // skip ahead to the index data
                do {
                    input = reader.ReadLine();
                } while (input != null && !input.StartsWith("{"));
                // Get the indices
                _skullIndexCount = 3 * tcount;
                for (var i = 0; i < tcount; i++) {
                    input = reader.ReadLine();
                    if (input == null) {
                        break;
                    }
                    var m = input.Trim().Split(new[] { ' ' });
                    indices.Add(Convert.ToInt32(m[0].Trim()));
                    indices.Add(Convert.ToInt32(m[1].Trim()));
                    indices.Add(Convert.ToInt32(m[2].Trim()));
                }
            }

            var vbd = new BufferDescription(VertPosNormTex.Stride * vcount, ResourceUsage.Immutable,
                BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            _skullVB = new Buffer(device, DataStream.Create(vertices.ToArray(), false, false), vbd);

            var ibd = new BufferDescription(sizeof(int) * _skullIndexCount, ResourceUsage.Immutable,
                BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            _skullIB = new Buffer(device, DataStream.Create(indices.ToArray(), false, false), ibd);
        }

        private Buffer _shapesVB;
        private Buffer _shapesIB;

        private Buffer _skullVB;
        private Buffer _skullIB;

        private ShaderResourceView _stoneTexSRV;
        private ShaderResourceView _brickTexSRV;

        private ShaderResourceView _stoneNormalTexSRV;
        private ShaderResourceView _brickNormalTexSRV;

        private DirectionalLight[] _dirLights;
        private Material _gridMat;
        private Material _boxMat;
        private Material _cylinderMat;
        private Material _sphereMat;
        private Material _skullMat;

        private Matrix[] _sphereWorld = new Matrix[10];
        private Matrix[] _cylWorld = new Matrix[10];
        private Matrix _boxWorld;
        private Matrix _gridWorld;
        private Matrix _skullWorld;

        private int _boxVertexOffset;
        private int _gridVertexOffset;
        private int _sphereVertexOffset;
        private int _cylinderVertexOffset;

        private int _boxIndexOffset;
        private int _gridIndexOffset;
        private int _sphereIndexOffset;
        private int _cylinderIndexOffset;

        private int _boxIndexCount;
        private int _gridIndexCount;
        private int _sphereIndexCount;
        private int _cylinderIndexCount;
        private int _skullIndexCount;

        private int _lightCount;

        private RenderOptions _renderOptions = RenderOptions.DisplacementMap;

        private enum RenderOptions {
            Basic,
            NormalMap,
            DisplacementMap
        }

    }
}
