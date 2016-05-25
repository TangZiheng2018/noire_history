using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Noire.Common;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WIC;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Resource = SharpDX.Direct3D11.Resource;

namespace Noire.Graphics.D3D11 {
    public static class TextureLoader {

        static TextureLoader() {
            SyncObject = new object();
        }

        public static void Initialize() {
            lock (SyncObject) {
                if (!_isInitailized) {
                    _factory = new ImagingFactory();
                    _isInitailized = true;
                }
            }
        }

        public static void Dispose() {
            Utilities.Dispose(ref _factory);
        }

        public static Texture2D CreateTextureFromFile(Device device, string filename) {
            return CreateTextureFromFile(device, filename, NormalTextureOptions);
        }

        public static Texture2D CreateTextureFromFile(Device device, string filename, TextureLoadOptions options) {
            using (var bitmapSource = LoadBitmapSourceFromFile(_factory, filename)) {
                return CreateTexture2DFromBitmapSource(device, bitmapSource, options);
            }
        }

        /// <summary>
        /// 原始思想：http://stackoverflow.com/questions/19364012/d3d11-creating-a-cube-map-from-6-images
        /// 上面的链接页面中，OptionFlags 的参数错误，后参考 http://www.gamedev.net/topic/647237-dx11-cube-texture-creation/ 做出修正。
        /// </summary>
        /// <param name="device"></param>
        /// <param name="texture2Ds"></param>
        /// <returns></returns>
        public static ShaderResourceView CreateCubeMapFrom6Textures(Device device, Texture2D[] texture2Ds) {
            Debug.Assert(texture2Ds.Length == 6);
            var texElemDesc = texture2Ds[0].Description;
            var texArrayDesc = new Texture2DDescription() {
                Width = texElemDesc.Width,
                Height = texElemDesc.Height,
                MipLevels = texElemDesc.MipLevels,
                ArraySize = 6,
                Format = texElemDesc.Format,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.TextureCube
            };
            var texArray = new Texture2D(device, texArrayDesc);
            var context = device.ImmediateContext;
            var sourceRegion = new ResourceRegion();
            for (var i = 0; i < 6; ++i) {
                for (var mipLevel = 0; mipLevel < texArrayDesc.MipLevels; ++mipLevel) {
                    sourceRegion.Left = 0;
                    sourceRegion.Right = texArrayDesc.Width >> mipLevel;
                    sourceRegion.Top = 0;
                    sourceRegion.Bottom = texArrayDesc.Height >> mipLevel;
                    sourceRegion.Front = 0;
                    sourceRegion.Back = 1;
                    if (sourceRegion.Bottom <= 0 || sourceRegion.Right <= 0) {
                        break;
                    }
                    var n = Resource.CalculateSubResourceIndex(mipLevel, i, texArrayDesc.MipLevels);
                    context.CopySubresourceRegion(texture2Ds[i], mipLevel, sourceRegion, texArray, n);
                }
            }

            var viewDesc = new ShaderResourceViewDescription() {
                Format = texArrayDesc.Format,
                Dimension = ShaderResourceViewDimension.TextureCube,
                TextureCube = new ShaderResourceViewDescription.TextureCubeResource() {
                    MostDetailedMip = 0,
                    MipLevels = texArrayDesc.MipLevels
                }
            };
            return new ShaderResourceView(device, texArray, viewDesc);
        }

        public static ShaderResourceView CreateTexture2DArray(Device device, DeviceContext context, string[] filePaths) {
            return CreateTexture2DArray(device, context, filePaths, TextureArrayOptions);
        }

        public static ShaderResourceView CreateTexture2DArray(Device device, DeviceContext context, string[] filePaths, TextureLoadOptions options) {
            var srcTex = new Texture2D[filePaths.Length];
            for (var i = 0; i < filePaths.Length; i++) {
                srcTex[i] = CreateTextureFromFile(device, filePaths[i], options);
            }
            var texElementDesc = srcTex[0].Description;

            var texArrayDesc = new Texture2DDescription {
                Width = texElementDesc.Width,
                Height = texElementDesc.Height,
                MipLevels = texElementDesc.MipLevels,
                ArraySize = srcTex.Length,
                Format = texElementDesc.Format,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };

            var texArray = new Texture2D(device, texArrayDesc);
            texArray.DebugName = "texture array: + " + filePaths.Aggregate((i, j) => i + ", " + j);
            for (int texElement = 0; texElement < srcTex.Length; texElement++) {
                for (int mipLevel = 0; mipLevel < texElementDesc.MipLevels; mipLevel++) {
                    int mippedSize;
                    DataBox mappedTex2D;
                    mappedTex2D = context.MapSubresource(srcTex[texElement], mipLevel, 0, MapMode.Read, MapFlags.None, out mippedSize);

                    context.UpdateSubresource(
                        mappedTex2D,
                        texArray,
                        Resource.CalculateSubResourceIndex(mipLevel, texElement, texElementDesc.MipLevels)
                        );
                    context.UnmapSubresource(srcTex[texElement], mipLevel);
                }
            }
            var viewDesc = new ShaderResourceViewDescription {
                Format = texArrayDesc.Format,
                Dimension = ShaderResourceViewDimension.Texture2DArray,
                Texture2DArray = new ShaderResourceViewDescription.Texture2DArrayResource() {
                    MostDetailedMip = 0,
                    MipLevels = texArrayDesc.MipLevels,
                    FirstArraySlice = 0,
                    ArraySize = srcTex.Length
                }
            };

            var texArraySRV = new ShaderResourceView(device, texArray, viewDesc);

            Utilities.Dispose(ref texArray);
            for (int i = 0; i < srcTex.Length; i++) {
                Utilities.Dispose(ref srcTex[i]);
            }

            return texArraySRV;
        }

        public static ShaderResourceView CreateRandomTexture1D(Device device) {
            var randomValues = new List<Vector4>();
            for (var i = 0; i < 1024; i++) {
                randomValues.Add(new Vector4(MathF.Rand.NextFloat(-1.0f, 1.0f), MathF.Rand.NextFloat(-1.0f, 1.0f), MathF.Rand.NextFloat(-1.0f, 1.0f), MathF.Rand.NextFloat(-1.0f, 1.0f)));
            }
            var texDesc = new Texture1DDescription() {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.R32G32B32A32_Float,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                Usage = ResourceUsage.Immutable,
                Width = 1024
            };
            var randTex = new Texture1D(device, texDesc, DataStream.Create(randomValues.ToArray(), false, false));

            var viewDesc = new ShaderResourceViewDescription() {
                Format = texDesc.Format,
                Dimension = ShaderResourceViewDimension.Texture1D,
                Texture1D = new ShaderResourceViewDescription.Texture1DResource() {
                    MipLevels = texDesc.MipLevels,
                    MostDetailedMip = 0
                }
            };
            var randTexSRV = new ShaderResourceView(device, randTex, viewDesc);
            Utilities.Dispose(ref randTex);
            return randTexSRV;
        }

        public static ShaderResourceView AsShaderResourceView(this Texture2D texture2D) {
            return new ShaderResourceView(texture2D.Device, texture2D);
        }

        private static BitmapSource LoadBitmapSourceFromFile(ImagingFactory factory, string filename) {
            using (var bitmapDecoder = new BitmapDecoder(factory, filename, DecodeOptions.CacheOnDemand)) {
                var result = new FormatConverter(factory);
                using (var bitmapFrameDecode = bitmapDecoder.GetFrame(0)) {
                    result.Initialize(bitmapFrameDecode, PixelFormat.Format32bppPRGBA, BitmapDitherType.None, null, 0, BitmapPaletteType.Custom);
                }
                return result;
            }
        }

        private static Texture2D CreateTexture2DFromBitmapSource(Device device, BitmapSource bitmapSource, TextureLoadOptions options) {
            // Allocate DataStream to receive the WIC image pixels
            var stride = bitmapSource.Size.Width * 4;
            using (var buffer = new DataStream(bitmapSource.Size.Height * stride, true, true)) {
                // Copy the content of the WIC to the buffer
                bitmapSource.CopyPixels(stride, buffer);
                var texture2DDescription = new Texture2DDescription() {
                    Width = bitmapSource.Size.Width,
                    Height = bitmapSource.Size.Height,
                    ArraySize = 1,
                    BindFlags = options.BindFlags,
                    Usage = options.ResourceUsage,
                    CpuAccessFlags = options.CpuAccessFlags,
                    Format = options.Format,
                    MipLevels = options.MipLevels,
                    OptionFlags = ResourceOptionFlags.None,
                    SampleDescription = new SampleDescription(1, 0),
                };
                bitmapSource.Dispose();
                var dataRectangle = new DataRectangle(buffer.DataPointer, stride);
                return new Texture2D(device, texture2DDescription, dataRectangle);
            }
        }

        private static ImagingFactory _factory;
        private static bool _isInitailized;
        private static readonly object SyncObject;

        public struct TextureLoadOptions {

            public Format Format;
            public ResourceUsage ResourceUsage;
            public BindFlags BindFlags;
            public CpuAccessFlags CpuAccessFlags;
            public int MipLevels;

        }

        private static readonly TextureLoadOptions NormalTextureOptions = new TextureLoadOptions() {
            Format = Format.R8G8B8A8_UNorm,
            BindFlags = BindFlags.ShaderResource,
            CpuAccessFlags = CpuAccessFlags.None,
            ResourceUsage = ResourceUsage.Immutable,
            MipLevels = 1
        };

        // Inspired by http://www.gamedev.net/topic/636900-how-to-create-a-texture2darray-from-files-in-dx11/
        private static readonly TextureLoadOptions TextureArrayOptions = new TextureLoadOptions() {
            Format = Format.R8G8B8A8_UNorm,
            BindFlags = BindFlags.None,
            CpuAccessFlags = CpuAccessFlags.Read,
            ResourceUsage = ResourceUsage.Staging,
            MipLevels = 1
        };

    }
}
