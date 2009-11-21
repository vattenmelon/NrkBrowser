using System;
using System.Collections.Generic;
using Vattenmelon.Nrk.Domain;
using NUnit.Framework;
using System.Xml;
using System.IO;
using System.Text;
using System.Net;

namespace Vattenmelon.Nrk.Parser.Xml
{
    [TestFixture]
    public class XMLRSSIntegrationTest
    {

        [Test]
        public void GetTopTabRSS()
        {
            XmlRSSParser parser = new XmlRSSParser(NrkParserConstants.RSS_URL, "NATUR");
            List<Item> natur = parser.getClips();
            sjekkTopTabRSSClips(natur);

            parser = new XmlRSSParser(NrkParserConstants.RSS_URL, "SUPER");
            List<Item> super = parser.getClips();
            sjekkTopTabRSSClips(super);
        }

        [Test]
        public void GetTopTabRSSUSA08()
        {
            XmlRSSParser parser = new XmlRSSParser(NrkParserConstants.RSS_URL, "USA08");
            List<Item> usa08 = parser.getClips();
            sjekkTopTabRSSClips(usa08);
        }

        [Test]
        public void GetTopTabRSSSomIkkeFinnes()
        {
            //behaviour has changed, earlier this gave nullreference exception, now it returns NYHETER list.
            XmlRSSParser parser = new XmlRSSParser(NrkParserConstants.RSS_URL, "denne finnes ikke");
            List<Item> ol = parser.getClips();
            sjekkTopTabRSSClips(ol);
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
            //string url = @"http://video.nrkbeta.no/feeds/kategori/tv-serier";
            NrkBetaXmlParser pars = new NrkBetaXmlParser("http://video.nrkbeta.no/feeds/kategori/", "tv-serier");
            List<Item> items = pars.getClips();
            Assert.IsNotEmpty(items);
            foreach(Item item in items)
            {
                Clip c = (Clip)item;
                Console.WriteLine("Tittel.............: " + c.Title);
                Console.WriteLine("ID.................: " + c.ID);
                Console.WriteLine("Beskrivelse........: " + c.Description);
                Console.WriteLine("Bilde..............: " + c.Bilde);
                Console.WriteLine("Type...............: " + c.Type);
                Console.WriteLine("Antall ganger vist.: " + c.AntallGangerVist);
                Console.WriteLine("Klokkeslett........: " + c.Klokkeslett);
                Console.WriteLine("-----------------------------------------");
            }
            
        }
      
    }
}