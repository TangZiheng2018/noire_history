using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Noire.Common.Vertices {
    [StructLayout(LayoutKind.Sequential)]
    public struct VertPos {

        public Vector3 Position;

        public VertPos(Vector3 position) {
            Position = position;
        }

        public static readonly int Stride = Marshal.SizeOf(typeof(VertPos));

    }
}
