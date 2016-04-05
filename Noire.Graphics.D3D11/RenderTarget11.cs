using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Common;
using Noire.Common.Camera;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace Noire.Graphics.D3D11 {
    public class RenderTarget11 : GameComponent {

        public RenderTarget11(CameraBase camera) {
            _camera = camera;
        }

        public CameraBase Camera => _camera;

        protected override void DrawInternal(GameTime gameTime) {
            var immediateContext = D3DApp11.I.ImmediateContext;

            // Clear views
            immediateContext.ClearDepthStencilView(_depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            immediateContext.ClearRenderTargetView(_renderView, Color.Black);

            base.DrawInternal(gameTime);

            // Present!
            D3DApp11.I.SwapChain.Present(0, PresentFlags.None);
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

            _camera.Aspect = (float)clientSize.Width / clientSize.Height;

            // Resize the backbuffer
            swapChain.ResizeBuffers(_swapChainDescription.BufferCount, clientSize.Width, clientSize.Height, Format.Unknown, SwapChainFlags.None);
            // Get the backbuffer from the swapchain
            _backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);

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
            immediateContext.Rasterizer.SetViewport(new Viewport(0, 0, clientSize.Width, clientSize.Height, 0.0f, 1.0f));
            immediateContext.OutputMerger.SetTargets(_depthView, _renderView);

            _camera.Aspect = (float)clientSize.Width / clientSize.Height;

            base.OnSurfaceInvalidated(sender, e);
        }

        protected override void UpdateInternal(GameTime gameTime) {
            base.UpdateInternal(gameTime);
            _camera.UpdateViewMatrix();
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

        private CameraBase _camera;
        private SwapChainDescription _swapChainDescription;
        private Texture2D _backBuffer;
        private RenderTargetView _renderView;
        private Texture2D _depthBuffer;
        private DepthStencilView _depthView;

    }
}
