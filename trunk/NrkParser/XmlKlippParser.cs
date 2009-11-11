using System;
using System.Xml;


namespace Vattenmelon.Nrk.Parser.Xml 
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
            XmlNode abba = doc.SelectSingleNode("//mediadefinition/mediaitems/mediaitem/mediaurl");
            return abba.FirstChild.Value;
        }
        public int GetStartTimeOfClip()
        {
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
