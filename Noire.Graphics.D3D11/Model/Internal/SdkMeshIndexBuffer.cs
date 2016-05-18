using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Noire.Graphics.D3D11.Model.Internal {
    [StructLayout(LayoutKind.Sequential)]
    internal struct SdkMeshIndexBuffer {

        public readonly ulong NumIndices;
        public readonly ulong SizeBytes;
        public readonly uint IndexType;
        public readonly ulong DataOffset;
        public readonly List<int> Indices;
        
        public SdkMeshIndexBuffer(BinaryReader reader) {

            NumIndices = reader.ReadUInt64();
            SizeBytes = reader.ReadUInt64();
            IndexType = reader.ReadUInt32();
            reader.ReadUInt32(); // padding
            DataOffset = reader.ReadUInt64();

            Indices = new List<int>();
            if (SizeBytes > 0) {
                ReadIndices(reader);
            }
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine("NumIndices: " + NumIndices);
            sb.AppendLine("SizeBytes: " + SizeBytes);
            sb.AppendLine("IndexType: " + IndexType);
            sb.AppendLine("DataOffset: " + DataOffset);
            sb.AppendLine("Number of indices in buffer: " + Indices.Count);
            return sb.ToString();
        }

        private void ReadIndices(BinaryReader reader) {
            var curPos = reader.BaseStream.Position;
            reader.BaseStream.Seek((long)DataOffset, SeekOrigin.Begin);
            for (ulong i = 0; i < NumIndices; i++) {
                int idx;
                if (IndexType == 0) {
                    idx = reader.ReadUInt16();
                    Indices.Add(idx);
                } else {
                    idx = reader.ReadInt32();
                    Indices.Add(idx);
                }
            }
            reader.BaseStream.Position = curPos;
        }

    }
}
