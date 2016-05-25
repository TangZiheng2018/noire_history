using Noire.Common;
using Noire.Graphics.D3D11;
using SharpDX.DirectInput;

namespace Noire.Demo.D3D11 {
    public sealed class InputHandler : GameComponent {

        public InputHandler(IGameComponentRoot root, IGameComponentContainer parent)
            : base(root, parent) {
            _directInput = new DirectInput();
            _keyboard = new Keyboard(_directInput);
        }

        protected override void InitializeInternal() {
            base.InitializeInternal();
            _keyboard.Acquire();
        }

        protected override void UpdateInternal(GameTime gameTime) {
            base.UpdateInternal(gameTime);

            var camera = D3DApp11.I.Camera;
            var state = _keyboard.GetCurrentState();
            var d = (float)gameTime.ElapsedGameTime.TotalSeconds;
            const float distanceFactor = 3f;
            const float angleFactor = 0.1f;
            const float zoomFactor = 0.01f;

            if (state.IsPressed(Key.W)) {
                camera.Walk(d * distanceFactor);
            }
            if (state.IsPressed(Key.S)) {
                camera.Walk(-d * distanceFactor);
            }
            if (state.IsPressed(Key.A)) {
                camera.Strafe(-d * distanceFactor);
            }
            if (state.IsPressed(Key.D)) {
                camera.Strafe(d * distanceFactor);
            }
            if (state.IsPressed(Key.Right)) {
                camera.Yaw(d * angleFactor);
            }
            if (state.IsPressed(Key.Left)) {
                camera.Yaw(-d * angleFactor);
            }
            if (state.IsPressed(Key.Up)) {
                camera.Pitch(-d * angleFactor);
            }
            if (state.IsPressed(Key.Down)) {
                camera.Pitch(d * angleFactor);
            }
            if (state.IsPressed(Key.Equals) || state.IsPressed(Key.PageUp) || state.IsPressed(Key.Add)) {
                camera.Zoom(-d * zoomFactor);
            }
            if (state.IsPressed(Key.Subtract) || state.IsPressed(Key.PageDown) || state.IsPressed(Key.Minus)) {
                camera.Zoom(d * zoomFactor);
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

    }
}
