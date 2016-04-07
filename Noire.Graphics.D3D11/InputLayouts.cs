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
            EffectPassDescription? passDesc;
            var e1 = EffectManager11.Instance.GetEffect<BasicEffect11>();
            passDesc = e1?.Light1Tech?.GetPassByIndex(0)?.Description;
            try {
                if (passDesc?.Signature != null) {
                    _positionNormal = new InputLayout(device, passDesc.Value.Signature, InputLayoutDescriptions.PositionNormal);
                }
            } catch (SharpDXException ex) {
                Debug.Print(ex.Message);
            }
            try {
                if (passDesc?.Signature != null) {
                    _positionNormalTC = new InputLayout(device, passDesc.Value.Signature, InputLayoutDescriptions.PositionNormalTC);
                }
            } catch (SharpDXException ex) {
                Debug.Print(ex.Message);
            }
            var e2 = EffectManager11.Instance.GetEffect<SkyboxEffect11>();
            passDesc = e2?.SkyTech?.GetPassByIndex(0)?.Description;
            if (passDesc?.Signature != null) {
                _position = new InputLayout(device, passDesc.Value.Signature, InputLayoutDescriptions.Position);
            }
        }

        public static void DisposeAll() {
            Utilities.Dispose(ref _positionNormal);
            Utilities.Dispose(ref _position);
            Utilities.Dispose(ref _positionNormalTC);
        }

        public static InputLayout PositionNormal => _positionNormal;

        public static InputLayout Position => _position;

        public static InputLayout PositionNormalTC => _positionNormalTC;

        private static InputLayout _positionNormal;
        private static InputLayout _position;
        private static InputLayout _positionNormalTC;

    }
}
