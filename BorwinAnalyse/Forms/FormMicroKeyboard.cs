using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BorwinAnalyse.Forms
{
    public partial class FormMicroKeyboard : Form
    {
        public FormMicroKeyboard()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 字符增加事件委托
        /// </summary>
        /// <param name="newText">新的字符</param>
        public delegate void AddCharEventHandler(string newText);

        /// <summary>
        /// 字符增加事件
        /// </summary>
        public static event AddCharEventHandler AddCharEvent;


        /// <summary>
        /// 字符删减事件委托
        /// </summary>
        public delegate void RemoveCharEventHandler();

        /// <summary>
        /// 字符删减事件
        /// </summary>
        public static event RemoveCharEventHandler RemoveCharEvent;

        /// <summary>
        /// 光标移动事件委托
        /// </summary>
        /// <param name="Dir">光标移动方向</param>
        public delegate void CursorMoveEventHandler(CursorMoveDir Dir);

        /// <summary>
        /// 光标移动事件
        /// </summary>
        public static event CursorMoveEventHandler CursorMoveEvent;


        /// <summary>
        /// 字符清空事件委托
        /// </summary>
        public delegate void TextClearEventHandler();

        /// <summary>
        /// 字符清空事件
        /// </summary>
        public static event TextClearEventHandler TextClearEvent;

        /// <summary>
        /// 字符清空事件委托
        /// </summary>
        public delegate void TextReturnEventHandler();

        /// <summary>
        /// 字符清空事件
        /// </summary>
        public static event TextReturnEventHandler TextReturnEvent;

        /// <summary>
        /// 窗体关闭委托
        /// </summary>
        public delegate void FormCloseEventHandler();

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        public static event FormCloseEventHandler FormCloseEvent;

        /// <summary>
        /// 窗体拖动
        /// </summary>
        Point CurPoint;

        /// <summary>
        /// 窗体初始显示位置
        /// </summary>
        Point InitLocalPoint;

        /// <summary>
        /// 拖动窗体用，确认鼠标按下
        /// </summary>
        bool IsMouseDown = false;

        private void panelTop_MouseDown(object sender, MouseEventArgs e)
        {
            CurPoint = new Point(e.X, e.Y);
            IsMouseDown = true;
        }

        private void panelTop_MouseUp(object sender, MouseEventArgs e)
        {
            IsMouseDown = false;
        }

        private void panelTop_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseDown)
                this.Location = new Point(this.Location.X + e.X - CurPoint.X, this.Location.Y + e.Y - CurPoint.Y);
        }

        //绘制边框
        public void OnPaint(Object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.RoyalBlue);
            pen.Width = 5.0F;
            e.Graphics.DrawRectangle(pen, e.ClipRectangle.X - 1, e.ClipRectangle.Y - 1, e.ClipRectangle.Width, e.ClipRectangle.Height);
        }

        //键盘事件
        private void btn_Click(object sender, EventArgs e)
        {
            string tag = (sender as Button).Tag.ToString();

            switch (tag)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                case "-":
                    AddCharEvent(tag);
                    break;
                case "Dot":
                    AddCharEvent(".");
                    break;
                case "L":
                    CursorMoveEvent(CursorMoveDir.Left);
                    break;
                case "R":
                    CursorMoveEvent(CursorMoveDir.Right);
                    break;
                case "CE":
                    TextClearEvent();
                    break;
                case "BS":
                    RemoveCharEvent();
                    break;
                case "Ent":
                    TextReturnEvent();
                    break;
            }
        }

        private void FormMicroKeyboard_Load(object sender, EventArgs e)
        {
            foreach (Control control in this.tableLayoutPanel1.Controls)
            {
                if (control is Button)
                {
                    if (control.Name != "buttonCloseWin")
                    {
                        control.MouseClick += btn_Click;
                    }
                }
            }

            foreach (Control control in this.tableLayoutPanel2.Controls)
            {
                if (control is Button)
                {
                    if (control.Name != "buttonCloseWin")
                    {
                        control.MouseClick += btn_Click;
                    }
                }
            }

            foreach (Control control in this.tableLayoutPanel4.Controls)
            {
                if (control is Button)
                {
                    if (control.Name != "buttonCloseWin")
                    {
                        control.MouseClick += btn_Click;
                    }
                }
            }
        }

        private void buttonCloseWin_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
            FormCloseEvent();
        }

        private void textBoxView_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }
    }
    public enum CursorMoveDir
    {
        Left,
        Right
    }
}
