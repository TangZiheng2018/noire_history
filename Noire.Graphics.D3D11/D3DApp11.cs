using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Noire.Common;
using Noire.Common.Camera;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace Noire.Graphics.D3D11 {
    public sealed class D3DApp11 : D3DApp {

        static D3DApp11() {
            _syncObject = new object();
        }

        public static D3DApp11 Create(Control control) {
            lock (_syncObject) {
                if (_app != null) {
                    throw new Exception("App is already created.");
                }
                _app = new D3DApp11(control);
                return _app;
            }
        }

        public static D3DApp11 I => _app;

        public override void Terminate() {
            IsRunning = false;
        }

        public DeviceContext ImmediateContext => _immediateContext;

        public SwapChain SwapChain => _swapChain;

        public Factory Factory => _factory;

        public SharpDX.Direct3D11.Device D3DDevice => _d3dDevice;

        public RenderTarget11 RenderTarget => _renderTarget;

        public Skybox Skybox => _skybox;

        protected override void Render(GameTime gameTime) {
            Draw(gameTime);
            // Present!
            _swapChain.Present(0, PresentFlags.None);
        }

        private D3DApp11(Control control)
            : base(control) {
            IsInitialized = false;
        }

        protected override void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }

            if (disposing) {
                TextureLoader.Dispose();
                EffectManager11.Instance?.Dispose();
                InputLayouts.DisposeAll();
                Utilities.Dispose(ref _factory);
                Utilities.Dispose(ref _immediateContext);
                Utilities.Dispose(ref _swapChain);
                Utilities.Dispose(ref _d3dDevice);
                Utilities.Dispose(ref _dxgiDevice);
            }
            base.Dispose(disposing);
        }

        protected override void InitializeInternal() {
            var clientSize = ControlWindow.ClientSize;
            _swapChainDescription = new SwapChainDescription() {
                BufferCount = 1,
                ModeDescription = new ModeDescription(clientSize.Width, clientSize.Height, new Rational(60, 1), Format.B8G8R8A8_UNorm),
                IsWindowed = true,
                OutputHandle = ControlWindow.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };
            SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, _swapChainDescription, out _d3dDevice, out _swapChain);
            _immediateContext = _d3dDevice.ImmediateContext;
            _factory = _swapChain.GetParent<Factory>();

            IsInitialized = true;

            NoireConfiguration.ResourceBase = "resources";
            EffectManager11.Initialize();
            EffectManager11.Instance?.InitializeAllEffects(_d3dDevice);
            TextureLoader.Initialize();
            InputLayouts.InitializeAll(_d3dDevice);

            var camera = new FpsCamera(MathUtil.DegreesToRadians(45), (float)clientSize.Width / clientSize.Height, 1, 1000);
            _renderTarget = new RenderTarget11(camera);
            _renderTarget.Initialize();
            ChildComponents.Add(_renderTarget);
            _skybox = new Skybox(NoireConfiguration.GetFullResourcePath("textures/cube.dds"), 5000);
            _skybox.Initialize();
            ChildComponents.Add(_skybox);

            ResetSurface(this);
        }

        private SharpDX.Direct3D11.Device _d3dDevice;
        private SharpDX.DXGI.Device _dxgiDevice;
        private SwapChainDescription _swapChainDescription;
        private SwapChain _swapChain;
        private DeviceContext _immediateContext;
        private Factory _factory;
        private RenderTarget11 _renderTarget;
        private Skybox _skybox;

        private static D3DApp11 _app;
        private static readonly object _syncObject;

    }
}
