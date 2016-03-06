using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;

namespace Noire.Graphics.Interop.Lights {
    public sealed class SpotLight : DXLight {

        public SpotLight(int index)
            : base(index) {
            _light.Type = LightType.Spot;
        }

        public float Attenuation0 {
            get { return _light.Attenuation0; }
            set { _light.Attenuation0 = value; }
        }

        public float Attenuation1 {
            get { return _light.Attenuation1; }
            set { _light.Attenuation1 = value; }
        }

        public float Attenuation2 {
            get { return _light.Attenuation2; }
            set { _light.Attenuation2 = value; }
        }

        public float Theta {
            get { return _light.Theta; }
            set { _light.Theta = value; }
        }

        public float Phi {
            get { return _light.Phi; }
            set { _light.Phi = value; }
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

        public float Falloff {
            get { return _light.Falloff; }
            set { _light.Falloff = value; }
        }

        public float Range {
            get { return _light.Range; }
            set { _light.Range = value; }
        }

        public Vector3 Direction {
            get { return _light.Direction; }
            set { _light.Direction = value; }
        }

        public Vector3 Position {
            get { return _light.Position; }
            set { _light.Position = value; }
        }

    }
}
