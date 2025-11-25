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
            this.btnSaveImg = new System.Windows.Forms.Button();
            this.btnDeleteRoi = new System.Windows.Forms.Button();
            this.btnAddRoi = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.roiPictureBox1 = new DirectionalROIPictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.trackcontrack = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.trackLight = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
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
            ((System.ComponentModel.ISupportInitialize)(this.trackcontrack)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackLight)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSaveImg);
            this.panel1.Controls.Add(this.btnDeleteRoi);
            this.panel1.Controls.Add(this.btnAddRoi);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1579, 93);
            this.panel1.TabIndex = 0;
            // 
            // btnSaveImg
            // 
            this.btnSaveImg.Location = new System.Drawing.Point(737, 16);
            this.btnSaveImg.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSaveImg.Name = "btnSaveImg";
            this.btnSaveImg.Size = new System.Drawing.Size(100, 49);
            this.btnSaveImg.TabIndex = 2;
            this.btnSaveImg.Text = "保存图片";
            this.btnSaveImg.UseVisualStyleBackColor = true;
            this.btnSaveImg.Click += new System.EventHandler(this.btnSaveImg_Click);
            // 
            // btnDeleteRoi
            // 
            this.btnDeleteRoi.Location = new System.Drawing.Point(1044, 16);
            this.btnDeleteRoi.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnDeleteRoi.Name = "btnDeleteRoi";
            this.btnDeleteRoi.Size = new System.Drawing.Size(100, 49);
            this.btnDeleteRoi.TabIndex = 1;
            this.btnDeleteRoi.Text = "删除ROI";
            this.btnDeleteRoi.UseVisualStyleBackColor = true;
            this.btnDeleteRoi.Click += new System.EventHandler(this.btnDeleteRoi_Click);
            // 
            // btnAddRoi
            // 
            this.btnAddRoi.Location = new System.Drawing.Point(887, 16);
            this.btnAddRoi.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnAddRoi.Name = "btnAddRoi";
            this.btnAddRoi.Size = new System.Drawing.Size(100, 49);
            this.btnAddRoi.TabIndex = 0;
            this.btnAddRoi.Text = "新增ROI";
            this.btnAddRoi.UseVisualStyleBackColor = true;
            this.btnAddRoi.Click += new System.EventHandler(this.btnAddRoi_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 93);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1579, 1164);
            this.panel2.TabIndex = 1;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.splitContainer2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(527, 0);
            this.panel4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1052, 1164);
            this.panel4.TabIndex = 3;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.roiPictureBox1.DrawingType = ROIType.Rectangle;
            this.roiPictureBox1.Image = null;
            this.roiPictureBox1.Location = new System.Drawing.Point(134, 4);
            this.roiPictureBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.roiPictureBox1.Mode = ROIMode.Select;
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
            this.panel3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(527, 1164);
            this.panel3.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.trackLight);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
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
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 53;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(527, 354);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // trackcontrack
            // 
            this.trackcontrack.Location = new System.Drawing.Point(93, 79);
            this.trackcontrack.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.trackcontrack.Maximum = 255;
            this.trackcontrack.Minimum = -255;
            this.trackcontrack.Name = "trackcontrack";
            this.trackcontrack.Size = new System.Drawing.Size(308, 58);
            this.trackcontrack.TabIndex = 3;
            this.trackcontrack.Scroll += new System.EventHandler(this.trackcontrack_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 92);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "对比度";
            // 
            // trackLight
            // 
            this.trackLight.Location = new System.Drawing.Point(93, 35);
            this.trackLight.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.trackLight.Maximum = 128;
            this.trackLight.Minimum = -128;
            this.trackLight.Name = "trackLight";
            this.trackLight.Size = new System.Drawing.Size(308, 58);
            this.trackLight.TabIndex = 1;
            this.trackLight.Scroll += new System.EventHandler(this.trackLight_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(47, 52);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "亮度";
            // 
            // FormModelItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1579, 1257);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormModelItem";
            this.Text = "FormModelItem";
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
            ((System.ComponentModel.ISupportInitialize)(this.trackcontrack)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackLight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button btnDeleteRoi;
        private System.Windows.Forms.Button btnAddRoi;
        private DirectionalROIPictureBox roiPictureBox1;
        private System.Windows.Forms.TrackBar trackcontrack;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackLight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSaveImg;
    }
}