using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;
using Noire.Extensions;

namespace Noire.Graphics.Nodes {
    public sealed class MaterialNode : Node {

        public MaterialNode(SceneNode scene)
            : base(scene) {
            _material = new Material();
        }

        public Color Ambient {
            get { return _material.Ambient.ToColor(); }
            set { _material.Ambient = value; }
        }

        public Color Diffuse {
            get { return _material.Diffuse.ToColor(); }
            set { _material.Diffuse = value; }
        }

        public Color Specular {
            get { return _material.Specular.ToColor(); }
            set { _material.Specular = value; }
        }

        public Color Emissive {
            get { return _material.Emissive.ToColor(); }
            set { _material.Emissive = value; }
        }

        public float Power {
            get { return _material.Power; }
            set { _material.Power = value; }
        }

        public Material Material => _material;

        protected override void RenderBeforeChildren() {
            var device = Scene.CurrentDevice;
            if (device != null) {
                _originalMaterial = device.Material;
                device.Material = _material;
            }
        }

        protected override void RenderAfterChildren() {
            var device = Scene.CurrentDevice;
            if (device != null) {
                device.Material = _originalMaterial;
            }
        }

        private Material _material;
        private Material _originalMaterial;

    }
}
