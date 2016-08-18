using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jovian.ClientMap.classes
{
    using Apache.NMS;
    using Apache.NMS.ActiveMQ;
    using Apache.NMS.ActiveMQ.Commands;
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using ESRI.ArcGIS.Client;
    using ESRI.ArcGIS.Client.Symbols;
    using ESRI.ArcGIS.Client.Geometry;
    public class Cases
    {

        public Cases()
        {
            Task taskInitCase = new Task(InitCases);
            taskInitCase.Start();
            //InitCases();
        }

        private void InitCases()
        {
            try
            {
                IConnectionFactory factoryCases = new ConnectionFactory(PublicParams.strMQUrl);
                IConnection connectionCases = factoryCases.CreateConnection();
                connectionCases.Start();
                ISession sessionCase= connectionCases.CreateSession();
                IMessageConsumer consumerCases = sessionCase.CreateConsumer(new ActiveMQTopic(PublicParams.topicCase));
                consumerCases.Listener += consumerCases_Listener;

            }
            catch (Exception)
            {

            }
        }

        void consumerCases_Listener(IMessage message)
        {
            ITextMessage msg = (ITextMessage)message;
            PublicParams.pubMainMap.Dispatcher.Invoke(new Action(delegate
            {
                JObject json = JObject.Parse(msg.Text);
                if (json["JJDBH"].ToString() == "00000")//演示用的程序会推送一些编号为00000的数据，清空案件图层
                {
                    MapLayers.ClearGLayerByID(PublicParams.gLayerCase);
                    MapMethods.SendClearGraphicsLayerByID(PublicParams.gLayerCase);
                    return;
                }
                    
                DrawCase(json);
            }));
        }

        private void DrawCase(JObject json)
        {
            try
            {
                Geometry geoCase = new MapPoint(Convert.ToDouble(json["ZDDWXZB"].ToString()), Convert.ToDouble(json["ZDDWYZB"].ToString()), new SpatialReference(4326));
                Graphic gCase = new Graphic() { Symbol = PublicParams.symbolCase, Geometry = geoCase };
                gCase.Attributes.Add("BJLBMC", json["BJLXMC"].ToString().Trim());
                gCase.Attributes.Add("BJSJ", Convert.ToDateTime(json["BJSJ"].ToString()).ToString("yyyy-MM-dd HH:mm"));
                MapLayers.AddGraphicToGLayerByLayerID(gCase, PublicParams.gLayerCase);

                ////LPY 2016-4-14 添加 新案件点周围视频点自动查找和播放
                //GraphicsLayer glCase = MapLayers.GetGraphicsLayerByID(PublicParams.gLayerCase);
                //if (glCase == null || glCase.Visible == false)
                //    return;
                //MapLayers.ClearGLayerByID(PublicParams.gLayerSearchCamerasNearCrime);//清空图层
                //MapMethods.SendClearGraphicsLayerByID(PublicParams.gLayerSearchCamerasNearCrime);

                //Geometry geoSearch = MapMethods.GetEllipseGeometry(PublicParams.SearchRadius / (106 * 1000), geoCase as MapPoint);

                //GeoServHelper gsh = new GeoServHelper();
                //gsh.ExecuteAsyncQueryForCasePoint(geoSearch, PublicParams.urlCamerasLayer);

                //PublicParams.pubCanvasChild1.BeginStoryboard(App.Current.FindResource("StoryboardForPadCamerasOpen") as System.Windows.Media.Animation.Storyboard);

                //Graphic gSearch = new Graphic() { Symbol = PublicParams.symbolSearchCameras, Geometry = geoSearch };
                //MapLayers.AddGraphicToGLayerByLayerID(gSearch, PublicParams.gLayerSearchCamerasNearCrime);
                //MapMethods.SendGraphicSearchCameras(gSearch);
            }
            catch (Exception)
            {
            }
        }
    }
}
