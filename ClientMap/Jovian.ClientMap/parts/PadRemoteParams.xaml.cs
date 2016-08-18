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



namespace Jovian.ClientMap.parts
{
    using Jovian.ClientMap.classes;
    using System.Collections;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Effects;
    using System.Windows.Threading;
    /// <summary>
    /// PadRemoteParams.xaml 的交互逻辑
    /// </summary>
    public partial class PadRemoteParams : UserControl
    {
        private DrawingBrush drawingBrushCanvas;

        private static int screenRaito = Convert.ToInt32(XmlHelper.GetValueByXPath(PublicParams.xmlFilePath, "/Root/ScreenRatio"));
        private double ScreenResolutionWidth = Convert.ToDouble(XmlHelper.GetValueByXPath(PublicParams.xmlFilePath, "/Root/ScreenResolutionWidth"));//单屏分辨率宽
        private double ScreenResolutionHeight = Convert.ToDouble(XmlHelper.GetValueByXPath(PublicParams.xmlFilePath, "/Root/ScreenResolutionHeight"));//分辨率高
        private int ScreenMatrixRows = Convert.ToInt32(XmlHelper.GetValueByXPath(PublicParams.xmlFilePath, "/Root/ScreenMatrixRows"));//屏幕矩阵行数
        private int ScreenMatrixColumns = Convert.ToInt32(XmlHelper.GetValueByXPath(PublicParams.xmlFilePath, "/Root/ScreenMatrixColumns"));//列数
        public PadRemoteParams()
        {
            InitializeComponent();
            //CreateRect("警情窗口", 192, 108, 100, 0, 1, "", 12);
            LoadParams();
            LoadModulars();
            //XmlHelper.GetValueByXPath("params.xml", "/Root/ScreenWidth");
            //ReloadCanvas();
        }

        private void LoadModulars()//加载模块框
        {
            RemoteWin[] Modulars = XmlHelper.GetModularsByXPath(PublicParams.xmlFilePath, "/Root/Windows");
            foreach (RemoteWin rw in Modulars)
            {
                CreateRect(rw.Name, rw.Width, rw.Height, rw.X, rw.Y, rw.Zindex, rw.Img, rw.FontSize);
                if (rw.Name == PublicParams.dynamicVideoWinName)//如果加载的是动态视频的窗口，要计算视频上墙的位置
                {
                    RecalculateVideosPosition((int)rw.Width, (int)rw.Height, (int)rw.X, (int)rw.Y);
                }
            }
        }
        private void LoadParams()//加载参数
        {
            try
            {
                cbScreenMatrix.Items.Clear();
                foreach (string screenMatrix in XmlHelper.GetScreenInfo(PublicParams.xmlFilePath, "/Root/ScreenMatrix"))
                {
                    ComboBoxItem cbi = new ComboBoxItem() { Content=screenMatrix };
                    cbScreenMatrix.Items.Add(cbi);
                }
                cbScreenResolution.Items.Clear();
                foreach (string screenResolution in XmlHelper.GetScreenInfo(PublicParams.xmlFilePath, "/Root/ScreenResolution"))
                {
                    ComboBoxItem cbi = new ComboBoxItem() { Content = screenResolution };
                    cbScreenResolution.Items.Add(cbi);
                }
            }
            catch (Exception)
            {
            }
            try
            {
                cbScreenMatrix.Text = ScreenMatrixRows.ToString() + "×" + ScreenMatrixColumns.ToString();
                cbScreenResolution.Text = ScreenResolutionWidth.ToString() + "×" + ScreenResolutionHeight.ToString();
            }
            catch (Exception)
            {
                LogHelper.WriteLog("加载参数错误，在LoadParams中");
            }
        }

        private void ReloadCanvas()
        {
            canvasParams.Width = ScreenMatrixRows * ScreenResolutionWidth / screenRaito;
            canvasParams.Height = ScreenMatrixColumns * ScreenResolutionHeight / screenRaito;

            drawingBrushCanvas = new DrawingBrush(new GeometryDrawing(
                new ImageBrush(new BitmapImage(new Uri("images\\logodelta.png", UriKind.Relative))),//new SolidColorBrush(Colors.White),
                        new Pen(new SolidColorBrush(Colors.Black), 1.0),
                            new RectangleGeometry(new Rect(0, 0, ScreenResolutionWidth / screenRaito, ScreenResolutionHeight / screenRaito))));

            drawingBrushCanvas.Stretch = Stretch.None;
            drawingBrushCanvas.TileMode = TileMode.Tile;
            drawingBrushCanvas.Viewport = new Rect(0, 0, ScreenResolutionWidth / screenRaito, ScreenResolutionHeight / screenRaito);
            drawingBrushCanvas.ViewportUnits = BrushMappingMode.Absolute;
            canvasParams.Background = drawingBrushCanvas;
        }
        //发送按钮
        private void tbSend_Click(object sender, RoutedEventArgs e)
        {
            //MapMethods.SendChangePadParams(Convert.ToDouble(tbWidth.Text), Convert.ToDouble(tbHeight.Text), Convert.ToDouble(tbFontSize.Text));
        }

        //
        private void btnRightSide_Click(object sender, RoutedEventArgs e)
        {
            switch (btnRightSide.Tag.ToString())
            {
                case "Close":
                    btnRightSide.Tag = "Open";
                    btnRightSide.Style = this.Resources["btnRightSideOpen"] as Style;
                    PublicParams.padRemoteParams.BeginStoryboard(App.Current.FindResource("StoryboardForPadRemoteParamsOpen") as Storyboard);
                    break;
                case "Open":
                    btnRightSide.Tag = "Close";
                    btnRightSide.Style = this.Resources["btnRightSideClose"] as Style;
                    PublicParams.padRemoteParams.BeginStoryboard(App.Current.FindResource("StoryboardForPadRemoteParamsClose") as Storyboard);
                    break;
                default:
                    break;
            }
        }

        private void canvasParams_Loaded(object sender, RoutedEventArgs e)
        {
            ReloadCanvas();
        }

        private void CreateRect(string name, double width, double height, double x, double y, int zindex, string img, int fontsize)
        {
            ContentControl cc1 = new ContentControl() { Width = width, Height = height, MinHeight = 50, MinWidth = 50,Opacity=1 };
            cc1.SetResourceReference(ContentControl.TemplateProperty, "DesignerItemTemplate");
            CreateContextMenu(cc1);

            Canvas.SetLeft(cc1, x); Canvas.SetTop(cc1, y); cc1.IsHitTestVisible = true; cc1.FontSize = fontsize;
            Rectangle rt1 = new Rectangle() { Width=width,Height=height};
            VisualBrush vb1 = new VisualBrush() { TileMode=TileMode.Tile};
            StackPanel sp1 = new StackPanel() { Height=height,Width=width};
            sp1.Background = new ImageBrush(new BitmapImage(new Uri(img == "" ? "images\\bgParams3.png" : img, UriKind.Relative))) { Opacity=0.75};
            
            Thickness tn1 = new Thickness(5, 5, 0, 0);
            TextBlock tb1 = new TextBlock() {Text=name, Height = 30, FontSize = 16, Foreground = new SolidColorBrush(Colors.Red), Margin = tn1, Effect = new DropShadowEffect() {Direction=0,ShadowDepth=0,Color=Colors.Black } };
            
            sp1.Children.Add(tb1);
            vb1.Visual = sp1;
            rt1.Fill = vb1;
            cc1.Content = rt1;
            Canvas.SetZIndex(cc1, zindex);
            rt1.IsHitTestVisible = false;
            canvasParams.Children.Add(cc1);
            cc1.Name = name;
            canvasParams.RegisterName(name, cc1);
        }

        //保存配置
        private void btnSetup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int count = canvasParams.Children.Count;
                ContentControl ccTemp=null;
                RemoteWin rwTemp = null;
                ArrayList alRemoteWin = new ArrayList();
                for (int i = 0; i < count; i++)
                {
                    ccTemp = canvasParams.Children[i] as ContentControl;
                    rwTemp = new RemoteWin(ccTemp.Width, ccTemp.Height, Canvas.GetLeft(ccTemp), Canvas.GetTop(ccTemp), Canvas.GetZIndex(ccTemp), ccTemp.Name, Convert.ToInt32(ccTemp.FontSize), GetImgPathFromRect(ccTemp));
                    //XmlHelper.UpdateModularByName(PublicParams.xmlFilePath, "/Root/Windows", rwTemp);
                    alRemoteWin.Add(rwTemp);
                }
                RemoteWin[] rws=(RemoteWin[])alRemoteWin.ToArray(typeof(RemoteWin));
                XmlHelper.UpdateModularsByName(PublicParams.xmlFilePath, "/Root/Windows",rws );

                //发送模块排列信息组到大屏
                MapMethods.SendModularsToServer(rws);

                MessageBox.Show("已保存！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message);
                return;
            }
            
            //MessageBox.Show(xx.ToString());
        }

        private string GetImgPathFromRect(ContentControl cc)
        {
            try
            {
                Rectangle rt = (Rectangle)cc.Content;
                VisualBrush vb = (VisualBrush)rt.Fill;
                StackPanel sp = (StackPanel)vb.Visual;
                ImageBrush ib = (ImageBrush)sp.Background;
                return ib.ImageSource.ToString();
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message);
                return "";
            }
        }

        private void cbScreenMatrix_DropDownClosed(object sender, EventArgs e)
        {
            string strMatrix = ((ComboBoxItem)cbScreenMatrix.SelectedItem).Content.ToString();
            ScreenMatrixRows = Convert.ToInt32(strMatrix.Split('×')[0]);
            ScreenMatrixColumns = Convert.ToInt32(strMatrix.Split('×')[1]);
            ReloadCanvas();
            XmlHelper.UpdateValueByXPath(PublicParams.xmlFilePath, "/Root/ScreenMatrixRows", ScreenMatrixRows.ToString());
            XmlHelper.UpdateValueByXPath(PublicParams.xmlFilePath, "/Root/ScreenMatrixColumns", ScreenMatrixColumns.ToString());
        }

        private void cbScreenResolution_DropDownClosed(object sender, EventArgs e)
        {
            string strRatio = ((ComboBoxItem)cbScreenResolution.SelectedItem).Content.ToString();
            ScreenResolutionWidth = Convert.ToInt32(strRatio.Split('×')[0]);
            ScreenResolutionHeight = Convert.ToInt32(strRatio.Split('×')[1]);
            ReloadCanvas();
            XmlHelper.UpdateValueByXPath(PublicParams.xmlFilePath, "/Root/ScreenResolutionWidth", ScreenResolutionWidth.ToString());
            XmlHelper.UpdateValueByXPath(PublicParams.xmlFilePath, "/Root/ScreenResolutionHeight", ScreenResolutionHeight.ToString());
        }

        private void CreateContextMenu(ContentControl ccRect)
        {
            ContextMenu cmRightButton = new ContextMenu();

            MenuItem miDetial = new MenuItem() { Header="详细参数"};
            miDetial.Click += miDetial_Click;
            MenuItem miZindexUp = new MenuItem() { Header = "置于顶层" };
            MenuItem miZindexUpTop = new MenuItem() { Header = "置于顶层", Tag = "miZindexUpTop" }; miZindexUpTop.Click += miZindex_Click;
            MenuItem miZindexUpOne = new MenuItem() { Header = "上移一层", Tag = "miZindexUpOne" }; miZindexUpOne.Click += miZindex_Click;

            MenuItem miZindexDown = new MenuItem() { Header="置于底层"};
            MenuItem miZindexDownBottom = new MenuItem() { Header = "置于底层", Tag = "miZindexDownBottom" }; miZindexDownBottom.Click += miZindex_Click;
            MenuItem miZindexDownOne = new MenuItem() { Header = "下移一层", Tag = "miZindexDownOne" }; miZindexDownOne.Click += miZindex_Click;
                        
            cmRightButton.Items.Add(miDetial); cmRightButton.Items.Add(new Separator());
            miZindexUp.Items.Add(miZindexUpTop); miZindexUp.Items.Add(miZindexUpOne);
            miZindexDown.Items.Add(miZindexDownBottom); miZindexDown.Items.Add(miZindexDownOne);
            cmRightButton.Items.Add(miZindexUp); cmRightButton.Items.Add(miZindexDown);
            ccRect.ContextMenu = cmRightButton;

        }

        private void miZindex_Click(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            try
            {
                int order = 0;
                MenuItem mi = sender as MenuItem;
                switch (mi.Tag.ToString())
                {
                    case "miZindexUpTop": //置于顶层
                        order = 1;
                        break;
                    case "miZindexUpOne"://上移一层
                        order = 2;
                        break;
                    case "miZindexDownBottom"://置于底层
                        order = 3;
                        break;
                    case "miZindexDownOne"://下移一层
                        order = 4;
                        break;
                    default:
                        break;
                }
                ContentControl ccModular = (ContentControl)ContextMenuService.GetPlacementTarget(LogicalTreeHelper.GetParent(LogicalTreeHelper.GetParent(mi)));
                SetZindex(ccModular, order);
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(ex.Message + "--miZindex_Click");
            }
        }

        /// <summary>
        /// 设置模块的层叠顺序
        /// </summary>
        /// <param name="cc">要设置的模块</param>
        /// <param name="order">顺序：1，置于顶层 2，上移一层 3，置于底层 4，下移一层</param>
        private void SetZindex(ContentControl cc, int order)
        {
            if (order == 0)
                return;
            string currentName = string.Empty;
            ArrayList alModulars = new ArrayList();
            ContentControl ccCurrent = null; int iCurrentZindex = 0;
            ContentControl ccTemp = null;
            for (int i = 0; i < canvasParams.Children.Count; i++)
                alModulars.Add((ContentControl)canvasParams.Children[i]);

            alModulars = MakeSort(alModulars);//按Zindex排序

            switch (order)
            {
                case 1://1，置于顶层
                    for (int i = 0; i < alModulars.Count-1; i++)
                    {
                        if (((ContentControl)alModulars[i]).Name == cc.Name)
                        {
                            ccCurrent = (ContentControl)alModulars[i];
                            iCurrentZindex = Canvas.GetZIndex((ContentControl)alModulars[alModulars.Count - 1]) + 1;
                            Canvas.SetZIndex(ccCurrent, iCurrentZindex);
                            RemoteWin rw = new RemoteWin(ccCurrent.Width, ccCurrent.Height, Canvas.GetLeft(ccCurrent), Canvas.GetTop(ccCurrent), iCurrentZindex, ccCurrent.Name, Convert.ToInt32(ccCurrent.FontSize), "");
                            rw.SendParamsToRemote();
                        }                            
                    }
                    break;
                case 2://2，上移一层
                    for (int i = 0; i < alModulars.Count-1; i++)
                    {
                        if (((ContentControl)alModulars[i]).Name == cc.Name)
                        {
                            ccCurrent = (ContentControl)alModulars[i];
                            iCurrentZindex = Canvas.GetZIndex((ContentControl)alModulars[i + 1]);

                            ccTemp = (ContentControl)alModulars[i + 1];
                            int tempzindex = Canvas.GetZIndex((ContentControl)alModulars[i]);

                            Canvas.SetZIndex(ccCurrent, iCurrentZindex);
                            Canvas.SetZIndex(ccTemp, tempzindex);

                            RemoteWin rw = new RemoteWin(ccCurrent.Width, ccCurrent.Height, Canvas.GetLeft(ccCurrent), Canvas.GetTop(ccCurrent), iCurrentZindex, ccCurrent.Name, Convert.ToInt32(ccCurrent.FontSize), "");
                            rw.SendParamsToRemote();
                            rw = new RemoteWin(ccTemp.Width, ccTemp.Height, Canvas.GetLeft(ccTemp), Canvas.GetTop(ccTemp), tempzindex, ccTemp.Name, Convert.ToInt32(ccTemp.FontSize), "");
                            rw.SendParamsToRemote();
                        }
                    }
                    break;
                case 3://3，置于底层
                    for (int i = 1; i < alModulars.Count; i++)
                    {
                        if (((ContentControl)alModulars[i]).Name == cc.Name)
                        {
                            ccCurrent = (ContentControl)alModulars[i];
                            iCurrentZindex = Canvas.GetZIndex((ContentControl)alModulars[0]) - 1;
                            Canvas.SetZIndex(ccCurrent, iCurrentZindex);
                            RemoteWin rw = new RemoteWin(ccCurrent.Width, ccCurrent.Height, Canvas.GetLeft(ccCurrent), Canvas.GetTop(ccCurrent), iCurrentZindex, ccCurrent.Name, Convert.ToInt32(ccCurrent.FontSize), "");
                            rw.SendParamsToRemote();
                        }
                            
                    }
                    break;
                case 4://4，下移一层
                    for (int i = 1; i < alModulars.Count; i++)
                    {
                        if (((ContentControl)alModulars[i]).Name == cc.Name)
                        {
                            ccCurrent = (ContentControl)alModulars[i];
                            iCurrentZindex = Canvas.GetZIndex((ContentControl)alModulars[i - 1]);

                            ccTemp = (ContentControl)alModulars[i - 1];
                            int tempzindex = Canvas.GetZIndex((ContentControl)alModulars[i]);

                            Canvas.SetZIndex(ccCurrent, iCurrentZindex);
                            Canvas.SetZIndex(ccTemp, tempzindex);

                            RemoteWin rw = new RemoteWin(ccCurrent.Width, ccCurrent.Height, Canvas.GetLeft(ccCurrent), Canvas.GetTop(ccCurrent), iCurrentZindex, ccCurrent.Name, Convert.ToInt32(ccCurrent.FontSize), "");
                            rw.SendParamsToRemote();
                            rw = new RemoteWin(ccTemp.Width, ccTemp.Height, Canvas.GetLeft(ccTemp), Canvas.GetTop(ccTemp), tempzindex, ccTemp.Name, Convert.ToInt32(ccTemp.FontSize), "");
                            rw.SendParamsToRemote();
                        }
                    }
                    break;
                default:
                    break;
            }

        }

        //排序
        private ArrayList MakeSort(ArrayList al)
        {
            object temp = null; //临时变量
            for (int i = 0; i < al.Count - 1; i++)
            {
                for (int j = 0; j < al.Count - 1 - i; j++)
                {
                    if (Canvas.GetZIndex((ContentControl)al[j]) > Canvas.GetZIndex((ContentControl)al[j + 1]))
                    {
                        temp = al[j];
                        al[j] = al[j + 1];
                        al[j + 1] = temp;
                    }
                }
            }
            return al;
        }
        private void miDetial_Click(object sender, RoutedEventArgs e)
        {
            //PublicParams.padRectParams.Visibility = Visibility.Visible;
            if (PublicParams.padRectParams != null)
                return;
            ContentControl ccTemp = (ContentControl)ContextMenuService.GetPlacementTarget(LogicalTreeHelper.GetParent(sender as MenuItem));
            PublicParams.padRectParams = new PadRectParams(ccTemp.Name, ccTemp.ActualWidth, ccTemp.ActualHeight, Canvas.GetLeft(ccTemp), Canvas.GetTop(ccTemp),Convert.ToInt32( ccTemp.FontSize),ref ccTemp) { Width = 400, Height = 300, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Name = "padRectParams", Visibility = Visibility.Visible };
            PublicParams.pubLayoutRoot.Children.Add(PublicParams.padRectParams);
            Canvas.SetZIndex(PublicParams.padRectParams, 65535);
            //Control c = ccTemp.DataContext as Control;
            //c.Height = 300;
            //ccTemp.Height = 300;
            //Rectangle rt = (Rectangle)ccTemp.Content;
            //rt.Height = 300;

        }

        /// <summary>
        /// LPY 2016-7-29 添加
        /// 首次加载或更新“动态视频”模块位置时，重新计算视频窗口开窗位置
        /// </summary>
        /// <param name="w">宽</param>
        /// <param name="h">高</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void RecalculateVideosPosition(int w,int h,int x,int y)
        {
            PublicParams.w = w * screenRaito / 2;
            PublicParams.h = (h * screenRaito - PublicParams.winHeight - PublicParams.titleHeight * 2) / 2;

            PublicParams.x1 = x * screenRaito;
            PublicParams.x2 = x * screenRaito + PublicParams.w;
            PublicParams.y1 = y * screenRaito + PublicParams.winHeight + PublicParams.titleHeight;
            PublicParams.y2 = y * screenRaito + PublicParams.winHeight + PublicParams.titleHeight + PublicParams.h + PublicParams.titleHeight;

            LogHelper.WriteLog(PublicParams.w.ToString() + " " + PublicParams.h.ToString() + " " + PublicParams.x1.ToString() + " " + PublicParams.x2.ToString() + " " + PublicParams.y1.ToString() + " " + PublicParams.y2.ToString());
        }
        
    }

}
