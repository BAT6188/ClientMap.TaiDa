using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;
using Jovian.ClientMap.parts;

namespace Jovian.ClientMap.classes
{
    public class ThumbResize : Thumb
    {
        public ThumbResize()
        {
            DragDelta += ThumbResize_DragDelta;
        }

        private void ThumbResize_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Control designerItem = this.DataContext as Control;
            Canvas ca = (Canvas)VisualTreeHelper.GetParent(designerItem);//寻找父控件
            //Canvas.SetZIndex(designerItem, 0);
            Rectangle rt = (Rectangle)((ContentControl)designerItem).Content;
            rt.Width = designerItem.Width;
            rt.Height = designerItem.Height;//保证背景图片和框的大小相同
            //TextBlock tb = (TextBlock)((StackPanel)((VisualBrush)rt.Fill).Visual).Children[0];
            //tb.Height = 30; tb.Width = designerItem.Width;

            //卢平义 2014-7-4 15:57:35 改变模块大小的时候，设置框内数字随着改动
            //Grid grid = (Grid)VisualTreeHelper.GetParent(ca);
            //TextBox h1 = (TextBox)((WrapPanel)grid.Children[1]).FindName("h1");//找到需要修改的控件                                //WrapPanel wp = (WrapPanel)grid.Children[1];//找到Grid的第二个子控件
            //h1.Text = designerItem.Height.ToString();
            //TextBox w1 = (TextBox)((WrapPanel)grid.Children[1]).FindName("w1");
            //w1.Text = designerItem.Width.ToString();
            //TextBox x1 = (TextBox)((WrapPanel)grid.Children[1]).FindName("x1");
            //x1.Text = Canvas.GetLeft(designerItem).ToString();
            //TextBox y1 = (TextBox)((WrapPanel)grid.Children[1]).FindName("y1");
            //y1.Text = Canvas.GetTop(designerItem).ToString();
            //ComboBox listbox = (ComboBox)((WrapPanel)grid.Children[1]).FindName("listBox");
            //listbox.Text = designerItem.Name;

            //string savedButton = System.Windows.Markup.XamlWriter.Save(this.buttonB);
            //textBox1.Text = savedButton;
            //MessageBox.Show(System.Windows.Markup.XamlWriter.Save(designerItem));

            RemoteWin rw = new RemoteWin(designerItem.Width, designerItem.Height, Canvas.GetLeft(designerItem), Canvas.GetTop(designerItem), Canvas.GetZIndex(designerItem), designerItem.Name, Convert.ToInt32(designerItem.FontSize), "");
            rw.SendParamsToRemote();
            if (rw.Name == PublicParams.dynamicVideoWinName)//如果加载的是动态视频的窗口，要计算视频上墙的位置
            {
                PadRemoteParams.RecalculateVideosPosition((int)rw.Width, (int)rw.Height, (int)rw.X, (int)rw.Y);
            }

            if (designerItem != null)
            {
                double deltaVertical, deltaHorizontal;

                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        deltaVertical = Math.Min(-e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                        //designerItem.Height -= deltaVertical;
                        if (Canvas.GetTop(designerItem) + designerItem.Height + e.VerticalChange >= ca.Height)//防止越界
                        {
                            designerItem.Height = ca.Height - Canvas.GetTop(designerItem);
                        }
                        else
                        {
                            designerItem.Height -= deltaVertical;
                        }
                        break;
                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange, designerItem.ActualHeight - designerItem.MinHeight);
                        double nTop = Canvas.GetTop(designerItem) + deltaVertical;
                        if (nTop >= 0)//防止越界
                        {
                            Canvas.SetTop(designerItem, nTop);
                            designerItem.Height -= deltaVertical;
                        }
                        break;
                    default:
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                        if (Canvas.GetLeft(designerItem) + deltaHorizontal >= 0)//调整模块大小的时候，防止模块左边缘出界
                        {
                            Canvas.SetLeft(designerItem, Canvas.GetLeft(designerItem) + deltaHorizontal);
                            designerItem.Width -= deltaHorizontal;
                        }
                        break;
                    case HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange, designerItem.ActualWidth - designerItem.MinWidth);
                        //designerItem.Width -= deltaHorizontal;
                        if (Canvas.GetLeft(designerItem) + designerItem.ActualWidth + e.HorizontalChange >= ca.Width)//调整模块大小的时候，防止模块下边缘出界
                        {
                            designerItem.Width = ca.Width - Canvas.GetLeft(designerItem);
                        }
                        else
                        {
                            designerItem.Width -= deltaHorizontal;
                        }
                        break;
                    default:
                        break;
                }
            }

            e.Handled = true;
        }
    }


}
