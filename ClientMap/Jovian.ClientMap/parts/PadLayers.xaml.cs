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

namespace Jovian.ClientMap.parts
{
    using Jovian.ClientMap.classes;
    /// <summary>
    /// LPY 2015-10-14 添加
    /// PadLayers.xaml 的交互逻辑
    /// </summary>
    public partial class PadLayers : UserControl
    {
        
        public PadLayers()
        {
            InitializeComponent();

            List<LayersList> listLayerList = new List<LayersList>() { 
                new LayersList("摄像头",PublicParams.fLayerCameras,"",null),
                new LayersList("银行",PublicParams.fLayerBank,"",null),
                new LayersList("网吧",PublicParams.fLayerNetBar,"",null),
                new LayersList("企业",PublicParams.fLayerCompany,"",null),
                new LayersList("加油站",PublicParams.fLayerGasoline,"",null),
                new LayersList("医院",PublicParams.fLayerHospital,"",null)
            };

            lstLayers.ItemsSource = listLayerList;
        }

        private void btnSwitch_Click(object sender, RoutedEventArgs e)
        {
            Button btnSwitch = (Button)sender;
            switch (btnSwitch.Name)
            {
                case "On":
                    btnSwitch.Style = App.Current.Resources["btnOff"] as Style;
                    MapLayers.ShowHideFeatureLayerByID(btnSwitch.Tag.ToString(), false);
                    btnSwitch.Name = "Off";
                    MapMethods.SendSwitchFeatureLayerByID(btnSwitch.Tag.ToString(), "0");
                    //MapMethods.CloseInfoWin();
                    //MapMethods.SendCloseInfoWindow();
                    break;
                case "Off":
                    btnSwitch.Style = App.Current.Resources["btnOn"] as Style;                    
                    MapLayers.ShowHideFeatureLayerByID(btnSwitch.Tag.ToString(), true);
                    btnSwitch.Name = "On";
                    MapMethods.SendSwitchFeatureLayerByID(btnSwitch.Tag.ToString(), "1");
                    break;
                default:
                    break;
            }
        }
    }

    public class LayersList : INotifyPropertyChanged
    {
        private string name;
        private string id;
        private string url;
        private object obj;

        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged(new PropertyChangedEventArgs("Name")); }
        }

        public string ID {
            get { return id; }
            set { id = value; OnPropertyChanged(new PropertyChangedEventArgs("ID")); }
        }

        public string Url {
            get { return url; }
            set { url = value; OnPropertyChanged(new PropertyChangedEventArgs("Url")); }
        }

        public object Obj
        {
            get { return obj; }
            set { obj = value; OnPropertyChanged(new PropertyChangedEventArgs("Obj")); }
        }

        public LayersList(string _name, string _id, string _url, object _obj)
        {
            Name = _name; ID = _id; Url = _url; Obj = _obj;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
    }
}
