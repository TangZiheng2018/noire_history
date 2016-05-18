using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11.FX {
    public class DebugTextureEffect11 : EffectBase11 {

        public DebugTextureEffect11(Device device, string filename)
            : base(device, filename) {
        }

        public EffectTechnique ViewArgbTech => _viewArgbTech;

        public EffectTechnique ViewRedTech => _viewRedTech;

        public EffectTechnique ViewGreenTech => _viewGreenTech;

        public EffectTechnique ViewBlueTech => _viewBlueTech;

        public EffectTechnique ViewAlphaTech => _viewAlphaTech;

        public void SetWorldViewProj(Matrix m) {
            _wvp.SetMatrix(m);
        }

        public void SetTexture(ShaderResourceView tex) {
            _texture.SetResource(tex);
        }

        protected override void Initialize() {
            var fx = DxEffect;

            _viewArgbTech = fx.GetTechniqueByName("ViewArgbTech");
            _viewRedTech = fx.GetTechniqueByName("ViewRedTech");
            _viewGreenTech = fx.GetTechniqueByName("ViewGreenTech");
            _viewBlueTech = fx.GetTechniqueByName("ViewBlueTech");
            _viewAlphaTech = fx.GetTechniqueByName("ViewAlphaTech");

            _texture = fx.GetVariableByName("gTexture").AsShaderResource();
            _wvp = fx.GetVariableByName("gWorldViewProj").AsMatrix();
        }

        private EffectTechnique _viewArgbTech;
        private EffectTechnique _viewRedTech;
        private EffectTechnique _viewGreenTech;
        private EffectTechnique _viewBlueTech;
        private EffectTechnique _viewAlphaTech;

        private EffectMatrixVariable _wvp;
        private EffectShaderResourceVariable _texture;

        private static readonly string FxFileName = "fx/DebugTexture.fx";

    }
}
