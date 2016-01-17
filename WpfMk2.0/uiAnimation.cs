using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

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
    public class uiAnimation//This class are far from perfect,in perent it can only using anination on Border type
    {
        MainWindow mw = null;

        Storyboard myStoryboard = null;

        public uiAnimation(Object parent,Object Item)
        {

        }

        public static void translation(Object mw, Canvas canvas)
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
            canvas.RenderTransform = new TranslateTransform();
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
            Storyboard.SetTargetName(InDoubleAnimation, canvas.Name);
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

            Storyboard.SetTargetName(dak, canvas.Name);
            Storyboard.SetTargetProperty(dak, new PropertyPath(Border.OpacityProperty));


            myStoryboard.Children.Add(dak);
            myStoryboard.Children.Add(InDoubleAnimation);
            myStoryboard.Begin(mw as Window);
        }



    }
}
