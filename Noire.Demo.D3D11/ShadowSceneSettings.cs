using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Noire.Demo.D3D11.DemoFinal;

namespace Noire.Demo.D3D11 {
    public struct ShadowSceneSettings : ICloneable {

        // Buffer Op!
        public DrawMode DrawMode { get; set; }

        public bool QuadVisible { get; set; }

        public bool ParticleFlameVisible { get; set; }

        public bool ParticleRainVisible { get; set; }

        public bool AreLightsMoving { get; set; }

        public NumberOfLights NumberOfLights { get; set; }

        public bool IsDeceleratorVisible { get; set; }

        public bool IsBarbecueBarVisible { get; set; }

        public bool IsShadowEnabled { get; set; }

        public bool IsReflectionEnabled { get; set; }

        public SurfaceMapping SurfaceMapping { get; set; }

        public bool IsTruckVisible { get; set; }

        public bool IsTireVisible { get; set; }

        public MaterialType MaterialType { get; set; }

        public SkyboxType SkyboxType { get; set; }

        object ICloneable.Clone() {
            return Clone();
        }

        public ShadowSceneSettings Clone() {
            return new ShadowSceneSettings() {
                DrawMode = DrawMode,
                QuadVisible = QuadVisible,
                ParticleFlameVisible = ParticleFlameVisible,
                ParticleRainVisible = ParticleRainVisible,
                AreLightsMoving = AreLightsMoving,
                NumberOfLights = NumberOfLights,
                IsDeceleratorVisible = IsDeceleratorVisible,
                IsBarbecueBarVisible = IsBarbecueBarVisible,
                IsShadowEnabled = IsShadowEnabled,
                IsReflectionEnabled = IsReflectionEnabled,
                SurfaceMapping = SurfaceMapping,
                IsTruckVisible = IsTruckVisible,
                IsTireVisible = IsTireVisible,
                MaterialType = MaterialType,
                SkyboxType = SkyboxType
            };
        }

    }
}
