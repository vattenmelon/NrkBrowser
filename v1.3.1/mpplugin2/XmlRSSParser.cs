using System;
using System.Collections.Generic;
using System.Xml;
using MediaPortal.ServiceImplementations;
using NrkBrowser.Domain;

namespace NrkBrowser.RSS
{
    public class XmlRSSParser
    {
        private string section;
        private string siteurl;
        public XmlRSSParser(String siteurl, String section)
        {
            this.section = section;
            this.siteurl = siteurl;
        }

        public List<Item> getClips()
        {
            List<Item> clips = new List<Item>();
            XmlDocument doc = new XmlDocument();
            XmlTextReader reader = new XmlTextReader(siteurl + section);
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

                if (NrkParser.isNotShortVignett(loRssItem))
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
    }
}
