using System;
using System.Collections.Generic;
using NrkBrowser.Domain;
using NUnit.Framework;

namespace NrkBrowser.Xml
{
    [TestFixture]
    public class XMLRSSIntegrationTest
    {

        [Test]
        public void GetTopTabRSS()
        {
            XmlRSSParser parser = new XmlRSSParser(NrkConstants.RSS_URL, "NATUR");
            List<Item> natur = parser.getClips();
            sjekkTopTabRSSClips(natur);

            parser = new XmlRSSParser(NrkConstants.RSS_URL, "SUPER");
            List<Item> super = parser.getClips();
            sjekkTopTabRSSClips(super);
        }

        [Test]
        public void GetTopTabRSSUSA08()
        {
            XmlRSSParser parser = new XmlRSSParser(NrkConstants.RSS_URL, "USA08");
            List<Item> usa08 = parser.getClips();
            sjekkTopTabRSSClips(usa08);
        }

        [Test]
        public void GetTopTabRSSSomIkkeFinnes()
        {
            //behaviour has changed, earlier this gave nullreference exception, now it returns NYHETER list.
            XmlRSSParser parser = new XmlRSSParser(NrkConstants.RSS_URL, "denne finnes ikke");
            List<Item> ol = parser.getClips();
            sjekkTopTabRSSClips(ol);
        }

        [Test]
        public void GetPictureUrlIfSectionIsKnown()
        {
            XmlRSSParser parser = new XmlRSSParser(NrkConstants.RSS_URL, NrkConstants.MENU_ITEM_ID_NYHETER);
            String bildeUrl = parser.GetBildeUrl();
            Assert.AreEqual(NrkPlugin.PICTURE_DIR + NrkConstants.MENU_ITEM_PICTURE_NYHETER, bildeUrl);
            
            parser = new XmlRSSParser(NrkConstants.RSS_URL, NrkConstants.MENU_ITEM_ID_NATUR);
            bildeUrl = parser.GetBildeUrl();
            Assert.AreEqual(NrkPlugin.PICTURE_DIR + NrkConstants.MENU_ITEM_PICTURE_NATURE, bildeUrl);

            parser = new XmlRSSParser(NrkConstants.RSS_URL, NrkConstants.MENU_ITEM_ID_SPORT);
            bildeUrl = parser.GetBildeUrl();
            Assert.AreEqual(NrkPlugin.PICTURE_DIR + NrkConstants.MENU_ITEM_PICTURE_SPORT, bildeUrl);

            parser = new XmlRSSParser(NrkConstants.RSS_URL, NrkConstants.MENU_ITEM_ID_SUPER);
            bildeUrl = parser.GetBildeUrl();
            Assert.AreEqual(NrkPlugin.PICTURE_DIR + NrkConstants.MENU_ITEM_PICTURE_SUPER, bildeUrl);
        }

        [Test]
        public void GetPictureUrlShouldDefaultToDefaultPictureIfSectionIsUnknown()
        {
            XmlRSSParser parser = new XmlRSSParser(NrkConstants.RSS_URL, "Abba");
            String bildeUrl = parser.GetBildeUrl();
            Assert.AreEqual(NrkConstants.DEFAULT_PICTURE, bildeUrl);

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

    }
}