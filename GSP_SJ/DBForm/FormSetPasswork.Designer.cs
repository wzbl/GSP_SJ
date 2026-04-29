namespace GSP_SJ.DBForm
{
    partial class FormSetPasswork
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
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.txtOld = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.txtNew = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.txtCheck = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnOK = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnCancle = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.SuspendLayout();
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(45, 26);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonLabel1.Size = new System.Drawing.Size(64, 26);
            this.kryptonLabel1.TabIndex = 0;
            this.kryptonLabel1.Values.Text = "旧密码";
            // 
            // txtOld
            // 
            this.txtOld.Location = new System.Drawing.Point(185, 23);
            this.txtOld.Name = "txtOld";
            this.txtOld.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.txtOld.PasswordChar = '*';
            this.txtOld.Size = new System.Drawing.Size(169, 29);
            this.txtOld.TabIndex = 1;
            // 
            // txtNew
            // 
            this.txtNew.Location = new System.Drawing.Point(185, 69);
            this.txtNew.Name = "txtNew";
            this.txtNew.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.txtNew.PasswordChar = '*';
            this.txtNew.Size = new System.Drawing.Size(169, 29);
            this.txtNew.TabIndex = 3;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(45, 72);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonLabel2.Size = new System.Drawing.Size(64, 26);
            this.kryptonLabel2.TabIndex = 2;
            this.kryptonLabel2.Values.Text = "新密码";
            // 
            // txtCheck
            // 
            this.txtCheck.Location = new System.Drawing.Point(185, 116);
            this.txtCheck.Name = "txtCheck";
            this.txtCheck.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.txtCheck.PasswordChar = '*';
            this.txtCheck.Size = new System.Drawing.Size(169, 29);
            this.txtCheck.TabIndex = 5;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(45, 119);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonLabel3.Size = new System.Drawing.Size(81, 26);
            this.kryptonLabel3.TabIndex = 4;
            this.kryptonLabel3.Values.Text = "确认密码";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(220, 182);
            this.btnOK.Name = "btnOK";
            this.btnOK.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnOK.Size = new System.Drawing.Size(134, 55);
            this.btnOK.TabIndex = 6;
            this.btnOK.Values.Image = global::GSP_SJ.Properties.Resources._155;
            this.btnOK.Values.Text = "确定";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancle
            // 
            this.btnCancle.Location = new System.Drawing.Point(45, 182);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnCancle.Size = new System.Drawing.Size(146, 55);
            this.btnCancle.TabIndex = 7;
            this.btnCancle.Values.Image = global::GSP_SJ.Properties.Resources._168;
            this.btnCancle.Values.Text = "取消";
            // 
            // FormSetPasswork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 252);
            this.Controls.Add(this.btnCancle);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtCheck);
            this.Controls.Add(this.kryptonLabel3);
            this.Controls.Add(this.txtNew);
            this.Controls.Add(this.kryptonLabel2);
            this.Controls.Add(this.txtOld);
            this.Controls.Add(this.kryptonLabel1);
            this.Name = "FormSetPasswork";
            this.Text = "设置密码";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtOld;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtNew;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtCheck;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnOK;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnCancle;
    }
}