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

        //导入user32.dll
        [System.Runtime.InteropServices.DllImport("user32")]
        //声明API函数
        private static extern bool AnimateWindow(IntPtr hwnd, int dwTime, int dwFlags);
        
        //正面_水平方向
        const int AW_HOR_POSITIVE = 0x0001;
        //负面_水平方向
        const int AW_HOR_NEGATIVE = 0x0002;
        //正面_垂直方向
        const int AW_VER_POSITIVE = 0x0004;
        //负面_垂直方向
        const int AW_VER_NEGATIVE = 0x0008;
        //由中间四周展开或
        const int AW_CENTER = 0x0010;
        //隐藏对象
        const int AW_HIDE = 0x10000;
        //显示对象
        const int AW_ACTIVATE = 0x20000;
        //拉幕滑动效果
        const int AW_SLIDE = 0x40000;
        //淡入淡出渐变效果
        const int AW_BLEND = 0x80000;

        bool flag = false;
        public NotifyForm()
        {
            InitializeComponent();
            flag = true;            
        }

        private void NotifyForm_Load(object sender, EventArgs e)
        {
            int xPos = Screen.PrimaryScreen.WorkingArea.Width - this.Width;
            int yPos = Screen.PrimaryScreen.WorkingArea.Height - this.Height - 30;
            Location = new Point(xPos, yPos);

            TopMost = true;
            TopLevel = true;
            ShowIcon = false;
            //AnimateWindow(this.Handle, 1000, AW_SLIDE | AW_HIDE | IsHorOrVer(1));
        }

        public int IsHorOrVer(int pos)
        {
            int rtn = 0;
            //判断是正方向还是反方向
            if (pos.Equals(0))
            {
                //判断是横向还是纵向
                if (flag)
                rtn = AW_HOR_POSITIVE;
                else rtn = AW_VER_POSITIVE;
            }
            else if (pos.Equals(1))
            {
                //判断是横向还是纵向
                if (flag)
                rtn = AW_HOR_NEGATIVE;
                else rtn = AW_VER_NEGATIVE;
            }
            return rtn;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //动画——窗体向上拖拉
            AnimateWindow(this.Handle, 1000, AW_SLIDE | AW_HIDE | IsHorOrVer(1));
            //动画——窗体向下拖拉
            AnimateWindow(this.Handle, 1000, AW_SLIDE | AW_ACTIVATE | IsHorOrVer(0));
            //动画——窗体淡出特效
            AnimateWindow(this.Handle, 1000, AW_BLEND | AW_HIDE | IsHorOrVer(1));
            //动画——窗体淡入特效
            AnimateWindow(this.Handle, 1000, AW_BLEND | AW_ACTIVATE | IsHorOrVer(0));
            //动画——窗体由四周向中心缩小直至消失
            AnimateWindow(this.Handle, 1000, AW_CENTER | AW_HIDE | IsHorOrVer(1));
            //动画——窗体由中心向四周扩展
            AnimateWindow(this.Handle, 1000, AW_CENTER | AW_ACTIVATE | IsHorOrVer(0));
        }

        private void content_TextChanged(object sender, EventArgs e)
        {

        }

        private void Close_Click(object sender, EventArgs e)
        {
            AnimateWindow(this.Handle, 1000, AW_SLIDE | AW_HIDE | IsHorOrVer(1));
            this.Close();
        }

        private void Close_MouseHover(object sender, EventArgs e)
        {
            this.Close.ForeColor = Color.Black;
        }

        private void Close_MouseClick(object sender, MouseEventArgs e)
        {
            this.Close.ForeColor = Color.Maroon;
        }


    }
}
