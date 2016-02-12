using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Graphics.Interop;
using SharpDX;
using SharpDX.Direct3D9;

namespace Noire.Graphics.Elements.Tests
{
    public sealed class LightAndMaterialTest : DisplayObject
    {

        public LightAndMaterialTest(RenderManager manager)
            : base(manager)
        {
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public LightType LightType { get; set; } = LightType.Point;

        protected override void RenderInternal(RenderTarget target)
        {
            var device = target.Device;
            var size = _manager.Control.ClientSize;
            var format = VertexFormat.Position | VertexFormat.Normal;

            var vertexBuffer = new VertexBuffer(device, Utilities.SizeOf(_vertices), Usage.WriteOnly, format, Pool.Managed);
            var ptr1 = vertexBuffer.Lock(0, 0, LockFlags.None);
            ptr1.WriteRange(_vertices);
            vertexBuffer.Unlock();

            var vEye = new Vector3(0, 1f, -5f);
            var vLookAt = new Vector3(0, 0, 0);
            var vUp = new Vector3(0, 1, 0);
            var matView = Matrix.LookAtLH(vEye, vLookAt, vUp);
            device.SetTransform(TransformState.View, matView);

            var matProj = Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(45), (float)size.Width / size.Height, 1.0f, 1000.0f);
            device.SetTransform(TransformState.Projection, matProj);

            device.SetTransform(TransformState.World, _worldMatrix);

            var material = new Material();
            material.Ambient = new Color(1f, 1f, 1f);
            material.Diffuse = new Color(1f, 1f, 1f);
            material.Specular = new Color(0.8f, 0.3f, 0.3f);
            material.Emissive = new Color(0f, 0f, 0f);
            device.Material = material; //(ref material);

            device.SetRenderState(RenderState.Lighting, true);
            device.SetRenderState(RenderState.NormalizeNormals, true);
            device.SetRenderState(RenderState.SpecularEnable, true);
            device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);

            device.SetStreamSource(0, vertexBuffer, 0, Utilities.SizeOf<CustomVertex3>());
            device.VertexFormat = format;
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, 4);

            vertexBuffer.Dispose();
        }

        protected override void UpdateInternal(RenderTarget target)
        {
            var light = new Light();
            light.Type = LightType;
            switch (LightType)
            {
                case LightType.Point:
                    light.Ambient = new Color(0.8f, 0.8f, 0.8f);
                    light.Diffuse = new Color(1f, 1f, 1f);
                    light.Specular = new Color(0.3f, 0.3f, 0.3f);
                    light.Position = new Vector3(-300, 0, 0);
                    light.Attenuation0 = 1f;
                    light.Attenuation1 = 0f;
                    light.Attenuation2 = 0f;
                    light.Range = 300f;
                    break;
                case LightType.Directional:
                    light.Ambient = new Color(0.8f, 0.8f, 0.8f);
                    light.Diffuse = new Color(1f, 1f, 1f);
                    light.Specular = new Color(0.3f, 0.3f, 0.3f);
                    light.Direction = new Vector3(1, 0, 0);
                    break;
                case LightType.Spot:
                    light.Ambient = new Color(0.8f, 0.8f, 0.8f);
                    light.Diffuse = new Color(1f, 1f, 1f);
                    light.Specular = new Color(0.3f, 0.3f, 0.3f);
                    light.Position = new Vector3(0, 300, 0);
                    light.Attenuation0 = 1f;
                    light.Attenuation1 = 0f;
                    light.Attenuation2 = 0f;
                    light.Range = 300f;
                    light.Direction = new Vector3(0, -1, 0);
                    light.Falloff = 0.1f;
                    light.Phi = (float)Math.PI / 3f;
                    light.Theta = (float)Math.PI / 6f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(LightType));
            }
            var device = target.Device;
            device.SetLight(0, ref light);
            device.EnableLight(0, true);
            device.SetRenderState(RenderState.Ambient, new Color(92, 92, 92).ToBgra());

            _rotation += 2f;
            var rad = MathUtil.DegreesToRadians(_rotation);
            _worldMatrix = Matrix.RotationY(rad);
        }

        private CustomVertex3[] _vertices = new[]
        {
            new CustomVertex3() {Position = new Vector3(-1, 0, -1), Normals = new Vector3(0, 0.707f, -0.707f)},
            new CustomVertex3() {Position = new Vector3(0, 1, 0), Normals = new Vector3(0, 0.707f, -0.707f)},
            new CustomVertex3() {Position = new Vector3(1, 0, -1), Normals = new Vector3(0, 0.707f, -0.707f)},

            new CustomVertex3() {Position = new Vector3(-1, 0, 1), Normals = new Vector3(-0.707f, 0.707f, 0)},
            new CustomVertex3() {Position = new Vector3(0, 1, 0), Normals = new Vector3(-0.707f, 0.707f, 0)},
            new CustomVertex3() {Position = new Vector3(-1, 0, -1), Normals = new Vector3(-0.707f, 0.707f, 0)},

            new CustomVertex3() {Position = new Vector3(1, 0, -1), Normals = new Vector3(0.707f, 0.707f, 0)},
            new CustomVertex3() {Position = new Vector3(0, 1, 0), Normals = new Vector3(0.707f, 0.707f, 0)},
            new CustomVertex3() {Position = new Vector3(1, 0, 1), Normals = new Vector3(0.707f, 0.707f, 0)},

            new CustomVertex3() {Position = new Vector3(1, 0, 1), Normals = new Vector3(0, 0.707f, 0.707f)},
            new CustomVertex3() {Position = new Vector3(0, 1, 0), Normals = new Vector3(0, 0.707f, 0.707f)},
            new CustomVertex3() {Position = new Vector3(-1, 0, 1), Normals = new Vector3(0, 0.707f, 0.707f)},
        };

        private Matrix _worldMatrix;
        private float _rotation = 0f;

    }
}
