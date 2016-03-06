using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D9;

namespace Noire.Graphics.Interop.Lights {
    public abstract class DXLight {

        public DXLight(int index) {
            Index = index;
        }

        public bool Enabled { get; set; } = true;

        public int Index { get; set; }

        public Light RawLight => _light;

        public static DXLight CreateDxLight(int index, bool enabled, Light light) {
            switch (light.Type) {
                case LightType.Point:
                    var pl = new PointLight(index);
                    pl.Ambient = light.Ambient.ToColor();
                    pl.Attenuation0 = light.Attenuation0;
                    pl.Attenuation1 = light.Attenuation1;
                    pl.Attenuation2 = light.Attenuation2;
                    pl.Diffuse = light.Diffuse.ToColor();
                    pl.Position = light.Position;
                    pl.Range = light.Range;
                    pl.Specular = light.Specular.ToColor();
                    pl.Enabled = enabled;
                    return pl;
                case LightType.Directional:
                    var dl = new DirectionalLight(index);
                    dl.Ambient = light.Ambient.ToColor();
                    dl.Diffuse = light.Diffuse.ToColor();
                    dl.Specular = light.Specular.ToColor();
                    dl.Direction = light.Direction;
                    dl.Enabled = enabled;
                    return dl;
                case LightType.Spot:
                    var spot = new SpotLight(index);
                    spot.Ambient = light.Ambient.ToColor();
                    spot.Attenuation0 = light.Attenuation0;
                    spot.Attenuation1 = light.Attenuation1;
                    spot.Attenuation2 = light.Attenuation2;
                    spot.Diffuse = light.Diffuse.ToColor();
                    spot.Position = light.Position;
                    spot.Range = light.Range;
                    spot.Specular = light.Specular.ToColor();
                    spot.Falloff = light.Falloff;
                    spot.Phi = light.Phi;
                    spot.Theta = light.Theta;
                    spot.Enabled = enabled;
                    return spot;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected Light _light;

    }
}

