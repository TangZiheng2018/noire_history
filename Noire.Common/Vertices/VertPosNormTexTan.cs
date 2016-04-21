using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Noire.Common.Vertices {
    [StructLayout(LayoutKind.Sequential)]
    public struct VertPosNormTexTan {

        public Vector3 Pos;
        public Vector3 Normal;
        public Vector2 Tex;
        public Vector3 Tan;

        public VertPosNormTexTan(Vector3 position, Vector3 normal, Vector2 texC, Vector3 tangentU) {
            Pos = position;
            Normal = normal;
            Tex = texC;
            Tan = tangentU;
        }

        public static readonly int Stride = Marshal.SizeOf(typeof(VertPosNormTexTan));

    }
}
