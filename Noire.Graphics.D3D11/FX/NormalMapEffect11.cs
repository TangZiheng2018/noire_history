using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D11;
using Noire.Common;

namespace Noire.Graphics.D3D11.FX {
    public class NormalMapEffect11 : BasicEffect11 {

        public NormalMapEffect11(Device device, string fileName)
            : base(device, fileName) {
        }

        public void SetNormalMap(ShaderResourceView tex) {
            _normalMap.SetResource(tex);
        }

        protected override void Initialize() {
            base.Initialize();
            var fx = DxEffect;
            _normalMap = fx.GetVariableByName("gNormalMap").AsShaderResource();
        }

        private EffectShaderResourceVariable _normalMap;

        private static readonly string FxFileName = "fx/NormalMap.fx";

    }
}
