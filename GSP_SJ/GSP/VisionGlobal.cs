using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSP
{
    public class VisionGlobal
    {
        /// <summary>
        /// 坐标系平移量X
        /// </summary>
        public static double TranslationX;
        /// <summary>
        /// 坐标系平移量Y
        /// </summary>
        public static double TranslationY;

        /// <summary>
        /// 像素比
        /// </summary>
        public static double Pix;

        /// <summary>
        /// X偏移
        /// </summary>
        public static double MakDx;
        /// <summary>
        /// Y偏移
        /// </summary>
        public static double MakDy;
        /// <summary>
        /// 角度偏移
        /// </summary>
        public static double DAngle;

        /// <summary>
        /// 纠偏Mask1X位置
        /// </summary>
        public static double Mak1_X;
        /// <summary>
        /// 纠偏Mask1Y位置
        /// </summary>
        public static double Mak1_Y;
        /// <summary>
        /// 纠偏Mask2X位置
        /// </summary>
        public static double Mak2_X;
        /// <summary>
        /// 纠偏Mask2Y位置
        /// </summary>
        public static double Mak2_Y;

        public static string UpCailb_Obj = "UpCamObj";
        public static string DownCailb_Obj = "DownCamObj";

        public static double bMak1_X = Global.Parm.BMak1Pos.Xpos;
        public static double bMak1_Y = Global.Parm.BMak1Pos.Ypos;

        public static double bMak2_X = Global.Parm.BMak2Pos.Xpos;
        public static double bMak2_Y = Global.Parm.BMak2Pos.Ypos;

        public static double Cccd0ffset_X;
        public static double Cccd0ffset_Y;
    }
}
