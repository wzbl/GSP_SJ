using ComponentFactory.Krypton.Toolkit;
using SqlHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP_SJ
{
    public class Global
    {
        public static Action<View_Eng_Program> ProguceItem;
        public static Action<P_Man_Report_Search_Result> ReportItem;
        public static Action<string> RefreshReport;

        /// <summary>
        /// 选择组件
        /// </summary>
        public static Action<string> SelectComponent;

        /// <summary>
        /// 绑定数据
        /// </summary>
        /// <param name="dgv"></param>
        /// <param name="colIndex"></param>
        /// <param name="datas"></param>
        public static void DgvColBindData(KryptonDataGridView dgv, int colIndex, List<string> datas)
        {
            try
            {
                (dgv.Columns[colIndex] as KryptonDataGridViewComboBoxColumn).Items.Clear();
                for (int i = 0; i < datas.Count; i++)
                {
                    (dgv.Columns[colIndex] as KryptonDataGridViewComboBoxColumn).Items.Add(datas[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public static void DgvColBindData(KryptonDataGridView dgv, int rowIndex, int colIndex, List<string> datas)
        {
            try
            {
                DataGridViewComboBoxCell comboBoxCell = (dgv.Rows[rowIndex].Cells[colIndex] as DataGridViewComboBoxCell);
                comboBoxCell.Items.Clear();
                for (int i = 0; i < datas.Count; i++)
                {
                    comboBoxCell.Items.Add(datas[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public static string GetDGVCellValue(KryptonDataGridView dgv, int rowIndex, int colIndex)
        {
            try
            {
                return dgv.Rows[rowIndex].Cells[colIndex].Value.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static Sys_User User = null;
    }
}
