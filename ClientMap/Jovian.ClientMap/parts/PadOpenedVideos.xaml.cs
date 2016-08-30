using System;
using System.Collections.Generic;
using System.ComponentModel;
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

using Jovian.ClientMap.classes;
namespace Jovian.ClientMap.parts
{
    /// <summary>
    /// PadOpenedVideos.xaml 的交互逻辑
    /// </summary>
    public partial class PadOpenedVideos : UserControl
    {
        public PadOpenedVideos()
        {
            InitializeComponent();

            lstOpenedVideos.ItemsSource = PublicParams.arrayOpenedVideos;//绑定已上墙视频列表

            lstOpenedVideosOfDVCS2.ItemsSource = PublicParams.arrayOpenedVideosDVCS2 ;
        }

        private void btnSwitch_Click(object sender, RoutedEventArgs e)
        {
            Button btnSwitch = (Button)sender;
            switch (btnSwitch.Name)
            {
                case "On":
                    btnSwitch.Style = App.Current.Resources["btnOff"] as Style;
                    if (btnSwitch.Tag == null)
                        break;
                    Camera camera = btnSwitch.Tag as Camera;
                    if (camera.Kind == PublicParams.dvcsServerMainName)
                        CloseOneVideo(camera);
                    else if (camera.Kind == PublicParams.dvcsServer2Name)
                        WallVideosHelper.CloseVideoToWallForDVCS2(camera);
                    btnSwitch.Tag = null;             
                    break;
                case "Off"://由于逻辑设定，不会执行到这段代码
                    //btnSwitch.Style = App.Current.Resources["btnOn"] as Style;
                    btnSwitch.Name = "On";
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// LPY 2016-3-31 添加
        /// 关闭一路视频
        /// </summary>
        /// <param name="camera">待关闭视频</param>
        private void CloseOneVideo(Camera camera)
        {
            if (camera == null)
                return;

            PublicParams.dvcsServerMain.SendCMD(DVCSAgreement.CloseWin(camera.WinID));
            MapMethods.SendShowHidePadVideosTextByID("", camera.ID + 1, "0");
            //LogHelper.WriteLog(string.Format("已发送指令，关闭WinID：{0}的视频--{1}",camera.WinID.ToString(),PublicParams.dvcsServerMainName));
            PublicParams.arrayOpenedVideos[camera.ID] = null;

            WallVideosHelper.CloseVideoToWallForDVCS2(camera);

            WallVideosHelper.RefreshOpenedVideos();
        }

        private void btnCloseAll_Click(object sender, RoutedEventArgs e)
        {
            MapMethods.SendClosePadVideos();//关闭大屏视频背景板
            for (int i = 0; i < PublicParams.arrayOpenedVideos.Length; i++)//逐个关闭视频
            {
                if (PublicParams.arrayOpenedVideos[i] == null)
                    return;
                CloseOneVideo(PublicParams.arrayOpenedVideos[i] as Camera);
            }            

            WallVideosHelper.InitOpenedVideos();
            WallVideosHelper.RefreshOpenedVideos();
        }

        //private void btnSwitchForDVCS2_Click(object sender, RoutedEventArgs e)
        //{
        //    Button btnSwitch = (Button)sender;
        //    switch (btnSwitch.Name)
        //    {
        //        case "On":
        //            btnSwitch.Style = App.Current.Resources["btnOff"] as Style;
        //            if (btnSwitch.Tag == null)
        //                break;
        //            Camera camera = btnSwitch.Tag as Camera;
        //            //CloseOneVideo(camera);
        //            WallVideosHelper.CloseVideoToWallForDVCS2(camera);
        //            btnSwitch.Tag = null;
        //            break;
        //        case "Off"://由于逻辑设定，不会执行到这段代码
        //            //btnSwitch.Style = App.Current.Resources["btnOn"] as Style;
        //            btnSwitch.Name = "On";
        //            break;
        //        default:
        //            break;
        //    }
        //}

        
    }

}
