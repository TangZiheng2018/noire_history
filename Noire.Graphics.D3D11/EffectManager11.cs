using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Graphics.D3D11.FX;
using SharpDX;

namespace Noire.Graphics.D3D11 {
    public sealed partial class EffectManager11 : DisposeBase {

        static EffectManager11() {
            LockObject = new object();
            Rand = new Random();
        }

        public static void Initialize() {
            lock (LockObject) {
                if (_isInitialized) {
                    return;
                }
                _instance = new EffectManager11();
                _isInitialized = true;
            }
        }

        public static EffectManager11 Instance => _instance;

        public static bool IsInitialized {
            get {
                lock (LockObject) {
                    return _isInitialized;
                }
            }
        }

        public int BeginRegisterEffect(Type type) {
            if (type == null || _effectsByType.ContainsKey(type)) {
                return -1;
            }
            var value = Rand.Next();
            while (_effectsByID.ContainsKey(value)) {
                value = Rand.Next();
            }
            return value;
        }

        public void EndRegisterEffect(EffectBase11 effect) {
            if (effect == null) {
                return;
            }
            _effectsByType[effect.GetType()] = effect;
            _effectsByID[effect.ID] = effect;
        }

        public bool EffectExists(Type type) {
            return _effectsByType.ContainsKey(type);
        }

        public bool EffectExists<T>() where T : EffectBase11 {
            return EffectExists(typeof(T));
        }

        public EffectBase11 GetEffect(Type type) {
            return _effectsByType[type];
        }

        public T GetEffect<T>() where T : EffectBase11 {
            return _effectsByType[typeof(T)] as T;
        }

        public EffectBase11 GetEffect(int id) {
            return _effectsByID[id];
        }

        public T GetEffect<T>(int id) where T : EffectBase11 {
            return _effectsByID[id] as T;
        }

        protected override void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            foreach (var effect in _effectsByType) {
                effect.Value.Dispose();
            }
            _effectsByID.Clear();
            _effectsByType.Clear();
        }

        private EffectManager11() {
            _effectsByID = new Dictionary<int, EffectBase11>();
            _effectsByType = new Dictionary<Type, EffectBase11>();
        }

        private readonly Dictionary<int, EffectBase11> _effectsByID;
        private readonly Dictionary<Type, EffectBase11> _effectsByType;

        private static EffectManager11 _instance;
        private static bool _isInitialized;
        private static readonly object LockObject;
        private static readonly Random Rand;

    }
}
