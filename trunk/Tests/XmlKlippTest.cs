using System;
using System.Xml;
using NUnit.Framework;

namespace Vattenmelon.Nrk.Parser.Xml
{
    [TestFixture]
    public class XMLKlippTest
    {
        const string ID = "569191";


        [Test]
        public void GetUrlAtSpeed10000()
        {
            int hastighet = 10000;
            XmlKlippParserStub parser =
                new XmlKlippParserStub(string.Format(NrkParserConstants.URL_GET_MEDIAXML, ID, hastighet));
            String url = parser.GetUrl();
            Assert.AreEqual(
                "mms://straumod.nrk.no/disk05/Puls/2009-11-02/Puls_02_11_09_1000_569191_20091102_193000.wmv", url);
        }
    }

    internal class XmlKlippParserStub : XmlKlippParser
    {
        public XmlKlippParserStub(String url) : base(url)
        {
            
        }
        override protected void LoadXmlDocument()
        {
            doc = new XmlDocument();
            XmlTextReader reader = new XmlTextReader("../../stubfiler/XmlKlippTest.xml");
            doc.Load(reader);
            reader.Close();
        }
    }
}