using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

using System.Net.Sockets;
using System.Threading;

using System.IO;

namespace WpfMk2._0
{

    public class socket
    {
        public Socket serviceSocket = null;
        int port = 4320;
        int transmitPort = 4380;
        public string ip = null;

        public static byte[] receiveBuffer = new byte[1024];//Receive buffer
        public byte[] sendBuffer = null;//Send buffer

        MainWindow mw = null;

        const int COMMAND_SENDFILE = 11;

        //委托的声明
        //public delegate void socket_Event_Handler(byte[] data);
        //用委托声明两个事件
        
        //public event socket_Event_Handler socket_sended_Event;
        //public event socket_Event_Handler socket_received_Event;
        public socket(MainWindow mw ,String ipAddress)
        {
            this.mw = mw;
            ip = ipAddress;

            sendBuffer = new byte[1024];//Send buffer //c# automatic assigesd to 0     
            try
            {
                //创建一个Socket
                serviceSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                System.Threading.Thread tt = new System.Threading.Thread(delegate()
                {
                    //连接到指定服务器的指定端口
                    //方法参考：http://msdn.microsoft.com/zh-cn/library/system.net.sockets.socket.connect.aspx
                    try
                    {
                        serviceSocket.Connect(ip, port);
                        initreceive(1);                        
                    }
                    catch(Exception ee)
                    {
                        //MessageBox.Show(ee.Message + "\r\n From: socket.socket(String ipAddress).thread");
                        lock (mw.dw.debugBox)
                        mw.dw.debugBox.Dispatcher.Invoke(new Action(delegate() { mw.dw.debugBox.AppendText(ee.Message + "\r\n From: socket.socket(String ipAddress).thread" + "\r\n"); }));
                    }
                });
                tt.IsBackground = true;
                tt.Start();
                
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message + "\r\n From: socket.socket(String ipAddress)");
                lock (mw.dw.debugBox)
                mw.dw.debugBox.AppendText(e.Message + "\r\n From: socket.socket(String ipAddress)" + "\r\n");
            }

        }

        public void reConnect()
        {
            serviceSocket.Close();
            serviceSocket = null;
            sendBuffer = new byte[1024];//Send buffer //c# automatic assigesd to 0     

            try
            {
                serviceSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                System.Threading.Thread tt = new System.Threading.Thread(delegate()
                {
                    try
                    {
                        serviceSocket.Connect(ip, port);
                    }
                    catch (Exception ee)
                    {
                        //MessageBox.Show(ee.Message + "\r\n From:" + this);
                    }
                });
                tt.Start();

            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message + "\r\n From:" + this);
            }
        }

        public void sendSingle(int index,byte data)
        {
            if (serviceSocket.Connected == true)
            {
                try
                {
                    sendBuffer[index] = data;
                    serviceSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, null, null);
                    sendBuffer[index] = 0x00;
                }
                catch (SocketException ex)
                {
                    //MessageBox.Show(ex.Message + "\r\n From: socket.sendSingle(int index,byte data)");
                    lock (mw.dw.debugBox)
                    mw.dw.debugBox.AppendText(ex.Message + "\r\n From: socket.sendSingle(int index,byte data)" + "\r\n");
                }
            }
            
        }

        public void sendFile(FileInfo MKFile, int PacketSize)
        {
            Thread sendFileThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    if (serviceSocket.Connected == true)
                    {
                        //打开文件流  
                        FileStream MKFileStream = MKFile.OpenRead();
                        //包的数量  
                        int PacketCount = (int)(MKFileStream.Length / ((long)PacketSize));
                        lock (mw.dw.debugBox)
                            mw.dw.debugBox.Dispatcher.Invoke(new Action(delegate() { mw.dw.debugBox.AppendText("PACKET COUNT: " + PacketCount.ToString() + "\r\n"); }));
                        //this.progressBar1.Maximum = PacketCount;
                        //最后一个包的大小  
                        int LastDataPacket = (int)(MKFileStream.Length - ((long)(PacketSize * PacketCount)));
                        lock (mw.dw.debugBox)
                            mw.dw.debugBox.Dispatcher.Invoke(new Action(delegate() { mw.dw.debugBox.AppendText("LAST PACKET SIZE: " + LastDataPacket.ToString() + "\r\n"); }));

                        byte[] commands = new byte[PacketCount];
                        commands[COMMAND_SENDFILE] = 0x10;

                        int offset = COMMAND_SENDFILE + 1;
                        int size_file = (int)MKFileStream.Length;
                        int count_packet = PacketCount;
                        int size_packet = PacketSize;
                        int size_last_packet = LastDataPacket;

                        commands[offset] = (byte)(size_file >> 24);
                        commands[offset + 1] = (byte)(size_file >> 16);
                        commands[offset + 2] = (byte)(size_file >> 8);
                        commands[offset + 3] = (byte)(size_file);

                        offset += 4;
                        commands[offset] = (byte)(count_packet >> 24);
                        commands[offset + 1] = (byte)(count_packet >> 16);
                        commands[offset + 2] = (byte)(count_packet >> 8);
                        commands[offset + 3] = (byte)(count_packet);

                        offset += 4;
                        commands[offset] = (byte)(size_packet >> 24);
                        commands[offset + 1] = (byte)(size_packet >> 16);
                        commands[offset + 2] = (byte)(size_packet >> 8);
                        commands[offset + 3] = (byte)(size_packet);

                        offset += 4;
                        commands[offset] = (byte)(size_last_packet >> 24);
                        commands[offset + 1] = (byte)(size_last_packet >> 16);
                        commands[offset + 2] = (byte)(size_last_packet >> 8);
                        commands[offset + 3] = (byte)(size_last_packet);

                        serviceSocket.Send(commands);

                        Socket TransmitSockt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        TransmitSockt.Connect(ip, transmitPort);

                        byte[] data = new byte[PacketSize];

                        for(int i = 0;i<count_packet;i++)
                        {
                            //从文件流读取数据并填充数据包  
                            MKFileStream.Read(data,0,data.Length);
                            TransmitSockt.Send(data);                            
                        }
                        if (size_last_packet != 0)
                        {
                            data = new byte[size_last_packet];
                            MKFileStream.Read(data, 0, data.Length);
                            TransmitSockt.Send(data);
                        }

                        TransmitSockt.Close();
                        MKFileStream.Close();

                        lock (mw.dw.debugBox)
                            mw.dw.debugBox.Dispatcher.Invoke(new Action(delegate() { mw.dw.debugBox.AppendText("SEND FILE DONE!" + "\r\n"); }));
                    }
                }
                catch (Exception ee)
                {
                    //MessageBox.Show(ee.Message + "\r\n From:socket.initreceive()");
                    lock (mw.dw.debugBox)
                        mw.dw.debugBox.Dispatcher.Invoke(new Action(delegate() { mw.dw.debugBox.AppendText(ee.Message + "\r\n From:socket.initreceive()" + "\r\n"); }));
                }
            }));
            sendFileThread.IsBackground = true;
            sendFileThread.Start();
        }

        public void initreceive(int i)
        {
            Thread receiveThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    serviceSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), serviceSocket);
                }
                catch (Exception ee)
                {
                    //MessageBox.Show(ee.Message + "\r\n From:socket.initreceive()");
                    lock (mw.dw.debugBox)
                    //mw.dw.debugBox.AppendText(ee.Message + "\r\n From:socket.initreceive()" + "\r\n");
                    mw.dw.debugBox.Dispatcher.Invoke(new Action(delegate() { mw.dw.debugBox.AppendText(ee.Message + "\r\n From:socket.initreceive()" + "\r\n"); }));
                }
            }));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        public void initreceive()
        {
            Thread receiveThread = new Thread(new ThreadStart(() =>
            {
                try
                {
                    while (true)
                    {
                        Thread.Sleep(2000);

                        byte[] buffer = new byte[1024];
                        if (0 == serviceSocket.Receive(buffer))
                        {
                            serviceSocket.Close();
                        }

                        //this.socket_received_Event(buffer);
                        mw.received(buffer);
                    }
                }
                catch (Exception ee)
                {
                    //MessageBox.Show(ee.Message + "\r\n From:socket.initreceive()");
                    lock (mw.dw.debugBox)
                    mw.dw.debugBox.Dispatcher.Invoke(new Action(delegate() { mw.dw.debugBox.AppendText(ee.Message + "\r\n From:socket.initreceive()" + "\r\n"); }));
                }
            }));
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }

        static byte[] buffer = new byte[1024];

        public void ReceiveMessage(IAsyncResult ar)
        {
            try
            {
                var socket = ar.AsyncState as Socket;
                var length = socket.EndReceive(ar);

                //socket_received_Event(socket, buffer);
                mw.received(buffer);

                //接收下一个消息(因为这是一个递归的调用，所以这样就可以一直接收消息了）
                serviceSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), serviceSocket);
            }
            catch(Exception ex){
                //MessageBox.Show(ex.Message + "\r\n From:" + "ReceiveMessage(IAsyncResult ar)");
                lock (mw.dw.debugBox)
                mw.dw.debugBox.AppendText(ex.Message + "\r\n From:" + "ReceiveMessage(IAsyncResult ar)" + "\r\n");
            }
        }
        

    }
}
