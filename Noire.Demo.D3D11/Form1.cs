using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Noire.Graphics.D3D11;

namespace Noire.Demo.D3D11 {
    public partial class Form1 : Form {

        public Form1() {
            InitializeComponent();
            InitializeEventHandlers();

            _app = new D3DApp11(this);
        }

        private void InitializeEventHandlers() {
            Load += Form1_Load;
            FormClosed += Form1_FormClosed;
        }

        private void Form1_FormClosed(object sender, EventArgs e) {
            _app.Terminate();
            _app.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e) {
            _app.Initialize();
            _app.ManualVSync = true;
            _app.Run();
        }

        private readonly D3DApp11 _app;

    }
}
