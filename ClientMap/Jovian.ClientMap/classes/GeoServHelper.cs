using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jovian.ClientMap.classes
{
    using ESRI.ArcGIS.Client;
    using ESRI.ArcGIS.Client.Tasks;
    using ESRI.ArcGIS.Client.Geometry;
    using ESRI.ArcGIS.Client.Symbols;

    using Jovian.ClientMap.parts;
    using System.Windows;

    /// <summary>
    /// LPY 2015-10-14 添加
    /// GeometryServiceHelper，根据Geometry异步完成Buffer、图层查找等工作
    /// </summary>
    public class GeoServHelper
    {
        public GeometryService geometryTask;
        public QueryTask queryTaskForDrawing;
        public QueryTask queryTaskForCasePoint;

        public GeoServHelper()
        {
            geometryTask = new GeometryService();
            geometryTask.Url = ESRI.ArcGIS.Client.Local.LocalGeometryService.GetService().UrlGeometryService;
            geometryTask.BufferCompleted += geometryTask_BufferCompleted;
            geometryTask.LengthsCompleted += geometryTask_LengthsCompleted;
            geometryTask.AreasAndLengthsCompleted += geometryTask_AreasAndLengthsCompleted;            
            geometryTask.Failed += geometryTask_Failed;

            
        }

        void geometryTask_AreasAndLengthsCompleted(object sender, AreasAndLengthsEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Results.Lengths.Count <= 0 || e.Results.Areas.Count <= 0)
                return;

            string strArea = e.Results.Areas[0].ToString("0.0000" + "平方千米");
            Graphic gAreaDraw = e.UserState as Graphic;
            ShowTextSymbolByGraphic(gAreaDraw, strArea);
        }
                

        void geometryTask_LengthsCompleted(object sender, LengthsEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Results.Count <= 0)
                return;

            Graphic gLenDraw = e.UserState as Graphic;
            string strLen = e.Results[0].ToString("0.00" + "千米");
            ShowTextSymbolByGraphic(gLenDraw, strLen);
        }

        private void ShowTextSymbolByGraphic(Graphic graphic,string result)
        {
            MapLayers.AddGraphicToGLayerByLayerID(graphic, PublicParams.gLayerLengthOrArea);

            double xCenter = (graphic.Geometry.Extent.XMax + graphic.Geometry.Extent.XMin) / 2;
            double yCenter = (graphic.Geometry.Extent.YMax + graphic.Geometry.Extent.YMin) / 2;
            MapPoint mpCenter = new MapPoint(xCenter, yCenter, graphic.Geometry.SpatialReference);
            TextSymbol txtSymbol = PublicParams.textSymbol;

            Graphic gResult = new Graphic() { Geometry = mpCenter, Symbol = txtSymbol };
            gResult.Attributes["Result"] = result; 
            gResult.SetZIndex(2);
            MapLayers.AddGraphicToGLayerByLayerID(gResult, PublicParams.gLayerLengthOrArea);

            //发送到大屏
            MapMethods.SendLengthOrAreaResult(graphic, result, graphic.Geometry.GetType().Name);
        }

        void geometryTask_BufferCompleted(object sender, GraphicsEventArgs e)
        {
            if (e.Results.Count <= 0)
            {
                PublicParams.listCameras.Clear();//清空列表
                return;
            }
                
            Graphic bufferGraphic = new Graphic();
            bufferGraphic.Geometry = e.Results[0].Geometry;
            ExecuteAsyncQueryForDrawing(e.Results[0].Geometry, PublicParams.urlCamerasLayer);//查找视频点
            bufferGraphic.Symbol = App.Current.Resources["DefaultFillSymbol"] as ESRI.ArcGIS.Client.Symbols.Symbol;
            MapLayers.AddGraphicToGLayerByLayerID(bufferGraphic, PublicParams.gLayerDrawing);
            MapMethods.SendGraphic(bufferGraphic);
        }

        void geometryTask_Failed(object sender, TaskFailedEventArgs e)
        {
            LogHelper.WriteLog("执行失败：GeoServHelper");
            //throw new NotImplementedException();
        }

        /// <summary>
        /// LPY 地图上鼠标画框查询监控点位
        /// </summary>
        /// <param name="geoSearch"></param>
        /// <param name="url"></param>
        public void ExecuteAsyncQueryForDrawing(Geometry geoSearch, string url)
        {
            queryTaskForDrawing = new QueryTask(url);
            queryTaskForDrawing.ExecuteCompleted += queryTaskForDrawing_ExecuteCompleted;
            queryTaskForDrawing.Failed += queryTaskForDrawing_Failed;

            Query queryForDrawing = new Query();
            queryForDrawing.ReturnGeometry = true;
            queryForDrawing.OutSpatialReference = new SpatialReference(4326);
            queryForDrawing.Geometry = geoSearch;
            queryForDrawing.OutFields.Add("*");
            queryTaskForDrawing.ExecuteAsync(queryForDrawing);
        }

        void queryTaskForDrawing_Failed(object sender, TaskFailedEventArgs e)
        {
            LogHelper.WriteLog("queryTaskForDrawing：查询失败！");
        }

        void queryTaskForDrawing_ExecuteCompleted(object sender, QueryEventArgs e)
        {
            try
            {
                PublicParams.listCameras.Clear();//清空列表

                FeatureSet featureSet = e.FeatureSet;
                int ID = 0;
                if (featureSet==null||featureSet.Features.Count==0)
                {
                    return;
                }
                
                foreach (Graphic graphic in featureSet.Features)
                {
                    PublicParams.listCameras.Add(new Camera(ID++, graphic.Attributes["CameraName"].ToString(), 0, graphic.Attributes["MAC"].ToString(),Convert.ToDouble(graphic.Attributes["X"]),Convert.ToDouble(graphic.Attributes["Y"]),Convert.ToInt32(graphic.Attributes["VideoID"])));
                }
                //MapMethods.SendOpenPadVideos();//打开视频背景板
            }
            catch (Exception)
            {
            }
            //throw new NotImplementedException();
        }

        /// <summary>
        /// LPY 2016-4-14 添加
        /// 案件点出现时，查询周边监控点位
        /// </summary>
        /// <param name="geoSearch"></param>
        /// <param name="url"></param>
        public void ExecuteAsyncQueryForCasePoint(Geometry geoSearch, string url)
        {
            ExecuteAsyncQueryForDrawing(geoSearch, url);
            //queryTaskForCasePoint = new QueryTask(url);
            //queryTaskForCasePoint.ExecuteCompleted += queryTaskForCasePoint_ExecuteCompleted;
            //queryTaskForCasePoint.Failed += queryTaskForCasePoint_Failed;

            //Query queryForCasePoint = new Query();
            //queryForCasePoint.ReturnGeometry = true;
            //queryForCasePoint.OutSpatialReference = new SpatialReference(4326);
            //queryForCasePoint.Geometry = geoSearch;
            //queryForCasePoint.OutFields.Add("*");
            //queryTaskForCasePoint.ExecuteAsync(queryForCasePoint);
        }

        //void queryTaskForCasePoint_Failed(object sender, TaskFailedEventArgs e)
        //{
        //    LogHelper.WriteLog("queryTaskForCasePoint：查询失败！");
        //}

        //void queryTaskForCasePoint_ExecuteCompleted(object sender, QueryEventArgs e)
        //{
            
        //}

        

    }
}
