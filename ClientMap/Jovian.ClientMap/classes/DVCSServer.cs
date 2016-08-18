using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jovian.ClientMap.classes
{
    using System.Net;
    using System.Net.Sockets;

    using Jovian.ClientMap.parts;
    /// <summary>
    /// LPY 2015-9-29 添加
    /// 与DVCS服务器通信的类
    /// </summary>
    public static class DVCSServer
    {
        public static Socket socketDVCSServer = null;

        public static void SendCMD(byte[] cmd)
        {
            try
            {
                if (socketDVCSServer != null)
                {
                    LogHelper.WriteLog(DVCSAgreement.GetStringFromBytes(cmd));
                    socketDVCSServer.Send(cmd);
                }
            }
            catch (Exception)
            {
                LogHelper.WriteLog("与DVCS服务器未连接！- DVCSServer.SendCMD()");
            }
        }

        public static void ListenToDVCSServer()
        {
            try
            {
                byte[] bytes = new byte[256];
                int intReceivedBytesLength;

                while (true)
                {
                    if (socketDVCSServer == null)
                        continue;
                    intReceivedBytesLength = socketDVCSServer.Receive(bytes, bytes.Length, 0);// socketDVCSServer.Receive( );
                    if (intReceivedBytesLength == 0)
                        break;
                    //byte[] bb = bytes.Take(intReceivedBytesLength).ToArray();
                    //string strReceived = Encoding.UTF8.GetString(bytes, 0, intReceivedBytesLength);

                    //JObject json = JObject.Parse(strReceived);

                    PublicParams.pubMainMap.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        HandleDVCSCommands(bytes.Take(intReceivedBytesLength).ToArray());
                    }));
                }
            }
            catch (Exception)
            {
            }
        }

        private static void HandleDVCSCommands(byte[] bytes)
        {
            try
            {
                //LogHelper.WriteLog(PublicParams.type, DVCSAgreement.GetStringFromBytes(bytes));
                if (bytes.Length <= 0)
                    return;
                switch (bytes[7])//回复代码第八位为动作指示，如：开窗结果回复，关窗结果回复，移窗结果回复等。
                {
                    case 0x87://开窗回复
                        if (bytes.Length==0x13)//开窗成功的指令长度，截取其第
                        {
                            List<byte> bytesWinID = bytes.Skip(13).Take(4).ToList();//第13位开始的4位为winid
                            int winID = DVCSAgreement.GetIntFromBytesList(bytesWinID, true);
                            if (PublicParams.isPoliceCarVideoSend == 1)
                            {
                                PublicParams.bigScreenCamera.WinID = winID;
                                PublicParams.isPoliceCarVideoSend = 0;
                                break;
                            }
                            PublicParams.arrayOpenedVideos[PublicParams.currentVideoFlag].WinID = winID;//设置其已打开窗口所返回的winid
                            PadOpenedVideos.RefreshOpenedVideos();
                        }
                        break;
                    case 0x89://关窗回复

                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
            }
        }


    }
}
