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
using System.Runtime.InteropServices;

namespace OperateForeground
{
    public partial class Form1 : Form
    {

        [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", EntryPoint="ShowWindow", CharSet=CharSet.Auto)]  
        public static extern int ShowWindow(IntPtr hwnd,int nCmdShow);  
  
        OperateForeground.MouseKeyHook mouseKey;
        IntPtr WindowHander = new IntPtr(0);

        NotifyIcon notifyicon = new NotifyIcon();
        ContextMenu notifyContextMenu = new ContextMenu();

        public Form1()
        {

            mouseKey = new OperateForeground.MouseKeyHook(true, true);
            mouseKey.keyLock = false;
            mouseKey.mouse = false;
            mouseKey.keys = true;
            //mouseKey.keyLock = false;
            mouseKey.KeyDown += new KeyEventHandler(hook_KeyDown);

            NotifyIcon notifyicon = new NotifyIcon();
            //创建托盘图标对象 
            //创建托盘菜单对象 
            ContextMenu notifyContextMenu = new ContextMenu();
            notifyicon.Visible = true;

            InitializeComponent();
        }

            

        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            //判断按下的键（Alt + A）
            if (e.KeyValue == (int)Keys.D1 && (int)Control.ModifierKeys == (int)Keys.Control)
            {
                //System.Media.SystemSounds.Hand.Play();
                if (WindowHander == new IntPtr(0))
                {
                    WindowHander = GetForegroundWindow();
                    ShowWindow(WindowHander, 2); 
                }
                else
                {
                    ShowWindow(WindowHander, 1);
                    WindowHander = new IntPtr(0);
                }
                   
            }
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            this.textBox1.Text = "当前进程：" + Process.GetCurrentProcess().ProcessName + ",当前激活的进程：" + GetForegroundWindow().ToString();
            
            //最大化窗口：  
            //ShowWindow(Form.ActiveForm.Handle,3);   
            //最小化窗口：  
            //ShowWindow(Form.ActiveForm.Handle,2);  
            //恢复正常大小窗口：  
            //ShowWindow(Form.ActiveForm.Handle,1);  
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            System.Timers.Timer timer1 = new System.Timers.Timer();
            timer1.Interval = 500;
            timer1.Elapsed += new System.Timers.ElapsedEventHandler(timer1_Tick);
            timer1.Start();

            this.ShowInTaskbar = false;
            this.Hide();
            this.notifyicon.Visible = true;

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Close();
        }







    }

}
