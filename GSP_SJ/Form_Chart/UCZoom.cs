using BrowApp.Language;
using GSP;
using Newtonsoft.Json.Linq;
using SqlHelper;
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

        private int ZoomWidth = 400;
        private int ZoomHeight = 300;
        public UCZoom()
        {
            InitializeComponent();
            Dock = DockStyle.Fill;
            pictureBox1.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
            pictureBox1.Paint += new PaintEventHandler(pictureBox1_Paint);

        }
        Pen pen = new Pen(Color.Black, 3);
        Point drawp = new Point();
        public double width = 100;
        public double height = 70;
        public double w = 100;
        public double h = 70;
        public float Zoom = 1.0f;
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (drawp != null)
            {
                e.Graphics.DrawRectangle(pen, new Rectangle(drawp.X - (int)width / 2, drawp.Y - (int)height / 2, (int)width, (int)height));
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            width = w;
            height = h;
            Zoom = 1.0f;
            Point Mousep = new Point(e.Location.X, e.Location.Y);
            drawp = new Point(e.Location.X, e.Location.Y);
            switch (zoomablePictureBox1.type_Window)
            {
                case Type_Window.None:
                    break;
                case Type_Window.Puzzle:
                    Mousep = new Point(e.Location.X * 12, e.Location.Y * 12);
                    break;
                case Type_Window.Position:
                    Mousep = new Point(e.Location.X * 40, e.Location.Y * 40);
                    break;
                case Type_Window.OCR:
                    Mousep = new Point(e.Location.X * 12, e.Location.Y * 12);
                    break;
            }
            zoomablePictureBox1.setSelectPos(Mousep, kryptonPanel1.Width, kryptonPanel1.Height);
            pictureBox1.Refresh();
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
                    tool_IsFurnace.Visible = true;
                    break;
                case Type_Window.Position:
                    tool坐标旋转.Visible = true;
                    break;
                case Type_Window.OCR:
                    toolSelect.Visible = true;
                    toolROI.Visible = true;
                    toolStripButton1.Visible = false;
                    break;
            }

            btnZoom.Width = 25;
            btnZoom.Height = 25;

            zoomablePictureBox1.ActionZoom += new Action(() =>
            {
                float z = Zoom - zoomablePictureBox1.Zoom;
                if (z == 0 && zoomablePictureBox1.Zoom > 1)
                {
                    width = w / 2;
                    height = h / 2;
                }
                else if (z > 0)
                {
                    width = width + w * z * 2;
                    height = height + h * z * 2;
                }
                else if (width > w && height > h)
                {
                    width = width + w * z;
                    height = height + h * z;
                }
                pictureBox1.Refresh();
                Zoom = zoomablePictureBox1.Zoom;
            });

            zoomablePictureBox1.ActionMove += new Action(() =>
            {
                switch (zoomablePictureBox1.type_Window)
                {
                    case Type_Window.None:
                        break;
                    case Type_Window.Puzzle:
                        drawp.X -= zoomablePictureBox1.deltaX / 12;
                        drawp.Y -= zoomablePictureBox1.deltaY / 12;
                        break;
                    case Type_Window.Position:
                        drawp.X -= zoomablePictureBox1.deltaX / 40;
                        drawp.Y -= zoomablePictureBox1.deltaY / 40;
                        break;
                    case Type_Window.OCR:
                        drawp.X -= zoomablePictureBox1.deltaX / 12;
                        drawp.Y -= zoomablePictureBox1.deltaY / 12;
                        break;
                }
                pictureBox1.Refresh();
            });
            btnZoom_Click(this, null);
        }

        public void SetImage(Image image)
        {
            zoomablePictureBox1.Image = image;
            switch (zoomablePictureBox1.type_Window)
            {
                case Type_Window.None:
                    break;
                case Type_Window.Puzzle:
                    ZoomWidth = image.Width / 12;
                    ZoomHeight = image.Height / 12;
                    break;
                case Type_Window.Position:
                    ZoomWidth = image.Width / 40;
                    ZoomHeight = image.Height / 40;
                    break;
                case Type_Window.OCR:
                    ZoomWidth = image.Width / 12;
                    ZoomHeight = image.Height / 12;
                    break;
            }

            pictureBox1.Image = new Bitmap(image, ZoomWidth, ZoomHeight);
            btnZoom_Click(this, null);

        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (zoomablePictureBox1.Image != null)
                zoomablePictureBox1.setSelectCompont(toolStripComboBox1.Text, kryptonPanel1.Width, kryptonPanel1.Height);
        }

        public void SetSelectCompont(string name)
        {
            if (zoomablePictureBox1.Image != null)
            {
                zoomablePictureBox1.setSelectCompont(name, kryptonPanel1.Width, kryptonPanel1.Height);
                toolStripComboBox1.Text = name;
                Component component = zoomablePictureBox1.clickedComponent;
                width = w;
                height = h;
                switch (zoomablePictureBox1.type_Window)
                {
                    case Type_Window.None:
                        break;
                    case Type_Window.Puzzle:
                        drawp = new Point((int)component.Position.X / 12, (int)component.Position.Y / 12);
                        break;
                    case Type_Window.Position:
                        drawp = new Point((int)component.Position.X / 40, (int)component.Position.Y / 40);
                        break;
                    case Type_Window.OCR:
                        drawp = new Point((int)component.Position.X / 12, (int)component.Position.Y / 12);
                        break;
                }
                Zoom = 1.0f;
                pictureBox1.Refresh();
            }

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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            //打开图片
            openFileDialog.Filter = "png|*.png;*.jpg;*.jpeg;*.bmp";
            openFileDialog.RestoreDirectory = true;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(openFileDialog.FileName);
                //byte [] imgbyte = PublicFunction.CompressImage(img, "", 100, img.Width, img.Height);
                SetImage(img);//PublicFunction.ByteToBitmap(imgbyte));
                DBEventAction.ChangeImg?.Invoke(zoomablePictureBox1.type_Window);
            }
        }

        public Image Image
        {
            get
            {
                return zoomablePictureBox1.Image;
            }
        }

        private void tool坐标旋转_Click(object sender, EventArgs e)
        {
            FormRotatePosition formRotatePosition = new FormRotatePosition(DBEventAction.man_ReportItems[0].ReportCode, zoomablePictureBox1.components[0].ProductCode);
            DialogResult dialogResult = formRotatePosition.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                //保存之后，要刷新数据
                DBEventAction.RefreshManReport?.Invoke();
            }

        }

        private void btn重新定位_Click(object sender, EventArgs e)
        {
            //是否定位标准
            if (BrowApp.APP.Tip.ShowTip(0, "提示".tr(), "是否重新定位？".tr(), "是".tr(), "否".tr()) == 1)
            {
                DBEventAction._Reports.IsFurnace = false;
                SQLDataControl.UpdatateManReport_IsFurnace(DBEventAction._Reports.ReportCode, false);
                tool_IsFurnace.Text = "未定位".tr();
                tool_IsFurnace.ForeColor = Color.Red;
                if (this.ContextMenuStrip != null)
                {
                    this.ContextMenuStrip.Items[0].Enabled = true;
                }
            }

        }

        public void Refresh_IsFurnace()
        {
            if (DBEventAction._Reports.IsFurnace == null)
            {
                tool_IsFurnace.Text = "未定位".tr();
                tool_IsFurnace.ForeColor = Color.Red;
            }
            else if ((bool)DBEventAction._Reports.IsFurnace)
            {
                tool_IsFurnace.Text = "已定位".tr();
                tool_IsFurnace.ForeColor = Color.Green;
            }
            else
            {
                tool_IsFurnace.Text = "未定位".tr();
                tool_IsFurnace.ForeColor = Color.Red;
            }

        }


        private void btnZoom_Click(object sender, EventArgs e)
        {

            if (zoomablePictureBox1.Image == null || zoomPanel.Width > btnZoom.Width)
            {
                zoomPanel.Width = btnZoom.Width;
                zoomPanel.Height = btnZoom.Height;
                this.btnZoom.Values.Image = global::GSP_SJ.Properties.Resources.icons8_扩大_16;
            }
            else
            {
                zoomPanel.Width = ZoomWidth;
                zoomPanel.Height = ZoomHeight;
                this.btnZoom.Values.Image = global::GSP_SJ.Properties.Resources.icons8_缩小_16;
            }

        }
    }
}
