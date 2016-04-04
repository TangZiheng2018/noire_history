using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Noire.Common.D3D11 {
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
