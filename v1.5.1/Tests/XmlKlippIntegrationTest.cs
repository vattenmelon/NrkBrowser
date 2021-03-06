using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vattenmelon.Nrk.Domain;

namespace Vattenmelon.Nrk.Parser.Xml
{
    [TestFixture]
    [Category("Integration Tests")]
    public class XMLKlippIntegrationTest
    {
        const string ID = "569191";
        

        [Test]
        public void GetUrlAtSpeed10000()
        {
            int hastighet = 10000;
            XmlKlippParser parser = new XmlKlippParser(string.Format(NrkParserConstants.URL_GET_MEDIAXML, ID, hastighet));
            String url = parser.GetUrl();
            Assert.AreEqual("mms://straumod.nrk.no/disk05/Puls/2009-11-02/Puls_02_11_09_1000_569191_20091102_193000.wmv", url);

        }

        [Test]
        public void GetUrlAtSpeed700()
        {
            int hastighet = 700;
            XmlKlippParser parser = new XmlKlippParser(string.Format(NrkParserConstants.URL_GET_MEDIAXML, ID, hastighet));
            String url = parser.GetUrl();
            Assert.AreEqual("mms://straumod.nrk.no/disk05/Puls/2009-11-02/Puls_02_11_09_600_569191_20091102_193000.wmv", url);

        }

        [Test]
        public void GetUrlAtSpeed500()
        {
            int hastighet = 500;
            XmlKlippParser parser = new XmlKlippParser(string.Format(NrkParserConstants.URL_GET_MEDIAXML, ID, hastighet));
            String url = parser.GetUrl();
            Assert.AreEqual("mms://straumod.nrk.no/disk05/Puls/2009-11-02/Puls_02_11_09_300_569191_20091102_193000.wmv", url);

        }

        [Test]
        public void GetStartTimeWhenItIsZero()
        {
            int hastighet = 500;
            XmlKlippParser parser = new XmlKlippParser(string.Format(NrkParserConstants.URL_GET_MEDIAXML, ID, hastighet));
            int startTime = parser.GetStartTimeOfClip();
            Assert.AreEqual(0, startTime);

        }

        [Test]
        public void GetStartTimeWhenItIsNotZero()
        {
            //Starttime verified to be 480 on the 29.11.2009
            int hastighet = 500;
            int clipId = 571151;
            XmlKlippParser parser = new XmlKlippParser(string.Format(NrkParserConstants.URL_GET_MEDIAXML, clipId, hastighet));
            int startTime = parser.GetStartTimeOfClip();
            Assert.AreEqual(480, startTime);

        }

        [Test]
        public void GetChapters()
        {
            //http://www1.nrk.no/nett-tv/silverlight/getmediaxml.ashx?id=571150, eksempel p� en stream med kapitler
            int hastighet = 350;
            int clipId = 571150;
            XmlKlippParser parser = new XmlKlippParser(string.Format(NrkParserConstants.URL_GET_MEDIAXML, clipId, hastighet));
            List<Clip> chapters = parser.GetChapters();
            Assert.AreEqual(3, chapters.Count);
            foreach (Clip clip in chapters)
            {
                Assert.IsNotEmpty(clip.ID, "Id skal ikke v�re blank");
                Assert.IsNotEmpty(clip.Title, "Title skal ikke v�re blank");
                Assert.IsNotEmpty(clip.Bilde);
                //                Console.WriteLine("id...: " + clip.ID);
                //                Console.WriteLine("title: " + clip.Title);
                //                Console.WriteLine("start: " + clip.StartTime);
                //                Console.WriteLine("desc.: " + clip.Description);
                //                Console.WriteLine("bilde: " + clip.Bilde);
            }

        }
    }
}