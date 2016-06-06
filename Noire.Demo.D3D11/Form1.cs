using System;
using System.Windows.Forms;
using Noire.Demo.D3D11.DemoFinal;
using Noire.Graphics.D3D11;
using Noire.Graphics.D3D11.FX;
using SharpDX;
using DrawMode = Noire.Demo.D3D11.DemoFinal.DrawMode;

namespace Noire.Demo.D3D11 {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
            InitializeEventHandlers();
            InitializeExtraControls();

            _app = D3DApp11.Create(label1);
        }

        private void InitializeEventHandlers() {
            Load += Form1_Load;
            ResizeEnd += Form1_ResizeEnd;
            SizeChanged += Form1_SizeChanged;
            FormClosed += Form1_FormClosed;
            Activated += Form1_Activated;
            Deactivate += Form1_Deactivate;
            label1.MouseDown += Form1_MouseDown;
            label1.MouseUp += Form1_MouseUp;
            label1.MouseMove += Form1_MouseMove;
            timer1.Tick += timer1_Tick;
        }

        private void timer1_Tick(object sender, EventArgs e) {
            Text = $"Noire Demo 06-06 ({_app.Fps.ToString("0.00")} fps on {_app.DriverName})";
        }

        private void InitializeExtraControls() {
            timer1.Enabled = true;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                var camera = D3DApp11.I.Camera;
                var dx = MathUtil.DegreesToRadians(0.25f * (e.X - _lastMousePos.X));
                var dy = MathUtil.DegreesToRadians(0.25f * (e.Y - _lastMousePos.Y));

                camera.Pitch(dy);
                camera.Yaw(dx);
            }
            _lastMousePos = e.Location;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e) {
            var c = sender as Control;
            if (c != null) {
                c.Capture = false;
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e) {
            _lastMousePos = e.Location;
            var c = sender as Control;
            if (c != null) {
                c.Capture = true;
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e) {
            if (WindowState == FormWindowState.Minimized) {
                _lastWindowState = FormWindowState.Minimized;
            } else {
                if (WindowState != _lastWindowState) {
                    _app.ResetSurface(this);
                    _lastWindowState = WindowState;
                }
            }
        }

        private void Form1_Deactivate(object sender, EventArgs e) {
            _app.IsPaused = true;
        }

        private void Form1_Activated(object sender, EventArgs e) {
            _app.IsPaused = false;
        }

        private void Form1_ResizeEnd(object sender, EventArgs e) {
            if (WindowState != FormWindowState.Minimized) {
                _app?.ResetSurface(this);
            }
        }

        private void Form1_FormClosed(object sender, EventArgs e) {
            _app.Terminate();
            _app.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e) {
            _lastWindowState = WindowState;
            _app.Initialize();
            _app.RunAsync();

            var camera = _app.Camera;
            camera.Position = new Vector3(0, 5, -15);
            camera.LookAt(Vector3.Zero, Vector3.UnitY);

            var scene = new ShadowScene(_app, _app);
            scene.Initialize();
            scene.Name = "ShapesScene";
            _scene = scene;
            _app.ChildComponents.Add(scene);
            var inputHandler = new InputHandler(_app, _app);
            inputHandler.Initialize();
            _app.ChildComponents.Add(inputHandler);

            scene.SetShadowEnabled(true);
            scene.SetReflectionEnabled(true);
            scene.SetMaterialType(MaterialType.Copper);
            scene.SetNumberOfLights(NumberOfLights.Three);
            scene.SetSkyboxType(SkyboxType.None);
            scene.SetShadowEnabled(false);

            mnuDisplayNormal.Checked = true;
            mnuLights3.Checked = true;
            mnuSkyboxDisabled.Checked = true;
            mnuTextureCopper.Checked = true;
            mnuSurfaceDisabled.Checked = true;
            mnuShadowDisabled.Checked = true;
        }

        private readonly D3DApp11 _app;
        private ShadowScene _scene;
        private System.Drawing.Point _lastMousePos;
        private FormWindowState _lastWindowState;

        private void mnuDisplayNormal_Click(object sender, EventArgs e) {
            mnuDisplayWireframe.Checked = false;
            _scene.SetDrawMode(DrawMode.Normal);
        }

        private void mnuDisplayWireframe_Click(object sender, EventArgs e) {
            mnuDisplayNormal.Checked = false;
            _scene.SetDrawMode(DrawMode.Wireframe);
        }

        private void mnuSkyboxDisabled_Click(object sender, EventArgs e) {
            _scene.SetSkyboxType(SkyboxType.None);
            mnuSkyboxDesert.Checked = mnuSkyboxSnowfield.Checked = mnuSkyboxFactory.Checked = false;
        }

        private void mnuSkyboxDesert_Click(object sender, EventArgs e) {
            _scene.SetSkyboxType(SkyboxType.Desert);
            mnuSkyboxDisabled.Checked = mnuSkyboxSnowfield.Checked = mnuSkyboxFactory.Checked = false;
        }

        private void mnuSkyboxSnowfield_Click(object sender, EventArgs e) {
            _scene.SetSkyboxType(SkyboxType.Snowfield);
            mnuSkyboxDesert.Checked = mnuSkyboxDisabled.Checked = mnuSkyboxFactory.Checked = false;
        }

        private void mnuSkyboxFactory_Click(object sender, EventArgs e) {
            _scene.SetSkyboxType(SkyboxType.Factory);
            mnuSkyboxDesert.Checked = mnuSkyboxSnowfield.Checked = mnuSkyboxDisabled.Checked = false;
        }

        private void mnuShadowDB_Click(object sender, EventArgs e) {
            _scene.SetQuadVisible(mnuShadowDB.Checked);
        }

        private void mnuParticleRain_Click(object sender, EventArgs e) {
            _scene.SetParticleRainVisible(mnuParticleRain.Checked);
        }

        private void mnuParticleFlame_Click(object sender, EventArgs e) {
            _scene.SetParticleFlameVisible(mnuParticleFlame.Checked);
        }

        private void mnuModelDecel_Click(object sender, EventArgs e) {
            _scene.SetDeceleratorVisible(mnuModelDecel.Checked);
        }

        private void mnuModelBarb_Click(object sender, EventArgs e) {
            _scene.SetBarbecueBarVisible(mnuModelBarb.Checked);
        }

        private void mnuLightsMoving_Click(object sender, EventArgs e) {
            _scene.SetLightsMoving(mnuLightsMoving.Checked);
        }

        private void mnuModelTire_Click(object sender, EventArgs e) {
            _scene.SetTireVisible(mnuModelTire.Checked);
        }

        private void mnuModelTruck_Click(object sender, EventArgs e) {
            _scene.SetTruckVisible(mnuModelTruck.Checked);
        }

        private void mnuLights1_Click(object sender, EventArgs e) {
            _scene.SetNumberOfLights(NumberOfLights.One);
            mnuLights2.Checked = mnuLights3.Checked = false;
        }

        private void mnuLights2_Click(object sender, EventArgs e) {
            _scene.SetNumberOfLights(NumberOfLights.Two);
            mnuLights1.Checked = mnuLights3.Checked = false;
        }

        private void mnuLights3_Click(object sender, EventArgs e) {
            _scene.SetNumberOfLights(NumberOfLights.Three);
            mnuLights2.Checked = mnuLights1.Checked = false;
        }

        private void mnuReflectionDisabled_Click(object sender, EventArgs e) {
            _scene.SetReflectionEnabled(!mnuReflectionDisabled.Checked);
        }

        private void mnuTextureDisabled_Click(object sender, EventArgs e) {
            _scene.SetMaterialType(MaterialType.None);
            mnuTextureCopper.Checked = mnuTextureSSteel.Checked = mnuTextureAl.Checked = mnuTexturePlywood.Checked = false;
        }

        private void mnuTextureCopper_Click(object sender, EventArgs e) {
            _scene.SetMaterialType(MaterialType.Copper);
            mnuTextureDisabled.Checked = mnuTextureSSteel.Checked = mnuTextureAl.Checked = mnuTexturePlywood.Checked = false;
        }

        private void mnuTextureSSteel_Click(object sender, EventArgs e) {
            _scene.SetMaterialType(MaterialType.StainlessSteel);
            mnuTextureCopper.Checked = mnuTextureDisabled.Checked = mnuTextureAl.Checked = mnuTexturePlywood.Checked = false;
        }

        private void mnuTextureAl_Click(object sender, EventArgs e) {
            _scene.SetMaterialType(MaterialType.Aluminium);
            mnuTextureCopper.Checked = mnuTextureSSteel.Checked = mnuTextureDisabled.Checked = mnuTexturePlywood.Checked = false;
        }

        private void mnuTexturePlywood_Click(object sender, EventArgs e) {
            _scene.SetMaterialType(MaterialType.Plywood);
            mnuTextureCopper.Checked = mnuTextureSSteel.Checked = mnuTextureAl.Checked = mnuTextureDisabled.Checked = false;
        }

        private void mnuShadowDisabled_Click(object sender, EventArgs e) {
            _scene.SetShadowEnabled(!mnuShadowDisabled.Checked);
        }

        private void mnuSurfaceDisabled_Click(object sender, EventArgs e) {
            _scene.SetSurfaceMapping(SurfaceMapping.Simple);
            mnuSurfaceNM.Checked = mnuSurfaceDM.Checked = false;
        }

        private void mnuSurfaceNM_Click(object sender, EventArgs e) {
            _scene.SetSurfaceMapping(SurfaceMapping.NormalMapping);
            mnuSurfaceDisabled.Checked = mnuSurfaceDM.Checked = false;
        }

        private void mnuReflectionReplaceMaterial_Click(object sender, EventArgs e) {
            _scene.ReplaceDeceleratorSurfaceMaterial();
        }
    }
}
