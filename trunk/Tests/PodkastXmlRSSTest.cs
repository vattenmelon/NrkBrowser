using System;
using System.Collections.Generic;
using Vattenmelon.Nrk.Domain;
using NUnit.Framework;

namespace Vattenmelon.Nrk.Parser.Xml
{
    [TestFixture]
    [Category("Unit Tests")]
    public class PodkastXMLRSSTest
    {
        private PodkastXmlParser podkastParser;

        [SetUp]
        public void OpprettParser()
        {
            podkastParser = new PodkastXmlParser("../../stubfiler/bergensbanen_minutt_for_minutt.rss");
        }
        

        [Test]
        public void TestPodKastBergensbanenGetPicture()
        { 
            String podKastPictureUrl = podkastParser.getPodkastPicture();
            Assert.AreEqual("http://fil.nrk.no/contentfile/file/1.6896395!img6896395.jpg", podKastPictureUrl);
        }

        [Test]
        public void TestPodKastBergensbanenGetDescription()
        {
            String podKastDescription = podkastParser.getPodkastDescription();
            Assert.AreEqual("Bergensbanen minutt for minutt", podKastDescription);
        }

        [Test]
        public void TestPodKastBergensbanenGetCopyright()
        {
            String podkastCopyright = podkastParser.getPodkastCopyright();
            Assert.AreEqual("NRK © 2009", podkastCopyright);
        }

        [Test]
        public void TestPodKastBergensbanenGetAuthor()
        {
            String podkastAuthor = podkastParser.getPodkastAuthor();
            Assert.AreEqual("NRK2", podkastAuthor);
        }
       
        [Test]
        public void TestPodKastBergensbanen()
        {
            List<Item> natur = podkastParser.getClips();
            String podKastPictureUrl = podkastParser.getPodkastPicture();
            Assert.AreEqual("http://fil.nrk.no/contentfile/file/1.6896395!img6896395.jpg", podKastPictureUrl);
            //Assert.AreEqual();
            
            foreach (Item item in natur)
            {
                Clip c = (Clip)item;
                            Console.WriteLine("Tittel.............: " + c.Title);
                            Console.WriteLine("ID.................: " + c.ID);
                            Console.WriteLine("Beskrivelse........: " + c.Description);
                            Console.WriteLine("Bilde..............: " + c.Bilde);
                            Console.WriteLine("Type...............: " + c.Type);
                            Console.WriteLine("Antall ganger vist.: " + c.AntallGangerVist);
                            Console.WriteLine("Klokkeslett........: " + c.Klokkeslett);
                            Console.WriteLine("-----------------------------------------");
            }

        }
    }
}