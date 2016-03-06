﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Noire.Graphics.Nodes;
using SharpDX.Direct3D9;
using SharpDX.Windows;

namespace Noire.Graphics {
    public sealed class Direct3DRuntime : Node {

        public Direct3DRuntime(Control control)
            : base(null, true) {
            _control = control;
            _direct3D = new Direct3D();
        }

        public override void Dispose() {
            NoireUtilities.SafeDispose(ref _direct3D);
            _control = null;
            base.Dispose();
        }

        public void Run() {
            using (var renderLoop = new RenderLoop(_control)) {
                while (renderLoop.NextFrame()) {
                    Update();
                    Render();
                }
            }
        }

        public override Direct3DRuntime D3DRuntime => this;

        public CameraNode CurrentCamera { get; set; }

        public Direct3D Direct3D => _direct3D;

        public Control Control => _control;

        private Control _control;
        private Direct3D _direct3D;

    }
}
