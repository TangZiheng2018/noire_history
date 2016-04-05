using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Noire.Common.Vertices {
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionColor {

        public Vector3 Position;
        public Color4 Color;

        public VertexPositionColor(Vector3 position, Color color) {
            Position = position;
            Color = color;
        }

        public VertexPositionColor(Vector3 position, Color4 color) {
            Position = position;
            Color = color;
        }

        public static readonly int Stride = Marshal.SizeOf(typeof(VertexPositionColor));

    }
}
