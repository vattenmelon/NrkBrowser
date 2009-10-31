using System;

namespace NrkBrowser
{
    public class NrkConstants
    {
        public const string PLUGIN_NAME = "NRK Browser";
        private static TranslationService trans;
        
        public static void Init()
        {
            createTranslationService();
            trans.Init();
        }

        /// <summary>
        /// Init method intended for testing purposes
        /// </summary>
        /// <param name="language">The language to load translations for</param>
        /// <param name="path">The path to the directory containing the language files</param>
        public static void InitWithParam(String language, String path)
        {
            createTranslationService();
            trans.InitWithParam(language, path);
        }

        private static void createTranslationService()
        {
            trans = new TranslationService();
        }

        private static string getTranslatedFromKeyOrGetDefault(String key, String defaultString)
        {
            String stringToReturn = String.Empty;
            if (trans.Contains(key))
            {
                stringToReturn = trans.Get(key);
            }
            else
            {
                stringToReturn = defaultString;
            }

            return stringToReturn;
        }

        public static int GetNumberOfTranslatedStrings()
        {
            return trans.GetNumberOfTranslatedStrings();
        }

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
            get{return getTranslatedFromKeyOrGetDefault("MENU_ITEM_TITLE_MEST_SETTE_UKE", "Most watched this week");}
        }

        public static string MENU_ITEM_TITLE_MEST_SETTE_MAANED
        {
            get{return getTranslatedFromKeyOrGetDefault("MENU_ITEM_TITLE_MEST_SETTE_MAANED", "Most watched this month");}
        }

        public static string MENU_ITEM_TITLE_MEST_SETTE_TOTALT
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_TITLE_MEST_SETTE_TOTALT", "All time most popular"); }
        }

        public static string MENU_ITEM_DESCRIPTION_MEST_SETTE_UKE
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_DESCRIPTION_MEST_SETTE_UKE", "Most popular clips this week!"); }
        }

        public static string MENU_ITEM_DESCRIPTION_MEST_SETTE_MAANED
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_DESCRIPTION_MEST_SETTE_MAANED", "Most popular clips this month!"); }
        }

        public static string MENU_ITEM_DESCRIPTION_MEST_SETTE_TOTALT
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_DESCRIPTION_MEST_SETTE_TOTALT", "All time most popular clips!"); }
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

        public static string CONTEXTMENU_ITEM_SE_TIDLIGERE_PROGRAMMER
        {
            get { return getTranslatedFromKeyOrGetDefault("CONTEXTMENU_ITEM_SE_TIDLIGERE_PROGRAMMER", "Watch earlier programs"); }
        }

        public static string CONTEXTMENU_ITEM_LEGG_TIL_I_FAVORITTER
        {
            get { return getTranslatedFromKeyOrGetDefault("CONTEXTMENU_ITEM_LEGG_TIL_I_FAVORITTER", "Add to favourites"); }
        }

        public static string CONTEXTMENU_ITEM_FJERN_FAVORITT
        {
            get { return getTranslatedFromKeyOrGetDefault("CONTEXTMENU_ITEM_FJERN_FAVORITT", "Remove favourite"); }
        }

        public static string CONTEXTMENU_ITEM_BRUK_VALGT_SOM_SOEKEORD
        {
            get { return getTranslatedFromKeyOrGetDefault("CONTEXTMENU_ITEM_BRUK_VALGT_SOM_SOEKEORD", "Search for similar"); }
        }

        public static string CONTEXTMENU_ITEM_KVALITET
        {
            get { return getTranslatedFromKeyOrGetDefault("CONTEXTMENU_ITEM_KVALITET", "Quality"); }
        }

        public static string SEARCH_NEXTPAGE_DESCRIPTION
        {
            get { return getTranslatedFromKeyOrGetDefault("SEARCH_NEXTPAGE_DESCRIPTION", "See next 25 hits"); }
        }

        public static string SEARCH_NEXTPAGE_TITLE
        {
            get { return getTranslatedFromKeyOrGetDefault("SEARCH_NEXTPAGE_TITLE", "Next page"); }
        }

        public static string SEARCH_NEXTPAGE_ID
        {
            get { return "nextPage"; }
        }

        public static string CONTEXTMENU_ITEM_CHECK_FOR_NEW_VERSION
        {
            get { return getTranslatedFromKeyOrGetDefault("CONTEXTMENU_ITEM_CHECK_FOR_NEW_VERSION", "Look for new version"); }
        }

        public static string NEW_VERSION_IS_AVAILABLE
        {
            get { return getTranslatedFromKeyOrGetDefault("NEW_VERSION_IS_AVAILABLE", "New version available (ver. {0}), download from www.team-mediaportal.com."); }
        }

        public static string NEW_VERSION_IS_NOT_AVAILABLE
        {
            get { return getTranslatedFromKeyOrGetDefault("NEW_VERSION_IS_NOT_AVAILABLE", "No newer version available."); }
        }

        public static string QUALITY_MENU_HEADER
        {
            get { return getTranslatedFromKeyOrGetDefault("QUALITY_MENU_HEADER", "Choose quality"); }
        }

        public static string QUALITY_MENU_LOW_QUALITY
        {
            get { return getTranslatedFromKeyOrGetDefault("QUALITY_MENU_LOW_QUALITY", "Low quality"); }
        }

        public static string QUALITY_MENU_MEDIUM_QUALITY
        {
            get { return getTranslatedFromKeyOrGetDefault("QUALITY_MENU_MEDIUM_QUALITY", "Medium quality"); }
        }

        public static string QUALITY_MENU_HIGH_QUALITY
        {
            get { return getTranslatedFromKeyOrGetDefault("QUALITY_MENU_HIGH_QUALITY", "High quality"); }
        }

        public static string FAVOURITES_COULD_NOT_BE_ADDED
        {
            get { return getTranslatedFromKeyOrGetDefault("FAVOURITES_COULD_NOT_BE_ADDED", "Favourite could not be added: {0}"); }
        }

        public static string FAVOURITES_UNSUPPORTED_TYPE
        {
            get { return getTranslatedFromKeyOrGetDefault("FAVOURITES_UNSUPPORTED_TYPE", "Only clips or programs can be added"); }
        }

        public static string FAVOURITES_COULD_NOT_BE_REMOVED
        {
            get { return getTranslatedFromKeyOrGetDefault("FAVOURITES_COULD_NOT_BE_REMOVED", "Favourite could not be removed"); }
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
            get { return getTranslatedFromKeyOrGetDefault("PLAYBACK_FAILED_TRY_DISABLING_VMR9", "Playback failed, try disabling VMR9/Osd player for webstreams"); }
        }

        public static string PLAYBACK_FAILED_GENERIC
        {
            get { return getTranslatedFromKeyOrGetDefault("PLAYBACK_FAILED_GENERIC", "Playback failed"); }
        }

        public static string PLAYBACK_FAILED_GEOBLOCKED_TO_NORWAY
        {
            get { return getTranslatedFromKeyOrGetDefault("PLAYBACK_FAILED_GEOBLOCKED_TO_NORWAY", "Chosen clip is only available in Norway"); }
        }

        public static string PLAYBACK_FAILED_URL_WAS_NULL
        {
            get { return getTranslatedFromKeyOrGetDefault("PLAYBACK_FAILED_URL_WAS_NULL", "Playback failed because link to clip was empty!"); }
        }

        public static string MENU_ITEM_TITLE_FAVOURITES
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_TITLE_FAVOURITES", "Favourites"); }
        }

        public static string MENU_ITEM_ID_FAVOURITES
        {
            get { return "favoritter"; }
        }

        public static string MENU_ITEM_DESCRIPTION_FAVOURITES
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_DESCRIPTION_FAVOURITES", "Watch your favourites"); }
        }

        public static string FOR_UNIT_TESTING
        {
            get { return getTranslatedFromKeyOrGetDefault("FOR_UNIT_TESTING", "Default language"); }
        }

        public static string MENU_ITEM_TITLE_LIVE_STREAMS
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_TITLE_LIVE_STREAMS", "Live streams"); }
        }

        public static string MENU_ITEM_TITLE_ALPHABETICAL_LIST
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_TITLE_ALPHABETICAL_LIST", "Alphabetical list"); }
        }

        public static string MENU_ITEM_TITLE_RECOMMENDED_PROGRAMS
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_TITLE_RECOMMENDED_PROGRAMS", "Recommended clips"); }
        }

        public static string MENU_ITEM_DESCRIPTION_RECOMMENDED_PROGRAMS
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_DESCRIPTION_RECOMMENDED_PROGRAMS", "Currently recommended clips from the frontpage"); }
        }

        public static string MENU_ITEM_TITLE_MOST_WATCHED
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_TITLE_MOST_WATCHED", "Most watched"); }
        }
       
        public static string MENU_ITEM_DESCRIPTION_MOST_WATCHED
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_DESCRIPTION_MOST_WATCHED", "Most popular clips!"); }
        }

        public static string MENU_ITEM_TITLE_NEWS
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_TITLE_NEWS", "News"); }
        }

        public static string MENU_ITEM_DESCRIPTION_NEWS
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_DESCRIPTION_NEWS", "The latest newsclips"); }
        }

        public static string MENU_ITEM_TITLE_SPORT
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_TITLE_SPORT", "Sport"); }
        }

        public static string MENU_ITEM_DESCRIPTION_SPORT
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_DESCRIPTION_SPORT", "The latest sportclips"); }
        }

        public static string MENU_ITEM_TITLE_NATURE
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_TITLE_NATURE", "Nature"); }
        }

        public static string MENU_ITEM_DESCRIPTION_NATURE
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_DESCRIPTION_NATURE", "The latest natureclips"); }
        }

        public static string MENU_ITEM_TITLE_SUPER
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_TITLE_SUPER", "Super"); }
        }

        public static string MENU_ITEM_DESCRIPTION_SUPER
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_DESCRIPTION_SUPER", "The latest clips from super"); }
        }
        public static string MENU_ITEM_TITLE_SEARCH
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_TITLE_SEARCH", "Search"); }
        }

        public static string MENU_ITEM_DESCRIPTION_SEARCH
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_DESCRIPTION_SEARCH", "Search the archive"); }
        }
        public static string MENU_ITEM_TITLE_ALTERNATIVE_LINKS
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_TITLE_ALTERNATIVE_LINKS", "Alternative links"); }
        }

        public static string MENU_ITEM_TITLE_CATEGORIES
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_TITLE_CATEGORIES", "Categories"); }
        }

        public static string DESCRIPTION_CLIP_SHOWN_TIMES
        {
            get { return getTranslatedFromKeyOrGetDefault("DESCRIPTION_CLIP_SHOWN_TIMES", "Clip viewed {0} times"); }
        }

        public static string MENU_ITEM_DESCRIPTION_NEWEST_CLIPS_FROM_GENERIC
        {
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_DESCRIPTION_NEWEST_CLIPS_FROM_GENERIC", "The latest clips from {0}"); }
        }

        public static string DESCRIPTION_CLIP_ADDED
        { 
            get { return getTranslatedFromKeyOrGetDefault("DESCRIPTION_CLIP_ADDED", "Clip added {0}"); }
        }

        public static string MENU_ITEM_TITLE_CHOOSE_STREAM_MANUALLY
        { 
            get { return getTranslatedFromKeyOrGetDefault("MENU_ITEM_TITLE_CHOOSE_STREAM_MANUALLY", "Choose stream manually"); }
        }

        public static string CLIP_COUNT
        { 
            get { return getTranslatedFromKeyOrGetDefault("CLIP_COUNT", "{0} clips"); }
        }
        
        public static string GEOBLOCK_URL_PART
        {
            get { return "geoblokk"; }
        }
    }
}
