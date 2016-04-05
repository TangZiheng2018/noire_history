using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Common;
using Noire.Graphics.D3D11;
using SharpDX.DirectInput;

namespace Noire.Demo.D3D11 {
    public sealed class InputHandler : GameComponent {

        public InputHandler() {
            _directInput = new DirectInput();
            _keyboard = new Keyboard(_directInput);
        }

        protected override void InitializeInternal() {
            base.InitializeInternal();
            _keyboard.Acquire();
        }

        protected override void UpdateInternal(GameTime gameTime) {
            base.UpdateInternal(gameTime);

            var camera = D3DApp11.I.RenderTarget.Camera;
            var state = _keyboard.GetCurrentState();
            var d = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            const float factor = 0.01f;

            if (state.IsPressed(Key.W)) {
                camera.Walk(d * factor);
            }
            if (state.IsPressed(Key.S)) {
                camera.Walk(-d * factor);
            }
            if (state.IsPressed(Key.A)) {
                camera.Strafe(-d * factor);
            }
            if (state.IsPressed(Key.D)) {
                camera.Strafe(d * factor);
            }
            if (state.IsPressed(Key.Right)) {
                camera.Yaw(d * factor);
            }
            if (state.IsPressed(Key.Left)) {
                camera.Yaw(-d * factor);
            }
            if (state.IsPressed(Key.Equals)) {
                _zoom *= 1.05f;
                camera.Zoom(_zoom);
            }
            if (state.IsPressed(Key.Minus)) {
                _zoom /= 1.05f;
                camera.Zoom(_zoom);
            }
        }

        protected override void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            if (disposing) {
                _keyboard.Unacquire();
                NoireUtilities.DisposeNonPublicDeclaredFields(this);
            }
            base.Dispose(disposing);
        }

        private Keyboard _keyboard;
        private DirectInput _directInput;
        private float _zoom = 1f;

    }
}
