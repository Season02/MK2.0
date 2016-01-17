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

namespace WpfMk2._0
{
    /// <summary>
    /// Interaction logic for DebugWindow.xaml
    /// </summary>
    public partial class DebugWindow : Window
    {
        MainWindow mw = null;
        public DebugWindow(MainWindow mw)
        {
            this.mw = mw;
            InitializeComponent();

            uiAnimation.translation(this, warningLine0);
            uiAnimation.translation(this, warningLine1);

            //mw.Debug_Event += new MainWindow.Debug_Event_Handler(debug);
            //mw.ipscan.Debug_Event += new ipScan.Debug_Event_Handler(debug);
        }

        void debug(Object sender,String msg)
        {
            this.debugBox.AppendText(msg);
        }

        private void CloseWindowTextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e){ e.Handled = true; }
        private void CloseWindowTextBlock_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e){ this.Close(); }
        private void TopContainerBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e){ this.DragMove(); }




    }
}
