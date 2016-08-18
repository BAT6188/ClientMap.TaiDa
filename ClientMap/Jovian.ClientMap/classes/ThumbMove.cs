using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Jovian.ClientMap.parts;

namespace Jovian.ClientMap.classes
{
    public class ThumbMove:Thumb
    {
        public ThumbMove()
        { 
            DragDelta  +=ThumbMove_DragDelta;
        }

        private void ThumbMove_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Control designerItem = this.DataContext as Control;
            Canvas ca = (Canvas)VisualTreeHelper.GetParent(designerItem);//寻找父控件

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

            RemoteWin rw = new RemoteWin(designerItem.Width, designerItem.Height, Canvas.GetLeft(designerItem), Canvas.GetTop(designerItem), Canvas.GetZIndex(designerItem), designerItem.Name, Convert.ToInt32(designerItem.FontSize), "");
            rw.SendParamsToRemote();
            if (rw.Name == PublicParams.dynamicVideoWinName)//如果加载的是动态视频的窗口，要计算视频上墙的位置
            {
                PadRemoteParams.RecalculateVideosPosition((int)rw.Width, (int)rw.Height, (int)rw.X, (int)rw.Y);
            }

            if (designerItem != null)
            {
                double left = Canvas.GetLeft(designerItem) + e.HorizontalChange;
                double top = Canvas.GetTop(designerItem) + e.VerticalChange;
                if (left <= 0)
                    left = 0;
                if (top <= 0)
                    top = 0;
                //Canvas.GetTop(designerItem.Parent);
                //if top>=(canv
                //Control ca = (Control)designerItem.Parent;
                //double a= ca.Width;
                //Canvas ca= (Canvas) VisualTreeHelper.GetParent(designerItem);
                //double a = ca.Width;
                if (left >= ca.Width - designerItem.Width)
                {
                    left = ca.Width - designerItem.Width;
                }
                if (top >= ca.Height - designerItem.Height)
                {
                    top = ca.Height - designerItem.Height;
                }

                Canvas.SetLeft(designerItem, left);
                Canvas.SetTop(designerItem, top);


            }
        }
    }
}
