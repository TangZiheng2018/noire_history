using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;

namespace Noire.Graphics.Nodes {
    public class TransformNode : Node {

        public TransformNode(SceneNode scene)
            : base(scene) {
            Transform = new Matrix();
        }

        public Matrix Transform { get; set; }

        protected override void RenderBeforeChildren() {
            var device = Scene.CurrentDevice;
            if (device != null) {
                _originalWorldMatrix = device.GetTransform(TransformState.World);
                device.SetTransform(TransformState.World, Transform);
            }
        }

        protected override void RenderAfterChildren() {
            Scene.CurrentDevice?.SetTransform(TransformState.World, _originalWorldMatrix);
        }

        private Matrix _originalWorldMatrix;

    }
}
