using BrowApp;
using GSP.Mes;
using GSP.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GSP
{
    public class MiddleLayer
    {
        public static UI_Prodution GetProdution;

        public static UI_Vision UI_Vision;

        public static UI_CalibForm UI_Calib;

        public static UI_Language UI_Language;

        public static UI_Program UI_Program;

        public static UI_SystemData UI_System;

        public static UI_MachineTest UI_MachineTest;

        public static MESControl UI_MESControl;

        public static void InitialProject()
        {
            GetProdution=new UI_Prodution();
            UI_Vision = new UI_Vision();
            UI_Vision.Dock = DockStyle.Fill;
            UI_Calib = new UI_CalibForm();
            UI_Calib.Dock = DockStyle.Fill;
            UI_Language = new UI_Language();
            UI_Language.Dock = DockStyle.Fill;
            UI_Program = new UI_Program();
            UI_Program.Dock = DockStyle.Fill;
            UI_System = new UI_SystemData();
            UI_System.Dock = DockStyle.Fill;
            UI_MachineTest=new UI_MachineTest();
            UI_MachineTest.Dock = DockStyle.Fill;
            UI_MESControl=new MESControl();
            UI_MESControl.Dock = DockStyle.Fill;
        }

    }
}
