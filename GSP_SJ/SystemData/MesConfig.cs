using BrowApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSP.SystemData
{
    public class MesConfig:BaseData
    {
        public MesConfig(string Path) { SavePath = Path;}

        private string SavePath;
        public MesConfig() { }
        public string Url_1 {  get; set; }
        public string Url_2 { get; set; }
        public string Url_3 { get; set; }
        public string sourceFlag {  get; set; }

        public string software { get; set; }
        public string equipmentNo { get; set; }

        public string station { get; set; }
        public bool Ulr1_ck { get; set; }
        public bool Ulr2_ck { get; set; }
        public bool Ulr3_ck { get; set; }

        public void Save()
        {
            SavePath = Global.GlabPath + "/Systemfile/MesConfig.dat";
            this.Save(SavePath, this);
        }
        public bool Read(ref MesConfig calib)
        {
            try
            {
                SavePath = Global.GlabPath + "/Systemfile/MesConfig.dat";
                calib = this.Read<MesConfig>(SavePath);
                return true;
            }
            catch (Exception ex) { APP.Log.W_Log(ex.ToString()); return false; }
        }
    }
}
