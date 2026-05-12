namespace GSP_SJ.Form_Chart
{
    partial class UCZoom
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.tool_IsFurnace = new System.Windows.Forms.ToolStripLabel();
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.btnZoom = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.zoomPanel = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolSelect = new System.Windows.Forms.ToolStripButton();
            this.toolROI = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.tool坐标旋转 = new System.Windows.Forms.ToolStripButton();
            this.btn重新定位 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zoomPanel)).BeginInit();
            this.zoomPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Segoe UI", 9.216F);
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(21, 21);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolSelect,
            this.toolROI,
            this.toolStripButton1,
            this.tool坐标旋转,
            this.btn重新定位,
            this.toolStripLabel1,
            this.toolStripComboBox1,
            this.tool_IsFurnace});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1581, 29);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(44, 25);
            this.toolStripLabel1.Text = "定位";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 29);
            this.toolStripComboBox1.SelectedIndexChanged += new System.EventHandler(this.toolStripComboBox1_SelectedIndexChanged);
            // 
            // tool_IsFurnace
            // 
            this.tool_IsFurnace.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tool_IsFurnace.BackColor = System.Drawing.Color.Red;
            this.tool_IsFurnace.Font = new System.Drawing.Font("Segoe UI", 13.824F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tool_IsFurnace.ForeColor = System.Drawing.Color.Red;
            this.tool_IsFurnace.IsLink = true;
            this.tool_IsFurnace.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.tool_IsFurnace.LinkColor = System.Drawing.Color.Red;
            this.tool_IsFurnace.Name = "tool_IsFurnace";
            this.tool_IsFurnace.Size = new System.Drawing.Size(92, 32);
            this.tool_IsFurnace.Text = "未定位";
            this.tool_IsFurnace.Visible = false;
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.zoomPanel);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 29);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonPanel1.Size = new System.Drawing.Size(1581, 935);
            this.kryptonPanel1.TabIndex = 1;
            // 
            // btnZoom
            // 
            this.btnZoom.Location = new System.Drawing.Point(-1, -2);
            this.btnZoom.Name = "btnZoom";
            this.btnZoom.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnZoom.Size = new System.Drawing.Size(25, 25);
            this.btnZoom.TabIndex = 0;
            this.btnZoom.Values.Image = global::GSP_SJ.Properties.Resources.icons8_缩小_16;
            this.btnZoom.Values.Text = "";
            this.btnZoom.Click += new System.EventHandler(this.btnZoom_Click);
            // 
            // zoomPanel
            // 
            this.zoomPanel.Controls.Add(this.btnZoom);
            this.zoomPanel.Controls.Add(this.pictureBox1);
            this.zoomPanel.Location = new System.Drawing.Point(1, 1);
            this.zoomPanel.Name = "zoomPanel";
            this.zoomPanel.Size = new System.Drawing.Size(650, 400);
            this.zoomPanel.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(650, 400);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // toolSelect
            // 
            this.toolSelect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolSelect.Image = global::GSP_SJ.Properties.Resources.Select;
            this.toolSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSelect.Name = "toolSelect";
            this.toolSelect.Size = new System.Drawing.Size(30, 29);
            this.toolSelect.Text = "toolStripButton2";
            this.toolSelect.Visible = false;
            this.toolSelect.Click += new System.EventHandler(this.toolSelect_Click);
            // 
            // toolROI
            // 
            this.toolROI.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolROI.Image = global::GSP_SJ.Properties.Resources.ROIRect;
            this.toolROI.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolROI.Name = "toolROI";
            this.toolROI.Size = new System.Drawing.Size(30, 25);
            this.toolROI.Text = "toolStripButton3";
            this.toolROI.Visible = false;
            this.toolROI.Click += new System.EventHandler(this.toolROI_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::GSP_SJ.Properties.Resources._89;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(30, 25);
            this.toolStripButton1.Text = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // tool坐标旋转
            // 
            this.tool坐标旋转.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tool坐标旋转.Image = global::GSP_SJ.Properties.Resources._23;
            this.tool坐标旋转.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tool坐标旋转.Name = "tool坐标旋转";
            this.tool坐标旋转.Size = new System.Drawing.Size(30, 25);
            this.tool坐标旋转.Text = "坐标旋转";
            this.tool坐标旋转.Visible = false;
            this.tool坐标旋转.Click += new System.EventHandler(this.tool坐标旋转_Click);
            // 
            // btn重新定位
            // 
            this.btn重新定位.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btn重新定位.Image = global::GSP_SJ.Properties.Resources._135crosshair;
            this.btn重新定位.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btn重新定位.Name = "btn重新定位";
            this.btn重新定位.Size = new System.Drawing.Size(30, 25);
            this.btn重新定位.Text = "toolStripButton2";
            this.btn重新定位.Click += new System.EventHandler(this.btn重新定位_Click);
            // 
            // UCZoom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "UCZoom";
            this.Size = new System.Drawing.Size(1581, 964);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.zoomPanel)).EndInit();
            this.zoomPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBox1;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private System.Windows.Forms.ToolStripButton toolSelect;
        private System.Windows.Forms.ToolStripButton toolROI;
        private System.Windows.Forms.ToolStripButton tool坐标旋转;
        private System.Windows.Forms.ToolStripButton btn重新定位;
        private System.Windows.Forms.ToolStripLabel tool_IsFurnace;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel zoomPanel;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnZoom;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
