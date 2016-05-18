using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Noire.Graphics.D3D11.Model.Internal {
    [StructLayout(LayoutKind.Sequential)]
    public struct SdkMeshHeader {

        public readonly uint Version;
        public readonly byte IsBigEndian;
        public readonly ulong HeaderSize;
        public readonly ulong NonBufferDataSize;
        public readonly ulong BufferDataSize;
        public readonly uint NumVertexBuffers;
        public readonly uint NumIndexBuffers;
        public readonly uint NumMeshes;
        public readonly uint NumTotalSubsets;
        public readonly uint NumFrames;
        public readonly uint NumMaterials;
        public readonly ulong VertexStreamHeaderOffset;
        public readonly ulong IndexStreamHeaderOffset;
        public readonly ulong MeshDataOffset;
        public readonly ulong SubsetDataOffset;
        public readonly ulong FrameDataOffset;
        public readonly ulong MaterialDataOffset;

        public SdkMeshHeader(BinaryReader reader) {
            Version = reader.ReadUInt32();
            IsBigEndian = reader.ReadByte();
            reader.ReadBytes(3); // allow for padding
            HeaderSize = reader.ReadUInt64();
            NonBufferDataSize = reader.ReadUInt64();
            BufferDataSize = reader.ReadUInt64();
            NumVertexBuffers = reader.ReadUInt32();
            NumIndexBuffers = reader.ReadUInt32();
            NumMeshes = reader.ReadUInt32();
            NumTotalSubsets = reader.ReadUInt32();
            NumFrames = reader.ReadUInt32();
            NumMaterials = reader.ReadUInt32();
            VertexStreamHeaderOffset = reader.ReadUInt64();
            IndexStreamHeaderOffset = reader.ReadUInt64();
            MeshDataOffset = reader.ReadUInt64();
            SubsetDataOffset = reader.ReadUInt64();
            FrameDataOffset = reader.ReadUInt64();
            MaterialDataOffset = reader.ReadUInt64();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var fieldInfo in GetType().GetFields()) {
                sb.AppendLine(fieldInfo.Name + ": " + fieldInfo.GetValue(this));
            }
            return sb.ToString();
        }

    }
}
