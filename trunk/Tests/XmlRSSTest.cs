using System;
using System.Collections.Generic;
using Vattenmelon.Nrk.Domain;
using NUnit.Framework;

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
            Assert.IsNotNull(liste, "Listen kan ikke v�re null");
            Assert.IsTrue(liste.Count > 0, "Listen skal v�re st�rre enn 0");
            foreach (Item item in liste)
            {
                Clip c = (Clip)item;
                Assert.IsNotNull(c.ID, "ID'en kan ikke v�re null");
                Assert.IsNotNull(c.Title, "Tittelen kan ikke v�re null");
                Assert.IsTrue(c.Playable, "Klipp m� v�re playable");
            }
        }

    }
}