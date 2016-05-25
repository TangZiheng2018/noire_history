using System.Diagnostics;
using System.IO;
using System.Reflection;
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
            effect = CreateEffect<BuildShadowMapEffect11>(device);
            SafeEffectRegister(ref effect);
            effect = CreateEffect<SsaoNormalDepthEffect11>(device);
            SafeEffectRegister(ref effect);
            effect = CreateEffect<DebugTextureEffect11>(device);
            SafeEffectRegister(ref effect);
            effect = CreateEffect<FireParticleEffect11>(device);
            SafeEffectRegister(ref effect);
            effect = CreateEffect<RainParticleEffect11>(device);
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
            try {
                var r = constructor.Invoke(new object[] { device, fileName });
                var effect = r as T;
                effect?.Compile();
                return effect;
            } catch (FileNotFoundException ex) {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

    }
}
