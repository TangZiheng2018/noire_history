﻿using System.Linq;
using Noire.Common;
using Noire.Common.Vertices;
using Noire.Graphics.D3D11.FX;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace Noire.Graphics.D3D11 {
    public sealed class Skybox : GameComponent {

        public Skybox(IGameComponentRoot root, IGameComponentContainer parent, float skySphereRadius)
            : base(root, parent) {
            _skySphereRadius = skySphereRadius;
        }

        public ShaderResourceView CubeMapSRV { get; set; }

        protected override void InitializeInternal() {
            base.InitializeInternal();
            var device = D3DApp11.I.D3DDevice;
            using (var r = CubeMapSRV.Resource) {
                r.DebugName = "sky cubemap";
            }

            var sphere = GeometryGenerator.CreateSphere(_skySphereRadius, 30, 30);
            var vertices = sphere.Vertices.Select(v => v.Position).ToArray();
            var vbd = new BufferDescription(
                VertPos.Stride * vertices.Length,
                ResourceUsage.Immutable,
                BindFlags.VertexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0
            );
            _vb = new Buffer(device, DataStream.Create(vertices, false, false), vbd);

            _indexCount = sphere.Indices.Count;
            var ibd = new BufferDescription(
                _indexCount * sizeof(int),
                ResourceUsage.Immutable,
                BindFlags.IndexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0
            );
            _ib = new Buffer(device, DataStream.Create(sphere.Indices.ToArray(), false, false), ibd);
        }

        protected override void DrawInternal(GameTime gameTime) {
            base.DrawInternal(gameTime);

            var camera = D3DApp11.I.Camera;
            var eyePos = camera.Position;
            var t = Matrix.Translation(eyePos);
            var wvp = t * camera.ViewProjectionMatrix;

            var skyFx = EffectManager11.Instance.GetEffect<SkyboxEffect11>();
            skyFx.SetWorldViewProj(wvp);
            skyFx.SetCubeMap(CubeMapSRV);

            var stride = VertPos.Stride;
            const int offset = 0;
            var context = D3DApp11.I.ImmediateContext;
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vb, stride, offset));
            context.InputAssembler.SetIndexBuffer(_ib, Format.R32_UInt, 0);
            context.InputAssembler.InputLayout = InputLayouts.Pos;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            var tech = skyFx.SkyTech;
            for (var p = 0; p < tech.Description.PassCount; p++) {
                using (var pass = tech.GetPassByIndex(p)) {
                    pass.Apply(context);
                    context.DrawIndexed(_indexCount, 0, 0);
                }
            }
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

        private Buffer _vb;
        private Buffer _ib;
        private int _indexCount;
        private string _filename;
        private float _skySphereRadius;

    }
}
