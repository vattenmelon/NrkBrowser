using System;
using System.Collections.Generic;
using Vattenmelon.Nrk.Browser;
using Vattenmelon.Nrk.Browser.Translation;
using Vattenmelon.Nrk.Parser;
using Vattenmelon.Nrk.Domain;
using NUnit.Framework;

namespace Vattenmelon.Nrk.Parser
{
    [TestFixture]
    public class NrkParserIntegrationTest
    {
        private NrkParser nrkParser;

        [TestFixtureSetUp]
        public void setOpp()
        {
            nrkParser = new NrkParser(10000, new NullLogger());
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
        public void TestGetForsiden()
        {
            List<Item> liste = nrkParser.GetAnbefaltePaaForsiden();
            Assert.IsNotNull(liste);
            Assert.Greater(liste.Count, 0, "Listen skal være større enn 0");
            foreach (Item item in liste)
            {
                Clip c = (Clip) item;
                //Console.WriteLine("type: " + c.Type);
                //Console.WriteLine("title: " + c.Title);
                String directLink = nrkParser.GetClipUrl(c);
                isMMSVideoStream(directLink);
                Assert.IsNotEmpty(c.ID, "ID'en kan ikke være null");
                Assert.IsNotNull(c.TilhoerendeProsjekt, "Tilhørende prosjekt skal være satt");
                Assert.IsNotEmpty(c.Description, "Beskrivelsen kan ikke være null");
                Assert.IsNotEmpty(c.Bilde, "Bilde kan ikke være null");
                Assert.IsNotEmpty(c.Title, "Tittelen kan ikke være null");
                Assert.IsTrue(c.Playable, "Klipp må være playable");
                Assert.AreEqual(Clip.KlippType.KLIPP, c.Type, "Skal være av typen KLIPP");
                Assert.IsEmpty(c.VerdiLink, "Klipp fra forsiden er ikke verdilinker");
            }
        }

        private static void isMMSVideoStream(String directLink)
        {
            Assert.IsTrue(directLink.ToLower().StartsWith("mms://"), "Videostreamer skal starte med mms://");
        }

        [Test]
        public void TestGetMestSette()
        {
            NrkTranslatableStrings.InitWithParam("en-US", @"C:\Users\Erling Reizer\Documents\Visual Studio 2005\Projects\NRKBrowser\languages");
            List<Item> liste = nrkParser.GetMestSette(31);
            Assert.IsNotNull(liste);
            Assert.Greater(liste.Count, 0, "Listen skal være større enn 0");
            Assert.AreEqual(12,liste.Count, "Listen skal ha 12 oppførsler"); 
            foreach (Item item in liste)
            {
               
                Clip c = (Clip) item;
                Assert.IsNotEmpty(c.ID, "ID'en kan ikke være null");
                Assert.IsNotEmpty(c.Description, "Beskrivelsen kan ikke være null");
                Assert.IsNotEmpty(c.Bilde, "Bilde kan ikke være null");
                //Assert.IsTrue(ErBildeFil(c.Bilde));
                Assert.IsNotEmpty(c.Title, "Tittelen kan ikke være null");
                Assert.IsTrue(c.Playable, "Klipp må være playable");
                Assert.IsTrue(erEntenKlippEllerIndexType(c), "Klipp skal være enten av type Klipp eller Index");
                Assert.IsNotEmpty(c.AntallGangerVist, "Antall ganger vist skal være satt");
                Assert.IsEmpty(c.VerdiLink, "Klipp fra mest sette er ikke verdilinker");
            }
        }

        private static bool erEntenKlippEllerIndexType(Clip c)
        {
            return c.Type == Clip.KlippType.KLIPP || c.Type == Clip.KlippType.INDEX;
        }

        private static bool ErBildeFil(String filename)
        {
            //Har etterhvert finne ut at bildefiler ikke nødvendigvis trenger å ha et filnavn, f.eks er denne i bruk:
            //http://fil.nrk.no/contentfile/imagecrop/1.413251.1144659591
            return filename.EndsWith(".jpg") || filename.EndsWith(".JPG") || filename.EndsWith(".png") || filename.EndsWith(".PNG");
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
                Assert.IsFalse(program.Playable, "Program skal ikke være spillbare");
            }
        }


        [Test]
        public void TestTopTabbDirekte()
        {
            List<Item> liste = nrkParser.GetDirektePage();
            Assert.IsNotEmpty(liste);
            Assert.AreEqual(3, liste.Count, "Skal være tre  alltid på direktelinker");
            foreach (Item item in liste)
            {
                Clip clip = (Clip) item;
                Assert.AreEqual(Clip.KlippType.DIREKTE, clip.Type);
                Assert.IsNotNull(clip.ID);
                Assert.IsNotNull(clip.Title);
                Assert.IsNotNull(clip.Description);
                Assert.IsNotNull(clip.Bilde);
            }
        }

        [Test]
        public void TestGetTopTabber()
        {
            NrkTranslatableStrings.InitWithParam("en-US", @"C:\Users\Erling Reizer\Documents\Visual Studio 2005\Projects\NRKBrowser\languages");
            List<Item> liste = nrkParser.GetTopTabber();
            Assert.IsNotNull(liste);
            Assert.IsNotEmpty(liste);
            foreach (Item item in liste)
            {
                //Console.WriteLine("id: " + item.ID);
                Assert.IsNotNull(item.ID);
                //Console.WriteLine("title: " + item.Title);
                Assert.IsNotNull(item.Title);
                //Console.Out.WriteLine("-------------------------------------------");
            }
            Assert.AreEqual(6, liste.Count, "Skal være seks oppførsler i lista"); 
            //verifisert at "super" endret seg ca 31. oktober 09 og at den nye linken går til en flashbasert side, ala p3tv tabben.

        }
        [Test]
        public void TestGetTopTabs()
        {
            List<Item> liste = nrkParser.GetTopTabContent("valg");
            Assert.IsNotEmpty(liste);

            foreach (Item item in liste)
            {
                Clip c = (Clip) item;
//                Console.WriteLine("id: " + c.ID);
                Assert.IsNotNull(c.ID);
//                Console.WriteLine("title: " + c.Title);
                Assert.IsNotNull(c.Title);
//                try
//                {
//                    Console.WriteLine("link: " + nrkParser.GetClipUrl(c));
//                }
//                catch(Exception e)
//                {
//                    Console.WriteLine("Kunne ikke finne url: "+ e.GetBaseException());
//                }
//                Console.WriteLine("bilde: " + c.Bilde);
//                Console.WriteLine("desc: " + c.Description);
                Assert.IsNotNull(c.Bilde);
                //Console.WriteLine("--------------------------");
            } 
        }

        [Test]
        public void TestGetClipUrlFraNyheterITopTab()
        {
            List<Item> liste = nrkParser.GetTopTabRSS("nyheter");
            foreach (Item item in liste)
            {
                Clip c = (Clip) item;
                string clipUrl = nrkParser.GetClipUrl(c);
                //Console.WriteLine(c.Title + ", " + c.ID);
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
                //Console.WriteLine(c.Title + ", " + c.ID);
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
                Assert.IsTrue(clipUrl.ToLower().StartsWith(NrkParserConstants.RSS_CLIPURL_PREFIX));
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
        public void TestSearchForNyttLivForCommodore64()
        {
            List<Item> liste = nrkParser.GetSearchHits("Nytt liv for Commodore 64", 0);
            Assert.AreEqual(1, liste.Count);
            Clip c = (Clip) liste[0];
            String klippUrl = nrkParser.GetClipUrl(c);
            string expectedUrl = "mms://straumod.nrk.no/disk03/Lydverket/2008-04-16/Lydverket_16_04_08_1000_358373_20080416_231000.wmv";
            Assert.AreEqual(
                expectedUrl,
                klippUrl, "KlippURL er blitt endret...denne testen er mest nyttig for å finne endringer hos nrk.");
            Assert.AreEqual(1658, c.StartTime, "Starttiden for klippet har endret seg.");
        }

    }
}