using BrowApp;
using BrowApp.Language;
using BrowApp.MessageTip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GSP.Mes.MesData;

namespace GSP.Mes
{
    public partial class MESControl : UserControl
    {

       
        public MESControl()
        {
            InitializeComponent();
            Global.GlobRefEvent += objfom;
        }

        private void fomobj()
        {
            Global.mesConfig.Url_1 = URL_1txt.Text;
            Global.mesConfig.Url_2 = URL_2txt.Text;
            Global.mesConfig.Url_3 = URL_3txt.Text;
            Global.mesConfig.sourceFlag = sourceFlag_txt.Text;
            Global.mesConfig.software=software_txt.Text;
            Global.mesConfig.station=station_txt.Text;
            Global.mesConfig.equipmentNo=equipmentNo_txt.Text;
            Global.mesConfig.Ulr1_ck = Url1_ck.Checked;
            Global.mesConfig.Ulr2_ck = Url2_ck.Checked;
            Global.mesConfig.Ulr3_ck = Url3_ck.Checked;
        }
        private void objfom()
        {
            URL_1txt.Text = Global.mesConfig.Url_1;
            URL_2txt.Text = Global.mesConfig.Url_2;
            URL_3txt.Text = Global.mesConfig.Url_3;
            sourceFlag_txt.Text = Global.mesConfig.sourceFlag;
            software_txt.Text = Global.mesConfig.software;
            station_txt.Text = Global.mesConfig.station;
            equipmentNo_txt.Text = Global.mesConfig.equipmentNo;
            Url1_ck.Checked = Global.mesConfig.Ulr1_ck;
            Url2_ck.Checked = Global.mesConfig.Ulr2_ck;
            Url3_ck.Checked = Global.mesConfig.Ulr3_ck;
        }

        private void SaveMes_btn_Click(object sender, EventArgs e)
        {
            try
            {
                fomobj();
                Global.mesConfig.Save();
                FloatingTip.ShowOk("保存MES参数成功".tr());
            }
            catch
            {
                FloatingTip.ShowError("保存MES参数失败".tr());
            }
        }

        private void UpData1_Click(object sender, EventArgs e)
        {
            try
            {
                BrowLib.FileClass.RwIni iniFile = new BrowLib.FileClass.RwIni(Global.Systemdata.CfgFile, "", Encoding.Default);
                string UserID = iniFile.Read("AutoFAI", "UserID", "AutoFAI", "UTF-8");
                string Updata;
                string Result = new MesData().UpDataAPI_S(UserID, "RUN", out Updata);
                Updata_txt.Text = Updata;
                mResult_txt.Text = Result;
                APP.Log.M_Log("Mes上传"+Updata);
            }
            catch { }
        }

        private void UpData2_Click(object sender, EventArgs e)
        {
            try
            {
                BrowLib.FileClass.RwIni iniFile = new BrowLib.FileClass.RwIni(Global.Systemdata.CfgFile, "", Encoding.Default);
                string UserID = iniFile.Read("AutoFAI", "UserID", "AutoFAI", "UTF-8");
                string TokenID = iniFile.Read("AutoFAI", "TokenID", "AutoFAI", "UTF-8");
                string Updata;
                string Result = new MesData().UpDataAPI_Alm(UserID, "YC0001", "BJ - ZJYJ001","异常", TokenID,out Updata);
                Updata_txt.Text = Updata;
                mResult_txt.Text = Result;
                
            }
            catch { }
        }

        private void UpData3_Click(object sender, EventArgs e)
        {
            try
            {
                BrowLib.FileClass.RwIni iniFile = new BrowLib.FileClass.RwIni(Global.Systemdata.CfgFile, "", Encoding.Default);
                string UserID = iniFile.Read("AutoFAI", "UserID", "AutoFAI", "UTF-8");
                List<ParmType> parms = new List<ParmType>();
                parms.Add(new ParmType
                {
                    param = "TWO",
                    value = "100",
                    unitNo = "",
                    time= DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                });
                string Updata;
                string Result = new MesData().UpDataAPI_Pam(UserID, parms,out Updata);
                Updata_txt.Text = Updata;
                mResult_txt.Text = Result;
            }
            catch { }
        }
    }
}
