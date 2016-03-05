using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D9;

namespace Noire.Graphics {

    public abstract class DisplayObject : IDisposable {

        public DisplayObject(RenderManager manager) {
            _manager = manager;
        }

        public void Render(RenderTarget target) {
            if (Visible) {
                RenderInternal(target);
            }
        }

        public void Update(RenderTarget target) {
            if (Enabled) {
                UpdateInternal(target);
            }
        }

        protected abstract void RenderInternal(RenderTarget target);

        protected abstract void UpdateInternal(RenderTarget target);

        public virtual void Initialize() {
        }

        /// <summary>
        /// 执行与释放或重置非托管资源关联的应用程序定义的任务。
        /// </summary>
        public virtual void Dispose() {
            _manager = null;
        }

        public bool Visible { get; set; } = true;

        public bool Enabled { get; set; } = true;

        protected RenderManager _manager;

    }

}
