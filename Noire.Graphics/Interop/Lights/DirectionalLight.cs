using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;
using Noire.Extensions;

namespace Noire.Graphics.Interop.Lights {
    public sealed class DirectionalLight : DXLight {

        public DirectionalLight(int index)
            : base(index) {
            _light.Type = LightType.Directional;
        }

        public Color Diffuse {
            get { return _light.Diffuse.ToColor(); }
            set { _light.Diffuse = value; }
        }

        public Color Ambient {
            get { return _light.Ambient.ToColor(); }
            set { _light.Ambient = value; }
        }

        public Color Specular {
            get { return _light.Specular.ToColor(); }
            set { _light.Specular = value; }
        }

        public Vector3 Direction {
            get { return _light.Direction; }
            set { _light.Direction = value; }
        }

    }
}
