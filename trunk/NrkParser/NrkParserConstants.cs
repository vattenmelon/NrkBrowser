using System;
using System.Collections.Generic;
using System.Text;

namespace Vattenmelon.Nrk.Parser
{
    public class NrkParserConstants
    {
       

        public static string STREAM_PREFIX
        {
            get { return "mms://straumV.nrk.no/nrk_tv_webvid"; }
        }

        public static string RSS_URL
        {
            get { return "http://www1.nrk.no/nett-tv/MediaRss.ashx?loop="; }
        }

        public static string URL_GET_MEDIAXML
        {
            get { return "http://www1.nrk.no/nett-tv/silverlight/getmediaxml.ashx?id={0}&hastighet={1}"; }
        }

        public static string GEOBLOCK_URL_PART
        {
            get { return "geoblokk"; }
        }
        
        public static string RSS_CLIPURL_PREFIX
        {
            get { return "http://pd.nrk.no/nett-tv-stream/stream.ashx?id="; }
        }

        public static string QTICKET
        {
            get { return "qticket"; }
        }

        public static string VIGNETT_ID_NATURE
        {
            get { return "372980"; }
        }

        public static string VIGNETT_ID_SUPER
        {
            get { return "381994"; }
        }

        public static string VIGNETT_ID_NYHETER
        {
            get { return "410330"; }
        }

        public static string VIGNETT_ID_SPORT
        {
            get { return "410335"; }
        }

        public const string LIBRARY_NAME = "NrkParser";

    }
}
