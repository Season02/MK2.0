using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices; 

namespace Laboratory_mk2_
{
    public partial class Form1 : Form
    {
        timeEvent te = null;
        
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(
            byte bVk,    //虚拟键值
            byte bScan,// 一般为0
            int dwFlags,  //这里是整数类型  0 为按下，2为释放
            int dwExtraInfo  //这里是整数类型 一般情况下设成为 0
        );
        
        public Form1()
        {
            InitializeComponent();

            te = new timeEvent();

            //te.timebombEvent += new timeEvent.timebombEventHandler(bombEvent);
        }

        private void bombEvent(String mEvent, String point)
        {
            //AddText(msg);
            MessageBox.Show(mEvent + point);
        }

        private void Form1_Activated(object sender, EventArgs e)
        {

        }



    }
}
