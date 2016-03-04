using SharpDX;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Graphics.Interop;

namespace Noire.Graphics.Elements.Tests
{
    public sealed class DefaultTriangle : DisplayObject
    {

        public DefaultTriangle(RenderManager manager)
            : base(manager)
        {
        }
        
        protected override void RenderInternal(RenderTarget target)
        {
            // 注释：其实这里可以用 XYZRHW
            // http://www.cppblog.com/lovedday/archive/2008/04/30/48507.html
            // 这是一个 D3D 比 OpenGL 方便的地方wwww
            var outerVertex = new[]
            {
                new CustomVertex1() { Position = new Vector4(400.0f, 62.5f, 0.5f, 1.0f), Color = new Color(255, 0, 0, 255) },
                new CustomVertex1() { Position = new Vector4(650.0f, 500.0f, 0.5f, 1.0f), Color = new Color(0, 255, 0, 0) },
                new CustomVertex1() { Position = new Vector4(150.0f, 500.0f, 0.5f, 1.0f), Color = new Color(0, 0, 255, 255) },
            };

            _vertexBuffer = new VertexBuffer(target.Device, outerVertex.Length * Utilities.SizeOf<CustomVertex1>(), Usage.WriteOnly, VertexFormat.None, Pool.Managed);
            var ptr = _vertexBuffer.Lock(0, 0, LockFlags.None);
            ptr.WriteRange(outerVertex);
            _vertexBuffer.Unlock();

            _vertexDeclaration = new VertexDeclaration(target.Device, new VertexElement[]
            {
                new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.PositionTransformed, 0),
                new VertexElement(0, 16, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                VertexElement.VertexDeclarationEnd
            });

            target.Device.SetStreamSource(0, _vertexBuffer, 0, Utilities.SizeOf<CustomVertex1>());
            target.Device.VertexDeclaration = _vertexDeclaration;
            target.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, 1);

            _vertexDeclaration.Dispose();
            _vertexBuffer.Dispose();
        }

        protected override void UpdateInternal(RenderTarget target)
        {
        }

        /// <summary>
        /// 执行与释放或重置非托管资源关联的应用程序定义的任务。
        /// </summary>
        public override void Dispose()
        {
            NoireUtilities.SafeDispose(ref _vertexBuffer);
            NoireUtilities.SafeDispose(ref _vertexDeclaration);
            base.Dispose();
        }

        private VertexBuffer _vertexBuffer = null;
        private VertexDeclaration _vertexDeclaration = null;

    }
}
