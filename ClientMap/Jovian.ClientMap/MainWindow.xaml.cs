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
using System.Windows.Navigation;
using System.Windows.Shapes;

using ESRI.ArcGIS.Client;
using ESRI.ArcGIS.Client.Local;
using ESRI.ArcGIS.Client.Toolkit;
using ESRI.ArcGIS.Client.Geometry;
using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.Client.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Threading;
using System.Windows.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Controls.Primitives;

using Jovian.ClientMap.classes;
using Jovian.ClientMap.parts;
using System.Windows.Media.Animation;
namespace Jovian.ClientMap
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Socket socketBigMapServer = null;
        private Task threadListenToServerByTCP = null;
        DispatcherTimer timerHeartBeat = new DispatcherTimer();

        private Socket socketDVCSServer = null;

        private static Envelope envelop = new Envelope(121.1, 28.5, 121.38, 28.65) ;//院桥：121.1, 28.5, 121.38, 28.65     舟山定海视频点：122.15,29.97,122.24,30.02


        private Draw drawBuffer;        
        private Symbol activeSymbolBuffer = null;

        private Draw drawLenOrArea;
        private Symbol activeSymoblLenOrArea = null;

        public MainWindow()
        {
            InitializeComponent();

            LoadVectorMap(); //加载矢量地图
            LoadImageMap();//加载影像地图-------考虑异步加载，节省程序加载时间
            
            PublicParams.pubMainMap = mainMap;
            PublicParams.pubLayoutRoot = LayoutRoot;
            PublicParams.pubInfoWin = mainInfoWindow;
            PublicParams.pubCanvasChild1 = canvasChild1;

            if (PublicParams.IsInitLayers != "0")
            {
                MapLayers.InitMapLayers();//初始化地图所需的图层，静态方法
            }
            
            //ShowOneCrimePoint();//添加一个模拟的案件点

            Task taskConnectDVCSServer = new Task(ConnectDVCSServerInTask);//DVCS服务器连接相关
            taskConnectDVCSServer.Start();

            PadHelper.InitPads();//初始化信息窗
            WallVideosHelper.InitOpenedVideos();//初始化已打开视频列表
            

            PoliceCarGPS gps = new PoliceCarGPS();//MQ相关，接收警车GPS信号
            Cases cases = new Cases();//MQ相关，接收案件信息
            Traffic traffic = new Traffic();

            drawBuffer = new Draw(mainMap) {
                LineSymbol = App.Current.Resources["DrawLineSymbol"] as LineSymbol,
                FillSymbol = App.Current.Resources["DrawFillSymbol"] as FillSymbol
            };
            drawBuffer.DrawComplete += drawBuffer_DrawComplete;

            drawLenOrArea = new Draw(PublicParams.pubMainMap)
            {
                LineSymbol = App.Current.Resources["DrawLineSymbol"] as LineSymbol,
                FillSymbol = App.Current.Resources["DrawFillSymbol"] as FillSymbol
            };
            drawLenOrArea.DrawComplete += drawLenOrArea_DrawComplete;

        }

        void drawLenOrArea_DrawComplete(object sender, DrawEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Geometry == null)
                return;

            Graphic graphic = new Graphic() { Geometry = e.Geometry, Symbol = activeSymoblLenOrArea };
            List<Graphic> listGraphics = new List<Graphic>();
            listGraphics.Add(graphic);
            switch (e.Geometry.GetType().Name)
            {
                case "Polyline"://                    
                    GeoServHelper gshPolyline = new GeoServHelper();
                    gshPolyline.geometryTask.LengthsAsync(listGraphics, LinearUnit.Kilometer, CalculationType.Geodesic, graphic);
                    break;
                case "Polygon"://测面积
                    //listGraphics.Add(graphic);
                    GeoServHelper gshPolygon = new GeoServHelper();
                    gshPolygon.geometryTask.AreasAndLengthsAsync(listGraphics, LinearUnit.Meter, LinearUnit.Kilometer, CalculationType.Geodesic, graphic);
                    break;
                default:
                    break;
            }

            //drawLenOrArea.DrawMode = DrawMode.None;
        }

        private void ShowOneCrimePoint()
        {
            GraphicsLayer CrimePointLayer = new GraphicsLayer()
            {
                ID = "FocusLayer",
                DisplayName = "Focus"
            };
            
            mainMap.Layers.Add(CrimePointLayer);
            Graphic CrimeSymbol = new Graphic();
            CrimeSymbol.Geometry = new MapPoint(121.2481, 28.5545, new SpatialReference(4326));
            CrimeSymbol.Symbol = App.Current.Resources["CrimePointSymbol"] as Symbol;
            
            CrimePointLayer.Graphics.Add(CrimeSymbol);

            mainInfoWindow.IsOpen = false;
            mainInfoWindow.Anchor = new MapPoint(121.2481, 28.5545);
            mainInfoWindow.ContentTemplate = this.Resources["DT1"] as DataTemplate;
            mainInfoWindow.Content = JObject.Parse("{\"MC\":\"InfoWindow\",\"Age\":\"卢平义\",\"Time\":\"2015-9-28\"}");
            mainInfoWindow.IsOpen = true;

            //SimpleRenderer render = new SimpleRenderer() { Symbol = (Symbol)App.Current.Resources["pmsForCamerasLayer"] };
            //FeatureLayer featureLayer = new FeatureLayer() { ID = "CamerasLayer", Url = PublicParams.urlCamerasLayer, Where = " 1=1 ", Renderer = render, Visible = true };
            //mainMap.Layers.Add(featureLayer);
        }

        /// <summary>
        /// LPY 2015-11-13 修改 把建立与DVCS服务器的过程异步进行
        /// </summary>
        private void ConnectDVCSServerInTask()
        {
            if (ConnectToServer(ref socketDVCSServer, PublicParams.DVCSIP, PublicParams.DVCSPort))//建立与DVCS服务器的连接
            {
                DVCSServer.socketDVCSServer = socketDVCSServer;
                Task listenToDVCSServer = new Task(DVCSServer.ListenToDVCSServer, TaskCreationOptions.LongRunning);
                listenToDVCSServer.Start();
            }
        }

        void drawBuffer_DrawComplete(object sender, DrawEventArgs e)
        {
            if (e.Geometry == null)
                return;

            Graphic graphic = new Graphic() { Geometry = e.Geometry, Symbol = activeSymbolBuffer };
            switch (e.Geometry.GetType().Name)
            {
                case "Polyline"://根据画线类型，如果为line类型，则对line添加buffer
                    BufferParameters bufferParams = new BufferParameters() { BufferSpatialReference = new SpatialReference(102113), OutSpatialReference = new SpatialReference(4326), Unit = LinearUnit.Meter };
                    bufferParams.Distances.Add(PublicParams.BufferRadius);
                    bufferParams.Features.Add(graphic);
                    GeoServHelper gshPolyline = new GeoServHelper();
                    gshPolyline.geometryTask.BufferAsync(bufferParams);
                    break;
                case "Polygon":
                    this.Dispatcher.BeginInvoke(new Action(() => { 
                        GeoServHelper gshPolygon = new GeoServHelper();
                        gshPolygon.ExecuteAsyncQueryForDrawing(e.Geometry, PublicParams.urlCamerasLayer);
                    }));
                    break;
                default:
                    break;
            }
            MapLayers.AddGraphicToGLayerByLayerID(graphic, PublicParams.gLayerDrawing);
            
            MapMethods.SendGraphic(graphic);//同步画线到大屏
            drawBuffer.DrawMode = DrawMode.None;
            PublicParams.pubCanvasChild1.BeginStoryboard(App.Current.FindResource("StoryboardForPadCamerasOpen") as Storyboard);
            MapMethods.SendOpenPadVideos();//大屏同步弹出视频背景板
        }

        /// <summary>
        /// LPY 2015-9-6 添加
        /// 加载矢量地图作为底图
        /// </summary>
        private void LoadVectorMap()
        {
            TiledMapHelper vectorMap = new TiledMapHelper()
            {
                Url = PublicParams.urlVectorMap,
                ID = "SL",
                Visible = true,
                EnableOffline = true,
                SaveOfflineTiles = true,
                LoadOfflineTileFirst = true
            };
            mainMap.Layers.Add(vectorMap);
            mainMap.Extent = envelop;
        }

        /// <summary>
        /// LPY  2015-9-6 添加
        /// 加载影像地图
        /// </summary>
        private void LoadImageMap()
        {
            TiledMapHelper imageMap = new TiledMapHelper()
            {
                Url = PublicParams.urlImageMap,
                ID = "YX",
                Visible = false,
                EnableOffline = true,
                SaveOfflineTiles = true,
                LoadOfflineTileFirst = true
            };
            mainMap.Layers.Add(imageMap);
            mainMap.Extent = envelop;
        }

        private void mainMap_ExtentChanged(object sender, ExtentEventArgs e)
        {
            //MapPoint mp = PublicParams.pubMainMap.Extent.
            ////this.Title = mp.X.ToString() + "," + mp.Y.ToString();
            //if (mp.X < 121.25)
            //    mp.X = 121.25;
            //if (mp.X > 121.28)
            //    mp.X = 121.28;
            //if (mp.Y < 28.55)
            //    mp.Y = 28.55;
            //if (mp.Y > 28.56)
            //    mp.Y = 28.56;
            //string strCenterJson = string.Format("{{CMD:'00001',CENTX:{0},CENTY:{1},LEVEL:{2}}}", mp.X.ToString(), mp.Y.ToString(), MapMethods.GetLevel(PublicParams.pubMainMap.Resolution));
            //MapMethods.MoveAndZoomMapByJson(JObject.Parse(strCenterJson));
            MapMethods.SendCurrentPosition();            
        }

        /// <summary>
        /// LPY 2015-9-15 点击底图切换按钮，更改按钮样式，切换地图，并发送指令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSwitch_Click(object sender, RoutedEventArgs e)
        {
            switch (btnSwitch.Tag.ToString())
            {
                case "Vector":
                    PublicParams.pubMainMap.Layers["SL"].Visible = false;
                    PublicParams.pubMainMap.Layers["YX"].Visible = true;
                    btnSwitch.Style = this.FindResource("btnImage") as Style;
                    btnSwitch.Tag = "Image";
                    MapMethods.SendCurrentState("symap");
                    break;
                case "Image":
                    PublicParams.pubMainMap.Layers["SL"].Visible = true;
                    PublicParams.pubMainMap.Layers["YX"].Visible = false;
                    btnSwitch.Style = this.FindResource("btnVector") as Style;
                    btnSwitch.Tag = "Vector";
                    MapMethods.SendCurrentState("slmap");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// LPY 2015-9-17 添加
        /// 根据参数连接到服务器
        /// </summary>
        /// <returns></returns>
        public bool ConnectToServer(ref Socket socket,string ip,int port)
        {
            try
            {
                if (socket == null)
                {
                    IPAddress IP = IPAddress.Parse(ip);
                    int Port = port;
                    IPEndPoint ipEP = new IPEndPoint(IP, Port);
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(ipEP);

                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                socket = null;
                return false;
            }
        }

        /// <summary>
        /// 从大屏断开连接
        /// </summary>
        /// <returns></returns>
        public bool DisconnectToServer(ref Socket socket)
        {
            try
            {
                if (socket != null)
                {
                    socket.Close();
                    socket.Dispose();
                    socket = null;
                    return true;
                }
                else
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// LPY 2015-9-18 添加
        /// 主要按钮单击事件处理
        /// 连接大屏按钮、回到原点、警情、警车、图层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMain_Click(object sender, RoutedEventArgs e)
        {
            Button btnMain = sender as Button;
            switch (btnMain.Tag.ToString())
            {
                case "NotConnected":
                    if (ConnectToServer(ref socketBigMapServer,PublicParams.targetIP,PublicParams.targetPort))
                    {
                        TCPServer.socketServer = socketBigMapServer;
                        threadListenToServerByTCP = new Task(TCPServer.ListenToServer);
                        threadListenToServerByTCP.Start();
                        //Timer timer = new Timer(MapMethods.SendHeartBeatToServer, null, 0, 1000 * 60);
                        timerHeartBeat.Tick += MapMethods.SendHeartBeatToServer; timerHeartBeat.Interval = TimeSpan.FromSeconds(60 * 5); timerHeartBeat.Start();
                        PublicParams.CanSend = true;
                        switch (MessageBox.Show("同步大屏地图位置到本地吗？", "地图同步", MessageBoxButton.YesNo, MessageBoxImage.Question))
                        {
                            case MessageBoxResult.No:
                                MapMethods.SendCurrentPosition();
                                break;
                            case MessageBoxResult.Yes:
                                TCPServer.SendCMD("{\"CMD\":\"SYNC\"}");
                                break;
                            default:
                                break;
                        }
                        btnMain.Style = this.FindResource("btnConnected") as Style;
                        btnMain.Tag = "Connected";
                    }
                    break;
                case "Connected":
                    if (DisconnectToServer(ref socketBigMapServer))
                    {
                        TCPServer.socketServer = null;
                        PublicParams.CanSend = false;
                        btnMain.Style = this.FindResource("btnNotConnected") as Style;
                        btnMain.Tag = "NotConnected";
                    }
                    break;
                case "Origin"://回到原点按钮
                    mainMap.Extent = envelop;
                    MapMethods.SendCurrentPosition();
                    break;
                case "CaseOn"://警情关闭
                    PublicParams.padCaseSwitch.Visibility = Visibility.Hidden;
                    
                    btnMain.Tag = "CaseOff";
                    break;
                case "CaseOff"://警情打开
                    PublicParams.padCaseSwitch.Visibility = Visibility.Visible;
                    
                    btnMain.Tag = "CaseOn";
                    break;
                case "LayersOff"://图层列表打开
                    PublicParams.padLayers.Visibility = Visibility.Visible;
                    //PublicParams.padLayers.BeginStoryboard(App.Current.FindResource("StoryboardForPadLayersOpen") as Storyboard);
                    btnMain.Tag = "LayersOn";
                    break;
                case "LayersOn"://图层列表关闭
                    PublicParams.padLayers.Visibility = Visibility.Hidden;
                    //PublicParams.padLayers.BeginStoryboard(App.Current.FindResource("StoryboardForPadLayersClose") as Storyboard);
                    btnMain.Tag = "LayersOff";
                    break;
                case "OpenedVideosOff":
                    PublicParams.padOpenedVideos.BeginStoryboard(App.Current.FindResource("StoryboardForPadOpenedVideosOpen") as Storyboard);
                    btnMain.Tag = "OpenedVideosOn";
                    break;
                case "OpenedVideosOn":
                    PublicParams.padOpenedVideos.BeginStoryboard(App.Current.FindResource("StoryboardForPadOpenedVideosClose") as Storyboard);
                    btnMain.Tag = "OpenedVideosOff";
                    break;
                case "PoliceCarOff"://警车图层打开
                    MapLayers.ShowHideGraphicsLayerByID(PublicParams.gLayerPoliceCarGPS, true);
                    MapMethods.SendSwitchGraphicsLayerByID(PublicParams.gLayerPoliceCarGPS, "1");
                    btnMain.Tag = "PoliceCarOn";
                    break;
                case "PoliceCarOn"://警车图层关闭
                    MapLayers.ShowHideGraphicsLayerByID(PublicParams.gLayerPoliceCarGPS, false);
                    MapMethods.SendSwitchGraphicsLayerByID(PublicParams.gLayerPoliceCarGPS, "0");
                    btnMain.Tag = "PoliceCarOff";
                    break;
                case "TrafficOff"://交通-路况和红绿灯
                    MapLayers.ShowHideFeatureLayerByID(PublicParams.fLayerTrafficLight, true);
                    MapLayers.ShowHideFeatureLayerByID(PublicParams.fLayerRoad, true);
                    MapMethods.SendSwitchFeatureLayerByID(PublicParams.fLayerTrafficLight, "1");
                    MapMethods.SendSwitchFeatureLayerByID(PublicParams.fLayerRoad, "1");
                    btnMain.Style = this.Resources["btnTrafficOn"] as Style;
                    btnMain.Tag = "TrafficOn";
                    break;
                case "TrafficOn":
                    MapLayers.ShowHideFeatureLayerByID(PublicParams.fLayerTrafficLight, false);
                    MapLayers.ShowHideFeatureLayerByID(PublicParams.fLayerRoad, false);
                    MapMethods.SendSwitchFeatureLayerByID(PublicParams.fLayerTrafficLight, "0");
                    MapMethods.SendSwitchFeatureLayerByID(PublicParams.fLayerRoad, "0");
                    btnMain.Style = this.Resources["btnTraffic"] as Style;
                    btnMain.Tag = "TrafficOff";
                    break;
                default :
                    break;
            }
        }

        private void Tools_Click(object sender, RoutedEventArgs e)
        {
            //Button btnMain = sender as Button;
            switch ((sender as Button).Tag.ToString())
            {
                case "DrawCircle":
                    drawBuffer.DrawMode = DrawMode.Circle;
                    activeSymbolBuffer = PublicParams.fillSymbol;
                    break;
                case "DrawEllipse":
                    drawBuffer.DrawMode = DrawMode.Ellipse;
                    activeSymbolBuffer = PublicParams.fillSymbol;
                    break;
                case "DrawFreehandLine":
                    drawBuffer.DrawMode = DrawMode.Freehand;
                    activeSymbolBuffer = PublicParams.lineSymbol;
                    break;
                case "DrawPolyline":
                    drawBuffer.DrawMode = DrawMode.Polyline;
                    activeSymbolBuffer = PublicParams.lineSymbol;
                    break;
                case "DrawPolygon":
                    drawBuffer.DrawMode = DrawMode.Polygon;
                    activeSymbolBuffer = PublicParams.fillSymbol;
                    break;
                case "DrawRectangle":
                    drawBuffer.DrawMode = DrawMode.Rectangle;
                    activeSymbolBuffer = PublicParams.fillSymbol;
                    break;
                
                default:
                    drawBuffer.DrawMode = DrawMode.None;
                    MapLayers.ClearGLayerByID(PublicParams.gLayerDrawing);
                    MapMethods.SendClearBufferLayer();//清除大屏BufferLayer上的Buffers
                    break;
            }
            drawBuffer.IsEnabled = (drawBuffer.DrawMode != DrawMode.None);
        }

        private void UserTools_Click(object sender, RoutedEventArgs e)
        {
            Button btnMain = sender as Button;
            switch (btnMain.Tag.ToString())
            {
                case "OpenTools":
                    borMapTools.BeginStoryboard(App.Current.FindResource("StoryboardForToolsOpen") as Storyboard);
                    btnMain.Style = this.FindResource("btnRight") as Style;
                    btnMain.Tag = "CloseTools";
                    break;
                case "CloseTools":
                    borMapTools.BeginStoryboard(App.Current.FindResource("StoryboardForToolsClose") as Storyboard);
                    btnMain.Style = this.FindResource("btnLeft") as Style;
                    btnMain.Tag = "OpenTools";
                    break;
                case "OpenUserTools":
                    borMapUserTools.BeginStoryboard(App.Current.FindResource("StoryboardForUserToolsOpen") as Storyboard);
                    btnMain.Style = this.FindResource("btnRight") as Style;
                    btnMain.Tag = "CloseUserTools";
                    break;
                case "CloseUserTools":
                    borMapUserTools.BeginStoryboard(App.Current.FindResource("StoryboardForUserToolsClose") as Storyboard);
                    btnMain.Style = this.FindResource("btnLeft") as Style;
                    btnMain.Tag = "OpenUserTools";
                    break;
                default:
                    drawLenOrArea.DrawMode = DrawMode.None;
                    MapLayers.ClearGLayerByID(PublicParams.gLayerLengthOrArea);
                    MapMethods.SendClearLengthOrAreaLayer();
                    break;
            }
        }

        private void DrawLenOrArea_Click(object sender, RoutedEventArgs e)
        {
            //MapLayers.ClearGLayerByID(PublicParams.gLayerLengthOrArea);
            //MapMethods.SendClearLengthOrAreaLayer();
            switch ((sender as Button).Tag.ToString())
            {
                case "DrawLength":
                    drawLenOrArea.DrawMode = DrawMode.Polyline;
                    activeSymoblLenOrArea = PublicParams.lengthSymbol;
                    break;
                case "DrawArea":
                    drawLenOrArea.DrawMode = DrawMode.Polygon;//Polygon;Arrow
                    activeSymoblLenOrArea = PublicParams.areaSymbol;
                    break;
                default:
                    drawLenOrArea.DrawMode = DrawMode.None;
                    break;
            }
            drawLenOrArea.IsEnabled = drawLenOrArea.DrawMode != DrawMode.None;
        }

        private void btnCloseInfoWindow_Click(object sender, RoutedEventArgs e)
        {
            //mainInfoWindow.IsOpen = false;
            MapMethods.CloseInfoWin();
            MapMethods.SendCloseInfoWindow();
            //
        }

        Point oldPoint = new Point();
        bool isMove = false;

        private void canvasChild1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMove)
            {
                FrameworkElement currEle = sender as FrameworkElement;
                double xPos = e.GetPosition(null).X - oldPoint.X + (double)currEle.GetValue(Canvas.LeftProperty);
                double yPos = e.GetPosition(null).Y - oldPoint.Y + (double)currEle.GetValue(Canvas.TopProperty);
                currEle.SetValue(Canvas.LeftProperty, xPos);
                currEle.SetValue(Canvas.TopProperty, yPos);

                oldPoint = e.GetPosition(null);
            }
        }

        private void canvasChild1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isMove = true;
            oldPoint = e.GetPosition(null);
        }

        private void canvasChild1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isMove = false;
        }

        /// <summary>
        /// 地图单击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainMap_MouseClick(object sender, Map.MouseEventArgs e)
        {
            //tbPosition.Text = string.Format("{0},{1}", Math.Round(e.MapPoint.X, 6), Math.Round(e.MapPoint.Y, 6));
            //LogHelper.WriteLog(PublicParams.type, string.Format("{0},{1}", Math.Round(e.MapPoint.X, 6), Math.Round(e.MapPoint.Y, 6)));

            ShowInfoWinByClickOrTouch(mainMap.MapToScreen(e.MapPoint));
        }

        private void mainMap_MouseMove(object sender, MouseEventArgs e)
        {
            //tbPosition.Text = string.Format("{0},{1}", Math.Round(e.MapPoint.X, 6), Math.Round(e.MapPoint.Y, 6));//获取当前鼠标单击的位置并显示出来
            //if (mainMap == null)
            //    return;
            
            if (mainMap.IsLoaded)
            {
                try
                {
                    Point screenPoint = e.GetPosition(mainMap);
                    MapPoint mapPoint = mainMap.ScreenToMap(screenPoint);
                    tbPosition.Text = string.Format("{0},{1}", Math.Round(mapPoint.X, 6), Math.Round(mapPoint.Y, 6));
                }
                catch (Exception)
                {
                }
                
            }
        }

        private void btnMainCar_Click(object sender, RoutedEventArgs e)
        {
            Button btnMainCar = sender as Button;
            try
            {
                switch (btnMainCar.Tag.ToString())
                {
                    case"CarVideoOff":
                        btnMainCar.Style = App.Current.Resources["btnOn"] as Style;
                        btnMainCar.Tag = "CarVideoOn";
                        //MessageBox.Show(PublicParams.bigScreenX.ToString() + "  " + PublicParams.bigScreenY.ToString());
                        Camera camera = new Camera();
                        WallVideosHelper.OpenPoliceCarVideoToWall(camera);
                        break;
                    case "CarVideoOn":
                        WallVideosHelper.ClosePoliceCarVideoToWall(PublicParams.bigScreenCamera);
                        btnMainCar.Style = App.Current.Resources["btnOff"] as Style;
                        btnMainCar.Tag = "CarVideoOff";
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

            }
            

        }

        private void mainMap_TouchDown(object sender, TouchEventArgs e)
        {
            ShowInfoWinByClickOrTouch( e.GetTouchPoint(PublicParams.pubMainMap).Position);
        }

        private void ShowInfoWinByClickOrTouch(Point screenPnt)
        {
            GeneralTransform generalTransform = mainMap.TransformToVisual(Application.Current.MainWindow);
            Point transformScreenPnt = generalTransform.Transform(screenPnt);

            foreach (FeatureLayer featureLayer in PublicParams.listFLayer)
            {
                if (featureLayer != null && featureLayer.Visible == true)
                {
                    IEnumerable<Graphic> selectedFLayer = featureLayer.FindGraphicsInHostCoordinates(transformScreenPnt);
                    foreach (Graphic graphic in selectedFLayer)
                    {
                        //mainInfoWindow.Anchor = e.MapPoint;
                        mainInfoWindow.Anchor = graphic.Geometry as MapPoint;
                        mainInfoWindow.IsOpen = true;
                        mainInfoWindow.ContentTemplate = this.FindResource("DT" + graphic.Attributes["Class"].ToString()) as DataTemplate;
                        mainInfoWindow.Content = graphic.Attributes;
                        MapMethods.SendOpenInfoWindow(graphic);
                        return;
                    }
                }
            }


            GraphicsLayer graphicsLayerPoliceCar = MapLayers.GetGraphicsLayerByID(PublicParams.gLayerPoliceCarGPS);
            if (graphicsLayerPoliceCar != null && graphicsLayerPoliceCar.Visible != false)
            {
                IEnumerable<Graphic> selectedGraphics = graphicsLayerPoliceCar.FindGraphicsInHostCoordinates(transformScreenPnt);
                foreach (Graphic graphic in selectedGraphics)
                {
                    mainInfoWindow.Anchor = graphic.Geometry as MapPoint;
                    mainInfoWindow.IsOpen = true;
                    mainInfoWindow.ContentTemplate = this.FindResource("DTPoliceCar") as DataTemplate;
                    mainInfoWindow.Content = graphic.Attributes;
                    MapMethods.SendOpenInfoWindow(graphic);
                    return;
                }
            }

            GraphicsLayer graphicsLayerCase = MapLayers.GetGraphicsLayerByID(PublicParams.gLayerCase);
            if (graphicsLayerCase != null && graphicsLayerCase.Visible == true)
            {
                IEnumerable<Graphic> selectedGraphics = graphicsLayerCase.FindGraphicsInHostCoordinates(transformScreenPnt);
                foreach (Graphic graphic in selectedGraphics)
                {
                    //LPY 2016-4-14 添加 新案件点周围视频点自动查找和播放
                    MapLayers.ClearGLayerByID(PublicParams.gLayerSearchCamerasNearCrime);//清空图层
                    MapMethods.SendClearGraphicsLayerByID(PublicParams.gLayerSearchCamerasNearCrime);

                    ESRI.ArcGIS.Client.Geometry.Geometry geoSearch = MapMethods.GetEllipseGeometry(PublicParams.SearchRadius / (106 * 1000), graphic.Geometry as MapPoint);

                    GeoServHelper gsh = new GeoServHelper();
                    gsh.ExecuteAsyncQueryForCasePoint(geoSearch, PublicParams.urlCamerasLayer);

                    PublicParams.pubCanvasChild1.BeginStoryboard(App.Current.FindResource("StoryboardForPadCamerasOpen") as System.Windows.Media.Animation.Storyboard);
                    MapMethods.SendOpenPadVideos();//打开视频背景板

                    Graphic gSearch = new Graphic() { Symbol = PublicParams.symbolSearchCameras, Geometry = geoSearch };
                    MapLayers.AddGraphicToGLayerByLayerID(gSearch, PublicParams.gLayerSearchCamerasNearCrime);
                    MapMethods.SendGraphicSearchCameras(gSearch);
                }
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
