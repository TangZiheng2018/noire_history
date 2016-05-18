using System.Collections.Generic;
using Noire.Common;
using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11.Model {
    public abstract class ModelBase<TVertex> : DisposeBase {

        public MeshGeometry ModelMesh { get; protected set; }
        public int SubsetCount { get { return Subsets.Count; } }
        public List<Material> Materials { get; protected set; }
        public List<ShaderResourceView> DiffuseMapSRV { get; protected set; }
        public List<ShaderResourceView> NormalMapSRV { get; protected set; }
        public BoundingBox BoundingBox { get; protected set; }

        public abstract void CreateBox(Device device, float width, float height, float depth);
        public abstract void CreateSphere(Device device, float radius, int slices, int stacks);
        public abstract void CreateCylinder(Device device, float bottomRadius, float topRadius, float height, int sliceCount, int stackCount);
        public abstract void CreateGrid(Device device, float width, float depth, int xVerts, int zVerts);
        public abstract void CreateGeosphere(Device device, float radius, SubdivisionCount subdivisionCount);

        protected override void Dispose(bool disposing) {
            if (!IsDisposed) {
                if (disposing) {
                    var meshGeometry = ModelMesh;
                    Utilities.Dispose(ref meshGeometry);
                    Indices.Clear();
                    Vertices.Clear();
                    Indices = null;
                    Vertices = null;
                }
            }
        }

        protected ModelBase() {
            Subsets = new List<MeshSubset>();
            Vertices = new List<TVertex>();
            Indices = new List<int>();
            DiffuseMapSRV = new List<ShaderResourceView>();
            NormalMapSRV = new List<ShaderResourceView>();
            Materials = new List<Material>();
            ModelMesh = new MeshGeometry();
        }

        protected abstract void InitFromMeshData(Device device, GeometryGenerator.MeshData mesh);

        protected List<MeshSubset> Subsets;
        protected List<int> Indices;
        protected List<TVertex> Vertices;

    }
}
