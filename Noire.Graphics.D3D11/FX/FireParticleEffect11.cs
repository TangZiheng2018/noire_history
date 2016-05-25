using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11.FX {
    public sealed class FireParticleEffect11 : ParticleEffectBase11 {

        public FireParticleEffect11(Device device, string filename) 
            : base(device, filename) {
        }

        private static readonly string FxFileName = "fx/Particle-Fire.fx";

    }
}
