using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using Noire.Common;
using Noire.Common.Lighting;
using Noire.Common.Vertices;
using Noire.Demo.D3D11.DemoFinal;
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
    public sealed class ShadowScene : GameComponentContainer {

        public ShadowScene(IGameComponentRoot root, IGameComponentContainer parent)
            : base(root, parent) {
            _bufferedSettings = new ShadowSceneSettings();
            _techniqueTable = new Dictionary<Tuple<bool, bool, SurfaceMapping, NumberOfLights>, Tuple<EffectTechnique, EffectTechnique, EffectTechnique>>();
            SetNumberOfLights(NumberOfLights.One);
            UpdateBufferedSettings();
        }

        // Buffer Op!
        public void SetDrawMode(DrawMode drawMode) {
            _bufferedSettings.DrawMode = drawMode;
        }

        public void SetQuadVisible(bool visible) {
            _bufferedSettings.QuadVisible = visible;
        }

        public void SetParticleFlameVisible(bool visible) {
            _bufferedSettings.ParticleFlameVisible = visible;
        }

        public void SetParticleRainVisible(bool visible) {
            _bufferedSettings.ParticleRainVisible = visible;
        }

        public void SetLightsMoving(bool moving) {
            _bufferedSettings.AreLightsMoving = moving;
        }

        public void SetNumberOfLights(NumberOfLights number) {
            _bufferedSettings.NumberOfLights = number;
        }

        public void SetDeceleratorVisible(bool visible) {
            _bufferedSettings.IsDeceleratorVisible = visible;
        }

        public void SetBarbecueBarVisible(bool visible) {
            _bufferedSettings.IsBarbecueBarVisible = visible;
        }

        public void SetShadowEnabled(bool enabled) {
            _bufferedSettings.IsShadowEnabled = enabled;
        }

        public void SetReflectionEnabled(bool enabled) {
            _bufferedSettings.IsReflectionEnabled = enabled;
        }

        public void SetSurfaceMapping(SurfaceMapping surfaceMapping) {
            _bufferedSettings.SurfaceMapping = surfaceMapping;
        }

        public void SetTruckVisible(bool visible) {
            _bufferedSettings.IsTruckVisible = visible;
        }

        public void SetTireVisible(bool visible) {
            _bufferedSettings.IsTireVisible = visible;
        }

        public void SetMaterialType(MaterialType materialType) {
            _bufferedSettings.MaterialType = materialType;
        }

        public void SetSkyboxType(SkyboxType skyboxType) {
            _bufferedSettings.SkyboxType = skyboxType;
        }

        public void ReplaceDeceleratorSurfaceMaterial() {
            if (IsInitialized) {
                for (var i = 0; i < _decelModel.Materials.Count; ++i) {
                    var mat = _decelModel.Materials[i];
                    mat.Reflect = new Color(0.5f, 0.5f, 0.5f, 0.75f);
                    _decelModel.Materials[i] = mat;
                }
            }
        }

        protected override void InitializeInternal() {
            base.InitializeInternal();

            _lightRotationAngle = 0;

            _sceneBounds = new BoundingSphere(new Vector3(), MathF.Sqrt(40 * 40 + 60 * 60));

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

            var device = D3DApp11.I.D3DDevice;
            _shadowMap = new ShadowMap(device, ShadowMapSize, ShadowMapSize);

            BuildShapeGeometryBuffers(device);
            BuildScreenQuadGeometryBuffers(device);

            var context = D3DApp11.I.ImmediateContext;

            _randomTex = TextureLoader.CreateRandomTexture1D(device);

            _flareTexSRV = TextureLoader.CreateTexture2DArray(device, context, new[] { NoireConfiguration.GetFullResourcePath("textures/flare0.png") });
            _fire = new ParticleSource(RootContainer, this, device, EffectManager11.Instance.GetEffect<FireParticleEffect11>(), _flareTexSRV, _randomTex, 500);
            _fire.Initialize();
            _fire.Visible = _settings.ParticleFlameVisible;
            ChildComponents.Add(_fire);

            _rainTexSRV = TextureLoader.CreateTexture2DArray(device, context, new[] { NoireConfiguration.GetFullResourcePath("textures/raindrop.png") });
            _rain = new ParticleSource(RootContainer, this, device, EffectManager11.Instance.GetEffect<RainParticleEffect11>(), _rainTexSRV, _randomTex, 10000);
            _rain.Initialize();
            _rain.Visible = _settings.ParticleRainVisible;
            ChildComponents.Add(_rain);

            var basicFx = EffectManager11.Instance.GetEffect<BasicEffect11>();
            var normalMapFx = EffectManager11.Instance.GetEffect<NormalMapEffect11>();
            var displacementMapFx = EffectManager11.Instance.GetEffect<DisplacementMapEffect11>();
            InitializeTechniqueTable(basicFx, normalMapFx, displacementMapFx);
        }

        protected override void UpdateInternal(GameTime gameTime) {
            UpdateBufferedSettings();

            base.UpdateInternal(gameTime);

            if (_settings.AreLightsMoving) {
                _lightRotationAngle = 0.1f * (float)gameTime.TotalGameTime.TotalSeconds;
                var r = Matrix.RotationY(_lightRotationAngle);
                for (var i = 0; i < _dirLights.Length; i++) {
                    var lightDir = _originalLightDirs[i];
                    lightDir = Vector3.TransformNormal(lightDir, r);
                    _dirLights[i].Direction = lightDir;
                }
            }

            var camera = D3DApp11.I.Camera;
            _fire.EyePosW = camera.Position;
            _rain.EyePosW = camera.Position;
            _rain.EmitPosW = camera.Position;

            BuildShadowTransform();
        }

        protected override void DrawInternal(GameTime gameTime) {
            var device = D3DApp11.I.D3DDevice;
            var context = D3DApp11.I.ImmediateContext;
            var camera = D3DApp11.I.Camera;
            var sky = D3DApp11.I.Skybox;
            var basicFx = EffectManager11.Instance.GetEffect<BasicEffect11>();
            var normalMapFx = EffectManager11.Instance.GetEffect<NormalMapEffect11>();
            var displacementMapFx = EffectManager11.Instance.GetEffect<DisplacementMapEffect11>();
            var depthStencilView = D3DApp11.I.RenderTarget.DepthStencilView;
            var renderTargetView = D3DApp11.I.RenderTarget.RenderTargetView;
            var viewport = D3DApp11.I.RenderTarget.Viewport;

            basicFx.SetShadowMap(null);
            normalMapFx.SetShadowMap(null);
            displacementMapFx.SetShadowMap(null);

            _shadowMap.BindDsvAndSetNullRenderTarget(context);

            DrawSceneToShadowMap(device);

            // 必须复原，避免加入法向贴图和粒子时错误渲染
            context.Rasterizer.State = null;
            context.OutputMerger.BlendState = null;
            if (_settings.DrawMode == DrawMode.Wireframe) {
                context.Rasterizer.State = RenderStates11.Instance.WireframeRS;
            }

            context.HullShader.Set(null);
            context.DomainShader.Set(null);
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.OutputMerger.SetTargets(depthStencilView, renderTargetView);
            context.Rasterizer.SetViewports(new RawViewportF[] { viewport });

            var view = camera.ViewMatrix;
            var proj = camera.ProjectionMatrix;

            basicFx.SetDirLights(_dirLights);
            basicFx.SetEyePosW(camera.Position);
            basicFx.SetCubeMap(_settings.IsReflectionEnabled ? sky.CubeMapSRV : null);
            basicFx.SetShadowMap(_settings.IsShadowEnabled ? _shadowMap.DepthMapSRV : null);

            normalMapFx.SetDirLights(_dirLights);
            normalMapFx.SetEyePosW(camera.Position);
            normalMapFx.SetCubeMap(_settings.IsReflectionEnabled ? sky.CubeMapSRV : null);
            normalMapFx.SetShadowMap(_settings.IsShadowEnabled ? _shadowMap.DepthMapSRV : null);

            displacementMapFx.SetDirLights(_dirLights);
            displacementMapFx.SetEyePosW(camera.Position);
            displacementMapFx.SetCubeMap(_settings.IsReflectionEnabled ? sky.CubeMapSRV : null);
            displacementMapFx.SetShadowMap(_settings.IsShadowEnabled ? _shadowMap.DepthMapSRV : null);
            displacementMapFx.SetMaxTessDistance(0.1f);
            displacementMapFx.SetMinTessDistance(0.05f);

            EffectTechnique activeNonReflectiveGeomTech = normalMapFx.Light3TexTech;
            EffectTechnique activeReflectiveGeomTech = basicFx.Light3TexReflectTech;
            EffectTechnique activeModelTech = basicFx.Light3TexReflectTech;

            UpdateTechniques(out activeNonReflectiveGeomTech, out activeReflectiveGeomTech, out activeModelTech);

            context.InputAssembler.InputLayout = InputLayouts.PosNormTexTan;

            RenderMode renderMode;
            switch (_settings.SurfaceMapping) {
                case SurfaceMapping.Simple:
                    renderMode = RenderMode.Basic;
                    break;
                case SurfaceMapping.NormalMapping:
                    renderMode = RenderMode.NormalMapped;
                    break;
                case SurfaceMapping.DisplacementMapping:
                    renderMode = RenderMode.DisplacementMapped;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_settings.SurfaceMapping));
            }

            for (var p = 0; p < activeNonReflectiveGeomTech.Description.PassCount; p++) {
                // draw grid
                var pass = activeNonReflectiveGeomTech.GetPassByIndex(p);
                _grid.ShadowTransform = _shadowTransform;
                _grid.Draw(context, pass, view, proj, renderMode);
                // draw box
                _box.ShadowTransform = _shadowTransform;
                _box.Draw(context, pass, view, proj, renderMode);

                // draw columns
                foreach (var cylinder in _cylinders) {
                    cylinder.ShadowTransform = _shadowTransform;
                    cylinder.Draw(context, pass, view, proj, renderMode);
                }
            }

            for (var p = 0; p < activeReflectiveGeomTech.Description.PassCount; p++) {
                var pass = activeReflectiveGeomTech.GetPassByIndex(p);
                foreach (var sphere in _spheres) {
                    sphere.ShadowTransform = _shadowTransform;
                    sphere.Draw(context, pass, view, proj, RenderMode.Basic);
                }
            }

            // MIC, decelerator
            for (var p = 0; p < activeModelTech.Description.PassCount; ++p) {
                if (_settings.IsDeceleratorVisible) {
                    _decelInstance.ShadowTransform = _shadowTransform;
                    _decelInstance.Draw(context, activeModelTech.GetPassByIndex(p), view, proj, RenderMode.Basic);
                }
                if (_settings.IsBarbecueBarVisible) {
                    _barbInstance.ShadowTransform = _shadowTransform;
                    _barbInstance.Draw(context, activeModelTech.GetPassByIndex(p), view, proj, RenderMode.Basic);
                }
                if (_settings.IsTruckVisible) {
                    _truckInstance.ShadowTransform = _shadowTransform;
                    _truckInstance.Draw(context, activeModelTech.GetPassByIndex(p), view, proj, RenderMode.Basic);
                }
                if (_settings.IsTireVisible) {
                    _tireInstance.ShadowTransform = _shadowTransform;
                    _tireInstance.Draw(context, activeModelTech.GetPassByIndex(p), view, proj, RenderMode.Basic);
                }
            }

            context.Rasterizer.State = null;

            if (_settings.QuadVisible) {
                DrawScreenQuad(device);
            }

            base.DrawInternal(gameTime);
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

        private void UpdateTechniques(out EffectTechnique nonReflective, out EffectTechnique reflective, out EffectTechnique models) {
            var pair = _techniqueTable[new Tuple<bool, bool, SurfaceMapping, NumberOfLights>(
                _settings.IsReflectionEnabled, _settings.MaterialType != MaterialType.None, _settings.SurfaceMapping, _settings.NumberOfLights)];
            nonReflective = pair.Item1;
            reflective = pair.Item2;
            models = pair.Item3;
        }

        private void InitializeTechniqueTable(BasicEffect11 basicFx, NormalMapEffect11 normalMapFx, DisplacementMapEffect11 displacementMapFx) {
            var dict1 = new Dictionary<SurfaceMapping, BasicEffect11>() {
                [SurfaceMapping.Simple] = basicFx,
                [SurfaceMapping.NormalMapping] = normalMapFx,
                [SurfaceMapping.DisplacementMapping] = displacementMapFx
            };
            // reflection, texture, surface, lights
            // non-ref, ref, model
            foreach (var surface in new[] { SurfaceMapping.Simple, SurfaceMapping.NormalMapping, SurfaceMapping.DisplacementMapping }) {
                foreach (var reflect in new[] { true, false }) {
                    foreach (var texture in new[] { true, false }) {
                        foreach (var lights in new[] { NumberOfLights.One, NumberOfLights.Two, NumberOfLights.Three }) {
                            _techniqueTable.Add(new Tuple<bool, bool, SurfaceMapping, NumberOfLights>(reflect, texture, surface, lights),
                                new Tuple<EffectTechnique, EffectTechnique, EffectTechnique>(
                                    GetTechnique(dict1[surface], texture, false, lights),
                                    GetTechnique(basicFx, texture, true, lights),
                                    GetTechnique(basicFx, texture, reflect, lights)));
                        }
                    }
                }
            }
        }

        private EffectTechnique GetTechnique(BasicEffect11 effect, bool texture, bool reflection, NumberOfLights lights) {
            string fieldName;
            switch (lights) {
                case NumberOfLights.One:
                    fieldName = "Light1";
                    break;
                case NumberOfLights.Two:
                    fieldName = "Light2";
                    break;
                case NumberOfLights.Three:
                    fieldName = "Light3";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (texture) {
                fieldName += "Tex";
            }
            if (reflection) {
                fieldName += "Reflect";
            }
            fieldName += "Tech";
            var prop = effect.GetType().GetProperty(fieldName);
            return prop.GetValue(effect) as EffectTechnique;
        }

        private void DrawSceneToShadowMap(Device device) {
            var buildShadowMapFx = EffectManager11.Instance.GetEffect<BuildShadowMapEffect11>();
            var context = device.ImmediateContext;
            var camera = D3DApp11.I.Camera;
            var view = _lightView;
            var proj = _lightProj;
            var viewProj = view * proj;

            buildShadowMapFx.SetEyePosW(camera.Position);
            buildShadowMapFx.SetViewProj(viewProj);

            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            var shadowMapTech = buildShadowMapFx.BuildShadowMapTech;

            context.InputAssembler.InputLayout = InputLayouts.PosNormTexTan;

            for (var p = 0; p < shadowMapTech.Description.PassCount; p++) {
                var pass = shadowMapTech.GetPassByIndex(p);
                _grid.Draw(context, pass, view, proj, RenderMode.ShadowMap);

                _box.Draw(context, pass, view, proj, RenderMode.ShadowMap);

                if (_settings.IsDeceleratorVisible) {
                    _decelInstance.Draw(context, pass, view, proj, RenderMode.ShadowMap);
                }
                if (_settings.IsBarbecueBarVisible) {
                    _barbInstance.Draw(context, pass, view, proj, RenderMode.ShadowMap);
                }
                if (_settings.IsTruckVisible) {
                    _truckInstance.Draw(context, pass, view, proj, RenderMode.ShadowMap);
                }
                if (_settings.IsTireVisible) {
                    _tireInstance.Draw(context, pass, view, proj, RenderMode.ShadowMap);
                }

                foreach (var cylinder in _cylinders) {
                    cylinder.Draw(context, pass, view, proj, RenderMode.ShadowMap);
                }
            }

            context.HullShader.Set(null);
            context.DomainShader.Set(null);
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            for (var p = 0; p < shadowMapTech.Description.PassCount; p++) {
                var pass = shadowMapTech.GetPassByIndex(p);
                foreach (var sphere in _spheres) {
                    sphere.Draw(context, pass, view, proj, RenderMode.ShadowMap);
                }
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
                debugTextureFx.SetTexture(_shadowMap.DepthMapSRV);
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

            _boxModel = BasicModel.CreateBox(device, 1, 1, 1);
            _boxModel.Materials[0] = new Material {
                Ambient = Color.White,
                Diffuse = Color.White,
                Specular = new Color(0.8f, 0.8f, 0.8f, 16.0f),
                Reflect = Color.Black
            };
            _boxModel.DiffuseMapSRV[0] = textureManager.CreateTexture(NoireConfiguration.GetFullResourcePath("textures/bricks.dds"));
            //_boxModel.DiffuseMapSRV[0] = textureManager.CreateTexture(NoireConfiguration.GetFullResourcePath("textures/metal.jpg"));
            _boxModel.NormalMapSRV[0] = textureManager.CreateTexture(NoireConfiguration.GetFullResourcePath("textures/bricks_nmap.png"));

            _gridModel = BasicModel.CreateGrid(device, 80, 120, 40, 60);
            _gridModel.Materials[0] = new Material {
                Ambient = new Color(0.8f, 0.8f, 0.8f),
                Diffuse = new Color(0.8f, 0.8f, 0.8f),
                Specular = new Color(0.8f, 0.8f, 0.8f, 16.0f),
                Reflect = Color.Black
            };
            _gridModel.DiffuseMapSRV[0] = textureManager.CreateTexture(NoireConfiguration.GetFullResourcePath("textures/floor.dds"));
            _gridModel.NormalMapSRV[0] = textureManager.CreateTexture(NoireConfiguration.GetFullResourcePath("textures/floor_nmap.png"));

            _sphereModel = BasicModel.CreateSphere(device, 0.5f, 20, 20);
            _sphereModel.Materials[0] = new Material {
                Ambient = new Color(0.6f, 0.8f, 0.9f),
                Diffuse = new Color(0.6f, 0.8f, 0.9f),
                Specular = new Color(0.9f, 0.9f, 0.9f, 16.0f),
                Reflect = new Color(0.4f, 0.4f, 0.4f)
            };
            _cylinderModel = BasicModel.CreateCylinder(device, 0.5f, 0.3f, 3.0f, 20, 20);
            _cylinderModel.Materials[0] = new Material {
                Ambient = Color.White,
                Diffuse = Color.White,
                //Specular = new Color(0.3f, 0.3f, 0.3f, 16.0f), // plywood
                //Specular = new Color(0.8f, 0.8f, 0.8f, 16.0f), // metal, stone
                Specular = new Color(0.6f, 0.6f, 0.6f, 16.0f), // leather
                Reflect = Color.Black
            };
            _cylinderModel.DiffuseMapSRV[0] = textureManager.CreateTexture(NoireConfiguration.GetFullResourcePath("textures/material/copper_Base_Color.png"));
            _cylinderModel.NormalMapSRV[0] = textureManager.CreateTexture(NoireConfiguration.GetFullResourcePath("textures/material/copper_Normal.png"));

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
                TexTransform = Matrix.Scaling(32, 40, 1),
                World = Matrix.Identity
            };

            _box = new BasicModelInstance(_boxModel) {
                TexTransform = Matrix.Scaling(2, 1, 1),
                World = Matrix.Scaling(3.0f, 1.0f, 3.0f) * Matrix.Translation(0, 0.5f, 0)
            };

            // MIC
            const string decelFileDir = @"C:\Users\MIC\Documents\3dsMax\export";
            var decelFile = Path.Combine(decelFileDir, "decel.3ds");
            _decelModel = BasicModel.Create(device, TextureManager11.Instance, decelFile, decelFileDir);
            _decelWorld = Matrix.Scaling(8f) * Matrix.Translation(0, 0.6f, 5); // * Matrix.RotationX(MathUtil.DegreesToRadians(-90f));
            _decelInstance = new BasicModelInstance(_decelModel) {
                TexTransform = Matrix.Identity,
                World = _decelWorld
            };

            const string barbFileDir = @"C:\Users\MIC\Desktop\models\barb01";
            var barbFile = Path.Combine(barbFileDir, "barb01.3ds");
            _barbModel = BasicModel.Create(device, TextureManager11.Instance, barbFile, barbFileDir);
            _barbWorld = Matrix.Scaling(0.003f) * Matrix.Translation(0, 1, 0.3f);
            _barbInstance = new BasicModelInstance(_barbModel) {
                TexTransform = Matrix.Identity,
                World = _barbWorld
            };

            const string truckFileDir = @"C:\Users\MIC\Desktop\m4\ea55";
            var truckFile = Path.Combine(truckFileDir, "mic.3ds");
            _truckModel = BasicModel.Create(device, TextureManager11.Instance, truckFile, truckFileDir);
            _truckWorld = Matrix.Scaling(0.003f) * Matrix.Translation(0, 8, 2) * Matrix.RotationX(MathUtil.DegreesToRadians(-90f));
            _truckInstance = new BasicModelInstance(_truckModel) {
                TexTransform = Matrix.Identity,
                World = _truckWorld
            };

            const string tireFileDir = @"C:\Users\MIC\Desktop\models\tire02";
            var tireFile = Path.Combine(tireFileDir, "tire02.3ds");
            _tireModel = BasicModel.Create(device, TextureManager11.Instance, tireFile, tireFileDir);
            _tireWorld = Matrix.Scaling(0.003f) * Matrix.Translation(0, 4, 1) * Matrix.RotationX(MathUtil.DegreesToRadians(-90f));
            _tireInstance = new BasicModelInstance(_tireModel) {
                TexTransform = Matrix.Identity,
                World = _tireWorld
            };
        }

        private void BuildScreenQuadGeometryBuffers(Device device) {
            var quad = GeometryGenerator.CreateFullScreenQuad();

            var verts = quad.Vertices.Select(v => new VertPosNormTex(v.Position, v.Normal, v.TexCoords)).ToList();
            var vbd = new BufferDescription(VertPosNormTex.Stride * verts.Count, ResourceUsage.Immutable, BindFlags.VertexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            _screenQuadVB = new Buffer(device, DataStream.Create(verts.ToArray(), false, false), vbd);

            var ibd = new BufferDescription(sizeof(int) * quad.Indices.Count, ResourceUsage.Immutable, BindFlags.IndexBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            _screenQuadIB = new Buffer(device, DataStream.Create(quad.Indices.ToArray(), false, false), ibd);
        }

        private void UpdateBufferedSettings() {
            _settings = _bufferedSettings.Clone();

            if (IsInitialized) {
                _fire.Visible = _settings.ParticleFlameVisible;
                _rain.Visible = _settings.ParticleRainVisible;

                var skybox = D3DApp11.I.Skybox;
                switch (_settings.SkyboxType) {
                    case SkyboxType.None:
                        skybox.CubeMapSRV = null;
                        break;
                    case SkyboxType.Desert:
                        skybox.CubeMapSRV = TextureManager11.Instance.QuickCreateCubeMap("desert");
                        break;
                    case SkyboxType.Snowfield:
                        skybox.CubeMapSRV = TextureManager11.Instance.QuickCreateCubeMap("snowfield");
                        break;
                    case SkyboxType.Factory:
                        skybox.CubeMapSRV = TextureManager11.Instance.QuickCreateCubeMap("factory");
                        break;
                    default:
                        skybox.CubeMapSRV = null;
                        break;
                }

                var textureManager = TextureManager11.Instance;
                string mat;
                switch (_settings.MaterialType) {
                    case MaterialType.None:
                        mat = null;
                        break;
                    case MaterialType.Copper:
                        mat = "copper";
                        break;
                    case MaterialType.StainlessSteel:
                        mat = "s_steel";
                        break;
                    case MaterialType.Aluminium:
                        mat = "aluminium";
                        break;
                    case MaterialType.Plywood:
                        mat = "plywood";
                        break;
                    default:
                        mat = null;
                        break;
                }
                if (mat != null) {
                    _cylinderModel.DiffuseMapSRV[0] = textureManager.CreateTexture(NoireConfiguration.GetFullResourcePath($"textures/material/{mat}_Base_Color.png"));
                    _cylinderModel.NormalMapSRV[0] = textureManager.CreateTexture(NoireConfiguration.GetFullResourcePath($"textures/material/{mat}_Normal.png"));
                } else {
                    _cylinderModel.DiffuseMapSRV[0] = null;
                    _cylinderModel.NormalMapSRV[0] = null;
                }
            }
        }

        private ShadowSceneSettings _bufferedSettings;
        private ShadowSceneSettings _settings;

        private static readonly int ShadowMapSize = 2048;

        private Buffer _screenQuadVB;
        private Buffer _screenQuadIB;
        private BoundingSphere _sceneBounds;

        private ShadowMap _shadowMap;
        private Matrix _lightView;
        private Matrix _lightProj;
        private Matrix _shadowTransform;

        private float _lightRotationAngle;
        private Vector3[] _originalLightDirs;
        private DirectionalLight[] _dirLights;

        private BasicModel _boxModel;
        private BasicModel _gridModel;
        private BasicModel _sphereModel;
        private BasicModel _cylinderModel;

        private BasicModelInstance _grid;
        private BasicModelInstance _box;
        private readonly BasicModelInstance[] _spheres = new BasicModelInstance[10];
        private readonly BasicModelInstance[] _cylinders = new BasicModelInstance[10];

        private Matrix _decelWorld;
        private BasicModel _decelModel;
        private BasicModelInstance _decelInstance;
        private Matrix _barbWorld;
        private BasicModel _barbModel;
        private BasicModelInstance _barbInstance;
        private Matrix _truckWorld;
        private BasicModel _truckModel;
        private BasicModelInstance _truckInstance;
        private Matrix _tireWorld;
        private BasicModel _tireModel;
        private BasicModelInstance _tireInstance;

        private ShaderResourceView _flareTexSRV;
        private ShaderResourceView _rainTexSRV;
        private ShaderResourceView _randomTex;

        private Dictionary<Tuple<bool, bool, SurfaceMapping, NumberOfLights>, Tuple<EffectTechnique, EffectTechnique, EffectTechnique>> _techniqueTable;

        private ParticleSource _fire;
        private ParticleSource _rain;

    }
}
