using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Graphics.Interop.Lights;
using SharpDX;
using SharpDX.Direct3D9;

namespace Noire.Graphics.Nodes {
    public class LightingNode : Node {

        public LightingNode(SceneNode scene)
            : base(scene, false) {
            _lights = new List<DXLight>();
            _originalLights = new Queue<DXLight>();
        }

        public bool Lighting { get; set; }

        public List<DXLight> Lights => _lights;

        protected override void RenderBeforeChildren() {
            var device = Scene.CurrentDevice;
            if (device != null) {
                _originalLighting = device.GetRenderState<bool>(RenderState.Lighting);
                device.SetRenderState(RenderState.Lighting, Lighting);
                if (Lighting) {
                    foreach (var light in Lights) {
                        var rawLight = light.RawLight;
                        try {
                            _originalLights.Enqueue(DXLight.CreateDxLight(light.Index, device.IsLightEnabled(light.Index), device.GetLight(light.Index)));
                        } catch (SharpDXException) {
                        }
                        device.SetLight(light.Index, ref rawLight);
                        device.EnableLight(light.Index, light.Enabled);

                        device.SetRenderState(RenderState.Ambient, new Color(92, 92, 92).ToBgra());
                    }
                }
            }
        }

        protected override void RenderAfterChildren() {
            var device = Scene.CurrentDevice;
            if (device != null) {
                if (Lighting) {
                    while (_originalLights.Count > 0) {
                        var light = _originalLights.Dequeue();
                        var rawLight = light.RawLight;
                        device.SetLight(light.Index, ref rawLight);
                        device.EnableLight(light.Index, light.Enabled);
                    }
                }
                device.SetRenderState(RenderState.Lighting, _originalLighting);
            }
        }

        private bool _originalLighting;
        private Queue<DXLight> _originalLights;
        private List<DXLight> _lights;

    }
}
