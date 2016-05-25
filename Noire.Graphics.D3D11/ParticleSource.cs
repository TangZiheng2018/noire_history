using Noire.Common;
using Noire.Graphics.D3D11.FX;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11 {
    public class ParticleSource : GameComponent {

        public ParticleSource(IGameComponentRoot root, IGameComponentContainer parent, Device device, ParticleEffectBase11 effect, ShaderResourceView texArraySRV, ShaderResourceView randomTexSRV, int maxParticles)
            : base(root, parent) {
            _firstRun = true;
            EmitDirW = new Vector3(0, 1, 0);
            _maxParticles = maxParticles;
            _fx = effect;
            _texArraySRV = texArraySRV;
            _randomTexSRV = randomTexSRV;
            _device = device;
        }

        // How long the system has existed
        public float Age { get; private set; }

        // The camera eye position.  Passed to the shader to align the billboarded lines/quads
        public Vector3 EyePosW { get; set; }
        // Used to set the position in world-space of the particle emitter
        public Vector3 EmitPosW { get; set; }
        // Used to set the initial direction of emitted particles, if the direction varies
        public Vector3 EmitDirW { get; set; }

        public void Reset() {
            _firstRun = true;
            Age = 0;
        }

        protected override void InitializeInternal() {
            BuildVertexBuffer(_device);
        }

        protected override void UpdateInternal(GameTime gameTime) {
            base.UpdateInternal(gameTime);

            _timeStep = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _gameTime = (float)gameTime.TotalGameTime.TotalSeconds;
            Age += _timeStep;
        }

        protected override void DrawInternal(GameTime gameTime) {
            base.DrawInternal(gameTime);

            var camera = D3DApp11.I.Camera;
            var context = D3DApp11.I.ImmediateContext;

            var vp = camera.ViewProjectionMatrix;

            // set shader variables
            _fx.SetViewProj(vp);
            _fx.SetGameTime(_gameTime);
            _fx.SetTimeStep(_timeStep);
            _fx.SetEyePosW(EyePosW);
            _fx.SetEmitPosW(EmitPosW);
            _fx.SetEmitDirW(EmitDirW);
            _fx.SetTexArray(_texArraySRV);
            _fx.SetRandomTex(_randomTexSRV);

            context.InputAssembler.InputLayout = InputLayouts.Particle;
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;

            var stride = Particle.Stride;
            const int offset = 0;

            // bind the input vertex buffer for the stream-out technique
            // use the _initVB when _firstRun = true
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_firstRun ? _initVB : _drawVB, stride, offset));
            // bind the stream-out vertex buffer
            context.StreamOutput.SetTargets(new[] { new StreamOutputBufferBinding(_streamOutVB, offset) });

            // draw the particles using the stream-out technique, which will update the particles positions
            // and output the resulting particles to the stream-out buffer
            var techDesc = _fx.StreamOutTech.Description;
            for (var p = 0; p < techDesc.PassCount; p++) {
                _fx.StreamOutTech.GetPassByIndex(p).Apply(context);
                if (_firstRun) {
                    context.Draw(1, 0);
                    _firstRun = false;
                } else {
                    // the _drawVB buffer was populated by the Stream-out technique, so we don't
                    // know how many vertices are contained within it.  Direct3D keeps track of this
                    // internally, however, and we can use DrawAuto to draw everything in the buffer.
                    context.DrawAuto();
                }
            }
            // Disable stream-out
            context.StreamOutput.SetTargets(null);

            // ping-pong the stream-out and draw buffers, since we will now want to draw the vertices
            // populated into the buffer that was bound to stream-out
            var temp = _drawVB;
            _drawVB = _streamOutVB;
            _streamOutVB = temp;

            // draw the particles using the draw technique that will transform the points to lines/quads
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_drawVB, stride, offset));
            techDesc = _fx.DrawTech.Description;
            for (var p = 0; p < techDesc.PassCount; p++) {
                _fx.DrawTech.GetPassByIndex(p).Apply(context);
                context.DrawAuto();
            }
        }

        protected override void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            if (disposing) {
                Utilities.Dispose(ref _initVB);
                Utilities.Dispose(ref _drawVB);
                Utilities.Dispose(ref _streamOutVB);
            }
            base.Dispose(disposing);
        }

        private void BuildVertexBuffer(Device device) {
            var vbd = new BufferDescription(
                Particle.Stride,
                ResourceUsage.Default,
                BindFlags.VertexBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0);

            var p = new Particle {
                Age = 0,
                Type = 0
            };

            _initVB = new Buffer(device, DataStream.Create(new[] { p }, true, true), vbd);

            vbd.SizeInBytes = Particle.Stride * _maxParticles;
            vbd.BindFlags = BindFlags.VertexBuffer | BindFlags.StreamOutput;

            _drawVB = new Buffer(device, vbd);
            _streamOutVB = new Buffer(device, vbd);
        }

        // The particles effect shader for this system
        private readonly ParticleEffectBase11 _fx;

        // Maximum number of particles that can be created
        private readonly int _maxParticles;
        // on the first run, we need to use a different vertex buffer to initialize the system
        private bool _firstRun;

        // used as a seed to index into the random-value texture
        private float _gameTime;
        // The time since the last update of the system
        private float _timeStep;

        // A vertex buffer containing the original emitter particles
        private Buffer _initVB;
        // vertex buffer to hold the particles to be drawn
        private Buffer _drawVB;
        // vertex buffer to receive the particles generated by the stream-out shader
        private Buffer _streamOutVB;

        private readonly Device _device;

        // a texture array to contain the sprites to be applied to the drawn particles
        private ShaderResourceView _texArraySRV;
        // a texture containing random floats, used to supply the shader with random values 
        private ShaderResourceView _randomTexSRV;

    }
}
