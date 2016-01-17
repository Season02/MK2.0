using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.InteropServices;

namespace S2_0
{
    public partial class NotifyForm : Form
    {
        const int CS_DropSHADOW = 0x20000;
        const int GCL_STYLE = (-26);
        //声明Win32 API
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SetClassLong(IntPtr hwnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassLong(IntPtr hwnd, int nIndex);

        public NotifyForm()
        {
            InitializeComponent();
        }

        private void NotifyForm_Load(object sender, EventArgs e)
        {
            int xPos = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            int yPos = Screen.PrimaryScreen.WorkingArea.Height - this.Height - 30;
            this.Location = new Point(xPos, yPos);

            this.TopMost = true;
            this.TopLevel = true;
            this.ShowIcon = false;
            SetClassLong(this.Handle, GCL_STYLE, GetClassLong(this.Handle, GCL_STYLE) | CS_DropSHADOW); //API函数加载，实现窗体边框阴影效果
        }


    }
}
