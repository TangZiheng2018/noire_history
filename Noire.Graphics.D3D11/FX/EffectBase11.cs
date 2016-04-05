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
    public abstract class EffectBase11 : DisposeBase {

        public Effect DxEffect => _dxEffect;

        public int ID { get; private set; }

        internal bool RegisterEffect() {
            if (!EffectManager11.IsInitialized) {
                throw new InvalidOperationException();
            }
            var type = GetType();
            var id = EffectManager11.Instance.BeginRegisterEffect(type);
            if (id < 0) {
                Debug.Print($"WARNING: An effect of type '{type.Name}' already registered.");
                return false;
            }
            ID = id;
            EffectManager11.Instance.EndRegisterEffect(this);
            Initialize();
            return true;
        }

        protected EffectBase11(Device device, string filename) {
            if (!File.Exists(filename)) {
                throw new FileNotFoundException($"Effect file '{filename}' is not found.", filename);
            }
            var fileInfo = new FileInfo(filename);
            using (var includeProcessor = new IncludeProcessor(fileInfo.DirectoryName)) {
                // SharpDX 当前（2016-04-04）使用的D3DCompiler版本为47，fx目标只支持 fx_5_0。
                // 详见 https://msdn.microsoft.com/en-us/library/windows/desktop/hh446869.aspx 和 https://msdn.microsoft.com/en-us/library/windows/desktop/jj215820.aspx。
                // 例如，使用 fx_4_0 的配置进行编译和设置，语法上没问题，但是无法创建 Effect。也就是说，D3DCompile2 是支持 fx_4_0 编译的，但是编译状态中有过时选项警告，
                // 导致能生成 bytecode（Bytecode 属性非空），但是 Effect 创建时抛出异常。
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
