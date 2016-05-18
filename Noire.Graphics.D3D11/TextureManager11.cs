using System.Collections.Generic;
using System.IO;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;

namespace Noire.Graphics.D3D11 {
    public sealed class TextureManager11 : DisposeBase {

        static TextureManager11() {
            LockObject = new object();
        }

        public static void Initialize(Device device) {
            lock (LockObject) {
                if (_isInitialized) {
                    return;
                }
                _instance = new TextureManager11(device);
                _instance._textureSRVs.Add(TexDefault, _instance.Create1By1Tex(device, Color.White));
                _instance._textureSRVs.Add(TexDefaultNorm, _instance.Create1By1Tex(device, Color.DeepSkyBlue));
                _isInitialized = true;
            }
        }

        public static TextureManager11 Instance => _instance;

        public ShaderResourceView CreateColor1By1(Color color) {
            var key = color.ToAbgr().ToString();
            if (_textureSRVs.ContainsKey(key)) {
                return _textureSRVs[key];
            } else {
                var texture = Create1By1Tex(_device, color);
                _textureSRVs.Add(key, texture);
                return texture;
            }
        }

        public static bool IsInitialized {
            get {
                lock (LockObject) {
                    return _isInitialized;
                }
            }
        }

        public ShaderResourceView CreateTexture(string filePath) {
            if (_textureSRVs.ContainsKey(filePath)) {
                return _textureSRVs[filePath];
            } else {
                var view = TextureLoader.BitmapFromFile(_device, filePath).AsShaderResourceView();
                _textureSRVs.Add(filePath, view);
                return view;
            }
        }

        public ShaderResourceView CreateCubemap(string filePath) {
            if (_textureSRVs.ContainsKey(filePath)) {
                return _textureSRVs[filePath];
            } else {
                var faces = new[] {
                    "front", "back", "top", "bottom", "left", "right"
                };
                var baseTextureInfo = new FileInfo(filePath);
                var dirName = baseTextureInfo.DirectoryName;
                var baseName = baseTextureInfo.Name.Substring(0, baseTextureInfo.Name.Length - baseTextureInfo.Extension.Length);
                var extName = baseTextureInfo.Extension;
                var faceFileNames = new string[6];
                for (var i = 0; i < faceFileNames.Length; ++i) {
                    faceFileNames[i] = Path.Combine(dirName, baseName + "-" + faces[i] + extName);
                }
                var textures = new Texture2D[6];
                for (var i = 0; i < textures.Length; ++i) {
                    textures[i] = TextureLoader.BitmapFromFile(_device, faceFileNames[i]);
                }
                var view = TextureLoader.CubeMapFrom6Textures(_device, textures);
                _textureSRVs.Add(filePath, view);
                return view;
            }
        }

        public ShaderResourceView this[string key] => _textureSRVs[key];

        public static readonly string TexDefault = "default";
        public static readonly string TexDefaultNorm = "defaultNorm";

        protected override void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            if (disposing) {
                foreach (var view in _textureSRVs) {
                    var value = view.Value;
                    Utilities.Dispose(ref value);
                }
                _textureSRVs.Clear();
                _textureSRVs = null;
            }
        }

        private TextureManager11(Device device) {
            _textureSRVs = new Dictionary<string, ShaderResourceView>();
            _device = device;
        }

        private ShaderResourceView Create1By1Tex(Device device, Color color) {
            var desc2 = new Texture2DDescription {
                SampleDescription = new SampleDescription(1, 0),
                Width = 1,
                Height = 1,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.R8G8B8A8_UNorm,
                Usage = ResourceUsage.Dynamic,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.Write
            };
            var texture = new Texture2D(device, desc2);

            DataStream dataStream;
            var db = device.ImmediateContext.MapSubresource(texture, 0, 0, MapMode.WriteDiscard, MapFlags.None, out dataStream);
            dataStream.Write(color);
            Utilities.Dispose(ref dataStream);
            device.ImmediateContext.UnmapSubresource(texture, 0);

            return texture.AsShaderResourceView();
        }

        private static readonly object LockObject;
        private static bool _isInitialized;
        private static TextureManager11 _instance;
        private Device _device;
        private Dictionary<string, ShaderResourceView> _textureSRVs;

    }
}
