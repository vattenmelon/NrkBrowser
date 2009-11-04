using System;
using System.Collections.Generic;
using System.Text;

namespace NrkBrowser
{
    public class NrkConstants
    {
        public const string PLUGIN_NAME = "NRK Browser";

        public static string MENU_ITEM_ID_MEST_SETTE_UKE
        {
            get { return "mestSettUke"; }
        }

        public static string MENU_ITEM_ID_MEST_SETTE_MAANED
        {
            get { return "mestSettMaaned"; }
        }

        public static string MENU_ITEM_ID_MEST_SETTE_TOTALT
        {
            get { return "mestSettTotalt"; }
        }

        public static string GUI_PROPERTY_PROGRAM_PICTURE
        {
            get { return "#picture"; }
        }

        public static string GUI_PROPERTY_PROGRAM_DESCRIPTION
        {
            get { return "#description"; }
        }

        public static string GUI_PROPERTY_CLIP_COUNT
        {
            get { return "#clipcount"; }
        }

        public static string ABOUT_DESCRIPTION
        {
            get { return "Plugin for watching NRK nett-tv"; }
        }

        public static string ABOUT_AUTHOR
        {
            get { return "Terje Wiesener <wiesener@samfundet.no> / Vattenmelon"; }
        }

        public static string CONFIG_FILE
        {
            get { return "NrkBrowserSettings.xml"; }
        }

        public static string STREAM_PREFIX
        {
            get { return "mms://straumV.nrk.no/nrk_tv_webvid"; }
        }

        public static string CONFIG_SECTION
        {
            get { return "NrkBrowser"; }
        }

        public static string CONFIG_ENTRY_PLUGIN_NAME
        {
            get { return "pluginName"; }
        }

        public static string CONFIG_ENTRY_SPEED
        {
            get { return "speed"; }
        }

        public static string CONFIG_ENTRY_LIVE_STREAM_QUALITY
        {
            get { return "liveStreamQuality"; }
        }

        public static string SKIN_FILENAME
        {
            get { return "NrkBrowser.xml"; }
        }

        public static string MEDIAPORTAL_CONFIG_FILE
        {
            get { return "MediaPortal.xml"; }
        }

        public static string DEFAULT_PICTURE
        {
            get { return "http://fil.nrk.no/contentfile/web/bgimages/special/nettv/bakgrunn_nett_tv.jpg"; }
        }

        public static string RSS_URL
        {
            get { return "http://www1.nrk.no/nett-tv/MediaRss.ashx?loop="; }
        }

        public static string URL_GET_MEDIAXML
        {
            get { return "http://www1.nrk.no/nett-tv/silverlight/getmediaxml.ashx?id={0}&hastighet={1}"; }
        }
        public static string SEARCH_NEXTPAGE_ID
        {
            get { return "nextPage"; }
        }
        public static string CONTEXTMENU_HEADER_TEXT
        {
            get { return "NRK Browser"; }
        }

        public static string MESSAGE_BOX_HEADER_TEXT
        {
            get { return "NRK Browser"; }
        }

        public static string MENU_ITEM_ID_FAVOURITES
        {
            get { return "favoritter"; }
        }

        public static string GEOBLOCK_URL_PART
        {
            get { return "geoblokk"; }
        }
        
        public static string RSS_CLIPURL_PREFIX
        {
            get { return "http://pd.nrk.no/nett-tv-stream/stream.ashx?id="; }
        }

        /// <summary>
        /// The minimum time start-time of the clip should be before we wants to seek.
        /// </summary>
        public static double MINIMUM_TIME_BEFORE_SEEK
        {
            get { return 4; }
        }
    }
}
