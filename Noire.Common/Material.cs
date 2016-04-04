using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Noire.Common {
    [StructLayout(LayoutKind.Sequential)]
    public struct Material {

        public Color4 Ambient;
        public Color4 Diffuse;
        public Color4 Specular;
        public Color4 Reflect;

        public static readonly int Stride = Marshal.SizeOf(typeof(Material));

    }
}
