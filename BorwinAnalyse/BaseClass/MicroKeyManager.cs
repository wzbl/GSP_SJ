using BorwinAnalyse.Forms;
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BorwinAnalyse.BaseClass
{
    public class MicroKeyManager
    {
        //单例
        private static MicroKeyManager instance;
        public static MicroKeyManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MicroKeyManager();
                }
                return instance;
            }
        }

        #region System Keyboard
        public static Process kbpr;
        private void btnOSK_Click(object sender, EventArgs e)
        {
            OSKPro();
        }
        /// <summary>
        /// 系统键盘
        /// </summary>
        public static void OSKPro()
        {
            if (kbpr != null && !kbpr.HasExited)
            {
                kbpr.Kill();
            }
            else
            {
                kbpr = System.Diagnostics.Process.Start("osk.exe");
            }
        }
        #endregion

        /// <summary>
        /// 微型键盘窗体
        /// </summary>
        FormMicroKeyboard WinFormMicroKeyboard;

        /// <summary>
        /// 指向未向键盘的文本框
        /// </summary>
        public TextBox TexeBoxView;

        /// <summary>
        /// 指向目标文本框
        /// </summary>
        public TextBox TexeBoxFocusing;

        /// <summary>
        /// 指向目标数据表
        /// </summary>
        public DataGridView DataGridViewFocusing;

        /// <summary>
        /// 指向目标数字显示框
        /// </summary>
        public NumericUpDown NumericUpDownFocusing;

        /// <summary>
        /// 目标字符串
        /// </summary>
        public string TargetText = String.Empty;

        /// <summary>
        /// 窗体初始显示位置
        /// </summary>
        Point InitLocalPoint = new Point(0, 0);

        /// <summary>
        /// 最上层父级控件的位置
        /// </summary>
        Point TopParentConLocal = new Point(0, 0);

        /// <summary>
        /// 最上层父级控件的尺寸
        /// </summary>
        Size TopParentConSize = new Size(0, 0);

        /// <summary>
        /// 当前文本框的名称
        /// </summary>
        String FocusingTexeBoxName { get; set; } = "+_+";                         // 不能为String.Empty,KryptonTextBox.TextBox的Name是String.Empty

        /// <summary>
        /// 当前数字框的名称
        /// </summary>
        String FocusingNumericUpDownName { get; set; } = "+_+";                   // 不能为String.Empty,KryptonTextBox.TextBox的Name是String.Empty

        /// <summary>
        /// 当前数据表的名称
        /// </summary>
        String FocusingDGVName { get; set; } = String.Empty;

        /// <summary>
        /// 当前选定的行
        /// </summary>
        int CellRow { get; set; } = -1;

        /// <summary>
        /// 当前选定的列
        /// </summary>
        int CellColumn { get; set; } = -1;

        public MicroKeyManager()
        {
            FormMicroKeyboard.AddCharEvent += AddChar;
            FormMicroKeyboard.RemoveCharEvent += RemoveChar;
            FormMicroKeyboard.TextClearEvent += ClearText;
            FormMicroKeyboard.CursorMoveEvent += TextBoxCursorMove;
            FormMicroKeyboard.TextReturnEvent += UpdateText;
            FormMicroKeyboard.FormCloseEvent += CloseMicroKey;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            if (WinFormMicroKeyboard != null)
            {
                WinFormMicroKeyboard.Dispose();
            }

            TexeBoxView = null;
            TexeBoxFocusing = null;
            DataGridViewFocusing = null;
            NumericUpDownFocusing = null;

            TargetText = String.Empty;

            InitLocalPoint = new Point(0, 0);
            TopParentConLocal = new Point(0, 0);

            TopParentConSize = new Size(0, 0);

            FocusingTexeBoxName = "+_+";
            FocusingDGVName = String.Empty;
            FocusingNumericUpDownName = "+_+";

            CellRow = -1;
            CellColumn = -1;
        }


        /// <summary>
        /// 给所有对象控件增加显示数字键盘的事件
        /// </summary>
        /// <param name="Control"></param>
        public void AddEventToControl(Control Control)
        {
            if (Control is KryptonTextBox || Control is KryptonNumericUpDown)            //子控件是TextBox、NumericUpDown，不退出，KryptonTextBox和KryptonNumericUpDown会增加两次事件
            {
                return;
            }
            foreach (Control control in Control.Controls)
            {
                if (control.Controls.Count > 0)
                {
                    if (control is TextBox || control is NumericUpDown)
                    {
                        control.MouseClick += ShowMicroKeyBoard;
                    }
                    if (control is KryptonTextBox)
                    {
                        (control as KryptonTextBox).TextBox.MouseClick += ShowMicroKeyBoard;
                    }
                    if (control is KryptonNumericUpDown)
                    {
                        (control as KryptonNumericUpDown).NumericUpDown.MouseClick += ShowMicroKeyBoard;
                    }
                    if (control is DataGridView || control is KryptonDataGridView)
                    {
                        (control as DataGridView).CellDoubleClick += ShowMicroKeyBoard;
                        (control as DataGridView).CellClick += CloseMicroKey;
                    }
                    AddEventToControl(control);
                }
                else
                {
                    if (control is TextBox || control is NumericUpDown)
                    {
                        control.MouseClick += ShowMicroKeyBoard;
                    }
                    if (control is KryptonTextBox)
                    {
                        (control as KryptonTextBox).TextBox.MouseClick += ShowMicroKeyBoard;
                    }
                    if (control is KryptonNumericUpDown || control is NumericUpDown)
                    {
                        (control as KryptonNumericUpDown).NumericUpDown.MouseClick += ShowMicroKeyBoard;
                    }
                    if (control is DataGridView || control is KryptonDataGridView)
                    {
                        (control as DataGridView).CellDoubleClick += ShowMicroKeyBoard;
                        (control as DataGridView).CellClick += CloseMicroKey;
                    }
                }
            }
        }

        /// <summary>
        /// 弹出键盘窗体事件的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ShowMicroKeyBoard(object sender, MouseEventArgs e)
        {
            KEYBOARD_TYPE ret = KEYBOARD_TYPE.None;
            if (sender is TextBox)
            {
                TextBox textBox = sender as TextBox;
                ret = MicroKeyManager.Instance.SetTextBox(textBox);
                if (ret == KEYBOARD_TYPE.SYSTEM)
                {
                    //try
                    //{
                    //    OSKPro();
                    //}
                    //catch
                    //{

                    //}
                    return;
                }
                else if (ret == KEYBOARD_TYPE.None)
                {
                    return;
                }
                InitLocalPoint = new Point(0, 0);
                if (textBox.Parent != null)
                {
                    GetInitPosition(textBox.Parent);
                    GetTopParentConSize(textBox.Parent);
                }

                Point MicroKeyBoardPos = new Point(0, 0);
                MicroKeyBoardPos.X = InitLocalPoint.X + textBox.Location.X;
                MicroKeyBoardPos.Y = InitLocalPoint.Y + textBox.Location.Y + textBox.Height + 45;

                ShowMicroKey(MicroKeyBoardPos);
                TexeBoxView.Focus();
            }

            if (sender is NumericUpDown)
            {
                NumericUpDown numericUpDown = sender as NumericUpDown;
                ret = MicroKeyManager.Instance.SetNumericUpDown(numericUpDown);
                if (ret == KEYBOARD_TYPE.SYSTEM)
                {
                    //try
                    //{
                    //    OSKPro();
                    //}
                    //catch
                    //{

                    //}
                    return;
                }
                else if (ret == KEYBOARD_TYPE.None)
                {
                    return;
                }
                InitLocalPoint = new Point(0, 0);
                if (numericUpDown.Parent != null)
                {
                    GetInitPosition(numericUpDown.Parent);
                    GetTopParentConSize(numericUpDown.Parent);
                }

                Point MicroKeyBoardPos = new Point(0, 0);
                MicroKeyBoardPos.X = InitLocalPoint.X + numericUpDown.Location.X;
                MicroKeyBoardPos.Y = InitLocalPoint.Y + numericUpDown.Location.Y + numericUpDown.Height + 45;

                ShowMicroKey(MicroKeyBoardPos);
                TexeBoxView.Focus();
            }
        }

        /// <summary>
        /// 弹出键盘窗体事件的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ShowMicroKeyBoard(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;

            KEYBOARD_TYPE ret = KEYBOARD_TYPE.None;
            if (sender is DataGridView)
            {
                DataGridView dataGridView = sender as DataGridView;
                ret = MicroKeyManager.Instance.SetDataGridViewCell(dataGridView);
                if (ret == KEYBOARD_TYPE.SYSTEM)
                {
                    //try
                    //{
                    //    OSKPro();
                    //}
                    //catch
                    //{

                    //}
                    return;
                }
                else if (ret == KEYBOARD_TYPE.None)
                {
                    return;
                }
                InitLocalPoint = new Point(0, 0);
                if (dataGridView.Parent != null)
                {
                    GetInitPosition(dataGridView.Parent);
                    GetTopParentConSize(dataGridView.Parent);
                }

                Point MicroKeyBoardPos = new Point(0, 0);
                MicroKeyBoardPos.X = InitLocalPoint.X + dataGridView.Location.X + dataGridView.GetCellDisplayRectangle(CellColumn, CellRow, false).X;
                MicroKeyBoardPos.Y = InitLocalPoint.Y + dataGridView.Location.Y + dataGridView.GetCellDisplayRectangle(CellColumn, CellRow, false).Y + 65;

                ShowMicroKey(MicroKeyBoardPos);
                TexeBoxView.Focus();
            }
        }

        /// <summary>
        /// 获得所有上级父类的坐标
        /// </summary>
        /// <param name="control"></param>
        public void GetInitPosition(Control control)
        {
            InitLocalPoint.X += control.Location.X;
            InitLocalPoint.Y += control.Location.Y;
            TopParentConLocal = control.Location;
            if (control.Parent == null)
            {
                return;
            }
            Control controlParent = control.Parent;
            GetInitPosition(controlParent);
        }

        /// <summary>
        /// 获得最上层父类的尺寸
        /// </summary>
        /// <param name="control"></param>
        public void GetTopParentConSize(Control control)
        {
            TopParentConSize = control.Size;
            if (control.Parent == null)
            {
                return;
            }
            Control controlParent = control.Parent;
            GetTopParentConSize(controlParent);
        }

        /// <summary>
        /// 设定目标文本框
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns></returns>
        public KEYBOARD_TYPE SetTextBox(TextBox textBox)
        {
            if (textBox.Name == FocusingTexeBoxName)
            {
                return KEYBOARD_TYPE.None;
            }

            string pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";                       //整型或浮点型
            Regex regex = new Regex(pattern);

            string text = textBox.Text;
            if (!regex.IsMatch(textBox.Text) || String.IsNullOrEmpty(text))
            {
                return KEYBOARD_TYPE.SYSTEM;
            }

            Init();
            TexeBoxFocusing = textBox;
            TargetText = text;
            FocusingTexeBoxName = textBox.Name;

            return KEYBOARD_TYPE.MICRO;
        }

        /// <summary>
        /// 设定目标文本框
        /// </summary>
        /// <param name="numericUpDown"></param>
        /// <returns></returns>
        public KEYBOARD_TYPE SetNumericUpDown(NumericUpDown numericUpDown)
        {
            if (numericUpDown.Name == FocusingNumericUpDownName)
            {
                return KEYBOARD_TYPE.None;
            }

            string pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";                       //整型或浮点型
            Regex regex = new Regex(pattern);

            string text = numericUpDown.Text;
            if (!regex.IsMatch(text) || String.IsNullOrEmpty(text))
            {
                return KEYBOARD_TYPE.SYSTEM;
            }

            Init();
            NumericUpDownFocusing = numericUpDown;
            TargetText = text;
            if (String.IsNullOrEmpty(numericUpDown.Name))
            {
                if (numericUpDown.Parent != null)
                {
                    FocusingNumericUpDownName = numericUpDown.Parent.Name;
                }
            }
            else
            {
                FocusingNumericUpDownName = numericUpDown.Name;
            }

            return KEYBOARD_TYPE.MICRO;
        }

        /// <summary>
        /// 设定目标单元格
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns></returns>
        public KEYBOARD_TYPE SetDataGridViewCell(DataGridView dataGridView)
        {
            int rowIndex = dataGridView.CurrentCell.RowIndex;
            int columnIndex = dataGridView.CurrentCell.ColumnIndex;

            if ((dataGridView.Name == FocusingDGVName && rowIndex == CellRow && columnIndex == CellColumn)
                || dataGridView.Rows[rowIndex].Cells[columnIndex].GetType().Name != "DataGridViewTextBoxCell"
                || dataGridView.Columns[columnIndex].ReadOnly
                || rowIndex == -1
                || dataGridView.Rows[rowIndex].Cells[columnIndex].ReadOnly)
            {
                return KEYBOARD_TYPE.None;
            }

            string pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";                       //整型或浮点型
            Regex regex = new Regex(pattern);

            String text = dataGridView.CurrentCell.FormattedValue.ToString();

            if (!regex.IsMatch(text) || String.IsNullOrEmpty(text))
            {
                return KEYBOARD_TYPE.SYSTEM;
            }

            Init();
            dataGridView.EndEdit();
            DataGridViewFocusing = dataGridView;
            FocusingDGVName = dataGridView.Name;
            CellRow = dataGridView.CurrentCell.RowIndex;
            CellColumn = dataGridView.CurrentCell.ColumnIndex;
            TargetText = text;
            return KEYBOARD_TYPE.MICRO;
        }

        /// <summary>
        /// 显示键盘窗体
        /// </summary>
        /// <param name="InitPoint"></param>
        public void ShowMicroKey(Point InitPoint)
        {
            if (this.WinFormMicroKeyboard != null)
            {
                WinFormMicroKeyboard.Dispose();
                Thread.Sleep(5);
            }
            WinFormMicroKeyboard = new FormMicroKeyboard();
            WinFormMicroKeyboard.Paint += new PaintEventHandler(WinFormMicroKeyboard.OnPaint);
            WinFormMicroKeyboard.Show();
            WinFormMicroKeyboard.TopMost = true;

            int DiffX = (InitPoint.X + WinFormMicroKeyboard.Size.Width) - (TopParentConSize.Width + TopParentConLocal.X);
            if (DiffX > 0)
            {
                InitPoint.X = InitPoint.X - DiffX - 40;
            }

            int DiffY = (InitPoint.Y + WinFormMicroKeyboard.Size.Height) - (TopParentConSize.Height + TopParentConLocal.Y);
            if (DiffY > 0)
            {
                int y = InitPoint.Y - WinFormMicroKeyboard.Size.Height - 40;
                if (y >= TopParentConLocal.Y)
                {
                    InitPoint.Y = y;
                }
            }

            WinFormMicroKeyboard.Location = InitPoint;
            TexeBoxView = WinFormMicroKeyboard.textBoxView;
            TexeBoxView.Text = TargetText;
            TexeBoxView.SelectionStart = TargetText.Length;

            if (kbpr != null && !kbpr.HasExited)
            {
                kbpr.Kill();
            }
        }

        /// <summary>
        /// 关闭、回收键盘窗体
        /// </summary>
        public void CloseMicroKey()
        {
            if (WinFormMicroKeyboard != null)
            {
                WinFormMicroKeyboard.Close();
                WinFormMicroKeyboard.Dispose();
            }

            Init();
        }

        /// <summary>
        /// 关闭、回收键盘窗体
        /// </summary>
        public void CloseMicroKey(object sender, DataGridViewCellEventArgs e)
        {
            if (WinFormMicroKeyboard != null)
            {
                WinFormMicroKeyboard.Close();
                WinFormMicroKeyboard.Dispose();
            }

            Init();
        }

        /// <summary>
        /// 键盘增加字符事件的方法
        /// </summary>
        /// <param name="addText"></param>
        public void AddChar(string addText)
        {
            int Len = TexeBoxView.Text.Length;
            int startIndex = TexeBoxView.SelectionStart;
            int LenRemove = TexeBoxView.SelectionLength;

            if (startIndex >= 0 && LenRemove > 0)
            {
                TexeBoxView.Text = TexeBoxView.Text.Substring(0, startIndex)
                        + TexeBoxView.Text.Substring(startIndex + LenRemove, Len - startIndex - LenRemove);
                TexeBoxView.Select(startIndex, 0);
            }

            Len = TexeBoxView.Text.Length;
            TexeBoxView.Text = TexeBoxView.Text.Substring(0, startIndex) + addText + TexeBoxView.Text.Substring(startIndex, Len - startIndex);
            TexeBoxView.Select(startIndex + 1, 0);
            TexeBoxView.Focus();
        }

        /// <summary>
        /// 键盘删除字符事件的方法
        /// </summary>
        public void RemoveChar()
        {
            int Len = TexeBoxView.Text.Length;
            int startIndex = TexeBoxView.SelectionStart;
            int LenRemove = TexeBoxView.SelectionLength;

            if (Len <= 0 || (startIndex == 0 && LenRemove == 0))
            {

            }
            else if (Len == 1)
            {
                TexeBoxView.Text = String.Empty;
            }
            else
            {

                if (startIndex >= 0 && LenRemove > 0)
                {
                    TexeBoxView.Text = TexeBoxView.Text.Substring(0, startIndex)
                            + TexeBoxView.Text.Substring(startIndex + LenRemove, Len - startIndex - LenRemove);
                    TexeBoxView.Select(startIndex, 0);
                }
                else
                {
                    TexeBoxView.Text = TexeBoxView.Text.Substring(0, startIndex - 1)
                        + TexeBoxView.Text.Substring(startIndex, Len - startIndex); ;
                    TexeBoxView.Select(startIndex - 1, 0);
                }

            }

            TexeBoxView.Focus();
        }

        /// <summary>
        /// 键盘清空字符事件的方法
        /// </summary>
        public void ClearText()
        {
            TexeBoxView.Text = String.Empty;

            TexeBoxView.Focus();
        }

        /// <summary>
        /// 键盘移动光标在文本框内位置事件的方法
        /// </summary>
        /// <param name="dir"></param>
        public void TextBoxCursorMove(CursorMoveDir dir)
        {
            int startIndex = TexeBoxView.SelectionStart;
            int Len = TexeBoxView.Text.Length;

            if (Len > 0)
            {
                switch (dir)
                {
                    case CursorMoveDir.Left:
                        if (startIndex > 0)
                        {
                            startIndex--;
                        }
                        break;
                    case CursorMoveDir.Right:
                        if (startIndex < Len)
                        {
                            startIndex++;
                        }
                        break;
                }
            }

            TexeBoxView.Select(startIndex, 0);
            TexeBoxView.Focus();
        }

        /// <summary>
        /// 更新目标TextBox的字符串
        /// </summary>
        public void UpdateText()
        {
            string text = TexeBoxView.Text;

            if (TexeBoxFocusing != null)
            {
                TexeBoxFocusing.Invoke(new Action(() =>
                {
                    TexeBoxFocusing.Text = text;
                    TexeBoxFocusing.SelectionStart = text.Length;
                }));
            }
            if (NumericUpDownFocusing != null)
            {
                NumericUpDownFocusing.Invoke(new Action(() =>
                {
                    NumericUpDownFocusing.Text = text;
                }));
            }
            if (DataGridViewFocusing != null)
            {
                DataGridViewFocusing.Invoke(new Action(() =>
                {
                    DataGridViewFocusing.CurrentCell.Value = text;
                    DataGridViewFocusing.Refresh();
                }));
            }


            CloseMicroKey();
        }
    }

    public enum KEYBOARD_TYPE
    {
        None = 0,
        MICRO,
        SYSTEM
    }
}
