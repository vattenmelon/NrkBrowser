using System;
using System.Collections.Generic;
using System.Xml;
using NrkBrowser.Domain;
using Vattenmelon.Nrk.Parser;
using Vattenmelon.Nrk.Domain;
using NUnit.Framework;
using Vattenmelon.Nrk.Parser.Xml;

namespace Vattenmelon.Nrk.Parser
{
    [TestFixture]
    [Category("Unit Tests")]
    public class NrkParserTest
    {
        private NrkParser nrkParser;
         
        [TestFixtureSetUp]
        public void setOpp()
        {
            nrkParser = new NrkParser(10000, new NullLogger());
            nrkParser.HttpClient = new StubHttpClient();
             
        }

       

        [Test]
        public void TestHentKategorier()
        {
            List<Item> categories = nrkParser.GetCategories();
            Assert.AreEqual(16, categories.Count);
            foreach (Item item in categories)
            {
                Category kat = (Category)item;
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
            Assert.AreEqual(45, liste.Count, "Skal være 45 oppførsler i lista");
            foreach (Item item in liste)
            {
                Clip c = (Clip)item;
                Assert.IsNotEmpty(c.ID, "ID'en kan ikke være null");
                Assert.IsNotNull(c.TilhoerendeProsjekt, "Tilhørende prosjekt skal være satt");
                Assert.IsNotEmpty(c.Description, "Beskrivelsen kan ikke være null");
                Assert.IsNotEmpty(c.Bilde, "Bilde kan ikke være null");
                Assert.IsNotEmpty(c.Title, "Tittelen kan ikke være null");
                Assert.IsTrue(c.Playable, "Klipp må være playable");
                Assert.AreEqual(Clip.KlippType.KLIPP, c.Type, "Skal være av typen KLIPP");
                Assert.IsEmpty(c.VerdiLink, "Klipp fra forsiden er ikke verdilinker");
                Assert.IsTrue(erJpgFilBortsettFraDenEne(c));
            }
        }

        /*Is only stubbed unit test */
        private bool erJpgFilBortsettFraDenEne(Clip c)
        {
            if (c.Bilde.Equals("http://fil.nrk.no/contentfile/imagecrop/1.6180984.1219045814"))
            {
                return true;
            }
            else
            {
                return c.Bilde.EndsWith(".jpg");
            }
        }

        [Test]
        public void TestGetMestSette()
        {

            List<Item> liste = nrkParser.GetMestSette(31);
            Assert.IsNotNull(liste);
            Assert.Greater(liste.Count, 0, "Listen skal være større enn 0");
            Assert.AreEqual(12, liste.Count, "Listen skal ha 12 oppførsler");
            foreach (Item item in liste)
            {

                Clip c = (Clip)item;
                Assert.IsNotEmpty(c.ID, "ID'en kan ikke være null");
                Assert.IsNotEmpty(c.Description, "Beskrivelsen kan ikke være null");
                Assert.IsNotEmpty(c.Bilde, "Bilde kan ikke være null");
                Assert.IsNotEmpty(c.Title, "Tittelen kan ikke være null");
                Assert.IsTrue(c.Playable, "Klipp må være playable");
                Assert.IsTrue(erEntenKlippEllerIndexType(c), "Klipp skal være enten av type Klipp eller Index");
                Assert.IsNotEmpty(c.AntallGangerVist, "Antall ganger vist skal være satt");
                Assert.IsEmpty(c.VerdiLink, "Klipp fra mest sette er ikke verdilinker");
                Assert.AreEqual(Clip.KlippType.KLIPP, c.Type);
            }
        }
        //TODO: duplicate test code
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
                Program program = (Program)item;
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
            Assert.AreEqual(3, liste.Count, "Skal være tre alltid på direktelinker");
            foreach (Item item in liste)
            {
                Clip clip = (Clip)item;
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
                Assert.IsTrue(item.Title.Equals("Direkte") || item.Title.Equals("Nyheter") || item.Title.Equals("Sport") || item.Title.Equals("Valget") || item.Title.Equals("Distrikt") || item.Title.Equals("Natur"));
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
                Clip c = (Clip)item;
                Assert.IsNotNull(c.ID);
                Assert.AreEqual(Clip.KlippType.VERDI, c.Type);
                Assert.IsNotNull(c.Title);
                //                try
                //                {
                //                    Console.WriteLine(c.Type + "link: c " + c.ID + " " + nrkParser.GetClipUrlAndPutStartTime(c));
                //                }
                //                catch(Exception e)
                //                {
                //                    Console.WriteLine("Kunne ikke finne url: "+ e.GetBaseException());
                //                }
                Assert.IsNotNull(c.Bilde);

            }
        }

        [Test]
        public void TestHentProgrammer()
        {
            List<Item> categories = nrkParser.GetCategories();
            bool funnetProgrammerForBarn = false;
            foreach (Item item in categories)
            {
                Category k = (Category)item;
                if (k.Title.Equals("Barn"))
                {
                    List<Item> programmer = nrkParser.GetPrograms(k);
                    foreach (Item item1 in programmer)
                    {
                        funnetProgrammerForBarn = true;
                        Program p = (Program)item1;
                        Assert.IsNotNull(p.ID, "ID skal ikke være null på et program");
                        Assert.IsNotNull(p.Title, "Title skal ikke være null på et program");
                        Assert.IsNotNull(p.Bilde, "Bilde skal ikke være null på et program");
                        Assert.IsTrue(p.Bilde.StartsWith("http://fil.nrk.no/contentfile"));
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

        [Test]
        public void TestGetVideoPodkaster()
        {
            IList<PodKast> items = nrkParser.GetVideoPodkaster();
            Assert.IsNotEmpty(items as List<PodKast>);
            Boolean funnetBokprogrammet = false;
            Boolean funnetBergensbanen = false;
            foreach (PodKast podKast in items)
            {
                if (podKast.ID.Equals("http://podkast.nrk.no/program/bokprogrammet.rss"))
                {
                    Assert.AreEqual("Bokprogrammet (NRK1)", podKast.Title);
                    Assert.AreEqual("Hans Olav Brenner møter forfattere.", podKast.Description);
                    funnetBokprogrammet = true;
                }
                else if (podKast.ID.Equals("http://podkast.nrk.no/program/bergensbanen_minutt_for_minutt.rss"))
                {
                    Assert.AreEqual("Bergensbanen minutt for minutt (NRK)", podKast.Title);
                    Assert.AreEqual("Bergensbanen bokstavlig talt minutt for minutt.", podKast.Description);
                    funnetBergensbanen = true;
                }
            }
            Assert.AreEqual(24, items.Count); //22/12-09: det er 27, men bare fireogtyve har rss feed.
            Assert.IsTrue(funnetBokprogrammet);
            Assert.IsTrue(funnetBergensbanen);
            
        }

        [Test]
        public void TestGetLydPodkaster()
        {
            IList<PodKast> items = nrkParser.GetLydPodkaster();
            Assert.IsNotEmpty(items as List<PodKast>);
            Boolean funnetRadioResepsjonenutenmusikk = false;
            Boolean funnetmorketsOpplevelser = false;
            Boolean funnetTelemarkssendinga = false;
            foreach (PodKast podKast in items)
            {
                if (podKast.ID.Equals("http://podkast.nrk.no/program/radioresepsjonen.rss"))
                {
                    Assert.AreEqual("Radioresepsjonen uten musikk (NRK P3)", podKast.Title);
                    Assert.AreEqual("En feit, en lang og en gammel mann prøver å lage program på P3.", podKast.Description);
                    funnetRadioResepsjonenutenmusikk = true;
                }
                else if (podKast.ID.Equals("http://podkast.nrk.no/program/moerkets_opplevelser.rss"))
                {
                    Assert.AreEqual("Mørkets opplevelser (NRK P2)", podKast.Title);
                    Assert.AreEqual("Et bredt filmprogram som er opptatt av smale filmer. Som skiller mellom gull og glitter og beundrer stjerner uten å la seg blende. Ny podkast hver torsdag.", podKast.Description);
                    funnetmorketsOpplevelser = true; 
                }
                else if (podKast.ID.Equals("http://podkast.nrk.no/program/telemarksendinga.rss"))
                {
                    Assert.AreEqual("Telemarksendinga (NRK P1)", podKast.Title);
                    Assert.AreEqual("Lokal nyhetshalvtime fra 1630. Ny podkast alle hverdager.", podKast.Description);
                    funnetTelemarkssendinga = true;
                }
            }
            Assert.IsTrue(funnetRadioResepsjonenutenmusikk);
            Assert.IsTrue(funnetmorketsOpplevelser);
            Assert.IsTrue(funnetTelemarkssendinga);
            Assert.AreEqual(77, items.Count);

        }
        //TODO: duplicate code
        /// <summary>
        /// Vanskelig å asserte på videostreamen siden det hender at klipp ikke er tilgjengelige hos nrk.
        /// </summary>
        /// <param name="item"></param>
        private void AssertValidMestSette(Item item)
        {
            Clip c = (Clip)item;

            //                Console.WriteLine("Tittel.............: " + c.Title);
            //                Console.WriteLine("ID.................: " + c.ID);
            //                Console.WriteLine("Beskrivelse........: " + c.Description);
            //                Console.WriteLine("Bilde..............: " + c.Bilde);
            //                Console.WriteLine("Type...............: " + c.Type);
            //                Console.WriteLine("Antall ganger vist.: " + c.AntallGangerVist);
            //                Console.WriteLine("Klokkeslett........: " + c.Klokkeslett);
            //String videoLink = nrkParser.GetClipUrlAndPutStartTime(c);
            //Console.WriteLine("Videostream........: " + videoLink);
            Assert.IsNotEmpty(c.Title);
            Assert.IsNotEmpty(c.ID);
            Assert.IsEmpty(c.Description);
            Assert.IsNotEmpty(c.Klokkeslett);
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

    }
}