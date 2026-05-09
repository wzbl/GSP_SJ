using BrowLib.FileClass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GSP.SystemData
{
   public abstract class BaseData
   {
        CDataXml CDataXml = new CDataXml();
        DataJson dataJson = new DataJson();
       
        protected virtual void Save(string SavePath,object obj)
        {
            dataJson.Serializer<object>(SavePath, this);
        }
        protected virtual T Read<T>(string SavePath) where T : BaseData 
        {
           return (T)dataJson.DeserializeFile<T>(SavePath);
        }
    }
}
