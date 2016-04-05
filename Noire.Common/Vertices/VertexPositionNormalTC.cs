using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Noire.Common.Vertices {
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionNormalTC {

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoords;

        public VertexPositionNormalTC(Vector3 position, Vector3 normal, Vector2 textureCoords) {
            Position = position;
            Normal = normal;
            TextureCoords = textureCoords;
        }

        public static readonly int Stride = Marshal.SizeOf(typeof(VertexPositionNormalTC));

    }
}
