using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;

namespace Noire.Graphics.Nodes {
    public class PerspectiveProjectionNode : Node {

        public PerspectiveProjectionNode(SceneNode scene)
            : base(scene) {
        }

        private static Matrix PerspectiveWorkaround(float fov, float aspect, float near, float far) {
            var m = new Matrix();
            m.M11 = (float)(1 / Math.Tan(fov * 0.5)) / aspect;
            m.M22 = (float)(1 / Math.Tan(fov * 0.5));
            m.M33 = far / (far - near);
            m.M34 = 1;
            m.M43 = far * near / (near - far);
            return m;
        }

        protected override void RenderBeforeChildren()
        {
            var device = Scene.CurrentDevice;
            if (device != null)
            {
                _originalProjectionMatrix = device.GetTransform(TransformState.Projection);
                var clientSize = Scene.Control.ClientSize;
                var projectionMatrix = PerspectiveWorkaround(MathUtil.DegreesToRadians(FieldOfViewDeg), (float) clientSize.Width/clientSize.Height, NearPlane, FarPlane);
                device.SetTransform(TransformState.Projection, projectionMatrix);
            }
        }

        protected override void RenderAfterChildren() {
            Scene.CurrentDevice?.SetTransform(TransformState.Projection, _originalProjectionMatrix);
        }

        public float FieldOfViewDeg { get; set; } = 45;

        public float NearPlane { get; set; } = 1;

        public float FarPlane { get; set; } = 1000;

        private Matrix _originalProjectionMatrix;

    }
}
