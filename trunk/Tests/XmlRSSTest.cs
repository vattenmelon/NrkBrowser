using System;
using System.Collections.Generic;
using Vattenmelon.Nrk.Domain;
using NUnit.Framework;

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