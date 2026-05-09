namespace GSP.UI
{
    partial class CalibFrom
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CalibFrom));
            this.Header = new ComponentFactory.Krypton.Toolkit.KryptonHeader();
            this.kryptonSplitContainer1 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
            this.View_pic = new System.Windows.Forms.PictureBox();
            this.View_txt = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.SetSize = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.SetBtn = new ComponentFactory.Krypton.Toolkit.ButtonSpecAny();
            this.kryptonCheckBox2 = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.kryptonCheckBox1 = new ComponentFactory.Krypton.Toolkit.KryptonCheckBox();
            this.Ange_num = new ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown();
            this.Exit_btn = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.Ok_Btn = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.Rectify_Btn2 = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.Rectify_Btn = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.DY_lab = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.Dx_lab = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.Code_txt = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).BeginInit();
            this.kryptonSplitContainer1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).BeginInit();
            this.kryptonSplitContainer1.Panel2.SuspendLayout();
            this.kryptonSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.View_pic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // Header
            // 
            this.Header.Dock = System.Windows.Forms.DockStyle.Top;
            this.Header.HeaderStyle = ComponentFactory.Krypton.Toolkit.HeaderStyle.DockActive;
            this.Header.Location = new System.Drawing.Point(0, 0);
            this.Header.Name = "Header";
            this.Header.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Header.Size = new System.Drawing.Size(788, 29);
            this.Header.TabIndex = 0;
            this.Header.Values.Description = "";
            this.Header.Values.Heading = "单点校正";
            this.Header.Values.Image = ((System.Drawing.Image)(resources.GetObject("Header.Values.Image")));
            // 
            // kryptonSplitContainer1
            // 
            this.kryptonSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.kryptonSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonSplitContainer1.IsSplitterFixed = true;
            this.kryptonSplitContainer1.Location = new System.Drawing.Point(0, 29);
            this.kryptonSplitContainer1.Name = "kryptonSplitContainer1";
            this.kryptonSplitContainer1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            // 
            // kryptonSplitContainer1.Panel1
            // 
            this.kryptonSplitContainer1.Panel1.Controls.Add(this.View_pic);
            this.kryptonSplitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(1);
            // 
            // kryptonSplitContainer1.Panel2
            // 
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.Code_txt);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.View_txt);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.SetSize);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.kryptonCheckBox2);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.kryptonCheckBox1);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.Ange_num);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.Exit_btn);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.Ok_Btn);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.Rectify_Btn2);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.Rectify_Btn);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.pictureBox2);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.DY_lab);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.Dx_lab);
            this.kryptonSplitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(1);
            this.kryptonSplitContainer1.Panel2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonSplitContainer1.Panel2.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.HeaderPrimary;
            this.kryptonSplitContainer1.SeparatorStyle = ComponentFactory.Krypton.Toolkit.SeparatorStyle.HighProfile;
            this.kryptonSplitContainer1.Size = new System.Drawing.Size(788, 427);
            this.kryptonSplitContainer1.SplitterDistance = 399;
            this.kryptonSplitContainer1.TabIndex = 1;
            // 
            // View_pic
            // 
            this.View_pic.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.View_pic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.View_pic.Location = new System.Drawing.Point(1, 1);
            this.View_pic.Margin = new System.Windows.Forms.Padding(0);
            this.View_pic.Name = "View_pic";
            this.View_pic.Padding = new System.Windows.Forms.Padding(1);
            this.View_pic.Size = new System.Drawing.Size(397, 425);
            this.View_pic.TabIndex = 1;
            this.View_pic.TabStop = false;
            // 
            // View_txt
            // 
            this.View_txt.Dock = System.Windows.Forms.DockStyle.Top;
            this.View_txt.Location = new System.Drawing.Point(1, 1);
            this.View_txt.Name = "View_txt";
            this.View_txt.ReadOnly = true;
            this.View_txt.Size = new System.Drawing.Size(382, 23);
            this.View_txt.TabIndex = 182;
            // 
            // SetSize
            // 
            this.SetSize.ButtonSpecs.AddRange(new ComponentFactory.Krypton.Toolkit.ButtonSpecAny[] {
            this.SetBtn});
            this.SetSize.Location = new System.Drawing.Point(6, 239);
            this.SetSize.Name = "SetSize";
            this.SetSize.Size = new System.Drawing.Size(81, 24);
            this.SetSize.TabIndex = 181;
            this.SetSize.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // SetBtn
            // 
            this.SetBtn.Image = ((System.Drawing.Image)(resources.GetObject("SetBtn.Image")));
            this.SetBtn.Style = ComponentFactory.Krypton.Toolkit.PaletteButtonStyle.NavigatorStack;
            this.SetBtn.UniqueName = "22ED88AF9DC548419B8613F160B6760E";
            this.SetBtn.Click += new System.EventHandler(this.SetBtn_Click);
            // 
            // kryptonCheckBox2
            // 
            this.kryptonCheckBox2.Location = new System.Drawing.Point(184, 187);
            this.kryptonCheckBox2.Name = "kryptonCheckBox2";
            this.kryptonCheckBox2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonCheckBox2.Size = new System.Drawing.Size(82, 23);
            this.kryptonCheckBox2.StateNormal.ShortText.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.kryptonCheckBox2.TabIndex = 180;
            this.kryptonCheckBox2.Values.Text = "批量修改";
            // 
            // kryptonCheckBox1
            // 
            this.kryptonCheckBox1.Location = new System.Drawing.Point(184, 161);
            this.kryptonCheckBox1.Name = "kryptonCheckBox1";
            this.kryptonCheckBox1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonCheckBox1.Size = new System.Drawing.Size(82, 23);
            this.kryptonCheckBox1.StateNormal.ShortText.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.kryptonCheckBox1.TabIndex = 180;
            this.kryptonCheckBox1.Values.Text = "修改角度";
            // 
            // Ange_num
            // 
            this.Ange_num.Location = new System.Drawing.Point(183, 120);
            this.Ange_num.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.Ange_num.Name = "Ange_num";
            this.Ange_num.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Ange_num.Size = new System.Drawing.Size(118, 22);
            this.Ange_num.TabIndex = 179;
            // 
            // Exit_btn
            // 
            this.Exit_btn.Location = new System.Drawing.Point(219, 364);
            this.Exit_btn.Name = "Exit_btn";
            this.Exit_btn.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Exit_btn.Size = new System.Drawing.Size(111, 46);
            this.Exit_btn.StateNormal.Content.ShortText.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Exit_btn.TabIndex = 178;
            this.Exit_btn.Values.Image = ((System.Drawing.Image)(resources.GetObject("Exit_btn.Values.Image")));
            this.Exit_btn.Values.Text = "退出";
            this.Exit_btn.Click += new System.EventHandler(this.Exit_btn_Click);
            // 
            // Ok_Btn
            // 
            this.Ok_Btn.Location = new System.Drawing.Point(48, 364);
            this.Ok_Btn.Name = "Ok_Btn";
            this.Ok_Btn.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Ok_Btn.Size = new System.Drawing.Size(111, 46);
            this.Ok_Btn.StateNormal.Content.ShortText.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Ok_Btn.TabIndex = 178;
            this.Ok_Btn.Values.Image = ((System.Drawing.Image)(resources.GetObject("Ok_Btn.Values.Image")));
            this.Ok_Btn.Values.Text = "确定";
            this.Ok_Btn.Click += new System.EventHandler(this.Ok_Btn_Click);
            // 
            // Rectify_Btn2
            // 
            this.Rectify_Btn2.Location = new System.Drawing.Point(219, 280);
            this.Rectify_Btn2.Name = "Rectify_Btn2";
            this.Rectify_Btn2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Rectify_Btn2.Size = new System.Drawing.Size(111, 46);
            this.Rectify_Btn2.StateNormal.Content.ShortText.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Rectify_Btn2.TabIndex = 178;
            this.Rectify_Btn2.Values.Image = ((System.Drawing.Image)(resources.GetObject("Rectify_Btn2.Values.Image")));
            this.Rectify_Btn2.Values.Text = "纠偏";
            this.Rectify_Btn2.Click += new System.EventHandler(this.Rectify_Btn2_Click);
            // 
            // Rectify_Btn
            // 
            this.Rectify_Btn.Location = new System.Drawing.Point(48, 280);
            this.Rectify_Btn.Name = "Rectify_Btn";
            this.Rectify_Btn.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Rectify_Btn.Size = new System.Drawing.Size(111, 46);
            this.Rectify_Btn.StateNormal.Content.ShortText.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Rectify_Btn.TabIndex = 178;
            this.Rectify_Btn.Values.Image = ((System.Drawing.Image)(resources.GetObject("Rectify_Btn.Values.Image")));
            this.Rectify_Btn.Values.Text = "纠正";
            this.Rectify_Btn.Click += new System.EventHandler(this.Rectify_Btn_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(1, 77);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(172, 156);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 176;
            this.pictureBox2.TabStop = false;
            // 
            // DY_lab
            // 
            this.DY_lab.Location = new System.Drawing.Point(184, 33);
            this.DY_lab.Name = "DY_lab";
            this.DY_lab.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.DY_lab.Size = new System.Drawing.Size(96, 26);
            this.DY_lab.StateNormal.LongText.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DY_lab.StateNormal.ShortText.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DY_lab.TabIndex = 1;
            this.DY_lab.Values.ExtraText = "0.000";
            this.DY_lab.Values.Image = ((System.Drawing.Image)(resources.GetObject("DY_lab.Values.Image")));
            this.DY_lab.Values.Text = "Y:";
            // 
            // Dx_lab
            // 
            this.Dx_lab.Location = new System.Drawing.Point(4, 33);
            this.Dx_lab.Name = "Dx_lab";
            this.Dx_lab.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Dx_lab.Size = new System.Drawing.Size(83, 26);
            this.Dx_lab.StateNormal.LongText.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Dx_lab.StateNormal.ShortText.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Dx_lab.TabIndex = 1;
            this.Dx_lab.Values.ExtraText = "454";
            this.Dx_lab.Values.Image = ((System.Drawing.Image)(resources.GetObject("Dx_lab.Values.Image")));
            this.Dx_lab.Values.Text = "X:";
            // 
            // Code_txt
            // 
            this.Code_txt.Location = new System.Drawing.Point(184, 88);
            this.Code_txt.Name = "Code_txt";
            this.Code_txt.ReadOnly = true;
            this.Code_txt.Size = new System.Drawing.Size(117, 26);
            this.Code_txt.StateCommon.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.Code_txt.StateCommon.Content.Color1 = System.Drawing.Color.Green;
            this.Code_txt.StateCommon.Content.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Code_txt.StateNormal.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.Code_txt.TabIndex = 183;
            this.Code_txt.Text = "B17";
            this.Code_txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CalibFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(788, 456);
            this.Controls.Add(this.kryptonSplitContainer1);
            this.Controls.Add(this.Header);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CalibFrom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CalibFrom";
            this.Load += new System.EventHandler(this.CalibFrom_Load);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).EndInit();
            this.kryptonSplitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).EndInit();
            this.kryptonSplitContainer1.Panel2.ResumeLayout(false);
            this.kryptonSplitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).EndInit();
            this.kryptonSplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.View_pic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonHeader Header;
        private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer1;
        private System.Windows.Forms.PictureBox View_pic;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel DY_lab;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel Dx_lab;
        private ComponentFactory.Krypton.Toolkit.KryptonButton Exit_btn;
        private ComponentFactory.Krypton.Toolkit.KryptonButton Ok_Btn;
        private ComponentFactory.Krypton.Toolkit.KryptonButton Rectify_Btn2;
        private ComponentFactory.Krypton.Toolkit.KryptonButton Rectify_Btn;
        private System.Windows.Forms.PictureBox pictureBox2;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox kryptonCheckBox2;
        private ComponentFactory.Krypton.Toolkit.KryptonCheckBox kryptonCheckBox1;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown Ange_num;
        private ComponentFactory.Krypton.Toolkit.KryptonNumericUpDown SetSize;
        private ComponentFactory.Krypton.Toolkit.ButtonSpecAny SetBtn;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox View_txt;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox Code_txt;
    }
}