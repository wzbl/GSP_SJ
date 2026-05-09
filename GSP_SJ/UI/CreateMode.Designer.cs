namespace GSP.UI
{
    partial class CreateMode
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateMode));
            this.Header = new ComponentFactory.Krypton.Toolkit.KryptonHeader();
            this.CloseBtn = new ComponentFactory.Krypton.Toolkit.ButtonSpecAny();
            this.SplitPanel = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
            this.Proc_pic = new System.Windows.Forms.PictureBox();
            this.Rectify_Btn = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.SplitPanel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SplitPanel.Panel1)).BeginInit();
            this.SplitPanel.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SplitPanel.Panel2)).BeginInit();
            this.SplitPanel.Panel2.SuspendLayout();
            this.SplitPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Proc_pic)).BeginInit();
            this.SuspendLayout();
            // 
            // Header
            // 
            this.Header.ButtonSpecs.AddRange(new ComponentFactory.Krypton.Toolkit.ButtonSpecAny[] {
            this.CloseBtn});
            this.Header.Dock = System.Windows.Forms.DockStyle.Top;
            this.Header.Location = new System.Drawing.Point(0, 0);
            this.Header.Name = "Header";
            this.Header.Size = new System.Drawing.Size(744, 34);
            this.Header.StateNormal.Back.GraphicsHint = ComponentFactory.Krypton.Toolkit.PaletteGraphicsHint.AntiAlias;
            this.Header.StateNormal.Border.DrawBorders = ((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders)((((ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Top | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Bottom) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Left) 
            | ComponentFactory.Krypton.Toolkit.PaletteDrawBorders.Right)));
            this.Header.StateNormal.Border.GraphicsHint = ComponentFactory.Krypton.Toolkit.PaletteGraphicsHint.AntiAlias;
            this.Header.StateNormal.Border.Rounding = 2;
            this.Header.TabIndex = 1;
            this.Header.Values.Description = "";
            this.Header.Values.Heading = "学习模版";
            this.Header.Values.Image = ((System.Drawing.Image)(resources.GetObject("Header.Values.Image")));
            // 
            // CloseBtn
            // 
            this.CloseBtn.Image = ((System.Drawing.Image)(resources.GetObject("CloseBtn.Image")));
            this.CloseBtn.UniqueName = "80D6C93B35F8495ABEB76483D5D8919B";
            this.CloseBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // SplitPanel
            // 
            this.SplitPanel.ContainerBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.HeaderForm;
            this.SplitPanel.Cursor = System.Windows.Forms.Cursors.Default;
            this.SplitPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SplitPanel.IsSplitterFixed = true;
            this.SplitPanel.Location = new System.Drawing.Point(0, 34);
            this.SplitPanel.Margin = new System.Windows.Forms.Padding(0);
            this.SplitPanel.Name = "SplitPanel";
            this.SplitPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.SplitPanel.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            // 
            // SplitPanel.Panel1
            // 
            this.SplitPanel.Panel1.Controls.Add(this.Proc_pic);
            this.SplitPanel.Panel1.Padding = new System.Windows.Forms.Padding(2, 0, 2, 2);
            this.SplitPanel.Panel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.SplitPanel.Panel1.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.TabHighProfile;
            // 
            // SplitPanel.Panel2
            // 
            this.SplitPanel.Panel2.Controls.Add(this.Rectify_Btn);
            this.SplitPanel.Panel2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.SplitPanel.Panel2.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.TabHighProfile;
            this.SplitPanel.Panel2MinSize = 5;
            this.SplitPanel.SeparatorStyle = ComponentFactory.Krypton.Toolkit.SeparatorStyle.HighProfile;
            this.SplitPanel.Size = new System.Drawing.Size(744, 409);
            this.SplitPanel.SplitterDistance = 364;
            this.SplitPanel.SplitterWidth = 3;
            this.SplitPanel.TabIndex = 2;
            // 
            // Proc_pic
            // 
            this.Proc_pic.BackColor = System.Drawing.Color.Black;
            this.Proc_pic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Proc_pic.Location = new System.Drawing.Point(2, 0);
            this.Proc_pic.Name = "Proc_pic";
            this.Proc_pic.Size = new System.Drawing.Size(740, 362);
            this.Proc_pic.TabIndex = 0;
            this.Proc_pic.TabStop = false;
            // 
            // Rectify_Btn
            // 
            this.Rectify_Btn.Location = new System.Drawing.Point(349, 5);
            this.Rectify_Btn.Name = "Rectify_Btn";
            this.Rectify_Btn.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Rectify_Btn.Size = new System.Drawing.Size(79, 32);
            this.Rectify_Btn.StateNormal.Content.ShortText.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Rectify_Btn.TabIndex = 179;
            this.Rectify_Btn.Values.Image = ((System.Drawing.Image)(resources.GetObject("Rectify_Btn.Values.Image")));
            this.Rectify_Btn.Values.Text = "纠正";
            this.Rectify_Btn.Click += new System.EventHandler(this.Rectify_Btn_Click);
            // 
            // CreateMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 443);
            this.Controls.Add(this.SplitPanel);
            this.Controls.Add(this.Header);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CreateMode";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CreateMode";
            ((System.ComponentModel.ISupportInitialize)(this.SplitPanel.Panel1)).EndInit();
            this.SplitPanel.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitPanel.Panel2)).EndInit();
            this.SplitPanel.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SplitPanel)).EndInit();
            this.SplitPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Proc_pic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ComponentFactory.Krypton.Toolkit.KryptonHeader Header;
        private ComponentFactory.Krypton.Toolkit.ButtonSpecAny CloseBtn;
        private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer SplitPanel;
        private System.Windows.Forms.PictureBox Proc_pic;
        private ComponentFactory.Krypton.Toolkit.KryptonButton Rectify_Btn;
    }
}