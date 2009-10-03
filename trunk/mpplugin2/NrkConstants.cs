using System;
using System.Collections.Generic;
using System.Text;

namespace NrkBrowser
{
    public class NrkConstants
    {
        
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

        public static string MENU_ITEM_TITLE_MEST_SETTE_UKE
        {
            get { return "Mest sett denne uken"; }
        }

        public static string MENU_ITEM_TITLE_MEST_SETTE_MAANED
        {
            get { return "Mest sett denne måneden"; }
        }

        public static string MENU_ITEM_TITLE_MEST_SETTE_TOTALT
        {
            get { return "Mest sett totalt"; }
        }

        public static string MENU_ITEM_DESCRIPTION_MEST_SETTE_UKE
        {
            get { return "De mest populære klippene denne uken!"; }
        }

        public static string MENU_ITEM_DESCRIPTION_MEST_SETTE_MAANED
        {
            get { return "De mest populære klippene denne måneden!"; }
        }

        public static string MENU_ITEM_DESCRIPTION_MEST_SETTE_TOTALT
        {
            get { return "De mest populære klippene!"; }
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

        public static string PLUGIN_NAME
        {
            get { return "NrkBrowser"; }
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
    }
}
