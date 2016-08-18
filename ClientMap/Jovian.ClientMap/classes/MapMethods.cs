using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jovian.ClientMap.classes
{
    using System.Threading;

    using ESRI.ArcGIS.Client;
    using ESRI.ArcGIS.Client.Graphics;
    using ESRI.ArcGIS.Client.Geometry;
    using ESRI.ArcGIS.Client.Symbols;
    using ESRI.ArcGIS.Client.Toolkit;
    using ESRI.ArcGIS.Client.Tasks.Utils.JSON;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// LPY 2015-9-7 添加
    /// 提供对地图的各种静态操作方法：切换地图、放大缩小、平移、根据ID查找GraphicsLayer、FeatureLayer等
    /// </summary>
    public static class MapMethods
    {
        private static int currentLevel = 9;
        private static double nowX;
        private static double nowY;
        private static int nowLevel;
        /// <summary>
        /// LPY 2015-9-7 添加 地图平移缩放
        /// 
        /// </summary>
        /// <param name="json"></param>
        public static void MoveAndZoomMapByJson(JObject json)
        {
            MoveAndZoomMapByXYL(Convert.ToDouble(json["CENTX"].ToString()), Convert.ToDouble(json["CENTY"].ToString()), Convert.ToInt32(json["LEVEL"].ToString()));
        }

        /// <summary>
        /// LPY 2015-9-7 添加
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="level"></param>
        public static void MoveAndZoomMapByXYL(double x, double y, int level)
        {
            try
            {
                double ratio = 1.0;
                double resolution;
                level += PublicParams.Delta;
                if (level > 20)
                    level = 20;

                MapPoint pointCenter = new MapPoint(x, y);
                resolution = GetResoulution(level);

                if (PublicParams.pubMainMap.Resolution != 0.0)
                    ratio = resolution / PublicParams.pubMainMap.Resolution;
                if (Math.Abs(1.0 - ratio) < 0.0001)
                {
                    PublicParams.pubMainMap.PanTo(pointCenter);
                }
                else
                {
                    MapPoint pointMapCenter = PublicParams.pubMainMap.Extent.GetCenter();
                    double X = (pointCenter.X - ratio * pointMapCenter.X) / (1 - ratio);
                    double Y = (pointCenter.Y - ratio * pointMapCenter.Y) / (1 - ratio);

                    bool flagSL = PublicParams.pubMainMap.Layers["SL"].Visible;
                    bool flagYX = PublicParams.pubMainMap.Layers["YX"].Visible;

                    if (flagSL)
                    {
                        PublicParams.pubMainMap.Layers["SL"].Visible = false;
                        PublicParams.pubMainMap.ZoomToResolution(resolution, new MapPoint(X, Y));
                        PublicParams.pubMainMap.Layers["SL"].Visible = true;
                    }
                    if (flagYX)
                    {
                        PublicParams.pubMainMap.Layers["YX"].Visible = false;
                        PublicParams.pubMainMap.ZoomToResolution(resolution, new MapPoint(X, Y));
                        PublicParams.pubMainMap.Layers["YX"].Visible = true;
                    }
                }

                currentLevel = level;

            }
            catch (Exception)
            {
                LogHelper.WriteLog("MapMethods.cs-MoveAndZoomMapByJson-地图缩放平移操作出错！");
            }
        }

        public static void RefreshMainMapThread()
        {
            while (true)
            {
                if (PublicParams.pubX!=nowX||PublicParams.pubY!=nowY||PublicParams.pubLevel!=nowLevel)
                {
                    nowX = PublicParams.pubX;
                    nowY = PublicParams.pubY;
                    nowLevel = PublicParams.pubLevel;
                    PublicParams.pubMainMap.Dispatcher.BeginInvoke(new Action(delegate { 
                        MoveAndZoomMapByXYL(nowX, nowY, nowLevel);
                    }));                    
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static double GetResoulution(int level)
        {
            double ResolutionLV20 = 0.0000019073515436137569;
            return (ResolutionLV20 * Math.Pow(2, 20 - level));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static int GetLevel(double resolution)
        {
            double ResolutionLV20 = 0.0000019073515436137569;
            //return (ResolutionLV20 * Math.Pow(2, 20 - level));
            return Convert.ToInt32(20 - Math.Log((resolution / ResolutionLV20), 2));
        }

        /// <summary>
        /// LPY 2015-9-7 添加 地图切换底图，目前在矢量、影像之间切换
        /// </summary>
        /// <param name="json"></param>
        public static void ChangeMapByJson(JObject json)
        {
            try
            {
                string strMapName = json["WITCHMAP"].ToString();
                if (strMapName == "slmap")
                {
                    PublicParams.pubMainMap.Layers["SL"].Visible = true;
                    PublicParams.pubMainMap.Layers["YX"].Visible = false;
                }
                else
                {
                    PublicParams.pubMainMap.Layers["SL"].Visible = false;
                    PublicParams.pubMainMap.Layers["YX"].Visible = true;
                }
            }
            catch (Exception)
            {
                LogHelper.WriteLog("MapMethods.cs-ChangeMapByJson-地图底图切换操作出错！");
            }            
        }

        /// <summary>
        /// LPY 2016-4-14 录入
        /// 根据半径返回一个圆形范围，用来查找
        /// </summary>
        /// <param name="radius">半径</param>
        /// <param name="centerP">中心点</param>
        /// <returns></returns>
        public static Geometry GetEllipseGeometry(double radius, MapPoint centerP)
        {
            try
            {
                PointCollection pCollection = new PointCollection();
                for (double i = 0; i <= 360; i += 1)
                    pCollection.Add(new MapPoint((centerP.X - Math.Cos(Math.PI * i / 180.0) * radius), (centerP.Y - Math.Sin(Math.PI * i / 180.0) * radius)));

                Polygon g = new Polygon();
                g.Rings.Add(pCollection);
                return g;
            }
            catch (Exception)
            {
                LogHelper.WriteLog("MapMethods.cs-GetEllipseGraphic-生成圆形范围出错！");
                return null;
            }
        }

        public static void CloseInfoWin()
        {
            PublicParams.pubInfoWin.IsOpen = false;
        }

        public static void SendCurrentPosition()
        {
            if (!PublicParams.CanSend)
                return;
            TCPServer.SendCMD(GetCurrentPositionJson());
            //LogHelper.WriteLog(PublicParams.type, "发到大屏：" + strMapCenter);
        }

        public static string GetCurrentPositionJson()
        {
            MapPoint MapCenter = PublicParams.pubMainMap.Extent.GetCenter();
            return string.Format("{{CMD:'00001',CENTX:{0},CENTY:{1},LEVEL:{2}}}", MapCenter.X.ToString(), MapCenter.Y.ToString(), MapMethods.GetLevel(PublicParams.pubMainMap.Resolution));
            
        }

        public static void SendCurrentState(string state)
        {
            if (!PublicParams.CanSend)
                return;
            string strMapState = string.Format("{{CMD:'00000',WITCHMAP:'{0}'}}", state);
            TCPServer.SendCMD(strMapState);
        }

        /// <summary>
        /// LPY 2015-9-17 添加
        /// 发送指定Graphic信息给大屏
        /// </summary>
        public static void SendGraphic(Graphic graphic)
        {
            if (!PublicParams.CanSend)
                return;
            try
            {
                string strToSend = string.Format("{{CMD:'00105',TYPE:'{0}',DISTANCE:'{1}',GRAPHIC:{2}}}", graphic.Geometry.GetType().Name, 50, JsonConvert.SerializeObject(graphic));
                TCPServer.SendCMD(strToSend);                
            }
            catch (Exception)
            {
            }
        }

        public static void SendClearBufferLayer()
        {
            if (!PublicParams.CanSend)
                return;
            try
            {
                string strToSend = "{CMD:'00106'}";
                TCPServer.SendCMD(strToSend);
            }
            catch (Exception)
            {
            }            
        }

        public static void SendClearLengthOrAreaLayer()
        {
            if (!PublicParams.CanSend)
                return;
            string strToSend = "{CMD:'00109'}";
            TCPServer.SendCMD(strToSend);
        }

        /// <summary>
        /// 发送窗口选择
        /// 警情监控窗口
        /// </summary>
        /// <param name="item"></param>
        /// <param name="_switch">1：打开  0：关闭</param>
        public static void SendSwitchWin(string item,string _switch)
        {
            if (!PublicParams.CanSend)
                return;
            try
            {
                string strToSend = string.Format("{{CMD:'10003',ITEM:'{0}',SWITCH:'{1}'}}",item,_switch);
                TCPServer.SendCMD(strToSend);
            }
            catch (Exception)
            {
            }
        }

        //打开大屏视频背景板
        public static void SendOpenPadVideos()
        {
            if (!PublicParams.CanSend)
                return;
            string strToSend = "{CMD:'00037'}";
            TCPServer.SendCMD(strToSend);
        }

        //关闭大屏视频背景板
        public static void SendClosePadVideos()
        {
            if (!PublicParams.CanSend)
                return;
            string strToSend = "{CMD:'00038'}";
            TCPServer.SendCMD(strToSend);
        }

        //打开大屏InfoWindow
        public static void SendOpenInfoWindow(Graphic graphic)
        {
            if (!PublicParams.CanSend)
                return;
            try
            {
                string strToSend = string.Format("{{CMD:'00107',CLASS:'{0}',GRAPHIC:{1}}}", graphic.Attributes["Class"].ToString(),  JsonConvert.SerializeObject(graphic));
                TCPServer.SendCMD(strToSend);
            }
            catch (Exception)
            {
            }
        }

        //
        public static void SendCloseInfoWindow()
        {
            if (!PublicParams.CanSend)
                return;
            string strToSend = "{CMD:'00108'}";
            TCPServer.SendCMD(strToSend);
        }

        public static void SendSwitchClusterLayerByID(string layerID, string _switch)
        {
            if (!PublicParams.CanSend)
                return;
            string strToSend = string.Format("{{CMD:'00029',LAYERID:'{0}',SWITCH:'{1}'}}", layerID, _switch);
            TCPServer.SendCMD(strToSend);
        }

        public static void SendSwitchHeatMapLayerByID(string layerID, string _switch)
        {
            if (PublicParams.CanSend)
            {
                string strToSend = string.Format("{{CMD:'00030',LAYERID:'{0}',SWITCH:'{1}'}}", layerID, _switch);
                TCPServer.SendCMD(strToSend);
            }
        }
        //大屏图层开关
        public static void SendSwitchFeatureLayerByID(string layerID,string _switch)
        {
            if (PublicParams.CanSend)
            {
                string strToSend = string.Format("{{CMD:'00031',LAYERID:'{0}',SWITCH:'{1}'}}",layerID,_switch);
                TCPServer.SendCMD(strToSend);
            }
        }

        public static void SendSwitchGraphicsLayerByID(string layerID, string _switch)
        {
            if (PublicParams.CanSend)
            {
                string strToSend = string.Format("{{CMD:'00032',LAYERID:'{0}',SWITCH:'{1}'}}", layerID, _switch);
                TCPServer.SendCMD(strToSend);
            }
        }

        public static void SendClearGraphicsLayerByID(string layerID)
        {
            if (PublicParams.CanSend)
            {
                string strToSend = string.Format("{{CMD:'00033',LAYERID:'{0}'}}", layerID);
                TCPServer.SendCMD(strToSend);
            }
        }

        public static void SendShowHidePadVideosTextByID(string title, int id, string _switch)
        {
            if (PublicParams.CanSend)
            {
                string strToSend = string.Format("{{CMD:'00039',TITLE:'{0}',ID:'{1}',SWITCH:'{2}'}}",title,id,_switch);
                TCPServer.SendCMD(strToSend);
            }
        }
        
        //心跳保持与大屏端的连接
        internal static void SendHeartBeatToServer(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            string strToSend = "{CMD:'99999'}";
            SendMsg(strToSend);
        }

        //发送大屏开窗参数
        internal static void SendChangePadParams(double w, double h, double x,double y,int zindex,string name,int fontsize)
        {
            string strToSend = string.Format("{{CMD:'00041',Width:'{0}',Height:'{1}',X:'{2}',Y:'{3}',Zindex:'{4}',Name:'{5}',FontSize:'{6}'}}", w * 10, h * 10, x * 10, y * 10, zindex, name, fontsize);
            SendMsg(strToSend);
        }

        internal static void SendModularsToServer(RemoteWin[] rws)
        {
            string strToSend = string.Format("{{CMD:'00042',Modulars:'{0}'}}",Convert.ToBase64String( Encoding.Default.GetBytes( JsonHelper.ToJson(rws))));
            SendMsg(strToSend);
        }

        internal static void SendChangePadParamsAndName(string oldName, string newName, double w, double h, double x, double y, int zindex, int fontsize)
        {
            string strToSend = string.Format("{{CMD:'00043',Width:'{0}',Height:'{1}',X:'{2}',Y:'{3}',Zindex:'{4}',FontSize:'{5}',OldName:'{6}',NewName:'{7}'}}", w * 10, h * 10, x * 10, y * 10, zindex,  fontsize, oldName, newName);
            SendMsg(strToSend);
        }

        internal static void SendLengthOrAreaResult(Graphic graphic, string result, string type)
        {//"{{CMD:'00105',TYPE:'{0}',DISTANCE:'{1}',GRAPHIC:{2}}}", graphic.Geometry.GetType().Name, 50, JsonConvert.SerializeObject(graphic)
            string strToSend = string.Format("{{CMD:'00110',TYPE:'{0}',RESULT:'{1}',GRAPHIC:{2}}}",type,result,JsonConvert.SerializeObject(graphic));
            SendMsg(strToSend);
        }

        internal static void SendGraphicSearchCameras(Graphic graphic)
        {
            string strToSend = string.Format("{{CMD:'00111',GRAPHIC:{0}}}",JsonConvert.SerializeObject(graphic));
            SendMsg(strToSend);
        }

        public static void SendMsg(string strToSend)
        {
            if (PublicParams.CanSend)
                TCPServer.SendCMD(strToSend);
        }
    }
}
