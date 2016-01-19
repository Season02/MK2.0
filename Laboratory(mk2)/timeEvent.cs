using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laboratory_mk2_
{
    class timeEvent
    {
        System.Timers.Timer bobTimer = new System.Timers.Timer(1000);

        public delegate void timebombEventHandler(String time, String key);

        public event timebombEventHandler timebombEvent;

        public timeEvent()
        {
            bobTimer.Elapsed += new System.Timers.ElapsedEventHandler((s, e) => bomb(s, e));//Guard Service
            bobTimer.AutoReset = true; //每到指定时间Elapsed事件是触发一次（false），还是一直触发（true）
            bobTimer.Enabled = true; //是否触发Elapsed事件
            bobTimer.Start();
        }

        private void bomb(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                string CurrTime = System.DateTime.Now.ToShortTimeString();
                string[] list = CurrTime.Split(':');

                int len = list.Length;
                string s = "12:00";

                //if (CurrTime == s)
                {
                    timebombEvent(list[0],list[1]);
                }
                
            }
            catch (Exception ex)
            {

            }
        }


    }
}
