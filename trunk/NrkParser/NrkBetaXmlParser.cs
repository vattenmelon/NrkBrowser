using System;
using System.Collections.Generic;
using System.Xml;
using Vattenmelon.Nrk.Parser;
using Vattenmelon.Nrk.Domain;
using Vattenmelon.Nrk.Parser.Http;

namespace Vattenmelon.Nrk.Parser.Xml
{
    public class NrkBetaXmlParser
    {
        protected XmlDocument doc;
        protected string url;

        public NrkBetaXmlParser(string siteurl, string section)
        {
            url = siteurl + section;
        }

        protected void LoadXmlDocument()
        {
            doc = new XmlDocument();
            HttpClient httpClient = new HttpClient(new System.Net.CookieContainer());
            String xmlAsString = httpClient.GetUrl(url);
            doc.LoadXml(xmlAsString);
        }

        public List<Item> getClips()
        {
            LoadXmlDocument();
            XmlNodeList nodeList = GetNodeList();
            List<Item> clips = new List<Item>();
            int itemCount = 0;
            foreach (XmlNode childNode in nodeList)
            {
                Clip loRssItem = CreateClipFromChildNode(childNode);
                // loRssItem.Bilde = GetPictureForSection();
                if (NrkParser.isNotShortVignett(loRssItem))
                {
                    loRssItem.Type = Clip.KlippType.NRKBETA;
                    clips.Add(loRssItem);
                }

                itemCount++;
                if (isItemCount100orOver(itemCount))
                {
                    //Log.Info(string.Format("{0}: Over 100 clips in document, breaking.", NrkParserConstants.PLUGIN_NAME));
                    break;
                }

            }
            return clips;
        }

        private static bool isItemCount100orOver(int itemCount)
        {
            return itemCount >= 100;
        }

        private XmlNodeList GetNodeList()
        {
            return doc.GetElementsByTagName("entry");
        }

        private static Clip CreateClipFromChildNode(XmlNode childNode)
        {
            Clip loRssItem = new Clip("", "");
            for (int i = 0; i < childNode.ChildNodes.Count; i++)
            {
                XmlNode n = childNode.ChildNodes[i];

                switch (n.Name)
                {
                    case "title":
                        loRssItem.Title = n.InnerText;
                        break;
                    case "link":
                        loRssItem.ID = n.Attributes["href"].Value;
                        break;
                    case "guid":
                        loRssItem.ID = n.InnerText;
                        break;
                    case "pubDate":

                        break;
                    case "summary":
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
                                        //Log.Error("catccccccccccched exception: " + e.Message);
                                        Console.Error.WriteLine(e.StackTrace);
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
