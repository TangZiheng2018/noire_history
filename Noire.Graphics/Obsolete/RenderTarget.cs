using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;

namespace Noire.Graphics.Obsolete {

    public class RenderTarget : IDisposable {

        internal RenderTarget(RenderManager manager, int adapter) {
            CreateFlags createFlags = CreateFlags.HardwareVertexProcessing;

            PresentParameters pp = new PresentParameters(manager.Control.Width, manager.Control.Height);
            pp.PresentationInterval = PresentInterval.Default;
            pp.SwapEffect = SwapEffect.Discard;
            pp.Windowed = true;
            pp.DeviceWindowHandle = manager.Control.Handle;
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = Format.D16;

            _device = new Device(manager.Direct3D, adapter, DeviceType.Hardware, manager.Control.Handle, createFlags, pp);
            _device.SetRenderState(RenderState.Lighting, false);
            _device.SetRenderState(RenderState.CullMode, Cull.None);
        }

        public void Clear() {
            if (_device == null) {
                return;
            }
            Color screenColor = Color.MidnightBlue;
            // depth: http://www.gamedev.net/page/resources/_/technical/graphics-programming-and-theory/perspective-projections-in-lh-and-rh-systems-r3598
            _device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, screenColor, 1, 0);
        }

        public void Present() {
            if (_device == null) {
                return;
            }
            _device.Present();
        }

        /// <summary>
        /// 执行与释放或重置非托管资源关联的应用程序定义的任务。
        /// </summary>
        public virtual void Dispose() {
            NoireUtilities.SafeDispose(ref _device);
        }

        public Device Device => _device;

        protected Device _device;

    }

}
