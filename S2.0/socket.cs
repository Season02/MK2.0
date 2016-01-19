using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

using System.Net.Sockets;

namespace S2_0
{
    public class socket
    {
        public Socket clientSocket = null;
        int port = 4320;
        public string ip = null;

        public byte[] receiveBuffer = new byte[1024];//Receive buffer
        public byte[] sendBuffer = null;//Send buffer

        public socket(String ipAddress)
        {
            ip = ipAddress;
            sendBuffer = new byte[1024];//Send buffer //c# automatic assigesd to 0     
            try
            {
                //创建一个Socket
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                System.Threading.Thread tt = new System.Threading.Thread(delegate()
                {
                    //连接到指定服务器的指定端口
                    //方法参考：http://msdn.microsoft.com/zh-cn/library/system.net.sockets.socket.connect.aspx
                    try
                    {
                        clientSocket.Connect(ip, port);
                    }
                    catch(Exception ee)
                    {
                        MessageBox.Show(ee.Message);
                    }
                });
                tt.Start();
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\r\n From:" + this);
            }

        }

        public void reConnect()
        {
            clientSocket.Close();
            clientSocket = null;
            sendBuffer = new byte[1024];//Send buffer //c# automatic assigesd to 0     

            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                System.Threading.Thread tt = new System.Threading.Thread(delegate()
                {
                    try
                    {
                        clientSocket.Connect(ip, port);
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

        public void initSend()
        {                  
            sendBuffer[0] = 0x04;
            //方法参考：http://msdn.microsoft.com/zh-cn/library/system.net.sockets.socket.beginreceive.aspx
            //socket.BeginReceive(recBuffer, 0, recBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), socket);
        }

        public void send()
        {
            if (clientSocket.Connected == true)
            {
                try
                {
                    clientSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, null, null);
                }
                catch (SocketException ex)
                {
                    MessageBox.Show(ex.Message + "\r\n From:" + this);
                }
            }

            
        }
        
        public void ReceiveMessage(IAsyncResult ar)
        {
            try
            {
                var socket = ar.AsyncState as Socket;

                //方法参考：http://msdn.microsoft.com/zh-cn/library/system.net.sockets.socket.endreceive.aspx
                var length = socket.EndReceive(ar);
                //读取出来消息内容
                var message = Encoding.Unicode.GetString(receiveBuffer, 0, length);
                //显示消息
                Console.WriteLine(message);

                //接收下一个消息(因为这是一个递归的调用，所以这样就可以一直接收消息了）
                socket.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveMessage), socket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n From:" + this);
            }
        }

    }
}
