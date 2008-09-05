using System;
using System.Collections.Generic;
using NrkBrowser;
using NUnit.Framework;

namespace Tests
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
            List<Item> liste = nrkParser.GetForsiden();
            Assert.AreEqual(22, liste.Count); //var 7 i denne listen den 6/9-08
            foreach (Item item in liste)
            {
                Clip c = (Clip) item;
                Assert.IsNotEmpty(c.ID, "ID'en kan ikke være null");
                Assert.IsNotEmpty(c.Description, "Beskrivelsen kan ikke være null");
                Assert.IsNotEmpty(c.Bilde, "Bilde kan ikke være null");
                Assert.IsNotEmpty(c.Title, "Tittelen kan ikke være null");
                Assert.IsTrue(c.Playable, "Klipp må være playable");
                Assert.AreEqual(Clip.KlippType.KLIPP, c.Type, "Skal være av typen KLIPP");
                Assert.IsNull(c.VerdiLink, "Klipp fra forsiden er ikke verdilinker");
            }
        }

        [Test]
        public void TestGetMestSettDenneUken()
        {
            List<Item> liste = nrkParser.GetMestSettDenneUken();
            Assert.AreEqual(7, liste.Count); //var 7 i denne listen den 6/9-08
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
                Assert.IsNull(c.VerdiLink, "Klipp fra mest sette er ikke verdilinker");
            }
        }

        [Test]
        public void TestGetAllPrograms()
        {
            List<Item> liste = nrkParser.GetAllPrograms();
            Assert.AreEqual(309, liste.Count); //var 7 i denne listen den 6/9-08
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
        public void TestGetTopTabber()
        {
            List<Item> liste = nrkParser.GetTopTabber("nyheter");
            Assert.AreEqual(40, liste.Count); // 6/9-08
            topTabTest(liste);

            liste = nrkParser.GetTopTabber("ol");
            Assert.AreEqual(34, liste.Count); //6/9-08
            topTabTest(liste);

            liste = nrkParser.GetTopTabber("sport");
            Assert.AreEqual(27, liste.Count); //6/9-08
            topTabTest(liste);

            liste = nrkParser.GetTopTabber("natur");
            Assert.AreEqual(58, liste.Count); //6/9-08
            topTabTest(liste);
        }

        [Test]
        public void TestGetClipUrlFraNyheterITopTab()
        {
            List<Item> liste = nrkParser.GetTopTabber("nyheter");
            foreach (Item item in liste)
            {
                Clip c = (Clip) item;
                string clipUrl = nrkParser.GetClipUrl(c);
                Assert.IsNotEmpty(clipUrl, "Klipp-url kan ikke være empty");
                Assert.IsNotNull(clipUrl, "Klipp-url kan ikke være null");
                Assert.IsTrue(clipUrl.ToLower().StartsWith("mms://"));
                Assert.IsTrue(clipUrl.ToLower().EndsWith(".wmv"));
            }
        }

        [Test]
        public void TestGetClipUrlFraNaturIRSSFeed()
        {
            List<Item> liste = nrkParser.GetTopTabRSS("natur");
            foreach (Item item in liste)
            {
                Clip c = (Clip)item;
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
                Assert.IsNull(c.AntallGangerVist, "Antall ganger vist skal være satt");
                Assert.IsNotNull(c.VerdiLink, "VerdiLink må være satt på et klipp av type VERDI");
                Assert.IsNotEmpty(c.VerdiLink, "VerdiLink må være satt på et klipp av type VERDI");
            }
        }
    }
}