using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Mathematics.Interop;

namespace Noire.Misc {

    public static class NoireUtilities {

        public static void SafeDispose<T>(ref T obj) where T : class, IDisposable {
            obj?.Dispose();
            obj = null;
        }

    }

}
