using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Noire.Common;
using Noire.Common.Camera;
using Noire.Common.Lighting;
using Noire.Common.Vertices;
using Noire.Graphics;
using Noire.Graphics.D3D11;
using Noire.Graphics.D3D11.FX;
using Noire.Graphics.D3D11.Model;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace Noire.Demo.D3D11 {
    public sealed class ShadowScene : GameComponent {

        protected override void InitializeInternal() {
            _lightRotationAngle = 0;

            _sceneBounds = new BoundingSphere(new Vector3(), MathF.Sqrt(10 * 10 + 15 * 15));

            _skullWorld = Matrix.Scaling(0.5f, 0.5f, 0.5f) * Matrix.Translation(0, 1.0f, 0);

            _dirLights = new[] {
                new DirectionalLight {
                    Ambient = new Color(0.2f, 0.2f, 0.2f),
                    Diffuse = new Color(0.7f, 0.7f, 0.7f),
                    Specular = new Color(0.8f, 0.8f, 0.8f),
                    Direction = new Vector3(-0.57735f, -0.57735f, 0.57735f)
                },
                new DirectionalLight {
                    Ambient = new Color(0, 0, 0),
                    Diffuse = new Color(0.4f, 0.4f, 0.4f),
                    Specular = new Color(0.2f, 0.2f, 0.2f),
                    Direction = new Vector3(-0.707f, -0.707f, 0)
                },
                new DirectionalLight {
                    Ambient = new Color(0, 0, 0),
                    Diffuse = new Color(0.2f, 0.2f, 0.2f),
                    Specular = new Color(0.2f, 0.2f, 0.2f),
                    Direction = new Vector3(0, 0, -1)
                }
            };

            _originalLightDirs = _dirLights.Select(l => l.Direction).ToArray();

            _skullMat = new Material {
                Ambient = new Color(0.4f, 0.4f, 0.4f),
                Diffuse = new Color(0.8f, 0.8f, 0.8f),
                Specular = new Color(0.8f, 0.8f, 0.8f, 16.0f),
                Reflect = new Color(0.5f, 0.5f, 0.5f)
            };

            var device = D3DApp11.I.D3DDevice;
            _sMap = new ShadowMap(device, ShadowMapSize, ShadowMapSize);

            BuildShapeGeometryBuffers(device);
            BuildSkullGeometryBuffers(device);
            BuildScreenQuadGeometryBuffers(device);
        }

        protected override void UpdateInternal(GameTime gameTime) {
            _lightRotationAngle = 0.1f * (float)gameTime.TotalGameTime.TotalSeconds;
            var r = Matrix.RotationY(_lightRotationAngle);
            for (var i = 0; i < 3; i++) {
                var lightDir = _originalLightDirs[i];
                lightDir = Vector3.TransformNormal(lightDir, r);
                _dirLights[i].Direction = lightDir;
            }

            BuildShadowTransform();
        }

        protected override void DrawInternal(GameTime gameTime) {
            var device = D3DApp11.I.D3DDevice;
            var context = D3DApp11.I.ImmediateContext;
            var camera = D3DApp11.I.Camera;
            var sky = D3DApp11.I.Skybox;
            var basicFx = EffectManager11.Instance.GetEffect<BasicEffect11>();
            var normalMapFx = EffectManager11.Instance.GetEffect<NormalMapEffect11>();
            var depthStencilView = D3DApp11.I.RenderTarget.DepthStencilView;
            var renderTargetView = D3DApp11.I.RenderTarget.RenderTargetView;
            var viewport = D3DApp11.I.RenderTarget.Viewport;

            basicFx.SetShadowMap(null);
            normalMapFx.SetShadowMap(null);

            _sMap.BindDsvAndSetNullRenderTarget(context);

            DrawSceneToShadowMap(device);

            context.Rasterizer.State = null;

            context.OutputMerger.SetTargets(depthStencilView, renderTargetView);
            context.Rasterizer.SetViewports(new RawViewportF[] { viewport });

            var viewProj = camera.ViewProjectionMatrix;
            var view = camera.ViewMatrix;
            var proj = camera.ProjectionMatrix;

            basicFx.SetDirLights(_dirLights);
            basicFx.SetEyePosW(camera.Position);
            basicFx.SetCubeMap(sky.CubeMapSRV);
            basicFx.SetShadowMap(_sMap.DepthMapSRV);

            normalMapFx.SetDirLights(_dirLights);
            normalMapFx.SetEyePosW(camera.Position);
            normalMapFx.SetCubeMap(sky.CubeMapSRV);
            normalMapFx.SetShadowMap(_sMap.DepthMapSRV);

            var activeTech = normalMapFx.Light3TexTech;
            var activeSphereTech = basicFx.Light3ReflectTech;
            var activeSkullTech = basicFx.Light3ReflectTech;

            context.InputAssembler.InputLayout = InputLayouts.PosNormTexTan;

            for (var p = 0; p < activeTech.Description.PassCount; p++) {
                // draw grid
                var pass = activeTech.GetPassByIndex(p);
                _grid.ShadowTransform = _shadowTransform;
                _grid.Draw(context, pass, view, proj);
                // draw box
                _box.ShadowTransform = _shadowTransform;
                _box.Draw(context, pass, view, proj);

                // draw columns
                foreach (var cylinder in _cylinders) {
                    cylinder.ShadowTransform = _shadowTransform;
                    cylinder.Draw(context, pass, view, proj);
                }
            }
            context.HullShader.Set(null);
            context.DomainShader.Set(null);
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            for (var p = 0; p < activeSphereTech.Description.PassCount; p++) {
                var pass = activeSphereTech.GetPassByIndex(p);

                foreach (var sphere in _spheres) {
                    sphere.ShadowTransform = _shadowTransform;
                    sphere.Draw(context, pass, view, proj, RenderMode.Basic);
                }

            }
            var stride = VertPosNormTex.Stride;
            const int offset = 0;

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
                basicFx.SetShadowTransform(world * _shadowTransform);
                basicFx.SetMaterial(_skullMat);

                activeSkullTech.GetPassByIndex(p).Apply(context);
                context.DrawIndexed(_skullIndexCount, 0, 0);
            }

            // MIC, decelerator
            for (var p = 0; p < activeSkullTech.Description.PassCount; ++p) {
                _decelerator.ShadowTransform = _shadowTransform;
                _decelerator.Draw(context, activeSkullTech.GetPassByIndex(p), view, proj, RenderMode.Basic);
            }

            DrawScreenQuad(device);

            context.Rasterizer.State = null;
            context.OutputMerger.DepthStencilState = null;
            context.OutputMerger.DepthStencilReference = 0;
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

        private void DrawSceneToShadowMap(Device device) {
            var buildShadowMapFx = EffectManager11.Instance.GetEffect<BuildShadowMapEffect11>();
            var context = D3DApp11.I.ImmediateContext;
            var camera = D3DApp11.I.Camera;
            var view = _lightView;
            var proj = _lightProj;
            var viewProj = view * proj;

            buildShadowMapFx.SetEyePosW(camera.Position);
            buildShadowMapFx.SetViewProj(viewProj);

            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            var smapTech = buildShadowMapFx.BuildShadowMapTech;

            const int offset = 0;

            context.InputAssembler.InputLayout = InputLayouts.PosNormTexTan;

            for (var p = 0; p < smapTech.Description.PassCount; p++) {
                var pass = smapTech.GetPassByIndex(p);
                _grid.Draw(context, pass, view, proj, RenderMode.ShadowMap);

                _box.Draw(context, pass, view, proj, RenderMode.ShadowMap);

                // MIC
                _decelerator.Draw(context, pass, view, proj, RenderMode.ShadowMap);

                foreach (var cylinder in _cylinders) {
                    cylinder.Draw(context, pass, view, proj, RenderMode.ShadowMap);
                }
            }

            context.HullShader.Set(null);
            context.DomainShader.Set(null);
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            for (var p = 0; p < smapTech.Description.PassCount; p++) {
                var pass = smapTech.GetPassByIndex(p);
                foreach (var sphere in _spheres) {
                    sphere.Draw(context, pass, view, proj, RenderMode.ShadowMap);
                }
            }

            var stride = VertPosNormTex.Stride;
            context.Rasterizer.State = null;
            context.InputAssembler.InputLayout = InputLayouts.PosNormTex;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_skullVB, stride, offset));
            context.InputAssembler.SetIndexBuffer(_skullIB, Format.R32_UInt, 0);

            for (var p = 0; p < smapTech.Description.PassCount; p++) {
                var world = _skullWorld;
                var wit = MathF.InverseTranspose(world);
                var wvp = world * viewProj;

                buildShadowMapFx.SetWorld(world);
                buildShadowMapFx.SetWorldInvTranspose(wit);
                buildShadowMapFx.SetWorldViewProj(wvp);
                buildShadowMapFx.SetTexTransform(Matrix.Scaling(1, 2, 1));
                smapTech.GetPassByIndex(p).Apply(context);
                context.DrawIndexed(_skullIndexCount, 0, 0);
            }
        }

        private void DrawScreenQuad(Device device) {
            var context = D3DApp11.I.ImmediateContext;
            var stride = VertPosNormTex.Stride;
            var debugTextureFx = EffectManager11.Instance.GetEffect<DebugTextureEffect11>();
            const int offset = 0;

            context.InputAssembler.InputLayout = InputLayouts.PosNormTex;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_screenQuadVB, stride, offset));
            context.InputAssembler.SetIndexBuffer(_screenQuadIB, Format.R32_UInt, 0);

            var world = new Matrix {
                M11 = 0.5f,
                M22 = 0.5f,
                M33 = 1.0f,
                M41 = 0.5f,
                M42 = -0.5f,
                M44 = 1.0f
            };
            var tech = debugTextureFx.ViewRedTech;
            for (var p = 0; p < tech.Description.PassCount; p++) {
                debugTextureFx.SetWorldViewProj(world);
                debugTextureFx.SetTexture(_sMap.DepthMapSRV);
                tech.GetPassByIndex(p).Apply(context);
                context.DrawIndexed(6, 0, 0);
            }
        }

        private void BuildShadowTransform() {
            var lightDir = _dirLights[0].Direction;
            var lightPos = -2.0f * _sceneBounds.Radius * lightDir;
            var targetPos = _sceneBounds.Center;
            var up = new Vector3(0, 1, 0);

            var v = Matrix.LookAtLH(lightPos, targetPos, up);

            var sphereCenterLS = Vector3.TransformCoordinate(targetPos, v);

            var l = sphereCenterLS.X - _sceneBounds.Radius;
            var b = sphereCenterLS.Y - _sceneBounds.Radius;
            var n = sphereCenterLS.Z - _sceneBounds.Radius;
            var r = sphereCenterLS.X + _sceneBounds.Radius;
            var t = sphereCenterLS.Y + _sceneBounds.Radius;
            var f = sphereCenterLS.Z + _sceneBounds.Radius;


            //var p = Matrix.OrthoLH(r - l, t - b+5, n, f);
            var p = Matrix.OrthoOffCenterLH(l, r, b, t, n, f);
            var T = new Matrix {
                M11 = 0.5f,
                M22 = -0.5f,
                M33 = 1.0f,
                M41 = 0.5f,
                M42 = 0.5f,
                M44 = 1.0f
            };

            var s = v * p * T;
            _lightView = v;
            _lightProj = p;
            _shadowTransform = s;
        }

        private void BuildShapeGeometryBuffers(Device device) {
            var textureManager = TextureManager11.Instance;

            _boxModel = new BasicModel();
            _boxModel.CreateBox(device, 1, 1, 1);
            _boxModel.Materials[0] = new Material {
                Ambient = Color.White,
                Diffuse = Color.White,
                Specular = new Color(0.8f, 0.8f, 0.8f, 16.0f),
                Reflect = Color.Black
            };
            _boxModel.DiffuseMapSRV[0] = textureManager.CreateTexture(NoireConfiguration.GetFullResourcePath("textures/bricks.dds"));
            _boxModel.NormalMapSRV[0] = textureManager.CreateTexture(NoireConfiguration.GetFullResourcePath("textures/bricks_nmap.png"));

            _gridModel = new BasicModel();
            _gridModel.CreateGrid(device, 20, 30, 40, 60);
            _gridModel.Materials[0] = new Material {
                Ambient = new Color(0.8f, 0.8f, 0.8f),
                Diffuse = new Color(0.8f, 0.8f, 0.8f),
                Specular = new Color(0.8f, 0.8f, 0.8f, 16.0f),
                Reflect = Color.Black
            };
            _gridModel.DiffuseMapSRV[0] = textureManager.CreateTexture(NoireConfiguration.GetFullResourcePath("textures/floor.dds"));
            _gridModel.NormalMapSRV[0] = textureManager.CreateTexture(NoireConfiguration.GetFullResourcePath("textures/floor_nmap.png"));

            _sphereModel = new BasicModel();
            _sphereModel.CreateSphere(device, 0.5f, 20, 20);
            _sphereModel.Materials[0] = new Material {
                Ambient = new Color(0.6f, 0.8f, 0.9f),
                Diffuse = new Color(0.6f, 0.8f, 0.9f),
                Specular = new Color(0.9f, 0.9f, 0.9f, 16.0f),
                Reflect = new Color(0.4f, 0.4f, 0.4f)
            };
            _cylinderModel = new BasicModel();
            _cylinderModel.CreateCylinder(device, 0.5f, 0.3f, 3.0f, 20, 20);
            _cylinderModel.Materials[0] = new Material {
                Ambient = Color.White,
                Diffuse = Color.White,
                Specular = new Color(0.8f, 0.8f, 0.8f, 16.0f),
                Reflect = Color.Black
            };
            _cylinderModel.DiffuseMapSRV[0] = textureManager.CreateTexture(NoireConfiguration.GetFullResourcePath("textures/bricks.dds"));
            _cylinderModel.NormalMapSRV[0] = textureManager.CreateTexture(NoireConfiguration.GetFullResourcePath("textures/bricks_nmap.png"));

            for (var i = 0; i < 5; i++) {
                _cylinders[i * 2] = new BasicModelInstance(_cylinderModel) {
                    World = Matrix.Translation(-5.0f, 1.5f, -10.0f + i * 5.0f),
                    TexTransform = Matrix.Scaling(1, 2, 1)
                };
                _cylinders[i * 2 + 1] = new BasicModelInstance(_cylinderModel) {
                    World = Matrix.Translation(5.0f, 1.5f, -10.0f + i * 5.0f),
                    TexTransform = Matrix.Scaling(1, 2, 1)
                };

                _spheres[i * 2] = new BasicModelInstance(_sphereModel) {
                    World = Matrix.Translation(-5.0f, 3.5f, -10.0f + i * 5.0f)
                };
                _spheres[i * 2 + 1] = new BasicModelInstance(_sphereModel) {
                    World = Matrix.Translation(5.0f, 3.5f, -10.0f + i * 5.0f)
                };
            }

            _grid = new BasicModelInstance(_gridModel) {
                TexTransform = Matrix.Scaling(8, 10, 1),
                World = Matrix.Identity
            };

            _box = new BasicModelInstance(_boxModel) {
                TexTransform = Matrix.Scaling(2, 1, 1),
                World = Matrix.Scaling(3.0f, 1.0f, 3.0f) * Matrix.Translation(0, 0.5f, 0)
            };

            // MIC
            _deceleratorModel = BasicModel.Create(device, TextureManager11.Instance, NoireConfiguration.GetFullResourcePath("models/decelerator.3ds"), NoireConfiguration.GetFullResourcePath("models"));
            _deceleratorWorld = Matrix.Scaling(8) * Matrix.Translation(0, 0, 5);
            _decelerator = new BasicModelInstance(_deceleratorModel) {
                TexTransform = Matrix.Identity,
                World = _deceleratorWorld
            };
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
                for (var i = 0; i < vcount; i++) {
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

        private void BuildScreenQuadGeometryBuffers(Device device) {
            var quad = GeometryGenerator.CreateFullScreenQuad();

            var verts = quad.Vertices.Select(v => new VertPosNormTex(v.Position, v.Normal, v.TexCoords)).ToList();
            var vbd = new BufferDescription(VertPosNormTex.Stride * verts.Count, ResourceUsage.Immutable, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            _screenQuadVB = new Buffer(device, DataStream.Create(verts.ToArray(), false, false), vbd);

            var ibd = new BufferDescription(sizeof(int) * quad.Indices.Count, ResourceUsage.Immutable, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            _screenQuadIB = new Buffer(device, DataStream.Create(quad.Indices.ToArray(), false, false), ibd);
        }

        private Buffer _skullVB;
        private Buffer _skullIB;
        private Buffer _screenQuadVB;
        private Buffer _screenQuadIB;

        private BoundingSphere _sceneBounds;

        private static readonly int ShadowMapSize = 2048;
        private ShadowMap _sMap;
        private Matrix _lightView;
        private Matrix _lightProj;
        private Matrix _shadowTransform;

        private float _lightRotationAngle;
        private Vector3[] _originalLightDirs;
        private DirectionalLight[] _dirLights;

        private Material _skullMat;
        private Matrix _skullWorld;
        private int _skullIndexCount;

        private BasicModel _boxModel;
        private BasicModel _gridModel;
        private BasicModel _sphereModel;
        private BasicModel _cylinderModel;

        private BasicModelInstance _grid;
        private BasicModelInstance _box;
        private readonly BasicModelInstance[] _spheres = new BasicModelInstance[10];
        private readonly BasicModelInstance[] _cylinders = new BasicModelInstance[10];

        private Matrix _deceleratorWorld;
        private BasicModel _deceleratorModel;
        private BasicModelInstance _decelerator;

    }
}
