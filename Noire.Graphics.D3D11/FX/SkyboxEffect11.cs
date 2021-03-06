﻿using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11.FX {
    public sealed class SkyboxEffect11 : EffectBase11 {

        public SkyboxEffect11(Device device, string fileName)
            : base(device, fileName) {
        }

        public EffectTechnique SkyTech => _skyTech;

        public void SetWorldViewProj(Matrix m) {
            _worldViewProj.SetMatrix(m);
        }

        public void SetCubeMap(ShaderResourceView cubemap) {
            _cubeMap.SetResource(cubemap);
        }

        protected override void Initialize() {
            var fx = DxEffect;
            _skyTech = fx.GetTechniqueByName("SkyTech");
            _worldViewProj = fx.GetVariableByName("gWorldViewProj").AsMatrix();
            _cubeMap = fx.GetVariableByName("gCubeMap").AsShaderResource();
        }

        private EffectMatrixVariable _worldViewProj;
        private EffectShaderResourceVariable _cubeMap;
        private EffectTechnique _skyTech;

        private static readonly string FxFileName = "fx/Skybox.fx";

    }
}
