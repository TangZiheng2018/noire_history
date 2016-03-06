﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D9;

namespace Noire.Graphics.Nodes {
    public class PerspectiveProjectionNode : Node {

        public PerspectiveProjectionNode(Direct3DRuntime runtime)
            : base(runtime, false) {
        }

        private static Matrix PerspectiveWorkaround(float fov, float aspect, float near, float far) {
            var m = new Matrix();
            m.M11 = (float)(1 / Math.Tan(fov * 0.5)) / aspect;
            m.M22 = (float)(1 / Math.Tan(fov * 0.5));
            m.M33 = far / (far - near);
            m.M34 = 1;
            m.M44 = far * near / (near - far);
            return m;
        }

        protected override void RenderA() {
            _originalProjectionMatrix = (D3DRuntime.CurrentCamera?.Device.GetTransform(TransformState.Projection)).GetValueOrDefault();
            var clientSize = D3DRuntime.Control.ClientSize;
            var projectionMatrix = PerspectiveWorkaround(MathUtil.DegreesToRadians(FieldOfViewDeg), (float)clientSize.Width / clientSize.Height, NearPlane, FarPlane);
            D3DRuntime.CurrentCamera?.Device?.SetTransform(TransformState.Projection, projectionMatrix);
        }

        protected override void RenderB() {
            D3DRuntime.CurrentCamera?.Device?.SetTransform(TransformState.Projection, _originalProjectionMatrix);
        }

        public float FieldOfViewDeg { get; set; } = 45;

        public float NearPlane { get; set; } = 0;

        public float FarPlane { get; set; } = 1000;

        private Matrix _originalProjectionMatrix;

    }
}