using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;

namespace Noire.Graphics.Nodes {
    public class CameraNode : Node {

        public CameraNode(Direct3DRuntime runtime)
            : base(runtime, false) {
        }

        protected override void RenderA() {
            _originalViewMatrix = (D3DRuntime.CurrentDevice?.Device?.GetTransform(TransformState.View)).GetValueOrDefault();
            var viewMatrix = Matrix.LookAtLH(Eye, LookAt, Up);
            D3DRuntime.CurrentDevice?.Device?.SetTransform(TransformState.View, viewMatrix);
        }

        protected override void RenderB() {
            D3DRuntime.CurrentDevice?.Device?.SetTransform(TransformState.View, _originalViewMatrix);
        }

        public Vector3 Eye { get; set; } = new Vector3(0, -1, 0);

        public Vector3 LookAt { get; set; } = new Vector3(0, 0, 0);

        public Vector3 Up { get; set; } = new Vector3(0, 0, 1);

        private Matrix _originalViewMatrix;

    }
}
