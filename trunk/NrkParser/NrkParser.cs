/*
 * Copyright (c) 2008 Terje Wiesener <wiesener@samfundet.no>
 * 
 * Loosely based on an anonymous (and slightly outdated) NRK parser in python for Myth-tv, 
 * please email me if you are the author :)
 * 
 * 2008-2009 by Vattenmelon
 * 
 * */

using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using NrkBrowser.Domain;
using Vattenmelon.Nrk.Parser;
using Vattenmelon.Nrk.Domain;
using Vattenmelon.Nrk.Parser.Http;
using Vattenmelon.Nrk.Parser.Xml;

namespace Vattenmelon.Nrk.Parser
{
    public class NrkParser
    {
        private static ILog Log;
        private IHttpClient httpClient;
       
        private static string BASE_URL = "http://www1.nrk.no/";
        private static string MAIN_URL = BASE_URL + "nett-tv/";
        private static string CATEGORY_URL = MAIN_URL + "tema/";
        private static string PROGRAM_URL = MAIN_URL + "prosjekt/";
        private static string DIREKTE_URL = MAIN_URL + "direkte/";
        private static string FOLDER_URL = MAIN_URL + "kategori/";
        private static string INDEX_URL = MAIN_URL + "indeks/";
        private static string STORY_URL = MAIN_URL + "spilleliste.ashx?story=";
        private static string SOK_URL_BEFORE = MAIN_URL + "DynamiskLaster.aspx?SearchResultList$search:{0}|sort:dato|page:{1}";
        private static string MEST_SETTE_URL = MAIN_URL + "ml/topp12.aspx?dager={0}&_=";
        private static string GET_COOKIE_URL = MAIN_URL + "hastighet.aspx?hastighet={0}&retururl=http://www1.nrk.no/nett-tv/";


        private CookieContainer cookieContainer;
        private int speed;

        public NrkParser(int speed, ILog logger)
        {
            Log = logger;
            cookieContainer = new CookieContainer();
            httpClient = new HttpClient(cookieContainer);
            this.Speed = speed;
        }

        public IHttpClient HttpClient
        {
            set { httpClient = value; }
        }

        public int Speed
        {
            get { return speed; }
            set
            {
                speed = value;
                FetchUrl(String.Format(GET_COOKIE_URL, speed));
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
            Regex query = new Regex("<a href=\"/nett-tv/tema/(?<id>\\w*).*?>(?<kategori>[^<]*)</a>");
            MatchCollection result = query.Matches(data);

            foreach (Match x in result)
            {
              categories.Add(new Category(x.Groups["id"].Value, x.Groups["kategori"].Value));
            }
            return categories;
        }

        public List<Item> GetAnbefaltePaaForsiden()
        {
            Log.Info(NrkParserConstants.LIBRARY_NAME + ": GetAnbefaltePaaForsiden()");
            string data;
            data = FetchUrl(MAIN_URL);

            Regex query =
                new Regex(
                    "<div class=\"img-left\" style=\"width: 100px;\">.*?<a href=\"[^\"]*\" title=\".*?\"><img src=\"(?<imgsrc>[^\"]*)\" width=\"100\" height=\"57\" alt=\".*?\" title=\".*?\".*?></a>.*?</div>.*?<div class=\"intro-element-content\"><h2><a href=\"/nett-tv/klipp/(?<id>[^\"]*)\" title=\"(?<desc>[^\"]*)\">(?<title>[^<]*)</a></h2>.*?<a href=\"/nett-tv/prosjekt/(?<prosjekt>[^\"]*)\">",
                    RegexOptions.Compiled | RegexOptions.Singleline);
            MatchCollection matches = query.Matches(data);
            List<Item> clips = new List<Item>();
            Log.Info(NrkParserConstants.LIBRARY_NAME + ": Matches {0}", matches.Count);
            foreach (Match x in matches)
            {
                Clip c = new Clip(x.Groups["id"].Value, x.Groups["title"].Value);
                c.TilhoerendeProsjekt = Int32.Parse(x.Groups["prosjekt"].Value);
                c.Description = x.Groups["desc"].Value;
                c.Bilde = x.Groups["imgsrc"].Value;
                clips.Add(c);
            }

            return clips;
        }

        public List<Item> GetMestSette(int dager)
        {
            Log.Info(string.Format("{0}: GetMestSette(), siste {1} dager", NrkParserConstants.LIBRARY_NAME, dager));
            string data;
            String url = String.Format(MEST_SETTE_URL, dager);
            data = FetchUrl(url);
            //"<a href=\".*?/nett-tv/klipp/.*?\" title=\".*?\"><img src=\"(.*?)\" .*? /></a></div><h2><a href=\".*?/nett-tv/klipp/.*?\" title=\".*?\">.*?</a></h2><keyword><a href=\".*?/nett-tv/klipp/(.*?)\" title=\"(.*?)\">Vist (.*?) ganger.</a></keyword>",
            //2009-11-23:  "<a href=\".*?/nett-tv/.*?/.*?\" title=\".*?\"><img src=\"(.*?)\" .*? /></a></div><h2><a href=\".*?/nett-tv/.*?/.*?\" title=\".*?\">.*?</a></h2><keyword><a href=\".*?/nett-tv/(.*?)/(.*?)\" title=\"(.*?)\">Vist (.*?) ganger.</a></keyword>",
            Regex query =
                new Regex(
                    "<a href=\".*?/nett-tv/.*?/.*?\" title=\".*?\"><img src=\"(?<imgsrc>[^\"]*)\" .*? /></a></div><h2><a href=\".*?/nett-tv/.*?/.*?\" title=\".*?\">.*?</a></h2><p><a href=\".*?/nett-tv/(?<type>[^/]*)/(?<id>[^\"]*)\" title=\"(?<title>[^\"]*)\">Vist (?<antallvist>[^ganger]*) ganger.</a></p>",
                    RegexOptions.Singleline);
            MatchCollection matches = query.Matches(data);
            List<Item> clips = new List<Item>();

            Log.Info(NrkParserConstants.LIBRARY_NAME + ": Matches {0}", matches.Count);
            foreach (Match x in matches)
            {
                Clip c = new Clip(x.Groups["id"].Value, x.Groups["title"].Value);
                c.AntallGangerVist = x.Groups["antallvist"].Value;
                c.Description = c.AntallGangerVist;
                c.Bilde = x.Groups["imgsrc"].Value;
                if (x.Groups["type"].Value.Trim().ToLower().Equals("indeks"))
                {
                    c.Type = Clip.KlippType.INDEX;
                }
                clips.Add(c);
            }

            return clips;
        }



        public List<Item> GetTopTabber()
        {
            Log.Info(NrkParserConstants.LIBRARY_NAME + ": GetTopTabber()");
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
                    "<li.*?><a href=\"/nett-tv/(?<id>[^\"]*)\" title=\".*?\"><span>(?<title>[^<]*)</span></a></li>",
                    RegexOptions.Singleline);
            MatchCollection matches = query.Matches(data);
            List<Item> items = new List<Item>();
            Log.Info(NrkParserConstants.LIBRARY_NAME + ": Matches {0}", matches.Count);
            foreach (Match x in matches)
            {
                string title = x.Groups["title"].Value;
                AutogeneratedMenuItem item = new AutogeneratedMenuItem(x.Groups["id"].Value, title);
                items.Add(item);
            }
            
            return items;
        }

        public List<Item> GetTopTabRSS(string site)
        {
            XmlRSSParser parser = new XmlRSSParser(NrkParserConstants.RSS_URL, site);
            return parser.getClips();
        }

        /**
         * Metode som sjekker om clippet er en vignett. Vi vil ikke ha disse inn i lista.
         */

        public static bool isNotShortVignett(Clip loRssItem)
        {
            return
                loRssItem.ID != NrkParserConstants.VIGNETT_ID_NATURE && loRssItem.ID != NrkParserConstants.VIGNETT_ID_SUPER && loRssItem.ID != NrkParserConstants.VIGNETT_ID_NYHETER &&
                loRssItem.ID != NrkParserConstants.VIGNETT_ID_SPORT;
        }

        public List<Item> GetDirektePage()
        {
            String tab = "direkte";
            Log.Info(NrkParserConstants.LIBRARY_NAME + ": GetDirektePage(String)", tab);
            string data;
            data = FetchUrl(MAIN_URL + tab + "/");
            Regex query =
                new Regex(
                    "<div class=\"img-left\">.*?<a href=\"/nett-tv/direkte/(.*?)\" title=\"(.*?)\"><img class=\"info\" src=\".*?\" alt=\"Direkte\" /><img src=\"(.*?)\" .*? /></a>.*?</div>",
                    RegexOptions.Singleline);
            

            MatchCollection matches = query.Matches(data);
            List<Item> clips = new List<Item>();
            Log.Info(NrkParserConstants.LIBRARY_NAME + ": Matches {0}", matches.Count);
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
            Log.Info(NrkParserConstants.LIBRARY_NAME + ": GetTopTabContent(String)", tab);
            string data;
            data = FetchUrl(MAIN_URL + tab + "/");
            Regex query =
                new Regex(
                    "<div class=\"img-left\" style=\"width: 120px;\"><a href=\".*?/nett-tv/" + tab + "/spill/verdi/(.*?)\" .*?><img src=\"(.*?)\" alt=\".*?\" title=\"(.*?)\" .*? />.*?</a>.*?</div>",
                    RegexOptions.Singleline);


            MatchCollection matches = query.Matches(data);
            List<Item> clips = new List<Item>();
            Log.Info(NrkParserConstants.LIBRARY_NAME + ": Matches {0}", matches.Count);
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
                    c.Description = NrkUtils.parseKlokkeSlettFraBilde(bildeUrl);
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
                new Regex("<a href=\"/nett-tv/prosjekt/.*?\" id=\".*?\" title=\".*?\".*?>\\s+<img src=\"(?<imgsrc>[^\"]*)\" id=");
            MatchCollection matchesBilder = queryBilder.Matches(data);
            List<string> bilder = new List<string>();
            foreach (Match x in matchesBilder)
            {
                bilder.Add(x.Groups["imgsrc"].Value);
            }

            Regex query =
                new Regex(
                    "<a href=\"/nett-tv/prosjekt/(?<id>[^\"]*)\" id=\".*ClipLink\" title=\".*?\".*? onclick=\".*?\">(?<title>[^<]*)</a>");
            MatchCollection matches = query.Matches(data);
            List<Item> programs = new List<Item>();
            int bildeTeller = 0;
            foreach (Match x in matches)
            {
                programs.Add(new Program(x.Groups["id"].Value, x.Groups["title"].Value, "", bilder[bildeTeller]));
                bildeTeller++;
            }

            Regex query2 =
                new Regex("<a href=\"/nett-tv/prosjekt/.*?\" id=\".*ClipTitle\" title=\".*?\".*?>(?<desc>[^<]*)</a>");
            MatchCollection matches2 = query2.Matches(data);
            int teller = 0;
            foreach (Match x in matches2)
            {
                programs[teller].Description = x.Groups["desc"].Value;
                teller++;
            }
            return programs;
        }

        public List<Item> GetAllPrograms() //all programs
        {
            string data = FetchUrl(MAIN_URL + "bokstav/@");
            //2009-11-23: "<div class=\"intro-element intro-element-small\">.*?<div.*?>.*?<a href=\"/nett-tv/prosjekt/(.*?)\" id=\".*?\" title=\"(.*?)\".*?>.*?<img src=\"(.*?)\".*?>.*?</a>.*?</div>.*?<h2>.*?<a href=\".*?\" id=\".*?\" title=\".*?\" onclick=\".*?\">(.*?)</a>.*?</h2>.*?<keyword>.*?</keyword>.*?</div>"
            Regex query =
                new Regex(
                    "<div class=\"intro-element intro-element-small\">.*?<div.*?>.*?<a href=\"/nett-tv/prosjekt/(.*?)\" id=\".*?\" title=\"(.*?)\".*?>.*?<img src=\"(.*?)\".*?>.*?</a>.*?</div>.*?<h2>.*?<a href=\".*?\" id=\".*?\" title=\".*?\" onclick=\".*?\">(.*?)</a>.*?</h2>.*?</div>",
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
            Log.Info("{0}: GetClips(int): {1}", NrkParserConstants.LIBRARY_NAME, id);
            string data = FetchUrl(PROGRAM_URL + id);
            List<Item> clips = new List<Item>();
            Regex query =
                new Regex("<a href=\"/nett-tv/klipp/(.*?)\"\\s+title=\"(.*?)\"\\s+class=\"(.*?)\".*?>(.*?)</a>");
            MatchCollection matches = query.Matches(data);
            foreach (Match x in matches)
            {
                Clip c = new Clip(x.Groups[1].Value, x.Groups[4].Value);
                c.TilhoerendeProsjekt = id;
                clips.Add(c);
            }

            return clips;
        }
        public List<Item> GetClips(Program program)
        {
            Log.Info("{0}: GetClips(Program): {1}", NrkParserConstants.LIBRARY_NAME, program);
            return GetClips(Int32.Parse(program.ID));
        }

        public List<Item> GetClipsTilhoerendeSammeProgram(Clip c)
        {
            //return GetClips(c.TilhoerendeProsjekt);
            List<Item> tItems = GetClips(c.TilhoerendeProsjekt);
            tItems.AddRange(GetFolders(c.TilhoerendeProsjekt));
            return tItems;
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

        public string GetClipUrlAndPutStartTime(Clip clip)
        {
            Log.Debug("{0}: GetClipUrlAndPutStartTime(Clip): {1}", NrkParserConstants.LIBRARY_NAME, clip);

            if (clip.Type == Clip.KlippType.KLIPP)
            {
                return GetClipUrlAndPutStartTimeForKlipp(clip);
            }
            else if (clip.Type == Clip.KlippType.KLIPP_CHAPTER)
            {
                return clip.ID;
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
            else if (clip.Type == Clip.KlippType.NRKBETA || clip.Type == Clip.KlippType.PODCAST)
            {
                return clip.ID;
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
            Log.Debug(NrkParserConstants.LIBRARY_NAME + ": Clip type is: " + clip.Type);
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
            Log.Debug(NrkParserConstants.LIBRARY_NAME + ": " + urldata);
            query = new Regex("<ref href=\"(.*?)\" />");
            MatchCollection movie_url = query.Matches(urldata);

            //Usikker på en del av logikken for å hente ut url her. Ikke sikker på hvorfor
            //det var en try/catch
            //skip any advertisement
            String urlToReturn;
            try
            {
                urlToReturn = movie_url[1].Groups[1].Value;
                if (urlToReturn.ToLower().EndsWith(".gif"))
                {
                    Log.Debug(NrkParserConstants.LIBRARY_NAME + ": Kan ikke spille av: " + urlToReturn + ", prøver annet treff.");
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
            Log.Debug(NrkParserConstants.LIBRARY_NAME + ": Clip type is: " + clip.Type);
            Log.Debug(NrkParserConstants.LIBRARY_NAME + ": Parsing xml: " + string.Format(NrkParserConstants.URL_GET_MEDIAXML, clip.ID, Speed));
            XmlKlippParser xmlKlippParser = new XmlKlippParser(string.Format(NrkParserConstants.URL_GET_MEDIAXML, clip.ID, Speed));
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
        /// <returns>Direct link to the clip</returns>
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
            return url.ToLower().Contains(NrkParserConstants.QTICKET);
        }

        private static string GetClipUrlForRSS(Clip clip)
        {
            Log.Debug(NrkParserConstants.LIBRARY_NAME + ": Clip type is RSS");
            return NrkParserConstants.RSS_CLIPURL_PREFIX + clip.ID;
        }

        private string GetClipUrlForIndex(Clip clip)
        {
            Regex query;
            Log.Debug(NrkParserConstants.LIBRARY_NAME + ": Clip type is INDEX");
            string data = FetchUrl(INDEX_URL + clip.ID);
            query = new Regex("<param name=\"FileName\" value=\"(.*?)\" />", RegexOptions.IgnoreCase);
            MatchCollection result = query.Matches(data);
            string urlToFetch = result[0].Groups[1].Value;
            urlToFetch = urlToFetch.Replace("amp;", ""); //noen ganger er det amper der..som må bort
            string urldata = FetchUrl(urlToFetch);
            Log.Debug(NrkParserConstants.LIBRARY_NAME + ": " + urldata);
            query = new Regex("<starttime value=\"(.*?)\" />.*?<ref href=\"(.*?)\" />", RegexOptions.Singleline);
            MatchCollection movie_url = query.Matches(urldata);
            //skip any advertisement

            string str_startTime = movie_url[0].Groups[1].Value;
            Log.Debug(NrkParserConstants.LIBRARY_NAME + ": Starttime er: " + str_startTime);
            //må gjøre string representasjon på formen: 00:27:38, om til en double
            Double dStartTime = NrkUtils.convertToDouble(str_startTime);
            clip.StartTime = dStartTime;
            return movie_url[0].Groups[2].Value;
        }

        private string GetClipUrlForVerdi(Clip clip)
        {
            Log.Info(NrkParserConstants.LIBRARY_NAME + ": Fetching verdi url");
            string data = FetchUrl(STORY_URL + clip.ID);
            
            Regex query = new Regex("<media:content url=\"(.*?)&amp;app=nett-tv\"", RegexOptions.IgnoreCase);
            MatchCollection result = query.Matches(data);
            string urlToFetch = result[0].Groups[1].Value;
            return urlToFetch;
        }

        public List<Item> GetMestSetteForKategoriOgPeriode(Periode periode, String category)
        {
            String url = getUrl(category);
            string postData = string.Format(NrkParserConstants.MOST_WATCHED_DATA_TO_POST, periode, NrkParserConstants.MOST_WATCHED_VIEWSTATE, category, periode);
            return GetMestSetteGeneric(url, postData);
        }

        private string getUrl(String cateogry)
        {
            if (cateogry.ToLower().Equals("nyheter"))
                return MAIN_URL + "nyheter";
            else if (cateogry.ToLower().Equals("sport"))
                return MAIN_URL + "sport";
            else if (cateogry.ToLower().Equals("distrikt"))
                return MAIN_URL + "distrikt";
            else if (cateogry.ToLower().Equals("natur"))
                return MAIN_URL + "natur";
            else
                throw new Exception("No valid URL found!");
        }

        public List<Item> GetMestSetteGeneric(String url, String dataToPost)
        {
            Log.Info(string.Format("{0}: GetMestSetteGeneric :{1}, {2}", NrkParserConstants.LIBRARY_NAME, url, dataToPost));
            String data = httpClient.PostUrl(url, dataToPost);
            Regex query =
                    new Regex(
                        "<div class=\"img-left\" style=\"width: 120px;\">.*?<a href=\".*?\"><img src=\"(.*?)\" alt=\".*?\" title=\".*?\" width=\"120\" height=\"68\".*?/></a></div><div class=\"active\"><h2><a href=\"(.*?)\">(.*?)</a></h2><p><a.*?><span.*?>(.*?) visninger</span><span>(.*?)</span></a>",
                        RegexOptions.Singleline);

            MatchCollection matches = query.Matches(data);
            List<Item> clips = new List<Item>();
            Log.Info(NrkParserConstants.LIBRARY_NAME + ": Matches {0}", matches.Count);
            foreach (Match x in matches)
            {
                string idUrl = x.Groups[2].Value;
                Clip c = new Clip("tmpId", x.Groups[3].Value);
                NrkUtils.BestemKlippTypeOgPuttPaaId(c, idUrl);
                String bildeUrl = x.Groups[1].Value;
                c.Bilde = bildeUrl;
                c.AntallGangerVist = x.Groups[4].Value;
                c.Klokkeslett = x.Groups[5].Value;
                clips.Add(c);
            }

            return clips;
        }

        private string FetchUrl(string url)
        {
            return httpClient.GetUrl(url);
        }

        public enum Periode
        {
            Uke,
            Maned,
            Totalt
        }

        public List<Clip> getChapters(Clip item)
        {
            XmlKlippParser xmlKlippParser = new XmlKlippParser(string.Format(NrkParserConstants.URL_GET_MEDIAXML, item.ID, Speed));
            return xmlKlippParser.GetChapters();
        }

        public IList<PodKast> GetVideoPodkaster()
        {            
            string videokastsSectionAsString = GetVideokastsSectionAsString();
            return CreatePodkasts(videokastsSectionAsString);
        }

        private static IList<PodKast> CreatePodkasts(string videokastsSectionAsString)
        {
            string tbodyExpression = "<tbody>(.*?)</tbody>";
            Regex queryTbodys = new Regex(tbodyExpression, RegexOptions.Singleline);
            MatchCollection matches = queryTbodys.Matches(videokastsSectionAsString);
            IList<PodKast> items = new List<PodKast>();
            foreach (Match x in matches)
            {
                string urlExpressionString =
                    "<tbody>.*?<tr class=\"pod-row\">.*?<th>(?<title>[^</]*)</th>.*?<td class=\"pod-rss\">.*?</td>.*?</tr>.*?<tr class=\"pod-desc\">.*?<td colspan=\"3\">.*?<p>(?<description>[^<]*)<a href=\".*?\" title=\".*?\">.*?</a></p>.*?</td>.*?</tr>.*?<tr class=\"pod-rss-url\">.*?<td colspan=\"3\">.*?<a href=\"(?<url>[^\"]*)\" title=\".*?\">.*?</a>.*?</td>.*?</tr>.*?</tbody>";
                PodKast kast = CreatePodkastItem(urlExpressionString, x.Groups[0].Value);
                if (kast != null)
                {
                    items.Add(kast);
                }

            }
            return items;
        }

        private static PodKast CreatePodkastItem(string urlExpressionString, string videokastsSectionAsString)
        {
            Regex queryVideokasts = new Regex(urlExpressionString, RegexOptions.Singleline);
            MatchCollection matches = queryVideokasts.Matches(videokastsSectionAsString);
            if (matches.Count == 1)
            {
                PodKast item = new PodKast(matches[0].Groups["url"].Value, matches[0].Groups["title"].Value);
                item.Description = matches[0].Groups["description"].Value;
                item.Bilde = String.Empty;
                return item;
            }
            return null;
            
        }

        private string GetVideokastsSectionAsString()
        {
            string videokastsExpression =
                "<table summary=\"Liste over videopodkaster fra NRK\">(?<media>[^</table>].*)</table>.*?</div>.*?<div class=\"pod\">.*?<table summary=\"Liste over lydpodkaster fra NRK\">";
            return GetMediaSection(videokastsExpression);
        }

        private string GetMediaSection(string videokastsExpression)
        {
            String pageAsString = FetchUrl("http://www.nrk.no/podkast/");           
            Regex queryVideokastsSection = new Regex(videokastsExpression, RegexOptions.Singleline);
            MatchCollection matches = queryVideokastsSection.Matches(pageAsString);
            return matches[0].Groups["media"].Value;
        }

        private string GetLydkastsSectionAsString()
        {
            string videokastsExpression =
                "<table summary=\"Liste over lydpodkaster fra NRK\">(?<media>[^</table>].*)</table>.*?</div>";
            return GetMediaSection(videokastsExpression);
        }

        public IList<PodKast> GetLydPodkaster()
        {
            string lydkastSectionAsString = GetLydkastsSectionAsString();
            return CreatePodkasts(lydkastSectionAsString);
        }
    }


}