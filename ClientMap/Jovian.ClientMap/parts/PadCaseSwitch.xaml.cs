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

using Jovian.ClientMap.classes;

namespace Jovian.ClientMap.parts
{
    /// <summary>
    /// PadCaseSwitch.xaml 的交互逻辑
    /// LPY 2016-3-31 添加
    /// </summary>
    public partial class PadCaseSwitch : UserControl
    {
        public PadCaseSwitch()
        {
            InitializeComponent();
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            switch (btn.Tag.ToString())
            {
                case "CasePointsOff":
                    //MapMethods.SendSwitchGraphicsLayerByID(PublicParams.gLayerCase, "1");
                    MapLayers.ShowHideFeatureLayerByID(PublicParams.fLayerCase, true);
                    MapMethods.SendSwitchFeatureLayerByID(PublicParams.fLayerCase, "1");
                    btn.Style = App.Current.Resources["btnOn"] as Style;
                    btn.Tag = "CasePointsOn";
                    break;
                case "CasePointsOn":
                    //MapMethods.SendSwitchGraphicsLayerByID(PublicParams.gLayerCase, "0");
                    MapLayers.ShowHideFeatureLayerByID(PublicParams.fLayerCase, false);
                    MapMethods.SendSwitchFeatureLayerByID(PublicParams.fLayerCase, "0");
                    btn.Style = App.Current.Resources["btnOff"] as Style;
                    btn.Tag = "CasePointsOff";
                    break;
                case "CaseHeatMapOff"://热力图
                    MapLayers.ShowHideHeatMapLayerByID(PublicParams.hLayerCase, true);
                    MapMethods.SendSwitchHeatMapLayerByID(PublicParams.hLayerCase, "1");
                    btn.Style = App.Current.Resources["btnOn"] as Style;
                    btn.Tag = "CaseHeatMapOn";
                    break;
                case "CaseHeatMapOn":
                    MapLayers.ShowHideHeatMapLayerByID(PublicParams.hLayerCase, false);
                    MapMethods.SendSwitchHeatMapLayerByID(PublicParams.hLayerCase, "0");
                    btn.Style = App.Current.Resources["btnOff"] as Style;
                    btn.Tag = "CaseHeatMapOff";
                    break;
                case "CaseTodayOff"://当日案件
                    MapMethods.SendSwitchWin("警情监控", "1");
                    MapLayers.ShowHideGraphicsLayerByID(PublicParams.gLayerCase, true);
                    MapLayers.ShowHideGraphicsLayerByID(PublicParams.gLayerSearchCamerasNearCrime, true);
                    btn.Style = App.Current.Resources["btnOn"] as Style;
                    btn.Tag = "CaseTodayOn";
                    break;
                case "CaseTodayOn":
                    MapMethods.SendSwitchWin("警情监控", "0");
                    MapLayers.ShowHideGraphicsLayerByID(PublicParams.gLayerCase, false);
                    MapLayers.ShowHideGraphicsLayerByID(PublicParams.gLayerSearchCamerasNearCrime, false);
                    MapLayers.ClearGLayerByID(PublicParams.gLayerSearchCamerasNearCrime);
                    MapMethods.SendClearGraphicsLayerByID(PublicParams.gLayerSearchCamerasNearCrime);
                    btn.Style = App.Current.Resources["btnOff"] as Style;
                    btn.Tag = "CaseTodayOff";
                    break;
                case "CaseClusterOff"://聚合图
                    MapLayers.ShowHideClusterLayerByID(PublicParams.cLayerCase, true);
                    MapMethods.SendSwitchClusterLayerByID(PublicParams.cLayerCase, "1");
                    btn.Style = App.Current.Resources["btnOn"] as Style;
                    btn.Tag = "CaseClusterOn";
                    break;
                case "CaseClusterOn":
                    MapLayers.ShowHideClusterLayerByID(PublicParams.cLayerCase, false);
                    MapMethods.SendSwitchClusterLayerByID(PublicParams.cLayerCase, "0");
                    btn.Style = App.Current.Resources["btnOff"] as Style;
                    btn.Tag = "CaseClusterOff";
                    break;
                default:
                    break;
            }
        }
    }
}
