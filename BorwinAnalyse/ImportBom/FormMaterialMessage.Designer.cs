namespace BorwinAnalyse.ImportBom
{
    partial class FormMaterialMessage
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
            this.kryptonPanel1 = new ComponentFactory.Krypton.Toolkit.KryptonPanel();
            this.txtMaterialName = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel21 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.txtMaterialCode = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            this.kryptonLabel22 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnDelete = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnOK = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.btnDelete);
            this.kryptonPanel1.Controls.Add(this.btnOK);
            this.kryptonPanel1.Controls.Add(this.txtMaterialName);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel21);
            this.kryptonPanel1.Controls.Add(this.txtMaterialCode);
            this.kryptonPanel1.Controls.Add(this.kryptonLabel22);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.Size = new System.Drawing.Size(568, 172);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // txtMaterialName
            // 
            this.txtMaterialName.Location = new System.Drawing.Point(159, 65);
            this.txtMaterialName.Name = "txtMaterialName";
            this.txtMaterialName.Size = new System.Drawing.Size(379, 29);
            this.txtMaterialName.TabIndex = 31;
            // 
            // kryptonLabel21
            // 
            this.kryptonLabel21.Location = new System.Drawing.Point(19, 68);
            this.kryptonLabel21.Name = "kryptonLabel21";
            this.kryptonLabel21.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonLabel21.Size = new System.Drawing.Size(81, 26);
            this.kryptonLabel21.TabIndex = 30;
            this.kryptonLabel21.Values.Text = "产品描述";
            // 
            // txtMaterialCode
            // 
            this.txtMaterialCode.Location = new System.Drawing.Point(159, 30);
            this.txtMaterialCode.Name = "txtMaterialCode";
            this.txtMaterialCode.ReadOnly = true;
            this.txtMaterialCode.Size = new System.Drawing.Size(379, 29);
            this.txtMaterialCode.TabIndex = 29;
            // 
            // kryptonLabel22
            // 
            this.kryptonLabel22.Location = new System.Drawing.Point(19, 31);
            this.kryptonLabel22.Name = "kryptonLabel22";
            this.kryptonLabel22.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonLabel22.Size = new System.Drawing.Size(81, 26);
            this.kryptonLabel22.TabIndex = 28;
            this.kryptonLabel22.Values.Text = "产品编码";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(410, 116);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnDelete.Size = new System.Drawing.Size(128, 44);
            this.btnDelete.TabIndex = 33;
            this.btnDelete.Values.Image = global::BorwinAnalyse.Properties.Resources._168;
            this.btnDelete.Values.Text = "取消";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(245, 116);
            this.btnOK.Name = "btnOK";
            this.btnOK.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnOK.Size = new System.Drawing.Size(123, 44);
            this.btnOK.TabIndex = 32;
            this.btnOK.Values.Image = global::BorwinAnalyse.Properties.Resources._155;
            this.btnOK.Values.Text = "确认";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FormMaterialMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 172);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMaterialMessage";
            this.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Silver;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormMaterialMessage";
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonPanel kryptonPanel1;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtMaterialName;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel21;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtMaterialCode;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel22;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnDelete;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnOK;
    }
}