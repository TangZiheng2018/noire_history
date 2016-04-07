using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Graphics.D3D11.FX;
using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11 {
    public sealed partial class EffectManager11 {

        public void InitializeAllEffects(Device device) {
            EffectBase11 effect = null;

            effect = new BasicEffect11(device);
            SafeEffectRegister(ref effect);
            effect = new SkyboxEffect11(device);
            SafeEffectRegister(ref effect);
        }

        private static void SafeEffectRegister(ref EffectBase11 effect) {
            var b = effect?.RegisterEffect();
            if (!b.HasValue || !b.Value) {
                Utilities.Dispose(ref effect);
            }
        }

    }
}
