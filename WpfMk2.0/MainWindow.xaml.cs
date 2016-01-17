using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using System.Net;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;


namespace WpfMk2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ipScan ipscan = null;//The class for IP scan
        disTime distime = null;//The class for time panel update

        public socket sersocket = null;

        public List<socket> socketCollection = new List<socket>();
        public int indexSC = 0; 

        public System.Collections.ObjectModel.ObservableCollection<slaveList> personalInfoList = null;
        
        public DebugWindow dw = null;
        public HEXWindow hexw = null;

        //委托的声明
        //public delegate void Debug_Event_Handler(object sender, String msg);
        //public event Debug_Event_Handler Debug_Event;

        public MainWindow()
        {
            System.Windows.Media.RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;

            InitializeComponent();

            distime = new disTime(this, this.RemoveAllMediaTextBlock);

            initWL();

            personalInfoList = new System.Collections.ObjectModel.ObservableCollection<slaveList>();

            this.iplistView.ItemsSource = personalInfoList;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(delegate()
            {
                this.Dispatcher.Invoke(delegate() { ipscan = new ipScan(this); });
            });
            t.Start();

            //Thread tdw = new Thread(delegate()
            //{
                dw = new DebugWindow(this);
                dw.Show();
            //});
            //tdw.IsBackground = true;
            //tdw.Start();            

            Thread checkListThread = new Thread(()=>
            {
                while(true)
                {
                    Thread.Sleep(2000);

                    foreach (socket soc in socketCollection)
                    {
                        Thread t1 = new Thread(() =>
                        {
                            var lis = personalInfoList.Single(p => p.IP == soc.ip);

                            if (soc.serviceSocket.Connected == true)
                            {
                                lis.CONDITION = "ONLINE";
                            }
                            else
                            {
                                lis.CONDITION = "OFF LINE";
                                soc.reConnect();
                            }
                        });
                        t1.IsBackground = true;
                        t1.Start();
                    }                    
                }
            });
            checkListThread.IsBackground = true;
            checkListThread.Start();

        }

        void initWL()
        {
            //this.warningLine.Visibility = Visibility.Visible;
            Storyboard myStoryboard = new Storyboard();
            //DoubleAnimation OpacityDoubleAnimation = new DoubleAnimation();
            //OpacityDoubleAnimation.From = 0.7;
            //OpacityDoubleAnimation.To = 0.7;
            //OpacityDoubleAnimation.By = 1;
            //OpacityDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(2.0));
            //OpacityDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            //Storyboard.SetTargetName(OpacityDoubleAnimation, warningLine.Name);
            //Storyboard.SetTargetProperty(OpacityDoubleAnimation, new PropertyPath(DataGrid.OpacityProperty));
            warningLine.RenderTransform = new TranslateTransform();
            DependencyProperty[] propertyChain = new DependencyProperty[]
            {
                DataGrid.RenderTransformProperty,
                TranslateTransform.XProperty
            };
            DoubleAnimation InDoubleAnimation = new DoubleAnimation();
            InDoubleAnimation.From = 0;
            InDoubleAnimation.To = 101.823;
            InDoubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(0.90));
            InDoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            Storyboard.SetTargetName(InDoubleAnimation, warningLine.Name);
            Storyboard.SetTargetProperty(InDoubleAnimation, new PropertyPath("(0).(1)", propertyChain));


            DoubleAnimationUsingKeyFrames dak = new DoubleAnimationUsingKeyFrames();
            //关键帧定义
            dak.KeyFrames.Add(new LinearDoubleKeyFrame(0.6, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            dak.KeyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5))));
            dak.KeyFrames.Add(new LinearDoubleKeyFrame(0.6, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1))));

            dak.BeginTime = TimeSpan.FromSeconds(0);//从第2秒开始动画
            dak.RepeatBehavior = RepeatBehavior.Forever;
            //开始动画
            //this.warningLine.BeginAnimation(Border.OpacityProperty, dak);

            Storyboard.SetTargetName(dak, warningLine.Name);
            Storyboard.SetTargetProperty(dak, new PropertyPath(Border.OpacityProperty));


            myStoryboard.Children.Add(dak);
            myStoryboard.Children.Add(InDoubleAnimation);
            myStoryboard.Begin(this);
        }

        public void received(byte[] data)
        {
            Thread ti = new Thread(new ThreadStart(() =>
            {
                try
                {
                    MessageBox.Show(Encoding.Unicode.GetString(data, 0, 1024));
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message + "\r\n From: MainWinsow.received(object sender,byte[] data)");
                }
            }));
            ti.IsBackground = true;
            ti.Start();
        }

        private void MinimizeTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //最小化
            this.WindowState = WindowState.Minimized;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //this.DragMove();
        }

        private void CloseAppTextBlock_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void TopContainerBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseAppTextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;  //Else the event will send to border,we do not expect that
        }

        private void FeedBackTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void FeedBackTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void iplistView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Thread ti = new Thread(new ParameterizedThreadStart((Object _sender) =>
            //{
            //    var i = _sender as ListView;
            //    try
            //    {
            //        slaveList item = null;
            //        i.Dispatcher.Invoke(new Action(delegate { item = i.SelectedItem as slaveList; }));
            //        //var item = i.SelectedItem as slaveList;
            //        socket soc = socketCollection.Find(
            //        delegate(socket s)
            //        {
            //            return s.ip == item.IP;
            //        });

            //        soc.sendSingle(10,0x10);
            //    }
            //    catch (Exception ee)
            //    {
            //        MessageBox.Show(ee.Message + "\r\n From: iplistView_PreviewMouseLeftButtonUp");
            //    }
            //}));
            //ti.IsBackground = true;
            //ti.Start(sender);
        }

        private void ButtonTest_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                slaveList item = null;
                iplistView.Dispatcher.Invoke(new Action(delegate { item = iplistView.SelectedItem as slaveList; }));
                socket soc = socketCollection.Find(
                delegate(socket s)
                {
                    return s.ip == item.IP;
                });
                //soc.sendSingle(10, 0x10);
                HEXWindow hexw = new HEXWindow(this, soc);
                hexw.Show();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message + "\r\n From: iplistView_PreviewMouseLeftButtonUp");
            }

            e.Handled = true;
        }

        private void ButtonSendFile_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                String SENDFILE = "test.jpg";
                slaveList item = null;
                iplistView.Dispatcher.Invoke(new Action(delegate { item = iplistView.SelectedItem as slaveList; }));
                socket soc = socketCollection.Find(
                delegate(socket s)
                {
                    return s.ip == item.IP;
                });
                //soc.sendSingle(10, 0x10);

                soc.sendFile(new FileInfo(SENDFILE),1024);

                //FileInfo MKFile = new FileInfo(SENDFILE);
                //打开文件流  
                //FileStream MKFileStream = MKFile.OpenRead();
                //包的大小  
                //int PacketSize = 1024;
                //包的数量  
                //int PacketCount = (int)(MKFileStream.Length / ((long)PacketSize));
                //dw.debugBox.Dispatcher.Invoke(new Action(delegate() { dw.debugBox.AppendText("PACKET COUNT: " + PacketCount.ToString() + "\r\n"); }));
                //this.progressBar1.Maximum = PacketCount;
                //最后一个包的大小  
                //int LastDataPacket = (int)(MKFileStream.Length - ((long)(PacketSize * PacketCount)));
                //dw.debugBox.Dispatcher.Invoke(new Action(delegate() { dw.debugBox.AppendText("LAST PACKET SIZE: " + LastDataPacket.ToString() + "\r\n"); }));


            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message + "\r\n From: iplistView_PreviewMouseLeftButtonUp");
            }

            e.Handled = true;
        }

        private void ButtonRelease_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                slaveList item = null;
                iplistView.Dispatcher.Invoke(new Action(delegate { item = iplistView.SelectedItem as slaveList; }));
                socket soc = socketCollection.Find(
                delegate(socket s)
                {
                    return s.ip == item.IP;
                });
                soc.sendSingle(1, 0x04);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message + "\r\n From: ButtonRelease_PreviewMouseLeftButtonUp");
            }

            e.Handled = true;
        }






    }
}
