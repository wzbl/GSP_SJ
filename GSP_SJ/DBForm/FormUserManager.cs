using BrowApp.Language;
using ComponentFactory.Krypton.Toolkit;
using HalconDotNet;
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

namespace GSP_SJ.DBForm
{
    public partial class FormUserManager : KryptonForm
    {
        public FormUserManager()
        {
            InitializeComponent();
            this.Load += FormUserManager_Load;
        }

        private void FormUserManager_Load(object sender, EventArgs e)
        {
            dgvUser.CellClick += dgvUser_CellClick;
            Refreshdgv();
        }

        List<Sys_User> systems;
        public void Refreshdgv()
        {
            dgvUser.Rows.Clear();
            systems = SQLDataControl.GetUser();
            for (int i = 0; i < systems.Count; i++)
            {
                dgvUser.Rows.Add(
                    systems[i].UserCode,
                    systems[i].UserName,
                    systems[i].RoleCode,
                    systems[i].Status,
                    systems[i].Creator,
                    systems[i].CreationDate,
                    systems[i].Modifier,
                    systems[i].ModificationDate);
            }

        }


        private void dgvUser_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                dgvUser.Rows[e.RowIndex].Selected = true;
        }
        Sys_User AddUser = null;
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (systems == null)
                return;
            if (dgvUser.Rows[dgvUser.Rows.Count - 1].Cells[0].Value == null)
            {
                return;
            }

            if (systems.Where(x => x.UserCode == dgvUser.Rows[dgvUser.Rows.Count - 1].Cells[0].Value.ToString()).ToList().Count == 0)
            {
                return;
            }
            AddUser = new Sys_User();
            dgvUser.Rows.Add
                (
                "",
                "",
                "",
                "NORMAL");
            dgvUser.Rows[dgvUser.Rows.Count - 1].Cells[0].ReadOnly = false;
        }

        private void btnSettingPassword_Click(object sender, EventArgs e)
        {
            if (dgvUser.SelectedRows.Count > 0)
            {
                if (dgvUser.SelectedRows[0].Cells[0].Value == null || string.IsNullOrEmpty(dgvUser.SelectedRows[0].Cells[0].Value.ToString()))
                {
                     BrowApp.APP.Tip.ShowTip(0, "提示".tr(), "请填写账号".tr(), "确定".tr());
                    return;
                }
                string userCode = dgvUser.SelectedRows[0].Cells[0].Value.ToString();
                if (AddUser != null)
                {
                    if (SQLDataControl.GetUser().Where(x => x.UserCode == dgvUser.SelectedRows[0].Cells[0].Value.ToString()).ToList().Count > 0)
                    {
                         BrowApp.APP.Tip.ShowTip(0, "提示".tr(), "该账号已存在".tr(), "确定".tr());
                        return;
                    }
                    else
                    {
                        FormSetPasswork formSetPasswork = new FormSetPasswork(userCode, 1);
                        if (formSetPasswork.ShowDialog() == DialogResult.OK)
                        {
                            AddUser.Password = formSetPasswork.password;
                        }
                    }
                }
                else
                {
                    FormSetPasswork formSetPasswork = new FormSetPasswork(userCode, 0);
                    if (formSetPasswork.ShowDialog() == DialogResult.OK)
                    {
                        AddUser.Password = formSetPasswork.password;
                    }
                }


            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvUser.SelectedRows.Count > 0)
            {
                if (dgvUser.SelectedRows[0].Cells[0].Value != null)
                {
                    string userCode = dgvUser.SelectedRows[0].Cells[0].Value.ToString();
                    SQLDataControl.DeleteUser(userCode);
                }
                dgvUser.Rows.RemoveAt(dgvUser.SelectedRows[0].Index);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (AddUser != null && string.IsNullOrEmpty(AddUser.Password))
            {
                 BrowApp.APP.Tip.ShowTip(1, "警告".tr(), "请设置密码".tr(), "确定".tr());
                return;
            }
            for (int i = 0; i < dgvUser.Rows.Count; i++)
            {
                if (dgvUser.Rows[i].Cells[0].Value == null || string.IsNullOrEmpty(dgvUser.Rows[i].Cells[0].Value.ToString()))
                {
                     BrowApp.APP.Tip.ShowTip(1, "警告".tr(), "请填写账号".tr(), "确定".tr());
                    return;
                }

                if (dgvUser.Rows[i].Cells[1].Value == null || string.IsNullOrEmpty(dgvUser.Rows[i].Cells[1].Value.ToString()))
                {
                     BrowApp.APP.Tip.ShowTip(1, "警告".tr(), "请填写用户名".tr(), "确定".tr());
                    return;
                }

                if (dgvUser.Rows[i].Cells[2].Value == null || string.IsNullOrEmpty(dgvUser.Rows[i].Cells[2].Value.ToString()))
                {
                     BrowApp.APP.Tip.ShowTip(1, "警告".tr(), "请填写角色".tr(), "确定".tr());
                    return;
                }

                if (dgvUser.Rows[i].Cells[3].Value == null || string.IsNullOrEmpty(dgvUser.Rows[i].Cells[3].Value.ToString()))
                {
                     BrowApp.APP.Tip.ShowTip(1, "警告".tr(), "请填写状态".tr(), "确定".tr());
                    return;
                }
                SQLDataControl.AddUser(dgvUser.Rows[i].Cells[0].Value.ToString(),
                dgvUser.Rows[i].Cells[1].Value.ToString(),
                AddUser == null ? "" : AddUser.Password,
                dgvUser.Rows[i].Cells[2].Value.ToString(),
                dgvUser.Rows[i].Cells[3].Value.ToString(),
               "test"
                );
            }
            dgvUser.Rows[dgvUser.Rows.Count - 1].Cells[0].ReadOnly = true;
            AddUser = null;
            Refreshdgv();
        }
    }
}
