using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Mathematics.Interop;

namespace Noire.Graphics.Interop.Vertices {

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct PositionColor {

        public Vector3 Position;
        public ColorBGRA Color;

        public static VertexFormat FVF => VertexFormat.Position | VertexFormat.Diffuse;

    }
}
