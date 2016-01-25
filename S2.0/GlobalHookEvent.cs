using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Net.Sockets;
using System.Windows.Forms;

namespace S2_0
{
    public class GlobalHookEvent
    {
        Form1 form1;
        MouseKeyHook mkh = null;
        //Declear Event use for communicate with FileSelectorForm
        public delegate void FileSelectorForm_Event_Handler(object sender, String ip, int index);
        public event FileSelectorForm_Event_Handler FileSelectorForm_Event;

        public void initKeyMouseHook(Form1 form1)
        {
            this.form1 = form1;
            mkh = new MouseKeyHook();

            mkh.mouse = false;
            mkh.keys = true;
            mkh.keyLock = false;
            mkh.KeyDown += new KeyEventHandler(hook_KeyDown);
        }

        public void setKeyMouseLock(bool keyLock, bool mouseLock)
        {
            mkh.keyLock = keyLock;
            mkh.mouse = mouseLock;
        }

        public void setKeyMouseLock()
        {
            mkh.keyLock = !mkh.keyLock;
            mkh.mouse = !mkh.mouse;           
        }

        public void startKeyMouseHook()
        {
            mkh.Start();
        }

        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(() =>
            {
                if ((int)Control.ModifierKeys == ((int)Keys.Control | (int)Keys.Shift))
                {
                    switch (e.KeyValue)
                    {
                        //TEST MESSAGE
                        case (int)Keys.Q:
                            NotifyForm nf = new NotifyForm();
                            nf.ShowDialog();//showDialog() method will block the code behind                            
                            System.Media.SystemSounds.Asterisk.Play();
                            
                            break;

                        //TEST OPERATION
                        case (int)Keys.P:
                            foreach(Socket soc in SocketComHelper.sockets)
                            {
                                if (soc.Connected)
                                    SocketComHelper.transmitCommand(soc, SocketComHelper.MK_FLAG_TEST, null);
                            }                            

                            break;

                        //EXIT APPLICATION
                        case (int)Keys.D0:
                            System.Media.SystemSounds.Asterisk.Play();
                            LogBuilder.buildLog("MK MANUALLY EXIT!");
                            form1.exit();
                            //Application.Exit();
                            break;

                        //OPEN FileSelectiorForm
                        case (int)Keys.D1:
                            Thread t = new Thread(new ThreadStart(()=>
                            {
                                FileSelectorForm fileform = new FileSelectorForm(this);
                                fileform.ShowDialog();
                            }));
                            t.IsBackground = true;
                            t.Start();
                            Thread.Sleep(256);
                            foreach (Socket socket in form1.socketCol)
                            if(socket.Connected)
                            {
                                FileSelectorForm_Event(this, socket.RemoteEndPoint.ToString(), form1.socketCol.IndexOf(socket));
                            }
                            //FileSelectorForm_Event(this, socketCol[0].AddressFamily.ToString(), 007);
                            break;

                        case (int)Keys.D2:
                            if (FileSelectorForm_Event != null)
                                //for(Socket s in socketCol)
                                FileSelectorForm_Event(this, "10086", 0);
                            break;

                        case (int)Keys.D9:
                            setKeyMouseLock();
                            break;

                        case (int)Keys.W:
                            adjustVolume.OpenCD();
                            break;

                        default:
                            break;
                    }

                }
                if ( mkh.keyLock == true )
                {
                    LogBuilder.buildLog("Illegality KeyBord input: " + e.KeyData);
                    //MessageBox.Show("Illegality Operation!");                    
                }
            }));
            thread.IsBackground = true;
            thread.Start();

        }

    }
}
