namespace GSP.UI
{
    partial class LaserFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LaserFrm));
            this.Header = new ComponentFactory.Krypton.Toolkit.KryptonHeader();
            this.kryptonSplitContainer1 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
            this.LaserValue_Lab = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.Result_lab = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.Apply_btn = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.Offset_txt = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.LaserValue_txt = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.Allowablevalue_txt = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel4 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.Exit_btn = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.Ok_Btn = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.Laser_lab = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.Code_txt = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).BeginInit();
            this.kryptonSplitContainer1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).BeginInit();
            this.kryptonSplitContainer1.Panel2.SuspendLayout();
            this.kryptonSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Header
            // 
            this.Header.Dock = System.Windows.Forms.DockStyle.Top;
            this.Header.HeaderStyle = ComponentFactory.Krypton.Toolkit.HeaderStyle.DockInactive;
            this.Header.Location = new System.Drawing.Point(0, 0);
            this.Header.Name = "Header";
            this.Header.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Header.Size = new System.Drawing.Size(522, 29);
            this.Header.TabIndex = 0;
            this.Header.Values.Description = "";
            this.Header.Values.Heading = "激光校正";
            this.Header.Values.Image = ((System.Drawing.Image)(resources.GetObject("Header.Values.Image")));
            // 
            // kryptonSplitContainer1
            // 
            this.kryptonSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.kryptonSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonSplitContainer1.Location = new System.Drawing.Point(0, 29);
            this.kryptonSplitContainer1.Name = "kryptonSplitContainer1";
            this.kryptonSplitContainer1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            // 
            // kryptonSplitContainer1.Panel1
            // 
            this.kryptonSplitContainer1.Panel1.Controls.Add(this.LaserValue_Lab);
            this.kryptonSplitContainer1.Panel1.Controls.Add(this.pictureBox1);
            this.kryptonSplitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(1);
            this.kryptonSplitContainer1.Panel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonSplitContainer1.Panel1.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.HeaderPrimary;
            // 
            // kryptonSplitContainer1.Panel2
            // 
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.Code_txt);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.Result_lab);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.Apply_btn);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.Offset_txt);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.LaserValue_txt);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.Allowablevalue_txt);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.kryptonLabel4);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.kryptonLabel3);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.kryptonLabel2);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.Exit_btn);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.Ok_Btn);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.Laser_lab);
            this.kryptonSplitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(1);
            this.kryptonSplitContainer1.Panel2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonSplitContainer1.Panel2.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.HeaderPrimary;
            this.kryptonSplitContainer1.SeparatorStyle = ComponentFactory.Krypton.Toolkit.SeparatorStyle.HighProfile;
            this.kryptonSplitContainer1.Size = new System.Drawing.Size(522, 256);
            this.kryptonSplitContainer1.SplitterDistance = 259;
            this.kryptonSplitContainer1.TabIndex = 1;
            // 
            // LaserValue_Lab
            // 
            this.LaserValue_Lab.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.BoldControl;
            this.LaserValue_Lab.Location = new System.Drawing.Point(4, 97);
            this.LaserValue_Lab.Name = "LaserValue_Lab";
            this.LaserValue_Lab.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.LaserValue_Lab.Size = new System.Drawing.Size(104, 26);
            this.LaserValue_Lab.StateNormal.ShortText.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LaserValue_Lab.TabIndex = 10;
            this.LaserValue_Lab.Values.Image = ((System.Drawing.Image)(resources.GetObject("LaserValue_Lab.Values.Image")));
            this.LaserValue_Lab.Values.Text = "0.000mm";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(1, 1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(257, 254);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // Result_lab
            // 
            this.Result_lab.Location = new System.Drawing.Point(99, 172);
            this.Result_lab.Name = "Result_lab";
            this.Result_lab.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Result_lab.Size = new System.Drawing.Size(61, 26);
            this.Result_lab.StateNormal.ShortText.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.Result_lab.StateNormal.ShortText.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Result_lab.TabIndex = 10;
            this.Result_lab.Values.Image = ((System.Drawing.Image)(resources.GetObject("Result_lab.Values.Image")));
            this.Result_lab.Values.Text = "OK";
            // 
            // Apply_btn
            // 
            this.Apply_btn.Location = new System.Drawing.Point(191, 89);
            this.Apply_btn.Name = "Apply_btn";
            this.Apply_btn.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Apply_btn.Size = new System.Drawing.Size(62, 33);
            this.Apply_btn.TabIndex = 9;
            this.Apply_btn.Values.Image = ((System.Drawing.Image)(resources.GetObject("Apply_btn.Values.Image")));
            this.Apply_btn.Values.Text = "";
            this.Apply_btn.Click += new System.EventHandler(this.Apply_btn_Click);
            // 
            // Offset_txt
            // 
            this.Offset_txt.Location = new System.Drawing.Point(101, 93);
            this.Offset_txt.Name = "Offset_txt";
            this.Offset_txt.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Offset_txt.ReadOnly = true;
            this.Offset_txt.Size = new System.Drawing.Size(80, 26);
            this.Offset_txt.StateCommon.Content.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.Offset_txt.StateCommon.Content.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Offset_txt.TabIndex = 8;
            this.Offset_txt.Text = "0.00";
            // 
            // LaserValue_txt
            // 
            this.LaserValue_txt.Location = new System.Drawing.Point(101, 64);
            this.LaserValue_txt.Name = "LaserValue_txt";
            this.LaserValue_txt.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.LaserValue_txt.ReadOnly = true;
            this.LaserValue_txt.Size = new System.Drawing.Size(80, 26);
            this.LaserValue_txt.StateCommon.Content.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.LaserValue_txt.StateCommon.Content.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LaserValue_txt.TabIndex = 8;
            this.LaserValue_txt.Text = "0.00";
            // 
            // Allowablevalue_txt
            // 
            this.Allowablevalue_txt.Location = new System.Drawing.Point(101, 125);
            this.Allowablevalue_txt.Name = "Allowablevalue_txt";
            this.Allowablevalue_txt.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Allowablevalue_txt.ReadOnly = true;
            this.Allowablevalue_txt.Size = new System.Drawing.Size(80, 26);
            this.Allowablevalue_txt.StateCommon.Content.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.Allowablevalue_txt.StateCommon.Content.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Allowablevalue_txt.TabIndex = 8;
            this.Allowablevalue_txt.Text = "0.00";
            // 
            // kryptonLabel4
            // 
            this.kryptonLabel4.Location = new System.Drawing.Point(8, 97);
            this.kryptonLabel4.Name = "kryptonLabel4";
            this.kryptonLabel4.Size = new System.Drawing.Size(77, 26);
            this.kryptonLabel4.TabIndex = 7;
            this.kryptonLabel4.Values.Image = ((System.Drawing.Image)(resources.GetObject("kryptonLabel4.Values.Image")));
            this.kryptonLabel4.Values.Text = "偏差值:";
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(8, 129);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.Size = new System.Drawing.Size(87, 26);
            this.kryptonLabel3.TabIndex = 7;
            this.kryptonLabel3.Values.Image = ((System.Drawing.Image)(resources.GetObject("kryptonLabel3.Values.Image")));
            this.kryptonLabel3.Values.Text = "允许偏差";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(8, 65);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(77, 26);
            this.kryptonLabel2.TabIndex = 7;
            this.kryptonLabel2.UseMnemonic = false;
            this.kryptonLabel2.Values.Image = ((System.Drawing.Image)(resources.GetObject("kryptonLabel2.Values.Image")));
            this.kryptonLabel2.Values.Text = "基准值:";
            // 
            // Exit_btn
            // 
            this.Exit_btn.Location = new System.Drawing.Point(156, 217);
            this.Exit_btn.Name = "Exit_btn";
            this.Exit_btn.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Exit_btn.Size = new System.Drawing.Size(90, 34);
            this.Exit_btn.TabIndex = 5;
            this.Exit_btn.Values.Image = ((System.Drawing.Image)(resources.GetObject("Exit_btn.Values.Image")));
            this.Exit_btn.Values.Text = "退出";
            this.Exit_btn.Click += new System.EventHandler(this.Exit_btn_Click);
            // 
            // Ok_Btn
            // 
            this.Ok_Btn.Location = new System.Drawing.Point(14, 217);
            this.Ok_Btn.Name = "Ok_Btn";
            this.Ok_Btn.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Ok_Btn.Size = new System.Drawing.Size(90, 34);
            this.Ok_Btn.TabIndex = 6;
            this.Ok_Btn.Values.Image = ((System.Drawing.Image)(resources.GetObject("Ok_Btn.Values.Image")));
            this.Ok_Btn.Values.Text = "确定";
            this.Ok_Btn.Click += new System.EventHandler(this.Ok_Btn_Click);
            // 
            // Laser_lab
            // 
            this.Laser_lab.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.BoldControl;
            this.Laser_lab.Location = new System.Drawing.Point(5, 33);
            this.Laser_lab.Name = "Laser_lab";
            this.Laser_lab.Size = new System.Drawing.Size(90, 26);
            this.Laser_lab.StateNormal.ShortText.Color1 = System.Drawing.Color.Green;
            this.Laser_lab.TabIndex = 0;
            this.Laser_lab.Values.Image = ((System.Drawing.Image)(resources.GetObject("Laser_lab.Values.Image")));
            this.Laser_lab.Values.Text = "0.000mm";
            // 
            // Code_txt
            // 
            this.Code_txt.Dock = System.Windows.Forms.DockStyle.Top;
            this.Code_txt.Location = new System.Drawing.Point(1, 1);
            this.Code_txt.Name = "Code_txt";
            this.Code_txt.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Code_txt.ReadOnly = true;
            this.Code_txt.Size = new System.Drawing.Size(256, 26);
            this.Code_txt.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.Code_txt.StateCommon.Content.Color1 = System.Drawing.Color.Green;
            this.Code_txt.StateCommon.Content.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Code_txt.StateNormal.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.Code_txt.TabIndex = 47;
            this.Code_txt.Text = "B17";
            this.Code_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // LaserFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 285);
            this.Controls.Add(this.kryptonSplitContainer1);
            this.Controls.Add(this.Header);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LaserFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LaserFrm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.LaserFrm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).EndInit();
            this.kryptonSplitContainer1.Panel1.ResumeLayout(false);
            this.kryptonSplitContainer1.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).EndInit();
            this.kryptonSplitContainer1.Panel2.ResumeLayout(false);
            this.kryptonSplitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).EndInit();
            this.kryptonSplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonHeader Header;
        private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel Laser_lab;
        private ComponentFactory.Krypton.Toolkit.KryptonButton Exit_btn;
        private ComponentFactory.Krypton.Toolkit.KryptonButton Ok_Btn;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel4;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonButton Apply_btn;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox Allowablevalue_txt;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox Offset_txt;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox LaserValue_txt;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel LaserValue_Lab;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel Result_lab;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox Code_txt;
    }
}