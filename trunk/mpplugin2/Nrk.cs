/*
 * Copyright (c) 2008 Terje Wiesener <wiesener@samfundet.no>
 * 
 * Loosely based on an anonymous (and slightly outdated) NRK parser in python for Myth-tv, 
 * please email me if you are the author :)
 * 
 * */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Text;
using System.Xml;
using MediaPortal.GUI.Library;
using NrkBrowser;

namespace Nrk
{
    #region Helper Classes

    public abstract class Item
    {
        private string _id;
        private string _title;
        private string _description = "";
        private string _bilde = "";

        public string Title
        {
            get { return _title; }
            set { _title = value;}
        }

        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual bool Playable
        {
            get { return false; }
        }
        public string Description
        {
            get { return _description; }
            set { this._description = value; }
        }
        public string Bilde
        {
            get { return _bilde; }
            set { this._bilde = value; }
        }
        public override string ToString()
        {
            return "(" + this.GetType().Name + ") " + this.Title;
        }

        public Item(string id, string title)
        {
            _id = id;
            _title = title;
        }
    }

    public class Category : Item
    {
        public Category(string id, string title) : base(id, title) { }
    }

    public class Program : Item
    {
        public Program(string id, string title, string description, string bilde) : base(id, title) { Description = description; Bilde = bilde; }
    }

    public class Folder : Item
    {
        public Folder(string id, string title) : base(id, title) { }
    }

    public class Clip : Item
    {
        public enum KlippType
        {
            KLIPP = 0,
            VERDI = 1,
            RSS = 2,
        }

        private string verdiLink;
        private string antallGangerVist;
        private string klokkeslett;
        // sier noe om hvilken måte man finner klipplinken
        private KlippType type = KlippType.KLIPP;

        public Clip(string id, string title) : base(id, title) { }

        public override bool Playable
        {
            get { return true; }
        }

        public string AntallGangerVist
        {
            get { return antallGangerVist; }
            set { this.antallGangerVist = value; }
        }

        public string VerdiLink
        {
            get { return verdiLink; }
            set { this.verdiLink = value; }
        }

        public string Klokkeslett
        {
            get { return klokkeslett; }
            set { this.klokkeslett = value; }
        }

        public KlippType Type
        {
            get { return type; }
            set { this.type = value; }
        }
    }

    public class MenuItem : Item
    {
        public MenuItem(string id, string title)
            : base(id, title)
        {
        }
    }

    public class Stream : Item
    {
        public Stream(string id, string title)
            : base(id, title)
        {
        }

        public override bool Playable
        {
            get { return true; }
        }
    }

    #endregion

    public class NrkParser
    {
        static string BASE_URL = "http://www1.nrk.no/";
        static string MAIN_URL = BASE_URL + "nett-tv/";
        static string CATEGORY_URL = MAIN_URL + "tema/";
        static string PROGRAM_URL = MAIN_URL + "prosjekt/";
        static string SETTINGS_URL = MAIN_URL + "innstillinger/";
        static string CLIP_URL = MAIN_URL + "klipp/";
        static string FOLDER_URL = MAIN_URL + "kategori/";
       

        CookieContainer _jar;
        int _speed;

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
                FetchUrl(NrkParser.MAIN_URL + "hastighet.aspx?hastighet=" + _speed + "&retururl=http://www1.nrk.no/nett-tv/");
            }
        }

        public List<Item> GetCategories()
        {
            List<Item> categories = new List<Item>();
            string data = FetchUrl(NrkParser.MAIN_URL);
            Regex query = new Regex("<a href=\"/nett-tv/tema/(\\w*).*?>(.*?)</a>");
            MatchCollection result = query.Matches(data);
            foreach (Match x in result)
            {
                if (x.Groups[2].Value != "Plusspakken")
                {
                    categories.Add(new Category(x.Groups[1].Value, x.Groups[2].Value));
                }
            }
            return categories;
        }

        public List<Item> GetForsiden()
        {
            Log.Info(NrkPlugin.PLUGIN_NAME + ": GetForsiden()");
            string data;
            data = FetchUrl(NrkParser.MAIN_URL);
            
            Regex query = new Regex("<div class=\"img-left\" style=\"width: 100px;\">.*?<a href=\".*?\" title=\".*?\"><img src=\"(.*?)\" width=\"100\" height=\"57\" alt=\".*?\" title=\".*?\".*?></a>.*?</div>.*?<div class=\"intro-element-content\"><h2><a href=\"/nett-tv/klipp/(.*?)\" title=\"(.*?)\">(.*?)</a></h2>", RegexOptions.Singleline);
            MatchCollection matches = query.Matches(data);
            List<Item> clips = new List<Item>();
            int bildeTeller = 0;
            Log.Info(NrkPlugin.PLUGIN_NAME + ": Matches {0}", matches.Count);
            foreach (Match x in matches)
            {
                Clip c = new Clip(x.Groups[2].Value, x.Groups[4].Value);
                c.Description = x.Groups[3].Value;
                c.Bilde = x.Groups[1].Value;
                clips.Add(c);
//                Log.Debug("Clip id: {0}", x.Groups[2].Value);
//                Log.Debug("Clip description: {0}", x.Groups[3].Value);
//                Log.Debug("Clip title: {0}", x.Groups[4].Value);
//                Log.Debug("Clip bilde: {0}", x.Groups[1].Value);
//                Log.Debug("-----");
                bildeTeller++;
            }

            return clips;
        }

        public List<Item> GetMestSettDenneUken()
        {
            Log.Info(NrkPlugin.PLUGIN_NAME + ": GetMestSettDenneUken()");
            string data;
            data = FetchUrl(NrkParser.MAIN_URL);

            Regex query = new Regex("<div class=\"img-left\" style=\"width: 150px;\"><a href=\"/nett-tv/klipp/.*?\" title=\".*?\"><img src=\"(.*?)\" alt=\"\" title=\"\" longdesc=\"\" height=\"85\" width=\"150\" /></a></div><h2><a href=\"/nett-tv/klipp/.*?\" title=\".*?\">.*?</a></h2><p><a href=\"/nett-tv/klipp/.*?\" title=\".*?\">Publisert .*?</a><br /><a href=\"/nett-tv/klipp/(.*?)\" title=\"(.*?)\">Vist (.*?) ganger</a>", RegexOptions.Singleline);
            MatchCollection matches = query.Matches(data);
            List<Item> clips = new List<Item>();
            int bildeTeller = 0;
            Log.Info(NrkPlugin.PLUGIN_NAME + ": Matches {0}", matches.Count);
            foreach (Match x in matches)
            {
                Clip c = new Clip(x.Groups[2].Value, x.Groups[3].Value);
                c.AntallGangerVist = x.Groups[4].Value;
                c.Description = String.Format("Klipp vist {0} ganger denne uken.", c.AntallGangerVist);
                c.Bilde = x.Groups[1].Value;
                clips.Add(c);
                bildeTeller++;
            }

            return clips;
        }

        public List<Item> GetTopTabRSS(string site)
        {
            List<Item> clips = new List<Item>();
            //natur clips uses flash
            XmlDocument doc = new XmlDocument();
            XmlTextReader reader = new XmlTextReader("http://www1.nrk.no/nett-tv/MediaRss.ashx?loop="+site);
            doc.Load(reader);
            XmlNamespaceManager expr = new XmlNamespaceManager(doc.NameTable);
            expr.AddNamespace("media", "http://search.yahoo.com/mrss");
            XmlNode root = doc.SelectSingleNode("//rss/channel/item", expr);
            XmlNodeList nodeList;
            nodeList = root.SelectNodes("//rss/channel/item");
            Clip loRssItem;
            //GUIListItem loListItem;
            
            int itemCount = 0;
            foreach (XmlNode chileNode in nodeList)
            {
                if (itemCount >= 100)
                {
                    break;
                }
                itemCount++;
                loRssItem = new Clip("", "");

                for (int i = 0; i < chileNode.ChildNodes.Count; i++)
                {
                    XmlNode n = chileNode.ChildNodes[i];

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
                                        catch (Exception) { };
                                        
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
                //372980 is the id for the short natur-vignett, and we do not want it in list
                //381994 is the for the super vignett
                if (loRssItem.ID != "372980" && loRssItem.ID != "381994")
                {
                    loRssItem.Type = Clip.KlippType.RSS;
                    clips.Add(loRssItem);
                }
                
            }
            return clips;
        }

        public List<Item> GetTopTabber(String tab)
        {
            Log.Info(NrkPlugin.PLUGIN_NAME + ": GetTopTabber(String)", tab);
            string data;
            data = FetchUrl(NrkParser.MAIN_URL + tab + "/");
            Regex query = null;
            if (tab == "sport")
            {
                query =
                    new Regex(
                        "<div class=\"img-left\" style=\"width: 120px;\">.*?<a href=\".*?\" onclick=\"return true;\"><img src=\"(.*?)\" alt=\".*?\" title=\".*?\" width=\"120\" height=\"68\".*?></a>.*?</div>.*?<div class=\"active\"><h2><a href=\"http://www1.nrk.no/nett-tv/sport/spill/verdi/(.*?)\" onclick=\"return true;\">(.*?)</a></h2>",
                        RegexOptions.Singleline);
            }
            if (tab == "nyheter")
            {
                query =
                    new Regex(
                        "<div class=\"img-left\" style=\"width: 120px;\">.*?<a href=\".*?\" onclick=\"return true;\"><img src=\"(.*?)\" alt=\".*?\" title=\".*?\" width=\"120\" height=\"68\".*?></a>.*?</div>.*?<div class=\"active\"><h2><a href=\"http://www1.nrk.no/nett-tv/nyheter/spill/verdi/(.*?)\" onclick=\"return true;\">(.*?)</a></h2>",
                        RegexOptions.Singleline);
                
            }
            if (tab == "natur")
            {
                query =
                    new Regex(
                        "<div class=\"img-left\" style=\"width: 120px;\">.*?<a href=\".*?\" onclick=\"return true;\"><img src=\"(.*?)\" alt=\".*?\" title=\".*?\" width=\"120\" height=\"68\".*?></a>.*?</div>.*?<div class=\"active\"><h2><a href=\"http://www1.nrk.no/nett-tv/natur/spill/verdi/(.*?)\" onclick=\"return true;\">(.*?)</a></h2>",
                        RegexOptions.Singleline);

            }
            if (tab == "ol")
            {
                query =
                    new Regex(
                        "<div class=\"img-left\" style=\"width: 120px;\">.*?<a href=\".*?\" onclick=\"return true;\"><img src=\"(.*?)\" alt=\".*?\" title=\".*?\" width=\"120\" height=\"68\".*?></a>.*?</div>.*?<div class=\"active\"><h2><a href=\"http://www1.nrk.no/nett-tv/ol/spill/verdi/(.*?)\" onclick=\"return true;\">(.*?)</a></h2>",
                        RegexOptions.Singleline);

            }
            MatchCollection matches = query.Matches(data);
            List<Item> clips = new List<Item>();
            int bildeTeller = 0;
            Log.Info(NrkPlugin.PLUGIN_NAME + ": Matches {0}", matches.Count);
            foreach (Match x in matches)
            {
                string klokkeslett = parseKlokkeSlettFraBilde(x.Groups[1].Value);
                Clip c = new Clip(x.Groups[2].Value, x.Groups[3].Value);
                c.Bilde = x.Groups[1].Value;
                c.Description = "Lagt ut " + klokkeslett;
                c.Type = Clip.KlippType.VERDI;
                c.VerdiLink = tab;
                clips.Add(c);
                bildeTeller++;
            }

            return clips;
        }

        private string parseKlokkeSlettFraBilde(string bildeUrl)
        {
            try
            {
                string temp = "";

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
                return time + ":" + minutt + " " + tab[2] + "/" + tab[1] + "-"+ tab[0];
            }
            catch(Exception)
            {
                Log.Info(NrkPlugin.PLUGIN_NAME + ": Could not parse date from image filename, but that is fine...just returning a blank string");
                return ""; 
            }
        }

        public List<Item> GetPrograms(Category category)
        {
            string data;
            
                data = FetchUrl(NrkParser.CATEGORY_URL + category.ID);
          
           
            //Regex query = new Regex("<a href=\"/nett-tv/prosjekt/(.*?)\" id=\".*?\" title=\"(.*?)\".*?>\\s+<img src=\"(.*?)\" id=");

            Regex queryBilder = new Regex("<a href=\"/nett-tv/prosjekt/(.*?)\" id=\".*?\" title=\"(.*?)\".*?>\\s+<img src=\"(.*?)\" id=");
            MatchCollection matchesBilder = queryBilder.Matches(data);
            List<string> bilder = new List<string>();
            foreach (Match x in matchesBilder)
            {
                bilder.Add(x.Groups[3].Value);
            }

            Regex query = new Regex("<a href=\"/nett-tv/prosjekt/(.*?)\" id=\".*ClipLink\" title=\"(.*?)\".*? onclick=\".*?\">(.*?)</a>");
            MatchCollection matches = query.Matches(data);
            List<Item> programs = new List<Item>();
            int bildeTeller = 0;
            foreach (Match x in matches)
            {
                programs.Add(new Program(x.Groups[1].Value, x.Groups[3].Value, "", bilder[bildeTeller]));
                bildeTeller++;
            }

            Regex query2 = new Regex("<a href=\"/nett-tv/prosjekt/(.*?)\" id=\".*ClipTitle\" title=\"(.*?)\".*?>(.*?)</a>");
            MatchCollection matches2 = query2.Matches(data);
            int teller = 0;
            foreach (Match x in matches2)
            {
                ((Program)programs[teller]).Description = x.Groups[3].Value;
                teller++;
            }
            return programs;
        }

        public List<Item> GetAllPrograms()//all programs
        {
            string data = FetchUrl(NrkParser.MAIN_URL + "bokstav/@");
            //Regex query = new Regex("<a href=\"/nett-tv/prosjekt/(.*?)\" id=\".*?\" title=\"(.*?)\".*?>\\s+<img src=\"(.*?)\" id=");
            Regex query = new Regex("<div class=\"intro-element intro-element-small\">.*?<div.*?>.*?<a href=\"/nett-tv/prosjekt/(.*?)\" id=\".*?\" title=\"(.*?)\".*?>.*?<img src=\"(.*?)\".*?>.*?</a>.*?</div>.*?<h2>.*?<a href=\".*?\" id=\".*?\" title=\".*?\" onclick=\".*?\">(.*?)</a>.*?</h2>.*?<p>.*?</p>.*?</div>", RegexOptions.Singleline);
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
            string data = FetchUrl(NrkParser.PROGRAM_URL + program.ID);
            Regex query = new Regex("<a id=\".*?\" href=\"/nett-tv/kategori/(.*?)\".*?title=\"(.*?)\".*?class=\"icon-(.*?)-black\".*?>(.*?)</a>");
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
            string data = FetchUrl(NrkParser.FOLDER_URL + folder.ID);
            Regex query = new Regex("<a id=\".*?\" href=\"/nett-tv/kategori/(.*?)\".*?title=\"(.*?)\".*?class=\"icon-(.*?)-black\".*?>(.*?)</a>");
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
            string data = FetchUrl(NrkParser.PROGRAM_URL + program.ID);
            List<Item> clips = new List<Item>();
            Regex query = new Regex("<a href=\"/nett-tv/klipp/(.*?)\"\\s+title=\"(.*?)\"\\s+class=\"(.*?)\".*?>(.*?)</a>");
            MatchCollection matches = query.Matches(data);
            foreach (Match x in matches)
            {
                clips.Add(new Clip(x.Groups[1].Value, x.Groups[4].Value));
            }

            return clips;
        }

        public List<Item> GetClips(Folder folder)
        {
            string data = FetchUrl(NrkParser.FOLDER_URL + folder.ID);

            List<Item> clips = new List<Item>();

            Regex query = new Regex("<a href=\"/nett-tv/klipp/(.*?)\"\\s+title=\"(.*?)\"\\s+class=\"(.*?)\".*?>(.*?)</a>");
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
            
            Regex query;
            if (clip.Type == Clip.KlippType.KLIPP)
            {
                Log.Debug(NrkPlugin.PLUGIN_NAME + ": Clip type is KLIPP");
                string data = FetchUrl(NrkParser.CLIP_URL + clip.ID);
                query = new Regex("<param name=\"FileName\" value=\"(.*?)\" />", RegexOptions.IgnoreCase);
                MatchCollection result = query.Matches(data);
                string urlToFetch = result[0].Groups[1].Value;
                string urldata = FetchUrl(urlToFetch);
                Log.Debug(NrkPlugin.PLUGIN_NAME +": " + urldata);
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
                catch { return movie_url[0].Groups[1].Value; }
            }
            else if (clip.Type == Clip.KlippType.RSS)
            {
                Log.Debug(NrkPlugin.PLUGIN_NAME + ": Clip type is RSS");
                return "http://pd.nrk.no/nett-tv-stream/stream.ashx?id=" + clip.ID;
            }
            else
            {
                //clip.Type == Clip.KlippType.VERDI;
                Log.Info(NrkPlugin.PLUGIN_NAME + ": Fetching verdi url");
                string data = FetchUrl(NrkParser.MAIN_URL + clip.VerdiLink +"/spill/verdi/" + clip.ID); //ser ut som om det er det samme om det står nyheter, sport osv før spill
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
            
            
        }

        string FetchUrl(string url)
        {
            Log.Info(NrkPlugin.PLUGIN_NAME + ": FetchUrl(String): " + url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)";
            request.CookieContainer = _jar;
            // Set some reasonable limits on resources used by this request
            request.MaximumAutomaticRedirections = 4;
            request.MaximumResponseHeadersLength = 4;
            // Set credentials to use for this request.
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Get the stream associated with the response.
            System.IO.Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            string ret = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
            return ret;
        }

        /* Test function.
         * Will simply call the different functions and print out the result
         */
        public static void Main()
        {
            NrkParser nrk = new NrkParser(900);

            List<Item> clips = nrk.GetForsiden();
            foreach(Item clip in clips)
            {
                System.Console.WriteLine(clip.ID);
            }

          /*  List<Item> categories = nrk.GetCategories();
            foreach (Item category in categories)
            {
                System.Console.WriteLine(category.ID + " " + category.Title);
            }
            System.Console.WriteLine();

            List<Item> programs = nrk.GetPrograms((Category)categories[1]);
            foreach (Program program in programs)
            {
                System.Console.WriteLine(program.ID + " " + program.Title);
            }
            System.Console.WriteLine();

            List<Item> clips = nrk.GetClips((Program)programs[0]);//list episodes
            foreach (Clip clip in clips)
            {
                System.Console.WriteLine(clip.ID + " " + clip.Title);
            }
            System.Console.WriteLine();

            List<Item> folders = nrk.GetFolders((Program)programs[0]);//list folders (ie. 2006, january etc)
            foreach (Folder folder in folders)
            {
                System.Console.WriteLine(folder.ID + " " + folder.Title);
            }
            System.Console.WriteLine();

            clips = nrk.GetClips((Folder)folders[1]);//list episodes
            foreach (Clip clip in clips)
            {
                System.Console.WriteLine(clip.ID + " " + clip.Title);
            }
            System.Console.WriteLine();

            string url = nrk.GetClipUrl((Clip)clips[0]);
            System.Console.WriteLine(url);

            programs = nrk.GetAllPrograms();//alphabetical list
            foreach (Program program in programs)
            {
                System.Console.WriteLine(program.ID + " " + program.Title);
            }
            System.Console.WriteLine();
            */

            System.Console.WriteLine("Press enter to quit");
            System.Console.Read();
        }
    }

}