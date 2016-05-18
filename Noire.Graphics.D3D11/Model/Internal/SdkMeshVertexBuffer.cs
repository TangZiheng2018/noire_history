using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Noire.Common.Vertices;
using SharpDX;
using SharpDX.Direct3D9;

namespace Noire.Graphics.D3D11.Model.Internal {
    [StructLayout(LayoutKind.Sequential)]
    internal struct SdkMeshVertexBuffer {
        private const int MaxVertexElements = 32;

        public readonly ulong NumVertices;
        public readonly ulong SizeBytes;
        public readonly ulong StrideBytes;
        public readonly List<VertexElement> Decl;
        public readonly ulong DataOffset;

        public readonly List<VertPosNormTexTan> Vertices;

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine("NumVertices: " + NumVertices);
            sb.AppendLine("SizeBytes: " + SizeBytes);
            sb.AppendLine("StrideBytes: " + StrideBytes);
            sb.AppendLine("Decl: ");
            foreach (var elem in Decl) {
                sb.AppendLine("\tVertexElement(Stream: " + elem.Stream + " Offset: " + elem.Offset + " Type: " + elem.Type + " Method: " + elem.Method + " Usage: " + elem.Usage + " UsageIndex: " + elem.UsageIndex + ")");
            }
            sb.AppendLine("DataOffset: " + DataOffset);
            sb.AppendLine("Vertices in vertex buffer: " + Vertices.Count);
            return sb.ToString();
        }

        public SdkMeshVertexBuffer(BinaryReader reader) {

            NumVertices = reader.ReadUInt64();
            SizeBytes = reader.ReadUInt64();
            StrideBytes = reader.ReadUInt64();
            Decl = new List<VertexElement>();
            var processElem = true;
            for (int j = 0; j < MaxVertexElements; j++) {
                var stream = reader.ReadUInt16();
                var offset = reader.ReadUInt16();
                var type = reader.ReadByte();
                var method = reader.ReadByte();
                var usage = reader.ReadByte();
                var usageIndex = reader.ReadByte();
                if (stream < 16 && processElem) {
                    var element = new VertexElement((short)stream, (short)offset, (DeclarationType)type, (DeclarationMethod)method, (DeclarationUsage)usage, usageIndex);
                    Decl.Add(element);
                } else {
                    processElem = false;
                }
            }
            DataOffset = reader.ReadUInt64();
            Vertices = new List<VertPosNormTexTan>();
            if (SizeBytes > 0) {
                ReadVertices(reader);
            }
        }

        private void ReadVertices(BinaryReader reader) {
            var curPos = reader.BaseStream.Position;
            reader.BaseStream.Seek((long)DataOffset, SeekOrigin.Begin);
            //var data = reader.ReadBytes((int) vbHeader.SizeBytes);
            for (ulong i = 0; i < NumVertices; i++) {
                var vertex = new VertPosNormTexTan();
                foreach (var element in Decl) {
                    switch (element.Type) {
                        case DeclarationType.Float3:
                            var v3 = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                            switch (element.Usage) {
                                case DeclarationUsage.Position:
                                    vertex.Pos = v3;
                                    break;
                                case DeclarationUsage.Normal:
                                    vertex.Normal = v3;
                                    break;
                                case DeclarationUsage.Tangent:
                                    vertex.Tan = v3;
                                    break;
                            }
                            //Console.WriteLine("{0} - {1}", element.Usage, v3);
                            break;
                        case DeclarationType.Float2:
                            var v2 = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                            switch (element.Usage) {
                                case DeclarationUsage.TextureCoordinate:
                                    vertex.Tex = v2;
                                    break;
                            }
                            //Console.WriteLine("{0} - {1}", element.Usage, v2);
                            break;
                    }
                }
                Vertices.Add(vertex);
            }
            reader.BaseStream.Position = curPos;
        }
    }
}
