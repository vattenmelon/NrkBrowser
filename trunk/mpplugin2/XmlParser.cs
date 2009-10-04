using System.Xml;

namespace NrkBrowser.Xml
{
    public class XmlParser
    {
        protected XmlDocument doc;
        protected string url;

        protected void LoadXmlDocument()
        {
            doc = new XmlDocument();
            XmlTextReader reader = new XmlTextReader(url);
            doc.Load(reader);
        }

    }
}
