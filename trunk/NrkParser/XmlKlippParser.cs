using System;
using System.Collections.Generic;
using System.Xml;
using Vattenmelon.Nrk.Domain;

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

        public List<Clip> GetChapters()
        {
            List<Clip> clips = new List<Clip>();
            String clipUrl = GetUrl();
            XmlNodeList nodeList = doc.SelectNodes("//mediadefinition/mediaitems/mediaitem/chapters/chapteritem");
            foreach (XmlNode xmlNode in nodeList)
            {
                Clip clip = new Clip(clipUrl, xmlNode["title"].InnerText);
                clip.StartTime = Double.Parse(xmlNode["timeindex"].InnerText);
                clip.Type = Clip.KlippType.NRKBETA; //XXX: ikke spesielt bra å bruke denne typen her..burde vært en generell klipptype for "ferdig-parsede" url'er.
                clips.Add(clip);
            }
            return clips;
        }
    }
}
