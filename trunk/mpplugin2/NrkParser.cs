/*
 * Copyright (c) 2008 Terje Wiesener <wiesener@samfundet.no>
 * 
 * Loosely based on an anonymous (and slightly outdated) NRK parser in python for Myth-tv, 
 * please email me if you are the author :)
 * 
 * */

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using MediaPortal.GUI.Library;

namespace NrkBrowser
{
    public class NrkParser
    {
        public const string RSS_CLIPURL_PREFIX = "http://pd.nrk.no/nett-tv-stream/stream.ashx?id=";
        private static string BASE_URL = "http://www1.nrk.no/";
        private static string MAIN_URL = BASE_URL + "nett-tv/";
        private static string CATEGORY_URL = MAIN_URL + "tema/";
        private static string PROGRAM_URL = MAIN_URL + "prosjekt/";
        private static string CLIP_URL = MAIN_URL + "klipp/";
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

            string regexQuery = "<li id=\"ctl00.*?\">.*?";
            regexQuery += "<a href=\"(.*?)\" id=\".*?\" style=\".*?\" title=\".*?\" class=\"(.*?)\">(.*?)</a>.*?";
            regexQuery += "<div.*?>(.*?)</div>.*?";
            regexQuery += "</li>";

            Regex query = new Regex(regexQuery, RegexOptions.Singleline);
            List<Item> categories = new List<Item>();
            MatchCollection result = query.Matches(data);
            int teller = 1 + (page*25);

            Console.WriteLine("matches: " + result.Count);
            foreach (Match x in result)
            {
                String link = x.Groups[1].Value;
                String type = x.Groups[2].Value;
                String title = string.Format("{0}", x.Groups[3].Value);
                String description = x.Groups[4].Value;

                String id = link.Substring(x.Groups[1].Value.LastIndexOf("/", x.Groups[1].Value.Length) + 1);

                if (type.Equals("video-index") || type.Equals("audio-index"))
                {
                    Clip c = new Clip(id, title);
                    c.Description = x.Groups[4].Value;
                    c.Type = Clip.KlippType.INDEX;
                    categories.Add(c);
                }
                else if (type.Equals("video") || type.Equals("audio"))
                {
                    Clip c = new Clip(id, title);
                    c.Description = x.Groups[4].Value;
                    categories.Add(c);
                }
                else if (type.Equals("project"))
                {
                    Program p = new Program(id, title, description, "");
                    categories.Add(p);
                }
                else if (type.Equals("folder"))
                {
                    Folder f = new Folder(id, title);
                    f.Description = x.Groups[4].Value;
                    categories.Add(f);
                }
                else
                {
                    Console.WriteLine("feil: " + type);
                    Log.Error(NrkPlugin.PLUGIN_NAME + ": unsupported type: " + x.Groups[2].Value);
                }
                teller++;
            }
            return categories;
        }

        public List<Item> GetCategories()
        {
            List<Item> categories = new List<Item>();
            string data = FetchUrl(MAIN_URL);
            Regex query = new Regex("<a href=\"/nett-tv/tema/(\\w*).*?>(.*?)</a>");
            MatchCollection result = query.Matches(data);

            foreach (Match x in result)
            {
                //TODO: 6/9, hvorfor er denne her egentlig?
                if (x.Groups[2].Value != "Plusspakken")
                {
                    categories.Add(new Category(x.Groups[1].Value, x.Groups[2].Value));
                }
            }
            return categories;
        }

        public List<Item> GetAnbefaltePaaForsiden()
        {
            Log.Info(NrkPlugin.PLUGIN_NAME + ": GetAnbefaltePaaForsiden()");
            string data;
            data = FetchUrl(MAIN_URL);

            Regex query =
                new Regex(
                    "<div class=\"img-left\" style=\"width: 100px;\">.*?<a href=\".*?\" title=\".*?\"><img src=\"(.*?)\" width=\"100\" height=\"57\" alt=\".*?\" title=\".*?\".*?></a>.*?</div>.*?<div class=\"intro-element-content\"><h2><a href=\"/nett-tv/klipp/(.*?)\" title=\"(.*?)\">(.*?)</a></h2>",
                    RegexOptions.Singleline);
            MatchCollection matches = query.Matches(data);
            List<Item> clips = new List<Item>();

            Log.Info(NrkPlugin.PLUGIN_NAME + ": Matches {0}", matches.Count);
            foreach (Match x in matches)
            {
                Clip c = new Clip(x.Groups[2].Value, x.Groups[4].Value);
                c.Description = x.Groups[3].Value;
                c.Bilde = x.Groups[1].Value;
                clips.Add(c);
            }

            return clips;
        }

        public List<Item> GetMestSette(int dager)
        {
            Log.Info(NrkPlugin.PLUGIN_NAME + ": GetMestSettDenneMaaneden()");
            string data;
            String url = String.Format("http://www1.nrk.no/nett-tv/ml/topp12.aspx?dager={0}&_=", dager);
            data = FetchUrl(url);

            Regex query =
                new Regex(
                    "<div class=\"img-left\" style=\"width: 150px;\"><a href=\"/nett-tv/klipp/.*?\" title=\".*?\"><img src=\"(.*?)\" alt=\"\" title=\"\" longdesc=\"\" height=\"85\" width=\"150\" /></a></div><h2><a href=\"/nett-tv/klipp/.*?\" title=\".*?\">.*?</a></h2><p><a href=\"/nett-tv/klipp/.*?\" title=\".*?\">Publisert .*?</a><br /><a href=\"/nett-tv/klipp/(.*?)\" title=\"(.*?)\">Vist (.*?) ganger</a>",
                    RegexOptions.Singleline);
            MatchCollection matches = query.Matches(data);
            List<Item> clips = new List<Item>();

            Log.Info(NrkPlugin.PLUGIN_NAME + ": Matches {0}", matches.Count);
            foreach (Match x in matches)
            {
                Clip c = new Clip(x.Groups[2].Value, x.Groups[3].Value);
                c.AntallGangerVist = x.Groups[4].Value;
                c.Description = String.Format("Klipp vist {0} ganger denne uken.", c.AntallGangerVist);
                c.Bilde = x.Groups[1].Value;
                clips.Add(c);
            }

            return clips;
        }

        public List<Item> GetTopTabber()
        {
            //virker ikke ennå
            Log.Info(NrkPlugin.PLUGIN_NAME + ": GetTopTabber()");
            string data;
            data = FetchUrl(MAIN_URL);
            Console.WriteLine(data);
            Regex query =
                new Regex(
                    "<li><a href=\"/nett-tv(.*?)\"><span>(.*?)</span></a></li>",
                    RegexOptions.Singleline);
            MatchCollection matches = query.Matches(data);
            List<Item> items = new List<Item>();

            Log.Info(NrkPlugin.PLUGIN_NAME + ": Matches {0}", matches.Count);
            foreach (Match x in matches)
            {
                MenuItem item = new MenuItem(x.Groups[1].Value, x.Groups[2].Value);
                items.Add(item);
            }

            return items;
        }

        public List<Item> GetTopTabRSS(string site)
        {
            List<Item> clips = new List<Item>();
            //natur clips uses flash...but we love flash ;)
            XmlDocument doc = new XmlDocument();
            XmlTextReader reader = new XmlTextReader("http://www1.nrk.no/nett-tv/MediaRss.ashx?loop=" + site);
            doc.Load(reader);
            XmlNamespaceManager expr = new XmlNamespaceManager(doc.NameTable);
            expr.AddNamespace("media", "http://search.yahoo.com/mrss");
            XmlNode root = doc.SelectSingleNode("//rss/channel/item", expr);
            XmlNodeList nodeList;
            nodeList = root.SelectNodes("//rss/channel/item");

            int itemCount = 0;
            foreach (XmlNode childNode in nodeList)
            {
                if (itemCount >= 100)
                {
                    break;
                }
                itemCount++;
                Clip loRssItem = CreateClipFromChildNode(childNode);

                if (isNotShortVignett(loRssItem))
                {
                    loRssItem.Type = Clip.KlippType.RSS;
                    clips.Add(loRssItem);
                }
            }
            return clips;
        }

        private static Clip CreateClipFromChildNode(XmlNode childNode)
        {
            Clip loRssItem;
            loRssItem = new Clip("", "");

            for (int i = 0; i < childNode.ChildNodes.Count; i++)
            {
                XmlNode n = childNode.ChildNodes[i];

                switch (n.Name)
                {
                    case "title":
                        loRssItem.Title = n.InnerText;
                        break;
                    case "link":

                        break;
                    case "guid":
                        loRssItem.ID = n.InnerText;
                        break;
                    case "pubDate":

                        break;
                    case "description":
                        loRssItem.Description = n.InnerText;
                        break;
                    case "author":

                        break;
                    case "exInfo:image":

                        break;
                    case "exInfo:gameID":

                        break;
                    case "feedburner:origLink":

                        break;
                    case "media:group":

                        for (int j = 0; j < n.ChildNodes.Count; j++)
                        {
                            XmlNode nin = n.ChildNodes[j];
                            switch (nin.Name)
                            {
                                case "media:content":
//                                        loMediaContent = new MediaContent();
                                    try
                                    {
//                                            loMediaContent.url = nin.Attributes["url"].Value;
//                                            loMediaContent.type = nin.Attributes["type"].Value;
//                                            loMediaContent.medium = nin.Attributes["medium"].Value;
//                                            loMediaContent.duration = nin.Attributes["duration"].Value;
//                                            loMediaContent.height = nin.Attributes["height"].Value;
//                                            loMediaContent.width = nin.Attributes["width"].Value;
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Error("catccccccccccched exception: " + e.Message);
                                    }
                                    ;
                                    break;
                                case "media:description":

                                    break;
                                case "media:thumbnail":

                                    break;
                                case "media:title":

                                    break;
                            }
                        }
                        break;

                    case "media:description":
//                            loRssItem.mediaDescription = n.InnerText;
                        break;
                    case "media:thumbnail":
//                            loRssItem.mediaThumbnail = n.Attributes["url"].Value;
                        break;
                    case "media:title":
//                            loRssItem.mediaTitle = n.InnerText;
                        break;
                    case "enclosure":
                        // loRssItem.ID = n.InnerText;
                        //loRssItem.ID = n.Attributes["url"].Value;
//                            if (n.Attributes["duration"] != null)
//                            {
//                                loRssItem.enclosureDuration = n.Attributes["duration"].Value;
//                            }

                        break;
                    case "media:category":
                        //  loRssItem.mediaCategory = n.InnerText;
                        break;
                    default:
                        break;
                }
            }
            return loRssItem;
        }

        /**
         * Metode som sjekker om clippet er en vignett
         */

        private static bool isNotShortVignett(Clip loRssItem)
        {
            //372980 is the id for the short natur-vignett, and we do not want it in list
            //381994 is the for the super vignett
            //410330 is the nyheter vignett
            //410335 is the sport vignett
            return
                loRssItem.ID != "372980" && loRssItem.ID != "381994" && loRssItem.ID != "410330" &&
                loRssItem.ID != "410335";
        }

        public List<Item> GetTopTabber(String tab)
        {
            Log.Info(NrkPlugin.PLUGIN_NAME + ": GetTopTabber(String)", tab);
            string data;
            data = FetchUrl(MAIN_URL + tab + "/");
            Regex query = null;
            if (tab == "sport")
            {
                //deprecated, bruker RSS feed istedet
                query =
                    new Regex(
                        "<div class=\"img-left\" style=\"width: 120px;\">.*?<a href=\".*?\" onclick=\"return true;\"><img src=\"(.*?)\" alt=\".*?\" title=\".*?\" width=\"120\" height=\"68\".*?></a>.*?</div>.*?<div class=\"active\"><h2><a href=\"http://www1.nrk.no/nett-tv/sport/spill/verdi/(.*?)\" onclick=\"return true;\">(.*?)</a></h2>",
                        RegexOptions.Singleline);
            }
            if (tab == "nyheter")
            {
                //deprecated, bruker RSS feed istedet
                query =
                    new Regex(
                        "<div class=\"img-left\" style=\"width: 120px;\">.*?<a href=\".*?\" onclick=\"return true;\"><img src=\"(.*?)\" alt=\".*?\" title=\".*?\" width=\"120\" height=\"68\".*?></a>.*?</div>.*?<div class=\"active\"><h2><a href=\"http://www1.nrk.no/nett-tv/nyheter/spill/verdi/(.*?)\" onclick=\"return true;\">(.*?)</a></h2>",
                        RegexOptions.Singleline);
            }
            if (tab == "natur")
            {
                //deprecated, bruker RSS feed istedet
                query =
                    new Regex(
                        "<div class=\"img-left\" style=\"width: 120px;\">.*?<a href=\".*?\" onclick=\"return true;\"><img src=\"(.*?)\" alt=\".*?\" title=\".*?\" width=\"120\" height=\"68\".*?></a>.*?</div>.*?<div class=\"active\"><h2><a href=\"http://www1.nrk.no/nett-tv/natur/spill/verdi/(.*?)\" onclick=\"return true;\">(.*?)</a></h2>",
                        RegexOptions.Singleline);
            }

            if (tab == "direkte")
            {
                //XXX not ready for primetime yet.
                query =
                    new Regex(
                        "<div class=\"img-left\">.*?<a href=\"/nett-tv/klipp/(.*?)\" title=\"(.*?)\" style=\".*?\"><img class=\"info\" src=\"http://www1.nrk.no/nett-tv/data/stillbilder/Kommer.jpg\" alt=\"Kommer\" /><img src=\"(.*?)\" .*? /></a>.*?</div>",
                        RegexOptions.Singleline);
            }

            MatchCollection matches = query.Matches(data);
            List<Item> clips = new List<Item>();
            Log.Info(NrkPlugin.PLUGIN_NAME + ": Matches {0}", matches.Count);
            foreach (Match x in matches)
            {
                string klokkeslett = parseKlokkeSlettFraBilde(x.Groups[1].Value);
                Clip c = new Clip(x.Groups[2].Value, x.Groups[3].Value);
                String bildeUrl = x.Groups[1].Value;
                c.Bilde = bildeUrl;
                c.Description = "Lagt ut " + klokkeslett;
                c.Type = Clip.KlippType.VERDI;
                c.VerdiLink = tab;
                clips.Add(c);
            }

            return clips;
        }

        private string parseKlokkeSlettFraBilde(string bildeUrl)
        {
            try
            {
                string temp;

                temp = bildeUrl.Substring(bildeUrl.LastIndexOf("/nsps_upload") + 13);
                string[] tab = temp.Split('_');
                string time = tab[3];
                if (time.Length == 1)
                {
                    time = "0" + time;
                }
                string minutt = tab[4];
                if (minutt.Length == 1)
                {
                    minutt = "0" + minutt;
                }
                return time + ":" + minutt + " " + tab[2] + "/" + tab[1] + "-" + tab[0];
            }
            catch (Exception)
            {
                Log.Info(NrkPlugin.PLUGIN_NAME +
                         ": Could not parse date from image filename, but that is fine...just returning a blank string");
                return "";
            }
        }

        public List<Item> GetPrograms(Category category)
        {
            string data;

            data = FetchUrl(CATEGORY_URL + category.ID);


            //Regex query = new Regex("<a href=\"/nett-tv/prosjekt/(.*?)\" id=\".*?\" title=\"(.*?)\".*?>\\s+<img src=\"(.*?)\" id=");

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
            //Regex query = new Regex("<a href=\"/nett-tv/prosjekt/(.*?)\" id=\".*?\" title=\"(.*?)\".*?>\\s+<img src=\"(.*?)\" id=");
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

        public List<Item> GetFolders(Program program)
        {
            string data = FetchUrl(PROGRAM_URL + program.ID);
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

        public List<Item> GetFolders(Folder folder)
        {
            string data = FetchUrl(FOLDER_URL + folder.ID);
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

        public List<Item> GetClips(Program program)
        {
            Log.Info("{0}: GetClips(Program): {1}", NrkPlugin.PLUGIN_NAME, program);
            string data = FetchUrl(PROGRAM_URL + program.ID);
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
            Log.Debug("{0}: GetClipUrl(Clip): {1}", NrkPlugin.PLUGIN_NAME, clip);

            if (clip.Type == Clip.KlippType.KLIPP)
            {
                return behandleDefaultKlipp(clip);
            }
            else if (clip.Type == Clip.KlippType.RSS)
            {
                return behandleRSSKlipp(clip);
            }
            else if (clip.Type == Clip.KlippType.INDEX)
            {
                return behandleIndexKlipp(clip);
            }
            else
            {
                return behandleVerdiKlipp(clip);
            }
        }

        private string behandleDefaultKlipp(Clip clip)
        {
            Regex query;
            Log.Debug(NrkPlugin.PLUGIN_NAME + ": Clip type is KLIPP");
            string data = FetchUrl(CLIP_URL + clip.ID);
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
            Log.Debug(NrkPlugin.PLUGIN_NAME + ": " + urldata);
            query = new Regex("<ref href=\"(.*?)\" />");
            MatchCollection movie_url = query.Matches(urldata);

            //Usikker på en del av logikken for å hente ut url her. Ikke sikker på hvorfor
            //det var en try/catch
            //skip any advertisement
            try
            {
                string url1 = movie_url[1].Groups[1].Value;
                if (url1.ToLower().EndsWith(".gif"))
                {
                    Log.Debug(NrkPlugin.PLUGIN_NAME + ": Kan ikke spille av: " + url1 + ", prøver annet treff.");
                    //vi kan ikke spille av fullt.gif, returnerer samme som i catch'en.
                    return movie_url[0].Groups[1].Value;
                }
                return url1;
            }
            catch
            {
                return movie_url[0].Groups[1].Value;
            }
        }

        private static string behandleRSSKlipp(Clip clip)
        {
            Log.Debug(NrkPlugin.PLUGIN_NAME + ": Clip type is RSS");
            return RSS_CLIPURL_PREFIX + clip.ID;
        }

        private string behandleIndexKlipp(Clip clip)
        {
            Regex query;
            Log.Debug(NrkPlugin.PLUGIN_NAME + ": Clip type is INDEX");
            string data = FetchUrl(INDEX_URL + clip.ID);
            query = new Regex("<param name=\"FileName\" value=\"(.*?)\" />", RegexOptions.IgnoreCase);
            MatchCollection result = query.Matches(data);
            string urlToFetch = result[0].Groups[1].Value;
            urlToFetch = urlToFetch.Replace("amp;", ""); //noen ganger er det amper der..som må bort
            string urldata = FetchUrl(urlToFetch);
            Log.Debug(NrkPlugin.PLUGIN_NAME + ": " + urldata);
            query = new Regex("<starttime value=\"(.*?)\" />.*?<ref href=\"(.*?)\" />", RegexOptions.Singleline);
            MatchCollection movie_url = query.Matches(urldata);
            //skip any advertisement

            string str_startTime = movie_url[0].Groups[1].Value;
            Log.Debug(NrkPlugin.PLUGIN_NAME + ": Starttime er: " + str_startTime);
            //må gjøre string representasjon på formen: 00:27:38, om til en double
            Double dStartTime = NrkUtils.convertToDouble(str_startTime);
            clip.StartTime = dStartTime;
            return movie_url[0].Groups[2].Value;
        }

        private string behandleVerdiKlipp(Clip clip)
        {
            Regex query;
            //clip.Type == Clip.KlippType.VERDI;
            Log.Info(NrkPlugin.PLUGIN_NAME + ": Fetching verdi url");
            string data = FetchUrl(MAIN_URL + clip.VerdiLink + "/spill/verdi/" + clip.ID);
            //ser ut som om det er det samme om det står nyheter, sport osv før spill
            query = new Regex("<param name=\"Filename\" value=\"(.*?)\" />", RegexOptions.IgnoreCase);
            MatchCollection result = query.Matches(data);
            string urlToFetch = result[0].Groups[1].Value;
            urlToFetch = urlToFetch.Replace("amp;", ""); //noen ganger er det amper der..som må bort
            string urldata = FetchUrl(urlToFetch);
            Log.Debug(NrkPlugin.PLUGIN_NAME + ": " + urldata);
            query = new Regex("<ref href=\"(.*?)\" />");
            MatchCollection movie_url = query.Matches(urldata);
            //skip any advertisement
            return movie_url[0].Groups[1].Value;
        }


        private string FetchUrl(string url)
        {
            Log.Info(NrkPlugin.PLUGIN_NAME + ": FetchUrl(String): " + url);
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