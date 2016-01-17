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
using System.Windows.Shapes;

using System.Threading;
using System.Net.Sockets;

namespace WpfMk2._0
{
    /// <summary>
    /// Interaction logic for HEXWindow.xaml
    /// </summary>
    public partial class HEXWindow : Window
    {
        MainWindow mw = null;
        socket socket = null;

        public delegate void HEX_Event_Handler(object sender, int index, byte com);
        public event HEX_Event_Handler Hex_Event;

        public HEXWindow(MainWindow mw,socket socket)
        {            
            InitializeComponent();
            this.mw = mw;
            this.socket = socket;
            

            uiAnimation.translation(this, warningLine0);
            uiAnimation.translation(this, warningLine1);
        }

        private void ButtonHEX_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke(new ThreadStart(() =>
            {
                try
                {
                    int index = Int32.Parse(TextBoxIndexHEX.Text);
                    //MessageBox.Show("INDEX: " + index.ToString() + "  BYTE: " + StringToByte(TextBoxHEX.Text).ToString() );
                    //Hex_Event(this, index, StringToByte(TextBoxHEX.Text));
                    socket.sendSingle(index, StringToByte(TextBoxHEX.Text));
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
            ));
            //Thread t = new Thread(new ThreadStart(() =>
            //{
            //    try
            //    {
            //        int index = Int32.Parse(TextBoxIndexHEX.Text);
            //        //MessageBox.Show("INDEX: " + index.ToString() + "  BYTE: " + StringToByte(TextBoxHEX.Text).ToString() );
            //        //Hex_Event(this, index, StringToByte(TextBoxHEX.Text));
            //        socket.sendSingle(index, StringToByte(TextBoxHEX.Text));
            //    }
            //    catch (Exception ee)
            //    {
            //        MessageBox.Show(ee.Message);
            //    }
            //}));
            //t.Start();
        }

        private void TopContainerBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) { this.DragMove(); }
        private void CloseWindowTextBlock_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) { this.Close(); }
        private void CloseWindowTextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) { e.Handled = true; }

        public byte StringToByte(String str)
        {
            char[] bytes = str.ToLower().ToCharArray();
            byte b = 0;

            if (bytes[0] >= 'a' && bytes[0] <= 'f')
            {
                b = (byte)(0x0a + (byte)(bytes[0] - 'a'));
            }
            else
                b += (byte)(bytes[0] - '0');

            b <<= 4;

            if (bytes[1] >= 'a' && bytes[1] <= 'f')
                b += (byte)(0x0a + (byte)(bytes[1] - 'a'));
            else
                b += (byte)(bytes[1] - '0');

            return b;
        }

        private void TopContainerBorder_Loaded(object sender, RoutedEventArgs e)
        {
            this.ConsoleTextBlock.Text = socket.ip;
        }

    }
}
