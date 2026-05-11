namespace GSP_SJ.DBForm
{
    partial class FormLogin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLogin));
            this.txtpassword = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel3 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.txtcode = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnCancle = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnOK = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtpassword
            // 
            this.txtpassword.Location = new System.Drawing.Point(179, 86);
            this.txtpassword.Name = "txtpassword";
            this.txtpassword.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.txtpassword.PasswordChar = '*';
            this.txtpassword.Size = new System.Drawing.Size(169, 29);
            this.txtpassword.TabIndex = 9;
            // 
            // kryptonLabel3
            // 
            this.kryptonLabel3.Location = new System.Drawing.Point(39, 89);
            this.kryptonLabel3.Name = "kryptonLabel3";
            this.kryptonLabel3.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonLabel3.Size = new System.Drawing.Size(46, 26);
            this.kryptonLabel3.TabIndex = 8;
            this.kryptonLabel3.Values.Text = "密码";
            // 
            // txtcode
            // 
            this.txtcode.Location = new System.Drawing.Point(179, 39);
            this.txtcode.Name = "txtcode";
            this.txtcode.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.txtcode.Size = new System.Drawing.Size(169, 29);
            this.txtcode.TabIndex = 7;
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(39, 42);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonLabel2.Size = new System.Drawing.Size(46, 26);
            this.kryptonLabel2.TabIndex = 6;
            this.kryptonLabel2.Values.Text = "账号";
            // 
            // btnCancle
            // 
            this.btnCancle.Location = new System.Drawing.Point(39, 169);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnCancle.Size = new System.Drawing.Size(146, 55);
            this.btnCancle.TabIndex = 11;
            this.btnCancle.Values.Image = global::GSP_SJ.Properties.Resources._168;
            this.btnCancle.Values.Text = "取消";
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(214, 169);
            this.btnOK.Name = "btnOK";
            this.btnOK.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnOK.Size = new System.Drawing.Size(134, 55);
            this.btnOK.TabIndex = 10;
            this.btnOK.Values.Image = global::GSP_SJ.Properties.Resources._155;
            this.btnOK.Values.Text = "登入";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.kryptonLabel2);
            this.kryptonPanel1.Controls.Add(this.btnCancle);
            this.kryptonPanel1.Controls.Add(this.txtcode);
            this.kryptonPanel1.Controls.Add(this.btnOK);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel3);
            this.kryptonPanel1.Controls.Add(this.txtpassword);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonPanel1.Size = new System.Drawing.Size(405, 256);
            this.kryptonPanel1.TabIndex = 12;
            // 
            // FormLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(405, 256);
            this.Controls.Add(this.kryptonPanel1);
            this.HeaderStyle = ComponentFactory.Krypton.Toolkit.HeaderStyle.Primary;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormLogin";
            this.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "登入";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtpassword;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel3;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtcode;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnCancle;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnOK;
        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
    }
}