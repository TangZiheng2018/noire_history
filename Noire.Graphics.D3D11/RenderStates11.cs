using System.Diagnostics;
using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11 {
    public sealed class RenderStates11 : DisposeBase {

        public RenderStates11(Device device) {
            _device = device;
        }

        public Device Device => _device;

        public RasterizerState WireframeRS => _wireframeRs;

        public RasterizerState NoCullRS => _noCullRs;

        public RasterizerState CullClockwiseRS => _cullClockwiseRs;

        public BlendState AlphaToCoverageBS => _alphaToCoverageBs;

        public BlendState TransparentBS => _transparentBs;

        public BlendState NoRenderTargetWritesBS => _noRenderTargetWritesBs;

        public DepthStencilState MarkMirrorDSS => _markMirrorDss;

        public DepthStencilState DrawReflectionDSS => _drawReflectionDss;

        public DepthStencilState NoDoubleBlendDSS => _noDoubleBlendDss;

        public DepthStencilState LessEqualDSS => _lessEqualDss;

        public DepthStencilState EqualsDSS => _equalsDss;

        public DepthStencilState NoDepthDSS => _noDepthDss;

        public void InitializeAll() {
            var device = _device;

            Debug.Assert(device != null);

            var wfDesc = new RasterizerStateDescription {
                FillMode = FillMode.Wireframe,
                CullMode = CullMode.Back,
                IsFrontCounterClockwise = false,
                IsDepthClipEnabled = true
            };
            _wireframeRs = new RasterizerState(device, wfDesc);

            var noCullDesc = new RasterizerStateDescription {
                FillMode = FillMode.Solid,
                CullMode = CullMode.None,
                IsFrontCounterClockwise = false,
                IsDepthClipEnabled = true
            };
            _noCullRs = new RasterizerState(device, noCullDesc);

            var cullClockwiseDesc = new RasterizerStateDescription {
                FillMode = FillMode.Solid,
                CullMode = CullMode.Back,
                IsFrontCounterClockwise = true,
                IsDepthClipEnabled = true
            };
            _cullClockwiseRs = new RasterizerState(device, cullClockwiseDesc);

            var atcDesc = new BlendStateDescription {
                AlphaToCoverageEnable = true,
                IndependentBlendEnable = false,
            };
            atcDesc.RenderTarget[0].IsBlendEnabled = false;
            atcDesc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
            _alphaToCoverageBs = new BlendState(device, atcDesc);

            var transDesc = new BlendStateDescription {
                AlphaToCoverageEnable = false,
                IndependentBlendEnable = false
            };
            transDesc.RenderTarget[0].IsBlendEnabled = true;
            transDesc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
            transDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
            transDesc.RenderTarget[0].BlendOperation = BlendOperation.Add;
            transDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
            transDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
            transDesc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
            transDesc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;

            _transparentBs = new BlendState(device, transDesc);

            var noRenderTargetWritesDesc = new BlendStateDescription {
                AlphaToCoverageEnable = false,
                IndependentBlendEnable = false
            };
            noRenderTargetWritesDesc.RenderTarget[0].IsBlendEnabled = false;
            noRenderTargetWritesDesc.RenderTarget[0].SourceBlend = BlendOption.One;
            noRenderTargetWritesDesc.RenderTarget[0].DestinationBlend = BlendOption.Zero;
            noRenderTargetWritesDesc.RenderTarget[0].BlendOperation = BlendOperation.Add;
            noRenderTargetWritesDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
            noRenderTargetWritesDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
            noRenderTargetWritesDesc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
            noRenderTargetWritesDesc.RenderTarget[0].RenderTargetWriteMask = 0;

            _noRenderTargetWritesBs = new BlendState(device, noRenderTargetWritesDesc);

            var mirrorDesc = new DepthStencilStateDescription {
                IsDepthEnabled = true,
                DepthWriteMask = DepthWriteMask.Zero,
                DepthComparison = Comparison.Less,
                IsStencilEnabled = true,
                StencilReadMask = 0xff,
                StencilWriteMask = 0xff,
                FrontFace = new DepthStencilOperationDescription {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Keep,
                    PassOperation = StencilOperation.Replace,
                    Comparison = Comparison.Always
                },
                BackFace = new DepthStencilOperationDescription {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Keep,
                    PassOperation = StencilOperation.Replace,
                    Comparison = Comparison.Always
                }
            };

            _markMirrorDss = new DepthStencilState(device, mirrorDesc);

            var drawReflectionDesc = new DepthStencilStateDescription {
                IsDepthEnabled = true,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Less,
                IsStencilEnabled = true,
                StencilReadMask = 0xff,
                StencilWriteMask = 0xff,
                FrontFace = new DepthStencilOperationDescription {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Keep,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Equal
                },
                BackFace = new DepthStencilOperationDescription {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Keep,
                    PassOperation = StencilOperation.Keep,
                    Comparison = Comparison.Equal
                }
            };
            _drawReflectionDss = new DepthStencilState(device, drawReflectionDesc);

            var noDoubleBlendDesc = new DepthStencilStateDescription {
                IsDepthEnabled = true,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.Less,
                IsStencilEnabled = true,
                StencilReadMask = 0xff,
                StencilWriteMask = 0xff,
                FrontFace = new DepthStencilOperationDescription {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Keep,
                    PassOperation = StencilOperation.Increment,
                    Comparison = Comparison.Equal
                },
                BackFace = new DepthStencilOperationDescription {
                    FailOperation = StencilOperation.Keep,
                    DepthFailOperation = StencilOperation.Keep,
                    PassOperation = StencilOperation.Increment,
                    Comparison = Comparison.Equal
                }
            };
            _noDoubleBlendDss = new DepthStencilState(device, noDoubleBlendDesc);

            var lessEqualDesc = new DepthStencilStateDescription {
                IsDepthEnabled = true,
                DepthWriteMask = DepthWriteMask.All,
                DepthComparison = Comparison.LessEqual,
                IsStencilEnabled = false
            };
            _lessEqualDss = new DepthStencilState(device, lessEqualDesc);

            var equalsDesc = new DepthStencilStateDescription() {
                IsDepthEnabled = true,
                DepthWriteMask = DepthWriteMask.Zero,
                DepthComparison = Comparison.LessEqual,

            };
            _equalsDss = new DepthStencilState(device, equalsDesc);

            var noDepthDesc = new DepthStencilStateDescription() {
                IsDepthEnabled = false,
                DepthComparison = Comparison.Always,
                DepthWriteMask = DepthWriteMask.Zero
            };
            _noDepthDss = new DepthStencilState(device, noDepthDesc);
        }

        protected override void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            if (disposing) {
                Utilities.Dispose(ref _wireframeRs);
                Utilities.Dispose(ref _noCullRs);
                Utilities.Dispose(ref _alphaToCoverageBs);
                Utilities.Dispose(ref _cullClockwiseRs);
                Utilities.Dispose(ref _transparentBs);
                Utilities.Dispose(ref _noRenderTargetWritesBs);
                Utilities.Dispose(ref _markMirrorDss);
                Utilities.Dispose(ref _drawReflectionDss);
                Utilities.Dispose(ref _noDoubleBlendDss);
                Utilities.Dispose(ref _lessEqualDss);
                Utilities.Dispose(ref _equalsDss);
                Utilities.Dispose(ref _noDepthDss);
                _device = null;
            }
        }

        private Device _device;
        private RasterizerState _wireframeRs;
        private RasterizerState _noCullRs;
        private BlendState _alphaToCoverageBs;
        private RasterizerState _cullClockwiseRs;
        private BlendState _transparentBs;
        private BlendState _noRenderTargetWritesBs;
        private DepthStencilState _markMirrorDss;
        private DepthStencilState _drawReflectionDss;
        private DepthStencilState _noDoubleBlendDss;
        private DepthStencilState _lessEqualDss;
        private DepthStencilState _equalsDss;
        private DepthStencilState _noDepthDss;

    }
}
