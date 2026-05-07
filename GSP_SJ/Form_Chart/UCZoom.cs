using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ.Form_Chart
{
    public partial class UCZoom : UserControl
    {
        ZoomablePictureBox zoomablePictureBox1;
        public UCZoom()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
        }


        public UCZoom(Type_Window type_Window) : this()
        {
            zoomablePictureBox1 = new ZoomablePictureBox(type_Window);
            kryptonPanel1.Controls.Add(zoomablePictureBox1);
            switch (type_Window)
            {
                case Type_Window.None:
                    break;
                case Type_Window.Puzzle:
                    break;
                case Type_Window.Screen:
                    break;
                case Type_Window.OCR:
                    toolSelect.Visible = true;
                    toolROI.Visible = true;
                    break;
            }
        }

        public void SetImage(Image image)
        {
            zoomablePictureBox1.Image = image;
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            zoomablePictureBox1.setSelectCompont(toolStripComboBox1.Text, kryptonPanel1.Width, kryptonPanel1.Height);
        }

        public void SetSelectCompont(string name)
        {
            zoomablePictureBox1.setSelectCompont(name, kryptonPanel1.Width, kryptonPanel1.Height);
            toolStripComboBox1.Text = name;
        }

        public void AddComponent(Component component)
        {
            zoomablePictureBox1.components.Add(component);
            toolStripComboBox1.Items.Add(component.Designator);
        }

        public List<Component> Components
        {
            get
            {
                return zoomablePictureBox1.components;
            }
        }

        public void UpdateComponent(string position, PointF point)
        {
            zoomablePictureBox1.components.Where(D => D.Designator == position).FirstOrDefault().Position = new PointF(point.X, point.Y);
        }

        public PointF PixPoint()
        {
            return zoomablePictureBox1.PixPoint();
        }

        private void toolSelect_Click(object sender, EventArgs e)
        {
            zoomablePictureBox1.SetROIModel(ROIModel.Select);
            toolSelect.Checked = true;
            toolROI.Checked = false;
        }

        private void toolROI_Click(object sender, EventArgs e)
        {
            zoomablePictureBox1.SetROIModel(ROIModel.RectangleROI);
            toolSelect.Checked = false;
            toolROI.Checked = true;
        }

      
        public bool GetSelectComponent(out Image image, out Component component)
        {
            return zoomablePictureBox1.GetSelectComponent(out image, out component);
        }
    }
}
