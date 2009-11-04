using System;
using System.Collections.Generic;
using NrkBrowser.Domain;
using NUnit.Framework;

namespace NrkBrowser.Xml
{
    [TestFixture]
    public class XMLKlippIntegrationTest
    {
        const string ID = "569191";
        

        [Test]
        public void GetUrlAtSpeed10000()
        {
            int hastighet = 10000;
            XmlKlippParser parser = new XmlKlippParser(string.Format(NrkConstants.URL_GET_MEDIAXML, ID, hastighet));
            String url = parser.GetUrl();
            Assert.AreEqual("mms://straumod.nrk.no/disk05/Puls/2009-11-02/Puls_02_11_09_1000_569191_20091102_193000.wmv", url);

        }

        [Test]
        public void GetUrlAtSpeed700()
        {
            int hastighet = 700;
            XmlKlippParser parser = new XmlKlippParser(string.Format(NrkConstants.URL_GET_MEDIAXML, ID, hastighet));
            String url = parser.GetUrl();
            Assert.AreEqual("mms://straumod.nrk.no/disk05/Puls/2009-11-02/Puls_02_11_09_600_569191_20091102_193000.wmv", url);

        }

        [Test]
        public void GetUrlAtSpeed500()
        {
            int hastighet = 500;
            XmlKlippParser parser = new XmlKlippParser(string.Format(NrkConstants.URL_GET_MEDIAXML, ID, hastighet));
            String url = parser.GetUrl();
            Assert.AreEqual("mms://straumod.nrk.no/disk05/Puls/2009-11-02/Puls_02_11_09_300_569191_20091102_193000.wmv", url);

        }

        [Test]
        public void GetStartTime()
        {
            int hastighet = 500;
            XmlKlippParser parser = new XmlKlippParser(string.Format(NrkConstants.URL_GET_MEDIAXML, ID, hastighet));
            int startTime = parser.GetStartTimeOfClip();
            Assert.AreEqual(0, startTime);

        }



    }
}