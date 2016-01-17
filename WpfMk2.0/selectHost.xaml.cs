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
using System.Windows.Media.Animation;
using System.Drawing;

namespace WpfMk2._0
{
    /// <summary>
    /// Interaction logic for selectHost.xaml
    /// </summary>
    public partial class selectHost : Window
    {
        List<string> ipList;
        int con_ipList = 0;
        MainWindow mW;

        public delegate void pipeevent_Handler(object sender, int index);
        //用委托声明两个事件
        public event pipeevent_Handler pipe_Event;

        public List<string> IPLIST 
        { 
            get
            {
                return ipList;
            }
            set
            {
                ipList = value;
            }
        }


        public selectHost(object parent)
        {
            InitializeComponent();
            mW = parent as MainWindow;
            initWL();
            initDis();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            con_ipList = ipList.Count();
            addipList();            
        }

        void addipList()
        {
            int ipCount = 0;
            for(int i = 0;i<con_ipList;i++)
            {
                String ipName = "IP" + ipCount++.ToString();
                System.Drawing.Color color = System.Drawing.ColorTranslator.FromHtml("#FF8800");
                System.Windows.Media.Color mediaColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);

                TextBlock tb = new TextBlock()
                {
                    Name = ipName,
                    Foreground = new SolidColorBrush(mediaColor),
                    FontSize = 26,
                    Margin = new Thickness(0, 0, 0, 0),
                    Text = ipList[i],
                    FontWeight = FontWeights.Bold,
                    FontFamily = new System.Windows.Media.FontFamily("proxy 1"),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                this.RegisterName(ipName, tb);
                DisplayArea.Children.Add(tb);
                //TextBlock _tb_test = (TextBlock)DisplayArea.FindName("tb_test");

                tb.MouseLeftButtonUp += new MouseButtonEventHandler(IP_MouseLeftButtonUp);

            }

            window.Height += con_ipList * 20 + 12 * (con_ipList - 1);//20 is the height of font size in 26
            //downWarningLine.Margin = (0, 6, 0, 0);

            Thickness thick = downWarningLine.Margin;
            thick.Top += con_ipList * 20 + 12 * (con_ipList - 1); // 20 + 6 + 11
            downWarningLine.Margin = thick;

            DisplayAreaBlackBack.Height += con_ipList * 20 + 12 * (con_ipList - 1);

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

        void initDis()
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

            DoubleAnimationUsingKeyFrames dak = new DoubleAnimationUsingKeyFrames();
            //关键帧定义
            dak.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0))));
            dak.KeyFrames.Add(new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(0.5))));
            dak.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1))));

            dak.BeginTime = TimeSpan.FromSeconds(0);//从第2秒开始动画
            dak.RepeatBehavior = RepeatBehavior.Forever;
            //开始动画
            //this.warningLine.BeginAnimation(Border.OpacityProperty, dak);

            Storyboard.SetTargetName(dak, DisplayArea.Name);
            Storyboard.SetTargetProperty(dak, new PropertyPath(Border.OpacityProperty));

            myStoryboard.Children.Add(dak);

            myStoryboard.Begin(this);
        }

        private void FeedBackTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void CloseAppTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void CloseAppTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void FeedBackTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CloseAppTextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void TopContainerBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void IP_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            //mW.POSTBACKINDEX = ipList.IndexOf(tb.Text);
            pipe_Event(this, ipList.IndexOf(tb.Text));

            this.Close();
        }


    }
}
