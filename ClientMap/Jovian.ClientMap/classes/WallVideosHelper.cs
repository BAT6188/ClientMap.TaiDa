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

                DVCSServer.SendCMD(DVCSAgreement.OpenWin(PublicParams.wallID, 0, DVCSAgreement.GetByteListByString(mac, false), channelID, xp, yp, PublicParams.w, PublicParams.h, PublicParams.zindex));
                MapMethods.SendShowHidePadVideosTextByID(camera.Name, id + 1, "1");
                camera.ID = id;
                PublicParams.currentVideoFlag = id;
                PublicParams.currentCamera = camera;
                PublicParams.arrayOpenedVideos[id] = camera;//把已打开的视频保存到已打开视频列表中，并等待更新已打开视频列表中该项的Camera.WinID，因为关闭窗口的时候要用到WinID
                PadOpenedVideos.RefreshOpenedVideos();

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

            string mac = PublicParams.MACPoliceCarVideo1; int channelID = PublicParams.channelPoliceCarVideo1;

            DVCSServer.SendCMD(DVCSAgreement.OpenWin(PublicParams.wallID, 0, DVCSAgreement.GetByteListByString(mac, false), channelID, bigScrX, bigScrY, PublicParams.w, PublicParams.h, PublicParams.zindex));
            PublicParams.bigScreenCamera = camera;
            PublicParams.isPoliceCarVideoSend = 1;//视频上墙标示，接收winid用
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

            DVCSServer.SendCMD(DVCSAgreement.MoveWin(camera.WinID, 1, bigScrX, bigScrY, PublicParams.w, PublicParams.h, PublicParams.zindex));
        }

        /// <summary>
        /// 关闭大屏上的视频
        /// </summary>
        /// <param name="camera"></param>
        public static void ClosePoliceCarVideoToWall(Camera camera)
        {
            if (PublicParams.bigScreenCamera==null)
                return;
            DVCSServer.SendCMD(DVCSAgreement.CloseWin(camera.WinID));
            PublicParams.bigScreenCamera = null;
        }
    }
}
