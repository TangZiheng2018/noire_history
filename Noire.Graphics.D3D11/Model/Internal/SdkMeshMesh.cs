using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using SharpDX;

namespace Noire.Graphics.D3D11.Model.Internal {
    [StructLayout(LayoutKind.Sequential)]
    internal struct SdkMeshMesh {
        public readonly string Name;
        public readonly byte NumVertexBuffers;
        public readonly List<uint> VertexBuffers;
        public readonly uint IndexBuffer;
        public readonly uint NumSubsets;
        public readonly uint NumFrameInfluences; // bones

        public readonly Vector3 BoundingBoxCenter;
        public readonly Vector3 BoundingBoxExtents;

        public readonly ulong SubsetOffset;
        public readonly ulong FrameInfluenceOffset; // offset to bone data
        public readonly List<int> SubsetData;

        public SdkMeshMesh(BinaryReader reader) {

            Name = Encoding.Default.GetString(reader.ReadBytes(MaxMeshName));
            NumVertexBuffers = reader.ReadByte();
            reader.ReadBytes(3);
            VertexBuffers = new List<uint>();
            for (int j = 0; j < MaxVertexStreams; j++) {
                VertexBuffers.Add(reader.ReadUInt32());
            }
            IndexBuffer = reader.ReadUInt32();
            NumSubsets = reader.ReadUInt32();
            NumFrameInfluences = reader.ReadUInt32();
            BoundingBoxCenter = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            BoundingBoxExtents = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            reader.ReadUInt32();
            SubsetOffset = reader.ReadUInt64();
            FrameInfluenceOffset = reader.ReadUInt64();

            SubsetData = new List<int>();
            if (NumSubsets > 0) {
                ReadSubsets(reader);
            }
            // NOTE: not bothering with bone data now
        }

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine("Name: " + Name);
            sb.AppendLine("NumVertexBuffers: " + NumVertexBuffers);
            sb.Append("VertexBuffers: ");
            foreach (var vertexBuffer in VertexBuffers) {
                sb.Append(vertexBuffer + ", ");
            }
            sb.AppendLine();
            sb.AppendLine("IndexBuffer: " + IndexBuffer);
            sb.AppendLine("NumSubsets: " + NumSubsets);
            sb.AppendLine("NumFrameInfluences: " + NumFrameInfluences);
            sb.AppendLine("BoundingBoxCenter: " + BoundingBoxCenter);
            sb.AppendLine("BoundingBoxExtents: " + BoundingBoxExtents);
            sb.AppendLine("SubsetOffset: " + SubsetOffset);
            sb.AppendLine("FrameInfluenceOffset: " + SubsetOffset);
            sb.Append("Subsets: ");
            foreach (var i in SubsetData) {
                sb.Append(i + ", ");
            }
            sb.AppendLine();

            return sb.ToString();
        }

        private void ReadSubsets(BinaryReader reader) {
            var curPos = reader.BaseStream.Position;
            reader.BaseStream.Seek((long)SubsetOffset, SeekOrigin.Begin);
            for (var i = 0; i < NumSubsets; i++) {
                var subsetId = reader.ReadInt32();
                SubsetData.Add(subsetId);
            }

            reader.BaseStream.Position = curPos;
        }

        private static readonly int MaxMeshName = 100;
        private static readonly int MaxVertexStreams = 16;

    }
}
