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
using Noire.Graphics.Elements.Tests;

namespace Noire.View
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Show();
            Focus();
            using (var dx = new RenderManager(this))
            {
                DisplayObject displayObject;
                displayObject = new IndexBufferTest(dx);
                displayObject.Initialize();
                dx.Stage.Children.Add(displayObject);
                dx.Run();
            }
        }

    }
}
