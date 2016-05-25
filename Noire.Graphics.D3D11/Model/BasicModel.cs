using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Assimp;
using Noire.Common;
using Noire.Common.Vertices;
using Noire.Graphics.D3D11.Model.Internal;
using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11.Model {
    public class BasicModel : ModelBase<VertPosNormTexTan> {

        public static BasicModel Create(Device device, TextureManager11 textureManager, string filePath, string texturePath, bool autoLoadTextures = true, bool flipUv = false, bool tex1By1 = true) {
            return new BasicModel(device, textureManager, filePath, texturePath, autoLoadTextures, flipUv, tex1By1);
        }

        public void ReplaceWithSingleDiffuseTexture(ShaderResourceView texture) {
            DiffuseMapSRV.Clear();
            DiffuseMapSRV.AddRange(texture.Repeat(_meshCount));
        }

        public void ReplaceWithSingleNormalTexture(ShaderResourceView texture) {
            NormalMapSRV.Clear();
            NormalMapSRV.AddRange(texture.Repeat(_meshCount));
        }

        public static BasicModel LoadFromTxtFile(Device device, string filename) {
            var vertices = new List<VertPosNormTex>();
            var indices = new List<int>();
            var vcount = 0;
            var tcount = 0;
            using (var reader = new StreamReader(filename)) {

                var input = reader.ReadLine();
                if (input != null)
                    // VertexCount: X
                    vcount = Convert.ToInt32(input.Split(new[] { ':' })[1].Trim());

                input = reader.ReadLine();
                if (input != null)
                    //TriangleCount: X
                    tcount = Convert.ToInt32(input.Split(new[] { ':' })[1].Trim());

                // skip ahead to the vertex data
                do {
                    input = reader.ReadLine();
                } while (input != null && !input.StartsWith("{"));
                // Get the vertices  
                for (int i = 0; i < vcount; i++) {
                    input = reader.ReadLine();
                    if (input != null) {
                        var vals = input.Split(new[] { ' ' });
                        vertices.Add(
                                     new VertPosNormTex(
                                         new Vector3(
                                             Convert.ToSingle(vals[0].Trim(), CultureInfo.InvariantCulture),
                                             Convert.ToSingle(vals[1].Trim(), CultureInfo.InvariantCulture),
                                             Convert.ToSingle(vals[2].Trim(), CultureInfo.InvariantCulture)),
                                         new Vector3(
                                             Convert.ToSingle(vals[3].Trim(), CultureInfo.InvariantCulture),
                                             Convert.ToSingle(vals[4].Trim(), CultureInfo.InvariantCulture),
                                             Convert.ToSingle(vals[5].Trim(), CultureInfo.InvariantCulture)),
                                         new Vector2()
                                         )
                            );
                    }
                }
                // skip ahead to the index data
                do {
                    input = reader.ReadLine();
                } while (input != null && !input.StartsWith("{"));
                // Get the indices

                for (var i = 0; i < tcount; i++) {
                    input = reader.ReadLine();
                    if (input == null) {
                        break;
                    }
                    var m = input.Trim().Split(new[] { ' ' });
                    indices.Add(Convert.ToInt32(m[0].Trim()));
                    indices.Add(Convert.ToInt32(m[1].Trim()));
                    indices.Add(Convert.ToInt32(m[2].Trim()));
                }
            }
            var ret = new BasicModel();

            var subset = new MeshSubset() {
                FaceCount = indices.Count / 3,
                FaceStart = 0,
                VertexCount = vertices.Count,
                VertexStart = 0
            };
            ret.Subsets.Add(subset);
            var max = new Vector3(float.MinValue);
            var min = new Vector3(float.MaxValue);
            foreach (var vertex in vertices) {
                max = MathF.Maximize(max, vertex.Position);
                min = MathF.Minimize(min, vertex.Position);
            }
            ret.BoundingBox = new BoundingBox(min, max);

            ret.Vertices.AddRange(vertices.Select(v => new VertPosNormTexTan(v.Position, v.Normal, v.TextureCoords, new Vector3(1, 0, 0))).ToList());
            ret.Indices.AddRange(indices.Select(i => i));

            ret.Materials.Add(new Noire.Common.Material { Ambient = Color.Gray, Diffuse = Color.White, Specular = new Color(1f, 1f, 1f, 16f) });
            ret.DiffuseMapSRV.Add(null);
            ret.NormalMapSRV.Add(null);

            ret.ModelMesh.SetSubsetTable(ret.Subsets);
            ret.ModelMesh.SetVertices(device, ret.Vertices);
            ret.ModelMesh.SetIndices(device, ret.Indices);

            ret._meshCount = 1;

            return ret;
        }

        public static BasicModel LoadSdkMesh(Device device, TextureManager11 texMgr, string filename, string texturePath) {
            // NOTE: this assumes that the model file only contains a single mesh
            var sdkMesh = new SdkMesh(filename);
            var ret = new BasicModel();

            var faceStart = 0;
            var vertexStart = 0;
            foreach (var sdkMeshSubset in sdkMesh.Subsets) {
                var subset = new MeshSubset() {
                    FaceCount = (int)(sdkMeshSubset.IndexCount / 3),
                    FaceStart = faceStart,
                    VertexCount = (int)sdkMeshSubset.VertexCount,
                    VertexStart = vertexStart
                };
                // fixup any subset indices that assume that all vertices and indices are not in the same buffers
                faceStart = subset.FaceStart + subset.FaceCount;
                vertexStart = subset.VertexStart + subset.VertexCount;
                ret.Subsets.Add(subset);
            }
            ret._meshCount = ret.SubsetCount;

            var max = new Vector3(float.MinValue);
            var min = new Vector3(float.MaxValue);
            foreach (var vb in sdkMesh.VertexBuffers) {
                foreach (var vertex in vb.Vertices) {
                    max = MathF.Maximize(max, vertex.Pos);
                    min = MathF.Minimize(min, vertex.Pos);
                    ret.Vertices.Add(vertex);
                }
            }
            ret.BoundingBox = new BoundingBox(min, max);

            foreach (var ib in sdkMesh.IndexBuffers) {
                ret.Indices.AddRange(ib.Indices.Select(i => i));
            }
            foreach (var sdkMeshMaterial in sdkMesh.Materials) {
                var material = new Noire.Common.Material {
                    Ambient = sdkMeshMaterial.Ambient,
                    Diffuse = sdkMeshMaterial.Diffuse,
                    Reflect = Color.Black,
                    Specular = sdkMeshMaterial.Specular
                };
                material.Specular.Alpha = sdkMeshMaterial.Power;
                ret.Materials.Add(material);
                if (!string.IsNullOrEmpty(sdkMeshMaterial.DiffuseTexture)) {
                    ret.DiffuseMapSRV.Add(texMgr.CreateTexture(Path.Combine(texturePath, sdkMeshMaterial.DiffuseTexture)));
                } else {
                    ret.DiffuseMapSRV.Add(texMgr[TextureManager11.TexDefault]);
                }
                if (!string.IsNullOrEmpty(sdkMeshMaterial.NormalTexture)) {
                    ret.NormalMapSRV.Add(texMgr.CreateTexture(Path.Combine(texturePath, sdkMeshMaterial.NormalTexture)));
                } else {
                    ret.NormalMapSRV.Add(texMgr[TextureManager11.TexDefaultNorm]);
                }
            }
            ret.ModelMesh.SetSubsetTable(ret.Subsets);
            ret.ModelMesh.SetVertices(device, ret.Vertices);
            ret.ModelMesh.SetIndices(device, ret.Indices);

            return ret;
        }

        public static BasicModel CreateBox(Device device, float width, float height, float depth) {
            var model = new BasicModel();
            model.CreateBoxInternal(device, width, height, depth);
            return model;
        }

        public static BasicModel CreateSphere(Device device, float radius, int slices, int stacks) {
            var model = new BasicModel();
            model.CreateSphereInternal(device, radius, slices, stacks);
            return model;
        }

        public static BasicModel CreateCylinder(Device device, float bottomRadius, float topRadius, float height, int sliceCount, int stackCount) {
            var model = new BasicModel();
            model.CreateCylinderInternal(device, bottomRadius, topRadius, height, sliceCount, stackCount);
            return model;
        }

        public static BasicModel CreateGrid(Device device, float width, float depth, int xVerts, int yVerts) {
            var model = new BasicModel();
            model.CreateGridInternal(device, width, depth, xVerts, yVerts);
            return model;
        }

        public static BasicModel CreateGeosphere(Device device, float radius, SubdivisionCount numSubdivisions) {
            var model = new BasicModel();
            model.CreateGeosphereInternal(device, radius, numSubdivisions);
            return model;
        }

        protected override void CreateBoxInternal(Device device, float width, float height, float depth) {
            var box = GeometryGenerator.CreateBox(width, height, depth);
            InitFromMeshData(device, box);
            _meshCount = 1;
        }

        protected override void CreateSphereInternal(Device device, float radius, int slices, int stacks) {
            var sphere = GeometryGenerator.CreateSphere(radius, slices, stacks);
            InitFromMeshData(device, sphere);
            _meshCount = 1;
        }

        protected override void CreateCylinderInternal(Device device, float bottomRadius, float topRadius, float height, int sliceCount, int stackCount) {
            var cylinder = GeometryGenerator.CreateCylinder(bottomRadius, topRadius, height, sliceCount, stackCount);
            InitFromMeshData(device, cylinder);
            _meshCount = 1;
        }

        protected override void CreateGridInternal(Device device, float width, float depth, int xVerts, int zVerts) {
            var grid = GeometryGenerator.CreateGrid(width, depth, xVerts, zVerts);
            InitFromMeshData(device, grid);
            _meshCount = 1;
        }

        protected override void CreateGeosphereInternal(Device device, float radius, SubdivisionCount numSubdivisions) {
            var geosphere = GeometryGenerator.CreateGeosphere(radius, numSubdivisions);
            InitFromMeshData(device, geosphere);
            _meshCount = 1;
        }

        protected override void InitFromMeshData(Device device, GeometryGenerator.MeshData mesh) {
            var subset = new MeshSubset() {
                FaceCount = mesh.Indices.Count / 3,
                FaceStart = 0,
                VertexCount = mesh.Vertices.Count,
                VertexStart = 0
            };
            Subsets.Add(subset);

            var max = new Vector3(float.MinValue);
            var min = new Vector3(float.MaxValue);
            foreach (var vertex in mesh.Vertices) {
                max = MathF.Maximize(max, vertex.Position);
                min = MathF.Minimize(min, vertex.Position);
            }

            BoundingBox = new BoundingBox(min, max);

            Vertices = mesh.Vertices.Select(v => new VertPosNormTexTan(v.Position, v.Normal, v.TexCoords, v.TangentU)).ToList();
            Indices = mesh.Indices.Select(i => i).ToList();

            Materials.Add(new Noire.Common.Material { Ambient = Color.Gray, Diffuse = Color.White, Specular = new Color(1f, 1f, 1f, 16f) });
            DiffuseMapSRV.Add(null);
            NormalMapSRV.Add(null);

            ModelMesh.SetSubsetTable(Subsets);
            ModelMesh.SetVertices(device, Vertices);
            ModelMesh.SetIndices(device, Indices);
        }

        private BasicModel() {
            _meshCount = 0;
        }

        private BasicModel(Device device, TextureManager11 textureManager, string filename, string texturePath, bool autoLoadTextures, bool flipUv, bool tex1By1) {
            var importer = new AssimpContext();
            if (!importer.IsImportFormatSupported(Path.GetExtension(filename))) {
                throw new ArgumentException($"Model format {Path.GetExtension(filename)} is not supported. Cannot load {filename}.", nameof(filename));
            }
#if DEBUG
            var logStream = new ConsoleLogStream();
            logStream.Attach();
#endif
            var postProcessFlags = PostProcessSteps.GenerateSmoothNormals | PostProcessSteps.CalculateTangentSpace;
            if (flipUv) {
                postProcessFlags |= PostProcessSteps.FlipUVs;
            }
            var model = importer.ImportFile(filename, postProcessFlags);

            var min = new Vector3(float.MaxValue);
            var max = new Vector3(float.MinValue);

            _meshCount = model.Meshes.Count;

            foreach (var mesh in model.Meshes) {
                var verts = new List<VertPosNormTexTan>();
                var subset = new MeshSubset() {
                    VertexCount = mesh.VertexCount,
                    VertexStart = Vertices.Count,
                    FaceStart = Indices.Count / 3,
                    FaceCount = mesh.FaceCount
                };
                Subsets.Add(subset);
                // bounding box corners

                for (var i = 0; i < mesh.VertexCount; i++) {
                    var pos = mesh.HasVertices ? mesh.Vertices[i].ToVector3() : new Vector3();
                    min = MathF.Minimize(min, pos);
                    max = MathF.Maximize(max, pos);

                    var norm = mesh.HasNormals ? mesh.Normals[i] : new Vector3D();
                    var texC = mesh.HasTextureCoords(0) ? mesh.TextureCoordinateChannels[0][i] : (tex1By1 ? new Vector3D(1, 1, 0) : new Vector3D());
                    var tan = mesh.HasTangentBasis ? mesh.Tangents[i] : new Vector3D();
                    var v = new VertPosNormTexTan(pos, norm.ToVector3(), texC.ToVector2(), tan.ToVector3());
                    verts.Add(v);
                }

                Vertices.AddRange(verts);

                var indices = mesh.GetIndices().Select(i => i + subset.VertexStart).ToList();
                Indices.AddRange(indices);

                var mat = model.Materials[mesh.MaterialIndex];
                var material = mat.ToMaterial();

                Materials.Add(material);

                if (autoLoadTextures) {
                    TextureSlot diffuseSlot;
                    mat.GetMaterialTexture(TextureType.Diffuse, 0, out diffuseSlot);
                    var diffusePath = diffuseSlot.FilePath;
                    if (Path.GetExtension(diffusePath) == ".tga") {
                        // DirectX doesn't like to load tgas, so you will need to convert them to pngs yourself with an image editor
                        diffusePath = diffusePath.Replace(".tga", ".png");
                    }
                    if (!string.IsNullOrEmpty(diffusePath)) {
                        DiffuseMapSRV.Add(textureManager.CreateTexture(Path.Combine(texturePath, diffusePath)));
                    } else {
                        DiffuseMapSRV.Add(textureManager.CreateColor1By1(material.Diffuse.ToColor()));
                    }
                    TextureSlot normalSlot;
                    mat.GetMaterialTexture(TextureType.Normals, 0, out normalSlot);
                    var normalPath = normalSlot.FilePath;
                    if (!string.IsNullOrEmpty(normalPath)) {
                        NormalMapSRV.Add(textureManager.CreateTexture(Path.Combine(texturePath, normalPath)));
                    } else {
                        if (diffusePath != null) {
                            var normalExt = Path.GetExtension(diffusePath);
                            normalPath = Path.GetFileNameWithoutExtension(diffusePath) + "_nmap" + normalExt;
                            NormalMapSRV.Add(textureManager.CreateTexture(Path.Combine(texturePath, normalPath)));
                        } else {
                            NormalMapSRV.Add(textureManager[TextureManager11.TexDefaultNorm]);
                        }
                    }
                }
            }

            BoundingBox = new BoundingBox(min, max);
            ModelMesh.SetSubsetTable(Subsets);
            ModelMesh.SetVertices(device, Vertices);
            ModelMesh.SetIndices(device, Indices);
        }

        private int _meshCount;

    }
}
