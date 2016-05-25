using System;
using SharpDX;

namespace Noire.Common {
    public abstract class GameComponent : DisposeBase, IGameComponent, IUpdateable, IDrawable {

        protected GameComponent() {
            Name = string.Empty;
        }

        public virtual bool Enabled { get; set; } = true;

        public void Update(GameTime gameTime) {
            if (!IsInitialized) {
                Initialize();
            }
            if (Enabled) {
                UpdateInternal(gameTime);
            }
        }

        public virtual bool Visible { get; set; } = true;

        public void Draw(GameTime gameTime) {
            if (Visible) {
                DrawInternal(gameTime);
            }
        }

        public bool IsInitialized => _isInitialized;

        public void Initialize() {
            if (!_isInitialized) {
                InitializeInternal();
                _isInitialized = true;
            }
        }

        public int UpdateOrder { get; set; }
        public int DrawOrder { get; set; }

        public string Name { get; set; }

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> EnabledChanged;

        protected override void Dispose(bool disposing) {
        }

        protected virtual void UpdateInternal(GameTime gameTime) {
        }

        protected virtual void DrawInternal(GameTime gameTime) {
        }

        protected virtual void InitializeInternal() {
        }

        protected internal virtual void RaiseSurfaceInvalidated(object sender, EventArgs e) {
            OnSurfaceInvalidated(sender, e);
        }

        protected virtual void OnSurfaceInvalidated(object sender, EventArgs e) {
        }

        private bool _isInitialized;

    }
}
