using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Graphics.Nodes;
using Noire.Misc;
using Noire.Misc.Extensions;
using SharpDX.Direct3D9;

namespace Noire.Graphics {
    public abstract class Node : INode {

        public Node(SceneNode scene, bool isRoot) {
            _isRoot = isRoot;
            _children = new SafeList<INode>();
            _childrenReflection = _children.AsReadOnly();
            _scene = scene;
        }

        public event EventHandler<DeviceChangedEventArgs> DeviceChanged;

        public event EventHandler<EventArgs> DeviceReset;

        public event EventHandler<EventArgs> DeviceLost;

        public bool UpdateEnabled { get; set; } = true;

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

                    var oldDevice = GetSourceDevice();
                    var newDevice = GetSourceDeviceOf(value);
                    _sourceDevice = newDevice;
                    if (oldDevice != newDevice) {
                        OnDeviceChanged(this, new DeviceChangedEventArgs(oldDevice, newDevice));
                    }
                }
            }
        }

        public virtual void Dispose() {
            foreach (var child in Children) {
                child.Dispose();
            }
        }

        public bool IsAncestorOf(INode node) {
            if (node == null || node == this) {
                return false;
            }
            if (Children.Contains(node)) {
                return true;
            }
            foreach (var child in Children) {
                if (child.IsAncestorOf(node)) {
                    return true;
                }
            }
            return false;
        }

        public bool RenderEnabled { get; set; } = true;

        public void AddChild(INode node) {
            _children.Add(node);
        }

        public void RemoveChild(INode node) {
            _children.Remove(node);
        }

        public ReadOnlyCollection<INode> Children => _childrenReflection;

        public virtual void Render() {
            if (RenderEnabled) {
                RenderBeforeChildren();
                foreach (var child in Children) {
                    child.Render();
                }
                RenderAfterChildren();
            }
        }

        public virtual void Update() {
            if (UpdateEnabled) {
                UpdateBeforeChildren();
                foreach (var child in Children) {
                    child.Update();
                }
                UpdateAfterChildren();
            }
        }

        public virtual SceneNode Scene => _scene;

        public Device SourceDevice => SourceDevice;

        protected virtual void RenderBeforeChildren() {
        }

        protected virtual void RenderAfterChildren() {
        }

        protected virtual void UpdateBeforeChildren() {
        }

        protected virtual void UpdateAfterChildren() {
        }

        protected virtual void OnDeviceChanged(object sender, DeviceChangedEventArgs e) {
            DeviceChanged.RaiseEvent(sender, e);
            foreach (var child in Children) {
                (child as Node)?.OnDeviceChanged(sender, e);
            }
        }

        protected virtual void OnDeviceReset(object sender, EventArgs e) {
            DeviceReset.RaiseEvent(sender, e);
            foreach (var child in Children) {
                (child as Node)?.OnDeviceReset(sender, e);
            }
        }

        protected virtual void OnDeviceLost(object sender, EventArgs e) {
            DeviceLost.RaiseEvent(sender, e);
            foreach (var child in Children) {
                (child as Node)?.OnDeviceLost(sender, e);
            }
        }

        private Device GetSourceDevice() => GetSourceDeviceOf(this);

        private static Device GetSourceDeviceOf(INode whichNode) {
            var node = whichNode.Parent;
            while (node != null) {
                if (node is CameraNode) {
                    return (node as CameraNode).Device;
                }
                node = node.Parent;
            }
            return null;
        }

        private SafeList<INode> _children;
        private ReadOnlyCollection<INode> _childrenReflection;
        private bool _isRoot;
        private INode _parent;
        private SceneNode _scene;
        private Device _sourceDevice;

    }
}
