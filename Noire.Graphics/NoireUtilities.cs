using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Mathematics.Interop;

namespace Noire.Graphics {

    public static class NoireUtilities {

        public static void SafeDispose<T>(ref T obj) where T : class, IDisposable {
            obj?.Dispose();
            obj = null;
        }

        public static Color ToColor(this RawColor4 color) {
            return new Color(color.R, color.G, color.B, color.A);
        }

        public static void InsertAfter(this INode node, INode referenceNode) {
            foreach (var child in referenceNode.Children) {
                child.Parent = node;
            }
            node.Parent = referenceNode;
        }

        public static void StripOut(this INode node) {
            if (node == null || node.IsRoot) {
                return;
            }
            foreach (var child in node.Children) {
                child.Parent = node.Parent;
            }
            node.Parent.RemoveChild(node);
        }

    }

}
