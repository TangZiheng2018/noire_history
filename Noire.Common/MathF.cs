using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Noire.Common {
    public static class MathF {

        static MathF() {
            Rand = new Random();
        }

        public const float PI = (float)Math.PI;

        public static float Sin(float a) {
            return (float)Math.Sin(a);
        }

        public static float Cos(float a) {
            return (float)Math.Cos(a);
        }

        public static float Tan(float a) {
            return (float)Math.Tan(a);
        }

        public static float Atan(float f) {
            return (float)Math.Atan(f);
        }

        public static float Atan2(float y, float x) {
            return (float)Math.Atan2(y, x);
        }

        public static float Clamp(float value, float min, float max) {
            return value < min ? min : (value > max ? max : value);
        }

        public static Matrix InverseTranspose(Matrix m) {
            m.M41 = m.M42 = m.M43 = 0;
            m.M44 = 1;
            return Matrix.Transpose(Matrix.Invert(m));
        }

        public static readonly Random Rand;

    }
}
