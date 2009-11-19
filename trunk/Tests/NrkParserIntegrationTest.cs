using System;
using System.Collections.Generic;
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
        public void TestGetClipsTilhoerendeSammeProgram()
        {
            int prosjektId = 1194;
            string clipId = "567540";
            Clip clip = new Clip(clipId, "True Blood");
            clip.TilhoerendeProsjekt = prosjektId;
            List<Item> itemsTilHoerendeTrueBlood = nrkParser.GetClipsTilhoerendeSammeProgram(clip);
            Assert.IsNotEmpty(itemsTilHoerendeTrueBlood);
            foreach (Item item in itemsTilHoerendeTrueBlood)
            {
                Assert.IsTrue(ItemErEntenClipEllerFolder(item));
            }
        }
        [Test]
        public void TestGetClipsTilhoerendeSammeProgramForbrukerInspektoerene()
        {
            int prosjektId = 67;
            string clipId = "574251";
            Clip clip = new Clip(clipId, "Forbruker");
            clip.TilhoerendeProsjekt = prosjektId;
            List<Item> itemsTilHoerendeTrueBlood = nrkParser.GetClipsTilhoerendeSammeProgram(clip);
            Assert.IsNotEmpty(itemsTilHoerendeTrueBlood);
            bool funnetMinstEtKlipp = false;
            foreach (Item item in itemsTilHoerendeTrueBlood)
            {
                Assert.IsTrue(ItemErEntenClipEllerFolder(item));
                if (item is Clip)
                {
                    Assert.AreEqual(prosjektId, ((Clip)item).TilhoerendeProsjekt);
                    funnetMinstEtKlipp = true;
                }
            }
            Assert.IsTrue(funnetMinstEtKlipp);
        }

        private bool ItemErEntenClipEllerFolder(Item item)
        {
            return item is Clip || item is Folder;
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

        [Test]
        public void TestGetForsidenChangeDetectionTestAntall()
        {
            List<Item> liste = nrkParser.GetAnbefaltePaaForsiden();
            Assert.AreEqual(52, liste.Count); //Verified to be fifty 2009-11-19
        }

        private static void isMMSVideoStream(String directLink)
        {
            Assert.IsTrue(directLink.ToLower().StartsWith("mms://"), "Videostreamer skal starte med mms://");
        }

        [Test]
        public void TestGetMestSette()
        {
            
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
        public void TestGetAllProgramsChangeDetectionTest()
        {
            List<Item> liste = nrkParser.GetAllPrograms();
            Assert.Greater(liste.Count, 500, "Verifisert til å være over 500: 2009-11-16"); //Var 513: 2009-11-16 
        }


        [Test]
        public void TestTopTabbDirekte()
        {
            List<Item> liste = nrkParser.GetDirektePage();
            Assert.IsNotEmpty(liste);
            Assert.AreEqual(3, liste.Count, "Skal være tre alltid på direktelinker");
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
            List<Item> liste = nrkParser.GetTopTabber();
            Assert.IsNotNull(liste);
            Assert.IsNotEmpty(liste);
            foreach (Item item in liste)
            {
                Assert.IsNotNull(item.ID);
                Assert.IsNotNull(item.Title);
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
                Assert.IsNotNull(c.ID);
                Assert.AreEqual(Clip.KlippType.VERDI, c.Type);
                Assert.IsNotNull(c.Title);
//                try
//                {
//                    Console.WriteLine(c.Type + "link: c " + c.ID + " " + nrkParser.GetClipUrl(c));
//                }
//                catch(Exception e)
//                {
//                    Console.WriteLine("Kunne ikke finne url: "+ e.GetBaseException());
//                }
                Assert.IsNotNull(c.Bilde);

            } 
        }
        [Test]
        public void TestGetVerdiClipUrlWhereNrkReportsCouldNotFindVideoChangeDetectionTest()
        {
            Clip c = new Clip("109378", "finnesikke");
            c.Type = Clip.KlippType.VERDI;
            String klippUrl = nrkParser.GetClipUrl(c);
            Assert.IsNull(klippUrl, "Verifisert at Nrk ikke finner videoen 2009-11-16");
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
                erHttpLink(clipUrl);
            }
        }

        private static void erHttpLink(string clipUrl)
        {
            Assert.IsFalse(clipUrl.ToLower().StartsWith("mms://"));
            Assert.IsFalse(clipUrl.ToLower().EndsWith(".wmv"));
            Assert.IsTrue(clipUrl.ToLower().StartsWith("http://"));
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
                erHttpLink(clipUrl);
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
                erHttpLink(clipUrl);
            }
        }


        [Test]
        public void TestHentProgrammer()
        {
            List<Item> categories = nrkParser.GetCategories();
            Boolean funnetProgrammerForBarn = false;
            foreach (Item item in categories)
            {
                Category k = (Category) item;
                if (k.Title.Equals("Barn"))
                {
                    List<Item> programmer = nrkParser.GetPrograms(k);
                    foreach (Item item1 in programmer)
                    {
                        funnetProgrammerForBarn = true;
                        Program p = (Program) item1;
                        Assert.IsNotNull(p.ID, "ID skal ikke være null på et program");
                        Assert.IsNotNull(p.Title, "Title skal ikke være null på et program");
                        Assert.IsNotNull(p.Bilde, "Bilde skal ikke være null på et program");
                        Assert.IsNotNull(p.Description, "Beskrivelse skal ikke være null på et program");
                        Assert.IsFalse(p.Playable, "Et program skal ikke være playable");
                    }
                }
            }
            Assert.IsTrue(funnetProgrammerForBarn);
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


        [Test]
        public void TestGetMestSetteNyheterSistUke()
        {
            List<Item> items = nrkParser.GetMestSetteForKategoriOgPeriode(NrkParser.Periode.Uke, "Nyheter");
            Assert.IsNotEmpty(items);
            Assert.AreEqual(20, items.Count);
            foreach (Item item in items)
            {
                AssertValidMestSette(item);
            }
        }

        /// <summary>
        /// Vanskelig å asserte på videostreamen siden det hender at klipp ikke er tilgjengelige hos nrk.
        /// </summary>
        /// <param name="item"></param>
        private void AssertValidMestSette(Item item)
        {
            Clip c = (Clip) item;
            
                Console.WriteLine("Tittel.............: " + c.Title);
                Console.WriteLine("ID.................: " + c.ID);
                Console.WriteLine("Beskrivelse........: " + c.Description);
                Console.WriteLine("Bilde..............: " + c.Bilde);
                Console.WriteLine("Type...............: " + c.Type);
                Console.WriteLine("Antall ganger vist.: " + c.AntallGangerVist);
                //String videoLink = nrkParser.GetClipUrl(c);
                //Console.WriteLine("Videostream........: " + videoLink);
                Console.WriteLine("--------------------------------------------");
            Assert.IsNotEmpty(c.Title);
            Assert.IsNotEmpty(c.ID);
            Assert.IsNotEmpty(c.Description);
            Assert.IsNotEmpty(c.Bilde);
            Assert.IsNotEmpty(c.AntallGangerVist);
//            if (c.Type == Clip.KlippType.KLIPP)
//            {
//                Assert.IsTrue(videoLink.EndsWith(".wmv"));
//            }
//            else
//            {
//                erHttpLink(videoLink);
//            }
        }

        [Test]
        public void TestGetMestSetteNyheterSistMaaned()
        {
            List<Item> items = nrkParser.GetMestSetteForKategoriOgPeriode(NrkParser.Periode.Maned, "Nyheter");
            Assert.IsNotEmpty(items);
            Assert.AreEqual(20, items.Count);
            foreach (Item item in items)
            {
                AssertValidMestSette(item);
            }
        }

        [Test]
        public void TestGetMestSetteNyheterTotalt()
        {
            List<Item> items = nrkParser.GetMestSetteForKategoriOgPeriode(NrkParser.Periode.Totalt, "Nyheter");
            Assert.IsNotEmpty(items);
            Assert.AreEqual(20, items.Count);
            foreach (Item item in items)
            {
                AssertValidMestSette(item);
            }
        }

        [Test]
        public void TestGetMestSetteSportSistUke()
        {
            List<Item> items = nrkParser.GetMestSetteForKategoriOgPeriode(NrkParser.Periode.Uke, "Sport");
            Assert.IsNotEmpty(items);
            Assert.AreEqual(20, items.Count);
            foreach (Item item in items)
            {
                AssertValidMestSette(item);
            }
        }

        [Test]
        public void TestGetMestSetteSportSistMaaned()
        {
            List<Item> items = nrkParser.GetMestSetteForKategoriOgPeriode(NrkParser.Periode.Maned, "Sport");
            Assert.IsNotEmpty(items);
            Assert.AreEqual(20, items.Count);
            foreach (Item item in items)
            {
                AssertValidMestSette(item);
            }
        }

        [Test]
        public void TestGetMestSetteSportTotalt()
        {
            List<Item> items = nrkParser.GetMestSetteForKategoriOgPeriode(NrkParser.Periode.Totalt, "Sport");
            Assert.IsNotEmpty(items);
            Assert.AreEqual(20, items.Count);
            foreach (Item item in items)
            {
                AssertValidMestSette(item);
            }
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void TestGetMestSetteForInvalidCategory()
        {
            nrkParser.GetMestSetteForKategoriOgPeriode(NrkParser.Periode.Totalt, "heythizdoesnoetexists");
            
        }

    }
}