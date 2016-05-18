using System.Runtime.InteropServices;
using SharpDX;

namespace Noire.Common.Vertices {
    [StructLayout(LayoutKind.Sequential)]
    public struct VertPosNormTex {

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoords;

        public VertPosNormTex(Vector3 position, Vector3 normal, Vector2 textureCoords) {
            Position = position;
            Normal = normal;
            TextureCoords = textureCoords;
        }

        public static readonly int Stride = Marshal.SizeOf(typeof(VertPosNormTex));

    }
}
