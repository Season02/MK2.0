using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MK2._0
{
    public partial class MK2 : Form
    {
        private Kennedy.ManagedHooks.MouseHook mouseHook = null;
        private Kennedy.ManagedHooks.KeyboardHook keyboardHook = null;

        public MK2()
        {
            InitializeComponent();

            mouseHook = new Kennedy.ManagedHooks.MouseHook();
            mouseHook.MouseEvent += new Kennedy.ManagedHooks.MouseHook.MouseEventHandler(mouseHook_MouseEvent);

            keyboardHook = new Kennedy.ManagedHooks.KeyboardHook();
            keyboardHook.KeyboardEvent += new Kennedy.ManagedHooks.KeyboardHook.KeyboardEventHandler(keyboardHook_KeyboardEvent);
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

            int length = textBoxMessages.Text.Length + message.Length;
            if (length >= textBoxMessages.MaxLength)
            {
                textBoxMessages.Text = "";
            }

            if (!message.EndsWith("\r\n"))
            {
                message += "\r\n";
            }

            textBoxMessages.Text = message + textBoxMessages.Text;
        }

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            AddText("Adding mouse hook.");
            mouseHook.InstallHook();

            AddText("Adding keyboard hook.");
            keyboardHook.InstallHook();

            buttonInstall.Enabled = false;
            buttonUninstall.Enabled = true;
        }

        private void buttonUninstall_Click(object sender, EventArgs e)
        {
            mouseHook.UninstallHook();
            AddText("Mouse hook removed.");

            keyboardHook.UninstallHook();
            AddText("Keyboard hook removed.");

            buttonInstall.Enabled = true;
            buttonUninstall.Enabled = false;
        }



    }
}
