using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.WIC;
using Device = SharpDX.Direct3D11.Device;
using Resource = SharpDX.Direct3D11.Resource;

namespace Noire.Graphics.D3D11 {
    public static class TextureLoader {

        static TextureLoader() {
            _syncObject = new object();
        }

        public static void Initialize() {
            lock (_syncObject) {
                if (!_isInitailized) {
                    _factory = new ImagingFactory();
                    _isInitailized = true;
                }
            }
        }

        public static void Dispose() {
            Utilities.Dispose(ref _factory);
        }

        public static Texture2D BitmapFromFile(Device device, string filename) {
            using (var bitmapSource = LoadBitmap(_factory, filename)) {
                return CreateTexture2DFromBitmap(device, bitmapSource);
            }
        }

        /// <summary>
        /// 原始思想：http://stackoverflow.com/questions/19364012/d3d11-creating-a-cube-map-from-6-images
        /// 上面的链接页面中，OptionFlags 的参数错误，后参考 http://www.gamedev.net/topic/647237-dx11-cube-texture-creation/ 做出修正。
        /// </summary>
        /// <param name="device"></param>
        /// <param name="texture2Ds"></param>
        /// <returns></returns>
        public static ShaderResourceView CubeMapFrom6Textures(Device device, Texture2D[] texture2Ds) {
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

        public static ShaderResourceView AsShaderResourceView(this Texture2D texture2D) {
            return new ShaderResourceView(texture2D.Device, texture2D);
        }

        private static BitmapSource LoadBitmap(ImagingFactory factory, string filename) {
            using (var bitmapDecoder = new BitmapDecoder(factory, filename, DecodeOptions.CacheOnDemand)) {
                var result = new FormatConverter(factory);
                using (var bitmapFrameDecode = bitmapDecoder.GetFrame(0)) {
                    result.Initialize(bitmapFrameDecode, PixelFormat.Format32bppPRGBA, BitmapDitherType.None, null, 0, BitmapPaletteType.Custom);
                }
                return result;
            }
        }

        private static Texture2D CreateTexture2DFromBitmap(Device device, BitmapSource bitmapSource) {
            // Allocate DataStream to receive the WIC image pixels
            var stride = bitmapSource.Size.Width * 4;
            using (var buffer = new DataStream(bitmapSource.Size.Height * stride, true, true)) {
                // Copy the content of the WIC to the buffer
                bitmapSource.CopyPixels(stride, buffer);
                var texture2DDescription = new Texture2DDescription() {
                    Width = bitmapSource.Size.Width,
                    Height = bitmapSource.Size.Height,
                    ArraySize = 1,
                    BindFlags = BindFlags.ShaderResource,
                    Usage = ResourceUsage.Immutable,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Format = Format.R8G8B8A8_UNorm,
                    MipLevels = 1,
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
        private static object _syncObject;

    }
}
