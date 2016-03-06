using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;

namespace Noire.Graphics.Nodes {
    public class TransformNode : Node {

        public TransformNode(Direct3DRuntime runtime)
            : base(runtime, false) {
            Transform = new Matrix();
        }

        public Matrix Transform { get; set; }

        protected override void RenderA() {
            _originalWorldMatrix = (D3DRuntime.CurrentCamera?.Device.GetTransform(TransformState.World)).GetValueOrDefault();
            D3DRuntime.CurrentCamera?.Device?.SetTransform(TransformState.World, Transform);
        }

        protected override void RenderB() {
            D3DRuntime.CurrentCamera?.Device?.SetTransform(TransformState.World, _originalWorldMatrix);
        }

        private Matrix _originalWorldMatrix;

    }
}
