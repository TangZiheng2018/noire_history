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
    public sealed class IndexBufferTest : DisplayObject
    {

        public IndexBufferTest(RenderManager manager)
            : base(manager)
        {
        }

        protected override void RenderInternal(RenderTarget target)
        {
            var device = target.Device;
            var size = _manager.Control.ClientSize;
            var format = VertexFormat.Position | VertexFormat.Diffuse;

            var vertexBuffer = new VertexBuffer(device, _vertices.Length * Utilities.SizeOf<CustomVertex2>(), Usage.WriteOnly, format, Pool.Managed);
            var ptr1 = vertexBuffer.Lock(0, 0, LockFlags.None);
            ptr1.WriteRange(_vertices);
            vertexBuffer.Unlock();

            var indexBuffer = new IndexBuffer(device, _indices.Length * Utilities.SizeOf<ushort>(), Usage.WriteOnly, Pool.Managed, true);
            var ptr2 = indexBuffer.Lock(0, 0, LockFlags.None);
            ptr2.WriteRange(_indices);
            indexBuffer.Unlock();

            var vEye = new Vector3(0, 0, -30f);
            var vLookAt = new Vector3(0, 0, 0);
            var vUp = new Vector3(0, 1, 0);
            var matView = Matrix.LookAtLH(vEye, vLookAt, vUp);
            device.SetTransform(TransformState.View, matView);

            var matProj = Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(45), (float)size.Width / size.Height, 1.0f, 1000.0f);
            device.SetTransform(TransformState.Projection, matProj);

            device.SetTransform(TransformState.World, _worldMatrix);

            device.SetRenderState(RenderState.Lighting, false);
            device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);

            device.SetStreamSource(0, vertexBuffer, 0, Utilities.SizeOf<CustomVertex2>());
            device.VertexFormat = format;
            device.Indices = indexBuffer;
            device.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);

            indexBuffer.Dispose();
            vertexBuffer.Dispose();
        }

        protected override void UpdateInternal(RenderTarget target)
        {
            _degree += 2;
            var rad = MathUtil.DegreesToRadians(_degree);
            _worldMatrix = Matrix.RotationY(rad) * Matrix.RotationZ(rad * 0.5f) * Matrix.RotationX(rad * 0.25f);
        }

        private CustomVertex2[] _vertices = new[]
        {
            new CustomVertex2() {Position = new Vector3(-5, 5, -5), Color = new Color(255, 0, 0)},
            new CustomVertex2() {Position = new Vector3(-5, 5, 5), Color = new Color(0, 255, 0)},
            new CustomVertex2() {Position = new Vector3(5, 5, 5), Color = new Color(0, 0, 255)},
            new CustomVertex2() {Position = new Vector3(5, 5, -5), Color = new Color(255, 255, 0)},
            new CustomVertex2() {Position = new Vector3(-5, -5, -5), Color = new Color(255, 0, 255)},
            new CustomVertex2() {Position = new Vector3(-5, -5, 5), Color = new Color(0, 255, 255)},
            new CustomVertex2() {Position = new Vector3(5, -5, 5), Color = new Color(0, 0, 0)},
            new CustomVertex2() {Position = new Vector3(5, -5, -5), Color = new Color(255, 255, 255)},
        };

        private ushort[] _indices = new ushort[]
        {
            0, 1, 2, 0, 2, 3,
            0, 3, 7, 0, 7, 4,
            0, 4, 5, 0, 5, 1,
            2, 6, 7, 2, 7, 3,
            2, 5, 6, 2, 1, 5,
            4, 6, 5, 4, 7, 6
        };

        private Matrix _worldMatrix;
        private float _degree;

    }
}
