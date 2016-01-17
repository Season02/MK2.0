using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Threading.Tasks;

namespace S2_0
{
    class LogBuilder
    {
        static private System.Timers.Timer logTimer = new System.Timers.Timer(1000);//Timer for log
        static private List<String> buffer = new List<String>();//buffer for reade to write to log file
        static System.IO.FileStream fs;
        static System.IO.StreamWriter sw;

        static public LogBuilder()
        {
            init();
            MessageBox.Show("Hi");
        }

        static public void buildLog(String str)
        {
            try
            {
                lock(buffer)
                {
                    buffer.Add(str);
                }           
            }
            catch (Exception e)
            {
                MessageBox.Show("Building log err: " + e.Message);
            }
            
        }

        private static void init()
        {
            logTimer.Elapsed += new System.Timers.ElapsedEventHandler((object sender, System.Timers.ElapsedEventArgs e) =>
            {
                try
                {
                    fs = new System.IO.FileStream(@"d:\SystemLog.txt", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write);
                    sw = new System.IO.StreamWriter(fs);
                    sw.BaseStream.Seek(0, System.IO.SeekOrigin.End);
                    lock(buffer)
                    {
                        foreach(String str in buffer)
                        {
                            sw.Write("[" + DateTime.Now.ToString() + "] ");
                            //sw.Flush();
                            sw.WriteLine(str);
                            sw.Flush();
                        }
                        buffer.Clear();
                    }                   

                    sw.Close();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            });//Guard Service
            logTimer.AutoReset = true; //每到指定时间Elapsed事件是触发一次（false），还是一直触发（true）
            logTimer.Enabled = true; //是否触发Elapsed事件
            logTimer.Start();
        }

        private void builder()
        {
        }

    }
}
