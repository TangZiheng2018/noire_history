using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Noire.Common;
using Noire.Graphics.D3D11.FX;
using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11 {
    public sealed partial class EffectManager11 {

        public void InitializeAllEffects(Device device) {
            EffectBase11 effect = null;

            effect = CreateEffect<BasicEffect11>(device);
            SafeEffectRegister(ref effect);
            effect = CreateEffect<SkyboxEffect11>(device);
            SafeEffectRegister(ref effect);
            effect = CreateEffect<NormalMapEffect11>(device);
            SafeEffectRegister(ref effect);
            effect = CreateEffect<DisplacementMapEffect11>(device);
            SafeEffectRegister(ref effect);
        }

        private static void SafeEffectRegister(ref EffectBase11 effect) {
            var b = effect?.RegisterEffect();
            if (!b.HasValue || !b.Value) {
                Utilities.Dispose(ref effect);
            }
        }

        private static T CreateEffect<T>(Device device) where T : EffectBase11 {
            const string fxFieldName = "FxFileName";
            var t = typeof(T);
            var fxFieldInfo = t.GetField(fxFieldName, BindingFlags.Static | BindingFlags.NonPublic);
            var fileName = (string)fxFieldInfo.GetValue(null);
            fileName = NoireConfiguration.GetFullResourcePath(fileName);
            var constructor = t.GetConstructor(new[] { typeof(Device), typeof(string) });
            var r = constructor.Invoke(new object[] { device, fileName });
            return r as T;
        }

    }
}
