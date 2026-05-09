using BrowApp;
using BrowApp.Language;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSP.SystemData
{
    public class CalibData:BaseData
    {
        public CalibData() { }  
        public CalibData(string Path) { this.SavePath = Path; }

        [NonSerialized]
        private string SavePath;
        public class PosType
        {
            public double Xpos = 0;
            public double Ypos = 0;
            public double Zpos = 0;
        }   
        public class CalibType
        {
            public double Xpos=0;
            public double Ypos=0;
            public double Zpos = 0;
            public double Offect = 0;
            public double Pix = 0;
        }

        public class OffsetType
        {
           public string Name { get; set; }
            public string Angle { get; set; }
            public string OffsetX { get; set; }
            public string OffsetY { get; set; }
        }
        public class SizeType
        {
            public string Name { get; set; }
            public string Size { get; set; }
            public string Hight { get; set; }
            public string Length { get; set; }
            public string width { get; set; }
            public string Clipsize { get; set; }
        }

        public class CenterType
        {
            public double CenterX { get; set; }
            public double CenterY { get; set; }
            public double CenterdX { get; set; }
            public double CenterdY { get; set; }
        }


        /// <summary>
        /// 旋转中心
        /// </summary>
        public CenterType CenterData = new CenterType();
        /// <summary>
        /// 相机激光偏差X
        /// </summary>
        public double Laser_X;
        /// <summary>
        /// 相机激光偏差Y
        /// </summary>
        public double Laser_Y;
        /// <summary>
        /// 激光基准值
        /// </summary>
        public double LaserValue;
        /// <summary>
        /// 允许误差
        /// </summary>
        public double Allowablevalue;
        /// <summary>
        /// 激光位置
        /// </summary>
        public PosType LaserPos=new PosType();
        /// <summary>
        /// 相机位置
        /// </summary>
        public PosType CamPos=new PosType();
        /// <summary>
        /// 标定参数
        /// </summary>
        public CalibType CalibParm=new CalibType();
        /// <summary>
        /// 对针位置
        /// </summary>
        public PosType PinPos = new PosType();
        /// <summary>
        /// 开路清零位置
        /// </summary>
        public PosType ZeroPos = new PosType();
        /// <summary>
        /// 短路清零位置
        /// </summary>
        public PosType ZeroPos2 = new PosType();
        /// <summary>
        /// 相机Vs针参数
        /// </summary>
        public List<OffsetType> Offsets { get; set; }=new List<OffsetType>();
        /// /// <summary>
        /// 开口大小参数
        /// </summary>
        public List<SizeType> Sizes { get; set; } = new List<SizeType>();
        /// <summary>
        /// 测试数据
        /// </summary>
        public DataTable TPosTable { get; set; } = new DataTable();
       
        public void Save()
        {
            SavePath = Global.GlabPath + "/Systemfile/CalibData.dat";
            this.Save(SavePath, this);
        }
        public bool Read(ref CalibData calib)
        {
            try
            {
                SavePath = Global.GlabPath + "/Systemfile/CalibData.dat";
                calib = this.Read<CalibData>(SavePath);
                return true;
            }
            catch (Exception ex) { APP.Log.W_Log(ex.ToString()); return false; }
        }
    }
}
