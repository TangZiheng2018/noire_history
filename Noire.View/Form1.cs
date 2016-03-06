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
using Noire.Graphics.Nodes;
using Noire.Graphics.Obsolete;
using Noire.Graphics.Obsolete.Elements.Tests;
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
                var o = new SphereLightTest(_dx);
                o.LoadSTL(@"res/models/sphere_bin.stl");
                _dx.Stage.Children.Add(o);
            } else {
                _dx.Stage.Children.RemoveAll(obj => obj is SphereLightTest);
            }
        }

        private void MnuTestsTexturedCubeAndLight_CheckedChanged(object sender, EventArgs e) {
            if (mnuTestsTexturedCubeAndLight.Checked) {
                var o = new TexturedCubeAndLightTest(_dx);
                o.SetTexture(@"res/images/318753-1312240Z43725.jpg");
                _dx.Stage.Children.Add(o);
            } else {
                _dx.Stage.Children.RemoveAll(obj => obj is TexturedCubeAndLightTest);
            }
        }

        private void MnuTestsTexturedCube_CheckedChanged(object sender, EventArgs e) {
            if (mnuTestsTexturedCube.Checked) {
                var o = new TexturedCubeTest(_dx);
                o.SetTexture(@"res/images/metal-material-texture.jpg");
                _dx.Stage.Children.Add(o);
            } else {
                _dx.Stage.Children.RemoveAll(obj => obj is TexturedCubeTest);
            }
        }

        private void MnuTestsLightAndMaterial_CheckedChanged(object sender, EventArgs e) {
            if (mnuTestsLightAndMaterial.Checked) {
                _dx.Stage.Children.Add(new LightAndMaterialTest(_dx));
            } else {
                _dx.Stage.Children.RemoveAll(obj => obj is LightAndMaterialTest);
            }
        }

        private void MnuTestsIndexBuffer_CheckedChanged(object sender, EventArgs e) {
            if (mnuTestsIndexBuffer.Checked) {
                _dx.Stage.Children.Add(new IndexBufferTest(_dx));
            } else {
                _dx.Stage.Children.RemoveAll(obj => obj is IndexBufferTest);
            }
        }

        private void MnuTestsRotatingTriangle_CheckedChanged(object sender, EventArgs e) {
            if (mnuTestsRotatingTriangle.Checked) {
                _dx.Stage.Children.Add(new RotatingTriangle(_dx));
            } else {
                _dx.Stage.Children.RemoveAll(obj => obj is RotatingTriangle);
            }
        }

        private void MnuTestsDefaultTriangle_CheckedChanged(object sender, EventArgs e) {
            if (mnuTestsDefaultTriangle.Checked) {
                _dx.Stage.Children.Add(new DefaultTriangle(_dx));
            } else {
                _dx.Stage.Children.RemoveAll(obj => obj is DefaultTriangle);
            }
        }

        private void MnuFileExit_Click(object sender, EventArgs e) {
            Close();
        }

        private void Form1_Load(object sender, EventArgs e) {
            Show();
            Focus();
            //using (_dx = new RenderManager(this)) {
            //    _dx.Run();
            //}
            using (var rt = new Direct3DRuntime(this)) {
                var device = new DeviceNode(rt, 0);
                var camera = new CameraNode(rt);
                camera.Eye = new Vector3(0, -30, 0);
                var pers = new PerspectiveProjectionNode(rt);
                var trans = new RotatingTransformNode(rt);
                var lighting = new LightingNode(rt);
                lighting.Lighting = false;
                var obj = new SimpleCubeNode(rt);
                lighting.AddChild(obj);
                trans.AddChild(lighting);
                pers.AddChild(trans);
                camera.AddChild(pers);
                device.AddChild(camera);
                rt.AddChild(device);
                rt.Run();
            }
        }

        private RenderManager _dx;

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
