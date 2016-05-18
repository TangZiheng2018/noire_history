using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11.FX {
    public sealed class SsaoNormalDepthEffect11 : EffectBase11 {

        public SsaoNormalDepthEffect11(Device device, string fileName)
           : base(device, fileName) {
        }

        public EffectTechnique NormalDepthTech {
            get { return _normalDepthTech; }
        }

        public EffectTechnique NormalDepthAlphaClipTech {
            get { return _normalDepthAlphaClipTech; }
        }

        public void SetWorldView(Matrix m) {
            _worldView.SetMatrix(m);
        }

        public void SetWorldInvTransposeView(Matrix m) {
            _worldInvTransposeView.SetMatrix(m);
        }

        public void SetWorldViewProj(Matrix m) {
            _worldViewProj.SetMatrix(m);
        }

        public void SetTexTransform(Matrix m) {
            _texTransform.SetMatrix(m);
        }

        public void SetDiffuseMap(ShaderResourceView srv) {
            _diffuseMap.SetResource(srv);
        }

        protected override void Initialize() {
            var fx = DxEffect;

            _normalDepthTech = fx.GetTechniqueByName("NormalDepth");
            _normalDepthAlphaClipTech = fx.GetTechniqueByName("NormalDepthAlphaClip");

            _worldView = fx.GetVariableByName("gWorldView").AsMatrix();
            _worldInvTransposeView = fx.GetVariableByName("gWorldInvTransposeView").AsMatrix();
            _worldViewProj = fx.GetVariableByName("gWorldViewProj").AsMatrix();
            _texTransform = fx.GetVariableByName("gTexTransform").AsMatrix();
            _diffuseMap = fx.GetVariableByName("gDiffuseMap").AsShaderResource();
        }

        private EffectMatrixVariable _worldView;
        private EffectMatrixVariable _worldInvTransposeView;
        private EffectMatrixVariable _worldViewProj;
        private EffectMatrixVariable _texTransform;

        private EffectTechnique _normalDepthTech;
        private EffectTechnique _normalDepthAlphaClipTech;

        private EffectShaderResourceVariable _diffuseMap;

        private static readonly string FxFileName = "fx/SsaoNormalDepth.fx";

    }
}
