using System.Runtime.InteropServices;
using SharpDX;

namespace Noire.Common {
    public struct Particle {

        public Vector3 InitialPosition;
        public Vector3 InitialVelocity;
        public Vector2 Size;
        public float Age;
        public uint Type;

        public static readonly int Stride = Marshal.SizeOf(typeof(Particle));

    }
}
