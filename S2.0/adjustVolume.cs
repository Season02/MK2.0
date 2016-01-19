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

namespace S2_0
{
    public class adjustVolume
    {       
        public const uint WM_APPCOMMAND = 0x319;
        public const uint APPCOMMAND_VOLUME_UP = 0x0a;
        public const uint APPCOMMAND_VOLUME_DOWN = 0x09;
        public const uint APPCOMMAND_VOLUME_MUTE = 0x08;
        public const uint APPCOMMAND_MEDIA_NEXTTRACK = 0x0b;//useless
        //APPCOMMAND_MEDIA_PLAY_PAUSE     = 14
        //APPCOMMAND_MEDIA_PREVIOUSTRACK  = 12
        //APPCOMMAND_MEDIA_NEXTTRACK      = 11
        //APPCOMMAND_VOLUME_MUTE          =  8
        //APPCOMMAND_VOLUME_UP            = 10
        //APPCOMMAND_VOLUME_DOWN          =  9

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]                     
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);

        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void keybd_event(
        byte bVk, //虚拟键值  
        byte bScan,// 一般为0  
        int dwFlags, //这里是整数类型 0 为按下，2为释放  
        int dwExtraInfo //这里是整数类型 一般情况下设成为0  
        );

        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi)]
        protected static extern int mciSendString(string lpstrCommand, StringBuilder lpstrReturnString, int uReturnLength, IntPtr hwndCallback);

        public static void OpenCD()
        {
            mciSendString("set cdaudio door open", null, 0, IntPtr.Zero);
        }

        public static void CloseCD()
        {
            mciSendString("set cdaudio door closed", null, 0, IntPtr.Zero);
        }

        //adjustVolume.SendMessage(this.Handle, adjustVolume.WM_APPCOMMAND, 0x200eb0, adjustVolume.APPCOMMAND_VOLUME_MUTE * 0x10000);//静音

        #region 模拟按键
        public static void Play()
        {
            keybd_event(179, 0, 0, 0);//179
            keybd_event(179, 0, 2, 0);//179
        }

        public static void Stop()
        {
            keybd_event(178, 0, 0, 0);//178
            keybd_event(178, 0, 2, 0);
        }

        public static void Previous()
        {
            keybd_event(177, 0, 0, 0);//177
            keybd_event(177, 0, 2, 0);
        }

        public static void Next()
        {            
            keybd_event(176, 0, 0, 0);//176
            keybd_event(176, 0, 2, 0);
        }



        public static void altF4()
        {
            keybd_event(18, 0, 0, 0);//179
            keybd_event(115, 0, 0, 0);
            keybd_event(18, 0, 2, 0);//179ddd
            keybd_event(115, 0, 2, 0);
        }
       
        public static void winD()
        {
            keybd_event(0x5b, 0, 0, 0); 
            keybd_event(68, 0, 0, 0);//179
            keybd_event(0x5b, 0, 2, 0);
            keybd_event(68, 0, 2, 0);//179            
        }

        public static void altTab()
        {
            keybd_event(18, 0, 0, 0);//alt
            keybd_event(9, 0, 0, 0);//tab      
            keybd_event(18, 0, 2, 0);//
            keybd_event(9, 0, 2, 0);
        }

        public static void esc()
        {
            keybd_event(27, 0, 0, 0);
            keybd_event(27, 0, 2, 0);                       
        }
        public static void win()
        {
            keybd_event(0x5b, 0, 0, 0);
            keybd_event(0x5b, 0, 2, 0);          
        }
        public static void numKey(int num)
        {
            if (num > 9 || num < 0)
                return;
            keybd_event((byte)(48 + num), 0, 0, 0);
            keybd_event((byte)(48 + num), 0, 2, 0);
        }
        public static void funKey(int fun)
        {
            if (fun > 16 || fun < 0)
                return;
            keybd_event((byte)(112 + fun), 0, 0, 0);
            keybd_event((byte)(112 + fun), 0, 2, 0);
        }
        public static void alpKey(int alp)
        {
            if (alp > 24 || alp < 0)
                return;

            keybd_event((byte)(65 + alp), 0, 0, 0);
            keybd_event((byte)(65 + alp), 0, 2, 0);
        }
        public static void spaceBar()
        {
            keybd_event(32, 0, 0, 0);
            keybd_event(32, 0, 2, 0);
        }
        public static void enter()
        {
            keybd_event(13, 0, 0, 0);
            keybd_event(13, 0, 2, 0);
        }
        public static void backSpace()
        {
            keybd_event(8, 0, 0, 0);
            keybd_event(8, 0, 2, 0);
        }
        public static void delete()
        {
            keybd_event(46, 0, 0, 0);
            keybd_event(46, 0, 2, 0);
        }
        public static void numLock()
        {
            keybd_event(144, 0, 0, 0);
            keybd_event(144, 0, 2, 0);
        }
        public static void arrUp()
        {
            keybd_event(38, 0, 0, 0);
            keybd_event(38, 0, 2, 0);
        }
        public static void arrDown()
        {
            keybd_event(40, 0, 0, 0);
            keybd_event(40, 0, 2, 0);
        }
        public static void arrLeft()
        {
            keybd_event(37, 0, 0, 0);
            keybd_event(37, 0, 2, 0);
        }
        public static void arrRight()
        {
            keybd_event(39, 0, 0, 0);
            keybd_event(39, 0, 2, 0);
        }
        public static void mouLeft()
        {
            keybd_event(1, 0, 0, 0);
            keybd_event(1, 0, 2, 0);
        }
        public static void mouRight()
        {
            keybd_event(2, 0, 0, 0);
            keybd_event(2, 0, 2, 0);
        }
        public static void mouMid()
        {
            keybd_event(4, 0, 0, 0);
            keybd_event(4, 0, 2, 0);
        }





        #endregion

    }



}
