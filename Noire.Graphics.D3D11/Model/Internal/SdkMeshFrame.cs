using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SharpDX;

namespace Noire.Graphics.D3D11.Model.Internal {
    [StructLayout(LayoutKind.Sequential)]
    internal struct SdkMeshFrame {

        public readonly string Name;
        public readonly uint Mesh;
        public readonly int ParentFrame;
        public readonly int ChildFrame;
        public readonly int SiblingFrame;
        public readonly Matrix Matrix;
        public readonly int AnimationDataIndex;

        public SdkMeshFrame(BinaryReader reader) {

            Name = Encoding.Default.GetString(reader.ReadBytes(MaxFrameName));
            if (Name[0] == '\0') {
                Name = "";
            }
            Mesh = reader.ReadUInt32();
            ParentFrame = reader.ReadInt32();
            ChildFrame = reader.ReadInt32();
            SiblingFrame = reader.ReadInt32();
            Matrix = new Matrix();
            for (int j = 0; j < 4; j++) {
                for (int k = 0; k < 4; k++) {
                    Matrix[k, j] = reader.ReadSingle();
                }
            }
            AnimationDataIndex = reader.ReadInt32();
        }

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var fieldInfo in GetType().GetFields().Where(fi => !fi.IsLiteral)) {
                sb.AppendLine(fieldInfo.Name + ": " + fieldInfo.GetValue(this));
            }
            return sb.ToString();
        }

        private static readonly int MaxFrameName = 100;

    }
}
