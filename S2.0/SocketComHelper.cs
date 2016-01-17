using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Windows.Forms;

namespace S2_0
{
    class SocketComHelper
    {
        public const int MK_FLAG_FILE_RECEIVED = 100;
	    public const int MK_FLAG_TEST = 900;

        static public List<Socket> sockets;// Working Sockets
        
        public static void initSockts(List<Socket> sockets)
        {
            SocketComHelper.sockets = sockets;
        }

        public static void transmitBytyArray(Socket socket,int direction,int bufferSize,byte[] data)
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                byte[] encapsulated;               //byte array actually to be sended
                byte[] head = new byte[16];        //package information are all here
                byte[] predata = new byte[1024];   //reserve for future use

                int space = 4;                     //bitS length of a numical data
                int size_last = 0;
                int data_length = data.Length;
                int count_package;
                if(data_length>bufferSize)
                {
                    count_package = (int)(data_length / bufferSize);
                    if (data_length % bufferSize != 0)
                    {
                        size_last = data_length - bufferSize * count_package;
                        count_package++;
                    }else
                    {
                        size_last = bufferSize;
                    }
                }else
                {
                    count_package = 1;
                    if (data_length < bufferSize)
                    {
                        size_last = data_length;
                    }else
                    {
                        size_last = bufferSize;
                    }
                }                    

                try
                {
                    fillArray(direction,     head, 0,  space);//DIRECTION
                    fillArray(count_package, head, 4,  space);//COUNT                    
                    fillArray(size_last,     head, 12, space);//SIZE OF LAST

                    for(int i = 1;i<=count_package;i++)
                    {
                        fillArray(i, head, 8, space);//INDEX

                        encapsulated = new byte[predata.Length + head.Length + bufferSize];//try to avoid 'new'
                        predata.CopyTo(encapsulated, 0);
                        head.CopyTo(encapsulated, predata.Length);

                        if(i<count_package)
                        {                            
                            Array.Copy(data, bufferSize * (i - 1), encapsulated, predata.Length + head.Length, bufferSize);
                        }else
                        {
                            encapsulated = new byte[predata.Length + head.Length + size_last];//size_last
                            predata.CopyTo(encapsulated, 0);
                            head.CopyTo(encapsulated, predata.Length);

                            if(count_package == 1)
                            {
                                Array.Copy(data, 0, encapsulated, predata.Length + head.Length, size_last);//size_last
                            }else
                            {                               
                                Array.Copy(data, bufferSize * (i - 1), encapsulated, predata.Length + head.Length, size_last);//size_last
                            }
                        }
                        
                        socket.Send(data);
                    }

                }
                catch (Exception e)
                {
                    LogBuilder.buildLog(e.Message + "\r\nFrom transmitBytyArray");
                    MessageBox.Show(e.Message + "\r\nFrom transmitBytyArray");
                }
            }));
            t.IsBackground = true;
            t.Start();   
        }

        public static void transmitCommand(Socket socket,int command,byte[] extradata)
        {
             Thread t = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        /*
                         * 0.sort of data
                         * 1.count of packets
                         * 2.current index 
                         * 3.size of last packet 
                         * 4.data start here!               
                         * */

                        int exlength;
                        if (extradata != null)
                            exlength = extradata.Length;
                        else
                            exlength = 0;

                        byte[] predata = new byte[1024];   //reserve for future use
                        byte[] head = new byte[16];
                        int space = 4;
                        fillArray(command, head, 0, space);//DIRECTION
                        fillArray(1, head, 4, space);//COUNT
                        fillArray(1, head, 8, space);//INDEX
                        fillArray(head.Length + exlength, head, 12, space);//SIZE OF LAST AND LAST IS THIS
                        LogBuilder.buildLog("Sending Command: " + getHead(head,0));
                        byte[] data = new byte[predata.Length + head.Length + exlength];//byte array to be send                 
                        //head.CopyTo(data, 0);

                        predata.CopyTo(data, 0);
                        head.CopyTo(data, predata.Length);

                        if(exlength > 0)
                            extradata.CopyTo(data, predata.Length + head.Length);

                        socket.Send(data);
                    }
                    catch (Exception e)
                    {
                        System.Media.SystemSounds.Hand.Play();
                        LogBuilder.buildLog(e.Message + "\r\nFrom transmitCommand");
                        MessageBox.Show(e.Message + "\r\nFrom: " + "transmitCommand");
                    }
                }));
                t.IsBackground = true;
                t.Start(); 
        }

        public static int getHead(byte[] array, int index)
        {
            int data = 0;
            int space = 4;

            for (int i = 0; i < space; i++)
            {
                //data |= (0xff) & array[i + index * space];
                data |= array[i + index * space];
                if (i < (space - 1))//at last loop we do not need shift data
                    data = (data) << 8;
            }
            return data;
        }

        static public void fillArray(int num, byte[] array, int offset, int space)
        {
            for (int i = 0; i < space; i++)
            {
                array[offset + i] = (byte)(num >> (8 * (space - 1 - i)));
            }
        }

    }
}
