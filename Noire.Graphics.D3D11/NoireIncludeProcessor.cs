using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.D3DCompiler;

namespace Noire.Graphics.D3D11 {
    public sealed class NoireIncludeProcessor : Include {

        internal NoireIncludeProcessor()
            : this(string.Empty) {
        }

        internal NoireIncludeProcessor(string baseDirectory) {
            _baseDirectory = baseDirectory;
        }

        public void Dispose() {
            Close(null);
            Shadow?.Dispose();
        }

        public IDisposable Shadow { get; set; }

        public Stream Open(IncludeType type, string fileName, Stream parentStream) {
            switch (type) {
                case IncludeType.Local:
                    fileName = Path.Combine(_baseDirectory, fileName);
                    break;
                case IncludeType.System:
                    break;
                default:
                    break;
            }
            using (var streamReader = new StreamReader(fileName, Encoding.UTF8, true)) {
                var text = streamReader.ReadToEnd();
                var textBytes = Encoding.ASCII.GetBytes(text);
                return new MemoryStream(textBytes);
            }
        }

        public void Close(Stream stream) {
            Utilities.Dispose(ref stream);
        }
        
        private readonly string _baseDirectory;

    }
}
