/*
 * Copyright (c) 2008 Terje Wiesener <wiesener@samfundet.no>
 * 
 * Loosely based on an anonymous (and slightly outdated) NRK parser in python for Myth-tv, 
 * please email me if you are the author :)
 * 
 * 2008 - 2009 Modified by Vattenmelon
 * 
 * */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using MediaPortal.Configuration;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Playlists;
using MediaPortal.Profile;
using NrkBrowser.Domain;
using MenuItem=NrkBrowser.Domain.MenuItem;
using Stream=NrkBrowser.Domain.Stream;

namespace NrkBrowser
{
    public class NrkPlugin : GUIWindow, ISetupForm
    {
        private static string PICTURE_DIR = string.Format(@"{0}\media\nrkbrowser\", GUIGraphicsContext.Skin);
        
        protected string _suffix = "_l";



        /// <summary>
        /// This is the name of the plugin as it appears in MediaPortal gui. Note: the name specified here will
        /// be shown in the plugins list in the configuration, while in the gui inside mediaportal it will use the name
        /// specified in the configuration, or the default name. 
        /// </summary>
        private string pluginName = NrkConstants.PLUGIN_NAME;

        [SkinControlAttribute(50)] protected GUIFacadeControl facadeView = null;

        protected NrkParser nrkParser = null;
        protected Stack<Item> activeStack = null;
        private List<Item> matchingItems = new List<Item>();
        private MenuItem favoritter; //used so we can se if option to remove favorite should be shown
        private bool _workerCompleted = false;

        protected bool _osdPlayer = true;

        public NrkPlugin()
        {
            GetID = 40918376;
        }

        #region ISetupForm Members

        public string PluginName()
        {
            return pluginName;
        }

        public string Description()
        {
            return NrkConstants.ABOUT_DESCRIPTION;
        }

        public string Author()
        {
            return NrkConstants.ABOUT_AUTHOR;
        }

        public void ShowPlugin()
        {
            String appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
//            Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Log.Debug(NrkConstants.PLUGIN_NAME + ": version: " + appVersion);
//            Log.Debug("major: " + v.Major);
//            Log.Debug("majorrevison: " + v.MajorRevision);
//            Log.Debug("build: " + v.Build);
//            Log.Debug("revision: " + v.Revision);
//            Log.Debug("minor: " + v.Minor);
//            Log.Debug("minorrevision: " + v.MinorRevision);
            //configuration
            SettingsForm form = new SettingsForm();
            form.LabelVersionVerdi.Text = appVersion;

            Settings settings = new Settings(Config.GetFile(Config.Dir.Config, NrkConstants.CONFIG_FILE));
            int speed = initSpeedSettings(form, settings);

            string quality = initLiveStreamQuality(form, settings);

            pluginName = settings.GetValueAsString(NrkConstants.CONFIG_SECTION, NrkConstants.CONFIG_ENTRY_PLUGIN_NAME, "My Nrk");
            form.NameTextbox.Text = pluginName;

            DialogResult res = form.ShowDialog();
            if (res == DialogResult.OK)
            {
                SaveSettings(form, settings);
            }
        }

        private void SaveSettings(SettingsForm form, Settings settings)
        {
            Log.Debug(NrkConstants.PLUGIN_NAME + ": SaveSettings");
            int speed = (int) form.speedUpDown.Value;
            settings.SetValue(NrkConstants.CONFIG_SECTION, NrkConstants.CONFIG_ENTRY_SPEED, speed);
            string quality = (string) form.liveStreamQualityCombo.Items[form.liveStreamQualityCombo.SelectedIndex];
            settings.SetValue(NrkConstants.CONFIG_SECTION, NrkConstants.CONFIG_ENTRY_LIVE_STREAM_QUALITY, quality);
            pluginName = form.NameTextbox.Text;
            settings.SetValue(NrkConstants.CONFIG_SECTION, NrkConstants.CONFIG_ENTRY_PLUGIN_NAME, pluginName);
        }

        private static string initLiveStreamQuality(SettingsForm form, Settings settings)
        {
            string quality = settings.GetValueAsString(NrkConstants.CONFIG_SECTION, NrkConstants.CONFIG_ENTRY_LIVE_STREAM_QUALITY, "Low");
            form.liveStreamQualityCombo.SelectedIndex = form.liveStreamQualityCombo.Items.IndexOf(quality);
            return quality;
        }

        private static int initSpeedSettings(SettingsForm form, Settings settings)
        {
            int speed = settings.GetValueAsInt(NrkConstants.CONFIG_SECTION, NrkConstants.CONFIG_ENTRY_SPEED, 2048);
            if (speed < form.speedUpDown.Minimum)
            {
                speed = (int) form.speedUpDown.Minimum;
                settings.SetValue(NrkConstants.CONFIG_SECTION, NrkConstants.CONFIG_ENTRY_SPEED, speed);
            }
            if (speed > form.speedUpDown.Maximum)
            {
                speed = (int) form.speedUpDown.Maximum;
                settings.SetValue(NrkConstants.CONFIG_SECTION, NrkConstants.CONFIG_ENTRY_SPEED, speed);
            }
            form.speedUpDown.Value = speed;
            return speed;
        }

        public bool CanEnable()
        {
            return true;
        }

        public int GetWindowId()
        {
            return GetID;
            //return 40918376;
        }

        public bool DefaultEnabled()
        {
            return true;
        }

        public bool HasSetup()
        {
            return true;
        }

        /// <summary>
        /// If the plugin should have it's own button on the main menu of Mediaportal then it
        /// should return true to this method, otherwise if it should not be on home
        /// it should return false
        /// </summary>
        /// <param name="strButtonText">text the button should have</param>
        /// <param name="strButtonImage">image for the button, or empty for default</param>
        /// <param name="strButtonImageFocus">image for the button, or empty for default</param>
        /// <param name="strPictureImage">subpicture for the button or empty for none</param>
        /// <returns>true : plugin needs it's own button on home
        /// false : plugin does not need it's own button on home</returns>
        public bool GetHome(out string strButtonText, out string strButtonImage,
                            out string strButtonImageFocus, out string strPictureImage)
        {
            strButtonText = PluginName();
            strButtonImage = String.Empty;
            strButtonImageFocus = String.Empty;
            strPictureImage = GUIGraphicsContext.Skin + @"\media\hover_my tv.png";
            return true;
        }

        #endregion

        public override bool Init()
        {
            Log.Debug(string.Format("{0}: Init()", NrkConstants.PLUGIN_NAME));
            using (Settings xmlreader = new Settings(Config.GetFile(Config.Dir.Config, NrkConstants.MEDIAPORTAL_CONFIG_FILE)))
            {
                _osdPlayer = xmlreader.GetValueAsBool("general", "usevrm9forwebstreams", true);
            }

            bool result = Load(string.Format(@"{0}\{1}", GUIGraphicsContext.Skin, NrkConstants.SKIN_FILENAME));
            Settings settings = new Settings(Config.GetFile(Config.Dir.Config, NrkConstants.CONFIG_FILE));
            int speed = settings.GetValueAsInt(NrkConstants.CONFIG_SECTION, NrkConstants.CONFIG_ENTRY_SPEED, 2048);
            activeStack = new Stack<Item>();
            String quality = settings.GetValueAsString(NrkConstants.CONFIG_SECTION, NrkConstants.CONFIG_ENTRY_LIVE_STREAM_QUALITY, "Low");
            pluginName = settings.GetValueAsString(NrkConstants.CONFIG_SECTION, NrkConstants.CONFIG_ENTRY_PLUGIN_NAME, "My Nrk");

            Dictionary<String, String> qualityMap = new Dictionary<string, string>();
            qualityMap["Low"] = "_l";
            qualityMap["Medium"] = "_m";
            qualityMap["High"] = "_h";

            _suffix = qualityMap[quality];

            return result;
        }

        protected override void OnPageLoad()
        {
            if (activeStack.Count == 0)
            {
                Activate(null);
            }
            else
            {
                //for å forhindre at keyboardet kommer opp når man går tilbake fra videovisning til søkresultatsida.
                if (activeStack.Peek().ID.Equals("sok"))
                {
                    UpdateList(matchingItems);
                    return;
                }
                Item lAct = activeStack.Pop();
                Activate(activeStack.Pop());
                setItemAsSelectedItemInListView(lAct);
            }
        }

        protected override void OnPreviousWindow()
        {
            
           if (erIHovedMeny())
            {
                base.OnPreviousWindow();
                return;
            }
            
            Item lAct = activeStack.Pop(); //we do not want to show the active item again

            if (harItemsIMenyStacken())
            {
                //for å forhindre at keyboardet kommer opp når man "browser bakover" til søkeresultatsida
                if (activeStack.Peek().ID.Equals("sok"))
                {
                    UpdateList(matchingItems);
                    return;
                }
                Activate(activeStack.Pop());
            }
            else
            {
                Activate(null);
            }
            setItemAsSelectedItemInListView(lAct);
        }

        private bool harItemsIMenyStacken()
        {
            return activeStack.Count > 0;
        }

        private bool erIHovedMeny()
        {
            return activeStack.Count <= 0;
        }

        private void setItemAsSelectedItemInListView(Item item)
        {
            int indexCounter = 0;
            foreach (GUIListItem guiListItem in this.facadeView.ListView.ListItems)
            {
                if (guiListItem.Label == item.Title)
                {
                    this.facadeView.SelectedListItemIndex = indexCounter; 
                    break;
                }
                indexCounter++;
            }
        }

        public override bool OnMessage(GUIMessage message)
        {
            Log.Debug(string.Format("{0}: onMessage()", NrkConstants.PLUGIN_NAME));
            switch (message.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
                    {
                        Log.Debug(string.Format("{0}: onMessage() with MessageType: [{1}]", NrkConstants.PLUGIN_NAME, "GUI_MSG_WINDOW_INIT"));
//                        base.OnMessage(message); //code here runs every time the window is displayed 
                        GUIPropertyManager.SetProperty("#currentmodule", NrkConstants.PLUGIN_NAME);
                        
                        if (nrkParser == null)
                        {
                            using (WaitCursor cursor = new WaitCursor())
                            {
                                Log.Info("Parser is null, getting speed-cookie and creates parser.");
                                Settings settings = new Settings(Config.GetFile(Config.Dir.Config, NrkConstants.CONFIG_FILE));
                                int speed = settings.GetValueAsInt("NrkBrowser", "speed", 2048);
                                nrkParser = new NrkParser(speed);
                            }
                        }
                        break;
                    }
            }
            return base.OnMessage(message);
        }

        public override void Process()
        {
            
            if (facadeView.SelectedListItem == null)
            {
                base.Process();
                return;
            }
            Item selecteditem = (Item) facadeView.SelectedListItem.TVTag;

            GUIPropertyManager.SetProperty(NrkConstants.GUI_PROPERTY_PROGRAM_DESCRIPTION, selecteditem.Description);

            if (selecteditem.Description == null || selecteditem.Description.Equals(""))
            {
                GUIPropertyManager.SetProperty(NrkConstants.GUI_PROPERTY_PROGRAM_DESCRIPTION, " ");
            }

            if (selecteditem.ID == "all" || selecteditem.ID == "categories" || selecteditem.ID == "anbefalte" ||
                selecteditem.ID == "live" || selecteditem.ID == "mestSett" || selecteditem.ID == "sok" || selecteditem.ID == "liveAlternate" ||
                selecteditem is Category)
            {
                GUIPropertyManager.SetProperty(NrkConstants.GUI_PROPERTY_PROGRAM_PICTURE, NrkConstants.DEFAULT_PICTURE);
            }

            if (!selecteditem.Bilde.Equals("") || erFavoritt())
            {
                GUIPropertyManager.SetProperty(NrkConstants.GUI_PROPERTY_PROGRAM_PICTURE, selecteditem.Bilde);
            }

            base.Process();
        }

        private bool erFavoritt()
        {
            if (activeStack.Count == 0)
            {
                return false;
            }
            else
            {
                return activeStack.Peek() == favoritter;
            }
        }

        protected override void OnClicked(int controlId, GUIControl control,
                                          Action.ActionType actionType)
        {
            if (control == facadeView) ItemSelected();
            base.OnClicked(controlId, control, actionType);
        }

        protected void UpdateList(List<Item> newitems)
        {
            this.facadeView.Clear();
            foreach (Item item in newitems)
            {
                GUIListItem listitem = new GUIListItem(item.Title);

                listitem.TVTag = item;
                if (item.Playable)
                {
                    listitem.IconImage = GUIGraphicsContext.Skin + @"\media\defaultVideo.png";
                    listitem.IconImageBig = GUIGraphicsContext.Skin + @"\media\defaultVideo.png";
                    
                    
                }
                else
                {
                    listitem.IconImage = GUIGraphicsContext.Skin + @"\media\defaultFolder.png";
                    listitem.IconImageBig = GUIGraphicsContext.Skin + @"\media\DefaultFolderBig.png";
                }
                this.facadeView.Add(listitem);
            }
        }

        protected void ItemSelected()
        {
            Item selecteditem = (Item) facadeView.SelectedListItem.TVTag;
            Activate(selecteditem);
        }

        private bool GetUserInputString(ref string sString)
        {
            VirtualKeyboard keyBoard =
                (VirtualKeyboard) GUIWindowManager.GetWindow((int) Window.WINDOW_VIRTUAL_KEYBOARD);
            keyBoard.Reset();
            keyBoard.IsSearchKeyboard = true;
            keyBoard.Text = sString;
            keyBoard.DoModal(GetID); // show it...
            if (keyBoard.IsConfirmed) sString = keyBoard.Text;
            return keyBoard.IsConfirmed;
        }

        public void search(SearchItem item)
        {
            matchingItems = nrkParser.GetSearchHits(item.Keyword, item.IndexPage + 1);
            createViewNextPageSearchItemIfNecessary(item.Keyword, item.IndexPage + 1);
            UpdateList(matchingItems);
        }

        private void createViewNextPageSearchItemIfNecessary(String keyword, int indexPage)
        {
            if (matchingItems.Count == 25)
            {
                createNextPageSearchItem(keyword, indexPage);
            }
        }

        public void search()
        {
            String keyword = String.Empty;
            GetUserInputString(ref keyword);
            keyword = keyword.ToLower();
            matchingItems.Clear();
            search(keyword);
        }

        private void search(String keyword)
        {
            matchingItems = nrkParser.GetSearchHits(keyword, 0);
            createViewNextPageSearchItemIfNecessary(keyword, 0);
            UpdateList(matchingItems);
        }

        private void createNextPageSearchItem(string keyword, int indexPage)
        {
            SearchItem nextPage = new SearchItem(NrkConstants.SEARCH_NEXTPAGE_ID, NrkConstants.SEARCH_NEXTPAGE_TITLE, indexPage);
            nextPage.Description = NrkConstants.SEARCH_NEXTPAGE_DESCRIPTION;
            nextPage.Keyword = keyword;
            matchingItems.Add(nextPage);
        }

        protected void Activate(Item item)
        {
            GUIPropertyManager.SetProperty(NrkConstants.GUI_PROPERTY_CLIP_COUNT, " ");
        
            if (item == null)
            {
                UpdateList(CreateInitialMenuItems());
                return;
            }

            activeStack.Push(item);

            if (item is Clip)
            {
                PlayClip((Clip) item);
                return;
            }

            if (item is Stream)
            {
                PlayStream((Stream) item);
                return;
            }

            if (item.ID == "favoritter")
            {
                FillStackWithFavourites();
                return;
            }
            if (item.ID == "sok")
            {
                search();
                return;
            }
            if (item.ID == "nextPage")
            {
                search((SearchItem) item);
                return;
            }

            if (item.ID == "anbefalte")
            {
                UpdateList(nrkParser.GetAnbefaltePaaForsiden());
                setClipCount(nrkParser.GetAnbefaltePaaForsiden());
                return;
            }

            if (item.ID == "mestSett")
            {
                UpdateList(CreateMestSetteListItems());
                return;
            }

            if (item.ID == "mestSettUke")
            {
                UpdateListAndSetClipCount(nrkParser.GetMestSette(7));
                return;
            }

            if (item.ID == "mestSettMaaned")
            {
                UpdateListAndSetClipCount(nrkParser.GetMestSette(31));
                return;
            }

            if (item.ID == "mestSettTotalt")
            {
                UpdateListAndSetClipCount(nrkParser.GetMestSette(3650));
                return;
            }

            if (item.ID == "nyheter" || item.ID == "sport" || item.ID == "natur" || item.ID == "super")
            {
                UpdateListAndSetClipCount(nrkParser.GetTopTabRSS(item.ID));
                return;
            }

            if (item.ID == "categories")
            {
                UpdateList(nrkParser.GetCategories());
                return;
            }

            if (item.ID == "all")
            {
                UpdateList(nrkParser.GetAllPrograms());
                return;
            }

            if (item.ID == "live")
            {
                UpdateList(CreateLiveMenuItems());
                return;
            }
            if (item.ID == "liveAlternate")
            {
                UpdateList(CreateLiveAlternateMenuItems());
                return;
            }

            if (item.ID == "liveall")
            {
                UpdateList(CreateLiveAlternateAllMenuItems());
                return;
            }
            if (item is AutogeneratedMenuItem)
            {
                UpdateList(nrkParser.GetTopTabContent(item.ID));
            }
            if (item is Category)
            {
                UpdateList(nrkParser.GetPrograms((Category) item));
            }
            if (item is Program)
            {
                List<Item> items = nrkParser.GetClips((Program) item);
                setClipCount(items);
                items.AddRange(nrkParser.GetFolders((Program) item));
                UpdateList(items);
            }
            if (item is Folder)
            {
                List<Item> items = nrkParser.GetClips((Folder) item);
                setClipCount(items);
                items.AddRange(nrkParser.GetFolders((Folder) item));
                UpdateList(items);
            }
        }

        private List<Item> CreateInitialMenuItems()
        {
            List<Item> items = new List<Item>();
            items.Add(new MenuItem("all", "Alfabetisk liste"));
            items.Add(new MenuItem("categories", "Kategorier"));
            items.Add(new MenuItem("live", "Direktestrømmer"));
            MenuItem anbefalte = new MenuItem("anbefalte", "Anbefalte programmer");
            anbefalte.Description = "Anbefalte programmer fra forsiden akkurat nå";
            items.Add(anbefalte);

            MenuItem mestSett = new MenuItem("mestSett", "Mest sett");
            mestSett.Description = "De mest populære klippene!";
            items.Add(mestSett);

            MenuItem nyheter = new MenuItem("nyheter", "Nyheter");
            nyheter.Description = "De siste nyhetsklippene";
            nyheter.Bilde = PICTURE_DIR + "nrknyheter.jpg";
            items.Add(nyheter);

            MenuItem sport = new MenuItem("sport", "Sport");
            sport.Description = "De siste sportsklippene";
            sport.Bilde = PICTURE_DIR + "nrksport.jpg";
            items.Add(sport);

            MenuItem natur = new MenuItem("natur", "Natur");
            natur.Description = "De siste naturklippene";
            natur.Bilde = PICTURE_DIR + "nrknatur.jpg";
            items.Add(natur);

            MenuItem super = new MenuItem("super", "Super");
            super.Description = "De siste klippene fra super";
            super.Bilde = PICTURE_DIR + "nrksuper.jpg";
            items.Add(super);
         
            List<Item> tabItems = GetTabItems();
            items.AddRange(tabItems);

            MenuItem sok = new MenuItem("sok", "Søk");
            sok.Description = "Søk i arkivet";
            items.Add(sok);

            favoritter = new MenuItem(NrkConstants.MENU_ITEM_ID_FAVOURITES, NrkConstants.MENU_ITEM_TITLE_FAVOURITES);
            favoritter.Description = NrkConstants.MENU_ITEM_DESCRIPTION_FAVOURITES;
            items.Add(favoritter);
            return items;
        }

        private List<Item> CreateLiveMenuItems()
        {
            List<Item> items = nrkParser.GetDirektePage("direkte");
            items.Add(new MenuItem("liveAlternate", "Alternative linker"));
            return items;
        }

        private List<Item> CreateLiveAlternateAllMenuItems()
        {
            List<Item> items = new List<Item>();
            for (int i = 0; i < 10; i++)
            {
                items.Add(new Stream(NrkConstants.STREAM_PREFIX + i.ToString("D2") + _suffix, "Strøm " + i));
            }
            return items;
        }

        private List<Item> CreateLiveAlternateMenuItems()
        {
            List<Item> items = new List<Item>();
            items.Add(new Stream(NrkConstants.STREAM_PREFIX + "03" + _suffix, "NRK 1"));
            items.Add(new Stream(NrkConstants.STREAM_PREFIX + "04" + _suffix, "NRK 2"));
            items.Add(new Stream(NrkConstants.STREAM_PREFIX + "05" + _suffix, "NRK Alltid Nyheter"));
            items.Add(new Stream(NrkConstants.STREAM_PREFIX + "08" + _suffix, "Testkanal (innhold varierer)"));
            items.Add(new MenuItem("liveall", "Velg strøm manuelt"));
            return items;
        }

        public List<Item> GetTabItems()
        {
           List<Item> menuItems = nrkParser.GetTopTabber();
            menuItems.RemoveAll(delegate(Item item)
                                    {
                                        return
                                            item.ID == "direkte" || item.ID == "nyheter" ||
                                            item.ID == "sport" || item.ID == "distrikt" ||
                                            item.ID == "natur" || item.ID == "super";

                                    });
           return menuItems;
        }
       
        public static List<Item> CreateMestSetteListItems()
        {
            List<Item> items = new List<Item>(3);
            MenuItem mestSettUke = new MenuItem(NrkConstants.MENU_ITEM_ID_MEST_SETTE_UKE, NrkConstants.MENU_ITEM_TITLE_MEST_SETTE_UKE);
            mestSettUke.Description = NrkConstants.MENU_ITEM_DESCRIPTION_MEST_SETTE_UKE;
            items.Add(mestSettUke);

            MenuItem mestSettMaaned = new MenuItem(NrkConstants.MENU_ITEM_ID_MEST_SETTE_MAANED, NrkConstants.MENU_ITEM_TITLE_MEST_SETTE_MAANED);
            mestSettMaaned.Description = NrkConstants.MENU_ITEM_DESCRIPTION_MEST_SETTE_MAANED;
            items.Add(mestSettMaaned);

            MenuItem mestSettTotalt = new MenuItem(NrkConstants.MENU_ITEM_ID_MEST_SETTE_TOTALT, NrkConstants.MENU_ITEM_TITLE_MEST_SETTE_TOTALT);
            mestSettTotalt.Description = NrkConstants.MENU_ITEM_DESCRIPTION_MEST_SETTE_TOTALT;
            items.Add(mestSettTotalt);
            return items;
        }

        private void UpdateListAndSetClipCount(List<Item> items)
        {
            UpdateList(items);
            setClipCount(items);
        }

        private static void setClipCount(List<Item> items)
        {
            if (items != null)
            {
                if (items.Count > 0)
                {
                    GUIPropertyManager.SetProperty(NrkConstants.GUI_PROPERTY_CLIP_COUNT, String.Format("{0} Klipp", items.Count));
                }
            }
        }

        /// <summary>
        /// Method that fills the menu with favourites
        /// </summary>
        private void FillStackWithFavourites()
        {
            Log.Debug(NrkConstants.PLUGIN_NAME + "FillStackWithFavourites()");
            List<Item> items = FavoritesUtil.getDatabase().getFavoriteVideos();
            UpdateList(items);
        }

        protected void PlayClip(Clip item)
        {
            Log.Info(string.Format("{0} PlayClip {1}", NrkConstants.PLUGIN_NAME, item));
            string url = nrkParser.GetClipUrl(item);
            if (url == null)
            {
                ShowMessageBox(NrkConstants.PLAYBACK_FAILED_URL_WAS_NULL);
                return;
            }
            Log.Info(NrkConstants.PLUGIN_NAME + " PlayClip, url is: " + url);
            PlayUrl(url, item.Title, item.StartTime);
            
        }

        protected void PlayStream(Stream item)
        {
            Log.Info(NrkConstants.PLUGIN_NAME + " PlayStream " + item.ID);
            PlayUrl(item.ID, item.Title, 0);
        }

        /// <summary>
        /// Plays the URL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="startTime"></param>
        protected void PlayUrl(string url, string title, double startTime)
        {
            GUIWaitCursor.Init();
            GUIWaitCursor.Show();
            _workerCompleted = false;
            BackgroundWorker worker = new BackgroundWorker();
            PlayArgs pArgs = new PlayArgs();
            pArgs.url = url;
            pArgs.title = title;
            pArgs.startTime = startTime;
            worker.DoWork += new DoWorkEventHandler(PlayMethodDelegate);
            worker.RunWorkerAsync(pArgs);
            //worker.IsBusy prøv å bruke denne istedetfor _workercompleted
            while (_workerCompleted == false) GUIWindowManager.Process();
            GUIWaitCursor.Hide();

            
        
        }
        private class PlayArgs
        {
            public string url;
            public string title;
            public double startTime;
        }
        private void PlayMethodDelegate(object sender, DoWorkEventArgs e)
        {
            Log.Info(string.Format("{0}: PlayMethodDelegate", NrkConstants.PLUGIN_NAME));
            PlayArgs pArgs = (PlayArgs) e.Argument;
            
            PlayListType type;
            if (isWMVVideo(pArgs))
            {
                type = PlayListType.PLAYLIST_VIDEO_TEMP;
            }
            else if (isWMAorMP3Audio(pArgs))
            {
                type = PlayListType.PLAYLIST_RADIO_STREAMS;
            }
            else if (isLiveStream(pArgs)) type = PlayListType.PLAYLIST_VIDEO_TEMP; //det er en av live streamene
            else
            {
                Log.Info(string.Format("Couldn't determine playlist type for: {0}", pArgs.url));
                type = PlayListType.PLAYLIST_VIDEO_TEMP;
            }
            bool playOk = false;

            if (_osdPlayer)
            {
                playOk = playWithOsd(pArgs.url, pArgs.title, type, pArgs.startTime);
            }
            else
            {
                playOk = playWithoutOsd(pArgs.url, pArgs.title, type, pArgs.startTime);
            }

            if (!playOk)
            {
                ifPlaybackFailed(pArgs);
            }
            else
            {
                ifPlayBackSucceded(type);
            }
            _workerCompleted = true;
        }

        private static void ifPlayBackSucceded(PlayListType type)
        {
            if (type == PlayListType.PLAYLIST_VIDEO_TEMP)
            {
                Log.Info(NrkConstants.PLUGIN_NAME + " Playing OK, switching to fullscreen");
                g_Player.ShowFullScreenWindow();
                g_Player.FullScreen = true;
            }
        }

        private void ifPlaybackFailed(PlayArgs pArgs)
        {
            string message = String.Empty;
            if (_osdPlayer)
            {
                message = NrkConstants.PLAYBACK_FAILED_TRY_DISABLING_VMR9;
            }
            else
            {
                message = NrkConstants.PLAYBACK_FAILED_GENERIC;
            }
            if (pArgs.url.Contains("geoblokk"))
            {
                message = NrkConstants.PLAYBACK_FAILED_GENERIC;
                message += NrkConstants.PLAYBACK_FAILED_GEOBLOCKED_TO_NORWAY;
            }
            ShowMessageBox(message);
            Log.Info(NrkConstants.PLUGIN_NAME + " Playing failed");
        }

        private static bool isLiveStream(PlayArgs pArgs)
        {
            return pArgs.url.ToLower().EndsWith("_h");
        }

        private static bool isWMVVideo(PlayArgs pArgs)
        {
            return pArgs.url.ToLower().EndsWith(".wmv");
        }

        private static bool isWMAorMP3Audio(PlayArgs pArgs)
        {
            return pArgs.url.ToLower().EndsWith(".wma") || pArgs.url.ToLower().EndsWith(".mp3");
        }

        /// <summary>
        /// Plays the url with the g_Player. It gives OSD
        /// </summary>
        /// <param name="url">The url of the video or audio file to play</param>
        /// <param name="title"></param>
        /// <param name="type">The Type of the clip to play, audio or video</param>
        /// <param name="startTime">From where in the clip we will start play</param>
        /// <returns></returns>
        private bool playWithOsd(String url, String title, PlayListType type, double startTime)
        {
            //g_Player.Init();
            Log.Info(NrkConstants.PLUGIN_NAME + " Trying to play with Osd (g_Player): " + url);
            Log.Debug("Title is: " + title);
            Log.Debug("Type of clip is: " + type);
            Log.Debug("Starttime of clip is: " + startTime);
            bool playOk;
            if (type == PlayListType.PLAYLIST_VIDEO_TEMP)
            {
                playOk = g_Player.PlayVideoStream(url, title);
                if (startTime != 0)
                {
                    g_Player.SeekAbsolute(startTime);
                }
            }
            else
            {
                playOk = g_Player.PlayAudioStream(url, false);
                if (startTime != 0)
                {
                    g_Player.SeekAbsolute(startTime);
                }
            }
            return playOk;
        }

        /// <summary>
        /// Plays the url without osd. It gives no OSD but may be more reliable.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="pType"></param>
        /// <param name="startTime"></param>
        /// <returns>True if playing of clip is successfully, false if not</returns>
        private bool playWithoutOsd(String url, String title, PlayListType pType, double startTime)
        {
            Log.Info(NrkConstants.PLUGIN_NAME + " Trying to play without Osd (PlayListPlayer): " + url);
            Log.Debug("Title is: " + title);
            Log.Debug("Type of clip is: " + pType);
            Log.Debug("Starttime of clip is: " + startTime);
            bool playIsOk = g_Player.Play(url);

            if (playIsOk && startTime != 0)
            {
                g_Player.SeekAbsolute(startTime);
            }
            return playIsOk;
        }

        /// <summary>
        /// Shows an messagebox with the given message.
        /// </summary>
        /// <param name="message">Message to show</param>
        private void ShowMessageBox(string message)
        {
            //  Log.Info(NrkConstants.PLUGIN_NAME + "Showing error: " + message);
            GUIDialogNotify dlg =
                (GUIDialogNotify) GUIWindowManager.GetWindow((int) Window.WINDOW_DIALOG_NOTIFY);
            dlg.SetHeading(NrkConstants.MESSAGE_BOX_HEADER_TEXT);
            dlg.SetText(message);
            dlg.DoModal(GUIWindowManager.ActiveWindow);
        }

        protected override void OnShowContextMenu()
        {
            Item item = (Item) facadeView.SelectedListItem.TVTag;

            GUIDialogMenu dlgMenu = (GUIDialogMenu) GUIWindowManager.GetWindow((int) Window.WINDOW_DIALOG_MENU);
            if (dlgMenu != null)
            {
                dlgMenu.Reset();
                dlgMenu.SetHeading(NrkConstants.CONTEXTMENU_HEADER_TEXT);
                if (item is Clip)
                {
                    Clip cl = (Clip) item;
                    if (cl.TilhoerendeProsjekt > 0)
                    {
                        dlgMenu.Add(NrkConstants.CONTEXTMENU_ITEM_SE_TIDLIGERE_PROGRAMMER);
                    }
                }
                if (!activeStack.Contains(favoritter))
                {
                    dlgMenu.Add(NrkConstants.CONTEXTMENU_ITEM_LEGG_TIL_I_FAVORITTER);
                }
                if (activeStack.Contains(favoritter))
                {
                    dlgMenu.Add(NrkConstants.CONTEXTMENU_ITEM_FJERN_FAVORITT);
                }
                dlgMenu.Add(NrkConstants.CONTEXTMENU_ITEM_BRUK_VALGT_SOM_SOEKEORD);
                dlgMenu.Add(NrkConstants.CONTEXTMENU_ITEM_KVALITET);
                dlgMenu.Add(NrkConstants.CONTEXTMENU_ITEM_CHECK_FOR_NEW_VERSION);
                dlgMenu.DoModal(GetWindowId());

                if (dlgMenu.SelectedLabel == -1) // Nothing was selected
                {
                    return;
                }

                if (dlgMenu.SelectedLabelText == NrkConstants.CONTEXTMENU_ITEM_SE_TIDLIGERE_PROGRAMMER)
                {
                    Clip cl = (Clip) item;
                    List<Item> items = nrkParser.GetClipsTilhoerendeSammeProgram(cl);
                    items.AddRange(nrkParser.GetFolders(cl));
                    activeStack.Push(item);
                    UpdateList(items);
                   
                }
                if (dlgMenu.SelectedLabelText == NrkConstants.CONTEXTMENU_ITEM_LEGG_TIL_I_FAVORITTER)
                {
                    addToFavourites(item);
                }
                else if (dlgMenu.SelectedLabelText == NrkConstants.CONTEXTMENU_ITEM_BRUK_VALGT_SOM_SOEKEORD)
                {
                    search(item.Title);
                }
                else if (dlgMenu.SelectedLabelText == NrkConstants.CONTEXTMENU_ITEM_FJERN_FAVORITT)
                {
                    removeFavourite(item);
                }
                else if (dlgMenu.SelectedLabelText == NrkConstants.CONTEXTMENU_ITEM_KVALITET)
                {
                    openQualityMenu(dlgMenu);
                }
                else if (dlgMenu.SelectedLabelText == NrkConstants.CONTEXTMENU_ITEM_CHECK_FOR_NEW_VERSION)
                {
                    String nyVer = String.Empty;
                    if (VersionChecker.newVersionAvailable(ref nyVer))
                    {
                        ShowMessageBox(string.Format(NrkConstants.NEW_VERSION_IS_AVAILABLE, nyVer));
                    }
                    else
                    {
                        ShowMessageBox(NrkConstants.NEW_VERSION_IS_NOT_AVAILABLE);
                    }
                }
               
            }
        }

        protected void openQualityMenu(GUIDialogMenu dlgMenu)
        {
                dlgMenu.Reset();
                dlgMenu.SetHeading(NrkConstants.QUALITY_MENU_HEADER);
                GUIListItem lowQuality = new GUIListItem(NrkConstants.QUALITY_MENU_LOW_QUALITY);
                lowQuality.ItemId = 1;
                GUIListItem mediumQuality = new GUIListItem(NrkConstants.QUALITY_MENU_MEDIUM_QUALITY);
                mediumQuality.ItemId = 2;
                GUIListItem highQuality = new GUIListItem(NrkConstants.QUALITY_MENU_HIGH_QUALITY);
                highQuality.ItemId = 3;
                dlgMenu.Add(lowQuality);
                dlgMenu.Add(mediumQuality);
                dlgMenu.Add(highQuality);
                dlgMenu.DoModal(GetWindowId());
                if (dlgMenu.SelectedId == lowQuality.ItemId)
                {
                    int speed = 400;
                    Log.Info(string.Format("{0}: Changing bitrate to {1}", NrkConstants.PLUGIN_NAME, speed));
                    nrkParser = new NrkParser(speed);
                }
                else if (dlgMenu.SelectedId == mediumQuality.ItemId)
                {
                    int speed = 1000;
                    Log.Info(string.Format("{0}: Changing bitrate to {1}", NrkConstants.PLUGIN_NAME, speed));
                    nrkParser = new NrkParser(speed);
                }
                else if (dlgMenu.SelectedId == highQuality.ItemId)
                {
                    int speed = 10000;
                    Log.Info(string.Format("{0}: Changing bitrate to {1}", NrkConstants.PLUGIN_NAME, speed));
                    nrkParser = new NrkParser(speed);
                }
        }

        /// <summary>
        /// Adds an item to favourites
        /// </summary>
        /// <param name="item"></param>
        private void addToFavourites(Item item)
        {
            Log.Debug(NrkConstants.PLUGIN_NAME + ": addToFavourites: " + item);
            FavoritesUtil db = FavoritesUtil.getDatabase(null);
            if (item is Clip)
            {
                Clip c = (Clip) item;
                String message = "";
                if (!db.addFavoriteVideo(c, ref message))
                {
                    ShowMessageBox(string.Format(NrkConstants.FAVOURITES_COULD_NOT_BE_ADDED, message));
                }
            }
            else if (item is Program)
            {
                Program p = (Program) item;
                String message = "";
                if (!db.addFavoriteProgram(p, ref message))
                {
                    ShowMessageBox(string.Format(NrkConstants.FAVOURITES_COULD_NOT_BE_ADDED, message));
                }
            }
            else
            {
                ShowMessageBox(NrkConstants.FAVOURITES_UNSUPPORTED_TYPE);
            }
        }

        /// <summary>
        /// Removes an item from favourites
        /// </summary>
        /// <param name="item"></param>
        private void removeFavourite(Item item)
        {
            Log.Debug(NrkConstants.PLUGIN_NAME + ": removeFavourite: " + item);
            FavoritesUtil db = FavoritesUtil.getDatabase(null);
            bool removedSuccessFully = false;
            if (item is Clip)
            {
                removedSuccessFully = db.removeFavoriteVideo((Clip) item);
            }
            else if (item is Program)
            {
                removedSuccessFully = db.removeFavoriteProgram((Program)item);
            }
            else
            {
                Log.Error("Item to remove from favourite is neither Clip nor Program, this should never happen.");
            }

            if (!removedSuccessFully)
            {
                ShowMessageBox(NrkConstants.FAVOURITES_COULD_NOT_BE_REMOVED);
            }
            else
            {
                //must remove removed item from list..do so by refreshing the whole favouriteslist
                FillStackWithFavourites();
            }
        }
    }
}