﻿using System;
using System.Xml.Serialization;
using MediaPortal.GUI.Library;
using MediaPortal.Player;

namespace OnlineVideos.Player
{
    [Serializable]
    public enum PlayerType
    {
        [XmlEnum(Name = "Auto")]
        Auto,
        [XmlEnum(Name = "Internal")]
        Internal,
        [XmlEnum(Name = "WMP")]
        WMP
    }
    public class PlayerFactory : IPlayerFactory
    {
        PlayerType playerType = PlayerType.Auto;        

        public PlayerFactory(PlayerType playerType)
        {
            this.playerType = playerType;
        }
        
        public IPlayer Create(string filename)
        {
            return Create(filename, g_Player.MediaType.Video);
        }  

        public IPlayer Create(string filename, g_Player.MediaType type)
        {
            switch (playerType)
            {
                case PlayerType.Internal:
                    Log.Debug("returnerer OnlineVideosPlayer");
                    return new OnlineVideosPlayer();
                case PlayerType.WMP:
                    Log.Debug("returnerer WMPVideoPlayer");
                    return new WMPVideoPlayer();
                default:
                    Uri uri = new Uri(filename);
                    
                    if (uri.Scheme == "rtsp" || uri.Scheme == "mms" || uri.PathAndQuery.Contains(".asf"))
                    {
                        Log.Debug("returnerer onlinevideosplayer");
                        return new OnlineVideosPlayer();
                    }
                    else if (uri.PathAndQuery.Contains(".asx"))
                    {
                        Log.Debug("returnerer WMPVideoPlayer");
                        return new WMPVideoPlayer();
                    }
                    else
                    {
//                        foreach (string anExt in OnlineVideoSettings.getInstance().videoExtensions.Keys)
//                        {
//                            if (uri.PathAndQuery.Contains(anExt))
//                            {
//                                if (anExt == ".wmv" && !string.IsNullOrEmpty(uri.Query))
//                                {
//                                    return new WMPVideoPlayer();
//                                }
//                                else
//                                {
//                                    return new OnlineVideosPlayer();
//                                }
//                            }
//                        }
//                        return new WMPVideoPlayer();
                        if (filename.ToLower().EndsWith(".wmv"))
                        {
                            Log.Debug("returnerer WMPVideoPlayer");
                              return new WMPVideoPlayer();
                                }
                                else
                                {
                                    Log.Debug("returnerer OnlineVideosPlayer");
                                    return new OnlineVideosPlayer();
                                }
                    }
            }            
        }              
    }
}
