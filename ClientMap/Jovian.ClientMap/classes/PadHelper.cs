using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jovian.ClientMap.classes
{
    using Jovian.ClientMap.parts;
    using System.Windows;
    using System.Windows.Controls;
    /// <summary>
    /// LPY 2015-10-20 添加
    /// 该类完成初始化主地图上的信息窗功能
    /// </summary>
    public class PadHelper
    {
        public PadHelper()
        {
            //CreatePadDVCS();
        }

        public static void InitPads()
        {
            //CreatePadDVCS();
            CreatePadOpenedVideos();//已打开视频点列表
            CreatePadCameras();//查找到的摄像头列表
            CreatePadLayers();//图层列表
            CreatePadRemoteParams();
            //CreatePadRectParams();
            CreatePadCaseSwitch();
        }

        private static void CreatePadDVCS()
        {
            PadDVCS padDVCS = new PadDVCS() { Width = 500, Height = 400, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(500, 60, 0, 0), Name = "PadDVCS" };
            PublicParams.pubLayoutRoot.Children.Add(padDVCS);
        }

        private static void CreatePadOpenedVideos()
        {
            PadOpenedVideos padOpenedVideos = new PadOpenedVideos() { Width = 400, Height = 0, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 60, 75, 0), Name = "PadOpenedVideos" };
            PublicParams.padOpenedVideos = padOpenedVideos;
            PublicParams.pubLayoutRoot.Children.Add(padOpenedVideos);
        }

        private static void CreatePadCameras()
        {
            PadCameras padCameras = new PadCameras() { Width = 400, Height = 500, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0, 0, 0, 0), Name = "PadCameras" };
            PublicParams.padCameras = padCameras;
            PublicParams.pubCanvasChild1.Children.Add(padCameras);//为了能让padCameras随鼠标拖动            
        }

        private static void CreatePadLayers()
        {
            PadLayers padLayers = new PadLayers() { Width = 200, Height = 400, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 60, 125, 0), Name = "PadLayers",Visibility=Visibility.Hidden };
            PublicParams.padLayers = padLayers;
            PublicParams.pubLayoutRoot.Children.Add(padLayers);

        }

        private static void CreatePadRemoteParams()
        {
            PadRemoteParams padRemoteParams = new PadRemoteParams() { Width=1000,Height=550,VerticalAlignment=VerticalAlignment.Bottom,HorizontalAlignment=HorizontalAlignment.Left,Margin=new Thickness(-1000,0,0,10),Name="padRemoteParams" ,Visibility=Visibility.Visible};
            PublicParams.padRemoteParams = padRemoteParams;
            PublicParams.pubLayoutRoot.Children.Add(padRemoteParams);
        }

        private static void CreatePadRectParams()
        {
            PublicParams.padRectParams = new PadRectParams() { Width = 400, Height = 300, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Name = "padRectParams", Visibility = Visibility.Hidden };
            PublicParams.pubLayoutRoot.Children.Add(PublicParams.padRectParams);
            Canvas.SetZIndex(PublicParams.padRectParams, 65535);
        }

        private static void CreatePadCaseSwitch()
        {
            PublicParams.padCaseSwitch = new PadCaseSwitch() { Width = 200, Height = 220, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 60, 75, 0), Name = "padCaseSwitch", Visibility = Visibility.Hidden };
            PublicParams.pubLayoutRoot.Children.Add(PublicParams.padCaseSwitch);
        }
    }
}
