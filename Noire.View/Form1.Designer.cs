namespace Noire.View
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTests = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTestsDefaultTriangle = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTestsRotatingTriangle = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTestsIndexBuffer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTestsLightAndMaterial = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTestsTexturedCube = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTestsTexturedCubeAndLight = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTestsSphereLight = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuTests});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(58, 21);
            this.mnuFile.Text = "文件(&F)";
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.Size = new System.Drawing.Size(152, 22);
            this.mnuFileExit.Text = "退出(&X)";
            // 
            // mnuTests
            // 
            this.mnuTests.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuTestsDefaultTriangle,
            this.mnuTestsRotatingTriangle,
            this.mnuTestsIndexBuffer,
            this.mnuTestsLightAndMaterial,
            this.mnuTestsTexturedCube,
            this.mnuTestsTexturedCubeAndLight,
            this.mnuTestsSphereLight});
            this.mnuTests.Name = "mnuTests";
            this.mnuTests.Size = new System.Drawing.Size(59, 21);
            this.mnuTests.Text = "测试(&T)";
            // 
            // mnuTestsDefaultTriangle
            // 
            this.mnuTestsDefaultTriangle.CheckOnClick = true;
            this.mnuTestsDefaultTriangle.Name = "mnuTestsDefaultTriangle";
            this.mnuTestsDefaultTriangle.Size = new System.Drawing.Size(208, 22);
            this.mnuTestsDefaultTriangle.Text = "DefaultTriangle";
            // 
            // mnuTestsRotatingTriangle
            // 
            this.mnuTestsRotatingTriangle.CheckOnClick = true;
            this.mnuTestsRotatingTriangle.Name = "mnuTestsRotatingTriangle";
            this.mnuTestsRotatingTriangle.Size = new System.Drawing.Size(208, 22);
            this.mnuTestsRotatingTriangle.Text = "RotatingTriangle";
            // 
            // mnuTestsIndexBuffer
            // 
            this.mnuTestsIndexBuffer.CheckOnClick = true;
            this.mnuTestsIndexBuffer.Name = "mnuTestsIndexBuffer";
            this.mnuTestsIndexBuffer.Size = new System.Drawing.Size(208, 22);
            this.mnuTestsIndexBuffer.Text = "IndexBuffer";
            // 
            // mnuTestsLightAndMaterial
            // 
            this.mnuTestsLightAndMaterial.CheckOnClick = true;
            this.mnuTestsLightAndMaterial.Name = "mnuTestsLightAndMaterial";
            this.mnuTestsLightAndMaterial.Size = new System.Drawing.Size(208, 22);
            this.mnuTestsLightAndMaterial.Text = "LightAndMaterial";
            // 
            // mnuTestsTexturedCube
            // 
            this.mnuTestsTexturedCube.CheckOnClick = true;
            this.mnuTestsTexturedCube.Name = "mnuTestsTexturedCube";
            this.mnuTestsTexturedCube.Size = new System.Drawing.Size(208, 22);
            this.mnuTestsTexturedCube.Text = "TexturedCube";
            // 
            // mnuTestsTexturedCubeAndLight
            // 
            this.mnuTestsTexturedCubeAndLight.CheckOnClick = true;
            this.mnuTestsTexturedCubeAndLight.Name = "mnuTestsTexturedCubeAndLight";
            this.mnuTestsTexturedCubeAndLight.Size = new System.Drawing.Size(208, 22);
            this.mnuTestsTexturedCubeAndLight.Text = "TexturedCubeAndLight";
            // 
            // mnuTestsSphereLight
            // 
            this.mnuTestsSphereLight.CheckOnClick = true;
            this.mnuTestsSphereLight.Name = "mnuTestsSphereLight";
            this.mnuTestsSphereLight.Size = new System.Drawing.Size(208, 22);
            this.mnuTestsSphereLight.Text = "SphereLight";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.ToolStripMenuItem mnuTests;
        private System.Windows.Forms.ToolStripMenuItem mnuTestsDefaultTriangle;
        private System.Windows.Forms.ToolStripMenuItem mnuTestsRotatingTriangle;
        private System.Windows.Forms.ToolStripMenuItem mnuTestsIndexBuffer;
        private System.Windows.Forms.ToolStripMenuItem mnuTestsLightAndMaterial;
        private System.Windows.Forms.ToolStripMenuItem mnuTestsTexturedCube;
        private System.Windows.Forms.ToolStripMenuItem mnuTestsTexturedCubeAndLight;
        private System.Windows.Forms.ToolStripMenuItem mnuTestsSphereLight;
    }
}

