using BrowApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSP.SystemData
{
    public class ParmData:BaseData
    {

        public ParmData() { }
        private string CodePath;
        public string SetCode
        {
            get => CodePath;
            set
            {
                CodePath = value;
            }
        }

        public struct PosType
        {
            public double Xpos;
            public double Ypos;
            public double Zpos;
        }

        /// <summary>
        /// Mark坐标
        /// </summary>
        public PosType Mak1Pos;
        /// <summary>
        /// Mark2坐标
        /// </summary>
        public PosType Mak2Pos;
        /// <summary>
        /// BomMark1坐标
        /// </summary>
        public PosType BMak1Pos;
        /// <summary>
        /// BomMark2坐标
        /// </summary>
        public PosType BMak2Pos;
        /// <summary>
        /// 拼版偏移X
        /// </summary>
        public double PbXoffset;
        /// <summary>
        /// 拼版偏移Y
        /// </summary>
        public double PbYoffset;

        /// <summary>
        /// 整体补偿X
        /// </summary>
        public double Offset_X;
        /// <summary>
        /// 整体补偿Y
        /// </summary>
        public double Offset_Y;
        /// <summary>
        /// Pcb长
        /// </summary>
        public double PcbLong;
        /// <summary>
        /// pcb宽
        /// </summary>
        public double PcbHight;
        /// <summary>
        /// 高度补偿
        /// </summary>
        public double Hoffset = 0;
        private void IsFilePath(string path)
        {
            String fileDirectory = Path.GetDirectoryName(path);
            if (!Directory.Exists(fileDirectory)) Directory.CreateDirectory(fileDirectory);
        }
        public void Save()
        {
            string SavePath = Global.GlabPath + "/Recipe" + "/" + CodePath + "/ParmData.Par";
            IsFilePath(SavePath);
            this.Save(SavePath, this);
        }
        public bool Read(ref ParmData calib)
        {
            try
            {
                string SavePath=  Global.GlabPath + "/Recipe"+ "/" + CodePath + "/ParmData.Par";
                String fileDirectory = Path.GetDirectoryName(SavePath);
                if (!Directory.Exists(fileDirectory)) { return false; }
                calib = this.Read<ParmData>(SavePath);
                return true;
            }
            catch (Exception ex) { APP.Log.W_Log(ex.ToString()); return false; }
            
        }
    }
}
