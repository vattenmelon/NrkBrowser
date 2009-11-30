using System;
using System.Collections.Generic;
using NUnit.Framework;
using Vattenmelon.Nrk.Domain;

namespace Vattenmelon.Nrk.Parser.Xml
{
    [TestFixture]
    [Category("Unit Tests")]
    public class XMLKlippTest
    {
        const string ID = "569191";


        [Test]
        public void GetUrlAtSpeed10000()
        {
            XmlKlippParser parser =
                new XmlKlippParser("../../stubfiler/XmlKlippTest.xml");
            String url = parser.GetUrl();
            Assert.AreEqual(
                "mms://straumod.nrk.no/disk05/Puls/2009-11-02/Puls_02_11_09_1000_569191_20091102_193000.wmv", url);
        }

        [Test]
        public void GetStartTimeWhenItIsNotZero()
        {
            XmlKlippParser parser = new XmlKlippParser("../../stubfiler/geturl571151.xml");
            int startTime = parser.GetStartTimeOfClip();
            Assert.AreEqual(480, startTime);

        }

        [Test]
        public void GetChapters()
        {
            XmlKlippParser parser = new XmlKlippParser("../../stubfiler/getklippurlmedkapitler.xml");
            List<Clip> chapters = parser.GetChapters();
            Assert.AreEqual(3, chapters.Count);
            foreach (Clip clip in chapters)
            {
                Assert.IsNotEmpty(clip.ID, "Id skal ikke være blank");
                Assert.IsNotEmpty(clip.Title, "Title skal ikke være blank");
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