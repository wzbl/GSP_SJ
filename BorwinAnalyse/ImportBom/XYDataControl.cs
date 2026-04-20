using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorwinAnalyse.ImportBom
{
    public class XYDataControl
    {
        /// <summary>
        /// 元件位置
        /// </summary>
        public List<string> Positions = new List<string>();

        /// <summary>
        /// X坐标
        /// </summary>
        public List<string> XPos = new List<string>();

        /// <summary>
        ///  Y坐标
        /// </summary>
        public List<string> YPos = new List<string>();


        /// <summary>
        /// 角度
        /// </summary>
        public List<string> Angles = new List<string>();

        /// <summary>
        /// 板面
        /// </summary>
        public List<string> Sides = new List<string>();
    }
}
