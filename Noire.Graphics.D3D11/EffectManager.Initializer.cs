using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Graphics.D3D11.FX;
using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11 {
    public sealed partial class EffectManager {

        public void InitializeAllEffects(Device device) {
            EffectBase effect = null;

            effect = new BasicEffect(device);
            SafeEffectRegister(ref effect);
        }

        private static void SafeEffectRegister(ref EffectBase effect) {
            var b = effect?.RegisterEffect();
            if (!b.HasValue || !b.Value) {
                Utilities.Dispose(ref effect);
            }
        }

    }
}
