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
    public class D3DApp11 : D3DApp {

        public D3DApp11(Control control)
            : base(control) {
            IsInitialized = false;
        }

        protected override void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            Utilities.Dispose(ref pixelShader);
            Utilities.Dispose(ref pixelShaderByteCode);
            Utilities.Dispose(ref vertexShader);
            Utilities.Dispose(ref vertexShaderByteCode);
            Utilities.Dispose(ref vertices);
            Utilities.Dispose(ref constantBuffer);

            Utilities.Dispose(ref backBuffer);
            Utilities.Dispose(ref renderView);
            Utilities.Dispose(ref depthBuffer);
            Utilities.Dispose(ref depthView);

            Utilities.Dispose(ref _factory);
            Utilities.Dispose(ref _immediateContext);
            Utilities.Dispose(ref _swapChain);
            Utilities.Dispose(ref _d3dDevice);
            Utilities.Dispose(ref _dxgiDevice);
        }

        public override void Initialize() {
            if (IsInitialized) {
                return;
            }

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

            vertexShaderByteCode = ShaderBytecode.CompileFromFile("Effects/MiniCube.fx", "VS", "vs_4_0");
            vertexShader = new VertexShader(_d3dDevice, vertexShaderByteCode);
            pixelShaderByteCode = ShaderBytecode.CompileFromFile("Effects/MiniCube.fx", "PS", "ps_4_0");
            pixelShader = new PixelShader(_d3dDevice, pixelShaderByteCode);
            var signature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
            // Layout from VertexShader input signature
            var layout = new InputLayout(_d3dDevice, signature, new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                    });
            // Instantiate Vertex buiffer from vertex data
            vertices = SharpDX.Direct3D11.Buffer.Create(_d3dDevice, BindFlags.VertexBuffer, new[]
                                  {
                                      new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Front
                                      new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),

                                      new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f), // BACK
                                      new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),

                                      new Vector4(-1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f), // Top
                                      new Vector4(-1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                                      new Vector4( 1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                                      new Vector4(-1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                                      new Vector4( 1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                                      new Vector4( 1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),

                                      new Vector4(-1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f), // Bottom
                                      new Vector4( 1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4(-1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4(-1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4( 1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4( 1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),

                                      new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f), // Left
                                      new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                                      new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                                      new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                                      new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                                      new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),

                                      new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f), // Right
                                      new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                                      new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                                      new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                                      new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                                      new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                            });
            // Create Constant Buffer
            constantBuffer = new SharpDX.Direct3D11.Buffer(_d3dDevice, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
            var context = ImmediateContext;
            // Prepare All the stages
            context.InputAssembler.InputLayout = layout;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertices, Utilities.SizeOf<Vector4>() * 2, 0));
            context.VertexShader.SetConstantBuffer(0, constantBuffer);
            context.VertexShader.Set(vertexShader);
            context.PixelShader.Set(pixelShader);

            _camera = new FpsCamera(45, (float)ControlWindow.ClientSize.Width / ControlWindow.ClientSize.Height, 1, 1000);
            //_camera = new OrthoCamera(ControlWindow.ClientSize.Width, ControlWindow.ClientSize.Height, 0.1f, 1000);
            _camera.Position = new Vector3(0, -5, 0);
            _camera.LookAt(Vector3.Zero);

            InvalidateSurface(this);
        }

        public override void Terminate() {
            EffectManager11.Instance?.Dispose();
            IsRunning = false;
        }

        public DeviceContext ImmediateContext => _immediateContext;

        public SwapChain SwapChain => _swapChain;

        public Factory Factory => _factory;

        protected override void OnInvalidateSurface(object sender, EventArgs e) {
            // Dispose all previous allocated resources
            Utilities.Dispose(ref backBuffer);
            Utilities.Dispose(ref renderView);
            Utilities.Dispose(ref depthBuffer);
            Utilities.Dispose(ref depthView);

            var clientSize = ControlWindow.ClientSize;
            _camera.Aspect = (float)clientSize.Width / clientSize.Height;

            // Resize the backbuffer
            _swapChain.ResizeBuffers(_swapChainDescription.BufferCount, clientSize.Width, clientSize.Height, Format.Unknown, SwapChainFlags.None);
            // Get the backbuffer from the swapchain
            backBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);

            // Renderview on the backbuffer
            renderView = new RenderTargetView(_d3dDevice, backBuffer);

            // Create the depth buffer
            depthBuffer = new Texture2D(_d3dDevice, new Texture2DDescription() {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = ControlWindow.ClientSize.Width,
                Height = ControlWindow.ClientSize.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });

            // Create the depth buffer view
            depthView = new DepthStencilView(_d3dDevice, depthBuffer);

            // Setup targets and viewport for rendering
            ImmediateContext.Rasterizer.SetViewport(new Viewport(0, 0, ControlWindow.ClientSize.Width, ControlWindow.ClientSize.Height, 0.0f, 1.0f));
            ImmediateContext.OutputMerger.SetTargets(depthView, renderView);
        }

        protected override void Render(GameTime gameTime) {
            var viewProj = _camera.ViewProjectionMatrix;

            // Clear views
            ImmediateContext.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
            ImmediateContext.ClearRenderTargetView(renderView, Color.Black);

            // Update WorldViewProj Matrix
            var r = MathUtil.DegreesToRadians(_degree);
            var worldViewProj = Matrix.RotationX(r) * Matrix.RotationY(r * 2) * Matrix.RotationZ(r * .7f) * viewProj;
            //var worldViewProj = Matrix.RotationZ(r) * viewProj;
            worldViewProj.Transpose();
            ImmediateContext.UpdateSubresource(ref worldViewProj, constantBuffer);

            // Draw the cube
            ImmediateContext.Draw(36, 0);

            // Present!
            _swapChain.Present(0, PresentFlags.None);
        }

        protected override void Update(GameTime gameTime) {
            _degree += 1f;
            _camera.UpdateViewMatrix();
        }

        private float _degree = 0;

        private SharpDX.Direct3D11.Device _d3dDevice;
        private SharpDX.DXGI.Device _dxgiDevice;
        private SwapChainDescription _swapChainDescription;
        private SwapChain _swapChain;
        private DeviceContext _immediateContext;
        private Factory _factory;

        private CompilationResult vertexShaderByteCode;
        private VertexShader vertexShader;
        private CompilationResult pixelShaderByteCode;
        private PixelShader pixelShader;
        private SharpDX.Direct3D11.Buffer vertices;
        private SharpDX.Direct3D11.Buffer constantBuffer;

        // Declare texture for rendering
        private bool userResized = true;
        private Texture2D backBuffer = null;
        private RenderTargetView renderView = null;
        private Texture2D depthBuffer = null;
        private DepthStencilView depthView = null;
        private Matrix proj = Matrix.Identity;

        private CameraBase _camera;

    }
}
