using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Noire.Common;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11.FX {
    public abstract class EffectBase : DisposeBase {

        public Effect DxEffect => _dxEffect;

        public int ID { get; private set; }

        internal bool RegisterEffect() {
            if (!EffectManager.IsInitialized) {
                throw new InvalidOperationException();
            }
            var type = GetType();
            var id = EffectManager.Instance.BeginRegisterEffect(type);
            if (id < 0) {
                Debug.Print($"WARNING: An effect of type '{type.Name}' already registered.");
                return false;
            }
            ID = id;
            EffectManager.Instance.EndRegisterEffect(this);
            Initialize();
            return true;
        }

        protected EffectBase(Device device, string filename) {
            if (!File.Exists(filename)) {
                throw new FileNotFoundException($"Effect file '{filename}' is not found.", filename);
            }
            var fileInfo = new FileInfo(filename);
            using (var includeProcessor = new NoireIncludeProcessor(fileInfo.DirectoryName)) {
                using (var compilationResult = ShaderBytecode.CompileFromFile(filename, null, "fx_5_0", ShaderFlags.None, EffectFlags.None, null, includeProcessor)) {
                    _dxEffect = new Effect(device, compilationResult.Bytecode, EffectFlags.None, filename);
                }
            }
        }

        protected abstract void Initialize();

        protected sealed override void Dispose(bool disposing) {
            if (!IsDisposed) {
                if (disposing) {
                    NoireUtilities.DisposeNonPublicFields(this);
                }
            }
        }

        private Effect _dxEffect;

    }
}
