using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11.FX {
    public abstract class ParticleEffectBase11 : EffectBase11 {

        protected ParticleEffectBase11(Device device, string filename)
            : base(device, filename) {
        }

        protected override void Initialize() {
            var fx = DxEffect;

            _streamOutTech = fx.GetTechniqueByName("StreamOutTech");
            _drawTech = fx.GetTechniqueByName("DrawTech");

            _viewProj = fx.GetVariableByName("gViewProj").AsMatrix();
            _gameTime = fx.GetVariableByName("gGameTime").AsScalar();
            _timeStep = fx.GetVariableByName("gTimeStep").AsScalar();

            _eyePosW = fx.GetVariableByName("gEyePosW").AsVector();
            _emitPosW = fx.GetVariableByName("gEmitPosW").AsVector();
            _emitDirW = fx.GetVariableByName("gEmitDirW").AsVector();

            _texArray = fx.GetVariableByName("gTexArray").AsShaderResource();
            _randomTex = fx.GetVariableByName("gRandomTex").AsShaderResource();
        }

        public EffectTechnique DrawTech => _drawTech;

        public EffectTechnique StreamOutTech => _streamOutTech;

        public void SetViewProj(Matrix m) {
            _viewProj.SetMatrix(m);
        }

        public void SetGameTime(float f) {
            _gameTime.Set(f);
        }

        public void SetTimeStep(float f) {
            _timeStep.Set(f);
        }

        public void SetEyePosW(Vector3 v) {
            _eyePosW.Set(v);
        }

        public void SetEmitPosW(Vector3 v) {
            _emitPosW.Set(v);
        }

        public void SetEmitDirW(Vector3 v) {
            _emitDirW.Set(v);
        }

        public void SetTexArray(ShaderResourceView tex) {
            _texArray.SetResource(tex);
        }

        public void SetRandomTex(ShaderResourceView tex) {
            _randomTex.SetResource(tex);
        }

        private EffectTechnique _streamOutTech;
        private EffectTechnique _drawTech;

        private EffectMatrixVariable _viewProj;
        private EffectScalarVariable _timeStep;
        private EffectScalarVariable _gameTime;
        private EffectVectorVariable _eyePosW;
        private EffectVectorVariable _emitPosW;
        private EffectVectorVariable _emitDirW;
        private EffectShaderResourceVariable _texArray;
        private EffectShaderResourceVariable _randomTex;

    }
}
