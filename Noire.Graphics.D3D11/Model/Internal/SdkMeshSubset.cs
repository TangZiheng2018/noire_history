using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Noire.Graphics.D3D11.Model.Internal {
    [StructLayout(LayoutKind.Sequential)]
    internal struct SdkMeshSubset {

        public readonly string Name;
        public readonly uint MaterialID;
        public readonly uint PrimitiveType;
        public readonly ulong IndexStart;
        public readonly ulong IndexCount;
        public readonly ulong VertexStart;
        public readonly ulong VertexCount;
        
        public SdkMeshSubset(BinaryReader reader) {
            Name = Encoding.Default.GetString(reader.ReadBytes(MaxSubsetName));
            if (Name[0] == '\0') {
                Name = string.Empty;
            }
            MaterialID = reader.ReadUInt32();
            PrimitiveType = reader.ReadUInt32();
            reader.ReadUInt32();
            IndexStart = reader.ReadUInt64();
            IndexCount = reader.ReadUInt64();
            VertexStart = reader.ReadUInt64();
            VertexCount = reader.ReadUInt64();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var fieldInfo in GetType().GetFields().Where(fi => !fi.IsLiteral)) {
                sb.AppendLine(fieldInfo.Name + ": " + fieldInfo.GetValue(this));
            }
            return sb.ToString();
        }

        private static readonly int MaxSubsetName = 100;

    }
}
