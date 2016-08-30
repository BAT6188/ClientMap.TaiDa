using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jovian.ClientMap.classes
{
    using System.Configuration;
    using System.Windows.Controls;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net;
    using System.Net.Sockets;

    using ESRI.ArcGIS.Client;
    using ESRI.ArcGIS.Client.Toolkit;
    using ESRI.ArcGIS.Client.Symbols;

    using Jovian.ClientMap.parts;
    using System.Collections.ObjectModel;

    //using Jovian.ClientMap.

    /// <summary>
    /// LPY 2015-9-6 添加
    /// 公共参数类
    /// </summary>
    public class PublicParams
    {
        public static string urlVectorMap = GetAppConfigValueByString("SL");//矢量地图URL
        public static string urlImageMap = GetAppConfigValueByString("YX");//影像地图URL

        public static string urlCamerasLayer = GetAppConfigValueByString("CamerasLayer");//
        public static string urlNetBarLayer = GetAppConfigValueByString("NetBarLayer");//
        public static string urlCompanyLayer = GetAppConfigValueByString("CompanyLayer");//
        public static string urlBankLayer = GetAppConfigValueByString("BankLayer");//
        public static string urlGasolineLayer = GetAppConfigValueByString("GasolineLayer");//
        public static string urlCaseLayer = GetAppConfigValueByString("CaseLayer");
        public static string urlHospitalLayer = GetAppConfigValueByString("HospitalLayer");//医院

        public static string urlTrafficLightLayer = GetAppConfigValueByString("TrafficLightLayer");
        public static string urlRoadLayer = GetAppConfigValueByString("RoadLayer");

        //系统内建GraphicsLayer ID
        public static string gLayerDrawing = "gLayerDrawing";//地图圈选功能图层名称
        public static string gLayerPoliceCarGPS = "gLayerPoliceCarGPS";//警车图层
        public static string gLayerCase = "gLayerCase";//案件图层
        public static string gLayerLengthOrArea = "gLayerLengthOrArea";//测量距离和面积图层
        public static string gLayerSearchCamerasNearCrime = "gLayerSearchCamerasNearCrime";//案件周边查询监控点范围 的图层

        public static string fLayerCameras = "fLayerCameras";//摄像头图层名称
        public static string fLayerBank = "fLayerBank";//银行图层名称
        public static string fLayerCompany = "fLayerCompany";//
        public static string fLayerGasoline = "fLayerGasoline";
        public static string fLayerNetBar = "fLayerNetBar";//网吧
        public static string fLayerCase = "fLayerCase";
        public static string fLayerHospital = "fLayerHospital";//医院

        public static string fLayerTrafficLight = "fLayerTrafficLight";
        public static string fLayerRoad = "fLayerRoad";

        public static string hLayerCase = "hLayerCase";//热力图-案件
        public static string cLayerCase = "cLayerCase";//聚合图

        public static string xmlFilePath = "params.xml";//默认xml配置文件名
        public static string IsInitLayers = GetAppConfigValueByString("IsInitLayers");//标记是否加载图层

        public static SimpleRenderer rendererCamera = new SimpleRenderer() { Symbol = (Symbol)App.Current.Resources["pmsForCamerasLayer"] };//
        public static SimpleRenderer rendererNetBar = new SimpleRenderer() { Symbol = (Symbol)App.Current.Resources["pmsForNetBarLayer"] };//
        public static SimpleRenderer rendererCompany = new SimpleRenderer() { Symbol = (Symbol)App.Current.Resources["pmsForCompanyLayer"] };//
        public static SimpleRenderer rendererBank = new SimpleRenderer() { Symbol = (Symbol)App.Current.Resources["pmsForBankLayer"] };//
        public static SimpleRenderer rendererGasoline = new SimpleRenderer() { Symbol = (Symbol)App.Current.Resources["pmsForGasolineLayer"] };//
        public static SimpleRenderer rendererHospital = new SimpleRenderer() { Symbol = (Symbol)App.Current.Resources["pmsForHospitalLayer"] };//医院

        public static Symbol symbolCase = App.Current.Resources["pmsForCase"] as Symbol;//案件点位符号
        public static Symbol symbolSearchCameras = App.Current.Resources["SearchCameras"] as Symbol;

        public static SimpleRenderer rendererCluster = new SimpleRenderer() { Symbol = (Symbol)App.Current.Resources["smsCluster"] };//聚合图默认符号
        public static System.Windows.Media.LinearGradientBrush lgbCluster = App.Current.Resources["lgbCluster"] as System.Windows.Media.LinearGradientBrush;

        public static Symbol symbolGreenLight = App.Current.Resources["pmsGreenLight"] as Symbol ;//绿灯
        public static Symbol symbolYellowLight = App.Current.Resources["pmsYellowLight"] as Symbol;//黄灯
        public static Symbol symbolRedLight = App.Current.Resources["pmsRedLight"] as Symbol;//红灯

        public static LineSymbol roadFreeSymbol = App.Current.Resources["RoadFreeSymbol"] as LineSymbol;//空闲
        public static LineSymbol roadNormalSymbol = App.Current.Resources["RoadNormalSymbol"] as LineSymbol;//正常
        public static LineSymbol roadBusySymbol = App.Current.Resources["RoadBusySymbol"] as LineSymbol;//忙碌


        public static bool CanSend = false;//标记与大屏是否联通

        public static string strMQUrl = GetAppConfigValueByString("MQ");
        public static string topicCase = GetAppConfigValueByString("TopicCase");                            //案件推送
        public static string topicGPS = GetAppConfigValueByString("TopicGPS");                              //警车、警员GSP信息推送
        //public static string topicReservoir = GetAppConfigValueByString("TopicReservoir");                  //水库信息
        //public static string topicPoliceCarPosition = GetAppConfigValueByString("TopicPoliceCarPosition");  //警车所在社区
        public static string topicTraffic = GetAppConfigValueByString("TopicTraffic");                      //交通
        public static string topicLight = GetAppConfigValueByString("TopicLight");

        public static string strDBPath = GetAppConfigValueByString("DBPath");
        public static string strOrigin = GetAppConfigValueByString("Origin");

        public static string targetIP = GetAppConfigValueByString("TargetIP");
        public static int targetPort = Convert.ToInt32(GetAppConfigValueByString("TargetPort"));//
        public static string DVCSIP = GetAppConfigValueByString("DVCSIP");
        public static int DVCSPort = Convert.ToInt32(GetAppConfigValueByString("DVCSPort"));
        public static string DVCSIP2 = GetAppConfigValueByString("DVCSIP2");
        public static int DVCSPort2 = Convert.ToInt32(GetAppConfigValueByString("DVCSPort2"));
        public static Type type = typeof(MainWindow);//
        public static string splitChar = GetAppConfigValueByString("SplitChar");//分隔符
        public static DVCSServer dvcsServerMain = new DVCSServer();
        public static DVCSServer dvcsServer2 = new DVCSServer();//add by Perry 2016-8-26 第二个DVCS服务器
        public static string dvcsServerMainName = "dvcsServerMain";
        public static string dvcsServer2Name = "dvcsServer2";

        public static Map pubMainMap = null;
        public static Grid pubLayoutRoot;
        public static InfoWindow pubInfoWin;
        public static Canvas pubCanvasChild1;
        public static List<FeatureLayer> listFLayer = new List<FeatureLayer>();//保存当前地图中所有可交互的FeatureLayer图层

        public static int Delta = Convert.ToInt32(GetAppConfigValueByString("Delta"));//大屏地图与客户端地图显示层级的差
        public static int BufferRadius = Convert.ToInt32(GetAppConfigValueByString("BufferRadius"));//buffer半径
        public static double SearchRadius = Convert.ToDouble(GetAppConfigValueByString("SearchRadius"));//搜寻半径

        public static string MAC1 = GetAppConfigValueByString("MAC1");
        public static string MAC2 = GetAppConfigValueByString("MAC2");
        public static string MAC3 = GetAppConfigValueByString("MAC3");
        public static string MAC4 = GetAppConfigValueByString("MAC4");

        public static int channel1 = Convert.ToInt32(GetAppConfigValueByString("Channel1"));
        public static int channel2 = Convert.ToInt32(GetAppConfigValueByString("Channel2"));
        public static int channel3 = Convert.ToInt32(GetAppConfigValueByString("Channel3"));
        public static int channel4 = Convert.ToInt32(GetAppConfigValueByString("Channel4"));
        
        //警车车载视频演示专用
        public static string MACPoliceCarVideo1 = GetAppConfigValueByString("MACPoliceCarVideo1");
        public static int channelPoliceCarVideo1 = Convert.ToInt32(GetAppConfigValueByString("ChannelPoliceCarVideo1"));

        //下面这三个参数用来存储当前地图中心坐标点X、Y、Level
        public static double pubX;
        public static double pubY;
        public static int pubLevel;

        //以下参数与大屏开窗有关
        public static int x1 = Convert.ToInt32(GetAppConfigValueByString("x1"));
        public static int x2 = Convert.ToInt32(GetAppConfigValueByString("x2"));
        public static int y1 = Convert.ToInt32(GetAppConfigValueByString("y1"));
        public static int y2 = Convert.ToInt32(GetAppConfigValueByString("y2"));
        public static int y3 = Convert.ToInt32(GetAppConfigValueByString("y3"));
        public static int w = Convert.ToInt32(GetAppConfigValueByString("w"));
        public static int h = Convert.ToInt32(GetAppConfigValueByString("h"));
        public static int zindex = Convert.ToInt32(GetAppConfigValueByString("zindex"));
        public static int titleHeight = Convert.ToInt32(GetAppConfigValueByString("titleHeight"));//标题高度自定义
        public static int winHeight = 140;//背景板标题高度（动态视频 四个字所在位置的高度）
        public static string dynamicVideoWinName = "动态视频";
        public static int AdjustmentNum = Convert.ToInt32(GetAppConfigValueByString("AdjustmentNum"));//微调整数

        //第二DVCS服务器开窗相关参数
        public static int dvcs2wallID = Convert.ToInt32(XmlHelper.GetValueByXPath(xmlFilePath, "/Root/DVCS2WallID"));//2;//
        public static int dvcs2w = Convert.ToInt32(XmlHelper.GetValueByXPath(xmlFilePath, "/Root/DVCS2Width"));//1920;//
        public static int dvcs2h = Convert.ToInt32(XmlHelper.GetValueByXPath(xmlFilePath, "/Root/DVCS2Height"));//1080;//
        public static int dvcs2x1 = Convert.ToInt32(XmlHelper.GetValueByXPath(xmlFilePath, "/Root/DVCS2X"));//0;//
        public static int dvcs2y1 = Convert.ToInt32(XmlHelper.GetValueByXPath(xmlFilePath, "/Root/DVCS2Y"));//0;//
        public static int dvcs2zindex = 65535;

        //视频相关
        public static ObservableCollection<Camera> listCameras = new ObservableCollection<Camera>();//查询到的摄像头列表
        public static Camera[] arrayOpenedVideos = new Camera[4];
        public static int currentVideoFlag = -1;//保存当前操作的视频在 arrayOpenedVideos中的位置，0-3，-1表示初始化
        public static Camera currentCamera = null;
        public static int wallID = Convert.ToInt32(GetAppConfigValueByString("WallID"));//屏幕墙ID

        public static int bigScreenX = 0;
        public static int bigScreenY = 0;
        public static Camera bigScreenCamera = null;
        public static int isPoliceCarVideoSend = 0;
        public static string MainPoliceCarName = "MainPoliceCar";

        //public static Camera dvcs2OpenedCamera = null;//第二DVCS服务器已打开的视频
        public static bool IsSendToDVCSServer2 = true;// Convert.ToBoolean(XmlHelper.GetValueByXPath(xmlFilePath, "/Root/SendCmdToDVCSServer2")); //标记是否向第二DVCS服务器发送指令
        public static Camera[] arrayOpenedVideosDVCS2 = new Camera[1];
        
        public static Thread threadForMQ = null;//监听MQ线程，程序关闭时，要判断该线程是否为null，不为null时要释放掉

        public static PadCameras padCameras;
        public static PadLayers padLayers;
        public static PadOpenedVideos padOpenedVideos;
        //public static PadDVCS padDVCS;
        public static PadRemoteParams padRemoteParams;
        public static PadRectParams padRectParams;
        public static PadCaseSwitch padCaseSwitch;        

        //两个Symbol模板
        public static FillSymbol fillSymbol = App.Current.Resources["DefaultFillSymbol"] as FillSymbol;
        public static LineSymbol lineSymbol = App.Current.Resources["DefaultLineSymbol"] as LineSymbol;
        //测长度、面积相关符号
        public static FillSymbol areaSymbol = App.Current.Resources["AreaSymbol"] as FillSymbol;
        public static LineSymbol lengthSymbol = App.Current.Resources["LengthSymbol"] as LineSymbol;
        public static TextSymbol textSymbol = App.Current.Resources["TxtSymbol"] as TextSymbol;



        public static bool IsLogWrite = GetAppConfigValueByString("IsLogWrite") == "1" ? true : false;//日志是否输出
        
        /// <summary>
        /// LPY 2015-9-9 添加
        /// 根据Key值，从App.config文件中获取配置项的value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetAppConfigValueByString(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}
