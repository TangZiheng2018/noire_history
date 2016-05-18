using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Noire.Graphics.D3D11.Model.Internal {

    /// <summary>
    /// Refer to https://dxut.codeplex.com/SourceControl/latest#Optional/SDKmesh.h
    /// and https://dxut.codeplex.com/SourceControl/latest#Optional/SDKmesh.cpp
    /// </summary>
    internal class SdkMesh {

        internal readonly List<SdkMeshVertexBuffer> VertexBuffers = new List<SdkMeshVertexBuffer>();
        internal readonly List<SdkMeshIndexBuffer> IndexBuffers = new List<SdkMeshIndexBuffer>();
        internal readonly List<SdkMeshMesh> Meshes = new List<SdkMeshMesh>();
        internal readonly List<SdkMeshSubset> Subsets = new List<SdkMeshSubset>();
        internal readonly List<SdkMeshFrame> Frames = new List<SdkMeshFrame>();
        internal readonly List<SdkMeshMaterial> Materials = new List<SdkMeshMaterial>();

        public override string ToString() {
            var sb = new StringBuilder();
            sb.AppendLine(_header.ToString());
            foreach (var vertexBuffer in VertexBuffers) {
                sb.AppendLine(vertexBuffer.ToString());
            }
            foreach (var indexBuffer in IndexBuffers) {
                sb.AppendLine(indexBuffer.ToString());
            }
            foreach (var mesh in Meshes) {
                sb.AppendLine(mesh.ToString());
            }
            foreach (var subset in Subsets) {
                sb.AppendLine(subset.ToString());
            }
            foreach (var frame in Frames) {
                sb.AppendLine(frame.ToString());
            }
            foreach (var material in Materials) {
                sb.AppendLine(material.ToString());
            }
            return sb.ToString();
        }

        public SdkMesh(string filename) {
            using (var reader = new BinaryReader(new FileStream(filename, FileMode.Open))) {
                _header = new SdkMeshHeader(reader);
                for (int i = 0; i < _header.NumVertexBuffers; i++) {
                    VertexBuffers.Add(new SdkMeshVertexBuffer(reader));
                }
                for (int i = 0; i < _header.NumIndexBuffers; i++) {
                    IndexBuffers.Add(new SdkMeshIndexBuffer(reader));
                }
                for (int i = 0; i < _header.NumMeshes; i++) {
                    Meshes.Add(new SdkMeshMesh(reader));
                }
                for (int i = 0; i < _header.NumTotalSubsets; i++) {
                    Subsets.Add(new SdkMeshSubset(reader));
                }
                for (int i = 0; i < _header.NumFrames; i++) {
                    Frames.Add(new SdkMeshFrame(reader));
                }
                for (int i = 0; i < _header.NumMaterials; i++) {
                    Materials.Add(new SdkMeshMaterial(reader));
                }
            }
        }

        private SdkMeshHeader _header;

    }
}
