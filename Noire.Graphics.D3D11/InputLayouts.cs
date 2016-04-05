using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Graphics.D3D11.FX;
using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11 {
    public static class InputLayouts {

        public static void InitializeAll(Device device) {
            var e1 = EffectManager11.Instance.GetEffect<BasicEffect11>();
            var passDesc = e1?.Light1Tech?.GetPassByIndex(0)?.Description;
            if (passDesc?.Signature != null) {
                _positionNormal = new InputLayout(device, passDesc.Value.Signature, InputLayoutDescriptions.PositionNormal);
            }
        }

        public static void DisposeAll() {
            Utilities.Dispose(ref _positionNormal);
        }

        public static InputLayout PositionNormal => _positionNormal;

        private static InputLayout _positionNormal;

    }
}
