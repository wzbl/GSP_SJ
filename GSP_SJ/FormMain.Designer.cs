namespace GSP_SJ
{
    partial class FormMain
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
            this.kryptonNavigator1 = new ComponentFactory.Krypton.Navigator.KryptonNavigator();
            this.kryptonPage1 = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.kryptonPage2 = new ComponentFactory.Krypton.Navigator.KryptonPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.资料库ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.资料库ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.尺寸管理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.电桥配置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.模板库ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.工具ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.系统ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.分析规则ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.系统参数ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.电桥参数ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonNavigator1)).BeginInit();
            this.kryptonNavigator1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage2)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonNavigator1
            // 
            this.kryptonNavigator1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonNavigator1.Location = new System.Drawing.Point(0, 0);
            this.kryptonNavigator1.Name = "kryptonNavigator1";
            this.kryptonNavigator1.Pages.AddRange(new ComponentFactory.Krypton.Navigator.KryptonPage[] {
            this.kryptonPage1,
            this.kryptonPage2});
            this.kryptonNavigator1.SelectedIndex = 0;
            this.kryptonNavigator1.Size = new System.Drawing.Size(1188, 648);
            this.kryptonNavigator1.TabIndex = 0;
            this.kryptonNavigator1.Text = "kryptonNavigator1";
            // 
            // kryptonPage1
            // 
            this.kryptonPage1.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.kryptonPage1.Flags = 65534;
            this.kryptonPage1.LastVisibleSet = true;
            this.kryptonPage1.MinimumSize = new System.Drawing.Size(50, 50);
            this.kryptonPage1.Name = "kryptonPage1";
            this.kryptonPage1.Size = new System.Drawing.Size(1186, 615);
            this.kryptonPage1.Text = "所有程序";
            this.kryptonPage1.ToolTipTitle = "Page ToolTip";
            this.kryptonPage1.UniqueName = "87FCC65972D0445785B338128E8405C7";
            // 
            // kryptonPage2
            // 
            this.kryptonPage2.AutoHiddenSlideSize = new System.Drawing.Size(200, 200);
            this.kryptonPage2.Flags = 65534;
            this.kryptonPage2.LastVisibleSet = true;
            this.kryptonPage2.MinimumSize = new System.Drawing.Size(50, 50);
            this.kryptonPage2.Name = "kryptonPage2";
            this.kryptonPage2.Size = new System.Drawing.Size(1186, 615);
            this.kryptonPage2.Text = "所有报告";
            this.kryptonPage2.ToolTipTitle = "Page ToolTip";
            this.kryptonPage2.UniqueName = "B8961C87036B4E3CB89243E05A1C27C0";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9.216F);
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(21, 21);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.资料库ToolStripMenuItem,
            this.工具ToolStripMenuItem,
            this.系统ToolStripMenuItem,
            this.帮助ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1188, 29);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 资料库ToolStripMenuItem
            // 
            this.资料库ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.资料库ToolStripMenuItem1,
            this.尺寸管理ToolStripMenuItem,
            this.电桥配置ToolStripMenuItem,
            this.模板库ToolStripMenuItem});
            this.资料库ToolStripMenuItem.Name = "资料库ToolStripMenuItem";
            this.资料库ToolStripMenuItem.Size = new System.Drawing.Size(58, 25);
            this.资料库ToolStripMenuItem.Text = "功能";
            // 
            // 资料库ToolStripMenuItem1
            // 
            this.资料库ToolStripMenuItem1.Name = "资料库ToolStripMenuItem1";
            this.资料库ToolStripMenuItem1.Size = new System.Drawing.Size(165, 30);
            this.资料库ToolStripMenuItem1.Text = "资料库";
            this.资料库ToolStripMenuItem1.Click += new System.EventHandler(this.资料库ToolStripMenuItem1_Click);
            // 
            // 尺寸管理ToolStripMenuItem
            // 
            this.尺寸管理ToolStripMenuItem.Name = "尺寸管理ToolStripMenuItem";
            this.尺寸管理ToolStripMenuItem.Size = new System.Drawing.Size(165, 30);
            this.尺寸管理ToolStripMenuItem.Text = "尺寸管理";
            this.尺寸管理ToolStripMenuItem.Click += new System.EventHandler(this.尺寸管理ToolStripMenuItem_Click);
            // 
            // 电桥配置ToolStripMenuItem
            // 
            this.电桥配置ToolStripMenuItem.Name = "电桥配置ToolStripMenuItem";
            this.电桥配置ToolStripMenuItem.Size = new System.Drawing.Size(165, 30);
            this.电桥配置ToolStripMenuItem.Text = "电桥配置";
            this.电桥配置ToolStripMenuItem.Click += new System.EventHandler(this.电桥配置ToolStripMenuItem_Click);
            // 
            // 模板库ToolStripMenuItem
            // 
            this.模板库ToolStripMenuItem.Name = "模板库ToolStripMenuItem";
            this.模板库ToolStripMenuItem.Size = new System.Drawing.Size(165, 30);
            this.模板库ToolStripMenuItem.Text = "模板库";
            this.模板库ToolStripMenuItem.Click += new System.EventHandler(this.模板库ToolStripMenuItem_Click);
            // 
            // 工具ToolStripMenuItem
            // 
            this.工具ToolStripMenuItem.Name = "工具ToolStripMenuItem";
            this.工具ToolStripMenuItem.Size = new System.Drawing.Size(58, 25);
            this.工具ToolStripMenuItem.Text = "工具";
            // 
            // 系统ToolStripMenuItem
            // 
            this.系统ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.分析规则ToolStripMenuItem,
            this.系统参数ToolStripMenuItem,
            this.电桥参数ToolStripMenuItem});
            this.系统ToolStripMenuItem.Name = "系统ToolStripMenuItem";
            this.系统ToolStripMenuItem.Size = new System.Drawing.Size(58, 25);
            this.系统ToolStripMenuItem.Text = "系统";
            // 
            // 分析规则ToolStripMenuItem
            // 
            this.分析规则ToolStripMenuItem.Name = "分析规则ToolStripMenuItem";
            this.分析规则ToolStripMenuItem.Size = new System.Drawing.Size(233, 30);
            this.分析规则ToolStripMenuItem.Text = "分析规则";
            this.分析规则ToolStripMenuItem.Click += new System.EventHandler(this.分析规则ToolStripMenuItem_Click);
            // 
            // 系统参数ToolStripMenuItem
            // 
            this.系统参数ToolStripMenuItem.Name = "系统参数ToolStripMenuItem";
            this.系统参数ToolStripMenuItem.Size = new System.Drawing.Size(233, 30);
            this.系统参数ToolStripMenuItem.Text = "系统参数";
            this.系统参数ToolStripMenuItem.Click += new System.EventHandler(this.系统参数ToolStripMenuItem_Click);
            // 
            // 帮助ToolStripMenuItem
            // 
            this.帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
            this.帮助ToolStripMenuItem.Size = new System.Drawing.Size(58, 25);
            this.帮助ToolStripMenuItem.Text = "帮助";
            // 
            // 电桥参数ToolStripMenuItem
            // 
            this.电桥参数ToolStripMenuItem.Name = "电桥参数ToolStripMenuItem";
            this.电桥参数ToolStripMenuItem.Size = new System.Drawing.Size(233, 30);
            this.电桥参数ToolStripMenuItem.Text = "电桥参数";
            this.电桥参数ToolStripMenuItem.Click += new System.EventHandler(this.电桥参数ToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1188, 648);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.kryptonNavigator1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "FormMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.kryptonNavigator1)).EndInit();
            this.kryptonNavigator1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPage2)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Navigator.KryptonNavigator kryptonNavigator1;
        private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage1;
        private ComponentFactory.Krypton.Navigator.KryptonPage kryptonPage2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 资料库ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 资料库ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 尺寸管理ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 工具ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 系统ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 帮助ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 电桥配置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 模板库ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 分析规则ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 系统参数ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 电桥参数ToolStripMenuItem;
    }
}