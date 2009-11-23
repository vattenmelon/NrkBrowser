using System;
using System.Collections.Generic;
using Tests;
using Vattenmelon.Nrk.Parser;
using Vattenmelon.Nrk.Domain;
using NUnit.Framework;
using Vattenmelon.Nrk.Parser.Http;

namespace Vattenmelon.Nrk.Parser
{
    [TestFixture]
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
            Assert.Greater(liste.Count, 0, "Listen skal være større enn 0");
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
            }
        }
        //TODO: duplicate test code
        private static bool erEntenKlippEllerIndexType(Clip c)
        {
            return c.Type == Clip.KlippType.KLIPP || c.Type == Clip.KlippType.INDEX;
        }
    }

}