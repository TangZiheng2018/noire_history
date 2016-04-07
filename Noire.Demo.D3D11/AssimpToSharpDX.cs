using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using SharpDX;

namespace Noire.Demo.D3D11 {
    internal static class AssimpToSharpDX {

        public static Vector3 ToVector3(this Vector3D v) {
            return new Vector3(v.X, v.Y, v.Z);
        }

    }
}
