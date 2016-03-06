using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Graphics.Interop.Vertices;
using SharpDX;
using SharpDX.Direct3D9;

namespace Noire.Graphics.Nodes.Tests {
    public class SimpleCubeNode : Node {

        public SimpleCubeNode(SceneNode runtime)
            : base(runtime, false) {
        }

        protected override void RenderAfterChildren() {
            var device = Scene.CurrentCamera?.Device;
            var size = Scene.Control.ClientSize;
            if (device != null) {
                var vertexBuffer = new VertexBuffer(device, _vertices.Length * Utilities.SizeOf<PositionColor>(), Usage.WriteOnly, PositionColor.FVF, Pool.Managed);
                var ptr1 = vertexBuffer.Lock(0, 0, LockFlags.None);
                ptr1.WriteRange(_vertices);
                vertexBuffer.Unlock();

                var indexBuffer = new IndexBuffer(device, _indices.Length * Utilities.SizeOf<ushort>(), Usage.WriteOnly, Pool.Managed, true);
                var ptr2 = indexBuffer.Lock(0, 0, LockFlags.None);
                ptr2.WriteRange(_indices);
                indexBuffer.Unlock();
                
                device.SetStreamSource(0, vertexBuffer, 0, Utilities.SizeOf<PositionColor>());
                device.VertexFormat = PositionColor.FVF;
                device.Indices = indexBuffer;
                device.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, _vertices.Length, 0, _indices.Length / 3);

                indexBuffer.Dispose();
                vertexBuffer.Dispose();
            }
        }

        private PositionColor[] _vertices = new[]
        {
            new PositionColor() {Position = new Vector3(-5, 5, -5), Color = new Color(255, 0, 0)},
            new PositionColor() {Position = new Vector3(-5, 5, 5), Color = new Color(0, 255, 0)},
            new PositionColor() {Position = new Vector3(5, 5, 5), Color = new Color(0, 0, 255)},
            new PositionColor() {Position = new Vector3(5, 5, -5), Color = new Color(255, 255, 0)},
            new PositionColor() {Position = new Vector3(-5, -5, -5), Color = new Color(255, 0, 255)},
            new PositionColor() {Position = new Vector3(-5, -5, 5), Color = new Color(0, 255, 255)},
            new PositionColor() {Position = new Vector3(5, -5, 5), Color = new Color(0, 0, 0)},
            new PositionColor() {Position = new Vector3(5, -5, -5), Color = new Color(255, 255, 255)},
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

    }
}
