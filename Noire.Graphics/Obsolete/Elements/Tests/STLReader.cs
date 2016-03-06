using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Graphics.Obsolete.Interop;
using SharpDX;
using Noire.Misc;

namespace Noire.Graphics.Obsolete.Elements.Tests {
    internal static class STLReader {

        public static CustomVertex3[] ReadBinary(string filename) {
            if (!File.Exists(filename)) {
                return null;
            }
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
                try {
                    stream.Seek(80, SeekOrigin.Begin);
                    var triangleCount = stream.ReadInt32();
                    var vertices = new CustomVertex3[triangleCount * 3];
                    for (var i = 0; i < triangleCount; ++i) {
                        var normal1 = stream.ReadSingle();
                        var normal2 = stream.ReadSingle();
                        var normal3 = stream.ReadSingle();
                        for (var j = 0; j < 3; ++j) {
                            var v = new CustomVertex3();
                            v.Normals = new Vector3(normal1, normal2, normal3);
                            var f1 = stream.ReadSingle();
                            var f2 = stream.ReadSingle();
                            var f3 = stream.ReadSingle();
                            v.Position = new Vector3(f1, f2, f3);
                            vertices[i * 3 + j] = v;
                        }
                        stream.Seek(2, SeekOrigin.Current);
                    }
                    return vertices;
                } catch (IOException ex) {
                    Debug.Print(ex.Message);
                    return null;
                }
            }
        }

    }
}
