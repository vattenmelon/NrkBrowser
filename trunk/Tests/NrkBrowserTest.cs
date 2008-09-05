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
            foreach (Item item in natur)
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
            foreach (Item item in liste)
            {
                Clip c = (Clip)item;
                Assert.IsNotEmpty(c.ID, "ID'en kan ikke være null");
                Assert.IsNotEmpty(c.Description, "Beskrivelsen kan ikke være null");
                Assert.IsNotEmpty(c.Bilde, "Bilde kan ikke være null");
                Assert.IsNotEmpty(c.Title, "Tittelen kan ikke være null");
                Assert.IsTrue(c.Playable, "Klipp må være playable");
                Assert.IsTrue(c.Playable, "Klipp må være playable");
                Assert.AreEqual(Clip.KlippType.KLIPP, c.Type, "Skal være av typen KLIPP");

            }
        }
    }
}