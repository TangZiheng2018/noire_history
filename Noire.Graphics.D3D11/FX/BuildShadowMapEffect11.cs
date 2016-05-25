using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11.FX {
    public sealed class BuildShadowMapEffect11 : EffectBase11 {

        public BuildShadowMapEffect11(Device device, string fileName)
            : base(device, fileName) {
        }

        public EffectTechnique BuildShadowMapTech => _buildShadowMapTech;

        public EffectTechnique BuildShadowMapAlphaClipTech => _buildShadowMapAlphaClipTech;

        public EffectTechnique TessBuildShadowMapTech => _tessBuildShadowMapTech;

        public EffectTechnique TessBuildShadowMapAlphaClipTech => _tessBuildShadowMapAlphaClipTech;

        public void SetHeightScale(float f) {
            _heightScale.Set(f);
        }

        public void SetMaxTessDistance(float f) {
            _maxTessDistance.Set(f);
        }

        public void SetMinTessDistance(float f) {
            _minTessDistance.Set(f);
        }

        public void SetMinTessFactor(float f) {
            _minTessFactor.Set(f);
        }

        public void SetMaxTessFactor(float f) {
            _maxTessFactor.Set(f);
        }

        public void SetViewProj(Matrix viewProj) {
            _viewProj.SetMatrix(viewProj);
        }

        public void SetNormalMap(ShaderResourceView tex) {
            _normalMap.SetResource(tex);
        }

        public void SetTexTransform(Matrix m) {
            _texTransform.SetMatrix(m);
        }

        public void SetDiffuseMap(ShaderResourceView tex) {
            _diffuseMap.SetResource(tex);
        }

        public void SetWorldViewProj(Matrix m) {
            _worldViewProj.SetMatrix(m);
        }

        public void SetWorld(Matrix m) {
            _world.SetMatrix(m);
        }

        public void SetWorldInvTranspose(Matrix m) {
            _worldInvTranspose.SetMatrix(m);
        }
        public void SetEyePosW(Vector3 v) {
            _eyePosW.Set(v);
        }

        protected override void Initialize() {
            var fx = DxEffect;

            _buildShadowMapTech = fx.GetTechniqueByName("BuildShadowMapTech");
            _buildShadowMapAlphaClipTech = fx.GetTechniqueByName("BuildShadowMapAlphaClipTech");
            _tessBuildShadowMapTech = fx.GetTechniqueByName("TessBuildShadowMapTech");
            _tessBuildShadowMapAlphaClipTech = fx.GetTechniqueByName("TessBuildShadowMapAlphaClipTech");

            _heightScale = fx.GetVariableByName("gHeightScale").AsScalar();
            _maxTessDistance = fx.GetVariableByName("gMaxTessDistance").AsScalar();
            _minTessDistance = fx.GetVariableByName("gMinTessDistance").AsScalar();
            _minTessFactor = fx.GetVariableByName("gMinTessFactor").AsScalar();
            _maxTessFactor = fx.GetVariableByName("gMaxTessFactor").AsScalar();
            _viewProj = fx.GetVariableByName("gViewProj").AsMatrix();

            _normalMap = fx.GetVariableByName("gNormalMap").AsShaderResource();
            _diffuseMap = fx.GetVariableByName("gDiffuseMap").AsShaderResource();

            _worldViewProj = fx.GetVariableByName("gWorldViewProj").AsMatrix();
            _world = fx.GetVariableByName("gWorld").AsMatrix();
            _worldInvTranspose = fx.GetVariableByName("gWorldInvTranspose").AsMatrix();
            _texTransform = fx.GetVariableByName("gTexTransform").AsMatrix();
            _eyePosW = fx.GetVariableByName("gEyePosW").AsVector();
        }

        private EffectMatrixVariable _viewProj;
        private EffectMatrixVariable _worldViewProj;
        private EffectMatrixVariable _world;
        private EffectMatrixVariable _worldInvTranspose;
        private EffectMatrixVariable _texTransform;
        private EffectVectorVariable _eyePosW;
        private EffectScalarVariable _heightScale;
        private EffectScalarVariable _maxTessDistance;
        private EffectScalarVariable _minTessDistance;
        private EffectScalarVariable _minTessFactor;
        private EffectScalarVariable _maxTessFactor;

        private EffectTechnique _buildShadowMapTech;
        private EffectTechnique _buildShadowMapAlphaClipTech;
        private EffectTechnique _tessBuildShadowMapTech;
        private EffectTechnique _tessBuildShadowMapAlphaClipTech;

        private EffectShaderResourceVariable _diffuseMap;
        private EffectShaderResourceVariable _normalMap;

        private static readonly string FxFileName = "fx/BuildShadowMap.fx";

    }
}
