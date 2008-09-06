/*
 * Copyright (c) 2008 Terje Wiesener <wiesener@samfundet.no>
 * 
 * Loosely based on an anonymous (and slightly outdated) NRK parser in python for Myth-tv, 
 * please email me if you are the author :)
 * 
 * */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Windows.Forms;
using MediaPortal.Configuration;
using MediaPortal.Dialogs;
using MediaPortal.GUI.Library;
using MediaPortal.Player;
using MediaPortal.Playlists;
using MediaPortal.Profile;
using NrkBrowser;

namespace NrkBrowser
{
    public class NrkPlugin : GUIWindow, ISetupForm
    {
        public const string PLUGIN_NAME = "NrkBrowser";
        public const string PROGRAM_PICTURE = "#picture";
        public const string PROGRAM_DESCRIPTION = "#description";
        private string streamPrefix = "mms://straumV.nrk.no/nrk_tv_webvid";
        protected string _suffix = "_l";

        [SkinControlAttribute(50)] protected GUIFacadeControl facadeView = null;

        protected NrkParser _nrk = null;
        protected Stack<Item> _active = null;

        protected bool _osdPlayer = true;


        private static string configfile = "NrkBrowserSettings.xml";

        public NrkPlugin()
        {
            GetID = 40918376;
        }

        #region ISetupForm Members

        public string PluginName()
        {
            return "My NRK";
        }

        public string Description()
        {
            return "Plugin for watching NRK nett-tv";
        }

        public string Author()
        {
            return "Terje Wiesener <wiesener@samfundet.no>";
        }

        public void ShowPlugin()
        {
            //configuration
            SettingsForm form = new SettingsForm();
            Settings settings = new Settings(Config.GetFile(Config.Dir.Config, configfile));

            int speed = settings.GetValueAsInt("NrkBrowser", "speed", 2048);
            if (speed < form.speedUpDown.Minimum)
            {
                speed = (int) form.speedUpDown.Minimum;
                settings.SetValue("NrkBrowser", "speed", speed);
            }
            if (speed > form.speedUpDown.Maximum)
            {
                speed = (int) form.speedUpDown.Maximum;
                settings.SetValue("NrkBrowser", "speed", speed);
            }
            form.speedUpDown.Value = speed;

            bool osd = settings.GetValueAsBool("NrkBrowser", "osdPlayer", false);
            form.osdPlayerCheckbox.Checked = osd;

            string quality = settings.GetValueAsString("NrkBrowser", "liveStreamQuality", "Low");
            form.liveStreamQualityCombo.SelectedIndex = form.liveStreamQualityCombo.Items.IndexOf(quality);

            DialogResult res = form.ShowDialog();
            if (res == DialogResult.OK)
            {
                speed = (int) form.speedUpDown.Value;
                settings.SetValue("NrkBrowser", "speed", speed);
                osd = form.osdPlayerCheckbox.Checked;
                settings.SetValueAsBool("NrkBrowser", "osdPlayer", osd);
                quality = (string) form.liveStreamQualityCombo.Items[form.liveStreamQualityCombo.SelectedIndex];
                settings.SetValue("NrkBrowser", "liveStreamQuality", quality);
            }
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
            strPictureImage = GUIGraphicsContext.Skin + @"\media\hover_my_nrk.png";
            return true;
        }

        #endregion

        public override bool Init()
        {
            bool result = Load(GUIGraphicsContext.Skin + @"\NrkBrowser.xml");
            Settings settings = new Settings(Config.GetFile(Config.Dir.Config, configfile));
            int speed = settings.GetValueAsInt("NrkBrowser", "speed", 2048);
            _nrk = new NrkParser(speed);
            _active = new Stack<Item>();
            _osdPlayer = settings.GetValueAsBool("NrkBrowser", "osdPlayer", false);
            String quality = settings.GetValueAsString("NrkBrowser", "liveStreamQuality", "Low");

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
            if (_active.Count > 0) Activate(_active.Pop());
            else Activate(null);
        }

        public override void Process()
        {
            Item selecteditem = (Item) facadeView.SelectedListItem.TVTag;

            GUIPropertyManager.SetProperty(PROGRAM_DESCRIPTION, selecteditem.Description);

            if (selecteditem.Description == null || selecteditem.Description.Equals(""))
            {
                GUIPropertyManager.SetProperty(PROGRAM_DESCRIPTION, " ");
            }

            if (selecteditem.ID == "all" || selecteditem.ID == "categories" || selecteditem.ID == "anbefalte" ||
                selecteditem.ID == "live" || selecteditem.ID == "mestSettUke" || selecteditem is Category)
            {
                GUIPropertyManager.SetProperty(PROGRAM_PICTURE, "");
            }

            if (selecteditem != null)
            {
                if (!selecteditem.Bilde.Equals(""))
                {
                    GUIPropertyManager.SetProperty(PROGRAM_PICTURE, selecteditem.Bilde);
                }
            }
            base.Process();
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


        protected void Activate(Item item)
        {
            GUIPropertyManager.SetProperty("#itemcount", "");
            if (item == null)
            {
                List<Item> items = new List<Item>();
                items.Add(new MenuItem("all", "Alfabetisk liste"));
                items.Add(new MenuItem("categories", "Kategorier"));
                items.Add(new MenuItem("live", "Direktestrømmer"));
                NrkBrowser.MenuItem anbefalte = new NrkBrowser.MenuItem("anbefalte", "Anbefalte programmer");
                anbefalte.Description = "Anbefalte programmer fra forsiden akkurat nå";
                items.Add(anbefalte);
                NrkBrowser.MenuItem mestSettUke = new NrkBrowser.MenuItem("mestSettUke", "Mest sett denne uken");
                mestSettUke.Description = "De mest populære klippene denne uken!";
                items.Add(mestSettUke);

                NrkBrowser.MenuItem nyheter = new NrkBrowser.MenuItem("nyheter", "Nyheter");
                nyheter.Description = "De siste nyhetsklippene";
                nyheter.Bilde = "nrknyheter.jpg";
                items.Add(nyheter);

                NrkBrowser.MenuItem sport = new NrkBrowser.MenuItem("sport", "Sport");
                sport.Description = "De siste sportsklippene";
                sport.Bilde = "nrksport.jpg";
                items.Add(sport);

                NrkBrowser.MenuItem natur = new NrkBrowser.MenuItem("natur", "Natur");
                natur.Description = "De siste naturklippene";
                natur.Bilde = "nrknatur.jpg";
                items.Add(natur);

                NrkBrowser.MenuItem super = new NrkBrowser.MenuItem("super", "Super");
                super.Description = "De siste klippene fra super";
                super.Bilde = "nrksuper.jpg";
                items.Add(super);

                NrkBrowser.MenuItem ol = new NrkBrowser.MenuItem("ol", "OL Beijing 2008");
                ol.Description = "De siste klippene fra OL i Beijing";
                ol.Bilde = "nrkbeijing.jpg";
                items.Add(ol);

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

            _active.Push(item);

            if (item.ID == "anbefalte")
            {
                List<Item> items = _nrk.GetForsiden();
                UpdateList(items);
                return;
            }

            if (item.ID == "mestSettUke")
            {
                List<Item> items = _nrk.GetMestSettDenneUken();
                UpdateList(items);
                return;
            }

            if (item.ID == "nyheter")
            {
                List<Item> items = _nrk.GetTopTabber("nyheter");
                UpdateList(items);
                return;
            }

            if (item.ID == "sport")
            {
                List<Item> items = _nrk.GetTopTabber("sport");
                UpdateList(items);
                return;
            }
            if (item.ID == "natur")
            {
                List<Item> items = _nrk.GetTopTabRSS("natur");
                UpdateList(items);
                return;
            }

            if (item.ID == "super")
            {
                List<Item> items = _nrk.GetTopTabRSS("super");
                UpdateList(items);
                return;
            }
            if (item.ID == "ol")
            {
                List<Item> items = _nrk.GetTopTabber("ol");
                UpdateList(items);
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
                List<Item> items = new List<Item>();
                items.Add(new Stream(streamPrefix + "03" + _suffix, "NRK 1"));
                items.Add(new Stream(streamPrefix + "04" + _suffix, "NRK 2"));
                items.Add(new Stream(streamPrefix + "05" + _suffix, "NRK Alltid Nyheter"));
                items.Add(new Stream(streamPrefix + "08" + _suffix, "Testkanal (innhold varierer)"));
                items.Add(new MenuItem("liveall", "Velg strøm manuelt"));
                UpdateList(items);
                return;
            }

            if (item.ID == "liveall")
            {
                List<Item> items = new List<Item>();
                for (int i = 0; i < 10; i++)
                {
                    items.Add(new Stream(streamPrefix + i.ToString("D2") + _suffix, "Strøm " + i));
                }
                UpdateList(items);
                return;
            }

            if (item is Category)
            {
                UpdateList(_nrk.GetPrograms((Category) item));
            }
            if (item is Program)
            {
                List<Item> items = _nrk.GetClips((Program) item);
                items.AddRange(_nrk.GetFolders((Program) item));
                UpdateList(items);
            }
            if (item is Folder)
            {
                List<Item> items = _nrk.GetClips((Folder) item);
                items.AddRange(_nrk.GetFolders((Folder) item));
                UpdateList(items);
                GUIPropertyManager.SetProperty("#itemcount", String.Format("{0} Klipp", items.Count));
            }
        }

        protected void PlayClip(Clip item)
        {
            string url = _nrk.GetClipUrl(item);
            Log.Info(PLUGIN_NAME + " PlayClip " + url);
            
            
            if (item.Type == Clip.KlippType.RSS)
            {
                Log.Info(PLUGIN_NAME + " TYPE IS NATUR, PLAYING WITH MPLAYER");
                //play with mplayer 
                PlayUrlWithMplayer(url, item.Title);
            }
            else
            {
                PlayUrl(url, item.Title);
            }
        }

        protected void PlayStream(Stream item)
        {
            Log.Info(PLUGIN_NAME + " PlayStream " + item.ID);
            PlayUrl(item.ID, item.Title);
        }

        protected void PlayUrlWithMplayer(string url, string title)
        {
            Log.Info(string.Format("{0}: PlayUrlWithMplayer(url, title): {1}, {2}", PLUGIN_NAME, url, title));
            //&txe=.flv is needed by .mplayer for some reason, .mplayer ensures that mplayer plugin plays it
            bool playOk = g_Player.Play(url + "&txe=.flv.mplayer");

            if (!playOk)
            {
                ShowError("Avspilling feilet");
            }
            else
            {
                Log.Info(PLUGIN_NAME + " Playing OK, switching to fullscreen");
                g_Player.ShowFullScreenWindow();
                g_Player.FullScreen = true;
            }
        }

        protected void PlayUrl(string url, string title)
        {
            Log.Info(PLUGIN_NAME + "PlayUrl");

            PlayListType type;
            if (url.EndsWith(".wmv")) type = PlayListType.PLAYLIST_VIDEO_TEMP;
            else if (url.EndsWith(".wma")) type = PlayListType.PLAYLIST_RADIO_STREAMS;
                //FIXME: Hangs on some audio streams...
            else if (url.EndsWith("_h")) type = PlayListType.PLAYLIST_VIDEO_TEMP; //det er en av live streamene
            else
            {
                //TODO: had to remove this because flash files doesn't have an extension
//                Log.Info(PLUGIN_NAME + " Unknown clip type " + url);
//                ShowError("Unknown clip type, please contact author");
//                return;
                Log.Info(PLUGIN_NAME + ": Unknown clip type, set as video");
                type = PlayListType.PLAYLIST_VIDEO_TEMP;
            }

            bool playOk = false;

            if (_osdPlayer)
            {
                playOk = playWithOsd(url, title, type);
            }
            else
            {
                playOk = playWithoutOsd(url, title, type);
            }

            if (!playOk)
            {
                if (_osdPlayer)
                {
                    ShowError("Avspilling feilet, prøv å slå av osd player/vmr 9 for webstreams");
                }
                else
                {
                    ShowError("Avspilling feilet");
                }

                Log.Info(PLUGIN_NAME + " Playing failed");
            }
            else
            {
                if (type == PlayListType.PLAYLIST_VIDEO_TEMP)
                {
                    Log.Info(PLUGIN_NAME + " Playing OK, switching to fullscreen");
                    g_Player.ShowFullScreenWindow();
                    g_Player.FullScreen = true;
                }
            }
        }

        private bool playWithOsd(String url, String title, PlayListType type)
        {
            //g_Player.Init();
            Log.Info(PLUGIN_NAME + " Trying to play with Osd (g_Player): " + url);
            bool playOk = false;
            if (type == PlayListType.PLAYLIST_VIDEO_TEMP)
            {
                playOk = g_Player.PlayVideoStream(url, title);
            }
            else
            {
                playOk = g_Player.PlayAudioStream(url, false);
            }
            return playOk;
        }

        private bool playWithoutOsd(String url, String title, PlayListType type)
        {
            Log.Info(PLUGIN_NAME + " Trying to play without Osd (PlayListPlayer): " + url);
            PlayListPlayer playlistPlayer = PlayListPlayer.SingletonPlayer;
            playlistPlayer.RepeatPlaylist = false;
            PlayList playlist = playlistPlayer.GetPlaylist(type);
            playlist.Clear();
            PlayListItem toPlay = new PlayListItem(title, url);
            playlist.Add(toPlay);
            playlistPlayer.CurrentPlaylistType = type;
            return playlistPlayer.Play(0);
        }


        private void ShowError(string message)
        {
            //  Log.Info(PLUGIN_NAME + "Showing error: " + message);
            GUIDialogNotify dlg =
                (GUIDialogNotify) GUIWindowManager.GetWindow((int) Window.WINDOW_DIALOG_NOTIFY);
            dlg.SetHeading("NrkBrowser Browser");
            dlg.SetText(message);
            dlg.DoModal(GUIWindowManager.ActiveWindow);
        }
    }
}