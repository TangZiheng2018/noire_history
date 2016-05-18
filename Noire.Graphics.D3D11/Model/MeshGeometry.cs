using System.Collections.Generic;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11.Model {
    public sealed class MeshGeometry : DisposeBase {

        public void SetVertices<TVertex>(Device device, List<TVertex> vertices) where TVertex : struct {
            Utilities.Dispose(ref _vb);
            _vertexStride = Marshal.SizeOf(typeof(TVertex));

            var vbd = new BufferDescription(
                _vertexStride * vertices.Count,
                ResourceUsage.Immutable,
                BindFlags.VertexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0
            );
            _vb = new Buffer(device, DataStream.Create(vertices.ToArray(), false, false), vbd);
        }

        public void SetIndices(Device device, List<int> indices) {
            var ibd = new BufferDescription(
                sizeof(int) * indices.Count,
                ResourceUsage.Immutable,
                BindFlags.IndexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0
            );
            _ib = new Buffer(device, DataStream.Create(indices.ToArray(), false, false), ibd);
        }

        public void SetSubsetTable(List<MeshSubset> subsets) {
            _subsetTable = subsets;
        }

        public void Draw(DeviceContext dc, int subsetId) {
            const int offset = 0;
            dc.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vb, _vertexStride, offset));
            dc.InputAssembler.SetIndexBuffer(_ib, SharpDX.DXGI.Format.R32_UInt, 0);
            dc.DrawIndexed(_subsetTable[subsetId].FaceCount * 3, _subsetTable[subsetId].FaceStart * 3, 0);
        }

        public void DrawInstanced(DeviceContext dc, int subsetId, Buffer instanceBuffer, int numInstances, int instanceStride) {
            const int offset = 0;
            dc.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vb, _vertexStride, offset), new VertexBufferBinding(instanceBuffer, instanceStride, 0));
            dc.InputAssembler.SetIndexBuffer(_ib, SharpDX.DXGI.Format.R32_UInt, 0);
            dc.DrawIndexedInstanced(_subsetTable[subsetId].FaceCount * 3, numInstances, _subsetTable[subsetId].FaceStart * 3, 0, 0);
        }

        protected override void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            if (disposing) {
                Utilities.Dispose(ref _vb);
                Utilities.Dispose(ref _ib);
            }
        }

        private Buffer _vb;
        private Buffer _ib;
        private int _vertexStride;
        private List<MeshSubset> _subsetTable;

    }
}
