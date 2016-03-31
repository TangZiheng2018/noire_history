using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Noire.VariablePipeline {
    internal static class VectorExtensions {

        public static Vector4 RightMultiply(this Vector4 left, Matrix right) {
            return new Vector4(
                    left.X * right.M11 + left.Y * right.M21 + left.Z * right.M31 + left.W * right.M41,
                    left.X * right.M12 + left.Y * right.M22 + left.Z * right.M32 + left.W * right.M42,
                    left.X * right.M13 + left.Y * right.M23 + left.Z * right.M33 + left.W * right.M43,
                    left.X * right.M14 + left.Y * right.M24 + left.Z * right.M34 + left.W * right.M44
                );
        }

    }
}
