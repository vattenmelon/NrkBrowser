using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using Vattenmelon.Nrk.Parser;
using Vattenmelon.Nrk.Domain;
using Vattenmelon.Nrk.Parser.Http;

namespace Vattenmelon.Nrk.Parser.Xml
{
    public class PodkastXmlParser : XmlRSSParser
    {
        private XmlNamespaceManager manager;

//        public PodkastXmlParser(string siteurl, string section)
//            : base(siteurl, section)
//        {
//            
//        }

        public PodkastXmlParser(String urltopodkast)
            : base("", "")
        {
            url = urltopodkast;
        }


//        override protected void LoadXmlDocument()
//        {
//            doc = new XmlDocument();
//            httpClient = GetHttClient();
//            String xmlAsString = httpClient.GetUrl(url);
//            doc.LoadXml(xmlAsString);
//        }

//        override protected string GetPicture(Clip clip)
//        {
//            String bilde = NrkParserConstants.NRK_BETA_THUMBNAIL_URL;
//            String videoFileName = clip.ID.Substring(clip.ID.LastIndexOf("/") + 1);
//            return string.Format(bilde, videoFileName);
//        }
//
//        override protected Clip.KlippType GetClipType()
//        {
//            return Clip.KlippType.NRKBETA;
//        }
//
//        override protected XmlNodeList GetNodeList()
//        {
//            return doc.GetElementsByTagName("entry");
//        }
//
//        override protected void PutLinkOnItem(Clip clip, XmlNode node)
//        {
//            clip.ID = node.Attributes["href"].Value;
//        }
//
//        override protected void PutSummaryOnItem(Clip clip, XmlNode node)
//        {
//            clip.Description = node.InnerText;
//        }

        protected override void PutPublicationDateOnItem(Clip item, XmlNode n)
        {
            item.Klokkeslett = n.InnerText;
        }

        protected override void PutDurationOnItem(Clip item, XmlNode n)
        {
            item.Duration = n.InnerText;
        }
        protected override void LoadXmlDocument()
        {
            if (doc == null)
            {
                InternalLoadXmlDocument();
                XPathNavigator xPathNavigator = doc.CreateNavigator();
                manager = new XmlNamespaceManager(xPathNavigator.NameTable);
                manager.AddNamespace("itunes", "http://www.itunes.com/dtds/podcast-1.0.dtd");
            }
        }

        override protected void PutGuidOnItem(Clip loRssItem, XmlNode n)
        {
            
        }

        protected override void PutEnclosureOnItem(Clip item, XmlNode n)
        {
            item.ID = n.Attributes["url"].Value;
            item.MediaType = n.Attributes["type"].Value;
        }

        public string getPodkastPicture()
        {
            return GetSingleNodeValue("//rss/channel/image/url");
        }

        public string getPodkastDescription()
        {
            return GetSingleNodeValue("//rss/channel/description");
        }

        public string getPodkastCopyright()
        {
            return GetSingleNodeValue("//rss/channel/copyright");
        }

        public string getPodkastAuthor()
        {
            LoadXmlDocument();
            return doc.SelectSingleNode("//rss/channel/itunes:author", manager).FirstChild.Value;
        }

        private string GetSingleNodeValue(String path)
        {
            LoadXmlDocument();
            return doc.SelectSingleNode(path).FirstChild.Value;
        }
    }
}
