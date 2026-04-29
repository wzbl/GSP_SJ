namespace GSP_SJ.ModelClass
{
    partial class FormModelItem
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSaveModel = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.roiPictureBox1 = new GSP_SJ.ModelClass.DirectionalROIPictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridView1 = new ComponentFactory.Krypton.Toolkit.KryptonDataGridView();
            this.btnDeleteRoi = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnSave = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.kryptonLabel1 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.kryptonLabel2 = new ComponentFactory.Krypton.Toolkit.KryptonLabel();
            this.trackLight = new ComponentFactory.Krypton.Toolkit.KryptonTrackBar();
            this.trackcontrack = new ComponentFactory.Krypton.Toolkit.KryptonTrackBar();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.roiPictureBox1)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.btnDeleteRoi);
            this.panel1.Controls.Add(this.btnSaveModel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1579, 93);
            this.panel1.TabIndex = 0;
            // 
            // btnSaveModel
            // 
            this.btnSaveModel.Location = new System.Drawing.Point(707, 12);
            this.btnSaveModel.Name = "btnSaveModel";
            this.btnSaveModel.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnSaveModel.Size = new System.Drawing.Size(180, 53);
            this.btnSaveModel.TabIndex = 2;
            this.btnSaveModel.Values.Image = global::GSP_SJ.Properties.Resources._110;
            this.btnSaveModel.Values.Text = "添加ROI";
            this.btnSaveModel.Click += new System.EventHandler(this.btnAddRoi_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 93);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1579, 1164);
            this.panel2.TabIndex = 1;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.splitContainer2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(527, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1052, 1164);
            this.panel4.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.roiPictureBox1);
            this.splitContainer2.Size = new System.Drawing.Size(1052, 1164);
            this.splitContainer2.SplitterDistance = 350;
            this.splitContainer2.SplitterWidth = 5;
            this.splitContainer2.TabIndex = 0;
            // 
            // roiPictureBox1
            // 
            this.roiPictureBox1.DrawingType = GSP_SJ.ModelClass.ROIType.Rectangle;
            this.roiPictureBox1.Image = null;
            this.roiPictureBox1.Location = new System.Drawing.Point(4, 4);
            this.roiPictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.roiPictureBox1.Mode = GSP_SJ.ModelClass.ROIMode.Select;
            this.roiPictureBox1.Name = "roiPictureBox1";
            this.roiPictureBox1.Size = new System.Drawing.Size(483, 343);
            this.roiPictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.roiPictureBox1.TabIndex = 0;
            this.roiPictureBox1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.splitContainer1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(527, 1164);
            this.panel3.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dataGridView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.trackcontrack);
            this.splitContainer1.Panel2.Controls.Add(this.trackLight);
            this.splitContainer1.Panel2.Controls.Add(this.kryptonLabel2);
            this.splitContainer1.Panel2.Controls.Add(this.kryptonLabel1);
            this.splitContainer1.Size = new System.Drawing.Size(527, 1164);
            this.splitContainer1.SplitterDistance = 354;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 53;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.Size = new System.Drawing.Size(527, 354);
            this.dataGridView1.TabIndex = 0;
            // 
            // btnDeleteRoi
            // 
            this.btnDeleteRoi.Location = new System.Drawing.Point(906, 12);
            this.btnDeleteRoi.Name = "btnDeleteRoi";
            this.btnDeleteRoi.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnDeleteRoi.Size = new System.Drawing.Size(180, 53);
            this.btnDeleteRoi.TabIndex = 3;
            this.btnDeleteRoi.Values.Image = global::GSP_SJ.Properties.Resources._2;
            this.btnDeleteRoi.Values.Text = "删除ROI";
            this.btnDeleteRoi.Click += new System.EventHandler(this.btnDeleteRoi_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(36, 12);
            this.btnSave.Name = "btnSave";
            this.btnSave.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.btnSave.Size = new System.Drawing.Size(180, 53);
            this.btnSave.TabIndex = 4;
            this.btnSave.Values.Image = global::GSP_SJ.Properties.Resources._002_07;
            this.btnSave.Values.Text = "保存";
            // 
            // kryptonLabel1
            // 
            this.kryptonLabel1.Location = new System.Drawing.Point(21, 50);
            this.kryptonLabel1.Name = "kryptonLabel1";
            this.kryptonLabel1.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonLabel1.Size = new System.Drawing.Size(46, 26);
            this.kryptonLabel1.TabIndex = 4;
            this.kryptonLabel1.Values.Text = "亮度";
            // 
            // kryptonLabel2
            // 
            this.kryptonLabel2.Location = new System.Drawing.Point(21, 82);
            this.kryptonLabel2.Name = "kryptonLabel2";
            this.kryptonLabel2.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.kryptonLabel2.Size = new System.Drawing.Size(64, 26);
            this.kryptonLabel2.TabIndex = 5;
            this.kryptonLabel2.Values.Text = "对比度";
            // 
            // trackLight
            // 
            this.trackLight.DrawBackground = true;
            this.trackLight.Location = new System.Drawing.Point(98, 50);
            this.trackLight.Maximum = 128;
            this.trackLight.Minimum = -128;
            this.trackLight.Name = "trackLight";
            this.trackLight.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.trackLight.Size = new System.Drawing.Size(303, 27);
            this.trackLight.TabIndex = 6;
            this.trackLight.Scroll += new System.EventHandler(this.trackLight_Scroll);
            // 
            // trackcontrack
            // 
            this.trackcontrack.DrawBackground = true;
            this.trackcontrack.Location = new System.Drawing.Point(98, 83);
            this.trackcontrack.Maximum = 128;
            this.trackcontrack.Minimum = -128;
            this.trackcontrack.Name = "trackcontrack";
            this.trackcontrack.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.Office2007Blue;
            this.trackcontrack.Size = new System.Drawing.Size(303, 27);
            this.trackcontrack.TabIndex = 7;
            this.trackcontrack.Scroll += new System.EventHandler(this.trackcontrack_Scroll);
            // 
            // FormModelItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1579, 1257);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FormModelItem";
            this.Text = "设置模版";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.roiPictureBox1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private DirectionalROIPictureBox roiPictureBox1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSaveModel;
        private ComponentFactory.Krypton.Toolkit.KryptonDataGridView dataGridView1;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnDeleteRoi;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnSave;
        private ComponentFactory.Krypton.Toolkit.KryptonTrackBar trackcontrack;
        private ComponentFactory.Krypton.Toolkit.KryptonTrackBar trackLight;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel2;
        private ComponentFactory.Krypton.Toolkit.KryptonLabel kryptonLabel1;
    }
}