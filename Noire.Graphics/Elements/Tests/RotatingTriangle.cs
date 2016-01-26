using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Graphics.Interop;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;

namespace Noire.Graphics.Elements.Tests
{
    public sealed class RotatingTriangle : DisplayObject
    {

        public RotatingTriangle(RenderManager manager)
            : base(manager)
        {
            _rotationDegree = 0.0f;
            Initialize();
        }

        void Initialize()
        {
            VertexFormat customFormat = VertexFormat.Position | VertexFormat.Diffuse;
            _vertexBuffer = new VertexBuffer(_manager.Screen.Device, 3 * Utilities.SizeOf<CustomVertex2>(), Usage.None, customFormat, Pool.Managed);
            var ptr = _vertexBuffer.Lock(0, 0, LockFlags.None);
            var vertices = new CustomVertex2[3];

            vertices[0].Position = new Vector3(3, -3, 0);
            vertices[0].Color = Color.Red;
            vertices[1].Position = new Vector3(0, 3, 0);
            vertices[1].Color = Color.Green;
            vertices[2].Position = new Vector3(-3, -3, 0);
            vertices[2].Color = Color.Blue;
            ptr.WriteRange(vertices);
            _vertexBuffer.Unlock();
        }

        static Matrix perspective(float fov, float aspect, float near, float far)
        {
            var m = new Matrix();
            m.M11 = (float)(1 / Math.Tan(fov * 0.5)) / aspect;
            m.M22 = (float)(1 / Math.Tan(fov * 0.5));
            m.M33 = far / (far - near);
            m.M34 = 1;
            m.M44 = far * near / (near - far);
            return m;
        }

        protected override void RenderInternal(RenderTarget target)
        {
            target.Device.VertexFormat = VertexFormat.Position | VertexFormat.Diffuse;

            Matrix matrix;
            matrix = Matrix.LookAtLH(new Vector3(0, 0, 10), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            target.Device.SetTransform(TransformState.View, matrix);
            var size = _manager.Control.Size;
            // Invalid matrix
            //matrix = Matrix.PerspectiveFovLH(MathUtil.DegreesToRadians(45), (float)_manager.Control.Width / _manager.Control.Height, 0f, 100f);
            // Works
            //matrix = Matrix.OrthoLH(size.Width, size.Height, -100, 100);
            matrix = perspective(MathUtil.DegreesToRadians(45), (float)size.Width / size.Height, 0, 100);
            target.Device.SetTransform(TransformState.Projection, matrix);
            target.Device.SetTransform(TransformState.World, Matrix.RotationY(_rotationDegree));
            
            target.Device.SetStreamSource(0, _vertexBuffer, 0, Utilities.SizeOf<CustomVertex2>());
            target.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);
        }

        protected override void UpdateInternal(RenderTarget target)
        {
            _rotationDegree += 0.05f;
        }

        /// <summary>
        /// 执行与释放或重置非托管资源关联的应用程序定义的任务。
        /// </summary>
        public override void Dispose()
        {
            _vertexBuffer.Dispose();
            base.Dispose();
        }

        private float _rotationDegree;
        private VertexBuffer _vertexBuffer;

    }
}
