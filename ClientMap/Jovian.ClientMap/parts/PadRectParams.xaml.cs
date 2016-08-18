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
    /// PadRectParams.xaml 的交互逻辑
    /// </summary>
    public partial class PadRectParams : UserControl
    {
        public string strOldName = "";
        private ContentControl ccRect;
        public PadRectParams()
        {
            //InitializeComponent();
        }

        public PadRectParams(string name, double width, double height, double x, double y, int fontsize,ref ContentControl cc)
        {
            InitializeComponent();
            strOldName = name;
            tbModularName.Text = name;
            tbWidth.Text = width.ToString();
            tbHeight.Text = height.ToString();
            tbX.Text = x.ToString();
            tbY.Text = y.ToString();
            tbFontSize.Text = fontsize.ToString();
            ccRect = cc;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            PublicParams.pubLayoutRoot.Children.Remove(this);
            PublicParams.padRectParams = null;
        }

        //确定按钮
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            double width = Convert.ToDouble(tbWidth.Text.Trim());
            double height = Convert.ToDouble(tbHeight.Text.Trim());
            double x = Convert.ToDouble(tbX.Text.Trim());
            double y = Convert.ToDouble(tbY.Text.Trim());
            int fontsize = Convert.ToInt32(tbFontSize.Text.Trim());
            string modularName = tbModularName.Text.Trim();

            ccRect.Width = width; ccRect.Height = height; Canvas.SetLeft(ccRect, x); Canvas.SetTop(ccRect, y); ccRect.FontSize = fontsize;
            Rectangle rtTemp = (Rectangle)ccRect.Content; rtTemp.Width = width; rtTemp.Height = height;
            ((TextBlock)((StackPanel)((VisualBrush)rtTemp.Fill).Visual).Children[0]).Text = modularName;
            ccRect.Name = modularName;

            //MapMethods.SendChangePadParamsAndName(strOldName, modularName, width, height, x, y, Canvas.GetZIndex(ccRect), fontsize);//参数发送到大屏
            MapMethods.SendChangePadParams(width, height, x, y, Canvas.GetZIndex(ccRect),modularName, fontsize);

            //RemoteWin rw = new RemoteWin(width, height, x, y, Canvas.GetZIndex(ccRect), modularName, fontsize, "");
            //XmlHelper.UpdateModularByName(PublicParams.xmlFilePath, "/Root/Windows", strOldName, rw);

            btnClose_Click(sender, e);            
        }
    }
}
