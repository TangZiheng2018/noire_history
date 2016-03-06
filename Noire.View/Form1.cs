using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Noire.Graphics;
using Noire.Graphics.Interop.Lights;
using Noire.Graphics.Nodes;
using SharpDX;
using Noire.Graphics.Nodes.Tests;

namespace Noire.View {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers() {
            Load += Form1_Load;
            mnuFileExit.Click += MnuFileExit_Click;
            mnuTestsDefaultTriangle.CheckedChanged += MnuTestsDefaultTriangle_CheckedChanged;
            mnuTestsRotatingTriangle.CheckedChanged += MnuTestsRotatingTriangle_CheckedChanged;
            mnuTestsIndexBuffer.CheckedChanged += MnuTestsIndexBuffer_CheckedChanged;
            mnuTestsLightAndMaterial.CheckedChanged += MnuTestsLightAndMaterial_CheckedChanged;
            mnuTestsTexturedCube.CheckedChanged += MnuTestsTexturedCube_CheckedChanged;
            mnuTestsTexturedCubeAndLight.CheckedChanged += MnuTestsTexturedCubeAndLight_CheckedChanged;
            mnuTestsSphereLight.CheckedChanged += MnuTestsSphereLight_CheckedChanged;
        }

        private void MnuTestsSphereLight_CheckedChanged(object sender, EventArgs e) {
            if (mnuTestsSphereLight.Checked) {
            } else {
            }
        }

        private void MnuTestsTexturedCubeAndLight_CheckedChanged(object sender, EventArgs e) {
            if (mnuTestsTexturedCubeAndLight.Checked) {
            } else {
            }
        }

        private void MnuTestsTexturedCube_CheckedChanged(object sender, EventArgs e) {
            if (mnuTestsTexturedCube.Checked) {
            } else {
            }
        }

        private void MnuTestsLightAndMaterial_CheckedChanged(object sender, EventArgs e) {
            if (mnuTestsLightAndMaterial.Checked) {
            } else {
            }
        }

        private void MnuTestsIndexBuffer_CheckedChanged(object sender, EventArgs e) {
            if (mnuTestsIndexBuffer.Checked) {
            } else {
            }
        }

        private void MnuTestsRotatingTriangle_CheckedChanged(object sender, EventArgs e) {
            if (mnuTestsRotatingTriangle.Checked) {
            } else {
            }
        }

        private void MnuTestsDefaultTriangle_CheckedChanged(object sender, EventArgs e) {
            if (mnuTestsDefaultTriangle.Checked) {
            } else {
            }
        }

        private void MnuFileExit_Click(object sender, EventArgs e) {
            Close();
        }

        private void Form1_Load(object sender, EventArgs e) {
            Show();
            Focus();
            using (var rt = new Direct3DRuntime(this)) {
                var camera = new CameraNode(rt);
                camera.Eye = new Vector3(0, -30, 0);
                var pers = new PerspectiveProjectionNode(rt);
                var trans = new RotatingTransformNode(rt);
                var lighting = new LightingNode(rt);
                lighting.Lighting = true;
                var mtl = new MaterialNode(rt);
                mtl.Ambient = new SharpDX.Color(1f, 1f, 1f);
                mtl.Diffuse = new SharpDX.Color(1f, 1f, 1f);
                mtl.Specular = new SharpDX.Color(0.8f, 0.3f, 0.3f);
                mtl.Emissive = new SharpDX.Color(0f, 0f, 0f);
                var obj = new SimpleCubeNode(rt);

                var dxLight = new PointLight(0);
                dxLight.Ambient = new SharpDX.Color(0.8f, 0.8f, 0.8f);
                dxLight.Diffuse = new SharpDX.Color(1f, 1f, 1f);
                dxLight.Specular = new SharpDX.Color(0.3f, 0.3f, 0.3f);
                dxLight.Position = new Vector3(20, 0, 0);
                dxLight.Attenuation0 = 1f;
                dxLight.Attenuation1 = 0f;
                dxLight.Attenuation2 = 0f;
                dxLight.Range = 20f;
                lighting.Lights.Add(dxLight);

                mtl.AddChild(obj);
                lighting.AddChild(mtl);
                trans.AddChild(lighting);
                pers.AddChild(trans);
                camera.AddChild(pers);
                rt.AddChild(camera);
                rt.Run();
            }
        }

        private class RotatingTransformNode : TransformNode {

            public RotatingTransformNode(Direct3DRuntime runtime)
                : base(runtime) {
                _degree = 0;
            }

            protected override void UpdateA() {
                _degree += 2;
                var rad = MathUtil.DegreesToRadians(_degree);
                Transform = Matrix.RotationY(rad) * Matrix.RotationZ(rad * 0.5f) * Matrix.RotationX(rad * 0.25f);
            }

            private float _degree;

        }

    }
}
