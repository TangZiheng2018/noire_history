using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Graphics.Interop;
using Noire.Graphics.Misc;
using SharpDX;
using SharpDX.Direct3D9;

namespace Noire.Graphics.Elements.Tests
{
    public sealed class SphereLightTest : DisplayObject
    {

        public SphereLightTest(RenderManager manager)
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

        public void LoadSTL(string fileName)
        {
            _vertices = STLReader.ReadBinary(fileName);
        }

        public LightType LightType { get; set; } = LightType.Spot;

        protected override void RenderInternal(RenderTarget target)
        {
            var device = target.Device;
            var size = _manager.Control.ClientSize;

            var vertexBuffer = new VertexBuffer(device, Utilities.SizeOf(_vertices), Usage.WriteOnly, CustomVertex3.FVF, Pool.Managed);
            var ptr1 = vertexBuffer.Lock(0, 0, LockFlags.None);
            ptr1.WriteRange(_vertices);
            vertexBuffer.Unlock();

            var vEye = new Vector3(-5f, -5f, 0f);
            var vLookAt = new Vector3(0, 0, 0);
            var vUp = new Vector3(0, 0, 1);
            vUp.Normalize();
            var matView = Matrix.LookAtLH(vEye, vLookAt, vUp);
            device.SetTransform(TransformState.View, matView);

            var matProj = Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(45), (float)size.Width / size.Height, 1.0f, 1000.0f);
            device.SetTransform(TransformState.Projection, matProj);

            device.SetTransform(TransformState.World, _worldMatrix);

            var material = new Material();
            material.Ambient = new Color(0.3f, 0.3f, 0.3f);
            material.Diffuse = new Color(0.2f, 0.2f, 0.2f);
            material.Specular = new Color(0.7f, 0.7f, 0.7f);
            material.Emissive = new Color(0f, 0f, 0f);
            device.Material = material; //(ref material);

            SetLight(target);

            device.SetRenderState(RenderState.Lighting, true);
            device.SetRenderState(RenderState.NormalizeNormals, true);
            device.SetRenderState(RenderState.SpecularEnable, true);
            device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);

            device.SetStreamSource(0, vertexBuffer, 0, Utilities.SizeOf<CustomVertex3>());
            device.VertexFormat = CustomVertex3.FVF;
            device.DrawPrimitives(PrimitiveType.TriangleList, 0, _vertices.Length / 3);

            vertexBuffer.Dispose();
        }

        private void SetLight(RenderTarget target)
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
                    light.Range = 320f;
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
                    light.Specular = new Color(0.5f, 0.5f, 0.5f);
                    light.Position = new Vector3(0, -10, 6);
                    light.Attenuation0 = 1f;
                    light.Attenuation1 = 0f;
                    light.Attenuation2 = 0f;
                    light.Range = 320f;
                    var spotDirection = new Vector3(0, 1, -1);
                    spotDirection.Normalize();
                    light.Direction = spotDirection;
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
        }

        protected override void UpdateInternal(RenderTarget target)
        {
            _rotation += 2f;
            var rad = MathUtil.DegreesToRadians(_rotation);
            _worldMatrix = Matrix.RotationX(rad);
        }

        private CustomVertex3[] _vertices;

        private Matrix _worldMatrix;
        private float _rotation = 0f;

    }
}
