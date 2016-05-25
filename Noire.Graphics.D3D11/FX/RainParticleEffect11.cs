using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11.FX {
    public sealed class RainParticleEffect11 :ParticleEffectBase11 {

        public RainParticleEffect11(Device device, string filename) 
            : base(device, filename) {
        }

        private static readonly string FxFileName = "fx/Particle-Rain.fx";

    }
}
