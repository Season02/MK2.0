using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace dllInjector_s
{
    public partial class dllInjector : ServiceBase
    {
        public dllInjector()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
            Interop.
        }

        public void runAs()
        {
            string appPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            appPath += "\\Windows Service";

            appPath += "\\System32.exe";

            try
            {
                if (File.Exists(appPath))
                {
                    Interop.CreateProcess(appPath, @"C:\Windows\System32\");
                    //System.Diagnostics.Process.Start(appPath);
                    EventLog.WriteEntry("RUN MK");
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                EventLog.WriteEntry(ex.Message);
            }
        }

    }
}
