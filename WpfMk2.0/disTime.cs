using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Data;

namespace WpfMk2._0
{
    class disTime : INotifyPropertyChanged
    {
        MainWindow mw = null;
        System.Windows.Controls.TextBlock desItem = null;

        System.Timers.Timer disTimer = new System.Timers.Timer(500);//Timer for time display pane update

        private string time;

        public disTime(Object parent,Object item)
        {
            mw = parent as MainWindow;
            desItem = item as System.Windows.Controls.TextBlock;
            bindItem();
            initTimer();
        }

        private void bindItem()
        {
            Binding binding = new Binding();
            binding.Source = this;
            binding.Path = new System.Windows.PropertyPath("Time");

            BindingOperations.SetBinding(desItem, System.Windows.Controls.TextBlock.TextProperty, binding);
        }

        private void initTimer()
        {
            disTimer.Elapsed += new System.Timers.ElapsedEventHandler((object sender, System.Timers.ElapsedEventArgs e) => 
                {
                    try
                    {
                        this.Time = DateTime.Now.ToString();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                    }
                });//Guard Service
            disTimer.AutoReset = true; //每到指定时间Elapsed事件是触发一次（false），还是一直触发（true）
            disTimer.Enabled = true; //是否触发Elapsed事件
            disTimer.Start();
        }

        //private void distimer(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        dist.Time = DateTime.Now.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.MessageBox.Show(ex.Message);
        //    }

        //}

        public string Time
        {
            get { return time; }
            set
            {
                time = value;
                if (PropertyChanged != null)
                {
                    this.PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Time"));
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }  

}
