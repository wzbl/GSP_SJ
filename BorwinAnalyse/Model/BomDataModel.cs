using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BorwinAnalyse.Model
{
    /// <summary>
    /// Bom数据
    /// </summary>
    public class BomDataModel
    {
        public string id;
        public string modelName;
        public string barCode;
        public string replaceCode;
        public string description;
        public string result;
        public string type;
        public string size;
        public string value;
        public string unit;
        public string grade;
        public string tapeType;         //exp1
        public string tapeWidth;        //exp2
        public string pitch;            //exp3
        public string judgeOCV;         //exp4
        public string exp5;
    }
}
