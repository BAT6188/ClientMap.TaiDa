using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jovian.ClientMap.classes
{
    using ESRI.ArcGIS.Client;
    using ESRI.ArcGIS.Client.Toolkit.DataSources;
    using ESRI.ArcGIS.Client.Geometry;
    using System.Windows;
    using System.Windows.Media;
    using System.Net;
    using System.Net.Http;
using System.Threading.Tasks;
    //using ESRI.ArcGIS.Client.Symbols;
    /// <summary>
    /// LPY 2015-9-17 添加
    /// 地图图层类，初始化所有图层，并提供图层操作方法
    /// </summary>
    public class MapLayers
    {
        private static string filter = " 1=1 ";
        public static void InitMapLayers()
        {
            AddGraphicsLayerByID(PublicParams.gLayerDrawing, true);
            AddGraphicsLayerByID(PublicParams.gLayerLengthOrArea, true);
            PublicParams.listFLayer.Add(AddFeatureLayer(PublicParams.fLayerCase, PublicParams.urlCaseLayer, null, filter, false));//警情
            PublicParams.listFLayer.Add(AddFeatureLayer(PublicParams.fLayerCameras, PublicParams.urlCamerasLayer, PublicParams.rendererCamera, filter, false));
            PublicParams.listFLayer.Add(AddFeatureLayer(PublicParams.fLayerNetBar, PublicParams.urlNetBarLayer, PublicParams.rendererNetBar, filter, false));
            PublicParams.listFLayer.Add(AddFeatureLayer(PublicParams.fLayerCompany, PublicParams.urlCompanyLayer, PublicParams.rendererCompany, filter, false));
            PublicParams.listFLayer.Add(AddFeatureLayer(PublicParams.fLayerBank, PublicParams.urlBankLayer, PublicParams.rendererBank, filter, false));
            PublicParams.listFLayer.Add(AddFeatureLayer(PublicParams.fLayerGasoline, PublicParams.urlGasolineLayer, PublicParams.rendererGasoline, filter, false));
            PublicParams.listFLayer.Add(AddFeatureLayer(PublicParams.fLayerHospital, PublicParams.urlHospitalLayer, PublicParams.rendererHospital, filter, false));

            AddGraphicsLayerByID(PublicParams.gLayerCase, false);//案件图层
            AddGraphicsLayerByID(PublicParams.gLayerPoliceCarGPS, false);//警车图层
            AddGraphicsLayerByID(PublicParams.gLayerSearchCamerasNearCrime, false);//搜寻监控点图层-突发案件周边

            AddHeatMapLayerByID(PublicParams.hLayerCase, PublicParams.urlCaseLayer, false);//热力图-案件
            AddFeatureLayer(PublicParams.fLayerRoad, PublicParams.urlRoadLayer, null, filter, false);//道路
            AddFeatureLayer(PublicParams.fLayerTrafficLight, PublicParams.urlTrafficLightLayer, null, filter, false);//红绿灯
            AddClusterLayerByID(PublicParams.cLayerCase, PublicParams.urlCaseLayer, PublicParams.rendererCluster, filter, false);//聚合图
        }

        #region GraphicsLayer 静态操作方法
        /// <summary>
        /// LPY 2015-9-17 添加
        /// 根据layerID图层ID把图层添加到主地图上
        /// </summary>
        /// <param name="layerID"></param>
        public static void AddGraphicsLayerByID(string layerID, bool visiable)
        {
            try
            {
                if (PublicParams.pubMainMap.Layers[layerID] != null)
                {
                    LogHelper.WriteLog(layerID + "图层已存在");
                    return;
                }
                GraphicsLayer graphicsLayer = new GraphicsLayer() { 
                    ID=layerID,
                    DisplayName=layerID,
                    Visible=visiable
                };                
                PublicParams.pubMainMap.Layers.Add(graphicsLayer);
            }
            catch (Exception)
            {
                LogHelper.WriteLog( layerID + ":发生了一个错误，在AddGraphicsLayerByID");
            }
        }
        /// <summary>
        /// 根据url向地图添加一个FeatureLayer
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="url"></param>
        /// <param name="renderer"></param>
        /// <param name="filter"></param>
        /// <param name="visiable"></param>
        /// <returns></returns>
        public static FeatureLayer AddFeatureLayer(string layerID, string url, SimpleRenderer renderer, string filter, bool visiable)
        {
            try
            {
                if (!HttpHelper.CheckUrl(url))
                {
                    LogHelper.WriteLog("图层url：" + url + "-无法连接！");
                    return null;
                }
                if (PublicParams.pubMainMap.Layers[layerID] != null)
                {
                    LogHelper.WriteLog(layerID + "图层已存在");
                    return null;
                }
                FeatureLayer featureLayer = new FeatureLayer() { ID = layerID, Url = url, Renderer = renderer, Where = filter, Visible = visiable };
                featureLayer.OutFields.Add("*");
                PublicParams.pubMainMap.Layers.Add(featureLayer);
                return featureLayer;
                //FeatureLayer featureLayer1 = AddFeatureLayerAsync(layerID, url, renderer, filter, visiable).Result;
                
            }
            catch (Exception)
            {
                LogHelper.WriteLog(layerID + ":发生了一个错误，在AddFeatureLayer");
                return null;
            }
        }


        /// <summary>
        /// FeatureLayer显隐
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="visiable"></param>
        public static void ShowHideFeatureLayerByID(string layerID, bool visiable)
        {
            ShowHideLayerByID(layerID, visiable);
        }
        /// <summary>
        /// GraphicsLayer显隐
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="visiable"></param>
        public static void ShowHideGraphicsLayerByID(string layerID, bool visiable)
        {
            ShowHideLayerByID(layerID, visiable);
        }
        /// <summary>
        /// 热力图显隐
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="visiable"></param>
        public static void ShowHideHeatMapLayerByID(string layerID, bool visiable)
        {
            ShowHideLayerByID(layerID, visiable);
        }

        /// <summary>
        /// 聚合图显隐
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="visiable"></param>
        public static void ShowHideClusterLayerByID(string layerID, bool visiable)
        {
            ShowHideLayerByID(layerID, visiable);
        }

        /// <summary>
        /// 根据layerID控制图层显隐
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="visiable"></param>
        private static void ShowHideLayerByID(string layerID, bool visiable)
        {
            try
            {
                if (PublicParams.pubMainMap.Layers[layerID] != null)
                    PublicParams.pubMainMap.Layers[layerID].Visible = visiable;
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 根据layerID返回地图图层中的FeatureLayer
        /// </summary>
        /// <param name="layerID"></param>
        /// <returns></returns>
        public static FeatureLayer GetFeatureLayerByID(string layerID)
        {
            try
            {
                if (PublicParams.pubMainMap.Layers[layerID] != null)
                    return PublicParams.pubMainMap.Layers[layerID] as FeatureLayer;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static GraphicsLayer GetGraphicsLayerByID(string layerID)
        {
            try
            {
                if (PublicParams.pubMainMap.Layers[layerID] != null)
                    return PublicParams.pubMainMap.Layers[layerID] as GraphicsLayer;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        

        /// <summary>
        /// LPY 2015-9-17 添加
        /// 根据图层ID layerID把图层从主地图上删除
        /// </summary>
        /// <param name="layerID"></param>
        public static void DelGraphicsLayerByID(string layerID)
        {
            try
            {
                if (PublicParams.pubMainMap.Layers[layerID] != null)
                    PublicParams.pubMainMap.Layers.Remove(PublicParams.pubMainMap.Layers[layerID]);
            }
            catch (Exception)
            {
                LogHelper.WriteLog( layerID + ":发生了一个错误，在DelGraphicsLayerByID");
            }
        }
        /// <summary>
        /// 向GraphicsLayer添加一个Graphic
        /// </summary>
        /// <param name="graphic"></param>
        /// <param name="layerID"></param>
        public static void AddGraphicToGLayerByLayerID(Graphic graphic, string layerID)
        {
            try
            {
                if (PublicParams.pubMainMap.Layers[layerID] != null)
                    ((GraphicsLayer)PublicParams.pubMainMap.Layers[layerID]).Graphics.Add(graphic);
            }
            catch (Exception)
            {
                LogHelper.WriteLog( layerID + ":发生了一个错误，在AddGraphicToLayerByLayerID");
            }
        }
        /// <summary>
        /// 清空GraphicsLayer
        /// </summary>
        /// <param name="layerID"></param>
        public static void ClearGLayerByID(string layerID)
        {
            try
            {
                if (PublicParams.pubMainMap.Layers[layerID] != null)
                    ((GraphicsLayer)PublicParams.pubMainMap.Layers[layerID]).Graphics.Clear();
            }
            catch (Exception)
            {
                LogHelper.WriteLog( layerID + ":发生了一个错误，在ClearGLayerByID");
            }
        }

        /// <summary>
        /// 从GraphicsLayer中查找一个Graphic
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="layerID"></param>
        /// <returns></returns>
        public static Graphic GetGraphicFromGLayerByID(string key, string value, string layerID)
        {
            try
            {
                Graphic result = null;
                if (PublicParams.pubMainMap.Layers[layerID] != null)
                {
                    foreach (Graphic graphic in (GraphicsLayer)PublicParams.pubMainMap.Layers[layerID])
                    {
                        if (graphic.Attributes[key].ToString() == value)
                            result = graphic;
                    }
                }
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 刷新GraphicsLayer
        /// </summary>
        /// <param name="layerID"></param>
        public static void RefreshGLayerByID(string layerID)
        {
            try
            {
                if (PublicParams.pubMainMap.Layers[layerID] != null)
                    ((GraphicsLayer)PublicParams.pubMainMap.Layers[layerID]).Refresh();
            }
            catch (Exception)
            {
                LogHelper.WriteLog( layerID + ":发生了一个错误，在RefreshGLayerByID");
            }
        }

        #endregion

        /// <summary>
        /// 热力图
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HeatMapLayer AddHeatMapLayerByID(string layerID, string url,bool visiable)
        {
            try
            {
                if (!HttpHelper.CheckUrl(url))
                {
                    LogHelper.WriteLog("图层url：" + url + "-无法连接！");
                    return null;
                }
                HeatMap heatMap = new HeatMap() { ID = layerID, setfilter = " 1=1 ", seturl = url, Opacity = 0.8, Visible = visiable };
                heatMap.setsource();
                PublicParams.pubMainMap.Layers.Add(heatMap);
                heatMap.refreshnow();
                return heatMap;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// LPY 2016-4-2 添加
        /// 聚合图层添加
        /// </summary>
        /// <param name="layerID"></param>
        /// <param name="url"></param>
        /// <param name="renderer"></param>
        /// <param name="filter"></param>
        /// <param name="visiable"></param>
        public static void AddClusterLayerByID(string layerID, string url, SimpleRenderer renderer, string filter, bool visiable)
        {
            try
            {
                if (!HttpHelper.CheckUrl(url))
                {
                    LogHelper.WriteLog("图层url：" + url + "-无法连接！");
                    return;
                }
                if (PublicParams.pubMainMap.Layers[layerID] != null)
                {
                    LogHelper.WriteLog( layerID + "图层已存在");
                    return;
                }

                SumClusterer sumClusterer = new SumClusterer() { Radius = 80 };
                FeatureLayer featureLayer = new FeatureLayer() { ID = layerID, Url = url, Renderer = renderer, Where = filter, Visible = visiable, Clusterer = sumClusterer };
                
                //FlareClusterer fc = new FlareClusterer()//设置聚合条件-----------------------------------LPY 2016-4-13 尝试更改默认样式，暂时注释，勿删
                //{
                //    FlareForeground = new SolidColorBrush(Colors.White),
                //    FlareBackground = new SolidColorBrush(Colors.Black),
                   
                //    //MaximumFlareCount = 10,//都是默认值，不需要改
                //    //Radius = 20,
                //    Gradient = PublicParams.lgbCluster
                //};                
                //featureLayer.Clusterer = fc;


                
                //featureLayer.Clusterer = sumClusterer;

                PublicParams.pubMainMap.Layers.Add(featureLayer);
            }
            catch (Exception)
            {
                return;
            }
        }
    }

}
