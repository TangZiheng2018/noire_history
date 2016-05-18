using Noire.Common;
using Noire.Common.Vertices;
using Noire.Graphics.D3D11.FX;
using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11.Model {
    public class BasicModelInstance : ModelInstanceBase<VertPosNormTexTan> {

        public BasicModelInstance(BasicModel model)
            : base(model) {
        }

        protected override void DrawNormalMapped(DeviceContext context, EffectPass effectPass, Matrix viewProjection) {
            var world = World;
            var wit = MathF.InverseTranspose(world);
            var wvp = world * viewProjection;
            var normalMapFx = EffectManager11.Instance.GetEffect<NormalMapEffect11>();

            normalMapFx.SetWorld(world);
            normalMapFx.SetWorldInvTranspose(wit);
            normalMapFx.SetWorldViewProj(wvp);
            normalMapFx.SetWorldViewProjTex(wvp * ToTexSpace);
            normalMapFx.SetShadowTransform(world * ShadowTransform);
            normalMapFx.SetTexTransform(TexTransform);

            for (var i = 0; i < Model.SubsetCount; i++) {
                normalMapFx.SetMaterial(Model.Materials[i]);
                normalMapFx.SetDiffuseMap(Model.DiffuseMapSRV[i]);
                normalMapFx.SetNormalMap(Model.NormalMapSRV[i]);

                effectPass.Apply(context);
                Model.ModelMesh.Draw(context, i);
            }
        }

        protected override void DrawNormalDepth(DeviceContext context, EffectPass pass, Matrix view, Matrix projection) {
            var world = World;
            var wit = MathF.InverseTranspose(world);
            var wv = world * view;
            var witv = wit * view;
            var wvp = world * view * projection;
            var ssaoNormalDepthFx = EffectManager11.Instance.GetEffect<SsaoNormalDepthEffect11>();

            ssaoNormalDepthFx.SetWorldView(wv);
            ssaoNormalDepthFx.SetWorldInvTransposeView(witv);
            ssaoNormalDepthFx.SetWorldViewProj(wvp);
            ssaoNormalDepthFx.SetTexTransform(TexTransform);

            pass.Apply(context);
            for (var i = 0; i < Model.SubsetCount; i++) {
                Model.ModelMesh.Draw(context, i);
            }
        }

        protected override void DrawBasic(DeviceContext context, EffectPass effectPass, Matrix viewProjection) {
            var world = World;
            var wit = MathF.InverseTranspose(world);
            var wvp = world * viewProjection;
            var basicFx = EffectManager11.Instance.GetEffect<BasicEffect11>();

            basicFx.SetWorld(world);
            basicFx.SetWorldInvTranspose(wit);
            basicFx.SetWorldViewProj(wvp);
            basicFx.SetWorldViewProjTex(wvp * ToTexSpace);
            basicFx.SetShadowTransform(world * ShadowTransform);
            basicFx.SetTexTransform(TexTransform);

            for (var i = 0; i < Model.SubsetCount; i++) {
                basicFx.SetMaterial(Model.Materials[i]);
                basicFx.SetDiffuseMap(Model.DiffuseMapSRV[i]);

                effectPass.Apply(context);
                Model.ModelMesh.Draw(context, i);
            }
        }

        protected override void DrawShadowMap(DeviceContext context, EffectPass effectPass, Matrix viewProjection) {
            var world = World;
            var wit = MathF.InverseTranspose(world);
            var wvp = world * viewProjection;
            var buildShadowMapFx = EffectManager11.Instance.GetEffect<BuildShadowMapEffect11>();

            buildShadowMapFx.SetWorld(world);
            buildShadowMapFx.SetWorldInvTranspose(wit);
            buildShadowMapFx.SetWorldViewProj(wvp);

            for (var i = 0; i < Model.SubsetCount; i++) {
                effectPass.Apply(context);
                Model.ModelMesh.Draw(context, i);
            }
        }

        protected override void DrawDisplacementMapped(DeviceContext context, EffectPass pass, Matrix viewProjection) {
            var world = World;
            var wit = MathF.InverseTranspose(world);
            var wvp = world * viewProjection;
            var displacementMapFx = EffectManager11.Instance.GetEffect<DisplacementMapEffect11>();

            displacementMapFx.SetWorld(world);
            displacementMapFx.SetWorldInvTranspose(wit);
            displacementMapFx.SetWorldViewProj(wvp);
            displacementMapFx.SetViewProj(viewProjection);
            displacementMapFx.SetWorldViewProjTex(wvp * ToTexSpace);
            displacementMapFx.SetShadowTransform(world * ShadowTransform);
            displacementMapFx.SetTexTransform(TexTransform);

            for (var i = 0; i < Model.SubsetCount; i++) {
                displacementMapFx.SetMaterial(Model.Materials[i]);
                displacementMapFx.SetDiffuseMap(Model.DiffuseMapSRV[i]);
                displacementMapFx.SetNormalMap(Model.NormalMapSRV[i]);

                pass.Apply(context);
                Model.ModelMesh.Draw(context, i);
            }
        }
    }
}
