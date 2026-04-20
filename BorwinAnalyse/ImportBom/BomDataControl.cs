using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorwinAnalyse.ImportBom
{
    public class BomDataControl
    {
        /// <summary>
        /// 物料代码
        /// </summary>
        public List<string> MaterialCodes = new List<string>();

        /// <summary>
        /// 物料描述
        /// </summary>
        public List<string> MaterialDescriptions = new List<string>();

        /// <summary>
        /// 用量
        /// </summary>
        public List<string> QTYs =new List<string>();


        /// <summary>
        /// 元件位置
        /// </summary>
        public List<string> Positions = new List<string>();

        /// <summary>
        /// 替代料
        /// </summary>
        public List<string> ReplaceMaterials = new List<string>();

    }
}
