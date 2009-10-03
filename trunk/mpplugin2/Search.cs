using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MediaPortal.ServiceImplementations;
using NrkBrowser.Domain;

namespace NrkBrowser
{
    public class Search
    {
        private List<Item> searchHits;

        public List<Item> SearchHits
        {
            get { return searchHits; }
        }

        public Search(String htmlData)
        {
            string regexQuery = "<li id=\"ctl00.*?\">.*?";
            regexQuery += "<a href=\"(.*?)\" id=\".*?\" style=\".*?\" title=\".*?\" class=\"(.*?)\">(.*?)</a>.*?";
            regexQuery += "<div.*?>(.*?)</div>.*?";
            regexQuery += "</li>";

            Regex query = new Regex(regexQuery, RegexOptions.Singleline);
            MatchCollection result = query.Matches(htmlData);
            Log.Info(string.Format("Matches found in search: {0}", result.Count));
            searchHits = Search.GetSearchHits(result);
        }

        public static List<Item> GetSearchHits(MatchCollection result)
        {
            List<Item> categories = new List<Item>();
            foreach (Match x in result)
            {
                addItemFromSearchHitToList(categories, x);
            }
            return categories;
        }

        private static void addItemFromSearchHitToList(List<Item> items, Match x)
        {
            String link = x.Groups[1].Value;
            String type = x.Groups[2].Value;
            String title = string.Format("{0}", x.Groups[3].Value);
            String description = x.Groups[4].Value;

            String id = link.Substring(x.Groups[1].Value.LastIndexOf("/", x.Groups[1].Value.Length) + 1);

            if (type.Equals("video-index") || type.Equals("audio-index"))
            {
                Clip c = CreateIndexClip(id, title, x);
                items.Add(c);
            }
            else if (type.Equals("video") || type.Equals("audio"))
            {
                Clip c = CreateClip(id, title, x);
                items.Add(c);
            }
            else if (type.Equals("project"))
            {
                Program p = new Program(id, title, description, "");
                items.Add(p);
            }
            else if (type.Equals("folder"))
            {
                Folder f = new Folder(id, title);
                f.Description = x.Groups[4].Value;
                items.Add(f);
            }
            else
            {
                Console.WriteLine("feil: " + type);
                Log.Error(NrkConstants.PLUGIN_NAME + ": unsupported type: " + x.Groups[2].Value);
            }
        }

        private static Clip CreateClip(string id, string title, Match x)
        {
            Clip c = new Clip(id, title);
            c.Description = x.Groups[4].Value;
            return c;
        }

        private static Clip CreateIndexClip(string id, string title, Match x)
        {
            Clip c = CreateClip(id, title, x);
            c.Type = Clip.KlippType.INDEX;
            return c;
        }
    }
}
