using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NrkBrowser
{
    [TestFixture]
    public class NrkBrowserTest
    {
        private NrkParser nrkParser;

        [TestFixtureSetUp]
        public void setOpp()
        {
            nrkParser = new NrkParser(10000);
        }

        [Test]
        public void TestHentKategorier()
        {
            List<Item> categories = nrkParser.GetCategories();
            Assert.AreEqual(16, categories.Count);
            foreach (Item item in categories)
            {
                Category kat = (Category) item;
                switch (kat.ID)
                {
                    case "2":
                        Assert.AreEqual("Barn", kat.Title);
                        break;
                    case "3":
                        Assert.AreEqual("Drama", kat.Title);
                        break;
                    case "4":
                        Assert.AreEqual("Fakta", kat.Title);
                        break;
                    case "5":
                        Assert.AreEqual("Kultur", kat.Title);
                        break;
                    case "6":
                        Assert.AreEqual("Musikk", kat.Title);
                        break;
                    case "7":
                        Assert.AreEqual("Natur", kat.Title);
                        break;
                    case "8":
                        Assert.AreEqual("Nyheter", kat.Title);
                        break;
                    case "9":
                        Assert.AreEqual("Livssyn", kat.Title);
                        break;
                    case "10":
                        Assert.AreEqual("Sport", kat.Title);
                        break;
                    case "11":
                        Assert.AreEqual("Underholdning", kat.Title);
                        break;
                    case "13":
                        Assert.AreEqual("Distrikt", kat.Title);
                        break;
                    case "17":
                        Assert.AreEqual("Mat", kat.Title);
                        break;
                    case "19":
                        Assert.AreEqual("På samisk", kat.Title);
                        break;
                    case "20":
                        Assert.AreEqual("Dokumentar", kat.Title);
                        break;
                    case "21":
                        Assert.AreEqual("Ung", kat.Title);
                        break;
                    case "22":
                        Assert.AreEqual("På tegnspråk", kat.Title);
                        break;
                    default:
                        Assert.Fail("Kjenner ikke til kategorien");
                        break;
                }
            }
        }

        [Test]
        public void TestGetTopTabRSS()
        {
            List<Item> natur = nrkParser.GetTopTabRSS("NATUR");
            sjekkTopTabRSSClips(natur);

            List<Item> super = nrkParser.GetTopTabRSS("SUPER");
            sjekkTopTabRSSClips(super);
        }

        [Test]
        public void TestGetTopTabRSSUSA08()
        {
            List<Item> usa08 = nrkParser.GetTopTabRSS("USA08");
            sjekkTopTabRSSClips(usa08);
        }

        [Test]
        [ExpectedException(typeof (NullReferenceException))]
        public void TestGetTopTabRSSSomIkkeFinnes()
        {
            List<Item> ol = nrkParser.GetTopTabRSS("dennefinnesikke");
            sjekkTopTabRSSClips(ol);
        }


        private void sjekkTopTabRSSClips(List<Item> liste)
        {
            Assert.IsNotNull(liste, "Listen kan ikke være null");
            Assert.IsTrue(liste.Count > 0, "Listen skal være større enn 0");
            foreach (Item item in liste)
            {
                Clip c = (Clip) item;
                Assert.IsNotNull(c.ID, "ID'en kan ikke være null");
                Assert.IsNotNull(c.Title, "Tittelen kan ikke være null");
                Assert.IsTrue(c.Playable, "Klipp må være playable");
            }
        }

        [Test]
        public void TestGetForsiden()
        {
            List<Item> liste = nrkParser.GetAnbefaltePaaForsiden();
            Assert.IsNotNull(liste);
            Assert.Greater(liste.Count, 0, "Listen skal være større enn 0");
            Console.WriteLine(liste.Count);
            foreach (Item item in liste)
            {
                Clip c = (Clip) item;
                Console.WriteLine("type: " + c.Type);
                Console.WriteLine("title: " + c.Title);
                Assert.IsNotEmpty(c.ID, "ID'en kan ikke være null");
                Assert.IsNotEmpty(c.Description, "Beskrivelsen kan ikke være null");
                Assert.IsNotEmpty(c.Bilde, "Bilde kan ikke være null");
                Assert.IsNotEmpty(c.Title, "Tittelen kan ikke være null");
                Assert.IsTrue(c.Playable, "Klipp må være playable");
                Assert.AreEqual(Clip.KlippType.KLIPP, c.Type, "Skal være av typen KLIPP");
                Assert.IsEmpty(c.VerdiLink, "Klipp fra forsiden er ikke verdilinker");
            }
        }

        [Test]
        public void TestGetMestSette()
        {
            List<Item> liste = nrkParser.GetMestSette(31);
            Assert.IsNotNull(liste);
            Assert.Greater(liste.Count, 0, "Listen skal være større enn 0");
            foreach (Item item in liste)
            {
                Clip c = (Clip) item;
                Assert.IsNotEmpty(c.ID, "ID'en kan ikke være null");
                Assert.IsNotEmpty(c.Description, "Beskrivelsen kan ikke være null");
                Assert.IsNotEmpty(c.Bilde, "Bilde kan ikke være null");
                Assert.IsNotEmpty(c.Title, "Tittelen kan ikke være null");
                Assert.IsTrue(c.Playable, "Klipp må være playable");
                Assert.AreEqual(Clip.KlippType.KLIPP, c.Type, "Skal være av typen KLIPP");
                Assert.IsNotEmpty(c.AntallGangerVist, "Antall ganger vist skal være satt");
                Assert.IsEmpty(c.VerdiLink, "Klipp fra mest sette er ikke verdilinker");
            }
        }

        [Test]
        public void TestGetAllPrograms()
        {
            List<Item> liste = nrkParser.GetAllPrograms();
            foreach (Item item in liste)
            {
                Program program = (Program) item;
                Assert.IsNotEmpty(program.ID, "ID'en kan ikke være null");
                Assert.IsNotEmpty(program.Description, "Beskrivelsen kan ikke være null");
                Assert.IsNotEmpty(program.Bilde, "Bilde kan ikke være null");
                Assert.IsNotEmpty(program.Title, "Tittelen kan ikke være null");
                Assert.IsFalse(program.Playable, "Klipp må være playable");
            }
        }

        [Test]
        public void TestGetTopTabberMedParameter()
        {
            List<Item> liste = nrkParser.GetTopTabber("nyheter");
            topTabTest(liste);

            liste = nrkParser.GetTopTabber("sport");
            topTabTest(liste);

            liste = nrkParser.GetTopTabber("natur");
            topTabTest(liste);
        }

        [Test]
        public void TestTopTabbDirekte()
        {
            List<Item> liste = nrkParser.GetTopTabber("direkte");
            Assert.IsNotEmpty(liste);
            Console.Out.WriteLine("count: " + liste.Count);
//            foreach (Item item in liste)
//            {
//                Console.Out.WriteLine(item);
//                Console.Out.WriteLine("id: "+item.ID);
//                Console.Out.WriteLine("title: " +item.Title);
//                Console.Out.WriteLine("description: "+item.Description);
//                Console.Out.WriteLine("bilde: " + item.Bilde);
//            }
        }

        [Test]
        public void TestGetTopTabber()
        {
            List<Item> liste = nrkParser.GetTopTabber();
            Assert.IsNotNull(liste);
            Assert.IsNotEmpty(liste);
            Console.Out.WriteLine("treff:");
//            foreach (Item item in liste)
//            {
//                Console.Out.WriteLine(item.ID + ", " + item.Title);
//                Console.Out.WriteLine("-------------------------------------------");
//            }
        }

        [Test]
        public void TestGetClipUrlFraNyheterITopTab()
        {
            List<Item> liste = nrkParser.GetTopTabRSS("nyheter");
            foreach (Item item in liste)
            {
                Clip c = (Clip) item;
                string clipUrl = nrkParser.GetClipUrl(c);
                Console.WriteLine(c.Title + ", " + c.ID);
                Assert.IsNotEmpty(clipUrl, "Klipp-url kan ikke være empty");
                Assert.IsNotNull(clipUrl, "Klipp-url kan ikke være null");
                Assert.IsFalse(clipUrl.ToLower().StartsWith("mms://"));
                Assert.IsFalse(clipUrl.ToLower().EndsWith(".wmv"));
                Assert.IsTrue(clipUrl.ToLower().StartsWith("http://"));
            }
        }

        [Test]
        public void TestGetClipUrlFraSportITopTab()
        {
            List<Item> liste = nrkParser.GetTopTabRSS("sport");
            foreach (Item item in liste)
            {
                Clip c = (Clip) item;
                string clipUrl = nrkParser.GetClipUrl(c);
                Console.WriteLine(c.Title + ", " + c.ID);
                Assert.IsNotEmpty(clipUrl, "Klipp-url kan ikke være empty");
                Assert.IsNotNull(clipUrl, "Klipp-url kan ikke være null");
                Assert.IsFalse(clipUrl.ToLower().StartsWith("mms://"));
                Assert.IsFalse(clipUrl.ToLower().EndsWith(".wmv"));
                Assert.IsTrue(clipUrl.ToLower().StartsWith("http://"));
            }
        }

        [Test]
        public void TestGetClipUrlFraNaturIRSSFeed()
        {
            List<Item> liste = nrkParser.GetTopTabRSS("natur");
            foreach (Item item in liste)
            {
                Clip c = (Clip) item;
                string clipUrl = nrkParser.GetClipUrl(c);
                Assert.IsNotEmpty(clipUrl, "Klipp-url kan ikke være empty");
                Assert.IsNotNull(clipUrl, "Klipp-url kan ikke være null");
                Assert.IsTrue(clipUrl.ToLower().StartsWith(NrkParser.RSS_CLIPURL_PREFIX));
            }
        }

        [Test]
        public void TestHentProgrammer()
        {
            List<Item> categories = nrkParser.GetCategories();
            foreach (Item item in categories)
            {
                Category k = (Category) item;
                if (k.Title.Equals("Barn"))
                {
                    List<Item> programmer = nrkParser.GetPrograms(k);
                    foreach (Item item1 in programmer)
                    {
                        Program p = (Program) item1;
                        Assert.IsNotNull(p.ID, "ID skal ikke være null på et program");
                        Assert.IsNotNull(p.Title, "Title skal ikke være null på et program");
                        Assert.IsNotNull(p.Bilde, "Bilde skal ikke være null på et program");
                        Assert.IsNotNull(p.Description, "Beskrivelse skal ikke være null på et program");
                        Assert.IsFalse(p.Playable, "Et program skal ikke være playable");
                    }
                }
            }
        }

        [Test]
        public void TestGetSearchHits()
        {
            List<Item> liste = nrkParser.GetSearchHits("Norge", 0);
            Assert.AreEqual(25, liste.Count);

            List<Item> listeSide2 = nrkParser.GetSearchHits("Norge", 1);
            Assert.AreEqual(25, liste.Count);

            Assert.AreNotEqual(liste[0].ID, listeSide2[0].ID, "Skal ikke være like");
        }

        [Test]
        public void TestDBLeggTilFjernKlipp()
        {
            FavoritesUtil db = FavoritesUtil.getDatabase("NrkBrowserTest.db3");
            Assert.IsNotNull(db);
            List<Item> fav = db.getFavoriteVideos();
            Assert.AreEqual(0, fav.Count, "Skal ikke ha noen favoritter nå.");
            List<Item> liste = nrkParser.GetTopTabRSS("natur");
            Clip c = (Clip) liste[0];
            string message = "";
            Assert.IsTrue(db.addFavoriteVideo(c, ref message));
            fav = db.getFavoriteVideos();
            Assert.AreEqual(1, fav.Count, "Skal ha en favoritt nå.");
            Clip clipFraDB = (Clip) fav[0];
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
                    p = (Program) item;
                    break;
                }
            }

            string message = "";
            Assert.IsTrue(db.addFavoriteProgram(p, ref message));
            fav = db.getFavoriteVideos();
            Assert.AreEqual(1, fav.Count, "Skal ha en favoritt nå.");
            Program programFraDB = (Program) fav[0];
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

        private void topTabTest(List<Item> liste)
        {
            Assert.IsNotNull(liste);
            Assert.IsTrue(liste.Count > 0, "Listen skal være større enn 1");
            foreach (Item item in liste)
            {
                Clip c = (Clip) item;
                Assert.IsNotEmpty(c.ID, "ID'en kan ikke være null");
                Assert.IsNotEmpty(c.Description, "Beskrivelsen kan ikke være null");
                Assert.IsNotEmpty(c.Bilde, "Bilde kan ikke være null");
                Assert.IsNotEmpty(c.Title, "Tittelen kan ikke være null");
                Assert.IsTrue(c.Playable, "Klipp må være playable");
                Assert.AreEqual(Clip.KlippType.VERDI, c.Type, "Skal være av typen VERDI");
                Assert.IsEmpty(c.AntallGangerVist, "Antall ganger vist skal ikke være satt");
                Assert.IsNotNull(c.VerdiLink, "VerdiLink må være satt på et klipp av type VERDI");
                Assert.IsNotEmpty(c.VerdiLink, "VerdiLink må være satt på et klipp av type VERDI");
            }
        }
    }
}