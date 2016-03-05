using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;

namespace Noire.Graphics.Interop {

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct CustomVertex1 {

        public Vector4 Position;
        public ColorBGRA Color;

        public static readonly VertexFormat FVF = VertexFormat.Position | VertexFormat.Diffuse;

    }
}
