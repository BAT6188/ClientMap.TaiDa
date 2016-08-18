using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jovian.ClientMap.classes
{
    using System.Net;
    using System.Net.Sockets;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    /// <summary>
    /// LPY 2015-9-15 添加
    /// 与大屏通过TCP通信
    /// 静态类
    /// </summary>
    public static class TCPServer
    {
        public static Socket socketServer = null;

        /// <summary>
        /// LPY 2015-9-15 添加
        /// 发送指令到大屏
        /// 静态方法
        /// </summary>
        /// <param name="cmdStr"></param>
        public static void SendCMD(string cmdStr)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(cmdStr.Replace(PublicParams.splitChar, "") + PublicParams.splitChar);
            try
            {
                if (socketServer != null)
                {
                    socketServer.Send(bytes);
                    //LogHelper.WriteLog(cmdStr);
                }
            }
            catch (Exception)
            {
            }
        }

        public static void ListenToServer()
        {
            try
            {
                byte[] bytes = new byte[256 * 1024];

                int intReceivedBytesLength;

                while (true)
                {
                    if (socketServer == null)
                        continue;
                    intReceivedBytesLength = socketServer.Receive(bytes, bytes.Length, 0);
                    if (intReceivedBytesLength == 0)
                        break;
                    string strReceived = Encoding.UTF8.GetString(bytes, 0, intReceivedBytesLength);

                    JObject json = JObject.Parse(strReceived);

                    PublicParams.pubMainMap.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        HandleCommands(json);
                    }));
                }
            }
            catch (Exception)
            {
            }
        }

        private static void HandleCommands(JObject rejson)
        {
            switch (rejson["CMD"].ToString())
            {
                case "SYNCR":
                    MapMethods.MoveAndZoomMapByJson(rejson);
                    break;
                case "PoliceCarPositon":
                    //LogHelper.WriteLog(PublicParams.type, rejson["TITLE"].ToString() + "   " + rejson["X"].ToString() + "   " + rejson["Y"].ToString());
                    PublicParams.bigScreenX = Convert.ToInt32(rejson["X"].ToString());
                    PublicParams.bigScreenY = Convert.ToInt32(rejson["Y"].ToString());
                    WallVideosHelper.MovePoliceCarVideoToWall(PublicParams.bigScreenCamera);
                    break;
                default:
                    break;
            }
        }

    }
}
