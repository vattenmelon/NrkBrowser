using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NrkBrowser
{
    [TestFixture]
    public class NrkParserTest
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
                        Assert.AreEqual("P� samisk", kat.Title);
                        break;
                    case "20":
                        Assert.AreEqual("Dokumentar", kat.Title);
                        break;
                    case "21":
                        Assert.AreEqual("Ung", kat.Title);
                        break;
                    case "22":
                        Assert.AreEqual("P� tegnspr�k", kat.Title);
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
        public void TestGetTopTabRSSSomIkkeFinnes()
        {
            //behaviour has changed, earlier this gave nullreference exception, now it returns NYHETER list.
            List<Item> ol = nrkParser.GetTopTabRSS("denne finnes ikke");
            sjekkTopTabRSSClips(ol);
        }


        private void sjekkTopTabRSSClips(List<Item> liste)
        {
            Assert.IsNotNull(liste, "Listen kan ikke v�re null");
            Assert.IsTrue(liste.Count > 0, "Listen skal v�re st�rre enn 0");
            foreach (Item item in liste)
            {
                Clip c = (Clip) item;
                Assert.IsNotNull(c.ID, "ID'en kan ikke v�re null");
                Assert.IsNotNull(c.Title, "Tittelen kan ikke v�re null");
                Assert.IsTrue(c.Playable, "Klipp m� v�re playable");
            }
        }

        [Test]
        public void TestGetForsiden()
        {
            List<Item> liste = nrkParser.GetAnbefaltePaaForsiden();
            Assert.IsNotNull(liste);
            Assert.Greater(liste.Count, 0, "Listen skal v�re st�rre enn 0");
            Console.WriteLine(liste.Count);
            foreach (Item item in liste)
            {
                Clip c = (Clip) item;
                Console.WriteLine("type: " + c.Type);
                Console.WriteLine("title: " + c.Title);
                Assert.IsNotEmpty(c.ID, "ID'en kan ikke v�re null");
                Assert.IsNotEmpty(c.Description, "Beskrivelsen kan ikke v�re null");
                Assert.IsNotEmpty(c.Bilde, "Bilde kan ikke v�re null");
                Assert.IsNotEmpty(c.Title, "Tittelen kan ikke v�re null");
                Assert.IsTrue(c.Playable, "Klipp m� v�re playable");
                Assert.AreEqual(Clip.KlippType.KLIPP, c.Type, "Skal v�re av typen KLIPP");
                Assert.IsEmpty(c.VerdiLink, "Klipp fra forsiden er ikke verdilinker");
            }
        }

        [Test]
        public void TestGetMestSette()
        {
            List<Item> liste = nrkParser.GetMestSette(31);
            Assert.IsNotNull(liste);
            Assert.Greater(liste.Count, 0, "Listen skal v�re st�rre enn 0");
            Assert.AreEqual(12,liste.Count, "Listen skal ha 12 oppf�rsler"); 
            foreach (Item item in liste)
            {
               
                Clip c = (Clip) item;
                Assert.IsNotEmpty(c.ID, "ID'en kan ikke v�re null");
                Assert.IsNotEmpty(c.Description, "Beskrivelsen kan ikke v�re null");
                Assert.IsNotEmpty(c.Bilde, "Bilde kan ikke v�re null");
                Assert.IsTrue(ErBildeFil(c.Bilde));
                Assert.IsNotEmpty(c.Title, "Tittelen kan ikke v�re null");
                Assert.IsTrue(c.Playable, "Klipp m� v�re playable");
                Assert.IsTrue(erEntenKlippEllerIndexType(c), "Klipp skal v�re enten av type Klipp eller Index");
                Assert.IsNotEmpty(c.AntallGangerVist, "Antall ganger vist skal v�re satt");
                Assert.IsEmpty(c.VerdiLink, "Klipp fra mest sette er ikke verdilinker");
            }
        }

        private static bool erEntenKlippEllerIndexType(Clip c)
        {
            return c.Type == Clip.KlippType.KLIPP || c.Type == Clip.KlippType.INDEX;
        }

        private static bool ErBildeFil(String filename)
        {
            return filename.EndsWith(".jpg") || filename.EndsWith(".JPG") || filename.EndsWith(".png") || filename.EndsWith(".PNG");
        }

        [Test]
        public void TestGetAllPrograms()
        {
            List<Item> liste = nrkParser.GetAllPrograms();
            foreach (Item item in liste)
            {
                Program program = (Program) item;
                Assert.IsNotEmpty(program.ID, "ID'en kan ikke v�re null");
                Assert.IsNotEmpty(program.Description, "Beskrivelsen kan ikke v�re null");
                Assert.IsNotEmpty(program.Bilde, "Bilde kan ikke v�re null");
                Assert.IsNotEmpty(program.Title, "Tittelen kan ikke v�re null");
                Assert.IsFalse(program.Playable, "Klipp m� v�re playable");
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

//        [Test]
//        public void TestTopTabbDirekte()
//        {
//            List<Item> liste = nrkParser.GetTopTabber("direkte");
//            Assert.IsNotEmpty(liste);
//            Console.Out.WriteLine("count: " + liste.Count);
////            foreach (Item item in liste)
////            {
////                Console.Out.WriteLine(item);
////                Console.Out.WriteLine("id: "+item.ID);
////                Console.Out.WriteLine("title: " +item.Title);
////                Console.Out.WriteLine("description: "+item.Description);
////                Console.Out.WriteLine("bilde: " + item.Bilde);
////            }
//        }

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
                Assert.IsNotEmpty(clipUrl, "Klipp-url kan ikke v�re empty");
                Assert.IsNotNull(clipUrl, "Klipp-url kan ikke v�re null");
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
                Assert.IsNotEmpty(clipUrl, "Klipp-url kan ikke v�re empty");
                Assert.IsNotNull(clipUrl, "Klipp-url kan ikke v�re null");
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
                Assert.IsNotEmpty(clipUrl, "Klipp-url kan ikke v�re empty");
                Assert.IsNotNull(clipUrl, "Klipp-url kan ikke v�re null");
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
                        Assert.IsNotNull(p.ID, "ID skal ikke v�re null p� et program");
                        Assert.IsNotNull(p.Title, "Title skal ikke v�re null p� et program");
                        Assert.IsNotNull(p.Bilde, "Bilde skal ikke v�re null p� et program");
                        Assert.IsNotNull(p.Description, "Beskrivelse skal ikke v�re null p� et program");
                        Assert.IsFalse(p.Playable, "Et program skal ikke v�re playable");
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

            Assert.AreNotEqual(liste[0].ID, listeSide2[0].ID, "Skal ikke v�re like");
        }

        [Test]
        public void TestSearchForNyttLivForCommodore64()
        {
            List<Item> liste = nrkParser.GetSearchHits("Nytt liv for Commodore 64", 0);
            Assert.AreEqual(1, liste.Count);
            Clip c = (Clip) liste[0];
            String klippUrl = nrkParser.GetClipUrl(c);
            Console.WriteLine(klippUrl);
            string expectedUrl = "mms://straumod.nrk.no/disk03/Lydverket/2008-04-16/Lydverket_16_04_08_1000_358373_20080416_231000.wmv";
            Assert.AreEqual(
                expectedUrl,
                klippUrl, "KlippURL er blitt endret...denne testen er mest nyttig for � finne endringer hos nrk.");
            Assert.AreEqual(1658, c.StartTime, "Starttiden for klippet har endret seg.");
        }


        private void topTabTest(List<Item> liste)
        {
            Assert.IsNotNull(liste);
            Assert.IsTrue(liste.Count > 0, "Listen skal v�re st�rre enn 1");
            foreach (Item item in liste)
            {
                Clip c = (Clip) item;
                Assert.IsNotEmpty(c.ID, "ID'en kan ikke v�re null");
                Assert.IsNotEmpty(c.Description, "Beskrivelsen kan ikke v�re null");
                Assert.IsNotEmpty(c.Bilde, "Bilde kan ikke v�re null");
                Assert.IsNotEmpty(c.Title, "Tittelen kan ikke v�re null");
                Assert.IsTrue(c.Playable, "Klipp m� v�re playable");
                Assert.AreEqual(Clip.KlippType.VERDI, c.Type, "Skal v�re av typen VERDI");
                Assert.IsEmpty(c.AntallGangerVist, "Antall ganger vist skal ikke v�re satt");
                Assert.IsNotNull(c.VerdiLink, "VerdiLink m� v�re satt p� et klipp av type VERDI");
                Assert.IsNotEmpty(c.VerdiLink, "VerdiLink m� v�re satt p� et klipp av type VERDI");
            }
        }
    }
}