namespace GSP.UI
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.Header = new ComponentFactory.Krypton.Toolkit.KryptonHeader();
            this.Vision_lab = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.Close_btn = new ComponentFactory.Krypton.Toolkit.ButtonSpecAny();
            this.Mode_lab = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Header
            // 
            this.Header.ButtonSpecs.AddRange(new ComponentFactory.Krypton.Toolkit.ButtonSpecAny[] {
            this.Close_btn});
            this.Header.Dock = System.Windows.Forms.DockStyle.Top;
            this.Header.HeaderStyle = ComponentFactory.Krypton.Toolkit.HeaderStyle.Form;
            this.Header.Location = new System.Drawing.Point(0, 0);
            this.Header.Name = "Header";
            this.Header.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Header.Size = new System.Drawing.Size(384, 28);
            this.Header.TabIndex = 0;
            this.Header.Values.Description = "";
            this.Header.Values.Heading = "关于";
            this.Header.Values.Image = ((System.Drawing.Image)(resources.GetObject("Header.Values.Image")));
            // 
            // Vision_lab
            // 
            this.Vision_lab.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.BoldControl;
            this.Vision_lab.Location = new System.Drawing.Point(3, 10);
            this.Vision_lab.Name = "Vision_lab";
            this.Vision_lab.Size = new System.Drawing.Size(149, 26);
            this.Vision_lab.TabIndex = 1;
            this.Vision_lab.Values.ExtraText = "v2.0.001_rc.1";
            this.Vision_lab.Values.Image = ((System.Drawing.Image)(resources.GetObject("Vision_lab.Values.Image")));
            this.Vision_lab.Values.Text = "版本：";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.BoldControl;
            this.kryptonLabel2.Location = new System.Drawing.Point(3, 43);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.Size = new System.Drawing.Size(131, 26);
            this.kryptonLabel2.TabIndex = 1;
            this.kryptonLabel2.Values.ExtraText = "永久授权";
            this.kryptonLabel2.Values.Image = ((System.Drawing.Image)(resources.GetObject("kryptonLabel2.Values.Image")));
            this.kryptonLabel2.Values.Text = "授权：";
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.Mode_lab);
            this.kryptonPanel1.Controls.Add(this.Vision_lab);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 28);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonPanel1.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.HeaderPrimary;
            this.kryptonPanel1.Size = new System.Drawing.Size(384, 226);
            this.kryptonPanel1.TabIndex = 2;
            // 
            // Close_btn
            // 
            this.Close_btn.Checked = ComponentFactory.Krypton.Toolkit.ButtonCheckState.Unchecked;
            this.Close_btn.Image = ((System.Drawing.Image)(resources.GetObject("Close_btn.Image")));
            this.Close_btn.UniqueName = "60699112E95D400B39A1203F42ACDA1C";
            this.Close_btn.Click += new System.EventHandler(this.Close_btn_Click);
            // 
            // Mode_lab
            // 
            this.Mode_lab.LabelStyle = ComponentFactory.Krypton.Toolkit.LabelStyle.BoldControl;
            this.Mode_lab.Location = new System.Drawing.Point(3, 80);
            this.Mode_lab.Name = "Mode_lab";
            this.Mode_lab.Size = new System.Drawing.Size(65, 26);
            this.Mode_lab.TabIndex = 2;
            this.Mode_lab.Values.Image = ((System.Drawing.Image)(resources.GetObject("kryptonLabel1.Values.Image")));
            this.Mode_lab.Values.Text = "机型:";
            // 
            // AboutForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 254);
            this.Controls.Add(this.kryptonPanel1);
            this.Controls.Add(this.Header);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AboutForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AboutForm";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonHeader Header;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel Vision_lab;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.ButtonSpecAny Close_btn;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel Mode_lab;
    }
}