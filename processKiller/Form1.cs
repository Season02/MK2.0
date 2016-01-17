using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace processKiller
{
    using System.Diagnostics;
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void killer(string name)
        {
            try
            {
                Process[] ps = Process.GetProcesses();
                foreach (Process item in ps)
                {
                    this.tb_log.AppendText(item.ProcessName + "\n");
                    if (String.Compare(item.ProcessName, name, true) == 0)
                    {
                        item.Kill();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                this.tb_log.AppendText(e.Message);
            }
        }

        private void btn_kill_Click(object sender, EventArgs e)
        {
            killer(this.tb_processName.Text.ToString());
        }

        private void btn_run_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(this.tb_processName.Text.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //System.Media.SystemSounds.Beep.Play();
            System.Media.SystemSounds.Asterisk.Play();
            //System.Media.SystemSounds.Exclamation.Play();
            //System.Media.SystemSounds.Hand.Play();
            //System.Media.SystemSounds.Question.Play();
        }


    }
}
