using System;
using System.Collections.Generic;
using NrkBrowser.Domain;
using NUnit.Framework;

namespace NrkBrowser.Xml
{
    [TestFixture]
    public class XMLKlippIntegrationTest
    {
        const string ID = "556191";
        

        [Test]
        public void GetUrlAtSpeed10000()
        {
            int hastighet = 10000;
            XmlKlippParser parser = new XmlKlippParser(string.Format(NrkConstants.URL_GET_MEDIAXML, ID, hastighet));
            String url = parser.GetUrl();
            Assert.AreEqual("mms://straumod.nrk.no/disk08/skavlan/2009-10-02/Skavlan_02_10_09_1000_556191_20091002_212500.wmv", url);

        }

        [Test]
        public void GetUrlAtSpeed700()
        {
            int hastighet = 700;
            XmlKlippParser parser = new XmlKlippParser(string.Format(NrkConstants.URL_GET_MEDIAXML, ID, hastighet));
            String url = parser.GetUrl();
            Assert.AreEqual("mms://straumod.nrk.no/disk08/skavlan/2009-10-02/Skavlan_02_10_09_600_556191_20091002_212500.wmv", url);

        }

        [Test]
        public void GetUrlAtSpeed500()
        {
            int hastighet = 500;
            XmlKlippParser parser = new XmlKlippParser(string.Format(NrkConstants.URL_GET_MEDIAXML, ID, hastighet));
            String url = parser.GetUrl();
            Assert.AreEqual("mms://straumod.nrk.no/disk08/skavlan/2009-10-02/Skavlan_02_10_09_300_556191_20091002_212500.wmv", url);

        }

        [Test]
        public void GetStartTime()
        {
            int hastighet = 500;
            XmlKlippParser parser = new XmlKlippParser(string.Format(NrkConstants.URL_GET_MEDIAXML, ID, hastighet));
            int startTime = parser.GetStartTimeOfClip();
            Assert.AreEqual(87, startTime);

        }



    }
}