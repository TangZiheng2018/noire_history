using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using Device = SharpDX.Direct3D11.Device;

namespace Noire.Graphics.D3D11 {
    public sealed class ShadowMap : DisposeBase {

        public ShadowMap(Device device, int width, int height) {
            _width = width;
            _height = height;

            _viewport = new Viewport(0, 0, _width, _height, 0, 1.0f);

            var texDesc = new Texture2DDescription {
                Width = _width,
                Height = _height,
                MipLevels = 1,
                ArraySize = 1,
                Format = Format.R24G8_Typeless,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };

            var depthMap = new Texture2D(device, texDesc) {
                DebugName = "shadowmap depthmap"
            };
            var dsvDesc = new DepthStencilViewDescription {
                Flags = DepthStencilViewFlags.None,
                Format = Format.D24_UNorm_S8_UInt,
                Dimension = DepthStencilViewDimension.Texture2D,
                Texture2D = new DepthStencilViewDescription.Texture2DResource() {
                    MipSlice = 0
                }
            };
            _depthMapDSV = new DepthStencilView(device, depthMap, dsvDesc);

            var srvDesc = new ShaderResourceViewDescription {
                Format = Format.R24_UNorm_X8_Typeless,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource() {
                    MipLevels = texDesc.MipLevels,
                    MostDetailedMip = 0
                }
            };

            DepthMapSRV = new ShaderResourceView(device, depthMap, srvDesc);

            Utilities.Dispose(ref depthMap);

        }

        public ShaderResourceView DepthMapSRV {
            get { return _depthMapSRV; }
            private set { _depthMapSRV = value; }
        }

        public void BindDsvAndSetNullRenderTarget(DeviceContext context) {
            context.Rasterizer.SetViewports(new RawViewportF[] { _viewport });
            context.OutputMerger.SetTargets(_depthMapDSV, (RenderTargetView)null);
            context.ClearDepthStencilView(_depthMapDSV, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
        }

        protected override void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            if (disposing) {
                Utilities.Dispose(ref _depthMapSRV);
                Utilities.Dispose(ref _depthMapDSV);
            }
        }

        private readonly int _width;
        private readonly int _height;
        private DepthStencilView _depthMapDSV;
        private readonly Viewport _viewport;
        private ShaderResourceView _depthMapSRV;


    }
}
