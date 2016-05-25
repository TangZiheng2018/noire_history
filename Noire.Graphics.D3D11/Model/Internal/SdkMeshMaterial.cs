using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SharpDX;

namespace Noire.Graphics.D3D11.Model.Internal {
    [StructLayout(LayoutKind.Sequential)]
    internal struct SdkMeshMaterial {

        public readonly string Name;
        public readonly string MaterialInstancePath;
        public readonly string DiffuseTexture;
        public readonly string NormalTexture;
        public readonly string SpecularTexture;

        public readonly Color4 Diffuse;
        public readonly Color4 Ambient;
        public readonly Color4 Specular;
        public readonly Color4 Emissive;
        public readonly float Power;

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var fieldInfo in GetType().GetFields().Where(fi => !fi.IsLiteral)) {
                sb.AppendLine(fieldInfo.Name + ": " + fieldInfo.GetValue(this));
            }
            return sb.ToString();
        }
        public SdkMeshMaterial(BinaryReader reader) {
            Name = Encoding.Default.GetString(reader.ReadBytes(MaxMaterialName));
            if (Name[0] == '\0') {
                Name = "";
            }
            MaterialInstancePath = Encoding.Default.GetString(reader.ReadBytes(MaxMaterialPath)).Trim(new[] { ' ', '\0' });
            DiffuseTexture = Encoding.Default.GetString(reader.ReadBytes(MaxTextureName)).Trim(new[] { ' ', '\0' });
            NormalTexture = Encoding.Default.GetString(reader.ReadBytes(MaxTextureName)).Trim(new[] { ' ', '\0' });
            SpecularTexture = Encoding.Default.GetString(reader.ReadBytes(MaxTextureName)).Trim(new[] { ' ', '\0' });

            Diffuse = new Color4 {
                Red = reader.ReadSingle(),
                Green = reader.ReadSingle(),
                Blue = reader.ReadSingle(),
                Alpha = reader.ReadSingle()
            };
            Ambient = new Color4 {
                Red = reader.ReadSingle(),
                Green = reader.ReadSingle(),
                Blue = reader.ReadSingle(),
                Alpha = reader.ReadSingle()
            };
            Specular = new Color4 {
                Red = reader.ReadSingle(),
                Green = reader.ReadSingle(),
                Blue = reader.ReadSingle(),
                Alpha = reader.ReadSingle()
            };
            Emissive = new Color4 {
                Red = reader.ReadSingle(),
                Green = reader.ReadSingle(),
                Blue = reader.ReadSingle(),
                Alpha = reader.ReadSingle()
            };
            Power = reader.ReadSingle();
            // Padding...
            reader.ReadUInt64();
            reader.ReadUInt64();
            reader.ReadUInt64();
            reader.ReadUInt64();
            reader.ReadUInt64();
            reader.ReadUInt64();
        }

        private static readonly int MaxMaterialName = 100;
        private static readonly int MaxMaterialPath = 260;
        private static readonly int MaxTextureName = 260;

    }
}
