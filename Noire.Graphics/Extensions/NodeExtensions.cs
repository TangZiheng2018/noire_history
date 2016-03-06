using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Graphics;

namespace Noire.Extensions {
    public static class NodeExtensions {

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
