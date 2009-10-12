using System;
using System.Collections.Generic;
using System.Text;

namespace NrkBrowser
{
    public class NrkConstants
    {
        private static string quality_MENU_LOW_QUALITY;
        private static string quality_MENU_MEDIUM_QUALITY;
        private static string quality_MENU_HIGH_QUALITY;
        private static string favourites_COULD_NOT_BE_ADDED;
        private static string favourites_UNSUPPORTED_TYPE;
        private static string contextmenu_HEADER_TEXT;
        private static string message_BOX_HEADER_TEXT;
        private static string menu_ITEM_ID_FAVOURITES;

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

        public static string URL_GET_MEDIAXML
        {
            get { return "http://www1.nrk.no/nett-tv/silverlight/getmediaxml.ashx?id={0}&hastighet={1}"; }
        }

        public static string CONTEXTMENU_ITEM_SE_TIDLIGERE_PROGRAMMER
        {
            get { return "Se tidligere programmer"; }
        }

        public static string CONTEXTMENU_ITEM_LEGG_TIL_I_FAVORITTER
        {
            get { return "Legg til i favoritter"; }
        }

        public static string CONTEXTMENU_ITEM_FJERN_FAVORITT
        {
            get { return "Fjern favoritt"; }
        }

        public static string CONTEXTMENU_ITEM_BRUK_VALGT_SOM_SOEKEORD
        {
            get { return "Bruk valgt som søkeord"; }
        }

        public static string CONTEXTMENU_ITEM_KVALITET
        {
            get { return "Kvalitet"; }
        }

        public static string SEARCH_NEXTPAGE_DESCRIPTION
        {
            get { return "Se de neste 25 treffene"; }
        }

        public static string SEARCH_NEXTPAGE_TITLE
        {
            get { return "Neste side"; }
        }

        public static string SEARCH_NEXTPAGE_ID
        {
            get { return "nextPage"; }
        }

        public static string CONTEXTMENU_ITEM_CHECK_FOR_NEW_VERSION
        {
            get { return "Se etter ny versjon"; }
        }

        public static string NEW_VERSION_IS_AVAILABLE
        {
            get { return "Ny versjon er tilgjengelig (ver. {0}), last ned fra mediaportal forumet."; }
        }

        public static string NEW_VERSION_IS_NOT_AVAILABLE
        {
            get { return "Ingen nyere versjon tilgjengelig."; }
        }

        public static string QUALITY_MENU_HEADER
        {
            get { return "Velg kvalitet"; }
        }

        public static string QUALITY_MENU_LOW_QUALITY
        {
            get { return "Lav kvalitet"; }
        }

        public static string QUALITY_MENU_MEDIUM_QUALITY
        {
            get { return "Middels kvalitet"; }
        }

        public static string QUALITY_MENU_HIGH_QUALITY
        {
            get { return "Høy kvalitet"; }
        }

        public static string FAVOURITES_COULD_NOT_BE_ADDED
        {
            get { return "Favoritt kunne ikke bli lagt til: {0}"; }
        }

        public static string FAVOURITES_UNSUPPORTED_TYPE
        {
            get { return "Kun klipp eller program kan legges til som favoritt"; }
        }

        public static string FAVOURITES_COULD_NOT_BE_REMOVED
        {
            get { return "Favoritt kunne ikke fjernes"; }
        }

        public static string CONTEXTMENU_HEADER_TEXT
        {
            get { return "NRK Browser"; }
        }

        public static string MESSAGE_BOX_HEADER_TEXT
        {
            get { return "NRK Browser"; }
        }

        public static string PLAYBACK_FAILED_TRY_DISABLING_VMR9
        {
            get { return "Avspilling feilet, prøv å slå av osd player/vmr 9 for webstreams"; }
        }

        public static string PLAYBACK_FAILED_GENERIC
        {
            get { return "Avspilling feilet"; }
        }

        public static string PLAYBACK_FAILED_GEOBLOCKED_TO_NORWAY
        {
            get { return "Valgt klipp er kun tilgjengelig i Norge. (Chosen clip is only available in Norway)"; }
        }

        public static string PLAYBACK_FAILED_URL_WAS_NULL
        {
            get { return "Avspilling feilet fordi lenke til klipp var tom!"; }
        }

        public static string MENU_ITEM_TITLE_FAVOURITES
        {
            get { return "Favoritter"; }
        }

        public static string MENU_ITEM_ID_FAVOURITES
        {
            get { return "favoritter"; }
        }

        public static string MENU_ITEM_DESCRIPTION_FAVOURITES
        {
            get { return "Se dine favoritter"; }
        }
    }
}
