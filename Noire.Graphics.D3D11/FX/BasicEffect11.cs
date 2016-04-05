using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Common;
using SharpDX;
using SharpDX.Direct3D11;
using DirectionalLight = Noire.Common.DirectionalLight;

namespace Noire.Graphics.D3D11.FX {
    public sealed class BasicEffect11 : EffectBase11 {

        public BasicEffect11(Device device)
            : base(device, NoireConfiguration.GetFullResourcePath(FxFileName)) {
        }

        public const int MaxLights = 3;
        public const int MaxBones = 96;

        public EffectTechnique Light1Tech => _light1Tech;

        public EffectTechnique Light2Tech => _light2Tech;

        public EffectTechnique Light3Tech => _light3Tech;

        public EffectTechnique Light0TexTech => _light0TexTech;

        public EffectTechnique Light1TexTech => _light1TexTech;

        public EffectTechnique Light2TexTech => _light2TexTech;

        public EffectTechnique Light3TexTech => _light3TexTech;

        public EffectTechnique Light0TexAlphaClipTech => _light0TexAlphaClipTech;

        public EffectTechnique Light1TexAlphaClipTech => _light1TexAlphaClipTech;

        public EffectTechnique Light2TexAlphaClipTech => _light2TexAlphaClipTech;

        public EffectTechnique Light3TexAlphaClipTech => _light3TexAlphaClipTech;

        public EffectTechnique Light1FogTech => _light1FogTech;

        public EffectTechnique Light2FogTech => _light2FogTech;

        public EffectTechnique Light3FogTech => _light3FogTech;

        public EffectTechnique Light0TexFogTech => _light0TexFogTech;

        public EffectTechnique Light1TexFogTech => _light1TexFogTech;

        public EffectTechnique Light2TexFogTech => _light2TexFogTech;

        public EffectTechnique Light3TexFogTech => _light3TexFogTech;

        public EffectTechnique Light0TexAlphaClipFogTech => _light0TexAlphaClipFogTech;

        public EffectTechnique Light1TexAlphaClipFogTech => _light1TexAlphaClipFogTech;

        public EffectTechnique Light2TexAlphaClipFogTech => _light2TexAlphaClipFogTech;

        public EffectTechnique Light3TexAlphaClipFogTech => _light3TexAlphaClipFogTech;

        public EffectTechnique Light1ReflectTech => _light1ReflectTech;

        public EffectTechnique Light2ReflectTech => _light2ReflectTech;

        public EffectTechnique Light3ReflectTech => _light3ReflectTech;

        public EffectTechnique Light0TexReflectTech => _light0TexReflectTech;

        public EffectTechnique Light1TexReflectTech => _light1TexReflectTech;

        public EffectTechnique Light2TexReflectTech => _light2TexReflectTech;

        public EffectTechnique Light3TexReflectTech => _light3TexReflectTech;

        public EffectTechnique Light0TexAlphaClipReflectTech => _light0TexAlphaClipReflectTech;

        public EffectTechnique Light1TexAlphaClipReflectTech => _light1TexAlphaClipReflectTech;

        public EffectTechnique Light2TexAlphaClipReflectTech => _light2TexAlphaClipReflectTech;

        public EffectTechnique Light3TexAlphaClipReflectTech => _light3TexAlphaClipReflectTech;

        public EffectTechnique Light1FogReflectTech => _light1FogReflectTech;

        public EffectTechnique Light2FogReflectTech => _light2FogReflectTech;

        public EffectTechnique Light3FogReflectTech => _light3FogReflectTech;

        public EffectTechnique Light0TexFogReflectTech => _light0TexFogReflectTech;

        public EffectTechnique Light1TexFogReflectTech => _light1TexFogReflectTech;

        public EffectTechnique Light2TexFogReflectTech => _light2TexFogReflectTech;

        public EffectTechnique Light3TexFogReflectTech => _light3TexFogReflectTech;

        public EffectTechnique Light0TexAlphaClipFogReflectTech => _light0TexAlphaClipFogReflectTech;

        public EffectTechnique Light1TexAlphaClipFogReflectTech => _light1TexAlphaClipFogReflectTech;

        public EffectTechnique Light2TexAlphaClipFogReflectTech => _light2TexAlphaClipFogReflectTech;

        public EffectTechnique Light3TexAlphaClipFogReflectTech => _light3TexAlphaClipFogReflectTech;

        public EffectTechnique Light1SkinnedTech => _light1SkinnedTech;

        public EffectTechnique Light2SkinnedTech => _light2SkinnedTech;

        public EffectTechnique Light3SkinnedTech => _light3SkinnedTech;

        public EffectTechnique Light0TexSkinnedTech => _light0TexSkinnedTech;

        public EffectTechnique Light1TexSkinnedTech => _light1TexSkinnedTech;

        public EffectTechnique Light2TexSkinnedTech => _light2TexSkinnedTech;

        public EffectTechnique Light3TexSkinnedTech => _light3TexSkinnedTech;

        public EffectTechnique Light0TexAlphaClipSkinnedTech => _light0TexAlphaClipSkinnedTech;

        public EffectTechnique Light1TexAlphaClipSkinnedTech => _light1TexAlphaClipSkinnedTech;

        public EffectTechnique Light2TexAlphaClipSkinnedTech => _light2TexAlphaClipSkinnedTech;

        public EffectTechnique Light3TexAlphaClipSkinnedTech => _light3TexAlphaClipSkinnedTech;

        public EffectTechnique Light1FogSkinnedTech => _light1FogSkinnedTech;

        public EffectTechnique Light2FogSkinnedTech => _light2FogSkinnedTech;

        public EffectTechnique Light3FogSkinnedTech => _light3FogSkinnedTech;

        public EffectTechnique Light0TexFogSkinnedTech => _light0TexFogSkinnedTech;

        public EffectTechnique Light1TexFogSkinnedTech => _light1TexFogSkinnedTech;

        public EffectTechnique Light2TexFogSkinnedTech => _light2TexFogSkinnedTech;

        public EffectTechnique Light3TexFogSkinnedTech => _light3TexFogSkinnedTech;

        public EffectTechnique Light0TexAlphaClipFogSkinnedTech => _light0TexAlphaClipFogSkinnedTech;

        public EffectTechnique Light1TexAlphaClipFogSkinnedTech => _light1TexAlphaClipFogSkinnedTech;

        public EffectTechnique Light2TexAlphaClipFogSkinnedTech => _light2TexAlphaClipFogSkinnedTech;

        public EffectTechnique Light3TexAlphaClipFogSkinnedTech => _light3TexAlphaClipFogSkinnedTech;

        public EffectTechnique Light1ReflectSkinnedTech => _light1ReflectSkinnedTech;

        public EffectTechnique Light2ReflectSkinnedTech => _light2ReflectSkinnedTech;

        public EffectTechnique Light3ReflectSkinnedTech => _light3ReflectSkinnedTech;

        public EffectTechnique Light0TexReflectSkinnedTech => _light0TexReflectSkinnedTech;

        public EffectTechnique Light1TexReflectSkinnedTech => _light1TexReflectSkinnedTech;

        public EffectTechnique Light2TexReflectSkinnedTech => _light2TexReflectSkinnedTech;

        public EffectTechnique Light3TexReflectSkinnedTech => _light3TexReflectSkinnedTech;

        public EffectTechnique Light0TexAlphaClipReflectSkinnedTech => _light0TexAlphaClipReflectSkinnedTech;

        public EffectTechnique Light1TexAlphaClipReflectSkinnedTech => _light1TexAlphaClipReflectSkinnedTech;

        public EffectTechnique Light2TexAlphaClipReflectSkinnedTech => _light2TexAlphaClipReflectSkinnedTech;

        public EffectTechnique Light3TexAlphaClipReflectSkinnedTech => _light3TexAlphaClipReflectSkinnedTech;

        public EffectTechnique Light1FogReflectSkinnedTech => _light1FogReflectSkinnedTech;

        public EffectTechnique Light2FogReflectSkinnedTech => _light2FogReflectSkinnedTech;

        public EffectTechnique Light3FogReflectSkinnedTech => _light3FogReflectSkinnedTech;

        public EffectTechnique Light0TexFogReflectSkinnedTech => _light0TexFogReflectSkinnedTech;

        public EffectTechnique Light1TexFogReflectSkinnedTech => _light1TexFogReflectSkinnedTech;

        public EffectTechnique Light2TexFogReflectSkinnedTech => _light2TexFogReflectSkinnedTech;

        public EffectTechnique Light3TexFogReflectSkinnedTech => _light3TexFogReflectSkinnedTech;

        public EffectTechnique Light0TexAlphaClipFogReflectSkinnedTech => _light0TexAlphaClipFogReflectSkinnedTech;

        public EffectTechnique Light1TexAlphaClipFogReflectSkinnedTech => _light1TexAlphaClipFogReflectSkinnedTech;

        public EffectTechnique Light2TexAlphaClipFogReflectSkinnedTech => _light2TexAlphaClipFogReflectSkinnedTech;

        public EffectTechnique Light3TexAlphaClipFogReflectSkinnedTech => _light3TexAlphaClipFogReflectSkinnedTech;

        protected override void Initialize() {
            var effect = DxEffect;

            _light1Tech = effect.GetTechniqueByName("Light1");
            _light2Tech = effect.GetTechniqueByName("Light2");
            _light3Tech = effect.GetTechniqueByName("Light3");

            _light0TexTech = effect.GetTechniqueByName("Light0Tex");
            _light1TexTech = effect.GetTechniqueByName("Light1Tex");
            _light2TexTech = effect.GetTechniqueByName("Light2Tex");
            _light3TexTech = effect.GetTechniqueByName("Light3Tex");

            _light0TexAlphaClipTech = effect.GetTechniqueByName("Light0TexAlphaClip");
            _light1TexAlphaClipTech = effect.GetTechniqueByName("Light1TexAlphaClip");
            _light2TexAlphaClipTech = effect.GetTechniqueByName("Light2TexAlphaClip");
            _light3TexAlphaClipTech = effect.GetTechniqueByName("Light3TexAlphaClip");

            _light1FogTech = effect.GetTechniqueByName("Light1Fog");
            _light2FogTech = effect.GetTechniqueByName("Light2Fog");
            _light3FogTech = effect.GetTechniqueByName("Light3Fog");

            _light0TexFogTech = effect.GetTechniqueByName("Light0TexFog");
            _light1TexFogTech = effect.GetTechniqueByName("Light1TexFog");
            _light2TexFogTech = effect.GetTechniqueByName("Light2TexFog");
            _light3TexFogTech = effect.GetTechniqueByName("Light3TexFog");

            _light0TexAlphaClipFogTech = effect.GetTechniqueByName("Light0TexAlphaClipFog");
            _light1TexAlphaClipFogTech = effect.GetTechniqueByName("Light1TexAlphaClipFog");
            _light2TexAlphaClipFogTech = effect.GetTechniqueByName("Light2TexAlphaClipFog");
            _light3TexAlphaClipFogTech = effect.GetTechniqueByName("Light3TexAlphaClipFog");

            _light1ReflectTech = effect.GetTechniqueByName("Light1Reflect");
            _light2ReflectTech = effect.GetTechniqueByName("Light2Reflect");
            _light3ReflectTech = effect.GetTechniqueByName("Light3Reflect");

            _light0TexReflectTech = effect.GetTechniqueByName("Light0TexReflect");
            _light1TexReflectTech = effect.GetTechniqueByName("Light1TexReflect");
            _light2TexReflectTech = effect.GetTechniqueByName("Light2TexReflect");
            _light3TexReflectTech = effect.GetTechniqueByName("Light3TexReflect");

            _light0TexAlphaClipReflectTech = effect.GetTechniqueByName("Light0TexAlphaClipReflect");
            _light1TexAlphaClipReflectTech = effect.GetTechniqueByName("Light1TexAlphaClipReflect");
            _light2TexAlphaClipReflectTech = effect.GetTechniqueByName("Light2TexAlphaClipReflect");
            _light3TexAlphaClipReflectTech = effect.GetTechniqueByName("Light3TexAlphaClipReflect");

            _light1FogReflectTech = effect.GetTechniqueByName("Light1FogReflect");
            _light2FogReflectTech = effect.GetTechniqueByName("Light2FogReflect");
            _light3FogReflectTech = effect.GetTechniqueByName("Light3FogReflect");

            _light0TexFogReflectTech = effect.GetTechniqueByName("Light0TexFogReflect");
            _light1TexFogReflectTech = effect.GetTechniqueByName("Light1TexFogReflect");
            _light2TexFogReflectTech = effect.GetTechniqueByName("Light2TexFogReflect");
            _light3TexFogReflectTech = effect.GetTechniqueByName("Light3TexFogReflect");

            _light0TexAlphaClipFogReflectTech = effect.GetTechniqueByName("Light0TexAlphaClipFogReflect");
            _light1TexAlphaClipFogReflectTech = effect.GetTechniqueByName("Light1TexAlphaClipFogReflect");
            _light2TexAlphaClipFogReflectTech = effect.GetTechniqueByName("Light2TexAlphaClipFogReflect");
            _light3TexAlphaClipFogReflectTech = effect.GetTechniqueByName("Light3TexAlphaClipFogReflect");

            // skinned techs
            _light1SkinnedTech = effect.GetTechniqueByName("Light1Skinned");
            _light2SkinnedTech = effect.GetTechniqueByName("Light2Skinned");
            _light3SkinnedTech = effect.GetTechniqueByName("Light3Skinned");

            _light0TexSkinnedTech = effect.GetTechniqueByName("Light0TexSkinned");
            _light1TexSkinnedTech = effect.GetTechniqueByName("Light1TexSkinned");
            _light2TexSkinnedTech = effect.GetTechniqueByName("Light2TexSkinned");
            _light3TexSkinnedTech = effect.GetTechniqueByName("Light3TexSkinned");

            _light0TexAlphaClipSkinnedTech = effect.GetTechniqueByName("Light0TexAlphaClipSkinned");
            _light1TexAlphaClipSkinnedTech = effect.GetTechniqueByName("Light1TexAlphaClipSkinned");
            _light2TexAlphaClipSkinnedTech = effect.GetTechniqueByName("Light2TexAlphaClipSkinned");
            _light3TexAlphaClipSkinnedTech = effect.GetTechniqueByName("Light3TexAlphaClipSkinned");

            _light1FogSkinnedTech = effect.GetTechniqueByName("Light1FogSkinned");
            _light2FogSkinnedTech = effect.GetTechniqueByName("Light2FogSkinned");
            _light3FogSkinnedTech = effect.GetTechniqueByName("Light3FogSkinned");

            _light0TexFogSkinnedTech = effect.GetTechniqueByName("Light0TexFogSkinned");
            _light1TexFogSkinnedTech = effect.GetTechniqueByName("Light1TexFogSkinned");
            _light2TexFogSkinnedTech = effect.GetTechniqueByName("Light2TexFogSkinned");
            _light3TexFogSkinnedTech = effect.GetTechniqueByName("Light3TexFogSkinned");

            _light0TexAlphaClipFogSkinnedTech = effect.GetTechniqueByName("Light0TexAlphaClipFogSkinned");
            _light1TexAlphaClipFogSkinnedTech = effect.GetTechniqueByName("Light1TexAlphaClipFogSkinned");
            _light2TexAlphaClipFogSkinnedTech = effect.GetTechniqueByName("Light2TexAlphaClipFogSkinned");
            _light3TexAlphaClipFogSkinnedTech = effect.GetTechniqueByName("Light3TexAlphaClipFogSkinned");

            _light1ReflectSkinnedTech = effect.GetTechniqueByName("Light1ReflectSkinned");
            _light2ReflectSkinnedTech = effect.GetTechniqueByName("Light2ReflectSkinned");
            _light3ReflectSkinnedTech = effect.GetTechniqueByName("Light3ReflectSkinned");

            _light0TexReflectSkinnedTech = effect.GetTechniqueByName("Light0TexReflectSkinned");
            _light1TexReflectSkinnedTech = effect.GetTechniqueByName("Light1TexReflectSkinned");
            _light2TexReflectSkinnedTech = effect.GetTechniqueByName("Light2TexReflectSkinned");
            _light3TexReflectSkinnedTech = effect.GetTechniqueByName("Light3TexReflectSkinned");

            _light0TexAlphaClipReflectSkinnedTech = effect.GetTechniqueByName("Light0TexAlphaClipReflectSkinned");
            _light1TexAlphaClipReflectSkinnedTech = effect.GetTechniqueByName("Light1TexAlphaClipReflectSkinned");
            _light2TexAlphaClipReflectSkinnedTech = effect.GetTechniqueByName("Light2TexAlphaClipReflectSkinned");
            _light3TexAlphaClipReflectSkinnedTech = effect.GetTechniqueByName("Light3TexAlphaClipReflectSkinned");

            _light1FogReflectSkinnedTech = effect.GetTechniqueByName("Light1FogReflectSkinned");
            _light2FogReflectSkinnedTech = effect.GetTechniqueByName("Light2FogReflectSkinned");
            _light3FogReflectSkinnedTech = effect.GetTechniqueByName("Light3FogReflectSkinned");

            _light0TexFogReflectSkinnedTech = effect.GetTechniqueByName("Light0TexFogReflectSkinned");
            _light1TexFogReflectSkinnedTech = effect.GetTechniqueByName("Light1TexFogReflectSkinned");
            _light2TexFogReflectSkinnedTech = effect.GetTechniqueByName("Light2TexFogReflectSkinned");
            _light3TexFogReflectSkinnedTech = effect.GetTechniqueByName("Light3TexFogReflectSkinned");

            _light0TexAlphaClipFogReflectSkinnedTech = effect.GetTechniqueByName("Light0TexAlphaClipFogReflectSkinned");
            _light1TexAlphaClipFogReflectSkinnedTech = effect.GetTechniqueByName("Light1TexAlphaClipFogReflectSkinned");
            _light2TexAlphaClipFogReflectSkinnedTech = effect.GetTechniqueByName("Light2TexAlphaClipFogReflectSkinned");
            _light3TexAlphaClipFogReflectSkinnedTech = effect.GetTechniqueByName("Light3TexAlphaClipFogReflectSkinned");

            _worldViewProj = effect.GetVariableByName("gWorldViewProj").AsMatrix();
            _world = effect.GetVariableByName("gWorld").AsMatrix();
            _worldInvTranspose = effect.GetVariableByName("gWorldInvTranspose").AsMatrix();
            _texTransform = effect.GetVariableByName("gTexTransform").AsMatrix();
            _eyePosW = effect.GetVariableByName("gEyePosW").AsVector();

            _fogColor = effect.GetVariableByName("gFogColor").AsVector();
            _fogStart = effect.GetVariableByName("gFogStart").AsScalar();
            _fogRange = effect.GetVariableByName("gFogRange").AsScalar();

            _dirLights = effect.GetVariableByName("gDirLights");
            _mat = effect.GetVariableByName("gMaterial");
            _diffuseMap = effect.GetVariableByName("gDiffuseMap").AsShaderResource();
            _shadowMap = effect.GetVariableByName("gShadowMap").AsShaderResource();
            _cubeMap = effect.GetVariableByName("gCubeMap").AsShaderResource();

            _boneTransforms = effect.GetVariableByName("gBoneTransforms").AsMatrix();

            _shadowTransform = effect.GetVariableByName("gShadowTransform").AsMatrix();

            _ssaoMap = effect.GetVariableByName("gSsaoMap").AsShaderResource();

            _worldViewProjTex = effect.GetVariableByName("gWorldViewProjTex").AsMatrix();
        }

        private EffectTechnique _light1Tech;
        private EffectTechnique _light2Tech;
        private EffectTechnique _light3Tech;

        private EffectTechnique _light0TexTech;
        private EffectTechnique _light1TexTech;
        private EffectTechnique _light2TexTech;
        private EffectTechnique _light3TexTech;

        private EffectTechnique _light0TexAlphaClipTech;
        private EffectTechnique _light1TexAlphaClipTech;
        private EffectTechnique _light2TexAlphaClipTech;
        private EffectTechnique _light3TexAlphaClipTech;

        private EffectTechnique _light1FogTech;
        private EffectTechnique _light2FogTech;
        private EffectTechnique _light3FogTech;

        private EffectTechnique _light0TexFogTech;
        private EffectTechnique _light1TexFogTech;
        private EffectTechnique _light2TexFogTech;
        private EffectTechnique _light3TexFogTech;

        private EffectTechnique _light0TexAlphaClipFogTech;
        private EffectTechnique _light1TexAlphaClipFogTech;
        private EffectTechnique _light2TexAlphaClipFogTech;
        private EffectTechnique _light3TexAlphaClipFogTech;

        private EffectTechnique _light1ReflectTech;
        private EffectTechnique _light2ReflectTech;
        private EffectTechnique _light3ReflectTech;

        private EffectTechnique _light0TexReflectTech;
        private EffectTechnique _light1TexReflectTech;
        private EffectTechnique _light2TexReflectTech;
        private EffectTechnique _light3TexReflectTech;

        private EffectTechnique _light0TexAlphaClipReflectTech;
        private EffectTechnique _light1TexAlphaClipReflectTech;
        private EffectTechnique _light2TexAlphaClipReflectTech;
        private EffectTechnique _light3TexAlphaClipReflectTech;

        private EffectTechnique _light1FogReflectTech;
        private EffectTechnique _light2FogReflectTech;
        private EffectTechnique _light3FogReflectTech;

        private EffectTechnique _light0TexFogReflectTech;
        private EffectTechnique _light1TexFogReflectTech;
        private EffectTechnique _light2TexFogReflectTech;
        private EffectTechnique _light3TexFogReflectTech;

        private EffectTechnique _light0TexAlphaClipFogReflectTech;
        private EffectTechnique _light1TexAlphaClipFogReflectTech;
        private EffectTechnique _light2TexAlphaClipFogReflectTech;
        private EffectTechnique _light3TexAlphaClipFogReflectTech;

        private EffectTechnique _light1SkinnedTech;
        private EffectTechnique _light2SkinnedTech;
        private EffectTechnique _light3SkinnedTech;

        private EffectTechnique _light0TexSkinnedTech;
        private EffectTechnique _light1TexSkinnedTech;
        private EffectTechnique _light2TexSkinnedTech;
        private EffectTechnique _light3TexSkinnedTech;

        private EffectTechnique _light0TexAlphaClipSkinnedTech;
        private EffectTechnique _light1TexAlphaClipSkinnedTech;
        private EffectTechnique _light2TexAlphaClipSkinnedTech;
        private EffectTechnique _light3TexAlphaClipSkinnedTech;

        private EffectTechnique _light1FogSkinnedTech;
        private EffectTechnique _light2FogSkinnedTech;
        private EffectTechnique _light3FogSkinnedTech;

        private EffectTechnique _light0TexFogSkinnedTech;
        private EffectTechnique _light1TexFogSkinnedTech;
        private EffectTechnique _light2TexFogSkinnedTech;
        private EffectTechnique _light3TexFogSkinnedTech;

        private EffectTechnique _light0TexAlphaClipFogSkinnedTech;
        private EffectTechnique _light1TexAlphaClipFogSkinnedTech;
        private EffectTechnique _light2TexAlphaClipFogSkinnedTech;
        private EffectTechnique _light3TexAlphaClipFogSkinnedTech;

        private EffectTechnique _light1ReflectSkinnedTech;
        private EffectTechnique _light2ReflectSkinnedTech;
        private EffectTechnique _light3ReflectSkinnedTech;

        private EffectTechnique _light0TexReflectSkinnedTech;
        private EffectTechnique _light1TexReflectSkinnedTech;
        private EffectTechnique _light2TexReflectSkinnedTech;
        private EffectTechnique _light3TexReflectSkinnedTech;

        private EffectTechnique _light0TexAlphaClipReflectSkinnedTech;
        private EffectTechnique _light1TexAlphaClipReflectSkinnedTech;
        private EffectTechnique _light2TexAlphaClipReflectSkinnedTech;
        private EffectTechnique _light3TexAlphaClipReflectSkinnedTech;

        private EffectTechnique _light1FogReflectSkinnedTech;
        private EffectTechnique _light2FogReflectSkinnedTech;
        private EffectTechnique _light3FogReflectSkinnedTech;

        private EffectTechnique _light0TexFogReflectSkinnedTech;
        private EffectTechnique _light1TexFogReflectSkinnedTech;
        private EffectTechnique _light2TexFogReflectSkinnedTech;
        private EffectTechnique _light3TexFogReflectSkinnedTech;

        private EffectTechnique _light0TexAlphaClipFogReflectSkinnedTech;
        private EffectTechnique _light1TexAlphaClipFogReflectSkinnedTech;
        private EffectTechnique _light2TexAlphaClipFogReflectSkinnedTech;
        private EffectTechnique _light3TexAlphaClipFogReflectSkinnedTech;

        private EffectMatrixVariable _worldViewProj;
        private EffectMatrixVariable _world;
        private EffectMatrixVariable _worldInvTranspose;
        private EffectMatrixVariable _texTransform;
        private EffectMatrixVariable _shadowTransform;
        private EffectVectorVariable _eyePosW;
        private EffectVectorVariable _fogColor;
        private EffectScalarVariable _fogStart;
        private EffectScalarVariable _fogRange;

        private EffectVariable _dirLights;
        private byte[] _dirLightsArray = new byte[DirectionalLight.Stride * MaxLights];

        private EffectMatrixVariable _boneTransforms;
        private Matrix[] _boneTransformsArray = new Matrix[MaxBones];

        private EffectVariable _mat;

        private EffectShaderResourceVariable _diffuseMap;
        private EffectShaderResourceVariable _shadowMap;
        private EffectShaderResourceVariable _cubeMap;

        private EffectShaderResourceVariable _ssaoMap;
        private EffectMatrixVariable _worldViewProjTex;

        private static readonly string FxFileName = "fx/Basic.fx";

    }
}
