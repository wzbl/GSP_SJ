namespace GSP.UI
{
    partial class UI_Language
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
            this.uI_Language1 = new BrowApp.Language.UI_Language();
            this.SuspendLayout();
            // 
            // uI_Language1
            // 
            this.uI_Language1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.uI_Language1.Location = new System.Drawing.Point(0, 0);
            this.uI_Language1.Name = "uI_Language1";
            this.uI_Language1.Size = new System.Drawing.Size(800, 541);
            this.uI_Language1.TabIndex = 0;
            // 
            // UI_Language
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uI_Language1);
            this.Name = "UI_Language";
            this.Size = new System.Drawing.Size(800, 541);
            this.ResumeLayout(false);

        }

        #endregion

        private BrowApp.Language.UI_Language uI_Language1;
    }
}
