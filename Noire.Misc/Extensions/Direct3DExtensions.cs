using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Mathematics.Interop;

namespace Noire.Extensions {
    public static class Direct3DExtensions {

        public static Color ToColor(this RawColor4 color) {
            return new Color(color.R, color.G, color.B, color.A);
        }

    }
}
