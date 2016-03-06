using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Misc;

namespace Noire.Graphics {
    public abstract class Node : INode {

        public Node(Direct3DRuntime d3dRuntime, bool isRoot) {
            _isRoot = isRoot;
            _children = new SafeList<INode>();
            _childrenReflection = _children.AsReadOnly();
            _d3dRuntime = d3dRuntime;
        }

        public bool Enabled { get; set; } = true;

        public bool IsRoot => _isRoot;

        public INode Parent {
            get {
                return _parent;
            }
            set {
                if (value != null && _parent != value) {
                    if (_parent != null) {
                        _parent.RemoveChild(this);
                    }
                    _parent = value;
                    _parent.AddChild(this);
                }
            }
        }

        public virtual void Dispose() {
            foreach (var child in Children) {
                child.Dispose();
            }
        }

        public bool Visible { get; set; } = true;

        public void AddChild(INode node) {
            _children.Add(node);
        }

        public void RemoveChild(INode node) {
            _children.Remove(node);
        }

        public ReadOnlyCollection<INode> Children => _childrenReflection;

        public virtual void Render() {
            if (Visible) {
                RenderA();
                foreach (var child in Children) {
                    child.Render();
                }
                RenderB();
            }
        }

        public virtual void Update() {
            if (Enabled) {
                UpdateA();
                foreach (var child in Children) {
                    child.Update();
                }
                UpdateB();
            }
        }

        public virtual Direct3DRuntime D3DRuntime => _d3dRuntime;

        protected virtual void RenderA() {
        }

        protected virtual void RenderB() {
        }

        protected virtual void UpdateA() {
        }

        protected virtual void UpdateB() {
        }

        private SafeList<INode> _children;
        private ReadOnlyCollection<INode> _childrenReflection;
        private bool _isRoot;
        private INode _parent;
        private Direct3DRuntime _d3dRuntime;

    }
}
