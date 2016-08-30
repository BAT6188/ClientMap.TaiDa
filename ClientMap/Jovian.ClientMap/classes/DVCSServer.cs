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
    public class DVCSServer
    {
        public Socket socketDVCSServer = null;
        public string dvcsServerIP;
        public int dvcsServerPort;
        public string dvcsName;

        public void SendCMD(byte[] cmd)
        {
            try
            {
                if (socketDVCSServer != null)
                {
                    socketDVCSServer.Send(cmd);
                    LogHelper.WriteLog(string.Format("{1}-向{0}发送，IP：{2}",dvcsName, DVCSAgreement.GetStringFromBytes(cmd),dvcsServerIP));
                    
                }
            }
            catch (Exception)
            {
                LogHelper.WriteLog("与DVCS服务器未连接！- DVCSServer.SendCMD()");
            }
        }

        public void ListenToDVCSServer()
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

        private void HandleDVCSCommands(byte[] bytes)
        {
            try
            {
                LogHelper.WriteLog(string.Format("{1}-从{0}收到", dvcsName, DVCSAgreement.GetStringFromBytes(bytes)));
                if (bytes.Length <= 0)
                    return;
                switch (bytes[7])//回复代码第八位为动作指示，如：开窗结果回复，关窗结果回复，移窗结果回复等。
                {
                    case 0x87://开窗回复
                        if (bytes.Length==0x13)//开窗成功的指令长度，截取其第
                        {
                            List<byte> bytesWinID = bytes.Skip(13).Take(4).ToList();//第13位开始的4位为winid
                            int winID = DVCSAgreement.GetIntFromBytesList(bytesWinID, true);
                            if (dvcsName == PublicParams.dvcsServerMainName)
                            {
                                if (PublicParams.isPoliceCarVideoSend == 1)
                                {
                                    PublicParams.isPoliceCarVideoSend = 0;
                                    PublicParams.bigScreenCamera.WinID = winID;
                                    break;
                                }
                                PublicParams.arrayOpenedVideos[PublicParams.currentVideoFlag].WinID = winID;//设置其已打开窗口所返回的winid

                                //LogHelper.WriteLog(string.Format("已将PublicParams.arrayOpenedVideos-{0}处的camera的winID设置为：{1}", PublicParams.currentVideoFlag.ToString(), winID.ToString()));
                                WallVideosHelper.RefreshOpenedVideos();
                                //LogHelper.WriteLog(string.Format("收到服务器返回的WinID：{0}--{1}", winID.ToString(), PublicParams.dvcsServerMainName));
                                break;
                            }
                            else if (dvcsName == PublicParams.dvcsServer2Name)
                            {
                                //LogHelper.WriteLog("在HandleDVCSCommands中打印，执行PublicParams.arrayOpenedVideosDVCS2[0].WinID = winID前-------------------------------" + PublicParams.dvcsServer2Name);
                                //WallVideosHelper.ShowKeyMessage();//mark by LPY 打印关键信息-调试用
                                PublicParams.arrayOpenedVideosDVCS2[0].WinID = winID;
                                WallVideosHelper.RefreshOpenedVideos();
                                //LogHelper.WriteLog("在HandleDVCSCommands中打印，执行PublicParams.arrayOpenedVideosDVCS2[0].WinID = winID后-------------------------------" + PublicParams.dvcsServer2Name);
                                //WallVideosHelper.ShowKeyMessage();//mark by LPY 打印关键信息-调试用
                                //LogHelper.WriteLog(string.Format("收到服务器返回的WinID：{0}--{1}", winID.ToString(), PublicParams.dvcsServer2Name));
                                break;
                            }
                        }
                        break;
                    case 0x89://关窗回复

                        break;
                    default:
                        break;
                }
                //LogHelper.WriteLog(dvcsName + "------------------------------------------------------------------------------------------------");
            }
            catch (Exception)
            {
            }
        }


    }
}
