using System;
using System.Collections.Generic;

namespace Noire.Common {
    internal class Subdivider {
        private List<GeometryGenerator.Vertex> _vertices;
        private List<int> _indices;
        private Dictionary<Tuple<int, int>, int> _newVertices;

        public void Subdivide4(GeometryGenerator.MeshData mesh) {
            _newVertices = new Dictionary<Tuple<int, int>, int>();
            _vertices = mesh.Vertices;
            _indices = new List<int>();
            var numTris = mesh.Indices.Count / 3;

            for (var i = 0; i < numTris; i++) {
                //       i2
                //       *
                //      / \
                //     /   \
                //   a*-----*b
                //   / \   / \
                //  /   \ /   \
                // *-----*-----*
                // i1    c      i3

                var i1 = mesh.Indices[i * 3];
                var i2 = mesh.Indices[i * 3 + 1];
                var i3 = mesh.Indices[i * 3 + 2];

                var a = GetNewVertex(i1, i2);
                var b = GetNewVertex(i2, i3);
                var c = GetNewVertex(i3, i1);

                _indices.AddRange(new[] {
                i1, a, c,
                i2, b, a,
                i3, c, b,
                a, b, c
            });
            }
#if DEBUG
            Console.WriteLine(mesh.Vertices.Count);
#endif
            mesh.Indices = _indices;
        }

        private int GetNewVertex(int i1, int i2) {
            var t1 = new Tuple<int, int>(i1, i2);
            var t2 = new Tuple<int, int>(i2, i1);

            if (_newVertices.ContainsKey(t2)) {
                return _newVertices[t2];
            }
            if (_newVertices.ContainsKey(t1)) {
                return _newVertices[t1];
            }
            var newIndex = _vertices.Count;
            _newVertices.Add(t1, newIndex);

            _vertices.Add(new GeometryGenerator.Vertex() { Position = (_vertices[i1].Position + _vertices[i2].Position) * 0.5f });

            return newIndex;
        }
    }
}
