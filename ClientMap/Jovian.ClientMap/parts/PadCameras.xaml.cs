using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jovian.ClientMap.parts
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Jovian.ClientMap.classes;
    using System.Windows.Media.Animation;
    /// <summary>
    /// PadCameras.xaml 的交互逻辑
    /// LPY 2015-10-8 添加
    /// </summary>
    public partial class PadCameras : UserControl
    {
        public PadCameras()
        {
            InitializeComponent();
            lstCameras.ItemsSource = PublicParams.listCameras;
        }

        //开关按钮
        private void btnSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (WallVideosHelper.GetEmptyWallVideo() == -1)//已打开满，不允许打开
                return;

            Button btnSwitch = (Button)sender;
            switch (btnSwitch.Name)
            {
                case "On":
                    //btnSwitch.Style = App.Current.Resources["btnOff"] as Style;                    //已上墙的视频不允许在此处关闭，要在已打开视频列表里关闭
                    //btnSwitch.Name = "Off";
                    break;
                case "Off":
                    btnSwitch.Style = App.Current.Resources["btnOn"] as Style;
                    WallVideosHelper.OpenVideoToWall(btnSwitch.Tag as Camera);
                    btnSwitch.Name = "On";
                    break;
                default:
                    break;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //this.Height = 1;
            //this.Width = 1;
            //PublicParams.pubCanvasChild1.Visibility = Visibility.Collapsed;
            PublicParams.pubCanvasChild1.BeginStoryboard(App.Current.FindResource("StoryboardForPadCamerasClose") as Storyboard);
        }
    }    
}
