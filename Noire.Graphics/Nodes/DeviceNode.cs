using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;

namespace Noire.Graphics.Nodes {
    public class DeviceNode : Node {

        public DeviceNode(Direct3DRuntime runtime, int adapter)
            : base(runtime, false) {
            CreateFlags createFlags = CreateFlags.HardwareVertexProcessing;

            var clientSize = runtime.Control.ClientSize;
            PresentParameters pp = new PresentParameters(clientSize.Width, clientSize.Height);
            pp.PresentationInterval = PresentInterval.Default;
            pp.SwapEffect = SwapEffect.Discard;
            pp.Windowed = true;
            pp.DeviceWindowHandle = runtime.Control.Handle;
            pp.EnableAutoDepthStencil = true;
            pp.AutoDepthStencilFormat = Format.D16;

            _device = new Device(runtime.Direct3D, adapter, DeviceType.Hardware, runtime.Control.Handle, createFlags, pp);
        }

        public override void Dispose() {
            if (D3DRuntime.CurrentDevice == this) {
                D3DRuntime.CurrentDevice = null;
            }
            NoireUtilities.SafeDispose(ref _device);
            base.Dispose();
        }

        protected override void RenderA() {
            D3DRuntime.CurrentDevice = this;
            _device?.BeginScene();
            // depth: http://www.gamedev.net/page/resources/_/technical/graphics-programming-and-theory/perspective-projections-in-lh-and-rh-systems-r3598
            _device?.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.MidnightBlue, 1, 0);
        }

        protected override void RenderB() {
            _device?.EndScene();
            _device?.Present();
        }

        public Device Device => _device;

        private Device _device;

    }
}
