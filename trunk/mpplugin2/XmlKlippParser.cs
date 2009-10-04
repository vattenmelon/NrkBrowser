using System;
using System.Xml;


namespace NrkBrowser.Xml 
{
    public class XmlKlippParser : XmlParser
    {
        public XmlKlippParser(String url)
        {
            base.url = url;
            LoadXmlDocument();
        }
        
        public String GetUrl()
        { 
            XmlNamespaceManager expr = new XmlNamespaceManager(doc.NameTable);
            XmlNode abba = doc.SelectSingleNode("//mediadefinition/mediaitems/mediaitem/mediaurl");
            return abba.FirstChild.Value;
        }
        public int GetStartTimeOfClip()
        {
            XmlNamespaceManager expr = new XmlNamespaceManager(doc.NameTable);
            XmlNode abba = doc.SelectSingleNode("//mediadefinition/mediaitems/mediaitem/timeindex");
            String strStartTime = abba.FirstChild.Value;
            int startTimeToReturn = 0;
            if (!String.IsNullOrEmpty(strStartTime))
            {
                startTimeToReturn = Int32.Parse(strStartTime);
            }
            return startTimeToReturn;
        }
    }
}
