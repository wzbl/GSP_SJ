using BorwinAnalyse.BaseClass;
using ComponentFactory.Krypton.Toolkit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BorwinAnalyse.Model;
using BrowApp.Language;

namespace BorwinAnalyse.UCControls
{
    public partial class UCAnalyseSet : UserControl
    {

        public string RuleName = "";

        public UCAnalyseSet()
        {
            InitializeComponent();
            this.components = new System.ComponentModel.Container();
            //设置窗体的双缓冲
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer
            | ControlStyles.ResizeRedraw
            | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            Dock = DockStyle.Fill;
            this.Load += UCAnalyseSet_Load;
        }


        public UCAnalyseSet(string ruleName) : this()
        {
            comBomRule.Items.Clear();
            for (int i = 0; i < CommonAnalyse.Instance.Rules.Count; i++)
            {
                comBomRule.Items.Add(CommonAnalyse.Instance.Rules[i]);
            }
            comBomRule.Text = ruleName;
        }

        private List<List<string>> SepartorDesr = new List<List<string>>();

        private void UCAnalyseSet_Load(object sender, EventArgs e)
        {
            InitUI();
            UpdataLanguage();
            MicroKeyManager.Instance.AddEventToControl(this);
        }

        public void UpdataLanguage()
        {
            SepartorDesr.Clear();
            SepartorDesr = GetSeparatorsDescriptions(CommonAnalyse.Instance.Separators);
            ShowSeparatorDescrption();
        }

        private void InitUI()
        {
            IsSubstitutionRules.Checked = CommonAnalyse.Instance.IsSubstitutionRules;
            for (int i = 0; i < CommonAnalyse.Instance.SubstitutionRules.Count; i++)
            {
                SubstitutionRules substitutionRules = CommonAnalyse.Instance.SubstitutionRules[i];
                dataGridRule1.Rows.Add(
                    substitutionRules.FindContent,
                    substitutionRules.Replace,
                    substitutionRules.Enable,
                    substitutionRules.Is_Case_sensitive,
                    substitutionRules.Is_Full_half_width,
                    substitutionRules.Remark
                    );
            }

            IsSeparator.Checked = CommonAnalyse.Instance.IsSeparator;
            for (int i = 0; i < CommonAnalyse.Instance.Separators.Count; i++)
            {
                Separator separator = CommonAnalyse.Instance.Separators[i];
                dataGridRule2.Rows.Add(
                   separator.Enable,
                   separator.Acsii,
                   separator.Illustrate
                   );
            }

            SepartorDesr.Clear();
            SepartorDesr = GetSeparatorsDescriptions(CommonAnalyse.Instance.Separators);
            ShowSeparatorDescrption();

            for (int i = 0; i < CommonAnalyse.Instance.GradeChanges.Count; i++)
            {
                GradeChange GradeChange = CommonAnalyse.Instance.GradeChanges[i];
                dataGridRule3.Rows.Add(
                   GradeChange.Grade,
                   GradeChange.Percent
                   );
            }

            for (int i = 0; i < CommonAnalyse.Instance.GradeChangesCustRes.Count; i++)
            {
                GradeChange GradeChange = CommonAnalyse.Instance.GradeChangesCustRes[i];
                dataGridRuleCustResGrade.Rows.Add(
                   GradeChange.Grade,
                   GradeChange.Percent,
                   GradeChange.PercentLow,
                   GradeChange.DiffUpper,
                   GradeChange.DiffLower
                   );
            }

            for (int i = 0; i < CommonAnalyse.Instance.GradeChangesCustCap.Count; i++)
            {
                GradeChange GradeChange = CommonAnalyse.Instance.GradeChangesCustCap[i];
                dataGridRuleCustCapGrade.Rows.Add(
                   GradeChange.Grade,
                   GradeChange.Percent,
                   GradeChange.PercentLow,
                   GradeChange.DiffUpper,
                   GradeChange.DiffLower
                   );
            }

            txtRes.Text = CommonAnalyse.Instance.Resistance;
            txtCAP.Text = CommonAnalyse.Instance.Capacitance;
            txtResUnit.Text = CommonAnalyse.Instance.ResistanceUnit;
            txtCapUnit.Text = CommonAnalyse.Instance.CapacitanceUnit;
            txtSize.Text = CommonAnalyse.Instance.ComponentSpecifications;

            IsDeleteString.Checked = CommonAnalyse.Instance.IsDeleteString;
            txtPrefixNumber.Text = CommonAnalyse.Instance.PrefixNumber.ToString();
            txtSuffixNumber.Text = CommonAnalyse.Instance.SuffixNumber.ToString();

            IsIntermediateUnit.Checked = CommonAnalyse.Instance.IsIntermediateUnit;
            IsTitleRow.Checked = CommonAnalyse.Instance.IsTitleRow;

            IsGrade_ON_NO_Find.Checked = CommonAnalyse.Instance.IsGrade_ON_NO_Find;
            txtResGrade_ON_NO_Find.Text = CommonAnalyse.Instance.ResGrade_ON_NO_Find;
            txtCapGrade_ON_NO_Find.Text = CommonAnalyse.Instance.CapGrade_ON_NO_Find;

            IsValueContainsGrade.Checked = CommonAnalyse.Instance.IsValueContainsGrade;
            IsResDefaultUnit.Checked = CommonAnalyse.Instance.IsResDefaultUnit;
            txtResDefaultUnit.Text = CommonAnalyse.Instance.ResDefaultUnit;
            IsIdentifyingDigits.Checked = CommonAnalyse.Instance.IsIdentifyingDigits;

            IsExcludeContext.Checked = CommonAnalyse.Instance.IsExcludeContext;
            IsSearchGradeByPos.Checked = CommonAnalyse.Instance.IsSearchGradeByPos;
            IsUseCustomerResGrade.Checked = CommonAnalyse.Instance.IsUseCustomerResGrade;
            IsUseCustomerCapGrade.Checked = CommonAnalyse.Instance.IsUseCustomerCapGrade;
            txtExcludeContext.Text = CommonAnalyse.Instance.ExcludeContext;
            txtGradePos.Text = CommonAnalyse.Instance.txtGradePos;
            txtStrAfterGrade.Text = CommonAnalyse.Instance.txtStrAfterGrade;

            IsHadReplaceCode.Checked = CommonAnalyse.Instance.IsHadReplaceCode;
            IsHadReplaceCodeCol.Checked = CommonAnalyse.Instance.IsHadReplaceCodeCol;
            IsSameLocationRule.Checked = CommonAnalyse.Instance.IsSameLocationRule;
            txtReplaceCodeSep.Text = CommonAnalyse.Instance.ReplaceCodeSeparator;

            IsMergeDescription.Checked = CommonAnalyse.Instance.IsMergeDescription;

        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Save()
        {
            CommonAnalyse.Instance.SubstitutionRules.Clear();
            CommonAnalyse.Instance.IsSubstitutionRules = IsSubstitutionRules.Checked;
            for (int i = 0; i < dataGridRule1.RowCount; i++)
            {
                if (dataGridRule1.Rows[i].IsNewRow)
                {
                    continue;
                }
                SubstitutionRules substitutionRules = new SubstitutionRules();
                substitutionRules.FindContent = dataGridRule1.Rows[i].Cells[0].AccessibilityObject.Value.ToString().Contains("null") ? "" : dataGridRule1.Rows[i].Cells[0].AccessibilityObject.Value.ToString();
                substitutionRules.Replace = dataGridRule1.Rows[i].Cells[1].AccessibilityObject.Value.ToString().Contains("null") ? "" : dataGridRule1.Rows[i].Cells[1].AccessibilityObject.Value.ToString();
                substitutionRules.Enable = bool.Parse(dataGridRule1.Rows[i].Cells[2].AccessibilityObject.Value.ToString());
                substitutionRules.Is_Case_sensitive = bool.Parse(dataGridRule1.Rows[i].Cells[3].AccessibilityObject.Value);
                substitutionRules.Is_Full_half_width = bool.Parse(dataGridRule1.Rows[i].Cells[4].AccessibilityObject.Value.ToString());
                substitutionRules.Remark = dataGridRule1.Rows[i].Cells[5].AccessibilityObject.Value.ToString().Contains("null") ? "" : dataGridRule1.Rows[i].Cells[5].AccessibilityObject.Value.ToString(); ;
                CommonAnalyse.Instance.SubstitutionRules.Add(substitutionRules);
            }
            CommonAnalyse.Instance.Separators.Clear();
            CommonAnalyse.Instance.IsSeparator = IsSeparator.Checked;
            for (int i = 0; i < dataGridRule2.RowCount; i++)
            {
                if (dataGridRule2.Rows[i].IsNewRow)
                {
                    continue;
                }
                Separator separator = new Separator();
                separator.Enable = bool.Parse(dataGridRule2.Rows[i].Cells[0].AccessibilityObject.Value.ToString());
                separator.Acsii = dataGridRule2.Rows[i].Cells[1].AccessibilityObject.Value.ToString().Contains("null") ? "" : dataGridRule2.Rows[i].Cells[1].AccessibilityObject.Value.ToString();
                separator.Illustrate = dataGridRule2.Rows[i].Cells[2].AccessibilityObject.Value.ToString().Contains("null") ? "" : dataGridRule2.Rows[i].Cells[2].AccessibilityObject.Value.ToString();
                CommonAnalyse.Instance.Separators.Add(separator);
            }
            CommonAnalyse.Instance.GradeChanges.Clear();
            for (int i = 0; i < dataGridRule3.RowCount; i++)
            {
                if (dataGridRule3.Rows[i].IsNewRow)
                {
                    continue;
                }
                GradeChange GradeChange = new GradeChange();
                GradeChange.Grade = dataGridRule3.Rows[i].Cells[0].AccessibilityObject.Value.ToString();
                GradeChange.Percent = dataGridRule3.Rows[i].Cells[1].AccessibilityObject.Value.ToString();
                CommonAnalyse.Instance.GradeChanges.Add(GradeChange);
            }
            CommonAnalyse.Instance.GradeChangesCustRes.Clear();
            for (int i = 0; i < dataGridRuleCustResGrade.RowCount; i++)
            {
                if (dataGridRuleCustResGrade.Rows[i].IsNewRow)
                {
                    continue;
                }
                GradeChange GradeChange = new GradeChange();
                GradeChange.Grade = dataGridRuleCustResGrade.Rows[i].Cells[0].AccessibilityObject.Value.ToString();
                GradeChange.Percent = dataGridRuleCustResGrade.Rows[i].Cells[1].AccessibilityObject.Value.ToString();
                GradeChange.PercentLow = dataGridRuleCustResGrade.Rows[i].Cells[2].AccessibilityObject.Value.ToString();
                GradeChange.DiffUpper = dataGridRuleCustResGrade.Rows[i].Cells[3].AccessibilityObject.Value.ToString();
                GradeChange.DiffLower = dataGridRuleCustResGrade.Rows[i].Cells[4].AccessibilityObject.Value.ToString();
                CommonAnalyse.Instance.GradeChangesCustRes.Add(GradeChange);
            }
            CommonAnalyse.Instance.GradeChangesCustCap.Clear();
            for (int i = 0; i < dataGridRuleCustCapGrade.RowCount; i++)
            {
                if (dataGridRuleCustCapGrade.Rows[i].IsNewRow)
                {
                    continue;
                }
                GradeChange GradeChange = new GradeChange();
                GradeChange.Grade = dataGridRuleCustCapGrade.Rows[i].Cells[0].AccessibilityObject.Value.ToString();
                GradeChange.Percent = dataGridRuleCustCapGrade.Rows[i].Cells[1].AccessibilityObject.Value.ToString();
                GradeChange.PercentLow = dataGridRuleCustCapGrade.Rows[i].Cells[2].AccessibilityObject.Value.ToString();
                GradeChange.DiffUpper = dataGridRuleCustCapGrade.Rows[i].Cells[3].AccessibilityObject.Value.ToString();
                GradeChange.DiffLower = dataGridRuleCustCapGrade.Rows[i].Cells[4].AccessibilityObject.Value.ToString();
                CommonAnalyse.Instance.GradeChangesCustCap.Add(GradeChange);
            }
            CommonAnalyse.Instance.Resistance = txtRes.Text;
            CommonAnalyse.Instance.Capacitance = txtCAP.Text;
            CommonAnalyse.Instance.ResistanceUnit = txtResUnit.Text;
            CommonAnalyse.Instance.CapacitanceUnit = txtCapUnit.Text;
            CommonAnalyse.Instance.ComponentSpecifications = txtSize.Text;
            CommonAnalyse.Instance.IsDeleteString = IsDeleteString.Checked;
            CommonAnalyse.Instance.PrefixNumber = int.Parse(txtPrefixNumber.Text);
            CommonAnalyse.Instance.SuffixNumber = int.Parse(txtSuffixNumber.Text);
            CommonAnalyse.Instance.IsIntermediateUnit = IsIntermediateUnit.Checked;
            CommonAnalyse.Instance.IsGrade_ON_NO_Find = IsGrade_ON_NO_Find.Checked;
            CommonAnalyse.Instance.ResGrade_ON_NO_Find = txtResGrade_ON_NO_Find.Text;
            CommonAnalyse.Instance.CapGrade_ON_NO_Find = txtCapGrade_ON_NO_Find.Text;
            CommonAnalyse.Instance.IsValueContainsGrade = IsValueContainsGrade.Checked;
            CommonAnalyse.Instance.IsResDefaultUnit = IsResDefaultUnit.Checked;
            CommonAnalyse.Instance.ResDefaultUnit = txtResDefaultUnit.Text.Trim();
            CommonAnalyse.Instance.IsIdentifyingDigits = IsIdentifyingDigits.Checked;
            CommonAnalyse.Instance.IsExcludeContext = IsExcludeContext.Checked;
            CommonAnalyse.Instance.IsUseCustomerCapGrade = IsUseCustomerCapGrade.Checked;
            CommonAnalyse.Instance.IsUseCustomerResGrade = IsUseCustomerResGrade.Checked;
            CommonAnalyse.Instance.ExcludeContext = txtExcludeContext.Text.Trim();
            CommonAnalyse.Instance.txtStrAfterGrade = txtStrAfterGrade.Text.Trim();
            CommonAnalyse.Instance.txtGradePos = txtGradePos.Text.Trim();
            CommonAnalyse.Instance.IsSearchGradeByPos = IsSearchGradeByPos.Checked;
            CommonAnalyse.Instance.IsTitleRow = IsTitleRow.Checked;
            CommonAnalyse.Instance.IsHadReplaceCode = IsHadReplaceCode.Checked;
            CommonAnalyse.Instance.IsHadReplaceCodeCol = IsHadReplaceCodeCol.Checked;
            CommonAnalyse.Instance.IsSameLocationRule = IsSameLocationRule.Checked;
            CommonAnalyse.Instance.ReplaceCodeSeparator = txtReplaceCodeSep.Text;
            CommonAnalyse.Instance.IsMergeDescription = IsMergeDescription.Checked;
            CommonAnalyse.Instance.Save(comBomRule.Text);
            MessageBox.Show("保存成功".tr());
        }

        private void kryptonDataGridView1_MouseEnter(object sender, EventArgs e)
        {

        }

        private void kryptonDataGridView1_MouseLeave(object sender, EventArgs e)
        {

        }

        private void ShowMenu(Control c, KryptonContextMenu kcm)
        {
            kcm.Show(c.RectangleToScreen(c.ClientRectangle), KryptonContextMenuPositionH.Left,
                     KryptonContextMenuPositionV.Top);
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            DeleteSubstitutionRules();
        }

        /// <summary>
        /// 菜单删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DeleteGradeChange();
        }

        private void DeleteGradeChange()
        {
            int count = dataGridRule3.SelectedRows.Count;
            for (int i = 0; i < count;)
            {
                if (!dataGridRule3.SelectedRows[i].IsNewRow)
                {
                    dataGridRule3.Rows.Remove(dataGridRule3.SelectedRows[i]);
                    count = dataGridRule3.SelectedRows.Count;
                }
                else
                {
                    dataGridRule3.SelectedRows[i].Selected = false;
                }

            }
        }

        /// <summary>
        /// 删除字符替换规则
        /// </summary>
        private void DeleteSubstitutionRules()
        {
            int count = dataGridRule1.SelectedRows.Count;
            for (int i = 0; i < count;)
            {
                if (!dataGridRule1.SelectedRows[i].IsNewRow)
                {
                    dataGridRule1.Rows.Remove(dataGridRule1.SelectedRows[i]);
                    count = dataGridRule1.SelectedRows.Count;
                }
                else
                {
                    dataGridRule1.SelectedRows[i].Selected = false;
                }

            }
        }

        /// <summary>
        /// 删除分隔符
        /// </summary>
        private void DeleteSeparator()
        {
            int count = dataGridRule2.SelectedRows.Count;
            for (int i = 0; i < count;)
            {
                if (!dataGridRule2.SelectedRows[i].IsNewRow)
                {
                    dataGridRule2.Rows.Remove(dataGridRule2.SelectedRows[i]);
                    count = dataGridRule2.SelectedRows.Count;
                }
                else
                {
                    dataGridRule2.SelectedRows[i].Selected = false;
                }

            }
        }

        private void kryptonButton2_Click(object sender, EventArgs e)
        {
            DeleteSeparator();
        }


        /// <summary>
        /// 获取分隔符的描述
        /// </summary>
        /// <param name="separators"></param>
        /// <returns></returns>
        private List<List<string>> GetSeparatorsDescriptions(List<Separator> separators)
        {
            List<List<string>> descriptions = new List<List<string>>();
            DataTable dt = NOPIHelperEX.ExcelToDataTable("Ini/ASCII.xlsx", true);

            int colCN = 6;
            int colEN = 7;
            int colDec = 0;

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if (dt.Rows[0].ItemArray[i].ToString().Contains("中文"))
                {
                    colCN = i;
                }
                if (dt.Rows[0].ItemArray[i].ToString().Contains("英文"))
                {
                    colEN = i;
                }
                if (dt.Rows[0].ItemArray[i].ToString().Contains("十进制"))
                {
                    colDec = i;
                }

            }

            foreach (Separator separator in separators)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (separator.Acsii == row.ItemArray[4].ToString())
                    {
                        List<string> list = new List<string>();
                        list.Add(row.ItemArray[colCN].ToString());
                        list.Add(row.ItemArray[colEN].ToString());
                        list.Add(row.ItemArray[colDec].ToString());
                        descriptions.Add(list);
                        break;
                    }
                }
            }

            dt.Dispose();
            return descriptions;
        }

        private void ShowSeparatorDescrption()
        {
            for (int i = 0; i < dataGridRule2.Rows.Count - 1; i++)
            {
                if (i >= SepartorDesr.Count)
                {
                    return;
                }

                dataGridRule2.Rows[i].Cells[2].Value = SepartorDesr[i][2];
            }
        }

        private void btnAddRule_Click(object sender, EventArgs e)
        {

        }

        private void btnLoadRule_Click(object sender, EventArgs e)
        {
            CommonAnalyse.Instance.Load(comBomRule.Text);
        }
    }
}
