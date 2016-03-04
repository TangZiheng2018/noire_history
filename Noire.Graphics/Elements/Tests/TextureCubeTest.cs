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
    public sealed class TextureCubeTest : DisplayObject
    {

        public TextureCubeTest(RenderManager manager)
            : base(manager)
        {
        }

        public void SetTexture(string filePath)
        {
            DisposeTexture();
            _texturePath = filePath;
        }

        /// <summary>
        /// 执行与释放或重置非托管资源关联的应用程序定义的任务。
        /// </summary>
        public override void Dispose()
        {
            DisposeTexture();
            base.Dispose();
        }

        protected override void RenderInternal(RenderTarget target)
        {
            var device = target.Device;
            var size = _manager.Control.ClientSize;

            var vertexBuffer = new VertexBuffer(device, _vertices.Length * Utilities.SizeOf<CustomVertex4>(), Usage.WriteOnly, CustomVertex4.FVF, Pool.Managed);
            var ptr1 = vertexBuffer.Lock(0, 0, LockFlags.None);
            ptr1.WriteRange(_vertices);
            vertexBuffer.Unlock();

            var indexBuffer = new IndexBuffer(device, _indices.Length * Utilities.SizeOf<ushort>(), Usage.WriteOnly, Pool.Managed, true);
            var ptr2 = indexBuffer.Lock(0, 0, LockFlags.None);
            ptr2.WriteRange(_indices);
            indexBuffer.Unlock();

            var vEye = new Vector3(0, -5, 5);
            var vLookAt = new Vector3(0, 0, 0);
            var vUp = new Vector3(0, 1, 0);
            var matView = Matrix.LookAtLH(vEye, vLookAt, vUp);
            device.SetTransform(TransformState.View, matView);

            var matProj = Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(45), (float)size.Width / size.Height, 1.0f, 1000.0f);
            device.SetTransform(TransformState.Projection, matProj);

            device.SetTransform(TransformState.World, _worldMatrix);

            device.SetStreamSource(0, vertexBuffer, 0, Utilities.SizeOf<CustomVertex4>());
            device.VertexFormat = CustomVertex4.FVF;
            device.Indices = indexBuffer;
            device.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, _vertices.Length, 0, _indices.Length / 3);

            indexBuffer.Dispose();
            vertexBuffer.Dispose();
        }

        protected override void UpdateInternal(RenderTarget target)
        {
            if (_texture == null && _texturePath != null)
            {
                var device = target.Device;
                _texture = Texture.FromFile(device, _texturePath);
                if (_texture != null)
                {
                    device.SetTexture(0, _texture);
                    device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
                    device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
                    device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Point);
                }
                device.SetRenderState(RenderState.Lighting, false);
                device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);
            }
            _degree += 2;
            var rad = MathUtil.DegreesToRadians(_degree);
            _worldMatrix = Matrix.RotationY(rad) * Matrix.RotationZ(rad * 0.5f) * Matrix.RotationX(rad * 0.25f);
        }

        private const float CR_ONE_THRID = 0.6933613f;

        private CustomVertex4[] _vertices = new[]
        {
            // Back
            new CustomVertex4() {Position = new Vector3(-1, 1, -1), Normals = new Vector3(0, 1, 0), TextureCoords = new Vector2(1, 1)},
            new CustomVertex4() {Position = new Vector3(-1, 1, 1), Normals = new Vector3(0, 1, 0), TextureCoords = new Vector2(1, 0)},
            new CustomVertex4() {Position = new Vector3(1, 1, -1), Normals = new Vector3(0, 1, 0), TextureCoords = new Vector2(0, 1)},
            new CustomVertex4() {Position = new Vector3(1, 1, 1), Normals = new Vector3(0, 1, 0), TextureCoords = new Vector2(0, 0)},
            // Left
            new CustomVertex4() {Position = new Vector3(-1, -1, -1), Normals = new Vector3(-1, 0, 0), TextureCoords = new Vector2(1, 1)},
            new CustomVertex4() {Position = new Vector3(-1, -1, 1), Normals = new Vector3(-1, 0, 0), TextureCoords = new Vector2(1, 0)},
            new CustomVertex4() {Position = new Vector3(-1, 1, -1), Normals = new Vector3(-1, 0, 0), TextureCoords = new Vector2(0, 1)},
            new CustomVertex4() {Position = new Vector3(-1, 1, 1), Normals = new Vector3(-1, 0, 0), TextureCoords = new Vector2(0, 0)},
            // Right
            new CustomVertex4() {Position = new Vector3(1, 1, -1), Normals = new Vector3(1, 0, 0), TextureCoords = new Vector2(1, 1)},
            new CustomVertex4() {Position = new Vector3(1, 1, 1), Normals = new Vector3(1, 0, 0), TextureCoords = new Vector2(1, 0)},
            new CustomVertex4() {Position = new Vector3(1, -1, -1), Normals = new Vector3(1, 0, 0), TextureCoords = new Vector2(0, 1)},
            new CustomVertex4() {Position = new Vector3(1, -1, 1), Normals = new Vector3(1, 0, 0), TextureCoords = new Vector2(0, 0)},
            // Front
            new CustomVertex4() {Position = new Vector3(1, -1, -1), Normals = new Vector3(0, -1, 0), TextureCoords = new Vector2(1, 1)},
            new CustomVertex4() {Position = new Vector3(1, -1, 1), Normals = new Vector3(0, -1, 0), TextureCoords = new Vector2(1, 0)},
            new CustomVertex4() {Position = new Vector3(-1, -1, -1), Normals = new Vector3(0, -1, 0), TextureCoords = new Vector2(0, 1)},
            new CustomVertex4() {Position = new Vector3(-1, -1, 1), Normals = new Vector3(0, -1, 0), TextureCoords = new Vector2(0, 0)},
            // Top
            new CustomVertex4() {Position = new Vector3(1, -1, 1), Normals = new Vector3(0, 0, 1), TextureCoords = new Vector2(1, 1)},
            new CustomVertex4() {Position = new Vector3(1, 1, 1), Normals = new Vector3(0, 0, 1), TextureCoords = new Vector2(1, 0)},
            new CustomVertex4() {Position = new Vector3(-1, -1, 1), Normals = new Vector3(0, 0, 1), TextureCoords = new Vector2(0, 1)},
            new CustomVertex4() {Position = new Vector3(-1, 1, 1), Normals = new Vector3(0, 0, 1), TextureCoords = new Vector2(0, 0)},
            // Bottom
            new CustomVertex4() {Position = new Vector3(-1, -1, -1), Normals = new Vector3(0, 0, -1), TextureCoords = new Vector2(1, 1)},
            new CustomVertex4() {Position = new Vector3(-1, 1, -1), Normals = new Vector3(0, 0, -1), TextureCoords = new Vector2(1, 0)},
            new CustomVertex4() {Position = new Vector3(1, -1, -1), Normals = new Vector3(0, 0, -1), TextureCoords = new Vector2(0, 1)},
            new CustomVertex4() {Position = new Vector3(1, 1, -1), Normals = new Vector3(0, 0, -1), TextureCoords = new Vector2(0, 0)},
        };

        private void DisposeTexture()
        {
            if (_texture != null)
            {
                _texture.Dispose();
                _texture = null;
            }
        }

        private ushort[] _indices = new ushort[]
        {
            0, 1, 2, 2, 1, 3,
            4, 5, 6, 6, 5, 7,
            8, 9, 10, 10, 9, 11,
            12, 13, 14, 14, 13, 15,
            16, 17, 18, 18, 17, 19,
            20, 21, 22, 22, 21, 23,
        };

        private Matrix _worldMatrix;
        private float _degree;
        private Texture _texture;
        private string _texturePath;

    }
}
