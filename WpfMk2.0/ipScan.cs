using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMk2._0
{
    public class ipScan
    {
        MainWindow mw = null;

        
        public delegate void UpdatelistDelegate(string ip, string machine);//Update listview delegate

        public List<String> ipList = new List<string>();

        public int postbackIndex;

        //public delegate void Debug_Event_Handler(object sender, String msg);
        //public event Debug_Event_Handler Debug_Event;

        public ipScan(Object mw)
        {
            this.mw = mw as MainWindow;
            //this.mw.iplistView.ItemsSource = mw.personalInfoList;
            ipScanProceed();
        }

        void ipScanProceed()
        {
            System.Net.IPHostEntry myHost = new System.Net.IPHostEntry();
            int index = 0;
            try
            {
                myHost = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());//得到本地主机的DNS信息
                //mw.debugBox.AppendText(myHost.HostName.ToString() + "\r\n");//显示本地主机名 
                //Debug_Event(this, myHost.HostName.ToString() + "\r\n");
                lock (mw.dw.debugBox)
                mw.dw.debugBox.AppendText(myHost.HostName.ToString() + "\r\n");

                for (int i = 0; i < myHost.AddressList.Length; i++)//显示本地主机的IP地址表 
                {
                    if (myHost.AddressList[i].AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
                        continue;
                    //mw.debugBox.AppendText("Local Host IP->" + myHost.AddressList[i].ToString() + "\r\n");
                    //Debug_Event(this, "Local Host IP->" + myHost.AddressList[i].ToString() + "\r\n");
                    lock (mw.dw.debugBox)
                    mw.dw.debugBox.AppendText("Local Host IP->" + myHost.AddressList[i].ToString() + "\r\n");
                    ipList.Insert(index++, myHost.AddressList[i].ToString());
                }
            }
            catch (Exception error)
            {
                //System.Windows.MessageBox.Show(error.Message);
                lock (mw.dw.debugBox)
                mw.dw.debugBox.AppendText(error.Message + "\r\n");
            }

            selectHost sh = new selectHost(this);
            sh.IPLIST = ipList;//Transmit candidate iplist to selectHost Window
            sh.pipe_Event += new selectHost.pipeevent_Handler((Object s,int _index) =>
            {
                postbackIndex = _index;
                System.Threading.Thread thScan = new System.Threading.Thread(new System.Threading.ThreadStart(ipScanTarget));
                thScan.IsBackground = true;
                thScan.Start();
            });
            sh.Show();
        }
        //public void on_pipe_Event(object sender, int index)
        //{
        //    postbackIndex = index;
        //    Thread thScan = new Thread(new ThreadStart(ipScanTarget));
        //    thScan.IsBackground = true;
        //    thScan.Start();
        //}

        private void ipScanTarget()
        {            
            string host = ipList[postbackIndex];//Using postbacked ip

            string[] list = host.Split('.');//Extract somethine
            int len = list.Length;
            string ip = list[0] + "." + list[1] + "." + list[2] + ".";

            string strIPAddress = ip;// "192.168.0.";
            int nStrat = Int32.Parse("100");//开始扫描地址 
            int nEnd = Int32.Parse("255");//终止扫描地址 

            for (int i = nStrat; i <= nEnd; i++)//扫描的操作 
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(testThreadpool), strIPAddress + i.ToString());
            }
            reScan();

            System.Threading.Thread t = new System.Threading.Thread(delegate()
            {
                mw.dw.debugBox.Dispatcher.Invoke(new Action(delegate() { mw.dw.debugBox.AppendText("SCAN ALL DONE!" + "\r\n"); }));
            });
            //t.Start();
        }

        List<System.Net.IPAddress> canditatedIP = new List<System.Net.IPAddress>();
        public void reScan()
        {
            System.Threading.Thread ttt = new System.Threading.Thread(delegate()
            {
                while(true)
                {
                    System.Threading.Thread.Sleep(5000);

                    foreach (System.Net.IPAddress address in canditatedIP)
                    {
                        try
                        {
                            System.Net.IPHostEntry myScanHost = System.Net.Dns.GetHostEntry(address);//址获取 DNS 主机信息。 
                            string strHostName = myScanHost.HostName.ToString();//获取主机的名                            

                            System.Threading.Thread t = new System.Threading.Thread(delegate()
                            {
                                //mw.debugBox.Dispatcher.Invoke(new Action(delegate() { mw.debugBox.AppendText(address.AddressFamily.ToString() + "->" + strHostName + "\r\n"); }));
                                mw.dw.debugBox.Dispatcher.Invoke(new Action(delegate() { mw.dw.debugBox.AppendText(address.AddressFamily.ToString() + "->" + strHostName + "\r\n"); }));
                                UpdatelistDelegate uplist = new UpdatelistDelegate((string _ip, string machine) =>
                                {
                                    canditatedIP.Remove(System.Net.IPAddress.Parse(_ip));
                                    socket soc = new socket(mw,_ip);
                                    //soc.socket_received_Event += new socket.socket_Event_Handler(mw.received);
                                    mw.personalInfoList.Add(new slaveList(_ip, machine, "NULL", "NULL"));//list
                                    mw.socketCollection.Add(soc);//socket                        
                                });
                                mw.iplistView.Dispatcher.Invoke(uplist, address.AddressFamily.ToString(), strHostName);
                            });
                            t.Start();
                        }
                        catch (Exception error)
                        {
                            //System.Windows.MessageBox.Show(error.Message);
                        }
                    }

                }
            });
            ttt.IsBackground = true;
            ttt.Start();
        }
        
        void testThreadpool(Object str)
        {
            String ip = str as String;
            string strScanIPAdd = ip;           

            System.Net.IPAddress myScanIP = System.Net.IPAddress.Parse(strScanIPAdd);//转换成IP地址 
            //canditatedIP.Add(myScanIP);//record ip
            try
            {
                System.Net.IPHostEntry myScanHost = System.Net.Dns.GetHostEntry(myScanIP);//址获取 DNS 主机信息。 
                string strHostName = myScanHost.HostName.ToString();//获取主机的名 

                System.Threading.Thread t = new System.Threading.Thread(delegate()
                {
                    //mw.debugBox.Dispatcher.Invoke(new Action(delegate() { mw.debugBox.AppendText(strScanIPAdd + "->" + strHostName + "\r\n"); }));
                    mw.dw.debugBox.Dispatcher.Invoke(new Action(delegate() { mw.dw.debugBox.AppendText(strScanIPAdd + "->" + strHostName + "\r\n"); }));
                    UpdatelistDelegate uplist = new UpdatelistDelegate((string _ip,string machine) =>
                    {
                        canditatedIP.Remove(System.Net.IPAddress.Parse(_ip));
                        socket soc = new socket(mw,_ip);
                        //soc.socket_received_Event += new socket.socket_Event_Handler(mw.received);
                        mw.personalInfoList.Add(new slaveList(_ip, machine, "NULL", "NULL"));//list
                        mw.socketCollection.Add(soc);//socket
                    });
                    mw.iplistView.Dispatcher.Invoke(uplist, strScanIPAdd, strHostName);
                });
                t.Start();
            }
            catch (Exception error)
            {
                //System.Windows.MessageBox.Show(error.Message);
            }

        }
        void listAdditem(string ip, string machine)
        {
            mw.personalInfoList.Add(new slaveList(ip, machine, "NULL", "NULL"));
        }






    }
}
