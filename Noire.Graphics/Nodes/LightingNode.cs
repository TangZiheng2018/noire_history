using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D9;

namespace Noire.Graphics.Nodes {
    public class LightingNode : Node {

        public LightingNode(Direct3DRuntime runtime)
            : base(runtime, false) {
        }

        public bool Lighting { get; set; }

        protected override void RenderA() {
            _originalLighting = (D3DRuntime.CurrentDevice?.Device?.GetRenderState<bool>(RenderState.Lighting)).GetValueOrDefault();
            D3DRuntime.CurrentDevice?.Device?.SetRenderState(RenderState.Lighting, Lighting);
        }

        protected override void RenderB() {
            D3DRuntime.CurrentDevice?.Device?.SetRenderState(RenderState.Lighting, _originalLighting);
        }

        private bool _originalLighting;

    }
}
