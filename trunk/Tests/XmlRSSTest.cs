using System;
using System.Collections.Generic;
using Vattenmelon.Nrk.Domain;
using NUnit.Framework;
using Vattenmelon.Nrk.Parser.Http;

namespace Vattenmelon.Nrk.Parser.Xml
{
    [TestFixture]
    public class XMLRSSTest
    {

        [Test]
        public void GetTopTabRSS()
        {
            
            XmlRSSParser parser = new XmlRSSParser("../../stubfiler/site_", "NATUR.xml");
            List<Item> natur = parser.getClips();
            sjekkTopTabRSSClips(natur);

        }


        private void sjekkTopTabRSSClips(List<Item> liste)
        {
            Assert.IsNotNull(liste, "Listen kan ikke være null");
            Assert.IsTrue(liste.Count > 0, "Listen skal være større enn 0");
            foreach (Item item in liste)
            {
                Clip c = (Clip)item;
                Assert.IsNotNull(c.ID, "ID'en kan ikke være null");
                Assert.IsNotNull(c.Title, "Tittelen kan ikke være null");
                Assert.IsTrue(c.Playable, "Klipp må være playable");
            }
        }

        [Test]
        public void TestGetNrkBetaTVSerier()
        {
            NrkBetaXmlParser parser = new NrkBetaXmlParser(NrkParserConstants.NRK_BETA_FEEDS_KATEGORI_URL, NrkParserConstants.NRK_BETA_SECTION_TV_SERIES);
            IHttpClient stubClient = new StubHttpClient();
            parser.HttpClient = stubClient;
            List<Item> items = parser.getClips();
            Assert.IsNotEmpty(items);
            foreach (Item item in items)
            {
                AssertNrkBetaClip(item);
            }

        }

        //duplicate code
        private static void AssertNrkBetaClip(Item item)
        {
            Clip c = (Clip)item;
            //            Console.WriteLine("Tittel.............: " + c.Title);
            //            Console.WriteLine("ID.................: " + c.ID);
            //            Console.WriteLine("Beskrivelse........: " + c.Description);
            //            Console.WriteLine("Bilde..............: " + c.Bilde);
            //            Console.WriteLine("Type...............: " + c.Type);
            //            Console.WriteLine("Antall ganger vist.: " + c.AntallGangerVist);
            //            Console.WriteLine("Klokkeslett........: " + c.Klokkeslett);
            //            Console.WriteLine("-----------------------------------------");
            Assert.IsNotEmpty(c.Title);
            Assert.IsNotEmpty(c.ID);
            Assert.IsNotEmpty(c.Description);
            Assert.IsNotEmpty(c.Bilde);
            Assert.AreEqual(Clip.KlippType.NRKBETA, c.Type);
        }
      
    }
}