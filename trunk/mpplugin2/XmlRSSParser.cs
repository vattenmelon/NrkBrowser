using System;
using System.Collections.Generic;
using System.Xml;
//using MediaPortal.ServiceImplementations;
using NrkBrowser.Domain;

namespace NrkBrowser.Xml
{
    public class XmlRSSParser : XmlParser
    {
 
        public XmlRSSParser(string siteurl, string section)
        {
            base.url = siteurl + section;
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
                if (NrkParser.isNotShortVignett(loRssItem))
                {
                    loRssItem.Type = Clip.KlippType.RSS;
                    clips.Add(loRssItem);
                }
                
                itemCount++;
                if (isItemCount100orOver(itemCount))
                {
                    //Log.Info(string.Format("{0}: Over 100 clips in document, breaking.", NrkConstants.PLUGIN_NAME));
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
            return doc.SelectNodes("//rss/channel/item");
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
