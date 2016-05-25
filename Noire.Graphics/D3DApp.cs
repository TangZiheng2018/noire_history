using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Noire.Common;
using Noire.Common.Camera;
using SharpDX;
using SharpDX.Windows;

namespace Noire.Graphics {
    public abstract class D3DApp : GameComponentRoot {

        protected D3DApp(Control control) {
            IsPaused = false;
            Timer = new AccurateTimer();
            ControlWindow = control;
            _fps = 0;
            _frameCount = 0;
            _lastFpsSecond = 0;
            _lastRenderSecond = 0;
            _hasRun = false;
        }

        public Control ControlWindow { get; }

        public bool IsPaused { get; set; }

        public AccurateTimer Timer { get; }

        public void Run() {
            if (!IsInitialized || _hasRun) {
                return;
            }
            IsRunning = true;
            _hasRun = true;
            Timer.Reset();
            ControlWindow.Show();
            ControlWindow.Focus();
            using (var renderLoop = new RenderLoop(ControlWindow)) {
                while (renderLoop.NextFrame()) {
                    Render();
                }
            }
        }

        public Task RunAsync() {
            return Task.Run(new Action(Run));
        }

        public abstract void Terminate();

        public double Fps => _fps;

        public bool ManualVSync { get; set; }

        public CameraBase Camera => _camera;

        public abstract string DriverName { get; }

        public void ResetSurface(object sender) {
            _userResized = true;
        }

        protected abstract void Render(GameTime gameTime);

        protected override void OnSurfaceInvalidated(object sender, EventArgs e) {
            var clientSize = ControlWindow.ClientSize;
            _camera.Aspect = (float)clientSize.Width / clientSize.Height;
            base.OnSurfaceInvalidated(sender, e);
        }

        protected override void UpdateInternal(GameTime gameTime) {
            base.UpdateInternal(gameTime);
            _camera.UpdateViewMatrix();
        }

        protected override void InitializeInternal() {
            base.InitializeInternal();

            var clientSize = ControlWindow.ClientSize;
            _camera = new FpsCamera(MathUtil.DegreesToRadians(45), (float)clientSize.Width / clientSize.Height, 0.1f, 1000);
            NoireConfiguration.ResourceBase = "resources";
        }

        private void Render() {
            if (!IsRunning) {
                return;
            }
            Timer.FrameTime = 1f / 60f;
            Timer.Tick();
            if (!IsPaused) {
                if (!ManualVSync || (Timer.TotalTime - _lastRenderSecond > Timer.FrameTime)) {
                    if (_userResized) {
                        RaiseSurfaceInvalidated(this, EventArgs.Empty);
                        _userResized = false;
                    }
                    var gameTime = new GameTime(TimeSpan.FromSeconds(Timer.FrameTime), TimeSpan.FromSeconds(Timer.TotalTime));
                    Update(gameTime);
                    Render(gameTime);
                    CalculateFps();
                    _lastRenderSecond = Timer.TotalTime;
                }
            } else {
                Thread.Sleep(100);
            }
        }

        private void CalculateFps() {
            ++_frameCount;
            var totalTime = Timer.TotalTime;
            var secondsElapsed = totalTime - _lastFpsSecond;
            if (secondsElapsed >= 1) {
                _fps = _frameCount / secondsElapsed;
                _frameCount = 0;
                _lastFpsSecond = totalTime;
            }
        }

        protected bool IsRunning;
        private CameraBase _camera;
        private bool _hasRun;
        private double _fps;
        private int _frameCount;
        private double _lastFpsSecond;
        private double _lastRenderSecond;
        private bool _userResized;

    }
}
