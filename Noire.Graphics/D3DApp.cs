using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Noire.Common;
using SharpDX;
using SharpDX.Windows;

namespace Noire.Graphics {
    public abstract class D3DApp : DisposeBase {

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

        public abstract void Initialize();

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
            using (var renderLoop = new RenderLoop(ControlWindow)) {
                while (renderLoop.NextFrame()) {
                    Render();
                }
            }
        }

        public abstract void Terminate();

        public double Fps => _fps;

        public bool ManualVSync { get; set; }

        protected abstract void Update(GameTime gameTime);

        protected abstract void Render(GameTime gameTime);

        private void Render() {
            if (!IsRunning) {
                return;
            }
            Timer.FrameTime = 1f / 60f;
            Timer.Tick();
            if (!IsPaused) {
                if (!ManualVSync || (Timer.TotalTime - _lastRenderSecond > Timer.FrameTime)) {
                    var gameTime = new GameTime(TimeSpan.FromSeconds(Timer.DeltaTime), TimeSpan.FromSeconds(Timer.TotalTime));
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
        protected bool IsInitialized;
        private bool _hasRun;
        private double _fps;
        private int _frameCount;
        private double _lastFpsSecond;
        private double _lastRenderSecond;

    }
}
