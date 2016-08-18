using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Jovian.ClientMap.classes
{
    /// <summary>
    /// LPY 2016-3-28 添加
    /// 上墙视频类
    /// </summary>
    public class WallVideo
    {
        private int winID;//窗口ID（预留）
        private int sourceID;//信号源ID
        private double x;
        private double y;
        private double width;
        private double height;
        private int zindex;

        private string title;//标题名称
        private int fontfamilyID;//标题字体名称索引
        private int fontsize;//标题字体大小
        private Color foreground;//标题字体颜色
        private int titleposition;//标题显示位置（顶部居中/底部居中）
        private int titlesize;//边框大小
        private Color titlecolor;//边框颜色
        private int isshow;//标题边框是否显示

        public int WinID { get { return winID; } set { winID = value; } }
        public int SourceID { get { return sourceID; } set { sourceID = value; } }
        public double X { get { return x; } set { x = value; } }
        public double Y { get { return y; } set { y = value; } }
        public double Width { get { return width; } set { width = value; } }
        public double Height { get { return height; } set { height = value; } }
        public int Zindex { get { return zindex; } set { zindex = value; } }
        public string Title { get { return title; } set { title = value; } }
        public int FontFamilyID { get { return fontfamilyID; } set { fontfamilyID = value; } }
        public int FontSize { get { return fontsize; } set { fontsize = value; } }
        public Color Foreground { get { return foreground; } set { foreground = value; } }
        public int TitlePosition { get { return titleposition; } set { titleposition = value; } }
        public int TitleSize { get { return titlesize; } set { titlesize = value; } }
        public Color TitleColor { get { return titlecolor; } set { titlecolor = value; } }
        public int IsShow { get { return isshow; } set { isshow = value; } }

        public WallVideo(string _title,int _sourceID,double _x,double _y,double _width,double _height,int _zindex,int _isshow)
        { 
            this.Title=_title;this.SourceID=_sourceID;this.X=_x;this.Y=_y;this.Width=_width;this.Height=_height;this.Zindex=_zindex;this.IsShow=_isshow;
            WinID=0;
            FontFamilyID=1;
            FontSize=16;
            Foreground=Colors.Yellow;
            TitlePosition=0;
            TitleSize=16;
            TitleColor=Colors.Gray;
            
        }
        //返回位数
        public int GetTitleDigits()
        {
            int digits = 4 * 9 + 1 + Encoding.Default.GetBytes(title).ToList().Count + 1 + 1 + 3 + 1 + 1 + 3 + 1;
            return digits;
        }

        public List<byte> GetOneWallVideoInBytes()
        {
            List<byte> cmd = new List<byte>();

            cmd.AddRange(DVCSAgreement.GetByteListByString(DVCSAgreement.GetHexStringByInt(GetTitleDigits()), true));
            cmd.AddRange(DVCSAgreement.GetByteListByString(DVCSAgreement.GetHexStringByInt(WinID), true));//窗口ID
            cmd.AddRange(DVCSAgreement.GetByteListByString(DVCSAgreement.GetHexStringByInt(63), true));//窗口信息掩码
            cmd.AddRange(DVCSAgreement.GetByteListByString(DVCSAgreement.GetHexStringByInt(SourceID), true));
            cmd.AddRange(DVCSAgreement.GetByteListByString(DVCSAgreement.GetHexStringByInt(Convert.ToInt32(X)), true));
            cmd.AddRange(DVCSAgreement.GetByteListByString(DVCSAgreement.GetHexStringByInt(Convert.ToInt32(Y)), true));
            cmd.AddRange(DVCSAgreement.GetByteListByString(DVCSAgreement.GetHexStringByInt(Convert.ToInt32(Width)), true));
            cmd.AddRange(DVCSAgreement.GetByteListByString(DVCSAgreement.GetHexStringByInt(Convert.ToInt32(Height)), true));
            cmd.AddRange(DVCSAgreement.GetByteListByString(DVCSAgreement.GetHexStringByInt(Zindex), true));
            cmd.AddRange(DVCSAgreement.GetByteListByString(DVCSAgreement.GetHexStringByInt(63), true));//标题边框信息掩码
            cmd.AddRange(DVCSAgreement.GetByteListByString(DVCSAgreement.GetHexStringByInt(DVCSAgreement.GetBytesFromString(Title).ToList().Count, 2), true));//标题长度
            cmd.AddRange(DVCSAgreement.GetBytesFromString(Title).ToList<byte>());
            cmd.AddRange(DVCSAgreement.GetByteListByString(DVCSAgreement.GetHexStringByInt(FontFamilyID, 2), true));
            cmd.AddRange(DVCSAgreement.GetByteListByString(DVCSAgreement.GetHexStringByInt(FontSize, 2), true));
            cmd.Add(Foreground.R); cmd.Add(Foreground.G); cmd.Add(Foreground.B);
            cmd.AddRange(DVCSAgreement.GetByteListByString(DVCSAgreement.GetHexStringByInt(TitlePosition, 2), true));
            cmd.AddRange(DVCSAgreement.GetByteListByString(DVCSAgreement.GetHexStringByInt(TitleSize, 2), true));
            cmd.Add(TitleColor.R); cmd.Add(TitleColor.G); cmd.Add(TitleColor.B);
            cmd.AddRange(DVCSAgreement.GetByteListByString(DVCSAgreement.GetHexStringByInt(IsShow, 2), true));

            return cmd;
        }

        public WallVideo()
        { }
    }
}
