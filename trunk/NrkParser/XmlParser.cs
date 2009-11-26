using System.Xml;
using System;

namespace Vattenmelon.Nrk.Parser.Xml
{
    public class XmlParser
    {
        protected XmlDocument doc;
        protected string url;

        virtual protected void LoadXmlDocument()
        {
            doc = new XmlDocument();
            XmlTextReader reader = new XmlTextReader(url);
            doc.Load(reader);
            doc.Save("natur.xml");
            reader.Close();
        }

    }
}
