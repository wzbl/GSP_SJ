namespace GSP
{
    partial class StartForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StartForm));
            this.SysInit_Listbox = new ComponentFactory.Krypton.Toolkit.KryptonListBox();
            this.lblPercent = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonSplitContainer1 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
            this.Log_pic = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.IniHeader = new ComponentFactory.Krypton.Toolkit.KryptonHeader();
            this.Continue = new ComponentFactory.Krypton.Toolkit.ButtonSpecAny();
            this.Close_btn = new ComponentFactory.Krypton.Toolkit.ButtonSpecAny();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).BeginInit();
            this.kryptonSplitContainer1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).BeginInit();
            this.kryptonSplitContainer1.Panel2.SuspendLayout();
            this.kryptonSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Log_pic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // SysInit_Listbox
            // 
            this.SysInit_Listbox.BackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.SeparatorHighProfile;
            this.SysInit_Listbox.BorderStyle = ComponentFactory.Krypton.Toolkit.PaletteBorderStyle.SeparatorHighProfile;
            this.SysInit_Listbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SysInit_Listbox.Location = new System.Drawing.Point(172, 0);
            this.SysInit_Listbox.Margin = new System.Windows.Forms.Padding(1);
            this.SysInit_Listbox.Name = "SysInit_Listbox";
            this.SysInit_Listbox.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.SysInit_Listbox.Size = new System.Drawing.Size(314, 209);
            this.SysInit_Listbox.TabIndex = 1;
            // 
            // lblPercent
            // 
            this.lblPercent.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.TitlePanel;
            this.lblPercent.Location = new System.Drawing.Point(117, 141);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.lblPercent.Size = new System.Drawing.Size(50, 29);
            this.lblPercent.StateNormal.ShortText.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblPercent.StateNormal.ShortText.Color2 = System.Drawing.Color.Green;
            this.lblPercent.TabIndex = 3;
            this.lblPercent.Values.Text = "50%";
            // 
            // kryptonSplitContainer1
            // 
            this.kryptonSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.kryptonSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonSplitContainer1.IsSplitterFixed = true;
            this.kryptonSplitContainer1.Location = new System.Drawing.Point(0, 35);
            this.kryptonSplitContainer1.Margin = new System.Windows.Forms.Padding(1);
            this.kryptonSplitContainer1.Name = "kryptonSplitContainer1";
            this.kryptonSplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.kryptonSplitContainer1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            // 
            // kryptonSplitContainer1.Panel1
            // 
            this.kryptonSplitContainer1.Panel1.Controls.Add(this.Log_pic);
            this.kryptonSplitContainer1.Panel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonSplitContainer1.Panel1.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.GridHeaderColumnCustom1;
            // 
            // kryptonSplitContainer1.Panel2
            // 
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.SysInit_Listbox);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.lblPercent);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.pictureBox1);
            this.kryptonSplitContainer1.Panel2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonSplitContainer1.Panel2.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.SeparatorHighProfile;
            this.kryptonSplitContainer1.SeparatorStyle = ComponentFactory.Krypton.Toolkit.SeparatorStyle.HighProfile;
            this.kryptonSplitContainer1.Size = new System.Drawing.Size(486, 345);
            this.kryptonSplitContainer1.SplitterDistance = 131;
            this.kryptonSplitContainer1.TabIndex = 5;
            // 
            // Log_pic
            // 
            this.Log_pic.BackColor = System.Drawing.Color.Transparent;
            this.Log_pic.Dock = System.Windows.Forms.DockStyle.Left;
            this.Log_pic.Location = new System.Drawing.Point(0, 0);
            this.Log_pic.Name = "Log_pic";
            this.Log_pic.Size = new System.Drawing.Size(172, 131);
            this.Log_pic.TabIndex = 0;
            this.Log_pic.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(172, 209);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // IniHeader
            // 
            this.IniHeader.AutoSize = false;
            this.IniHeader.ButtonSpecs.AddRange(new ComponentFactory.Krypton.Toolkit.ButtonSpecAny[] {
            this.Continue,
            this.Close_btn});
            this.IniHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.IniHeader.HeaderStyle = ComponentFactory.Krypton.Toolkit.HeaderStyle.Form;
            this.IniHeader.Location = new System.Drawing.Point(0, 0);
            this.IniHeader.Name = "IniHeader";
            this.IniHeader.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.IniHeader.Size = new System.Drawing.Size(486, 35);
            this.IniHeader.TabIndex = 6;
            this.IniHeader.Values.Description = "版本号：";
            this.IniHeader.Values.Heading = "首件检测机系统初始化";
            this.IniHeader.Values.Image = global::GSP_SJ.Properties.Resources.Edit_24px;
            // 
            // Continue
            // 
            this.Continue.Image = GSP_SJ.Properties.Resources._002_61;
            this.Continue.Text = "启动";
            this.Continue.UniqueName = "ECE53561E17A4C199EB20E8F152EFC9A";
            // 
            // Close_btn
            // 
            this.Close_btn.Checked = ComponentFactory.Krypton.Toolkit.ButtonCheckState.Unchecked;
            this.Close_btn.Image = GSP_SJ.Properties.Resources._114;
            this.Close_btn.Text = "退出";
            this.Close_btn.UniqueName = "EF6D0B9D4C6A4ECE5FB5AAA5327E59B6";
            this.Close_btn.Click += new System.EventHandler(this.Close_btn_Click);
            // 
            // StartForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 380);
            this.Controls.Add(this.kryptonSplitContainer1);
            this.Controls.Add(this.IniHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.HeaderStyle = ComponentFactory.Krypton.Toolkit.HeaderStyle.Primary;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "StartForm";
            this.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2010Blue;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "StartForm";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).EndInit();
            this.kryptonSplitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).EndInit();
            this.kryptonSplitContainer1.Panel2.ResumeLayout(false);
            this.kryptonSplitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).EndInit();
            this.kryptonSplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Log_pic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private ComponentFactory.Krypton.Toolkit.KryptonListBox SysInit_Listbox;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel lblPercent;
        private System.Windows.Forms.PictureBox pictureBox1;
        private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer1;
        private ComponentFactory.Krypton.Toolkit.KryptonHeader IniHeader;
        private ComponentFactory.Krypton.Toolkit.ButtonSpecAny Close_btn;
        private System.Windows.Forms.PictureBox Log_pic;
        private ComponentFactory.Krypton.Toolkit.ButtonSpecAny Continue;
    }
}