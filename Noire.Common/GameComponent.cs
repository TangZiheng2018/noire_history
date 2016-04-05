using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Noire.Common {
    public abstract class GameComponent : DisposeBase, IGameComponent, IUpdateable, IDrawable {

        public virtual bool Enabled { get; set; } = true;

        public void Update(GameTime gameTime) {
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

        public abstract void Initialize();

        public int UpdateOrder { get; set; }
        public int DrawOrder { get; set; }

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

    }
}
