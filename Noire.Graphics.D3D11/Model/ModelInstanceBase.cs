using System;
using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11.Model {
    public abstract class ModelInstanceBase<T> {

        public ModelBase<T> Model { get; }
        public Matrix World { get; set; }
        public Matrix ShadowTransform { get; set; }
        public Matrix TexTransform { get; set; }
        public Matrix ToTexSpace { get; set; }

        public BoundingBox BoundingBox => new BoundingBox(Vector3.TransformCoordinate(Model.BoundingBox.Minimum, World), Vector3.TransformCoordinate(Model.BoundingBox.Maximum, World));

        public void Draw(DeviceContext context, EffectPass effectPass, Matrix view, Matrix projection, ModelDrawDelegate method) {
            method(context, effectPass, view, projection);
        }

        public void Draw(DeviceContext context, EffectPass effectPass, Matrix view, Matrix projection, RenderMode renderMode = RenderMode.NormalMapped) {
            switch (renderMode) {
                case RenderMode.NormalMapped:
                    DrawNormalMapped(context, effectPass, view * projection);
                    break;
                case RenderMode.Basic:
                    DrawBasic(context, effectPass, view * projection);
                    break;
                case RenderMode.DisplacementMapped:
                    DrawDisplacementMapped(context, effectPass, view * projection);
                    break;
                case RenderMode.ShadowMap:
                    DrawShadowMap(context, effectPass, view * projection);
                    break;
                case RenderMode.NormalDepthMap:
                    DrawNormalDepth(context, effectPass, view, projection);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(renderMode));
            }
        }

        protected ModelInstanceBase() {
            World = Matrix.Identity;
            ShadowTransform = Matrix.Identity;
            TexTransform = Matrix.Identity;
            ToTexSpace = Matrix.Identity;
        }

        protected ModelInstanceBase(ModelBase<T> model)
            : this() {
            Model = model;
        }

        protected abstract void DrawNormalDepth(DeviceContext context, EffectPass effectPass, Matrix view, Matrix projection);
        protected abstract void DrawShadowMap(DeviceContext context, EffectPass effectPass, Matrix viewProjection);
        protected abstract void DrawDisplacementMapped(DeviceContext context, EffectPass effectPass, Matrix viewProjection);
        protected abstract void DrawBasic(DeviceContext context, EffectPass effectPass, Matrix viewProjection);
        protected abstract void DrawNormalMapped(DeviceContext context, EffectPass effectPass, Matrix viewProjection);

    }
}
