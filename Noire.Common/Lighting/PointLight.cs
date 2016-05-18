using System.Runtime.InteropServices;
using SharpDX;

namespace Noire.Common.Lighting {
    [StructLayout(LayoutKind.Sequential)]
    public struct PointLight {

        public Color4 Ambient;
        public Color4 Diffuse;
        public Color4 Specular;
        public Vector3 Position;
        public float Range;
        public Vector3 Attenuation;
        private readonly float Pad;

        public static readonly int Stride = Marshal.SizeOf(typeof(PointLight));

    }

}
