using System.Diagnostics;
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
                    _posNorm = new InputLayout(device, passDesc.Value.Signature, InputLayoutDescriptions.PosNorm);
                }
            } catch (SharpDXException ex) {
                Debug.Print(ex.Message);
            }
            try {
                if (passDesc?.Signature != null) {
                    _posNormTex = new InputLayout(device, passDesc.Value.Signature, InputLayoutDescriptions.PosNormTex);
                }
            } catch (SharpDXException ex) {
                Debug.Print(ex.Message);
            }
            var e2 = EffectManager11.Instance.GetEffect<SkyboxEffect11>();
            passDesc = e2?.SkyTech?.GetPassByIndex(0)?.Description;
            if (passDesc?.Signature != null) {
                _pos = new InputLayout(device, passDesc.Value.Signature, InputLayoutDescriptions.Pos);
            }
            var e3 = EffectManager11.Instance.GetEffect<NormalMapEffect11>();
            passDesc = e3?.Light1Tech?.GetPassByIndex(0)?.Description;
            if (passDesc?.Signature != null) {
                _posNormTexTan = new InputLayout(device, passDesc.Value.Signature, InputLayoutDescriptions.PosNormTexTan);
            }
        }

        public static void DisposeAll() {
            Utilities.Dispose(ref _pos);
            Utilities.Dispose(ref _posNorm);
            Utilities.Dispose(ref _posNormTex);
            Utilities.Dispose(ref _posNormTexTan);
        }


        public static InputLayout Pos => _pos;

        public static InputLayout PosNorm => _posNorm;

        public static InputLayout PosNormTex => _posNormTex;

        public static InputLayout PosNormTexTan => _posNormTexTan;

        private static InputLayout _posNorm;
        private static InputLayout _pos;
        private static InputLayout _posNormTex;
        private static InputLayout _posNormTexTan;

    }
}
