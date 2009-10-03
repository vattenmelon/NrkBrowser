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

        protected NrkParser _nrk = null;
        protected Stack<Item> _active = null;
        private List<Item> matchingItems = new List<Item>();
        private MenuItem favoritter; //used so we can se if option to remove favorite should be shown

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
            _active = new Stack<Item>();
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
            if (_active.Count == 0)
            {
                Activate(null);
            }
            else
            {
                //for å forhindre at keyboardet kommer opp når man går tilbake fra videovisning til søkresultatsida.
                if (_active.Peek().ID.Equals("sok"))
                {
                    UpdateList(matchingItems);
                    return;
                }
                Activate(_active.Pop());
            }
        }

        protected override void OnPreviousWindow()
        {
            if (_active.Count <= 0)
            {
                base.OnPreviousWindow();
                return;
            }
            _active.Pop(); //we do not want to show the active item again

            if (_active.Count > 0)
            {
                //for å forhindre at keyboardet kommer opp når man "browser bakover" til søkeresultatsida
                if (_active.Peek().ID.Equals("sok"))
                {
                    UpdateList(matchingItems);
                    return;
                }
                Activate(_active.Pop());
            }
            else Activate(null);
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
                        
                        if (_nrk == null)
                        {
                            using (WaitCursor cursor = new WaitCursor())
                            {
                                Log.Info("Parser is null, getting speed-cookie and creates parser.");
                                Settings settings = new Settings(Config.GetFile(Config.Dir.Config, NrkConstants.CONFIG_FILE));
                                int speed = settings.GetValueAsInt("NrkBrowser", "speed", 2048);
                                _nrk = new NrkParser(speed);
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
//                GUIPropertyManager.SetProperty(PROGRAM_PICTURE, "");
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
            if (_active.Count == 0)
            {
                return false;
            }
            else
            {
                return _active.Peek() == favoritter;
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
            matchingItems = _nrk.GetSearchHits(item.Keyword, item.IndexPage + 1);

            if (matchingItems.Count == 25)
            {
                SearchItem nextPage = new SearchItem("nextPage", "Neste side", item.IndexPage + 1);
                nextPage.Description = "Se de neste 25 treffene";
                nextPage.Keyword = item.Keyword;
                matchingItems.Add(nextPage);
            }
            UpdateList(matchingItems);
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
            matchingItems = _nrk.GetSearchHits(keyword, 0);
            if (matchingItems.Count == 25)
            {
                SearchItem nextPage = new SearchItem("nextPage", "Neste side", 0);
                nextPage.Description = "Se de neste 25 treffene";
                nextPage.Keyword = keyword;
                matchingItems.Add(nextPage);
            }
            UpdateList(matchingItems);
        }

        protected void Activate(Item item)
        {
            GUIPropertyManager.SetProperty(NrkConstants.GUI_PROPERTY_CLIP_COUNT, " ");
        
            if (item == null)
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

                favoritter = new MenuItem("favoritter", "Favoritter");
                favoritter.Description = "Se dine favoritter";
                items.Add(favoritter);

                UpdateList(items);
                return;
            }

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

            //push var her tidligere
            _active.Push(item);

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
                SearchItem sItem = (SearchItem) item;
                search(sItem);
                return;
            }

            if (item.ID == "anbefalte")
            {
                List<Item> items = _nrk.GetAnbefaltePaaForsiden();
                UpdateList(items);
                setClipCount(items);
                return;
            }

            if (item.ID == "mestSett")
            {
                List<Item> items = CreateMestSetteListItems();
                UpdateList(items);
                return;
            }

            if (item.ID == "mestSettUke")
            {
                List<Item> items = _nrk.GetMestSette(7);
                UpdateListAndSetClipCount(items);
                return;
            }

            if (item.ID == "mestSettMaaned")
            {
                List<Item> items = _nrk.GetMestSette(31);
                UpdateListAndSetClipCount(items);
                return;
            }

            if (item.ID == "mestSettTotalt")
            {
                List<Item> items = _nrk.GetMestSette(3650);
                UpdateListAndSetClipCount(items);
                return;
            }

            if (item.ID == "nyheter" || item.ID == "sport" || item.ID == "natur" || item.ID == "super")
            {
                List<Item> items = _nrk.GetTopTabRSS(item.ID);
                UpdateListAndSetClipCount(items);
                return;
            }

            if (item.ID == "categories")
            {
                List<Item> items = _nrk.GetCategories();
                UpdateList(items);
                return;
            }

            if (item.ID == "all")
            {
                List<Item> items = _nrk.GetAllPrograms();
                UpdateList(items);
                return;
            }

            if (item.ID == "live")
            {
                List<Item> items = _nrk.GetDirektePage("direkte");
                items.Add(new MenuItem("liveAlternate", "Alternative linker"));
                UpdateList(items);
                return;
            }
            if (item.ID == "liveAlternate")
            {
                List<Item> items = new List<Item>();
                items.Add(new Stream(NrkConstants.STREAM_PREFIX + "03" + _suffix, "NRK 1"));
                items.Add(new Stream(NrkConstants.STREAM_PREFIX + "04" + _suffix, "NRK 2"));
                items.Add(new Stream(NrkConstants.STREAM_PREFIX + "05" + _suffix, "NRK Alltid Nyheter"));
                items.Add(new Stream(NrkConstants.STREAM_PREFIX + "08" + _suffix, "Testkanal (innhold varierer)"));
                items.Add(new MenuItem("liveall", "Velg strøm manuelt"));
                UpdateList(items);
                return;
            }

            if (item.ID == "liveall")
            {
                List<Item> items = new List<Item>();
                for (int i = 0; i < 10; i++)
                {
                    items.Add(new Stream(NrkConstants.STREAM_PREFIX + i.ToString("D2") + _suffix, "Strøm " + i));
                }
                UpdateList(items);
                return;
            }
            if (item is AutogeneratedMenuItem)
            {
                UpdateList(_nrk.GetTopTabContent(item.ID));
            }
            if (item is Category)
            {
                UpdateList(_nrk.GetPrograms((Category) item));
            }
            if (item is Program)
            {
                List<Item> items = _nrk.GetClips((Program) item);
                setClipCount(items);
                items.AddRange(_nrk.GetFolders((Program) item));
                UpdateList(items);
            }
            if (item is Folder)
            {
                List<Item> items = _nrk.GetClips((Folder) item);
                setClipCount(items);
                items.AddRange(_nrk.GetFolders((Folder) item));
                UpdateList(items);
            }
        }

        public List<Item> GetTabItems()
        {
           List<Item> menuItems = _nrk.GetTopTabber();
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
            List<Item> items = FavoritesUtil.getDatabase(null).getFavoriteVideos();
            UpdateList(items);
        }

        protected void PlayClip(Clip item)
        {
            Log.Info(NrkConstants.PLUGIN_NAME + " PlayClip " + item);
            string url = _nrk.GetClipUrl(item);
            if (url == null)
            {
                ShowMessageBox("Kunne ikke spille av klipp");
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
            Log.Info(NrkConstants.PLUGIN_NAME + "PlayUrl");

            PlayListType type;
            if (url.EndsWith(".wmv")) type = PlayListType.PLAYLIST_VIDEO_TEMP;
            else if (url.EndsWith(".wma")) type = PlayListType.PLAYLIST_RADIO_STREAMS;
                //FIXME: Hangs on some audio streams...
            else if (url.EndsWith("_h")) type = PlayListType.PLAYLIST_VIDEO_TEMP; //det er en av live streamene
            else
            {
                type = PlayListType.PLAYLIST_VIDEO_TEMP;
            }
            bool playOk = false;

            if (_osdPlayer)
            {
                playOk = playWithOsd(url, title, type, startTime);
            }
            else
            {
                playOk = playWithoutOsd(url, title, type, startTime);
            }

            if (!playOk)
            {
                string message = String.Empty;
                //if contains geoblokk
                if (_osdPlayer)
                {
                    message = "Avspilling feilet, prøv å slå av osd player/vmr 9 for webstreams";
                }
                else
                {
                    message = "Avspilling feilet";
                }
                if (url.Contains("geoblokk"))
                {
                    message = "Avspilling feilet";
                    message += "Valgt klipp er kun tilgjengelig i Norge. (Chosen clip is only available in Norway)";
                }
                ShowMessageBox(message);
                Log.Info(NrkConstants.PLUGIN_NAME + " Playing failed");
            }
            else
            {
                if (type == PlayListType.PLAYLIST_VIDEO_TEMP)
                {
                    Log.Info(NrkConstants.PLUGIN_NAME + " Playing OK, switching to fullscreen");
                    g_Player.ShowFullScreenWindow();
                    g_Player.FullScreen = true;
                }
            }
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
            dlg.SetHeading("NrkBrowser");
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
                dlgMenu.SetHeading("NRK Browser");
                if (!_active.Contains(favoritter))
                {
                    dlgMenu.Add("Legg til i favoritter");
                }
                if (_active.Contains(favoritter))
                {
                    dlgMenu.Add("Fjern favoritt");
                }
                dlgMenu.Add("Bruk valgt som søkeord");
                dlgMenu.Add("Kvalitet");
//                dlgMenu.Add("Se etter ny versjon");
                dlgMenu.DoModal(GetWindowId());

                if (dlgMenu.SelectedLabel == -1) // Nothing was selected
                {
                    return;
                }
                if (dlgMenu.SelectedLabelText == "Legg til i favoritter")
                {
                    addToFavourites(item);
                }
                else if (dlgMenu.SelectedLabelText == "Bruk valgt som søkeord")
                {
                    search(item.Title);
                }
                else if (dlgMenu.SelectedLabelText == "Fjern favoritt")
                {
                    removeFavourite(item);
                }
                else if (dlgMenu.SelectedLabelText == "Kvalitet")
                {
                    openQualityMenu(dlgMenu);
                }
//                else if (dlgMenu.SelectedLabelText == "Se etter ny versjon")
//                {
//                    String nyVer = String.Empty;
//                    if (newVersionAvailable(ref nyVer))
//                    {
//                        ShowMessageBox(string.Format("Ny versjon er tilgjengelig (ver. {0}), last ned fra mediaportal forumet.", nyVer));
//                    }
//                    else
//                    {
//                        ShowMessageBox("Ingen nyere versjon tilgjengelig.");
//                    }
//                }
               
            }
        }

//        public static bool newVersionAvailable(ref String nyVer)
//        {
//            Log.Info("Checking for new version of plugin");
//            String availableVersion = GetNewestAvailableVersion();
//            nyVer = availableVersion;
//            String thisVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
//            String[] splittedAvailable = availableVersion.Split('.');
//            String[] splittedThis = thisVersion.Split('.');
//            if (Int32.Parse(splittedAvailable[0]) > Int32.Parse(splittedThis[0]))
//            {
//                return true;
//            }
//            else if (Int32.Parse(splittedAvailable[1]) > Int32.Parse(splittedThis[1]))
//            {
//                return true;
//            }
//            else if (Int32.Parse(splittedAvailable[2]) > Int32.Parse(splittedThis[2]))
//            {
//                return true;
//            }
//            return false;
//        }
//
//        private static string GetNewestAvailableVersion()
//        {
//            System.Net.ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(trustAllCertificates);
//            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("someurl/ver.txt");
//            request.UserAgent = string.Format("NrkBrowser Updater(plugin-version: {0})", Assembly.GetExecutingAssembly().GetName().Version.ToString());
//            // Set some reasonable limits on resources used by this request
//            request.MaximumAutomaticRedirections = 4;
//            request.MaximumResponseHeadersLength = 4;
//            NetworkCredential nc = new NetworkCredential("updateUser", "update");
//            request.Credentials = nc;
//            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
//
//            // Get the stream associated with the response.
//            System.IO.Stream receiveStream = response.GetResponseStream();
//
//            // Pipes the stream to a higher level stream reader with the required encoding format. 
//            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
//
//            string ret = readStream.ReadToEnd();
//            Log.Info("Newest version of plugin is: " + ret);
//            response.Close();
//            readStream.Close();
//            return ret;
//        }
//
//        private static bool trustAllCertificates(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
//        {
//            return true;
//        }

        protected void openQualityMenu(GUIDialogMenu dlgMenu)
        {
                dlgMenu.Reset();
                dlgMenu.SetHeading("Velg kvalitet");
                GUIListItem lowQuality = new GUIListItem("Lav kvalitet");
                lowQuality.ItemId = 1;
                GUIListItem mediumQuality = new GUIListItem("Middels kvalitet");
                mediumQuality.ItemId = 2;
                GUIListItem highQuality = new GUIListItem("Høy kvalitet");
                highQuality.ItemId = 3;
                dlgMenu.Add(lowQuality);
                dlgMenu.Add(mediumQuality);
                dlgMenu.Add(highQuality);
                dlgMenu.DoModal(GetWindowId());
                if (dlgMenu.SelectedId == lowQuality.ItemId)
                {
                    int speed = 400;
                    Log.Info(string.Format("{0}: Changing bitrate to {1}", NrkConstants.PLUGIN_NAME, speed));
                    _nrk = new NrkParser(speed);
                }
                else if (dlgMenu.SelectedId == mediumQuality.ItemId)
                {
                    int speed = 1000;
                    Log.Info(string.Format("{0}: Changing bitrate to {1}", NrkConstants.PLUGIN_NAME, speed));
                    _nrk = new NrkParser(speed);
                }
                else if (dlgMenu.SelectedId == highQuality.ItemId)
                {
                    int speed = 10000;
                    Log.Info(string.Format("{0}: Changing bitrate to {1}", NrkConstants.PLUGIN_NAME, speed));
                    _nrk = new NrkParser(speed);
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
                    ShowMessageBox("Favoritt kunne ikke bli lagt til: " + message);
                }
            }
            else if (item is Program)
            {
                Program p = (Program) item;
                String message = "";
                if (!db.addFavoriteProgram(p, ref message))
                {
                    ShowMessageBox("Favoritt kunne ikke bli lagt til: " + message);
                }
            }
            else
            {
                ShowMessageBox("Kun klipp eller program kan legges til som favoritt");
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
                Clip c = (Clip) item;
                removedSuccessFully = db.removeFavoriteVideo(c);
            }
            else if (item is Program)
            {
                Program p = (Program) item;
                removedSuccessFully = db.removeFavoriteProgram(p);
            }
            else
            {
                Log.Error("Item to remove from favourite is neither Clip nor Program, this should never happen.");
            }

            if (!removedSuccessFully)
            {
                ShowMessageBox("Favoritt kunne ikke fjernes");
            }
            else
            {
                //must remove removed item from list..do so by refreshing the whole favouriteslist
                FillStackWithFavourites();
            }
        }
    }
}