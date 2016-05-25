using System;
using Noire.Common;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Resource = SharpDX.Direct3D11.Resource;

namespace Noire.Graphics.D3D11 {
    public class RenderTarget11 : GameComponent {

        public RenderTarget11(IGameComponentRoot root, IGameComponentContainer parent)
            : base(root, parent) {
        }

        public DepthStencilView DepthStencilView => _depthView;

        public RenderTargetView RenderTargetView => _renderView;

        public Viewport Viewport => _viewport;

        protected override void DrawInternal(GameTime gameTime) {
            var immediateContext = D3DApp11.I.ImmediateContext;

            // Clear views
            immediateContext.ClearDepthStencilView(_depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            immediateContext.ClearRenderTargetView(_renderView, Color.Black);

            base.DrawInternal(gameTime);
        }

        protected override void InitializeInternal() {
            var clientSize = D3DApp11.I.ControlWindow.ClientSize;
            _swapChainDescription = new SwapChainDescription() {
                BufferCount = 1,
                ModeDescription = new ModeDescription(clientSize.Width, clientSize.Height, new Rational(60, 1), Format.B8G8R8A8_UNorm),
                IsWindowed = true,
                OutputHandle = D3DApp11.I.ControlWindow.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };
        }

        protected override void OnSurfaceInvalidated(object sender, EventArgs e) {
            var swapChain = D3DApp11.I.SwapChain;
            var clientSize = D3DApp11.I.ControlWindow.ClientSize;
            var device = D3DApp11.I.D3DDevice;
            var immediateContext = D3DApp11.I.ImmediateContext;

            // Dispose all previous allocated resources
            Utilities.Dispose(ref _backBuffer);
            Utilities.Dispose(ref _renderView);
            Utilities.Dispose(ref _depthBuffer);
            Utilities.Dispose(ref _depthView);

            // Resize the backbuffer
            swapChain.ResizeBuffers(_swapChainDescription.BufferCount, clientSize.Width, clientSize.Height, Format.Unknown, SwapChainFlags.None);
            // Get the backbuffer from the swapchain
            _backBuffer = Resource.FromSwapChain<Texture2D>(swapChain, 0);

            // Renderview on the backbuffer
            _renderView = new RenderTargetView(device, _backBuffer);

            // Create the depth buffer
            _depthBuffer = new Texture2D(device, new Texture2DDescription() {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = clientSize.Width,
                Height = clientSize.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            // Create the depth buffer view
            _depthView = new DepthStencilView(device, _depthBuffer);

            // Setup targets and viewport for rendering
            _viewport = new Viewport(0, 0, clientSize.Width, clientSize.Height, 0.0f, 1.0f);
            immediateContext.Rasterizer.SetViewport(_viewport);
            immediateContext.OutputMerger.SetTargets(_depthView, _renderView);

            base.OnSurfaceInvalidated(sender, e);
        }

        protected override void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            if (disposing) {
                NoireUtilities.DisposeNonPublicDeclaredFields(this);
            }
            base.Dispose(disposing);
        }

        private Viewport _viewport;
        private SwapChainDescription _swapChainDescription;
        private Texture2D _backBuffer;
        private RenderTargetView _renderView;
        private Texture2D _depthBuffer;
        private DepthStencilView _depthView;

    }
}
