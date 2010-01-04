/*
 * Copyright (c) 2008 Terje Wiesener <wiesener@samfundet.no>
 * 
 * Loosely based on an anonymous (and slightly outdated) NRK parser in python for Myth-tv, 
 * please email me if you are the author :)
 * 
 * 2008 - 2009 Vattenmelon
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
using Vattenmelon.Nrk.Browser.Db;
using Vattenmelon.Nrk.Browser.Domain;
using Vattenmelon.Nrk.Browser.Translation;
using Vattenmelon.Nrk.Parser;
using Vattenmelon.Nrk.Domain;
using Vattenmelon.Nrk.Parser.Xml;
using MenuItem = Vattenmelon.Nrk.Browser.Domain.MenuItem;

namespace Vattenmelon.Nrk.Browser
{
    public class NrkPlugin : GUIWindow, ISetupForm
    {
        public static string PICTURE_DIR = string.Format(@"{0}\media\nrkbrowser\", GUIGraphicsContext.Skin);

        protected string liveStreamUrlSuffix = "_l";
        

        /// <summary>
        /// This is the name of the plugin as it appears in MediaPortal gui. Note: the name specified here will
        /// be shown in the plugins list in the configuration, while in the gui inside mediaportal it will use the name
        /// specified in the configuration, or the default name. 
        /// </summary>
        private string pluginName = NrkBrowserConstants.PLUGIN_NAME;

        [SkinControlAttribute(50)] protected GUIFacadeControl facadeView = null;

        protected NrkParser nrkParser = null;
        protected Stack<Item> activeStack = null;
        private List<Item> matchingItems = new List<Item>();
        private MenuItem favoritter; //used so we can se if option to remove favorite should be shown
    
        protected bool useOsdPlayer = true;

        public NrkPlugin()
        {
            NrkTranslatableStrings.Init();
            VersionChecker.SetLog(new MPLogger());
        }
        /// <summary>
        /// This constructor is intended for testing purposes
        /// </summary>
        /// <param name="language"></param>
        /// <param name="languagePath"></param>
        public NrkPlugin(String language, string languagePath)
        {
            NrkTranslatableStrings.InitWithParam(language, languagePath);
        }

        public override int GetID
        {
            get { return 40918376; }
        }

        #region ISetupForm Members

        public string PluginName()
        {
            return pluginName;
        }
        
        public string Description()
        {
            return NrkBrowserConstants.ABOUT_DESCRIPTION;
        }

        public string Author()
        {
            return NrkBrowserConstants.ABOUT_AUTHOR;
        }

        public void ShowPlugin()
        {
            string appVersion = getVersion();
            string libraryVersion = getLibraryVersion();
            string domainVersion = getDomainVersion();
            Log.Debug(string.Format("{0}: pluginversion: {1}, libraryversion: {2}, domainversion: {3}", NrkBrowserConstants.PLUGIN_NAME, appVersion, libraryVersion, domainVersion));
            SettingsForm form = new SettingsForm();
            setVersionNumberInConfigDialog(appVersion, form, libraryVersion);
            Settings settings = new Settings(Config.GetFile(Config.Dir.Config, NrkBrowserConstants.CONFIG_FILE));
            initValuesInConfigDialog(form, settings);
            DialogResult res = form.ShowDialog();
            if (res == DialogResult.OK)
            {
                SaveSettings(form, settings);
            }
        }

        private static void setVersionNumberInConfigDialog(string appVersion, SettingsForm form, string libraryVersion)
        {
            form.LabelVersionPluginVerdi.Text = appVersion;
            form.LabelVersionLibraryVerdi.Text = libraryVersion;
        }

        private void initValuesInConfigDialog(SettingsForm form, Settings settings)
        {
            initSpeedSettings(form, settings);
            initLiveStreamQuality(form, settings);
            initPluginName(form, settings);
        }

        private void initPluginName(SettingsForm form, Settings settings)
        {
            pluginName = settings.GetValueAsString(NrkBrowserConstants.CONFIG_SECTION, NrkBrowserConstants.CONFIG_ENTRY_PLUGIN_NAME, NrkBrowserConstants.PLUGIN_NAME);
            form.NameTextbox.Text = pluginName;
        }

        private static string initLiveStreamQuality(SettingsForm form, Settings settings)
        {
            string quality =
                settings.GetValueAsString(NrkBrowserConstants.CONFIG_SECTION, NrkBrowserConstants.CONFIG_ENTRY_LIVE_STREAM_QUALITY,
                                          NrkBrowserConstants.CONFIG_DEFAULT_LIVE_STREAM_QUALITY);
            form.liveStreamQualityCombo.SelectedIndex = form.liveStreamQualityCombo.Items.IndexOf(quality);
            return quality;
        }

        private static int initSpeedSettings(SettingsForm form, Settings settings)
        {
            int speed = settings.GetValueAsInt(NrkBrowserConstants.CONFIG_SECTION, NrkBrowserConstants.CONFIG_ENTRY_SPEED, 2048);
            if (speed < form.speedUpDown.Minimum)
            {
                speed = (int)form.speedUpDown.Minimum;
                settings.SetValue(NrkBrowserConstants.CONFIG_SECTION, NrkBrowserConstants.CONFIG_ENTRY_SPEED, speed);
            }
            if (speed > form.speedUpDown.Maximum)
            {
                speed = (int)form.speedUpDown.Maximum;
                settings.SetValue(NrkBrowserConstants.CONFIG_SECTION, NrkBrowserConstants.CONFIG_ENTRY_SPEED, speed);
            }
            form.speedUpDown.Value = speed;
            return speed;
        }

        private string getDomainVersion()
        {
            return Assembly.GetAssembly(typeof(ILog)).GetName().Version.ToString();
        }

        public static string getVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        public static string getLibraryVersion()
        {
            return Assembly.GetAssembly(typeof(NrkParser)).GetName().Version.ToString();
        }

        private void SaveSettings(SettingsForm form, Settings settings)
        {
            Log.Debug(NrkBrowserConstants.PLUGIN_NAME + ": SaveSettings");
            int speed = (int) form.speedUpDown.Value;
            settings.SetValue(NrkBrowserConstants.CONFIG_SECTION, NrkBrowserConstants.CONFIG_ENTRY_SPEED, speed);
            string quality = (string) form.liveStreamQualityCombo.Items[form.liveStreamQualityCombo.SelectedIndex];
            settings.SetValue(NrkBrowserConstants.CONFIG_SECTION, NrkBrowserConstants.CONFIG_ENTRY_LIVE_STREAM_QUALITY, quality);
            pluginName = form.NameTextbox.Text;
            settings.SetValue(NrkBrowserConstants.CONFIG_SECTION, NrkBrowserConstants.CONFIG_ENTRY_PLUGIN_NAME, pluginName);
        }

        public bool CanEnable()
        {
            return true;
        }

        public int GetWindowId()
        {
            return GetID;
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
            strPictureImage = GUIGraphicsContext.Skin + NrkBrowserConstants.HOVER_IMAGE;
            return true;
        }

        #endregion

        public override bool Init()
        {
            Log.Debug(string.Format("{0}: Init()", NrkBrowserConstants.PLUGIN_NAME));
            using (
                Settings xmlreader =
                    new Settings(Config.GetFile(Config.Dir.Config, NrkBrowserConstants.MEDIAPORTAL_CONFIG_FILE)))
            {
                useOsdPlayer = xmlreader.GetValueAsBool("general", "usevrm9forwebstreams", true);
            }

            bool result = Load(string.Format(@"{0}\{1}", GUIGraphicsContext.Skin, NrkBrowserConstants.SKIN_FILENAME));
            Settings settings = new Settings(Config.GetFile(Config.Dir.Config, NrkBrowserConstants.CONFIG_FILE));
            int speed = settings.GetValueAsInt(NrkBrowserConstants.CONFIG_SECTION, NrkBrowserConstants.CONFIG_ENTRY_SPEED, 2048);
            activeStack = new Stack<Item>();
            String alternativeLiveStreamsQuality =
                settings.GetValueAsString(NrkBrowserConstants.CONFIG_SECTION, NrkBrowserConstants.CONFIG_ENTRY_LIVE_STREAM_QUALITY,
                                          NrkBrowserConstants.CONFIG_DEFAULT_LIVE_STREAM_QUALITY);
            pluginName =
                settings.GetValueAsString(NrkBrowserConstants.CONFIG_SECTION, NrkBrowserConstants.CONFIG_ENTRY_PLUGIN_NAME, NrkBrowserConstants.PLUGIN_NAME);

            SetAlternativeLiveStreamQuality(alternativeLiveStreamsQuality);

            return result;
        }

        private void SetAlternativeLiveStreamQuality(string quality)
        {
            Dictionary<String, String> qualityMap = new Dictionary<string, string>();
            qualityMap[NrkBrowserConstants.QUALITY_LOW] = NrkBrowserConstants.QUALITY_LOW_SUFFIX;
            qualityMap[NrkBrowserConstants.QUALITY_MEDIUM] = NrkBrowserConstants.QUALITY_MEDIUM_SUFFIX;
            qualityMap[NrkBrowserConstants.QUALITY_HIGH] = NrkBrowserConstants.QUALITY_HIGH_SUFFIX;
            liveStreamUrlSuffix = qualityMap[quality];
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
                if (activeStack.Peek().ID.Equals(NrkBrowserConstants.MENU_ITEM_ID_SEARCH))
                {
                    UpdateList(matchingItems);
                    return;
                }
                
                Item sisteAktiveItem = activeStack.Pop();
                //Activate(activeStack.Pop());
                Activate(sisteAktiveItem);
                setItemAsSelectedItemInListView(sisteAktiveItem);
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
                if (activeStack.Peek().ID.Equals(NrkBrowserConstants.MENU_ITEM_ID_SEARCH))
                {
                    UpdateList(matchingItems);
                    return;
                }
                Item itemToActivate = activeStack.Pop();
                /*if (lAct is Clip)
                {
                    lAct = activeStack.Pop();
                }*/
                Activate(itemToActivate);
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
            Log.Debug(string.Format("{0}: onMessage()", NrkBrowserConstants.PLUGIN_NAME));
            switch (message.Message)
            {
                case GUIMessage.MessageType.GUI_MSG_WINDOW_INIT:
                    {
                        Log.Debug(
                            string.Format("{0}: onMessage() with MessageType: [{1}]", NrkBrowserConstants.PLUGIN_NAME,
                                          "GUI_MSG_WINDOW_INIT"));
                        GUIPropertyManager.SetProperty("#currentmodule", NrkBrowserConstants.PLUGIN_NAME);

                        if (nrkParser == null)
                        {
                            using (WaitCursor cursor = new WaitCursor())
                            {
                                Log.Info("Parser is null, getting speed-cookie and creates parser.");
                                Settings settings =
                                    new Settings(Config.GetFile(Config.Dir.Config, NrkBrowserConstants.CONFIG_FILE));
                                int speed = settings.GetValueAsInt(NrkBrowserConstants.CONFIG_SECTION, NrkBrowserConstants.CONFIG_ENTRY_SPEED, NrkBrowserConstants.CONFIG_DEFAULT_SPEED);
                                nrkParser = new NrkParser(speed, new MPLogger());
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

            GUIPropertyManager.SetProperty(NrkBrowserConstants.GUI_PROPERTY_PROGRAM_DESCRIPTION, selecteditem.Description);

            if (selecteditem.Description == null || selecteditem.Description.Equals(""))
            {
                GUIPropertyManager.SetProperty(NrkBrowserConstants.GUI_PROPERTY_PROGRAM_DESCRIPTION, " ");
            }

            if (selecteditem.ID == NrkBrowserConstants.MENU_ITEM_ID_ALPHABETICAL_LIST || selecteditem.ID == NrkBrowserConstants.MENU_ITEM_ID_CATEGORIES || selecteditem.ID == NrkBrowserConstants.MENU_ITEM_ID_RECOMMENDED ||
                selecteditem.ID == NrkBrowserConstants.MENU_ITEM_ID_LIVE || selecteditem.ID == NrkBrowserConstants.MENU_ITEM_ID_MOST_WATCHED || selecteditem.ID == NrkBrowserConstants.MENU_ITEM_ID_SEARCH ||
                selecteditem.ID == NrkBrowserConstants.MENU_ITEM_ID_LIVE_ALTERNATE ||
                selecteditem is Category)
            {
                GUIPropertyManager.SetProperty(NrkBrowserConstants.GUI_PROPERTY_PROGRAM_PICTURE, NrkBrowserConstants.DEFAULT_PICTURE);
            }

            if (!selecteditem.Bilde.Equals("") || erFavoritt())
            {
                GUIPropertyManager.SetProperty(NrkBrowserConstants.GUI_PROPERTY_PROGRAM_PICTURE, selecteditem.Bilde);
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

        protected void UpdateList<T>(List<T> newitems, Boolean addBackItem) where T : Item
        {
            facadeView.Clear();
            if (addBackItem)
            {
                GUIListItem backItem = new GUIListItem("..");
                BackMenuItem backMenuItem = new BackMenuItem("back", "..");
                backItem.TVTag = backMenuItem;
                facadeView.Add(backItem);
            }
            foreach (T item in newitems)
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
                facadeView.Add(listitem);
            }
        }

        protected void UpdateList<T>(List<T> newitems) where T:Item
        {
            UpdateList(newitems, true);
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
            search(GetSearchKeywordFromUser());
        }

        private string GetSearchKeywordFromUser()
        {
            String keyword = String.Empty;
            GetUserInputString(ref keyword);
            keyword = keyword.ToLower();
            matchingItems.Clear();
            return keyword;
        }

        private void searchNrkBeta()
        {
            searchNrkBeta(GetSearchKeywordFromUser());
        }

        private void searchNrkBeta(String keyword)
        {
            NrkBetaXmlParser parser = new NrkBetaXmlParser();
            parser.SearchFor(keyword);
            matchingItems = parser.getClips();
            UpdateList(matchingItems);
        }

        private void search(String keyword)
        {
            matchingItems = nrkParser.GetSearchHits(keyword, 0);
            createViewNextPageSearchItemIfNecessary(keyword, 0);
            UpdateList(matchingItems);
        }

        private void createNextPageSearchItem(string keyword, int indexPage)
        {
            SearchItem nextPage =
                new SearchItem(NrkBrowserConstants.SEARCH_NEXTPAGE_ID, NrkTranslatableStrings.SEARCH_NEXTPAGE_TITLE, indexPage);
            nextPage.Description = NrkTranslatableStrings.SEARCH_NEXTPAGE_DESCRIPTION;
            nextPage.Keyword = keyword;
            matchingItems.Add(nextPage);
        }
        private void SetDescriptionForMostWatched(Item item)
        {
            item.Description = String.Format(NrkTranslatableStrings.DESCRIPTION_CLIP_SHOWN_TIMES, item.Description);
        }
        
        protected void Activate(Item item)
        {
            GUIPropertyManager.SetProperty(NrkBrowserConstants.GUI_PROPERTY_CLIP_COUNT, " ");
            Log.Debug("Activate(Item): " + item);
            if (item == null)
            {
                UpdateList(CreateInitialMenuItems(), false);
                return;
            }

            if (!(item is Clip) && !(item is Stream) && !(item is BackMenuItem))
            {
                activeStack.Push(item);   
            }

            if (item is Clip)
            {
                PlayClip((Clip) item);
            }
            else if (item is Stream)
            {
                PlayStream((Stream) item);
            }
            else if (item is BackMenuItem)
            {      
                Item poppedItem = activeStack.Pop();
                if (activeStack.Count > 0)
                {
                    Item itemToActivate = activeStack.Pop();
                    Activate(itemToActivate);
                    setItemAsSelectedItemInListView(poppedItem);
                }
                else
                {
                    UpdateList(CreateInitialMenuItems(), false);
                    setItemAsSelectedItemInListView(poppedItem);
                }
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_FAVOURITES)
            {
                FillStackWithFavourites();
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_SEARCH)
            {
                search();
            }
            else if (item.ID == NrkBrowserConstants.SEARCH_NEXTPAGE_ID)
            {
                search((SearchItem) item);
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_RECOMMENDED)
            {
                UpdateList(nrkParser.GetAnbefaltePaaForsiden());
                setClipCount(nrkParser.GetAnbefaltePaaForsiden());
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_MOST_WATCHED)
            {
                UpdateList(CreateMestSetteListItems());
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_MEST_SETTE_UKE)
            {
                List<Item> tItems = nrkParser.GetMestSette(7);
                tItems.ForEach(SetDescriptionForMostWatched);
                UpdateListAndSetClipCount(tItems);
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_MEST_SETTE_MAANED)
            {
                List<Item> tItems = nrkParser.GetMestSette(31);
                tItems.ForEach(SetDescriptionForMostWatched);
                UpdateListAndSetClipCount(tItems);
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_MEST_SETTE_TOTALT)
            {
                List<Item> tItems = nrkParser.GetMestSette(3650);
                tItems.ForEach(SetDescriptionForMostWatched);
                UpdateListAndSetClipCount(tItems);
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_NRKBETA)
            {
                UpdateList(CreateNrkBetaListItems());
            }
            else if (isNrkBetaSectionItem(item))
            {
                String section = GetNrkBetaSection(item.ID);
                NrkBetaXmlParser nrkBetaParser = new NrkBetaXmlParser(NrkParserConstants.NRK_BETA_FEEDS_KATEGORI_URL, section);
                List<Item> tItems = nrkBetaParser.getClips();
                UpdateListAndSetClipCount(tItems);
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_SEARCH)
            {
                searchNrkBeta();
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_HD_KLIPP)
            {
                List<Item> tItems = new NrkBetaXmlParser().FindHDClips().getClips();
                UpdateListAndSetClipCount(tItems);
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_SISTE_KLIPP)
            {
                List<Item> tItems = new NrkBetaXmlParser().FindLatestClips().getClips();
                UpdateListAndSetClipCount(tItems);
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_NYHETER || item.ID == NrkBrowserConstants.MENU_ITEM_ID_SPORT || item.ID == NrkBrowserConstants.MENU_ITEM_ID_NATUR || item.ID == NrkBrowserConstants.MENU_ITEM_ID_SUPER)
            {
                List<Item> tItems = nrkParser.GetTopTabRSS(item.ID);
                tItems.ForEach(delegate(Item thisItem)
                                   {
                                       thisItem.Bilde = PICTURE_DIR + GetPictureFile(item.ID);
                                   });
                UpdateListAndSetClipCount(tItems);
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_CATEGORIES)
            {
                UpdateList(nrkParser.GetCategories());
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_ALPHABETICAL_LIST)
            {
                UpdateList(nrkParser.GetAllPrograms());
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_LIVE)
            {
                UpdateList(CreateLiveMenuItems());
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_LIVE_ALTERNATE)
            {
                UpdateList(CreateLiveAlternateMenuItems());
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_CHOOSE_STREAM_MANUALLY)
            {
                UpdateList(CreateLiveAlternateAllMenuItems());
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_PODCASTS)
            {
                UpdateList(CreatePodcastMenuItems());
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_PODCASTS_AUDIO)
            {
                try
                {
                    List<PodKast> podkasts = (List<PodKast>) nrkParser.GetLydPodkaster();
                    podkasts.ForEach(new Action<PodKast>(setNrkLogoPictureOnItem));
                    UpdateList(podkasts);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    ShowMessageBox("Could not retrieve podcast list");
                }
            }
            else if (item.ID == NrkBrowserConstants.MENU_ITEM_ID_PODCASTS_VIDEO)
            {
                try
                {
                    List<PodKast> podkasts = (List<PodKast>)nrkParser.GetVideoPodkaster();
                    podkasts.ForEach(new Action<PodKast>(setNrkLogoPictureOnItem));
                    UpdateList(podkasts);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                    ShowMessageBox("Could not retrieve podcast list");
                }
            }
            else if (item is AutogeneratedMenuItem)
            {
                List<Item> tItems = nrkParser.GetTopTabContent(item.ID);
                tItems.ForEach(delegate(Item itemToChangeDescription)
                                   {
                                       itemToChangeDescription.Description = string.Format(NrkTranslatableStrings.DESCRIPTION_CLIP_ADDED, itemToChangeDescription.Description);
                                   });
                UpdateList(tItems);

            }
            else if (item is Category)
            {
                UpdateList(nrkParser.GetPrograms((Category)item));
            }
            else if (item is Program)
            {
                List<Item> items = nrkParser.GetClips((Program)item);
                setClipCount(items);
                items.AddRange(nrkParser.GetFolders((Program)item));
                UpdateList(items);
            }
            else if (item is Folder)
            {
                List<Item> items = nrkParser.GetClips((Folder)item);
                setClipCount(items);
                items.AddRange(nrkParser.GetFolders((Folder)item));
                UpdateList(items);
            }
            else if (item is PodKast)
            {
                PodkastXmlParser pxp = new PodkastXmlParser(item.ID);
                List<Item> items = pxp.getClips();
                setClipCount(items);
                UpdateList(items);
            }
        }

        private void setNrkLogoPictureOnItem(Item item)
        {
            item.Bilde = PICTURE_DIR + NrkBrowserConstants.NRK_LOGO_PICTURE;
        }

        public List<Item> CreatePodcastMenuItems()
        {
            List<Item> items = new List<Item>(2);
            MenuItem item1 = new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_PODCASTS_AUDIO, NrkTranslatableStrings.MENU_ITEM_TITLE_PODCASTS_AUDIO);
            item1.Bilde = PICTURE_DIR + NrkBrowserConstants.NRK_LOGO_PICTURE;
            MenuItem item2 = new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_PODCASTS_VIDEO, NrkTranslatableStrings.MENU_ITEM_TITLE_PODCASTS_VIDEO);
            item2.Bilde = PICTURE_DIR + NrkBrowserConstants.NRK_LOGO_PICTURE;
            items.Add(item1);
            items.Add(item2);
            return items;
        }


        private string GetNrkBetaSection(string id)
        {
            if (id.Equals(NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_TVSERIER))
            {
                return NrkParserConstants.NRK_BETA_SECTION_FRA_TV;
            }
            else if (id.Equals(NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_DIVERSE))
            {
                return NrkParserConstants.NRK_BETA_SECTION_DIVERSE;
            }
            else if (id.Equals(NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_PRESENTASJONER))
            {
                return NrkParserConstants.NRK_BETA_SECTION_PRESENTASJONER;
            }
            else if (id.Equals(NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_KONFERANSER_OG_MESSER))
            {
                return NrkParserConstants.NRK_BETA_SECTION_KONFERANSER_OG_MESSER;
            }
            else if (id.Equals(NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_FRA_TV))
            {
                return NrkParserConstants.NRK_BETA_SECTION_FRA_TV;
            }
            else
            {
                throw new Exception("Unknown NRKBETA section!");
            }
        }

        private static bool isNrkBetaSectionItem(Item item)
        {
            return item.ID == NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_TVSERIER || item.ID == NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_DIVERSE || item.ID == NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_FRA_TV || item.ID == NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_KONFERANSER_OG_MESSER || item.ID == NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_PRESENTASJONER;
        }

        private List<Item> CreateNrkBetaListItems()
        {
            List<Item> items = new List<Item>(3);
            items.Add(new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_SISTE_KLIPP, NrkTranslatableStrings.MENU_ITEM_TITLE_NRKBETA_LATEST_CLIPS));
            items.Add(new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_TVSERIER, NrkTranslatableStrings.MENU_ITEM_TITLE_NRKBETA_TV_SERIES));
            items.Add(new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_DIVERSE, NrkTranslatableStrings.MENU_ITEM_TITLE_NRKBETA_OTHER));
            items.Add(new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_PRESENTASJONER, NrkTranslatableStrings.MENU_ITEM_TITLE_NRKBETA_PRESENTATIONS));
            items.Add(new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_KONFERANSER_OG_MESSER, NrkTranslatableStrings.MENU_ITEM_TITLE_NRKBETA_CONFERENCES));
            items.Add(new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_FRA_TV, NrkTranslatableStrings.MENU_ITEM_TITLE_NRKBETA_FROM_TV));
            items.Add(new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_HD_KLIPP, NrkTranslatableStrings.MENU_ITEM_TITLE_NRKBETA_HD_CLIPS));
            items.Add(new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_NRKBETA_SEARCH, NrkTranslatableStrings.MENU_ITEM_TITLE_NRKBETA_SEARCH));
            items.ForEach(delegate(Item item)
                              {
                                  item.Bilde = PICTURE_DIR + NrkBrowserConstants.MENU_ITEM_PICTURE_NRKBETA;
                              });
            return items;
        }

        public string GetPictureFile(string id)
        {
            String pictureFile;
                        switch (id)
                        {
                            case NrkBrowserConstants.MENU_ITEM_ID_NYHETER:
                                pictureFile = NrkBrowserConstants.MENU_ITEM_PICTURE_NYHETER;
                                break;
                            case NrkBrowserConstants.MENU_ITEM_ID_SPORT:
                                pictureFile = NrkBrowserConstants.MENU_ITEM_PICTURE_SPORT;
                                break;
                            case NrkBrowserConstants.MENU_ITEM_ID_NATUR:
                                pictureFile = NrkBrowserConstants.MENU_ITEM_PICTURE_NATURE;
                                break;
                            case NrkBrowserConstants.MENU_ITEM_ID_SUPER:
                                pictureFile = NrkBrowserConstants.MENU_ITEM_PICTURE_SUPER;
                                break;
                            default:
                                pictureFile = NrkBrowserConstants.DEFAULT_PICTURE;
                                break;
                        }
            return pictureFile;
        }

        private List<Item> CreateInitialMenuItems()
        {
            List<Item> items = new List<Item>();
            items.Add(new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_ALPHABETICAL_LIST, NrkTranslatableStrings.MENU_ITEM_TITLE_ALPHABETICAL_LIST));
            items.Add(new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_CATEGORIES, NrkTranslatableStrings.MENU_ITEM_TITLE_CATEGORIES));
            items.Add(new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_LIVE, NrkTranslatableStrings.MENU_ITEM_TITLE_LIVE_STREAMS));
            MenuItem anbefalte = new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_RECOMMENDED, NrkTranslatableStrings.MENU_ITEM_TITLE_RECOMMENDED_PROGRAMS);
            anbefalte.Description = NrkTranslatableStrings.MENU_ITEM_DESCRIPTION_RECOMMENDED_PROGRAMS;
            items.Add(anbefalte);

            MenuItem mestSett = new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_MOST_WATCHED, NrkTranslatableStrings.MENU_ITEM_TITLE_MOST_WATCHED);
            mestSett.Description = NrkTranslatableStrings.MENU_ITEM_DESCRIPTION_MOST_WATCHED;
            items.Add(mestSett);

            MenuItem nrkBeta = new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_NRKBETA, NrkTranslatableStrings.MENU_ITEM_TITLE_NRKBETA);
            nrkBeta.Description = NrkTranslatableStrings.MENU_ITEM_DESCRIPTON_NRKBETA;
            nrkBeta.Bilde = PICTURE_DIR + NrkBrowserConstants.MENU_ITEM_PICTURE_NRKBETA;
            items.Add(nrkBeta);

            MenuItem nyheter = new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_NYHETER, NrkTranslatableStrings.MENU_ITEM_TITLE_NEWS);
            nyheter.Description = NrkTranslatableStrings.MENU_ITEM_DESCRIPTION_NEWS;
            nyheter.Bilde = PICTURE_DIR + NrkBrowserConstants.MENU_ITEM_PICTURE_NYHETER;
            items.Add(nyheter);

            MenuItem sport = new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_SPORT, NrkTranslatableStrings.MENU_ITEM_TITLE_SPORT);
            sport.Description = NrkTranslatableStrings.MENU_ITEM_DESCRIPTION_SPORT;
            sport.Bilde = PICTURE_DIR + NrkBrowserConstants.MENU_ITEM_PICTURE_SPORT;
            items.Add(sport);

            MenuItem natur = new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_NATUR, NrkTranslatableStrings.MENU_ITEM_TITLE_NATURE);
            natur.Description = NrkTranslatableStrings.MENU_ITEM_DESCRIPTION_NATURE;
            natur.Bilde = PICTURE_DIR + NrkBrowserConstants.MENU_ITEM_PICTURE_NATURE;
            items.Add(natur);

            MenuItem super = new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_SUPER, NrkTranslatableStrings.MENU_ITEM_TITLE_SUPER);
            super.Description = NrkTranslatableStrings.MENU_ITEM_DESCRIPTION_SUPER;
            super.Bilde = PICTURE_DIR + NrkBrowserConstants.MENU_ITEM_PICTURE_SUPER;
            items.Add(super);

            List<Item> tabItems = GetTabItems();
            items.AddRange(tabItems);

            MenuItem sok = new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_SEARCH, NrkTranslatableStrings.MENU_ITEM_TITLE_SEARCH);
            sok.Description = NrkTranslatableStrings.MENU_ITEM_DESCRIPTION_SEARCH;
            items.Add(sok);

            MenuItem podCasts = new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_PODCASTS, NrkTranslatableStrings.MENU_ITEM_TITLE_PODCASTS);
            podCasts.Description = NrkTranslatableStrings.MENU_ITEM_DESCRIPTION_PODCASTS;
            items.Add(podCasts);

            favoritter = new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_FAVOURITES, NrkTranslatableStrings.MENU_ITEM_TITLE_FAVOURITES);
            favoritter.Description = NrkTranslatableStrings.MENU_ITEM_DESCRIPTION_FAVOURITES;
            items.Add(favoritter);
            return items;
        }

        private List<Item> CreateLiveMenuItems()
        {
            List<Item> items = nrkParser.GetDirektePage();
            items.Add(new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_LIVE_ALTERNATE, NrkTranslatableStrings.MENU_ITEM_TITLE_ALTERNATIVE_LINKS));
            return items;
        }

        private List<Item> CreateLiveAlternateAllMenuItems()
        {
            List<Item> items = new List<Item>();
            for (int i = 0; i < 10; i++)
            {
                items.Add(new Stream(NrkParserConstants.STREAM_PREFIX + i.ToString("D2") + liveStreamUrlSuffix, "Strøm " + i));
            }
            return items;
        }

        private List<Item> CreateLiveAlternateMenuItems()
        {
            List<Item> items = new List<Item>();
            items.Add(new Stream(NrkParserConstants.STREAM_PREFIX + "03" + liveStreamUrlSuffix, NrkBrowserConstants.MENU_ITEM_LIVE_ALTERNATE_NRK1));
            items.Add(new Stream(NrkParserConstants.STREAM_PREFIX + "04" + liveStreamUrlSuffix, NrkBrowserConstants.MENU_ITEM_LIVE_ALTERNATE_NRK2));
            items.Add(new Stream(NrkParserConstants.STREAM_PREFIX + "05" + liveStreamUrlSuffix, NrkBrowserConstants.MENU_ITEM_LIVE_ALTERNATE_3));
            items.Add(new Stream(NrkParserConstants.STREAM_PREFIX + "08" + liveStreamUrlSuffix, NrkBrowserConstants.MENU_ITEM_LIVE_ALTERNATE_4));
            items.Add(new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_CHOOSE_STREAM_MANUALLY, NrkTranslatableStrings.MENU_ITEM_TITLE_CHOOSE_STREAM_MANUALLY));
            return items;
        }

        public List<Item> GetTabItems()
        {
            List<Item> menuItems = nrkParser.GetTopTabber();
            menuItems.ForEach(delegate (Item item)
                                  {
                                      item.Description = string.Format(NrkTranslatableStrings.MENU_ITEM_DESCRIPTION_NEWEST_CLIPS_FROM_GENERIC, item.Title);
                                      item.Bilde = NrkBrowserConstants.DEFAULT_PICTURE;
                                  });
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
            MenuItem mestSettUke =
                new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_MEST_SETTE_UKE, NrkTranslatableStrings.MENU_ITEM_TITLE_MEST_SETTE_UKE);
            mestSettUke.Description = NrkTranslatableStrings.MENU_ITEM_DESCRIPTION_MEST_SETTE_UKE;
            items.Add(mestSettUke);

            MenuItem mestSettMaaned =
                new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_MEST_SETTE_MAANED, NrkTranslatableStrings.MENU_ITEM_TITLE_MEST_SETTE_MAANED);
            mestSettMaaned.Description = NrkTranslatableStrings.MENU_ITEM_DESCRIPTION_MEST_SETTE_MAANED;
            items.Add(mestSettMaaned);

            MenuItem mestSettTotalt =
                new MenuItem(NrkBrowserConstants.MENU_ITEM_ID_MEST_SETTE_TOTALT, NrkTranslatableStrings.MENU_ITEM_TITLE_MEST_SETTE_TOTALT);
            mestSettTotalt.Description = NrkTranslatableStrings.MENU_ITEM_DESCRIPTION_MEST_SETTE_TOTALT;
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
                    GUIPropertyManager.SetProperty(NrkBrowserConstants.GUI_PROPERTY_CLIP_COUNT,
                                                   String.Format(NrkTranslatableStrings.CLIP_COUNT, items.Count));
                }
            }
        }

        /// <summary>
        /// Method that fills the menu with favourites
        /// </summary>
        private void FillStackWithFavourites()
        {
            Log.Debug(NrkBrowserConstants.PLUGIN_NAME + "FillStackWithFavourites()");
            List<Item> items = FavoritesUtil.getDatabase().getFavoriteVideos();
            UpdateList(items);
        }

        protected void PlayClip(Clip item)
        {
            Log.Info(string.Format("{0} PlayClip {1}", NrkBrowserConstants.PLUGIN_NAME, item));
            if (item.Type == Clip.KlippType.KLIPP)
            {
                List<Clip> chapters = nrkParser.getChapters(item);
                if (chapters.Count > 0)
                {
                    item.Type = Clip.KlippType.KLIPP_CHAPTER; //et stygt lite hakk for å unngå evig løkke når man klikker på klippet
                    item.ID = chapters[0].ID; //også en ikke spesielt pen måte..url til "hovedklipp" vil være samme som til andre
                    chapters.Insert(0, item);
                    UpdateList(chapters);
                    return;
                }
            }
            string url = nrkParser.GetClipUrlAndPutStartTime(item);
            if (url == null)
            {
                ShowMessageBox(NrkTranslatableStrings.PLAYBACK_FAILED_URL_WAS_NULL);
                return;
            }
            Log.Info(NrkBrowserConstants.PLUGIN_NAME + " PlayClip, url is: " + url);
            PlayUrl(url, item.Title, item.StartTime, item);
        }

        protected void PlayStream(Stream item)
        {
            Log.Info(NrkBrowserConstants.PLUGIN_NAME + " PlayStream " + item.ID);
            PlayUrl(item.ID, item.Title, 0, item);
        }

        /// <summary>
        /// Plays the URL
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="startTime"></param>
        protected void PlayUrl(string url, string title, double startTime, Item item)
        {
            GUIWaitCursor.Init();
            GUIWaitCursor.Show();
            BackgroundWorker worker = new BackgroundWorker();
            PlayArgs pArgs = new PlayArgs();
            pArgs.url = url;
            pArgs.title = title;
            pArgs.startTime = startTime;
            pArgs.item = item;
            pArgs.playBackOk = true;
            worker.DoWork += new DoWorkEventHandler(PlayMethodDelegate);
            worker.RunWorkerAsync(pArgs);
            //worker.IsBusy prøv å bruke denne istedetfor _workercompleted
            //while (_workerCompleted == false) GUIWindowManager.Process();
            while(worker.IsBusy) GUIWindowManager.Process();
            GUIWaitCursor.Hide();

            if (!pArgs.playBackOk)
            {
                ifPlaybackFailed(pArgs);
            }
            else
            {
                ifPlayBackSucceded(GetPlayListType(pArgs), pArgs.item);
            }
           
           

        }

        private class PlayArgs
        {
            public string url;
            public string title;
            public double startTime;
            public Item item;
            public bool playBackOk;
        }

        private void PlayMethodDelegate(Object sender, DoWorkEventArgs e)
        {
            Log.Info(string.Format("{0}: PlayMethodDelegate", NrkBrowserConstants.PLUGIN_NAME));
            PlayArgs pArgs = (PlayArgs) e.Argument;

            PlayListType type;
            type = GetPlayListType(pArgs);

            if (useOsdPlayer)
            {
                pArgs.playBackOk = playWithOsd(pArgs.url, pArgs.title, type, pArgs.startTime);
            }
            else
            {
                pArgs.playBackOk = playWithoutOsd(pArgs.url, pArgs.title, type, pArgs.startTime);
            }
        }

        private static PlayListType GetPlayListType(PlayArgs pArgs)
        {
            PlayListType type;
            if (isWMVVideo(pArgs))
            {
                type = PlayListType.PLAYLIST_VIDEO_TEMP;
            }
            else if (isWMAorMP3Audio(pArgs))
            {
                type = PlayListType.PLAYLIST_RADIO_STREAMS;
            }
            else if (isLiveStream(pArgs))
            {
                type = PlayListType.PLAYLIST_VIDEO_TEMP;
            } 
            else
            {
                Log.Info(string.Format("Couldn't determine playlist type for: {0}", pArgs.url));
                type = PlayListType.PLAYLIST_VIDEO_TEMP;
            }
            return type;
        }

        private void ifPlayBackSucceded(PlayListType type, Item item)
        {
            
            if (type == PlayListType.PLAYLIST_VIDEO_TEMP)
            {
                Log.Info(NrkBrowserConstants.PLUGIN_NAME + " Playing OK, switching to fullscreen");
                g_Player.ShowFullScreenWindow();
                g_Player.FullScreen = true;
                
             
                // Update OSD (delayed). RSS/Flash-based need shorter time to load than wmv-based
                Clip clip = (Clip) item;
                if(clip.Type == Clip.KlippType.RSS){
                    new UpdatePlayBackInfo(2000, item);
                }
                else
                {
                    new UpdatePlayBackInfo(10000, item);
                }
                 
            }
        }

        private void ifPlaybackFailed(PlayArgs pArgs)
        {
            string message;
            if (useOsdPlayer)
            {
                message = NrkTranslatableStrings.PLAYBACK_FAILED_TRY_DISABLING_VMR9;
            }
            else
            {
                message = NrkTranslatableStrings.PLAYBACK_FAILED_GENERIC;
            }
            if (pArgs.url.Contains(NrkParserConstants.GEOBLOCK_URL_PART))
            {
                message = NrkTranslatableStrings.PLAYBACK_FAILED_GENERIC;
                message += NrkTranslatableStrings.PLAYBACK_FAILED_GEOBLOCKED_TO_NORWAY;
            }
            ShowMessageBox(message);
            Log.Info(NrkBrowserConstants.PLUGIN_NAME + " Playing failed");
        }

        private static bool isLiveStream(PlayArgs pArgs)
        {
            return pArgs.url.ToLower().EndsWith(NrkBrowserConstants.QUALITY_HIGH_SUFFIX) || pArgs.url.ToLower().EndsWith(NrkBrowserConstants.QUALITY_MEDIUM_SUFFIX) || pArgs.url.ToLower().EndsWith(NrkBrowserConstants.QUALITY_LOW_SUFFIX);
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
            Log.Info(NrkBrowserConstants.PLUGIN_NAME + " Trying to play with Osd (g_Player): " + url);
            Log.Debug("Title is: " + title);
            Log.Debug("Type of clip is: " + type);
            Log.Debug("Starttime of clip is: " + startTime);
            bool playOk;
            if (type == PlayListType.PLAYLIST_VIDEO_TEMP)
            {
                playOk = g_Player.PlayVideoStream(url, title);
                if (startTime > NrkBrowserConstants.MINIMUM_TIME_BEFORE_SEEK)
                {
                    g_Player.SeekAbsolute(startTime);
                }
            }
            else
            {
                playOk = g_Player.PlayAudioStream(url, false);
                if (startTime > NrkBrowserConstants.MINIMUM_TIME_BEFORE_SEEK)
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
            Log.Info(NrkBrowserConstants.PLUGIN_NAME + " Trying to play without Osd (PlayListPlayer): " + url);
            Log.Debug("Title is: " + title);
            Log.Debug("Type of clip is: " + pType);
            Log.Debug("Starttime of clip is: " + startTime);
            bool playIsOk = g_Player.Play(url);

            if (playIsOk && startTime < NrkBrowserConstants.MINIMUM_TIME_BEFORE_SEEK)
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
            GUIDialogNotify dlg =
                (GUIDialogNotify) GUIWindowManager.GetWindow((int) Window.WINDOW_DIALOG_NOTIFY);
            dlg.SetHeading(NrkBrowserConstants.MESSAGE_BOX_HEADER_TEXT);
            dlg.SetText(message);
            dlg.DoModal(GUIWindowManager.ActiveWindow);
        }

        protected override void OnShowContextMenu()
        {
            Item item = (Item) facadeView.SelectedListItem.TVTag;

            GUIDialogMenu dlgMenu = CreateDlgMenu(item);
            dlgMenu.DoModal(GetWindowId());

            HandleInputFromContextMenu(dlgMenu, item);
        }

        private GUIDialogMenu CreateDlgMenu(Item item)
        {
            GUIDialogMenu dlgMenu = (GUIDialogMenu)GUIWindowManager.GetWindow((int)Window.WINDOW_DIALOG_MENU);

            dlgMenu.Reset();
            dlgMenu.SetHeading(NrkBrowserConstants.CONTEXTMENU_HEADER_TEXT);
            if (item is Clip)
            {
                Clip cl = (Clip)item;
                if (cl.TilhoerendeProsjekt > 0)
                {
                    dlgMenu.Add(NrkTranslatableStrings.CONTEXTMENU_ITEM_SE_TIDLIGERE_PROGRAMMER);
                }
            }
            if (item is MenuItem)
            {
                if (erMestSetteEnabled(item))
                {
                    GUIListItem mostWatched = new GUIListItem(String.Format(NrkTranslatableStrings.CONTEXTMENU_ITEM_MOST_WATCHED_FOR, item.Title));
                    dlgMenu.Add(mostWatched);
                }
            }
            if (!activeStack.Contains(favoritter))
            {
                dlgMenu.Add(NrkTranslatableStrings.CONTEXTMENU_ITEM_LEGG_TIL_I_FAVORITTER);
            }
            if (activeStack.Contains(favoritter))
            {
                dlgMenu.Add(NrkTranslatableStrings.CONTEXTMENU_ITEM_FJERN_FAVORITT);
            }
            dlgMenu.Add(NrkTranslatableStrings.CONTEXTMENU_ITEM_BRUK_VALGT_SOM_SOEKEORD);
            dlgMenu.Add(NrkTranslatableStrings.CONTEXTMENU_ITEM_KVALITET);
            dlgMenu.Add(NrkTranslatableStrings.CONTEXTMENU_ITEM_CHECK_FOR_NEW_VERSION);
            return dlgMenu;
        }

        private void HandleInputFromContextMenu(GUIDialogMenu dlgMenu, Item item)
        {
            Log.Info("dlgMenu.SelectedId: " + dlgMenu.SelectedId);
            Log.Info("dlgMenu.SelectedLabelText: " + dlgMenu.SelectedLabelText);
            if (dlgMenu.SelectedLabelText == NrkTranslatableStrings.CONTEXTMENU_ITEM_SE_TIDLIGERE_PROGRAMMER)
            {
                Clip cl = (Clip) item;
                List<Item> items = nrkParser.GetClipsTilhoerendeSammeProgram(cl);
                items.AddRange(nrkParser.GetFolders(cl));
                activeStack.Push(item);
                UpdateList(items);
            }
            else if (dlgMenu.SelectedLabelText == NrkTranslatableStrings.CONTEXTMENU_ITEM_LEGG_TIL_I_FAVORITTER)
            {
                addToFavourites(item);
            }
            else if (dlgMenu.SelectedLabelText == NrkTranslatableStrings.CONTEXTMENU_ITEM_BRUK_VALGT_SOM_SOEKEORD)
            {
                search(item.Title);
            }
            else if (dlgMenu.SelectedLabelText == NrkTranslatableStrings.CONTEXTMENU_ITEM_FJERN_FAVORITT)
            {
                removeFavourite(item);
            }
            else if (dlgMenu.SelectedLabelText == NrkTranslatableStrings.CONTEXTMENU_ITEM_KVALITET)
            {
                openQualityMenu(dlgMenu);
            }
            else if (dlgMenu.SelectedLabelText == String.Format(NrkTranslatableStrings.CONTEXTMENU_ITEM_MOST_WATCHED_FOR, item.Title))
            {
                openMostWatchedMenu(dlgMenu, item);
            }
            else if (dlgMenu.SelectedLabelText == NrkTranslatableStrings.CONTEXTMENU_ITEM_CHECK_FOR_NEW_VERSION)
            {
                CheckForNewVersionAndDisplayResultInMessageBox();
            }
        }

        private void openMostWatchedMenu(GUIDialogMenu dlgMenu, Item item)
        {
            dlgMenu.Reset();
            dlgMenu.SetHeading(string.Format(NrkTranslatableStrings.CONTEXTMENU_ITEM_MOST_WATCHED_FOR, item.Title));
            GUIListItem sisteUke = new GUIListItem(NrkTranslatableStrings.CONTEXT_MENU_ITEM_MOST_WATCHED_FOR_LAST_WEEK);
            sisteUke.ItemId = 1;
            GUIListItem sisteManed = new GUIListItem(NrkTranslatableStrings.CONTEXT_MENU_ITEM_MOST_WATCHED_FOR_LAST_MONTH);
            sisteManed.ItemId = 2;
            GUIListItem totalt = new GUIListItem(NrkTranslatableStrings.CONTEXT_MENU_ITEM_MOST_WATCHED_FOR_TOTAL);
            totalt.ItemId = 3;
            dlgMenu.Add(sisteUke);
            dlgMenu.Add(sisteManed);
            dlgMenu.Add(totalt);
            dlgMenu.DoModal(GetWindowId());
            if (dlgMenu.SelectedId == sisteUke.ItemId)
            {
                AddMostWatchedForPeriodToList(NrkParser.Periode.Uke, item);
            }
            else if (dlgMenu.SelectedId == sisteManed.ItemId)
            {
                AddMostWatchedForPeriodToList(NrkParser.Periode.Maned, item);
            }
            else if (dlgMenu.SelectedId == totalt.ItemId)
            {
                AddMostWatchedForPeriodToList(NrkParser.Periode.Totalt, item);
            }
        }

        private void AddMostWatchedForPeriodToList(NrkParser.Periode periode, Item item)
        {
            List<Item> tItems = nrkParser.GetMestSetteForKategoriOgPeriode(periode, item.ID);
            tItems.ForEach(AddDescriptionToMostWatched);
            activeStack.Push(item);
            UpdateList(tItems);
        }

        private static void AddDescriptionToMostWatched(Item titem)
        {
            Clip c = (Clip)titem;
            string descriptionPart1 = string.Format(NrkTranslatableStrings.DESCRIPTION_CLIP_ADDED, c.Klokkeslett);
            string descriptionPart2 = string.Format(NrkTranslatableStrings.DESCRIPTION_CLIP_SHOWN_TIMES, c.AntallGangerVist);
            titem.Description = string.Format("{0}. {1}", descriptionPart1, descriptionPart2);
        }

        private void CheckForNewVersionAndDisplayResultInMessageBox()
        {
            String nyVer = String.Empty;
            String thisVersion = getVersion();
            if (VersionChecker.newVersionAvailable(ref nyVer))
            {
                ShowMessageBox(string.Format(NrkTranslatableStrings.NEW_VERSION_IS_AVAILABLE, thisVersion, nyVer));
            }
            else
            {
                ShowMessageBox(string.Format(NrkTranslatableStrings.NEW_VERSION_IS_NOT_AVAILABLE, thisVersion));
            }
        }



        private bool erMestSetteEnabled(Item item)
        {
            return
                item.ID.Equals(NrkBrowserConstants.MENU_ITEM_ID_NYHETER) || item.ID.Equals(NrkBrowserConstants.MENU_ITEM_ID_SPORT) || item.ID.Equals(NrkBrowserConstants.MENU_ITEM_ID_NATUR);
        }

        protected void openQualityMenu(GUIDialogMenu dlgMenu)
        {
            dlgMenu.Reset();
            dlgMenu.SetHeading(NrkTranslatableStrings.QUALITY_MENU_HEADER);
            GUIListItem lowQuality = new GUIListItem(NrkTranslatableStrings.QUALITY_MENU_LOW_QUALITY);
            lowQuality.ItemId = 1;
            GUIListItem mediumQuality = new GUIListItem(NrkTranslatableStrings.QUALITY_MENU_MEDIUM_QUALITY);
            mediumQuality.ItemId = 2;
            GUIListItem highQuality = new GUIListItem(NrkTranslatableStrings.QUALITY_MENU_HIGH_QUALITY);
            highQuality.ItemId = 3;
            dlgMenu.Add(lowQuality);
            dlgMenu.Add(mediumQuality);
            dlgMenu.Add(highQuality);
            dlgMenu.DoModal(GetWindowId());
            if (dlgMenu.SelectedId == lowQuality.ItemId)
            {
                int speed = NrkBrowserConstants.PREDEFINED_LOW_SPEED;
                Log.Info(string.Format("{0}: Changing bitrate to {1}", NrkBrowserConstants.PLUGIN_NAME, speed));
                nrkParser = new NrkParser(speed, new MPLogger());
            }
            else if (dlgMenu.SelectedId == mediumQuality.ItemId)
            {
                int speed = NrkBrowserConstants.PREDEFINED_MEDIUM_SPEED;
                Log.Info(string.Format("{0}: Changing bitrate to {1}", NrkBrowserConstants.PLUGIN_NAME, speed));
                nrkParser = new NrkParser(speed, new MPLogger());
            }
            else if (dlgMenu.SelectedId == highQuality.ItemId)
            {
                int speed = NrkBrowserConstants.PREDEFINED_HIGH_SPEED;
                Log.Info(string.Format("{0}: Changing bitrate to {1}", NrkBrowserConstants.PLUGIN_NAME, speed));
                nrkParser = new NrkParser(speed, new MPLogger());
            }
        }

        /// <summary>
        /// Adds an item to favourites
        /// </summary>
        /// <param name="item"></param>
        private void addToFavourites(Item item)
        {
            Log.Debug(NrkBrowserConstants.PLUGIN_NAME + ": addToFavourites: " + item);
            FavoritesUtil db = FavoritesUtil.getDatabase(null);
            if (item is Clip)
            {
                Clip c = (Clip) item;
                String message = "";
                if (!db.addFavoriteVideo(c, ref message))
                {
                    ShowMessageBox(string.Format(NrkTranslatableStrings.FAVOURITES_COULD_NOT_BE_ADDED, message));
                }
            }
            else if (item is Program)
            {
                Program p = (Program) item;
                String message = "";
                if (!db.addFavoriteProgram(p, ref message))
                {
                    ShowMessageBox(string.Format(NrkTranslatableStrings.FAVOURITES_COULD_NOT_BE_ADDED, message));
                }
            }
            else
            {
                ShowMessageBox(NrkTranslatableStrings.FAVOURITES_UNSUPPORTED_TYPE);
            }
        }

        /// <summary>
        /// Removes an item from favourites
        /// </summary>
        /// <param name="item"></param>
        private void removeFavourite(Item item)
        {
            Log.Debug(NrkBrowserConstants.PLUGIN_NAME + ": removeFavourite: " + item);
            FavoritesUtil db = FavoritesUtil.getDatabase(null);
            bool removedSuccessFully = false;
            if (item is Clip)
            {
                removedSuccessFully = db.removeFavoriteVideo((Clip) item);
            }
            else if (item is Program)
            {
                removedSuccessFully = db.removeFavoriteProgram((Program) item);
            }
            else
            {
                Log.Error("Item to remove from favourite is neither Clip nor Program, this should never happen.");
            }

            if (!removedSuccessFully)
            {
                ShowMessageBox(NrkTranslatableStrings.FAVOURITES_COULD_NOT_BE_REMOVED);
            }
            else
            {
                //must remove removed item from list..do so by refreshing the whole favouriteslist
                FillStackWithFavourites();
            }
        }
    }
}