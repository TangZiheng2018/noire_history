using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Noire.Common.Vertices {
    [StructLayout(LayoutKind.Sequential)]
    public struct VertPosNorm {

        public Vector3 Position;
        public Vector3 Normal;

        public VertPosNorm(Vector3 position, Vector3 normal) {
            Position = position;
            Normal = normal;
        }

        public static readonly int Stride = Marshal.SizeOf(typeof(VertPosNorm));

    }
}
