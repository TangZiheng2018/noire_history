using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;
using Noire.Misc;

namespace Noire.Graphics.Nodes {
    public class CameraNode : Node {

        public CameraNode(SceneNode scene)
            : this(scene, 0) {
        }

        public CameraNode(SceneNode runtime, int adapter)
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
            if (Scene.CurrentCamera == this) {
                Scene.CurrentCamera = null;
            }
            NoireUtilities.SafeDispose(ref _device);
            base.Dispose();
        }

        protected override void RenderBeforeChildren() {
            Scene.CurrentCamera = this;
            _device?.BeginScene();
            // depth: http://www.gamedev.net/page/resources/_/technical/graphics-programming-and-theory/perspective-projections-in-lh-and-rh-systems-r3598
            _device?.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.MidnightBlue, 1, 0);

            _originalViewMatrix = (_device?.GetTransform(TransformState.View)).GetValueOrDefault();
            var viewMatrix = Matrix.LookAtLH(Eye, LookAt, Up);
            _device?.SetTransform(TransformState.View, viewMatrix);
        }

        protected override void RenderAfterChildren() {
            _device?.SetTransform(TransformState.View, _originalViewMatrix);
            _device?.EndScene();
            _device?.Present();
        }

        public Vector3 Eye { get; set; } = new Vector3(0, -1, 0);

        public Vector3 LookAt { get; set; } = new Vector3(0, 0, 0);

        public Vector3 Up { get; set; } = new Vector3(0, 0, 1);

        public Device Device => _device;

        private Matrix _originalViewMatrix;

        private Device _device;

    }
}
