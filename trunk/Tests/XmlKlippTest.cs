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
            XmlKlippParser parser =
                new XmlKlippParser("../../stubfiler/XmlKlippTest.xml");
            String url = parser.GetUrl();
            Assert.AreEqual(
                "mms://straumod.nrk.no/disk05/Puls/2009-11-02/Puls_02_11_09_1000_569191_20091102_193000.wmv", url);
        }
    }
}