using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jovian.ClientMap.classes
{
    /// <summary>
    /// LPY 2015-9-21 添加
    /// 静态类
    /// DVCS协议
    /// 大屏与DVCS服务器通过TCP/IP协议连接，并发送指令控制在大屏上加载预案、开窗、移窗、关窗等
    /// </summary>
    public static class DVCSAgreement
    {
        private static List<byte> head = new List<byte> { 0xFF, 0xFF, 0x0A, 0x0E, 0x0C, 0x01 }; //指令头
        private static byte reserve = 0xFF;                                                     //固定位
        private static List<byte> end = new List<byte> { 0xFE, 0xFE };                          //指令尾

        /// <summary>
        /// 预案加载
        /// </summary>
        /// <param name="dwID"></param>
        /// <param name="layoutID"></param>
        /// <returns></returns>
        public static byte[] LoadPlan(int dwID, int layoutID)
        {
            try
            {
                List<byte> cmd = new List<byte>();
                //计算指令总长度
                int len = head.Count + 1 + 1 + 4 + 1 + 1 + end.Count;//head + reserve + cmdid + cmdlength + contentslen + end                
                List<byte> totalLen = GetByteListByString(GetHexStringByInt(len),true);

                cmd.AddRange(head);
                cmd.Add(reserve);
                cmd.Add(0x03);//0x03:预案加载
                cmd.AddRange(totalLen);//
                cmd.Add(Convert.ToByte(dwID));
                cmd.Add(Convert.ToByte(layoutID));
                cmd.AddRange(end);
                return cmd.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// LPY 2015-9-22 添加
        /// 开窗1：根据大屏幕墙ID开窗
        /// </summary>
        /// <param name="wallID">大屏幕墙ID</param>
        /// <param name="winID">窗口ID</param>
        /// <param name="mac">MAC地址</param>
        /// <param name="channelID">信号源通道</param>
        /// <param name="x">左上点X</param>
        /// <param name="y">左上点Y</param>
        /// <param name="w">窗口宽度</param>
        /// <param name="h">窗口高度</param>
        /// <param name="zindex">窗口层次（65535：顶层，0：底层）</param>
        /// <returns></returns>
        public static byte[] OpenWin(int wallID, int winID, List<byte> mac,int channelID, int x, int y, int w, int h, int zindex)
        {
            try
            {
                List<byte> cmd = new List<byte>();

                int len = 50;//计算指令总长度

                cmd.AddRange(head);
                cmd.Add(reserve);
                cmd.Add(0x07);//0x07:开窗指令
                cmd.AddRange(GetByteListByString(GetHexStringByInt(len),true));//指令长度
                cmd.Add(0x01);//01:代表大屏幕墙ID，而不是大屏幕墙名称
                cmd.AddRange(GetByteListByString(GetHexStringByInt(wallID), true));//大屏幕墙ID
                cmd.AddRange(GetByteListByString(GetHexStringByInt(winID), true));//窗口ID
                cmd.AddRange(mac);//MAC地址
                cmd.Add(Convert.ToByte(channelID));
                cmd.AddRange(GetByteListByString(GetHexStringByInt(x), true));//左上点X
                cmd.AddRange(GetByteListByString(GetHexStringByInt(y), true));//左上点Y
                cmd.AddRange(GetByteListByString(GetHexStringByInt(w), true));//窗口宽度
                cmd.AddRange(GetByteListByString(GetHexStringByInt(h), true));//窗口高度
                cmd.AddRange(GetByteListByString(GetHexStringByInt(zindex), true));//窗口层次（65535：顶层，0：底层）
                cmd.AddRange(end);
              
                return cmd.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// LPY 2015-9-22 添加
        /// 开窗1：根据大屏幕墙名称开窗
        /// </summary>
        /// <param name="wallNameLen">大屏幕墙名字长度</param>
        /// <param name="wallName">大屏幕墙名字</param>
        /// <param name="winID">窗口ID</param>
        /// <param name="mac">MAC地址</param>
        /// <param name="channelID">信号源通道</param>
        /// <param name="x">左上点X</param>
        /// <param name="y">左上点Y</param>
        /// <param name="w">窗口宽度</param>
        /// <param name="h">窗口高度</param>
        /// <param name="zindex">窗口层次（65535：顶层，0：底层）</param>
        /// <returns></returns>
        public static byte[] OpenWin(int wallNameLen, List<byte> wallName, int winID, List<byte> mac, int channelID, int x, int y, int w, int h, int zindex)
        {
            try
            {
                List<byte> cmd = new List<byte>();

                int len = 47+wallName.Count;//计算指令总长度///////////////////////////这里要考虑大屏幕墙名字的长度不定，要改写47+

                cmd.AddRange(head);
                cmd.Add(reserve);
                cmd.Add(0x07);//0x07:开窗指令
                cmd.AddRange(GetByteListByString(GetHexStringByInt(len), true));//指令长度
                cmd.Add(0x00);//00代表大屏幕墙名称，而不是大屏幕墙ID
                cmd.Add(Convert.ToByte(wallName.Count));//大屏幕墙名字长度
                cmd.AddRange(wallName);//大屏幕墙名字
                cmd.AddRange(GetByteListByString(GetHexStringByInt(winID), true));//窗口ID
                cmd.AddRange(mac);//MAC地址
                cmd.Add(Convert.ToByte(channelID));
                cmd.AddRange(GetByteListByString(GetHexStringByInt(x), true));//左上点X
                cmd.AddRange(GetByteListByString(GetHexStringByInt(y), true));//左上点Y
                cmd.AddRange(GetByteListByString(GetHexStringByInt(w), true));//窗口宽度
                cmd.AddRange(GetByteListByString(GetHexStringByInt(h), true));//窗口高度
                cmd.AddRange(GetByteListByString(GetHexStringByInt(zindex), true));//窗口层次（65535：顶层，0：底层）
                cmd.AddRange(end);

                return cmd.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 带标题边框版的开窗
        /// </summary>
        /// <param name="wallID"></param>
        /// <param name="wv"></param>
        /// <returns></returns>
        public static byte[] OpenWinWithTitle(int wallID, WallVideo wv)
        {
            try
            {
                List<byte> cmd = new List<byte>();
                int len = 22 + wv.GetTitleDigits() + 4;//4是窗口信息长度的长度

                cmd.AddRange(head);
                cmd.Add(reserve);
                cmd.Add(0x17);
                cmd.AddRange(GetByteListByString(GetHexStringByInt(len), true));
                cmd.AddRange(GetByteListByString(GetHexStringByInt(wallID), true));
                cmd.AddRange(GetByteListByString(GetHexStringByInt(1), true));
                //cmd.AddRange(GetOneWallVideoInfo(wv));
                cmd.AddRange(wv.GetOneWallVideoInBytes());
                cmd.AddRange(end);
                return cmd.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /*
        public static List<byte> GetOneWallVideoInfo(WallVideo wv)
        {
            List<byte> cmd = new List<byte>();
            cmd.AddRange(GetByteListByString(GetHexStringByInt(wv.GetTitleDigits()), true));
            cmd.AddRange(GetByteListByString(GetHexStringByInt(wv.WinID), true));//窗口ID
            cmd.AddRange(GetByteListByString(GetHexStringByInt(63), true));//窗口信息掩码
            cmd.AddRange(GetByteListByString(GetHexStringByInt(wv.SourceID), true));
            cmd.AddRange(GetByteListByString(GetHexStringByInt(Convert.ToInt32(wv.X)), true));
            cmd.AddRange(GetByteListByString(GetHexStringByInt(Convert.ToInt32(wv.Y)), true));
            cmd.AddRange(GetByteListByString(GetHexStringByInt(Convert.ToInt32(wv.Width)), true));
            cmd.AddRange(GetByteListByString(GetHexStringByInt(Convert.ToInt32(wv.Height)), true));
            cmd.AddRange(GetByteListByString(GetHexStringByInt(wv.Zindex), true));
            cmd.AddRange(GetByteListByString(GetHexStringByInt(63), true));//标题边框信息掩码
            cmd.AddRange(GetByteListByString(GetHexStringByInt(GetBytesFromString(wv.Title).ToList().Count, 2), true));//标题长度
            cmd.AddRange(DVCSAgreement.GetBytesFromString(wv.Title).ToList<byte>());
            cmd.AddRange(GetByteListByString(GetHexStringByInt(wv.FontFamilyID,2), true));
            cmd.AddRange(GetByteListByString(GetHexStringByInt(wv.FontSize, 2), true));
            cmd.Add(wv.Foreground.R); cmd.Add(wv.Foreground.G); cmd.Add(wv.Foreground.B);
            cmd.AddRange(GetByteListByString(GetHexStringByInt(wv.TitlePosition, 2), true));
            cmd.AddRange(GetByteListByString(GetHexStringByInt(wv.TitleSize, 2), true));
            cmd.Add(wv.TitleColor.R); cmd.Add(wv.TitleColor.G); cmd.Add(wv.TitleColor.B);
            cmd.AddRange(GetByteListByString(GetHexStringByInt(wv.IsShow, 2), true));


            return cmd;
        }
        */
        /// <summary>
        /// LPY 2015-9-22 添加
        /// 移窗指令
        /// </summary>
        /// <param name="winID">窗口ID</param>
        /// <param name="way">方式：移动位置和层次</param>
        /// <param name="x">左上点X</param>
        /// <param name="y">左上点Y</param>
        /// <param name="w">窗口宽度</param>
        /// <param name="h">窗口高度</param>
        /// <param name="zindex">窗口层次（65535：顶层，0：底层）</param>
        /// <returns></returns>
        public static byte[] MoveWin(int winID,int way,int x,int y,int w,int h,int zindex)
        {
            try
            {
                List<byte> cmd = new List<byte>();

                int len = 39;//计算指令总长度

                cmd.AddRange(head);
                cmd.Add(reserve);
                cmd.Add(0x08);//0x08:移窗指令
                cmd.AddRange(GetByteListByString(GetHexStringByInt(len), true));//指令长度
                cmd.AddRange(GetByteListByString(GetHexStringByInt(winID), true));//窗口ID
                cmd.Add(Convert.ToByte(way));//方式：移动位置和层次
                cmd.AddRange(GetByteListByString(GetHexStringByInt(x), true));//左上点X
                cmd.AddRange(GetByteListByString(GetHexStringByInt(y), true));//左上点Y
                cmd.AddRange(GetByteListByString(GetHexStringByInt(w), true));//窗口宽度
                cmd.AddRange(GetByteListByString(GetHexStringByInt(h), true));//窗口高度
                cmd.AddRange(GetByteListByString(GetHexStringByInt(zindex), true));//窗口层次（65535：顶层，0：底层）
                cmd.AddRange(end);

                return cmd.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// LPY 2015-9-22 添加
        /// 关窗
        /// </summary>
        /// <param name="winID">窗口ID</param>
        /// <returns></returns>
        public static byte[] CloseWin(int winID)
        {
            try
            {
                List<byte> cmd = new List<byte>();

                int len = 18;//计算指令总长度

                cmd.AddRange(head);
                cmd.Add(reserve);
                cmd.Add(0x09);//0x09:关窗指令
                cmd.AddRange(GetByteListByString(GetHexStringByInt(len), true));//指令长度
                cmd.AddRange(GetByteListByString(GetHexStringByInt(winID), true));//窗口ID
                cmd.AddRange(end);

                return cmd.ToArray();

            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// LPY 2016-3-29 添加
        /// 关窗（带标题版）
        /// </summary>
        /// <param name="winID"></param>
        /// <returns></returns>
        public static byte[] CloseWinWithTitle(int wallID, int winID)
        {
            try
            {
                List<byte> cmd = new List<byte>();
                int len = 24;

                cmd.AddRange(head);
                cmd.Add(reserve);
                cmd.Add(0x09);//0x09:关窗指令
                cmd.AddRange(GetByteListByString(GetHexStringByInt(len), true));//指令长度
                cmd.AddRange(GetByteListByString(GetHexStringByInt(wallID), true));
                cmd.AddRange(GetByteListByString(GetHexStringByInt(1), true));//Count 1个
                cmd.AddRange(GetByteListByString(GetHexStringByInt(winID), true));//窗口ID
                cmd.AddRange(end);

                return cmd.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// LPY 2016-3-29 添加
        /// 清屏
        /// </summary>
        /// <param name="winID"></param>
        /// <returns></returns>
        public static byte[] ClearAllWins(int wallID)
        {
            try
            {
                List<byte> cmd = new List<byte>();
                int len = 18;

                cmd.AddRange(head);
                cmd.Add(reserve);
                cmd.Add(0x09);//0x09:关窗指令
                cmd.AddRange(GetByteListByString(GetHexStringByInt(len), true));//指令长度
                cmd.AddRange(GetByteListByString(GetHexStringByInt(wallID), true));//窗口ID
                cmd.AddRange(end);

                return cmd.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 返回整数的8位16进制字符串
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static string GetHexStringByInt(int num)
        {
            return num.ToString("X8");
        }

        /// <summary>
        /// 返回整数的16进制字符串，位数自定义
        /// </summary>
        /// <param name="num">待转换的整数</param>
        /// <param name="digit">位数</param>
        /// <returns></returns>
        public static string GetHexStringByInt(int num, int digit)
        {
            return num.ToString("X" + digit.ToString());
        }

        /// <summary>
        /// LPY 2015-9-22 添加
        /// 根据整数转换成的十六进制字符串，每两位进行分割，并分别转换成List
        /// </summary>
        /// <param name="hexStr">待转换的字符串</param>
        /// <param name="reverse">标记是否反转结果，true反转</param>
        /// <returns></returns>
        public static List<byte> GetByteListByString(string hexStr,bool reverse)
        {
            List<byte> result = new List<byte>();
            if (hexStr.Length==0)
            {
                return null;
            }
            if (hexStr.Length%2==0)//偶数位且位数不为0
            {
                for (int i = 0; i < hexStr.Length-1; i+=2)
                {
                    result.Add(Convert.ToByte(Convert.ToInt32("0x" + hexStr.Substring(i, 2), 16)));
                }
                if (reverse)
                    result.Reverse();
                return result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetStringFromBytes(byte[] bytes)
        {
            string result = string.Empty;
            if (bytes!=null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    result += bytes[i].ToString("X2");
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GetBytesFromString(string str)
        {
            return Encoding.Default.GetBytes(str);
        }

        public static int GetIntFromBytesList(List<byte> list, bool reverse)
        {
            try
            {
                string strTemp = string.Empty;
                //for (int i = bytes.Length-1; i >=0; i--)
                //    strTemp += bytes[i].ToString("X2");
                //List<byte> list=bytes.ToList();
                if (reverse)
                    list.Reverse();
                foreach (byte b in list)
                {
                    strTemp += b.ToString("X2");
                }
                return Convert.ToInt32("0x" + strTemp, 16);                
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}
