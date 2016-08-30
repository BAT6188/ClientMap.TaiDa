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

    /// <summary>
    /// PadDVCS.xaml 的交互逻辑
    /// </summary>
    public partial class PadDVCS : UserControl
    {
        public PadDVCS()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Height = 0;
        }

        //预案加载
        private void LoadPlan_Click(object sender, RoutedEventArgs e)
        {
            byte[] cmd = DVCSAgreement.LoadPlan(GetInt(dwID1.Text),GetInt(layoutID1.Text));
            MessageBox.Show("将发送：" + DVCSAgreement.GetStringFromBytes(cmd));
            PublicParams.dvcsServerMain.SendCMD(cmd);
        }

        //根据大屏幕墙ID开窗
        private void btnOpenWin21_Click(object sender, RoutedEventArgs e)
        {
            byte[] cmd = DVCSAgreement.OpenWin(GetInt(wallID21.Text), GetInt(winID2.Text), DVCSAgreement.GetByteListByString(MAC.Text, false), GetInt(channelID.Text), GetInt(x.Text), GetInt(y.Text), GetInt(w.Text), GetInt(h.Text), GetInt(zindex.Text));
            MessageBox.Show("将发送：" + DVCSAgreement.GetStringFromBytes(cmd));
            //LogHelper.WriteLog(PublicParams.type, "");
            PublicParams.dvcsServerMain.SendCMD(cmd);
        }

        //根据大屏幕墙名称开窗
        private void btnOpenWin22_Click(object sender, RoutedEventArgs e)
        {
            byte[] cmd = DVCSAgreement.OpenWin(0, DVCSAgreement.GetBytesFromString(winName.Text).ToList<byte>(),GetInt(winID2.Text), DVCSAgreement.GetByteListByString(MAC.Text, false), GetInt(channelID.Text), GetInt(x.Text), GetInt(y.Text), GetInt(w.Text), GetInt(h.Text), GetInt(zindex.Text));
            //byte[] bb = DVCSAgreement.GetBytesFromString(winName.Text);
            //string ss = Encoding.UTF8.GetString(bb);
            MessageBox.Show("将发送：" + DVCSAgreement.GetStringFromBytes(cmd));
            PublicParams.dvcsServerMain.SendCMD(cmd);
            LogHelper.WriteLog(DVCSAgreement.GetStringFromBytes(cmd));
        }

        //移动窗口
        private void btnMoveWin3_Click(object sender, RoutedEventArgs e)
        {
            byte[] cmd = DVCSAgreement.MoveWin(GetInt(winID2.Text), GetInt(way3.Text), GetInt(x.Text), GetInt(y.Text), GetInt(w.Text), GetInt(h.Text), GetInt(zindex.Text));
            MessageBox.Show("将发送：" + DVCSAgreement.GetStringFromBytes(cmd));
            PublicParams.dvcsServerMain.SendCMD(cmd);
        }

        //关窗口根据ID
        private void btnCloseWin_Click(object sender, RoutedEventArgs e)
        {
            //byte[] cmd = DVCSAgreement.CloseWin(GetInt(winID2.Text));
            //MessageBox.Show("将发送：" + DVCSAgreement.GetStringFromBytes(cmd));
            //DVCSServer.SendCMD(cmd);

            //WallVideo wv = new WallVideo("Title", 1, 0, 0, 2048, 1536, 65535, 1);
            //byte[] cmd = DVCSAgreement.OpenWinWithTitle(2, wv);
            ////way3.Text = DVCSAgreement.GetStringFromBytes(cmd);
            //MessageBox.Show(DVCSAgreement.GetStringFromBytes(cmd));
        }

        private int GetInt(string str)
        {
            return Convert.ToInt32(str);
        }
    }
}
