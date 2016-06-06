namespace Noire.Demo.D3D11 {
    partial class Form1 {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuDisplay = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplayNormal = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplayWireframe = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLights = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLights1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLights2 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLights3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuLightsMoving = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSkybox = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSkyboxDisabled = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSkyboxDesert = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSkyboxSnowfield = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSkyboxFactory = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReflection = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuReflectionDisabled = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTexture = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTextureDisabled = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTextureCopper = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTextureSSteel = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTextureAl = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTexturePlywood = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSurface = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSurfaceDisabled = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSurfaceNM = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSurfaceDM = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShadow = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuShadowDisabled = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuShadowDB = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuModel = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuModelTire = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuModelDecel = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuModelBarb = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuModelTruck = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuParticle = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuParticleRain = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuParticleFlame = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuReflectionReplaceMaterial = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDisplay,
            this.mnuLights,
            this.mnuSkybox,
            this.mnuReflection,
            this.mnuTexture,
            this.mnuSurface,
            this.mnuShadow,
            this.mnuModel,
            this.mnuParticle});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(691, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuDisplay
            // 
            this.mnuDisplay.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDisplayNormal,
            this.mnuDisplayWireframe});
            this.mnuDisplay.Name = "mnuDisplay";
            this.mnuDisplay.Size = new System.Drawing.Size(59, 21);
            this.mnuDisplay.Text = "显示(&Y)";
            // 
            // mnuDisplayNormal
            // 
            this.mnuDisplayNormal.CheckOnClick = true;
            this.mnuDisplayNormal.Name = "mnuDisplayNormal";
            this.mnuDisplayNormal.Size = new System.Drawing.Size(124, 22);
            this.mnuDisplayNormal.Text = "正常模式";
            this.mnuDisplayNormal.Click += new System.EventHandler(this.mnuDisplayNormal_Click);
            // 
            // mnuDisplayWireframe
            // 
            this.mnuDisplayWireframe.CheckOnClick = true;
            this.mnuDisplayWireframe.Name = "mnuDisplayWireframe";
            this.mnuDisplayWireframe.Size = new System.Drawing.Size(124, 22);
            this.mnuDisplayWireframe.Text = "线框模式";
            this.mnuDisplayWireframe.Click += new System.EventHandler(this.mnuDisplayWireframe_Click);
            // 
            // mnuLights
            // 
            this.mnuLights.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLights1,
            this.mnuLights2,
            this.mnuLights3,
            this.toolStripMenuItem1,
            this.mnuLightsMoving});
            this.mnuLights.Name = "mnuLights";
            this.mnuLights.Size = new System.Drawing.Size(59, 21);
            this.mnuLights.Text = "光源(&E)";
            // 
            // mnuLights1
            // 
            this.mnuLights1.CheckOnClick = true;
            this.mnuLights1.Name = "mnuLights1";
            this.mnuLights1.Size = new System.Drawing.Size(124, 22);
            this.mnuLights1.Text = "1个";
            this.mnuLights1.Click += new System.EventHandler(this.mnuLights1_Click);
            // 
            // mnuLights2
            // 
            this.mnuLights2.CheckOnClick = true;
            this.mnuLights2.Name = "mnuLights2";
            this.mnuLights2.Size = new System.Drawing.Size(124, 22);
            this.mnuLights2.Text = "2个";
            this.mnuLights2.Click += new System.EventHandler(this.mnuLights2_Click);
            // 
            // mnuLights3
            // 
            this.mnuLights3.CheckOnClick = true;
            this.mnuLights3.Name = "mnuLights3";
            this.mnuLights3.Size = new System.Drawing.Size(124, 22);
            this.mnuLights3.Text = "3个";
            this.mnuLights3.Click += new System.EventHandler(this.mnuLights3_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(121, 6);
            // 
            // mnuLightsMoving
            // 
            this.mnuLightsMoving.CheckOnClick = true;
            this.mnuLightsMoving.Name = "mnuLightsMoving";
            this.mnuLightsMoving.Size = new System.Drawing.Size(124, 22);
            this.mnuLightsMoving.Text = "光源移动";
            this.mnuLightsMoving.Click += new System.EventHandler(this.mnuLightsMoving_Click);
            // 
            // mnuSkybox
            // 
            this.mnuSkybox.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSkyboxDisabled,
            this.mnuSkyboxDesert,
            this.mnuSkyboxSnowfield,
            this.mnuSkyboxFactory});
            this.mnuSkybox.Name = "mnuSkybox";
            this.mnuSkybox.Size = new System.Drawing.Size(72, 21);
            this.mnuSkybox.Text = "天空盒(&K)";
            // 
            // mnuSkyboxDisabled
            // 
            this.mnuSkyboxDisabled.CheckOnClick = true;
            this.mnuSkyboxDisabled.Name = "mnuSkyboxDisabled";
            this.mnuSkyboxDisabled.Size = new System.Drawing.Size(124, 22);
            this.mnuSkyboxDisabled.Text = "无天空盒";
            this.mnuSkyboxDisabled.Click += new System.EventHandler(this.mnuSkyboxDisabled_Click);
            // 
            // mnuSkyboxDesert
            // 
            this.mnuSkyboxDesert.CheckOnClick = true;
            this.mnuSkyboxDesert.Name = "mnuSkyboxDesert";
            this.mnuSkyboxDesert.Size = new System.Drawing.Size(124, 22);
            this.mnuSkyboxDesert.Text = "沙漠";
            this.mnuSkyboxDesert.Click += new System.EventHandler(this.mnuSkyboxDesert_Click);
            // 
            // mnuSkyboxSnowfield
            // 
            this.mnuSkyboxSnowfield.CheckOnClick = true;
            this.mnuSkyboxSnowfield.Name = "mnuSkyboxSnowfield";
            this.mnuSkyboxSnowfield.Size = new System.Drawing.Size(124, 22);
            this.mnuSkyboxSnowfield.Text = "雪地";
            this.mnuSkyboxSnowfield.Click += new System.EventHandler(this.mnuSkyboxSnowfield_Click);
            // 
            // mnuSkyboxFactory
            // 
            this.mnuSkyboxFactory.CheckOnClick = true;
            this.mnuSkyboxFactory.Name = "mnuSkyboxFactory";
            this.mnuSkyboxFactory.Size = new System.Drawing.Size(124, 22);
            this.mnuSkyboxFactory.Text = "工厂";
            this.mnuSkyboxFactory.Click += new System.EventHandler(this.mnuSkyboxFactory_Click);
            // 
            // mnuReflection
            // 
            this.mnuReflection.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuReflectionDisabled,
            this.toolStripMenuItem2,
            this.mnuReflectionReplaceMaterial});
            this.mnuReflection.Name = "mnuReflection";
            this.mnuReflection.Size = new System.Drawing.Size(60, 21);
            this.mnuReflection.Text = "反射(&R)";
            // 
            // mnuReflectionDisabled
            // 
            this.mnuReflectionDisabled.CheckOnClick = true;
            this.mnuReflectionDisabled.Name = "mnuReflectionDisabled";
            this.mnuReflectionDisabled.Size = new System.Drawing.Size(232, 22);
            this.mnuReflectionDisabled.Text = "关闭反射";
            this.mnuReflectionDisabled.Click += new System.EventHandler(this.mnuReflectionDisabled_Click);
            // 
            // mnuTexture
            // 
            this.mnuTexture.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuTextureDisabled,
            this.mnuTextureCopper,
            this.mnuTextureSSteel,
            this.mnuTextureAl,
            this.mnuTexturePlywood});
            this.mnuTexture.Name = "mnuTexture";
            this.mnuTexture.Size = new System.Drawing.Size(59, 21);
            this.mnuTexture.Text = "纹理(&T)";
            // 
            // mnuTextureDisabled
            // 
            this.mnuTextureDisabled.CheckOnClick = true;
            this.mnuTextureDisabled.Name = "mnuTextureDisabled";
            this.mnuTextureDisabled.Size = new System.Drawing.Size(152, 22);
            this.mnuTextureDisabled.Text = "不使用纹理";
            this.mnuTextureDisabled.Click += new System.EventHandler(this.mnuTextureDisabled_Click);
            // 
            // mnuTextureCopper
            // 
            this.mnuTextureCopper.CheckOnClick = true;
            this.mnuTextureCopper.Name = "mnuTextureCopper";
            this.mnuTextureCopper.Size = new System.Drawing.Size(152, 22);
            this.mnuTextureCopper.Text = "铜";
            this.mnuTextureCopper.Click += new System.EventHandler(this.mnuTextureCopper_Click);
            // 
            // mnuTextureSSteel
            // 
            this.mnuTextureSSteel.CheckOnClick = true;
            this.mnuTextureSSteel.Name = "mnuTextureSSteel";
            this.mnuTextureSSteel.Size = new System.Drawing.Size(152, 22);
            this.mnuTextureSSteel.Text = "不锈钢";
            this.mnuTextureSSteel.Click += new System.EventHandler(this.mnuTextureSSteel_Click);
            // 
            // mnuTextureAl
            // 
            this.mnuTextureAl.CheckOnClick = true;
            this.mnuTextureAl.Name = "mnuTextureAl";
            this.mnuTextureAl.Size = new System.Drawing.Size(152, 22);
            this.mnuTextureAl.Text = "铝";
            this.mnuTextureAl.Click += new System.EventHandler(this.mnuTextureAl_Click);
            // 
            // mnuTexturePlywood
            // 
            this.mnuTexturePlywood.CheckOnClick = true;
            this.mnuTexturePlywood.Name = "mnuTexturePlywood";
            this.mnuTexturePlywood.Size = new System.Drawing.Size(152, 22);
            this.mnuTexturePlywood.Text = "胶合板";
            this.mnuTexturePlywood.Click += new System.EventHandler(this.mnuTexturePlywood_Click);
            // 
            // mnuSurface
            // 
            this.mnuSurface.CheckOnClick = true;
            this.mnuSurface.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSurfaceDisabled,
            this.mnuSurfaceNM,
            this.mnuSurfaceDM});
            this.mnuSurface.Name = "mnuSurface";
            this.mnuSurface.Size = new System.Drawing.Size(97, 21);
            this.mnuSurface.Text = "不光滑表面(&U)";
            // 
            // mnuSurfaceDisabled
            // 
            this.mnuSurfaceDisabled.CheckOnClick = true;
            this.mnuSurfaceDisabled.Name = "mnuSurfaceDisabled";
            this.mnuSurfaceDisabled.Size = new System.Drawing.Size(152, 22);
            this.mnuSurfaceDisabled.Text = "完全光滑";
            this.mnuSurfaceDisabled.Click += new System.EventHandler(this.mnuSurfaceDisabled_Click);
            // 
            // mnuSurfaceNM
            // 
            this.mnuSurfaceNM.CheckOnClick = true;
            this.mnuSurfaceNM.Name = "mnuSurfaceNM";
            this.mnuSurfaceNM.Size = new System.Drawing.Size(152, 22);
            this.mnuSurfaceNM.Text = "法线贴图";
            this.mnuSurfaceNM.Click += new System.EventHandler(this.mnuSurfaceNM_Click);
            // 
            // mnuSurfaceDM
            // 
            this.mnuSurfaceDM.CheckOnClick = true;
            this.mnuSurfaceDM.Enabled = false;
            this.mnuSurfaceDM.Name = "mnuSurfaceDM";
            this.mnuSurfaceDM.Size = new System.Drawing.Size(152, 22);
            this.mnuSurfaceDM.Text = "位移贴图";
            this.mnuSurfaceDM.Visible = false;
            // 
            // mnuShadow
            // 
            this.mnuShadow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuShadowDisabled,
            this.toolStripSeparator1,
            this.mnuShadowDB});
            this.mnuShadow.Name = "mnuShadow";
            this.mnuShadow.Size = new System.Drawing.Size(61, 21);
            this.mnuShadow.Text = "阴影(&D)";
            // 
            // mnuShadowDisabled
            // 
            this.mnuShadowDisabled.CheckOnClick = true;
            this.mnuShadowDisabled.Name = "mnuShadowDisabled";
            this.mnuShadowDisabled.Size = new System.Drawing.Size(184, 22);
            this.mnuShadowDisabled.Text = "关闭阴影";
            this.mnuShadowDisabled.Click += new System.EventHandler(this.mnuShadowDisabled_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(181, 6);
            // 
            // mnuShadowDB
            // 
            this.mnuShadowDB.CheckOnClick = true;
            this.mnuShadowDB.Name = "mnuShadowDB";
            this.mnuShadowDB.Size = new System.Drawing.Size(184, 22);
            this.mnuShadowDB.Text = "阴影深度缓冲可视化";
            this.mnuShadowDB.Click += new System.EventHandler(this.mnuShadowDB_Click);
            // 
            // mnuModel
            // 
            this.mnuModel.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuModelTire,
            this.mnuModelDecel,
            this.mnuModelBarb,
            this.mnuModelTruck});
            this.mnuModel.Name = "mnuModel";
            this.mnuModel.Size = new System.Drawing.Size(58, 21);
            this.mnuModel.Text = "模型(&L)";
            // 
            // mnuModelTire
            // 
            this.mnuModelTire.CheckOnClick = true;
            this.mnuModelTire.Name = "mnuModelTire";
            this.mnuModelTire.Size = new System.Drawing.Size(152, 22);
            this.mnuModelTire.Text = "轮胎";
            this.mnuModelTire.Click += new System.EventHandler(this.mnuModelTire_Click);
            // 
            // mnuModelDecel
            // 
            this.mnuModelDecel.CheckOnClick = true;
            this.mnuModelDecel.Name = "mnuModelDecel";
            this.mnuModelDecel.Size = new System.Drawing.Size(152, 22);
            this.mnuModelDecel.Text = "减速器剖视";
            this.mnuModelDecel.Click += new System.EventHandler(this.mnuModelDecel_Click);
            // 
            // mnuModelBarb
            // 
            this.mnuModelBarb.CheckOnClick = true;
            this.mnuModelBarb.Name = "mnuModelBarb";
            this.mnuModelBarb.Size = new System.Drawing.Size(152, 22);
            this.mnuModelBarb.Text = "烧烤架";
            this.mnuModelBarb.Click += new System.EventHandler(this.mnuModelBarb_Click);
            // 
            // mnuModelTruck
            // 
            this.mnuModelTruck.CheckOnClick = true;
            this.mnuModelTruck.Name = "mnuModelTruck";
            this.mnuModelTruck.Size = new System.Drawing.Size(152, 22);
            this.mnuModelTruck.Text = "货车";
            this.mnuModelTruck.Click += new System.EventHandler(this.mnuModelTruck_Click);
            // 
            // mnuParticle
            // 
            this.mnuParticle.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuParticleRain,
            this.mnuParticleFlame});
            this.mnuParticle.Name = "mnuParticle";
            this.mnuParticle.Size = new System.Drawing.Size(59, 21);
            this.mnuParticle.Text = "粒子(&P)";
            // 
            // mnuParticleRain
            // 
            this.mnuParticleRain.CheckOnClick = true;
            this.mnuParticleRain.Name = "mnuParticleRain";
            this.mnuParticleRain.Size = new System.Drawing.Size(152, 22);
            this.mnuParticleRain.Text = "降雨";
            this.mnuParticleRain.Click += new System.EventHandler(this.mnuParticleRain_Click);
            // 
            // mnuParticleFlame
            // 
            this.mnuParticleFlame.CheckOnClick = true;
            this.mnuParticleFlame.Name = "mnuParticleFlame";
            this.mnuParticleFlame.Size = new System.Drawing.Size(152, 22);
            this.mnuParticleFlame.Text = "火焰";
            this.mnuParticleFlame.Click += new System.EventHandler(this.mnuParticleFlame_Click);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(691, 371);
            this.label1.TabIndex = 2;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(229, 6);
            // 
            // mnuReflectionReplaceMaterial
            // 
            this.mnuReflectionReplaceMaterial.Name = "mnuReflectionReplaceMaterial";
            this.mnuReflectionReplaceMaterial.Size = new System.Drawing.Size(232, 22);
            this.mnuReflectionReplaceMaterial.Text = "替换减速器表面材质为抛光铝";
            this.mnuReflectionReplaceMaterial.Click += new System.EventHandler(this.mnuReflectionReplaceMaterial_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 396);
            this.Controls.Add(this.label1);
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ToolStripMenuItem mnuLights;
        private System.Windows.Forms.ToolStripMenuItem mnuLights1;
        private System.Windows.Forms.ToolStripMenuItem mnuLights2;
        private System.Windows.Forms.ToolStripMenuItem mnuLights3;
        private System.Windows.Forms.ToolStripMenuItem mnuTexture;
        private System.Windows.Forms.ToolStripMenuItem mnuTextureDisabled;
        private System.Windows.Forms.ToolStripMenuItem mnuSkybox;
        private System.Windows.Forms.ToolStripMenuItem mnuSkyboxDisabled;
        private System.Windows.Forms.ToolStripMenuItem mnuSkyboxDesert;
        private System.Windows.Forms.ToolStripMenuItem mnuSkyboxSnowfield;
        private System.Windows.Forms.ToolStripMenuItem mnuReflection;
        private System.Windows.Forms.ToolStripMenuItem mnuReflectionDisabled;
        private System.Windows.Forms.ToolStripMenuItem mnuSurface;
        private System.Windows.Forms.ToolStripMenuItem mnuSurfaceDisabled;
        private System.Windows.Forms.ToolStripMenuItem mnuSurfaceNM;
        private System.Windows.Forms.ToolStripMenuItem mnuSurfaceDM;
        private System.Windows.Forms.ToolStripMenuItem mnuShadow;
        private System.Windows.Forms.ToolStripMenuItem mnuShadowDisabled;
        private System.Windows.Forms.ToolStripMenuItem mnuModel;
        private System.Windows.Forms.ToolStripMenuItem mnuModelTire;
        private System.Windows.Forms.ToolStripMenuItem mnuModelDecel;
        private System.Windows.Forms.ToolStripMenuItem mnuModelBarb;
        private System.Windows.Forms.ToolStripMenuItem mnuModelTruck;
        private System.Windows.Forms.ToolStripMenuItem mnuDisplay;
        private System.Windows.Forms.ToolStripMenuItem mnuDisplayNormal;
        private System.Windows.Forms.ToolStripMenuItem mnuDisplayWireframe;
        private System.Windows.Forms.ToolStripMenuItem mnuTextureSSteel;
        private System.Windows.Forms.ToolStripMenuItem mnuTextureAl;
        private System.Windows.Forms.ToolStripMenuItem mnuTexturePlywood;
        private System.Windows.Forms.ToolStripMenuItem mnuParticle;
        private System.Windows.Forms.ToolStripMenuItem mnuParticleRain;
        private System.Windows.Forms.ToolStripMenuItem mnuParticleFlame;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuShadowDB;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuLightsMoving;
        private System.Windows.Forms.ToolStripMenuItem mnuSkyboxFactory;
        private System.Windows.Forms.ToolStripMenuItem mnuTextureCopper;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem mnuReflectionReplaceMaterial;
    }
}

