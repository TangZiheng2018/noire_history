using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;

namespace Noire.Graphics.Nodes {
    public sealed class MaterialNode : Node {

        public MaterialNode(Direct3DRuntime runtime)
            : base(runtime, false) {
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

        protected override void RenderA() {
            var device = D3DRuntime.CurrentCamera?.Device;
            if (device != null) {
                _originalMaterial = device.Material;
                device.Material = _material;
            }
        }

        protected override void RenderB() {
            var device = D3DRuntime.CurrentCamera?.Device;
            if (device != null) {
                device.Material = _originalMaterial;
            }
        }

        private Material _material;
        private Material _originalMaterial;

    }
}
