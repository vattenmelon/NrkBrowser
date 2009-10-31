/*
 * Created by: 
 * Created: 17. november 2008
 */

using System;
using System.Collections.Generic;
using NrkBrowser.Domain;
using NUnit.Framework;

namespace NrkBrowser
{
    [TestFixture]
    public class NrkPluginTest
    {
        private NrkPlugin nrkPlugin;
        private static int pluginId = 40918376;

        [TestFixtureSetUp]
        public void setOpp()
        {

            nrkPlugin = new NrkPlugin("no", @"C:\Users\Erling Reizer\Documents\Visual Studio 2005\Projects\NRKBrowser\languages");
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
                if (item.ID.Equals(NrkConstants.MENU_ITEM_ID_MEST_SETTE_UKE))
                {
                    Assert.AreEqual(NrkTranslatableStrings.MENU_ITEM_TITLE_MEST_SETTE_UKE, item.Title);
                    Assert.AreEqual("De mest populære klippene denne uken!", item.Description);
                    funnet1 = true;
                }
                else if (item.ID.Equals(NrkConstants.MENU_ITEM_ID_MEST_SETTE_MAANED))
                {
                    Assert.AreEqual(NrkTranslatableStrings.MENU_ITEM_TITLE_MEST_SETTE_MAANED, item.Title);
                    Assert.AreEqual("De mest populære klippene denne måneden!", item.Description);
                    funnet2 = true;
                }
                else if (item.ID.Equals(NrkConstants.MENU_ITEM_ID_MEST_SETTE_TOTALT))
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