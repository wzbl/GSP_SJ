namespace GSP.UI
{
    partial class MarkRectify
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MarkRectify));
            this.Header = new ComponentFactory.Krypton.Toolkit.KryptonHeader();
            this.kryptonSplitContainer1 = new ComponentFactory.Krypton.Toolkit.KryptonSplitContainer();
            this.View_pic = new System.Windows.Forms.PictureBox();
            this.Exit_btn = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.OK_btn = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).BeginInit();
            this.kryptonSplitContainer1.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).BeginInit();
            this.kryptonSplitContainer1.Panel2.SuspendLayout();
            this.kryptonSplitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.View_pic)).BeginInit();
            this.SuspendLayout();
            // 
            // Header
            // 
            this.Header.Dock = System.Windows.Forms.DockStyle.Top;
            this.Header.HeaderStyle = ComponentFactory.Krypton.Toolkit.HeaderStyle.DockActive;
            this.Header.Location = new System.Drawing.Point(0, 0);
            this.Header.Name = "Header";
            this.Header.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Header.Size = new System.Drawing.Size(449, 29);
            this.Header.StateNormal.Content.LongText.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.Header.StateNormal.Content.LongText.Color2 = System.Drawing.Color.Green;
            this.Header.StateNormal.Content.LongText.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Header.TabIndex = 0;
            this.Header.Values.Heading = "Mark校正";
            this.Header.Values.Image = ((System.Drawing.Image)(resources.GetObject("Header.Values.Image")));
            // 
            // kryptonSplitContainer1
            // 
            this.kryptonSplitContainer1.Cursor = System.Windows.Forms.Cursors.Default;
            this.kryptonSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonSplitContainer1.IsSplitterFixed = true;
            this.kryptonSplitContainer1.Location = new System.Drawing.Point(0, 29);
            this.kryptonSplitContainer1.Name = "kryptonSplitContainer1";
            this.kryptonSplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // kryptonSplitContainer1.Panel1
            // 
            this.kryptonSplitContainer1.Panel1.Controls.Add(this.View_pic);
            this.kryptonSplitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(2);
            this.kryptonSplitContainer1.Panel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Silver;
            this.kryptonSplitContainer1.Panel1.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.HeaderPrimary;
            // 
            // kryptonSplitContainer1.Panel2
            // 
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.Exit_btn);
            this.kryptonSplitContainer1.Panel2.Controls.Add(this.OK_btn);
            this.kryptonSplitContainer1.Panel2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Silver;
            this.kryptonSplitContainer1.Panel2.PanelBackStyle = ComponentFactory.Krypton.Toolkit.PaletteBackStyle.HeaderPrimary;
            this.kryptonSplitContainer1.SeparatorStyle = ComponentFactory.Krypton.Toolkit.SeparatorStyle.HighProfile;
            this.kryptonSplitContainer1.Size = new System.Drawing.Size(449, 305);
            this.kryptonSplitContainer1.SplitterDistance = 260;
            this.kryptonSplitContainer1.SplitterWidth = 3;
            this.kryptonSplitContainer1.TabIndex = 1;
            // 
            // View_pic
            // 
            this.View_pic.BackColor = System.Drawing.Color.Black;
            this.View_pic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.View_pic.Location = new System.Drawing.Point(2, 2);
            this.View_pic.Margin = new System.Windows.Forms.Padding(0);
            this.View_pic.Name = "View_pic";
            this.View_pic.Size = new System.Drawing.Size(445, 256);
            this.View_pic.TabIndex = 0;
            this.View_pic.TabStop = false;
            // 
            // Exit_btn
            // 
            this.Exit_btn.Location = new System.Drawing.Point(281, 5);
            this.Exit_btn.Name = "Exit_btn";
            this.Exit_btn.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.Exit_btn.Size = new System.Drawing.Size(90, 34);
            this.Exit_btn.TabIndex = 0;
            this.Exit_btn.Values.Image = ((System.Drawing.Image)(resources.GetObject("Exit_btn.Values.Image")));
            this.Exit_btn.Values.Text = "退出";
            this.Exit_btn.Click += new System.EventHandler(this.Exit_btn_Click);
            // 
            // OK_btn
            // 
            this.OK_btn.Location = new System.Drawing.Point(57, 5);
            this.OK_btn.Name = "OK_btn";
            this.OK_btn.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.OK_btn.Size = new System.Drawing.Size(90, 34);
            this.OK_btn.TabIndex = 0;
            this.OK_btn.Values.Image = ((System.Drawing.Image)(resources.GetObject("OK_btn.Values.Image")));
            this.OK_btn.Values.Text = "确定";
            this.OK_btn.Click += new System.EventHandler(this.OK_btn_Click);
            // 
            // MarkRectify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 334);
            this.Controls.Add(this.kryptonSplitContainer1);
            this.Controls.Add(this.Header);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MarkRectify";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MarkRectify";
            this.Load += new System.EventHandler(this.MarkRectify_Load);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel1)).EndInit();
            this.kryptonSplitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1.Panel2)).EndInit();
            this.kryptonSplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonSplitContainer1)).EndInit();
            this.kryptonSplitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.View_pic)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonHeader Header;
        private ComponentFactory.Krypton.Toolkit.KryptonSplitContainer kryptonSplitContainer1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton Exit_btn;
        private ComponentFactory.Krypton.Toolkit.KryptonButton OK_btn;
        private System.Windows.Forms.PictureBox View_pic;
    }
}