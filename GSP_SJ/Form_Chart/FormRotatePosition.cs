using BrowApp.Language;
using ComponentFactory.Krypton.Toolkit;
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
    public partial class FormRotatePosition : KryptonForm
    {
        public ComponentCanvas canvas;
        public FormRotatePosition()
        {
            InitializeComponent();
        }

        public FormRotatePosition(string reportCode, string productCode) : this()
        {
            InitializeComponent();
            canvas = new ComponentCanvas();
            canvas.Dock = DockStyle.Fill;
            this.Controls.Add(canvas);
            canvas.ProductCode = productCode;
            canvas.ReportCode = reportCode;
            // 添加工具栏
            AddToolbar();
            this.Load += (s, e) =>
            {
                RefreshXYData();
            };
        }

        private void RefreshXYData()
        {
            canvas.Components.Clear();
            foreach (var item in DBEventAction.man_ReportItems)
            {
                Size size = new Size(86, 55);
                Color color = Color.HotPink;
                if (item.Size == null)
                {
                    continue;
                }
                if (item.Size.Contains("01005"))
                {
                    size = new Size(16, 9);
                    color = Color.BlueViolet;
                }
                else if (item.Size.Contains("0201"))
                {
                    size = new Size(25, 13);
                    color = Color.DarkCyan;
                }
                else if (item.Size.Contains("0402"))
                {
                    size = new Size(43, 23);
                    color = Color.Blue;
                }
                else if (item.Size.Contains("0603"))
                {
                    size = new Size(68, 37);
                    color = Color.OrangeRed;
                }
                else if (item.Size.Contains("0805"))
                {
                    size = new Size(86, 55);
                    color = Color.Cyan;
                }
                else if (item.Size.Contains("1206"))
                {
                    size = new Size(133, 68);
                    color = Color.Brown;
                }
                else if (item.Position.Contains("MARK"))
                {
                    size = new Size(85, 47);
                    color = Color.DarkGreen;
                }
                string _x = item.X.ToString();
                string _y = item.Y.ToString();

                if (float.TryParse(_x.Trim().ToString(), out float x) && float.TryParse(_y.Trim().ToString(), out float y))
                {
                    float angle = 0;
                    if (float.TryParse(item.Angle.ToString(), out
                        angle))
                    {

                    }
                    Component component = new Component(item.Position, new PointF(x * Component.radio, (-y) * Component.radio), new SizeF(size.Width * Component.radio / 38.5f, size.Height * Component.radio / 38.5f), angle, "null", color);
                    canvas.Components.Add(component);
                }


            }
            canvas.FitToView();
        }

        private void AddToolbar()
        {
            ToolStrip toolStrip = new ToolStrip();

            //    // 缩放控制
            ToolStripLabel scaleLabel = new ToolStripLabel("缩放:");
            ToolStripTextBox scaleBox = new ToolStripTextBox();
            scaleBox.Text = "100%";
            scaleBox.Width = 60;
            scaleBox.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    if (scaleBox.Text.EndsWith("%"))
                    {
                        string percentStr = scaleBox.Text.TrimEnd('%');
                        if (float.TryParse(percentStr, out float percent))
                        {
                            canvas.Scale = percent / 100.0f;
                        }
                    }
                }
            };
            ToolStripButton toolStrip1 = new ToolStripButton("重置视图".tr());
            toolStrip1.Click += (s, e) => canvas.ResetView();
            ToolStripButton toolStrip2 = new ToolStripButton("适合视图".tr());
            toolStrip2.Click += (s, e) => canvas.FitToView();
            ToolStripButton toolStrip3 = new ToolStripButton("水平X镜像".tr());//原点X轴对称
            toolStrip3.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Position = new PointF(-canvas.Components[i].Position.X, canvas.Components[i].Position.Y);
                }
                canvas.FitToView();
            };
            ToolStripButton toolStrip4 = new ToolStripButton("水平Y镜像".tr());//原点Y轴对称
            toolStrip4.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Position = new PointF(canvas.Components[i].Position.X, -canvas.Components[i].Position.Y);
                }
                canvas.FitToView();
            };

            ToolStripButton toolStrip5 = new ToolStripButton("逆时针90".tr());
            toolStrip5.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.RotateCounterClockwise90(canvas.Components[i].Position.X, canvas.Components[i].Position.Y, out double x1, out double y1);
                    canvas.Components[i].Position = new PointF((float)x1, (float)y1);
                }
                canvas.FitToView();
            };
            ToolStripButton toolStrip6 = new ToolStripButton("顺时针90".tr());
            toolStrip6.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.RotateClockwise90(canvas.Components[i].Position.X, canvas.Components[i].Position.Y, out double x1, out double y1);
                    canvas.Components[i].Position = new PointF((float)x1, (float)y1);
                }
                canvas.FitToView();
            };

            ToolStripButton toolStrip7 = new ToolStripButton("旋转180".tr());
            toolStrip7.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Position = new PointF(-canvas.Components[i].Position.X, -canvas.Components[i].Position.Y);
                }
                canvas.FitToView();
            };

            ToolStripButton toolStrip8 = new ToolStripButton("元件水平X镜像".tr());
            toolStrip8.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    if (Math.Abs(Math.Sin(canvas.Components[i].Angle)) != 1)
                    {
                        canvas.Components[i].Angle = 180 - canvas.Components[i].Angle;
                    }
                }
                canvas.FitToView();
            };
            ToolStripButton toolStrip9 = new ToolStripButton("元件水平Y镜像".tr());
            toolStrip9.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    if (Math.Abs(Math.Sin(canvas.Components[i].Angle)) != 0)
                    {
                        canvas.Components[i].Angle = 360 - canvas.Components[i].Angle;

                    }
                }
                canvas.FitToView();
            };

            ToolStripButton toolStrip10 = new ToolStripButton("元件逆时针90".tr());
            toolStrip10.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Angle = canvas.Components[i].Angle - 90;
                }
                canvas.FitToView();
            };
            ToolStripButton toolStrip11 = new ToolStripButton("元件顺时针90".tr());
            toolStrip11.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Angle = 90 + canvas.Components[i].Angle;
                }
                canvas.FitToView();
            };

            ToolStripButton toolStrip12 = new ToolStripButton("元件旋转180".tr());
            toolStrip12.Click += (s, e) =>
            {
                for (int i = 0; i < canvas.Components.Count; i++)
                {
                    canvas.Components[i].Angle = 180 + canvas.Components[i].Angle;
                }
                canvas.FitToView();
            };

            ToolStripButton toolStrip13 = new ToolStripButton("保存数据".tr());
            toolStrip13.Click += async (s, e) =>
            {
                await canvas.SaveReportImageToDB();
                this.DialogResult = DialogResult.OK;
            };

            toolStrip.Items.AddRange(new ToolStripItem[]
            {
             toolStrip1,
             toolStrip2 ,
             toolStrip3,
             toolStrip4,
             toolStrip5,
             toolStrip6,
             toolStrip7,
             toolStrip8,
             toolStrip9,
             toolStrip10,
             toolStrip11,
             toolStrip12,
             toolStrip13
            });

            this.Controls.Add(toolStrip);
        }
    }
}
