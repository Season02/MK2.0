using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows;
using System.IO;

namespace S2_0
{
    class TransmitFile
    {
        public Socket server = null;
        Thread SendThread = null;
        public int port = 4380;
        public Socket socket = null;

        public String FILENAME = "testfile";

        public TransmitFile()
        {
            try
            {
                //Thread t = new Thread(new ThreadStart(() =>
                //{
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    server.Bind(new IPEndPoint(IPAddress.Any, port));
                    server.Listen(20);
                    socket = server.Accept();
                    //MessageBox.Show("Transmin Socket Connected!");
                //}));
                //t.IsBackground = true;
                //t.Start();     
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message + "\r\nForm : " + this);
            }       
        }

        public int ReceiveFile(int size_file,int count_packet,int size_packet,int size_last_packet)
        {
            try
            {
                Thread t = new Thread(new ThreadStart(() =>
                {
                    FileStream MyFileStream = new FileStream(FILENAME, FileMode.OpenOrCreate, FileAccess.Write);
                    int count_received = 0;
                    byte[] data = new byte[size_packet];

                    //if (socket.Connected == true)
                    {
                        while(true)//for (int i = 0; i < count_packet;i++ )
                        {
                            if (socket.Receive(data) == 0)
                            {
                                socket.Close();
                                Thread.CurrentThread.Abort();
                                break;
                            }
                            if (data.Length == 0)
                            {
                                break;
                            }
                            else
                            {
                                count_received++;
                                //将接收到的数据包写入到文件流对象  
                                MyFileStream.Write(data, 0, data.Length);
                                //显示已发送包的个数  
                                //MessageBox.Show("已发送包个数"+SendedCount.ToString());  
                            }
                        }
                        if(size_last_packet != 0)
                        {
                            data = new byte[size_last_packet];
                            if(socket.Receive(data) == 0)
                            {
                                socket.Close();
                                Thread.CurrentThread.Abort();
                                MessageBox.Show("slp");
                            }
                            else
                            {
                                count_received++;
                                MyFileStream.Write(data, 0, data.Length);
                                MessageBox.Show("done");
                            }
                        }
                    }

                    MyFileStream.Close();
                    socket.Close();

                    MessageBox.Show("RECEIVED COUNT : " + count_received.ToString());

                }));
                t.IsBackground = true;
                t.Start();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\r\nForm : " + this);
            }


            return 1;
        }

    }
}
