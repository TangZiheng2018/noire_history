using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Common;
using SharpDX.Direct3D11;
using SharpDX;

namespace Noire.Graphics.D3D11.FX {
    public sealed class DisplacementMapEffect11 : NormalMapEffect11 {

        public DisplacementMapEffect11(Device device, string fileName)
            : base(device, fileName) {
        }

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

        protected override void Initialize() {
            base.Initialize();
            var fx = DxEffect;
            _heightScale = fx.GetVariableByName("gHeightScale").AsScalar();
            _maxTessDistance = fx.GetVariableByName("gMaxTessDistance").AsScalar();
            _minTessDistance = fx.GetVariableByName("gMinTessDistance").AsScalar();
            _minTessFactor = fx.GetVariableByName("gMinTessFactor").AsScalar();
            _maxTessFactor = fx.GetVariableByName("gMaxTessFactor").AsScalar();
            _viewProj = fx.GetVariableByName("gViewProj").AsMatrix();
        }

        private EffectScalarVariable _heightScale;
        private EffectScalarVariable _maxTessDistance;
        private EffectScalarVariable _minTessDistance;
        private EffectScalarVariable _minTessFactor;
        private EffectScalarVariable _maxTessFactor;
        private EffectMatrixVariable _viewProj;

        private static readonly string FxFileName = "fx/DisplacementMap.fx";

    }
}
