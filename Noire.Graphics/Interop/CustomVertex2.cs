using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Mathematics.Interop;

namespace Noire.Graphics.Interop
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct CustomVertex2
    {

        public Vector3 Position;
        public ColorBGRA Color;

    }
}
