namespace GSP_SJ
{
    partial class FormLocation_Point
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
            this.comPosition = new ComponentFactory.Krypton.Toolkit.KryptonComboBox();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.btnOK = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.txtChoise = new ComponentFactory.Krypton.Toolkit.KryptonTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.comPosition)).BeginInit();
            this.SuspendLayout();
            // 
            // comPosition
            // 
            this.comPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comPosition.DropDownWidth = 139;
            this.comPosition.Location = new System.Drawing.Point(105, 63);
            this.comPosition.Name = "comPosition";
            this.comPosition.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.comPosition.Size = new System.Drawing.Size(139, 27);
            this.comPosition.TabIndex = 0;
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(5, 62);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonLabel1.Size = new System.Drawing.Size(46, 26);
            this.kryptonLabel1.TabIndex = 1;
            this.kryptonLabel1.Values.Text = "位号";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(268, 21);
            this.btnOK.Name = "btnOK";
            this.btnOK.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnOK.Size = new System.Drawing.Size(139, 66);
            this.btnOK.TabIndex = 2;
            this.btnOK.Values.Text = "确定";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(5, 22);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonLabel2.Size = new System.Drawing.Size(46, 26);
            this.kryptonLabel2.TabIndex = 3;
            this.kryptonLabel2.Values.Text = "筛选";
            // 
            // txtChoise
            // 
            this.txtChoise.Location = new System.Drawing.Point(105, 21);
            this.txtChoise.Name = "txtChoise";
            this.txtChoise.Size = new System.Drawing.Size(139, 29);
            this.txtChoise.TabIndex = 4;
            this.txtChoise.TextChanged += new System.EventHandler(this.txtChoise_TextChanged);
            // 
            // FormLocation_Point
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 106);
            this.Controls.Add(this.txtChoise);
            this.Controls.Add(this.kryptonLabel2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.kryptonLabel1);
            this.Controls.Add(this.comPosition);
            this.Name = "FormLocation_Point";
            this.Text = "选择位号";
            ((System.ComponentModel.ISupportInitialize)(this.comPosition)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ComponentFactory.Krypton.Toolkit.KryptonComboBox comPosition;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnOK;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonTextBox txtChoise;
    }
}