/*
 * Created by: 
 * Created: 17. november 2008
 */

using System;
using System.Collections.Generic;
using Vattenmelon.Nrk.Browser;
using Vattenmelon.Nrk.Browser.Translation;
using Vattenmelon.Nrk.Domain;
using NUnit.Framework;

namespace Vattenmelon.Nrk.Browser
{
    [TestFixture]
    [Category("Unit Tests")]
    public class NrkPluginTest
    {
        private NrkPlugin nrkPlugin;
        private static int pluginId = 40918376;

        [TestFixtureSetUp]
        public void setOpp()
        {
            nrkPlugin = new NrkPlugin("no", @"..\..\..\languages");
        }

        [Test]
        public void testGetID()
        {
            Assert.AreEqual(pluginId, nrkPlugin.GetID);
        }

        [Test]
        public void testInit()
        {
            Assert.IsTrue(nrkPlugin.Init());
        }
        [Test]
        public void testCreateMestSetteListItems()
        {
           List<Item> items = NrkPlugin.CreateMestSetteListItems();
           Assert.AreEqual(3, items.Count);
            bool funnet1 = false;
            bool funnet2 = false;
            bool funnet3 = false;
            foreach (Item item in items)
            {
                if (item.ID.Equals(NrkBrowserConstants.MENU_ITEM_ID_MEST_SETTE_UKE))
                {
                    Assert.AreEqual(NrkTranslatableStrings.MENU_ITEM_TITLE_MEST_SETTE_UKE, item.Title);
                    Assert.AreEqual("De mest populære klippene denne uken!", item.Description);
                    funnet1 = true;
                }
                else if (item.ID.Equals(NrkBrowserConstants.MENU_ITEM_ID_MEST_SETTE_MAANED))
                {
                    Assert.AreEqual(NrkTranslatableStrings.MENU_ITEM_TITLE_MEST_SETTE_MAANED, item.Title);
                    Assert.AreEqual("De mest populære klippene denne måneden!", item.Description);
                    funnet2 = true;
                }
                else if (item.ID.Equals(NrkBrowserConstants.MENU_ITEM_ID_MEST_SETTE_TOTALT))
                {
                    Assert.AreEqual(NrkTranslatableStrings.MENU_ITEM_TITLE_MEST_SETTE_TOTALT, item.Title);
                    Assert.AreEqual("De mest populære klippene!", item.Description);
                    funnet3 = true;
                }
            }
            Assert.IsTrue(funnet1);
            Assert.IsTrue(funnet2);
            Assert.IsTrue(funnet3);
        }
        [Test]
        public void getVersion()
        {
            String version = NrkPlugin.getVersion();
            Console.WriteLine(version);
            Assert.IsNotEmpty(version);
        }

        [Test]
        public void GetPictureUrlIfSectionIsKnown()
        {

            String bildeUrl = nrkPlugin.GetPictureFile(NrkBrowserConstants.MENU_ITEM_ID_NYHETER);
            Assert.AreEqual(NrkBrowserConstants.MENU_ITEM_PICTURE_NYHETER, bildeUrl);


            bildeUrl = nrkPlugin.GetPictureFile(NrkBrowserConstants.MENU_ITEM_ID_NATUR);
            Assert.AreEqual(NrkBrowserConstants.MENU_ITEM_PICTURE_NATURE, bildeUrl);


            bildeUrl = nrkPlugin.GetPictureFile(NrkBrowserConstants.MENU_ITEM_ID_SPORT);
            Assert.AreEqual(NrkBrowserConstants.MENU_ITEM_PICTURE_SPORT, bildeUrl);


            bildeUrl = nrkPlugin.GetPictureFile(NrkBrowserConstants.MENU_ITEM_ID_SUPER);
            Assert.AreEqual(NrkBrowserConstants.MENU_ITEM_PICTURE_SUPER, bildeUrl);
        }

        [Test]
        public void GetPictureUrlShouldDefaultToDefaultPictureIfSectionIsUnknown()
        {

            String bildeUrl = nrkPlugin.GetPictureFile("abba");
            Assert.AreEqual(NrkBrowserConstants.DEFAULT_PICTURE, bildeUrl);

        }
        [Test]
        public void GetPodKastMenuItems()
        {
            List<Item> podkastItems = nrkPlugin.CreatePodcastMenuItems();
            Assert.AreEqual(2, podkastItems.Count);
            bool funnetAudio = false;
            bool funnetVideo = false;
            foreach (Item item in podkastItems)
            {
                Assert.AreEqual(NrkBrowserConstants.NRK_LOGO_PICTURE, item.Bilde);
                if (item.ID.Equals(NrkBrowserConstants.MENU_ITEM_ID_PODCASTS_AUDIO))
                {
                    funnetAudio = true;
                }
                else if (item.ID.Equals(NrkBrowserConstants.MENU_ITEM_ID_PODCASTS_VIDEO))
                {
                    funnetVideo = true;
                }
            }
            Assert.IsTrue(funnetAudio);
            Assert.IsTrue(funnetVideo);
        }
/*
        [Test]
        public void testGetTopTabs()
        {
           nrkPlugin.Init();
           List<Item> liste = nrkPlugin.GetTabItems();
           Assert.AreEqual(1, liste.Count, "Skal være et i lista");
        } */

//        [Test]
//        public void checkForNewVersionTest()
//        {
//            String nyVer = String.Empty;
//            Assert.IsFalse(NrkPlugin.newVersionAvailable(ref nyVer));
//        }
    }
}