using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Misc;
using SharpDX.Direct3D9;

namespace Noire.Graphics.Nodes {
    public abstract class EffectNode : Node {

        public EffectNode(SceneNode scene)
            : base(scene, false) {
        }

        public override void Dispose() {
            DisposeEffect();
            base.Dispose();
        }

        public Effect Effect => _effect;

        public void LoadFromFile(string filename, ShaderFlags flags) {
            _filenameSource = filename;
            _sourceType = SourceType.File;
            _shaderFlags = flags;
            if (SourceDevice != null) {
                DisposeEffect();
                ReadEffect();
            }
        }

        public void LoadFromString(string source, ShaderFlags flags) {
            _stringSource = source;
            _sourceType = SourceType.String;
            _shaderFlags = flags;
            if (SourceDevice != null) {
                DisposeEffect();
                ReadEffect();
            }
        }

        protected override void OnDeviceChanged(object sender, DeviceChangedEventArgs e) {
            base.OnDeviceChanged(sender, e);
            if (e.NewDevice != null) {
                DisposeEffect();
                ReadEffect();
            }
        }

        protected override void OnDeviceLost(object sender, EventArgs e) {
            base.OnDeviceLost(sender, e);
            _effect?.OnLostDevice();
        }

        protected override void OnDeviceReset(object sender, EventArgs e) {
            base.OnDeviceReset(sender, e);
            _effect?.OnResetDevice();
        }

        private void ReadEffect() {
            switch (_sourceType) {
                case SourceType.File:
                    _effect = Effect.FromFile(SourceDevice, _filenameSource, _shaderFlags);
                    break;
                case SourceType.String:
                    _effect = Effect.FromString(SourceDevice, _stringSource, _shaderFlags);
                    break;
                default:
                    break;
            }
        }

        private void DisposeEffect() {
            NoireUtilities.SafeDispose(ref _effect);
        }

        private Effect _effect;
        private SourceType _sourceType;
        private ShaderFlags _shaderFlags;
        private string _filenameSource;
        private string _stringSource;

        private enum SourceType {
            File,
            Buffer,
            Stream,
            String
        }

    }
}
