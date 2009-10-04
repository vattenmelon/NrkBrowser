/*
 * Copyright (c) 2008 Terje Wiesener <wiesener@samfundet.no>
 * 
 * Loosely based on an anonymous (and slightly outdated) NRK parser in python for Myth-tv, 
 * please email me if you are the author :)
 * 
 * 2008-2009 Modified by Vattenmelon
 * 
 * */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MediaPortal.GUI.Library;
using NrkBrowser.Domain;
using NrkBrowser.Xml;

namespace NrkBrowser
{
    public class NrkParser
    {
        public const string RSS_CLIPURL_PREFIX = "http://pd.nrk.no/nett-tv-stream/stream.ashx?id=";
        private static string STORY_URL = "http://www1.nrk.no/nett-tv/spilleliste.ashx?story=";
        private static string BASE_URL = "http://www1.nrk.no/";
        private static string MAIN_URL = BASE_URL + "nett-tv/";
        private static string CATEGORY_URL = MAIN_URL + "tema/";
        private static string PROGRAM_URL = MAIN_URL + "prosjekt/";
        private static string CLIP_URL = MAIN_URL + "klipp/";
        private static string DIREKTE_URL = MAIN_URL + "direkte/";
        private static string FOLDER_URL = MAIN_URL + "kategori/";
        

        private static string SOK_URL_BEFORE =
            "http://www.nrk.no/nett-tv/DynamiskLaster.aspx?SearchResultList$search:{0}|sort:dato|page:{1}";

        private static string INDEX_URL = MAIN_URL + "indeks/";


        private CookieContainer _jar;
        private int _speed;

        public NrkParser(int speed)
        {
            _jar = new CookieContainer();
            this.Speed = speed;
        }

        public int Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                FetchUrl(MAIN_URL + "hastighet.aspx?hastighet=" + _speed + "&retururl=http://www1.nrk.no/nett-tv/");
            }
        }

        public List<Item> GetSearchHits(String keyword, int page)
        {
            String urlToFetch = String.Format(SOK_URL_BEFORE, keyword, page + 1);
            string data = FetchUrl(urlToFetch);
            Search search = new Search(data);
            return search.SearchHits;
        }

        

        public List<Item> GetCategories()
        {
            List<Item> categories = new List<Item>();
            string data = FetchUrl(MAIN_URL);
            Regex query = new Regex("<a href=\"/nett-tv/tema/(\\w*).*?>(.*?)</a>");
            MatchCollection result = query.Matches(data);

            foreach (Match x in result)
            {
              categories.Add(new Category(x.Groups[1].Value, x.Groups[2].Value));
            }
            return categories;
        }

        public List<Item> GetAnbefaltePaaForsiden()
        {
            Log.Info(NrkConstants.PLUGIN_NAME + ": GetAnbefaltePaaForsiden()");
            string data;
            data = FetchUrl(MAIN_URL);

            Regex query =
                new Regex(
                    "<div class=\"img-left\" style=\"width: 100px;\">.*?<a href=\".*?\" title=\".*?\"><img src=\"(.*?)\" width=\"100\" height=\"57\" alt=\".*?\" title=\".*?\".*?></a>.*?</div>.*?<div class=\"intro-element-content\"><h2><a href=\"/nett-tv/klipp/(.*?)\" title=\"(.*?)\">(.*?)</a></h2>.*?<a href=\"/nett-tv/prosjekt/(.*?)\">",
                    RegexOptions.Singleline);
            MatchCollection matches = query.Matches(data);
            List<Item> clips = new List<Item>();

            Log.Info(NrkConstants.PLUGIN_NAME + ": Matches {0}", matches.Count);
            foreach (Match x in matches)
            {
                Clip c = new Clip(x.Groups[2].Value, x.Groups[4].Value);
                c.TilhoerendeProsjekt = Int32.Parse(x.Groups[5].Value);
                c.Description = x.Groups[3].Value;
                c.Bilde = x.Groups[1].Value;
                clips.Add(c);
            }

            return clips;
        }

        public List<Item> GetMestSette(int dager)
        {
            Log.Info(string.Format("{0}: GetMestSettDenneMaaneden()", NrkConstants.PLUGIN_NAME));
            string data;
            String url = String.Format("http://www1.nrk.no/nett-tv/ml/topp12.aspx?dager={0}&_=", dager);
            data = FetchUrl(url);
            Console.WriteLine(data);
            //"<a href=\".*?/nett-tv/klipp/.*?\" title=\".*?\"><img src=\"(.*?)\" .*? /></a></div><h2><a href=\".*?/nett-tv/klipp/.*?\" title=\".*?\">.*?</a></h2><p><a href=\".*?/nett-tv/klipp/(.*?)\" title=\"(.*?)\">Vist (.*?) ganger.</a></p>",
            Regex query =
                new Regex(
                    "<a href=\".*?/nett-tv/.*?/.*?\" title=\".*?\"><img src=\"(.*?)\" .*? /></a></div><h2><a href=\".*?/nett-tv/.*?/.*?\" title=\".*?\">.*?</a></h2><p><a href=\".*?/nett-tv/(.*?)/(.*?)\" title=\"(.*?)\">Vist (.*?) ganger.</a></p>",
                    RegexOptions.Singleline);
            MatchCollection matches = query.Matches(data);
            List<Item> clips = new List<Item>();

            Log.Info(NrkConstants.PLUGIN_NAME + ": Matches {0}", matches.Count);
            Console.WriteLine("matches: " + matches.Count);
            foreach (Match x in matches)
            {
                Clip c = new Clip(x.Groups[3].Value, x.Groups[4].Value);
                c.AntallGangerVist = x.Groups[5].Value;
                c.Description = String.Format("Klipp vist {0} ganger denne uken.", c.AntallGangerVist);
                c.Bilde = x.Groups[1].Value;
                if (x.Groups[2].Value.Trim().ToLower().Equals("indeks"))
                {
                    c.Type = Clip.KlippType.INDEX;
                }
                clips.Add(c);
            }

            return clips;
        }



        public List<Item> GetTopTabber()
        {
            Log.Info(NrkConstants.PLUGIN_NAME + ": GetTopTabber()");
            string data1;
            data1 = FetchUrl(MAIN_URL);
            //narrow it down
            Regex query =
                new Regex(
                    "<div id=\"tabs-menu-container\" class=\"no-menu-container\">.*?<div id=\"tabs-menu\">.*?<ul id=\"ctl00_ucTop_tabs\">(.*?)</ul>.*?<div class=\"nettv-search\">.*?</div>.*?</div>",
                    RegexOptions.Singleline);
            MatchCollection matches1 = query.Matches(data1);
            string data = matches1[0].Groups[1].Value;
            query =
                new Regex(
                    "<li.*?><a href=\"/nett-tv/(.*?)\" title=\".*?\"><span>(.*?)</span></a></li>",
                    RegexOptions.Singleline);
            MatchCollection matches = query.Matches(data);
            List<Item> items = new List<Item>();
            Log.Info(NrkConstants.PLUGIN_NAME + ": Matches {0}", matches.Count);
            foreach (Match x in matches)
            {
                string title = x.Groups[2].Value;
                AutogeneratedMenuItem item = new AutogeneratedMenuItem(x.Groups[1].Value, title);
                item.Bilde = NrkConstants.DEFAULT_PICTURE;
                item.Description = string.Format("Siste klipp fra fra {0}", title);
                items.Add(item);
            }
            
            return items;
        }

        public List<Item> GetTopTabRSS(string site)
        {
            XmlRSSParser parser = new XmlRSSParser(NrkConstants.RSS_URL, site);
            return parser.getClips();
        }

        

        /**
         * Metode som sjekker om clippet er en vignett
         */

        public static bool isNotShortVignett(Clip loRssItem)
        {
            //372980 is the id for the short natur-vignett, and we do not want it in list
            //381994 is the for the super vignett
            //410330 is the nyheter vignett
            //410335 is the sport vignett
            return
                loRssItem.ID != "372980" && loRssItem.ID != "381994" && loRssItem.ID != "410330" &&
                loRssItem.ID != "410335";
        }

        public List<Item> GetDirektePage(String tab)
        {
            Log.Info(NrkConstants.PLUGIN_NAME + ": GetDirektePage(String)", tab);
            string data;
            data = FetchUrl(MAIN_URL + tab + "/");
            Regex query =
                new Regex(
                    "<div class=\"img-left\">.*?<a href=\"/nett-tv/direkte/(.*?)\" title=\"(.*?)\"><img class=\"info\" src=\".*?\" alt=\"Direkte\" /><img src=\"(.*?)\" .*? /></a>.*?</div>",
                    RegexOptions.Singleline);
            

            MatchCollection matches = query.Matches(data);
            List<Item> clips = new List<Item>();
            Log.Info(NrkConstants.PLUGIN_NAME + ": Matches {0}", matches.Count);
            foreach (Match x in matches)
            {
                Clip c = new Clip(x.Groups[1].Value, x.Groups[2].Value);
                String bildeUrl = x.Groups[3].Value;
                c.Bilde = bildeUrl;
                c.Type = Clip.KlippType.DIREKTE;
                c.VerdiLink = tab;
                clips.Add(c);
            }

            return clips;
        }

        public List<Item> GetTopTabContent(String tab)
        {
            Log.Info(NrkConstants.PLUGIN_NAME + ": GetTopTabContent(String)", tab);
            string data;
            data = FetchUrl(MAIN_URL + tab + "/");
            Regex query =
                new Regex(
                    "<div class=\"img-left\" style=\"width: 120px;\"><a href=\".*?/nett-tv/" + tab + "/spill/verdi/(.*?)\" .*?><img src=\"(.*?)\" alt=\".*?\" title=\"(.*?)\" .*? />.*?</a>.*?</div>",
                    RegexOptions.Singleline);


            MatchCollection matches = query.Matches(data);
            List<Item> clips = new List<Item>();
            Log.Info(NrkConstants.PLUGIN_NAME + ": Matches {0}", matches.Count);
            foreach (Match x in matches)
            {
                Clip c = new Clip(x.Groups[1].Value, x.Groups[3].Value);
                String bildeUrl = x.Groups[2].Value;
                c.Bilde = bildeUrl;
                if (c.Title.StartsWith("–"))
                {
                    c.Title = c.Title.Replace("–", "-"); //den første er en raring av en minusstrek..
                }
                c.Type = Clip.KlippType.VERDI;
                try
                {
                    c.Description = "Klipp lagt ut " + NrkUtils.parseKlokkeSlettFraBilde(bildeUrl);
                }
                catch(Exception e)
                {
                    
                    Log.Warn(string.Format("Parsing time out of picture file failed, but this is ok... File: {0}. Error: {1}", bildeUrl, e.Message));
                }
                c.VerdiLink = tab;
                clips.Add(c);
            }

            return clips;
        }

        public List<Item> GetPrograms(Category category)
        {
            string data;

            data = FetchUrl(CATEGORY_URL + category.ID);

            Regex queryBilder =
                new Regex("<a href=\"/nett-tv/prosjekt/(.*?)\" id=\".*?\" title=\"(.*?)\".*?>\\s+<img src=\"(.*?)\" id=");
            MatchCollection matchesBilder = queryBilder.Matches(data);
            List<string> bilder = new List<string>();
            foreach (Match x in matchesBilder)
            {
                bilder.Add(x.Groups[3].Value);
            }

            Regex query =
                new Regex(
                    "<a href=\"/nett-tv/prosjekt/(.*?)\" id=\".*ClipLink\" title=\"(.*?)\".*? onclick=\".*?\">(.*?)</a>");
            MatchCollection matches = query.Matches(data);
            List<Item> programs = new List<Item>();
            int bildeTeller = 0;
            foreach (Match x in matches)
            {
                programs.Add(new Program(x.Groups[1].Value, x.Groups[3].Value, "", bilder[bildeTeller]));
                bildeTeller++;
            }

            Regex query2 =
                new Regex("<a href=\"/nett-tv/prosjekt/(.*?)\" id=\".*ClipTitle\" title=\"(.*?)\".*?>(.*?)</a>");
            MatchCollection matches2 = query2.Matches(data);
            int teller = 0;
            foreach (Match x in matches2)
            {
                programs[teller].Description = x.Groups[3].Value;
                teller++;
            }
            return programs;
        }

        public List<Item> GetAllPrograms() //all programs
        {
            string data = FetchUrl(MAIN_URL + "bokstav/@");
            Regex query =
                new Regex(
                    "<div class=\"intro-element intro-element-small\">.*?<div.*?>.*?<a href=\"/nett-tv/prosjekt/(.*?)\" id=\".*?\" title=\"(.*?)\".*?>.*?<img src=\"(.*?)\".*?>.*?</a>.*?</div>.*?<h2>.*?<a href=\".*?\" id=\".*?\" title=\".*?\" onclick=\".*?\">(.*?)</a>.*?</h2>.*?<p>.*?</p>.*?</div>",
                    RegexOptions.Singleline);
            MatchCollection matches = query.Matches(data);
            List<Item> programs = new List<Item>();
            foreach (Match x in matches)
            {
                programs.Add(new Program(x.Groups[1].Value, x.Groups[4].Value, x.Groups[2].Value, x.Groups[3].Value));
            }
            return programs;
        }

        private List<Item> GetFolders(int id)
        {
            string data = FetchUrl(PROGRAM_URL + id);
            Regex query =
                new Regex(
                    "<a id=\".*?\" href=\"/nett-tv/kategori/(.*?)\".*?title=\"(.*?)\".*?class=\"icon-(.*?)-black\".*?>(.*?)</a>");
            MatchCollection matches = query.Matches(data);
            List<Item> folders = new List<Item>();
            foreach (Match x in matches)
            {
                folders.Add(new Folder(x.Groups[1].Value, x.Groups[2].Value));
            }
            return folders;
        }

/*        public List<Item> GetFolders<T>(T notPlayableItem) where T : NotPlayable
        {
            return GetFolders(int.Parse(notPlayableItem.ID))
        }
        */
        public List<Item> GetFolders(Program program)
        {
            return GetFolders(int.Parse(program.ID));
        }

        public List<Item> GetFolders(Folder folder)
        {
            return GetFolders(int.Parse(folder.ID));
        }

        public List<Item> GetFolders(Clip cl)
        {
            return GetFolders(cl.TilhoerendeProsjekt);
        }


        private List<Item> GetClips(int id)
        {
            Log.Info("{0}: GetClips(int): {1}", NrkConstants.PLUGIN_NAME, id);
            string data = FetchUrl(PROGRAM_URL + id);
            List<Item> clips = new List<Item>();
            Regex query =
                new Regex("<a href=\"/nett-tv/klipp/(.*?)\"\\s+title=\"(.*?)\"\\s+class=\"(.*?)\".*?>(.*?)</a>");
            MatchCollection matches = query.Matches(data);
            foreach (Match x in matches)
            {
                clips.Add(new Clip(x.Groups[1].Value, x.Groups[4].Value));
            }

            return clips;
        }
        public List<Item> GetClips(Program program)
        {
            Log.Info("{0}: GetClips(Program): {1}", NrkConstants.PLUGIN_NAME, program);
            return GetClips(Int32.Parse(program.ID));
        }

        public List<Item> GetClipsTilhoerendeSammeProgram(Clip c)
        {
            return GetClips(c.TilhoerendeProsjekt);
        }

        public List<Item> GetClips(Folder folder)
        {
            string data = FetchUrl(FOLDER_URL + folder.ID);

            List<Item> clips = new List<Item>();

            Regex query =
                new Regex("<a href=\"/nett-tv/klipp/(.*?)\"\\s+title=\"(.*?)\"\\s+class=\"(.*?)\".*?>(.*?)</a>");
            MatchCollection matches = query.Matches(data);
            foreach (Match x in matches)
            {
                clips.Add(new Clip(x.Groups[1].Value, x.Groups[4].Value));
            }

            return clips;
        }

        public string GetClipUrl(Clip clip)
        {
            Log.Debug("{0}: GetClipUrl(Clip): {1}", NrkConstants.PLUGIN_NAME, clip);

            if (clip.Type == Clip.KlippType.KLIPP)
            {
                return GetClipUrlAndPutStartTimeForKlipp(clip);
            }
            else if (clip.Type == Clip.KlippType.DIREKTE)
            {
                return GetClipUrlForDirekte(clip);
            }
            else if (clip.Type == Clip.KlippType.RSS)
            {
                return GetClipUrlForRSS(clip);
            }
            else if (clip.Type == Clip.KlippType.INDEX)
            {
                return GetClipUrlForIndex(clip);
            }
            else
            {
                try
                {
                    return GetClipUrlForVerdi(clip);
                }
                catch(Exception e)
                {
                    Log.Error("Kunne ikke finne url til klipp", e);
                    return null;
                }
            }
        }

        private string GetClipUrlForDirekte(Clip clip)
        {
            Log.Debug(NrkConstants.PLUGIN_NAME + ": Clip type is: " + clip.Type);
            string data = FetchUrl(DIREKTE_URL + clip.ID);
            Regex query;
            query = new Regex("<param name=\"FileName\" value=\"(.*?)\" />", RegexOptions.IgnoreCase);
            MatchCollection result = query.Matches(data);
            string urlToFetch;
            try
            {
                urlToFetch = result[0].Groups[1].Value;
            }
            catch (Exception e)
            {
                Console.WriteLine("feilet: " + e.Message);
                return null;
            }
            string urldata = FetchUrl(urlToFetch);
            Log.Debug(NrkConstants.PLUGIN_NAME + ": " + urldata);
            query = new Regex("<ref href=\"(.*?)\" />");
            MatchCollection movie_url = query.Matches(urldata);

            //Usikker på en del av logikken for å hente ut url her. Ikke sikker på hvorfor
            //det var en try/catch
            //skip any advertisement
            String urlToReturn = String.Empty;
            try
            {
                urlToReturn = movie_url[1].Groups[1].Value;
                if (urlToReturn.ToLower().EndsWith(".gif"))
                {
                    Log.Debug(NrkConstants.PLUGIN_NAME + ": Kan ikke spille av: " + urlToReturn + ", prøver annet treff.");
                    //vi kan ikke spille av fullt.gif, returnerer samme som i catch'en.
                    urlToReturn = movie_url[0].Groups[1].Value;
                }

            }
            catch
            {
                urlToReturn = movie_url[0].Groups[1].Value;
            }
            if (urlIsQTicketEnabled(urlToReturn))
            {
                urlToReturn = getDirectLinkWithTicket(urlToReturn);
            }
            return urlToReturn;
        }

        private string GetClipUrlAndPutStartTimeForKlipp(Clip clip)
        {
            Log.Debug(NrkConstants.PLUGIN_NAME + ": Clip type is: " + clip.Type);
            Log.Debug(NrkConstants.PLUGIN_NAME + ": Parsing xml: " + string.Format(NrkConstants.URL_GET_MEDIAXML, clip.ID, Speed));
            XmlKlippParser xmlKlippParser = new XmlKlippParser(string.Format(NrkConstants.URL_GET_MEDIAXML, clip.ID, Speed));
            string url = xmlKlippParser.GetUrl();
            clip.StartTime = xmlKlippParser.GetStartTimeOfClip();
            if (urlIsQTicketEnabled(url))
            {
                url = getDirectLinkWithTicket(url);
            }
            return url;
        }

        /// <summary>
        /// Method that returns the direct link to a qticket enabled clip, including the ticket
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string getDirectLinkWithTicket(string url)
        {
            String data = FetchUrl(url);
            Regex query = new Regex("<ref href=\"(.*?)\"/>", RegexOptions.IgnoreCase);
            MatchCollection result = query.Matches(data);
            string ticketEnabledUrl = result[0].Groups[1].Value;
            return ticketEnabledUrl;         
        }

        /// <summary>
        /// Method that returns true if the url-string contains "qticket"
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool urlIsQTicketEnabled(string url)
        {
            return url.ToLower().Contains("qticket");
        }

        private static string GetClipUrlForRSS(Clip clip)
        {
            Log.Debug(NrkConstants.PLUGIN_NAME + ": Clip type is RSS");
            return RSS_CLIPURL_PREFIX + clip.ID;
        }

        private string GetClipUrlForIndex(Clip clip)
        {
            Regex query;
            Log.Debug(NrkConstants.PLUGIN_NAME + ": Clip type is INDEX");
            string data = FetchUrl(INDEX_URL + clip.ID);
            query = new Regex("<param name=\"FileName\" value=\"(.*?)\" />", RegexOptions.IgnoreCase);
            MatchCollection result = query.Matches(data);
            string urlToFetch = result[0].Groups[1].Value;
            urlToFetch = urlToFetch.Replace("amp;", ""); //noen ganger er det amper der..som må bort
            string urldata = FetchUrl(urlToFetch);
            Log.Debug(NrkConstants.PLUGIN_NAME + ": " + urldata);
            query = new Regex("<starttime value=\"(.*?)\" />.*?<ref href=\"(.*?)\" />", RegexOptions.Singleline);
            MatchCollection movie_url = query.Matches(urldata);
            //skip any advertisement

            string str_startTime = movie_url[0].Groups[1].Value;
            Log.Debug(NrkConstants.PLUGIN_NAME + ": Starttime er: " + str_startTime);
            //må gjøre string representasjon på formen: 00:27:38, om til en double
            Double dStartTime = NrkUtils.convertToDouble(str_startTime);
            clip.StartTime = dStartTime;
            return movie_url[0].Groups[2].Value;
        }

        private string GetClipUrlForVerdi(Clip clip)
        {
            Log.Info(NrkConstants.PLUGIN_NAME + ": Fetching verdi url");
            string data = FetchUrl(STORY_URL + clip.ID);
            
            Regex query = new Regex("<media:content url=\"(.*?)&amp;app=nett-tv\"", RegexOptions.IgnoreCase);
            MatchCollection result = query.Matches(data);
            string urlToFetch = result[0].Groups[1].Value;
            return urlToFetch;
        }


        private string FetchUrl(string url)
        {
            Log.Info(NrkConstants.PLUGIN_NAME + ": FetchUrl(String): " + url);
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            //Console.WriteLine("address: " + request.Address);
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)";
            request.CookieContainer = _jar;
            // Set some reasonable limits on resources used by this request
            request.MaximumAutomaticRedirections = 4;
            request.MaximumResponseHeadersLength = 4;
            // Set credentials to use for this request.
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();

            // Get the stream associated with the response.
            System.IO.Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            string ret = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
            return ret;
        }


    }
}