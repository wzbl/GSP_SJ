namespace GSP_SJ.ModelClass
{
    partial class UCOCRModel
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtOCRText = new System.Windows.Forms.TextBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.picImg = new System.Windows.Forms.PictureBox();
            this.hWindowControl1 = new HalconDotNet.HWindowControl();
            ((System.ComponentModel.ISupportInitialize)(this.picImg)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "方法";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "OCR字符";
            // 
            // txtOCRText
            // 
            this.txtOCRText.Location = new System.Drawing.Point(102, 49);
            this.txtOCRText.Name = "txtOCRText";
            this.txtOCRText.Size = new System.Drawing.Size(115, 21);
            this.txtOCRText.TabIndex = 2;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "OCV",
            "OCR"});
            this.comboBox1.Location = new System.Drawing.Point(102, 16);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(115, 20);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // picImg
            // 
            this.picImg.Location = new System.Drawing.Point(223, 3);
            this.picImg.Name = "picImg";
            this.picImg.Size = new System.Drawing.Size(116, 67);
            this.picImg.TabIndex = 4;
            this.picImg.TabStop = false;
            // 
            // hWindowControl1
            // 
            this.hWindowControl1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl1.Location = new System.Drawing.Point(348, 1);
            this.hWindowControl1.Name = "hWindowControl1";
            this.hWindowControl1.Size = new System.Drawing.Size(119, 71);
            this.hWindowControl1.TabIndex = 5;
            this.hWindowControl1.WindowSize = new System.Drawing.Size(119, 71);
            // 
            // UCOCRModel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.hWindowControl1);
            this.Controls.Add(this.picImg);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.txtOCRText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "UCOCRModel";
            this.Size = new System.Drawing.Size(642, 74);
            ((System.ComponentModel.ISupportInitialize)(this.picImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtOCRText;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.PictureBox picImg;
        private HalconDotNet.HWindowControl hWindowControl1;
    }
}
