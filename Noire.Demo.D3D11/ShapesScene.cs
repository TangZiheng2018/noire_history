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

namespace Noire.Demo.D3D11 {
    public sealed class ShapesScene : GameComponent {

        public ShapesScene() {
            _lightCount = 1;
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
                    Diffuse = new Color(1.0f, 0.2f, 0.2f, 0.2f),
                    Specular = new Color(1.0f, 0.25f, 0.25f, 0.25f),
                    Direction = new Vector3(-0.57735f, -0.57735f, 0.57735f)
                },
                new DirectionalLight {
                    Ambient = new Color(0, 0, 0),
                    Diffuse = new Color(1.0f, 0.2f, 0.2f, 0.2f),
                    Specular = new Color(0, 0, 0),
                    Direction = new Vector3(0, -0.707f, -0.707f)
                }
            };
            _gridMat = new Material {
                Ambient = new Color(0.48f, 0.77f, 0.46f),
                Diffuse = new Color(0.48f, 0.77f, 0.46f),
                Specular = new Color(16.0f, 0.2f, 0.2f, 0.2f)
            };
            _cylinderMat = new Material {
                Ambient = new Color(0.7f, 0.85f, 0.7f),
                Diffuse = new Color(0.7f, 0.85f, 0.7f),
                Specular = new Color(16.0f, 0.8f, 0.8f, 0.8f)
            };
            _sphereMat = new Material {
                Ambient = new Color(0.1f, 0.2f, 0.3f),
                Diffuse = new Color(0.2f, 0.4f, 0.6f),
                Specular = new Color(16.0f, 0.9f, 0.9f, 0.9f)
            };
            _boxMat = new Material {
                Ambient = new Color(0.651f, 0.5f, 0.392f),
                Diffuse = new Color(0.651f, 0.5f, 0.392f),
                Specular = new Color(16.0f, 0.2f, 0.2f, 0.2f)
            };
            _skullMat = new Material {
                Ambient = new Color(0.8f, 0.8f, 0.8f),
                Diffuse = new Color(0.8f, 0.8f, 0.8f),
                Specular = new Color(16.0f, 0.8f, 0.8f, 0.8f)
            };
            var device = D3DApp11.I.D3DDevice;
            BuildShapeGeometryBuffers(device);
            BuildSkullGeometryBuffers(device);
        }

        protected override void UpdateInternal(GameTime gameTime) {
            base.UpdateInternal(gameTime);
        }

        protected override void DrawInternal(GameTime gameTime) {
            base.DrawInternal(gameTime);
            var camera = D3DApp11.I.RenderTarget.Camera;
            var context = D3DApp11.I.ImmediateContext;
            var effect = EffectManager11.Instance.GetEffect<BasicEffect11>();
            var viewProj = camera.ViewProjectionMatrix;

            context.InputAssembler.InputLayout = InputLayouts.PositionNormal;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            effect.SetDirLights(_dirLights);
            effect.SetEyePosW(camera.Position);
            var activeTech = effect.Light1Tech;
            switch (_lightCount) {
                case 1:
                    activeTech = effect.Light1Tech;
                    break;
                case 2:
                    activeTech = effect.Light2Tech;
                    break;
                case 3:
                    activeTech = effect.Light3Tech;
                    break;
            }
            for (var p = 0; p < activeTech.Description.PassCount; p++) {
                var pass = activeTech.GetPassByIndex(p);
                context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_shapesVB, VertexPositionNormal.Stride, 0));
                context.InputAssembler.SetIndexBuffer(_shapesIB, Format.R32_UInt, 0);

                var world = _gridWorld;
                var worldInvTranspose = MathF.InverseTranspose(world);
                var wvp = world * viewProj;
                effect.SetWorld(world);
                effect.SetWorldInvTranspose(worldInvTranspose);
                effect.SetWorldViewProj(wvp);
                effect.SetMaterial(_gridMat);
                pass.Apply(context);
                context.DrawIndexed(_gridIndexCount, _gridIndexOffset, _gridVertexOffset);

                world = _boxWorld;
                worldInvTranspose = MathF.InverseTranspose(world);
                wvp = world * viewProj;
                effect.SetWorld(world);
                effect.SetWorldInvTranspose(worldInvTranspose);
                effect.SetWorldViewProj(wvp);
                effect.SetMaterial(_boxMat);
                pass.Apply(context);
                context.DrawIndexed(_boxIndexCount, _boxIndexOffset, _boxVertexOffset);

                foreach (var matrix in _cylWorld) {
                    world = matrix;
                    worldInvTranspose = MathF.InverseTranspose(world);
                    wvp = world * viewProj;
                    effect.SetWorld(world);
                    effect.SetWorldInvTranspose(worldInvTranspose);
                    effect.SetWorldViewProj(wvp);
                    effect.SetMaterial(_cylinderMat);
                    pass.Apply(context);
                    context.DrawIndexed(_cylinderIndexCount, _cylinderIndexOffset, _cylinderVertexOffset);
                }
                foreach (var matrix in _sphereWorld) {
                    world = matrix;
                    worldInvTranspose = MathF.InverseTranspose(world);
                    wvp = world * viewProj;
                    effect.SetWorld(world);
                    effect.SetWorldInvTranspose(worldInvTranspose);
                    effect.SetWorldViewProj(wvp);
                    effect.SetMaterial(_sphereMat);
                    pass.Apply(context);
                    context.DrawIndexed(_sphereIndexCount, _sphereIndexOffset, _sphereVertexOffset);
                }
                context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_skullVB, VertexPositionNormal.Stride, 0));
                context.InputAssembler.SetIndexBuffer(_skullIB, Format.R32_UInt, 0);

                world = _skullWorld;
                worldInvTranspose = MathF.InverseTranspose(world);
                wvp = world * viewProj;
                effect.SetWorld(world);
                effect.SetWorldInvTranspose(worldInvTranspose);
                effect.SetWorldViewProj(wvp);
                effect.SetMaterial(_skullMat);
                pass.Apply(context);
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

            var vertices = box.Vertices.Select(v => new VertexPositionNormal(v.Position, v.Normal)).ToList();
            vertices.AddRange(grid.Vertices.Select(v => new VertexPositionNormal(v.Position, v.Normal)));
            vertices.AddRange(sphere.Vertices.Select(v => new VertexPositionNormal(v.Position, v.Normal)));
            vertices.AddRange(cylinder.Vertices.Select(v => new VertexPositionNormal(v.Position, v.Normal)));

            var vbd = new BufferDescription(VertexPositionNormal.Stride * totalVertexCount, ResourceUsage.Immutable, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
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
            var vertices = new List<VertexPositionNormal>();
            var indices = new List<int>();
            var vcount = 0;
            var tcount = 0;
            using (var reader = new StreamReader("Models/skull.txt")) {
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
                            new VertexPositionNormal(
                                new Vector3(
                                    Convert.ToSingle(vals[0].Trim(), CultureInfo.InvariantCulture),
                                    Convert.ToSingle(vals[1].Trim(), CultureInfo.InvariantCulture),
                                    Convert.ToSingle(vals[2].Trim(), CultureInfo.InvariantCulture)),
                                new Vector3(
                                    Convert.ToSingle(vals[3].Trim(), CultureInfo.InvariantCulture),
                                    Convert.ToSingle(vals[4].Trim(), CultureInfo.InvariantCulture),
                                    Convert.ToSingle(vals[5].Trim(), CultureInfo.InvariantCulture))
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

            var vbd = new BufferDescription(VertexPositionNormal.Stride * vcount, ResourceUsage.Immutable,
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

    }
}
