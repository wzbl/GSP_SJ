namespace GSP.Light
{
    partial class LightControl
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
            this.R_TrackBar = new ComponentFactory.Krypton.Toolkit.KryptonTrackBar();
            this.G_TrackBar = new ComponentFactory.Krypton.Toolkit.KryptonTrackBar();
            this.B_TrackBar = new ComponentFactory.Krypton.Toolkit.KryptonTrackBar();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.R_num = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.G_num = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.B_num = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // R_TrackBar
            // 
            this.R_TrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.R_TrackBar.DrawBackground = true;
            this.R_TrackBar.LargeChange = 1;
            this.R_TrackBar.Location = new System.Drawing.Point(3, 3);
            this.R_TrackBar.Maximum = 255;
            this.R_TrackBar.Name = "R_TrackBar";
            this.R_TrackBar.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.R_TrackBar.Size = new System.Drawing.Size(231, 27);
            this.R_TrackBar.StateNormal.Position.Color4 = System.Drawing.Color.Red;
            this.R_TrackBar.StateNormal.Position.Color5 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.R_TrackBar.TabIndex = 0;
            this.R_TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.R_TrackBar.TrackBarSize = ComponentFactory.Krypton.Toolkit.PaletteTrackBarSize.Large;
            this.R_TrackBar.ValueChanged += new System.EventHandler(this.R_TrackBar_ValueChanged);
            // 
            // G_TrackBar
            // 
            this.G_TrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.G_TrackBar.DrawBackground = true;
            this.G_TrackBar.LargeChange = 1;
            this.G_TrackBar.Location = new System.Drawing.Point(3, 36);
            this.G_TrackBar.Maximum = 255;
            this.G_TrackBar.Name = "G_TrackBar";
            this.G_TrackBar.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.G_TrackBar.Size = new System.Drawing.Size(231, 26);
            this.G_TrackBar.StateNormal.Position.Color4 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.G_TrackBar.StateNormal.Position.Color5 = System.Drawing.Color.Green;
            this.G_TrackBar.TabIndex = 0;
            this.G_TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.G_TrackBar.TrackBarSize = ComponentFactory.Krypton.Toolkit.PaletteTrackBarSize.Large;
            this.G_TrackBar.ValueChanged += new System.EventHandler(this.G_TrackBar_ValueChanged);
            // 
            // B_TrackBar
            // 
            this.B_TrackBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.B_TrackBar.DrawBackground = true;
            this.B_TrackBar.LargeChange = 1;
            this.B_TrackBar.Location = new System.Drawing.Point(3, 68);
            this.B_TrackBar.Maximum = 255;
            this.B_TrackBar.Name = "B_TrackBar";
            this.B_TrackBar.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.B_TrackBar.Size = new System.Drawing.Size(231, 28);
            this.B_TrackBar.StateNormal.Position.Color4 = System.Drawing.Color.DodgerBlue;
            this.B_TrackBar.StateNormal.Position.Color5 = System.Drawing.Color.RoyalBlue;
            this.B_TrackBar.TabIndex = 0;
            this.B_TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.B_TrackBar.TrackBarSize = ComponentFactory.Krypton.Toolkit.PaletteTrackBarSize.Large;
            this.B_TrackBar.ValueChanged += new System.EventHandler(this.B_TrackBar_ValueChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 79.2517F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20.7483F));
            this.tableLayoutPanel1.Controls.Add(this.R_TrackBar, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.B_TrackBar, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.G_TrackBar, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.R_num, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.G_num, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.B_num, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.72464F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.27536F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(300, 99);
            this.tableLayoutPanel1.TabIndex = 1;
            this.tableLayoutPanel1.MouseHover += new System.EventHandler(this.tableLayoutPanel1_MouseHover);
            // 
            // R_num
            // 
            this.R_num.Dock = System.Windows.Forms.DockStyle.Fill;
            this.R_num.Location = new System.Drawing.Point(240, 3);
            this.R_num.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.R_num.Name = "R_num";
            this.R_num.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.R_num.Size = new System.Drawing.Size(57, 27);
            this.R_num.StateCommon.Content.Color1 = System.Drawing.Color.Red;
            this.R_num.StateCommon.Content.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.R_num.TabIndex = 1;
            this.R_num.ValueChanged += new System.EventHandler(this.R_num_ValueChanged);
            // 
            // G_num
            // 
            this.G_num.Dock = System.Windows.Forms.DockStyle.Fill;
            this.G_num.Location = new System.Drawing.Point(240, 36);
            this.G_num.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.G_num.Name = "G_num";
            this.G_num.Size = new System.Drawing.Size(57, 26);
            this.G_num.StateCommon.Content.Color1 = System.Drawing.Color.Green;
            this.G_num.StateCommon.Content.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.G_num.TabIndex = 1;
            this.G_num.ValueChanged += new System.EventHandler(this.G_num_ValueChanged);
            // 
            // B_num
            // 
            this.B_num.Dock = System.Windows.Forms.DockStyle.Fill;
            this.B_num.Location = new System.Drawing.Point(240, 68);
            this.B_num.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.B_num.Name = "B_num";
            this.B_num.Size = new System.Drawing.Size(57, 28);
            this.B_num.StateCommon.Content.Color1 = System.Drawing.Color.DodgerBlue;
            this.B_num.StateCommon.Content.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.B_num.TabIndex = 1;
            this.B_num.ValueChanged += new System.EventHandler(this.B_num_ValueChanged);
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.tableLayoutPanel1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonPanel1.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.HeaderPrimary;
            this.kryptonPanel1.Size = new System.Drawing.Size(300, 99);
            this.kryptonPanel1.TabIndex = 2;
            // 
            // LightControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.kryptonPanel1);
            this.Name = "LightControl";
            this.Size = new System.Drawing.Size(300, 99);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonTrackBar R_TrackBar;
        private ComponentFactory.Krypton.Toolkit.KryptonTrackBar G_TrackBar;
        private ComponentFactory.Krypton.Toolkit.KryptonTrackBar B_TrackBar;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown R_num;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown G_num;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown B_num;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
    }
}
