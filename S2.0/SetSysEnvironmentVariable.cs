using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Windows.Forms;

namespace S2_0
{
    class SetSysEnvironmentVariable
    {
        [DllImport("Kernel32.DLL ", SetLastError = true)]
        public static extern bool SetEnvironmentVariable(string lpName, string lpValue);

        public static void SetPath(string pathValue)
        {
            string pathlist;
            //pathlist = SysEnvironment.GetSysEnvironmentByName("PATH");
            pathlist = System.Environment.GetEnvironmentVariable("Path");

            //MessageBox.Show(pathlist);%java_home%\bin;D:\IDM Computer Solutions\UltraEdit\;C:\Users\Butterfly\AppData\Roaming\npm

            string[] list = pathlist.Split(';');
            bool isPathExist = false;

            foreach (string item in list)
            {
                if (item == pathValue)
                    isPathExist = true;
            }
            if (!isPathExist)
            {
                SetEnvironmentVariable("PATH", pathlist + ";" + pathValue);
            }

        }
    }
}
