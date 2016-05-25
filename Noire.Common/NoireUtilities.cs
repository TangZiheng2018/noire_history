using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Noire.Common {
    public static class NoireUtilities {

        public static void RaiseEvent<T>(EventHandler<T> handler, object sender, T e) where T : EventArgs {
            handler?.Invoke(sender, e);
        }

        public static void RaiseEvent(EventHandler handler, object sender, EventArgs e) {
            handler?.Invoke(sender, e);
        }

        public static void DisposeNonPublicDeclaredFields<T>(T disposeBase) where T : IDisposable {
            DisposeNonPublicFields(disposeBase, BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static void DisposeNonPublicFields<T>(T disposeBase) where T : IDisposable {
            DisposeNonPublicFields(disposeBase, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static byte[] StructureToBytes<T>(T obj) where T : struct {
            var len = Marshal.SizeOf(obj);
            var buf = new byte[len];
            var ptr = Marshal.AllocHGlobal(len);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, buf, 0, len);
            Marshal.FreeHGlobal(ptr);
            return buf;
        }

        private static void DisposeNonPublicFields<T>(T disposeBase, BindingFlags flags) where T : IDisposable {
            var type = typeof(T);
            var disposeBaseType = typeof(IDisposable);
            var fields = type.GetFields(flags);
            foreach (var field in fields) {
                if (field.FieldType.IsSubclassOf(disposeBaseType)) {
                    var value = field.GetValue(disposeBase) as IDisposable;
                    value?.Dispose();
                    field.SetValue(disposeBase, null);
                }
            }
        }

        public static T[] Repeat<T>(this T obj, int times) {
            var t = new T[times];
            for (var i = 0; i < times; ++i) {
                t[i] = obj;
            }
            return t;
        }

    }
}
