using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jovian.ClientMap.classes
{
    using Jovian.ClientMap.parts;
    public class WallVideosHelper
    {
        public static void InitOpenedVideos()
        {
            for (int i = 0; i < PublicParams.arrayOpenedVideos.Length; i++)
            {
                PublicParams.arrayOpenedVideos[i] = null;
            }
        }

        public static void OpenVideoToWall(Camera camera)
        {
            /*
            for (int i = 0; i < PublicParams.arrayWallVideo.Length; i++)
            {
                if (PublicParams.arrayWallVideo[i]!=null)
                {
                    continue;
                }
                switch (i)
                {
                    case 0:
                        DVCSServer.SendCMD(DVCSAgreement.OpenWin(1, 0, DVCSAgreement.GetByteListByString(MAC,false), 0, x2, y1, w, h, zindex));
                        WallVideo wv0 = new WallVideo("XXXXX", 0);
                        PublicParams.listWallVideos.Add(wv0);
                        PublicParams.arrayWallVideo[i] = wv0;
                        MapMethods.SendOpenPadVideos();
                        break;
                        
                    case 1:
                        DVCSServer.SendCMD(DVCSAgreement.OpenWin(1, 0, DVCSAgreement.GetByteListByString(MAC, false), 1, x2, y2, w, h, zindex));
                        WallVideo wv1 = new WallVideo("XXXXX", 0);
                        PublicParams.listWallVideos.Add(wv1);
                        PublicParams.arrayWallVideo[i] = wv1;
                        break;
                    case 2:
                        DVCSServer.SendCMD(DVCSAgreement.OpenWin(1, 0, DVCSAgreement.GetByteListByString(MAC, false), 2, x1, y1, w, h, zindex));
                        WallVideo wv2 = new WallVideo("XXXXX", 0);
                        PublicParams.listWallVideos.Add(wv2);
                        PublicParams.arrayWallVideo[i] = wv2;
                        break;
                    case 3:
                        DVCSServer.SendCMD(DVCSAgreement.OpenWin(1, 0, DVCSAgreement.GetByteListByString(MAC, false), 3, x1, y2, w, h, zindex));
                        WallVideo wv3 = new WallVideo("XXXXX", 0);
                        PublicParams.listWallVideos.Add(wv3);
                        PublicParams.arrayWallVideo[i] = wv3;
                        break;
                    default:
                        break;
                }
                break;
             
            }
             */
            int id = GetEmptyWallVideo();
            string mac = string.Empty;
            int channelID = 0;
            if (id!=-1)
            {
                int xp = 0;int yp = 0;
                switch (id)
                {
                    case 0:
                        xp=PublicParams.x2;yp=PublicParams.y1;mac = PublicParams.MAC1;channelID=PublicParams.channel1;
                        break;
                    case 1:
                        xp=PublicParams.x2;yp=PublicParams.y2;mac = PublicParams.MAC2;channelID=PublicParams.channel2;
                        break;
                    case 2:
                        xp=PublicParams.x1;yp=PublicParams.y1;mac = PublicParams.MAC3;channelID=PublicParams.channel3;
                        break;
                    case 3:
                        xp=PublicParams.x1;yp=PublicParams.y2;mac = PublicParams.MAC4;channelID=PublicParams.channel4;
                        break;
                    default:
                        break;
                }
                camera.ID = id; camera.MAC = mac; camera.ChannelID = channelID; camera.Kind = PublicParams.dvcsServerMainName;
                PublicParams.currentVideoFlag = id;
                PublicParams.arrayOpenedVideos[id] = new Camera(camera);//把已打开的视频保存到已打开视频列表中，并等待更新已打开视频列表中该项的Camera.WinID，因为关闭窗口的时候要用到WinID

                PublicParams.dvcsServerMain.SendCMD(DVCSAgreement.OpenWin(PublicParams.wallID, 0, DVCSAgreement.GetByteListByString(mac, false), channelID, xp, yp, PublicParams.w, PublicParams.h, PublicParams.zindex));
                MapMethods.SendShowHidePadVideosTextByID(camera.Name, id + 1, "1");

                //LogHelper.WriteLog("在OpenVideoToWall中打印，执行OpenVideoToWallForDVCS2前-------------------------------");
                //WallVideosHelper.ShowKeyMessage();//mark by LPY 打印关键信息-调试用

                WallVideosHelper.RefreshOpenedVideos();
                camera.Kind = PublicParams.dvcsServer2Name;
                OpenVideoToWallForDVCS2(camera);
                //LogHelper.WriteLog("在OpenVideoToWall中打印，执行OpenVideoToWallForDVCS2后-------------------------------");
                //WallVideosHelper.ShowKeyMessage();//mark by LPY 打印关键信息-调试用

                ////LPY 2016-3-29 添加该段代码 但是由于中达新接口不打算启用，注释掉
                //WallVideo wv = new WallVideo(camera.Name, camera.SourceID, xp, yp, PublicParams.w, PublicParams.h, PublicParams.zindex, 1);
                //DVCSServer.SendCMD(DVCSAgreement.OpenWinWithTitle(PublicParams.wallID, wv));
                //camera.ID = id;
                //PublicParams.currentVideoFlag = id;
                //PublicParams.currentCamera = camera;
                //PublicParams.arrayOpenedVideos[id] = camera;
                //PadOpenedVideos.RefreshOpenedVideos();                
            }
        }

        
        public static int GetEmptyWallVideo()//返回还未有视频的序号，依据该序号定位视频上墙的位置
        {
            for (int i = 0; i < PublicParams.arrayOpenedVideos.Length; i++)
            {
                if (PublicParams.arrayOpenedVideos[i] != null)
                    continue;
                return i;
            }
            return -1;
        }


        public static void OpenPoliceCarVideoToWall(Camera camera)
        {
            if (PublicParams.bigScreenCamera != null)
                return;

            int bigScrX = PublicParams.bigScreenX - PublicParams.w / 2;
            int bigScrY = PublicParams.bigScreenY - PublicParams.h;

            bigScrX = bigScrX > 0 ? bigScrX : 0;
            bigScrY = bigScrY > 0 ? bigScrY : 0;
            
            PublicParams.isPoliceCarVideoSend = 1;//视频上墙标示，接收winid用
            camera.MAC = PublicParams.MACPoliceCarVideo1;
            camera.ChannelID = PublicParams.channelPoliceCarVideo1;
            PublicParams.dvcsServerMain.SendCMD(DVCSAgreement.OpenWin(PublicParams.wallID, 0, DVCSAgreement.GetByteListByString(PublicParams.MACPoliceCarVideo1, false), PublicParams.channelPoliceCarVideo1, bigScrX, bigScrY, PublicParams.w, PublicParams.h, PublicParams.zindex));            
            PublicParams.bigScreenCamera = camera;

            camera.Kind = PublicParams.dvcsServer2Name;//为了增加Kind标示，关闭的时候有用
            OpenVideoToWallForDVCS2(new Camera(camera));
        }

        /// <summary>
        /// 移动大屏上的视频
        /// </summary>
        /// <param name="camera"></param>
        public static void MovePoliceCarVideoToWall(Camera camera)
        {
            if (PublicParams.bigScreenCamera == null)
                return;
            if (PublicParams.bigScreenCamera.WinID == -1)//已打开的视频ID可能还未回发回来
                return;

            int bigScrX = PublicParams.bigScreenX - PublicParams.w / 2;
            int bigScrY = PublicParams.bigScreenY - PublicParams.h;

            bigScrX = bigScrX > 0 ? bigScrX : 0;
            bigScrY = bigScrY > 0 ? bigScrY : 0;

            PublicParams.dvcsServerMain.SendCMD(DVCSAgreement.MoveWin(camera.WinID, 1, bigScrX, bigScrY, PublicParams.w, PublicParams.h, PublicParams.zindex));
        }

        /// <summary>
        /// 关闭大屏上的视频
        /// </summary>
        /// <param name="camera"></param>
        public static void ClosePoliceCarVideoToWall(Camera camera)
        {
            if (PublicParams.bigScreenCamera==null)
                return;
            PublicParams.dvcsServerMain.SendCMD(DVCSAgreement.CloseWin(camera.WinID));
            PublicParams.bigScreenCamera = null;
            CloseVideoToWallForDVCS2(camera);
        }


        public static void OpenVideoToWallForDVCS2(Camera camera)
        {
            //add by LPY 开视频同时向第二DVCS服务器发送开视频指令
            if (PublicParams.arrayOpenedVideosDVCS2[0] != null)
            {
                if (PublicParams.arrayOpenedVideosDVCS2[0].WinID != 0)
                    PublicParams.dvcsServer2.SendCMD(DVCSAgreement.CloseWin(PublicParams.arrayOpenedVideosDVCS2[0].WinID));
            }
            PublicParams.arrayOpenedVideosDVCS2[0] =new Camera(camera);
            PublicParams.dvcsServer2.SendCMD(DVCSAgreement.OpenWin(PublicParams.dvcs2wallID, 0, DVCSAgreement.GetByteListByString(camera.MAC, false),camera.ChannelID, PublicParams.dvcs2x1, PublicParams.dvcs2y1, PublicParams.dvcs2w, PublicParams.dvcs2h, PublicParams.zindex));
            
            RefreshOpenedVideos();
        }

        public static void CloseVideoToWallForDVCS2(Camera camera)
        {
            if (PublicParams.arrayOpenedVideosDVCS2[0] == null)
                return;
            try
            {
                if (camera.Name == PublicParams.arrayOpenedVideosDVCS2[0].Name)
                {
                    if (PublicParams.arrayOpenedVideosDVCS2[0].WinID != 0)
                    {
                        int dvcs2WinID = PublicParams.arrayOpenedVideosDVCS2[0].WinID;
                        PublicParams.dvcsServer2.SendCMD(DVCSAgreement.CloseWin(dvcs2WinID));
                        PublicParams.arrayOpenedVideosDVCS2[0] = null;
                        //LogHelper.WriteLog(string.Format("已发送指令，关闭WinID：{0}的视频--{1}", dvcs2WinID.ToString(), PublicParams.dvcsServer2Name));
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog("关闭DVCS2视频出现错误--CloseVideoToWallForDVCS2--" + ex.Message);
            }
            
            
            RefreshOpenedVideos();
        }

        public static void RefreshOpenedVideos()
        {
            PublicParams.padOpenedVideos.lstOpenedVideos.ItemsSource = null;
            PublicParams.padOpenedVideos.lstOpenedVideos.ItemsSource = PublicParams.arrayOpenedVideos;

            PublicParams.padOpenedVideos.lstOpenedVideosOfDVCS2.ItemsSource = null;
            PublicParams.padOpenedVideos.lstOpenedVideosOfDVCS2.ItemsSource = PublicParams.arrayOpenedVideosDVCS2;
        }

        //public static void ShowKeyMessage()
        //{
        //    for (int i = 0; i < PublicParams.arrayOpenedVideos.Length; i++)
        //    {
        //        if (PublicParams.arrayOpenedVideos[i] != null)
        //            LogHelper.WriteLog(PublicParams.arrayOpenedVideos[i].WinID.ToString() + "---" + PublicParams.arrayOpenedVideos[i].Name);
        //        else
        //            LogHelper.WriteLog("Null-" + i.ToString());
        //    }

        //    if (PublicParams.arrayOpenedVideosDVCS2[0] != null)
        //        LogHelper.WriteLog(PublicParams.arrayOpenedVideosDVCS2[0].WinID.ToString() + "---" + PublicParams.arrayOpenedVideosDVCS2[0].Name);
        //    else
        //        LogHelper.WriteLog("Null-");
        //}
    }
}
