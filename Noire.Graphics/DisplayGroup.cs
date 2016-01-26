using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D9;

namespace Noire.Graphics
{

    public abstract class DisplayGroup : DisplayObject
    {

        public DisplayGroup(RenderManager manager)
            : base(manager)
        {
            _children = new List<DisplayObject>();
        }

        public List<DisplayObject> Children => _children;

        protected override void RenderInternal(RenderTarget target)
        {
            for (var i = 0; i < _children.Count; ++i)
            {
                _children[i].Render(target);
            }
        }

        protected override void UpdateInternal(RenderTarget target)
        {
            for (var i = 0; i < _children.Count; ++i)
            {
                _children[i].Update(target);
            }
        }

        /// <summary>
        /// 执行与释放或重置非托管资源关联的应用程序定义的任务。
        /// </summary>
        public override void Dispose()
        {
            if (_children.Count > 0)
            {
                for (var i = 0; i < _children.Count; ++i)
                {
                    _children[i].Dispose();
                }
                _children.Clear();
            }
            base.Dispose();
        }

        protected List<DisplayObject> _children;

    }

}
