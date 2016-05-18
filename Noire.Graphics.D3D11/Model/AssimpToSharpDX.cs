using Assimp;
using SharpDX;

namespace Noire.Graphics.D3D11.Model {
    public static class AssimpToSharpDX {

        public static Vector3 ToVector3(this Vector3D v) {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Vector2 ToVector2(this Vector2D v) {
            return new Vector2(v.X, v.Y);
        }

        public static Vector4 ToVector4(this Color4D v) {
            return new Vector4(v.R, v.G, v.B, v.A);
        }

        public static Vector2 ToVector2(this Vector3D v) {
            return new Vector2(v.X, v.Y);
        }

        public static Color ToColor(this Color4D v) {
            return new Color(v.R, v.G, v.B, v.A);
        }

        public static Noire.Common.Material ToMaterial(this Material material) {
            return new Common.Material() {
                Ambient = material.ColorAmbient.ToColor(),
                Diffuse = material.ColorDiffuse.ToColor(),
                Reflect = material.ColorReflective.ToColor(),
                Specular = material.ColorSpecular.ToColor()
            };
        }

    }
}
