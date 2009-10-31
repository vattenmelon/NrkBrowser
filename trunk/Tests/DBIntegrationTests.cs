/*
 * Created by: 
 * Created: 17. november 2008
 */

using System.Collections.Generic;
using NrkBrowser.Domain;
using NUnit.Framework;

namespace NrkBrowser
{
    [TestFixture]
    public class DBIntegrationTests
    {
        private NrkParser nrkParser;

        [TestFixtureSetUp]
        public void setOpp()
        {
            nrkParser = new NrkParser(10000, new MPLogger());
        }

        [Test]
        public void TestDBLeggTilFjernKlipp()
        {
            FavoritesUtil db = FavoritesUtil.getDatabase("NrkBrowserTest.db3");
            Assert.IsNotNull(db);
            List<Item> fav = db.getFavoriteVideos();
            Assert.AreEqual(0, fav.Count, "Skal ikke ha noen favoritter nå.");
            List<Item> liste = nrkParser.GetTopTabRSS("natur");
            Clip c = (Clip)liste[0];
            string message = "";
            Assert.IsTrue(db.addFavoriteVideo(c, ref message));
            fav = db.getFavoriteVideos();
            Assert.AreEqual(1, fav.Count, "Skal ha en favoritt nå.");
            Clip clipFraDB = (Clip)fav[0];
            Assert.AreEqual(c.Title, clipFraDB.Title);
            Assert.AreEqual(c.Description, clipFraDB.Description);
            Assert.AreEqual(c.ID, clipFraDB.ID);
            Assert.AreEqual(c.Bilde, clipFraDB.Bilde);
            Assert.AreEqual(c.AntallGangerVist, clipFraDB.AntallGangerVist);
            Assert.AreEqual(c.Klokkeslett, clipFraDB.Klokkeslett);
            Assert.AreEqual(c.VerdiLink, clipFraDB.VerdiLink);
            Assert.IsFalse(db.addFavoriteVideo(c, ref message));
            fav = db.getFavoriteVideos();
            Assert.AreEqual(1, fav.Count, "Skal fortsatt ha en favoritt nå.");
            Assert.IsTrue(db.removeFavoriteVideo(clipFraDB));
            fav = db.getFavoriteVideos();
            Assert.AreEqual(0, fav.Count, "Skal ikke ha noen favoritter nå.");
        }

        [Test]
        public void TestDBLeggTilFjernProgram()
        {
            FavoritesUtil db = FavoritesUtil.getDatabase("NrkBrowserTest.db3");
            Assert.IsNotNull(db);
            List<Item> fav = db.getFavoriteVideos();
            Assert.AreEqual(0, fav.Count, "Skal ikke ha noen favoritter nå.");
            List<Item> liste = nrkParser.GetAllPrograms();
            Program p = null;
            foreach (Item item in liste)
            {
                if (item.Title == "Autofil")
                {
                    p = (Program)item;
                    break;
                }
            }

            string message = "";
            Assert.IsTrue(db.addFavoriteProgram(p, ref message));
            fav = db.getFavoriteVideos();
            Assert.AreEqual(1, fav.Count, "Skal ha en favoritt nå.");
            Program programFraDB = (Program)fav[0];
            Assert.AreEqual(p.Title, programFraDB.Title);
            Assert.AreEqual(p.Description.Trim(), programFraDB.Description);
            Assert.AreEqual(p.ID, programFraDB.ID);
            Assert.AreEqual(p.Bilde, programFraDB.Bilde);
            Assert.IsFalse(db.addFavoriteProgram(p, ref message));
            fav = db.getFavoriteVideos();
            Assert.AreEqual(1, fav.Count, "Skal fortsatt ha en favoritt nå.");
            Assert.IsTrue(db.removeFavoriteProgram(programFraDB));
            fav = db.getFavoriteVideos();
            Assert.AreEqual(0, fav.Count, "Skal ikke ha noen favoritter nå.");
        }
    }
}