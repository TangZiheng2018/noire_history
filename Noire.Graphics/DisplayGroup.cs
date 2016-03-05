using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Graphics.Misc;
using SharpDX.Direct3D9;

namespace Noire.Graphics {

    public abstract class DisplayGroup : DisplayObject {

        public DisplayGroup(RenderManager manager)
            : base(manager) {
            _children = new SafeList<DisplayObject>();
        }

        public SafeList<DisplayObject> Children => _children;

        protected override void RenderInternal(RenderTarget target) {
            foreach (var child in Children) {
                child.Render(target);
            }
        }

        protected override void UpdateInternal(RenderTarget target) {
            foreach (var child in Children) {
                child.Update(target);
            }
        }
        
        public override void Dispose() {
            if (_children.Count > 0) {
                for (var i = 0; i < _children.Count; ++i) {
                    _children[i].Dispose();
                }
                _children.Clear();
            }
            base.Dispose();
        }

        protected SafeList<DisplayObject> _children;

    }

}
