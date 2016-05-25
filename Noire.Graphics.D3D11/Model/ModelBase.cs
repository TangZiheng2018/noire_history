using System.Collections.Generic;
using Noire.Common;
using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11.Model {
    public abstract class ModelBase<TVertex> : DisposeBase {

        public MeshGeometry ModelMesh { get; protected set; }

        public int SubsetCount => Subsets.Count;

        public List<Material> Materials { get; protected set; }
        public List<ShaderResourceView> DiffuseMapSRV { get; protected set; }
        public List<ShaderResourceView> NormalMapSRV { get; protected set; }
        public BoundingBox BoundingBox { get; protected set; }

        protected abstract void CreateBoxInternal(Device device, float width, float height, float depth);
        protected abstract void CreateSphereInternal(Device device, float radius, int slices, int stacks);
        protected abstract void CreateCylinderInternal(Device device, float bottomRadius, float topRadius, float height, int sliceCount, int stackCount);
        protected abstract void CreateGridInternal(Device device, float width, float depth, int xVerts, int zVerts);
        protected abstract void CreateGeosphereInternal(Device device, float radius, SubdivisionCount subdivisionCount);

        protected override void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            if (disposing) {
                var meshGeometry = ModelMesh;
                Utilities.Dispose(ref meshGeometry);
                Indices.Clear();
                Vertices.Clear();
                Indices = null;
                Vertices = null;
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
