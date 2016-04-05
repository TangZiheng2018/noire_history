using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Noire.Graphics.D3D11;
using SharpDX;

namespace Noire.Demo.D3D11 {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
            InitializeEventHandlers();

            _app = D3DApp11.Create(this);
        }

        private void InitializeEventHandlers() {
            Load += Form1_Load;
            ResizeEnd += Form1_ResizeEnd;
            SizeChanged += Form1_SizeChanged;
            FormClosed += Form1_FormClosed;
            Activated += Form1_Activated;
            Deactivate += Form1_Deactivate;
        }

        protected override void OnMouseDown(MouseEventArgs mouseEventArgs) {
            _lastMousePos = mouseEventArgs.Location;
            Capture = true;
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            Capture = false;
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                var camera = D3DApp11.I.RenderTarget.Camera;
                var dx = MathUtil.DegreesToRadians(0.25f * (e.X - _lastMousePos.X));
                var dy = MathUtil.DegreesToRadians(0.25f * (e.Y - _lastMousePos.Y));

                camera.Pitch(dy);
                camera.Yaw(dx);
            }
            _lastMousePos = e.Location;
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

            var camera = _app.RenderTarget.Camera;
            camera.Position = new Vector3(0, 5, -15);
            camera.LookAt(Vector3.Zero, Vector3.UnitY);
            var scene = new ShapesScene();
            scene.Initialize();
            _app.ChildComponents.Add(scene);
            var inputHandler = new InputHandler();
            inputHandler.Initialize();
            _app.ChildComponents.Add(inputHandler);
        }

        private readonly D3DApp11 _app;
        private System.Drawing.Point _lastMousePos;
        private FormWindowState _lastWindowState;

    }
}
