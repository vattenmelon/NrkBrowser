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

    }

}