using System;
using System.Collections.Generic;
using System.Globalization;
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
            Assert.AreEqual("NRK � 2009", podkastCopyright);
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
            List<Item> clips = podkastParser.getClips();
            Assert.AreEqual(5, clips.Count);
            Boolean funnetDel1 = false;
            Boolean funnetDel2 = false;
            Boolean funnetDel3 = false;
            Boolean funnetDel4 = false;
            Boolean funnetDel5 = false;


            foreach (Item item in clips)
            {
                Clip c = (Clip)item;
                Assert.IsNotNull(c.Title);
                Assert.IsNotNull(c.ID);
                Assert.IsNotNull(c.Klokkeslett);
                Assert.AreEqual("video/mp4", c.MediaType);
                Assert.AreEqual(Clip.KlippType.PODCAST, c.Type);
                Assert.IsNotNull("00:00:00");
                if (c.ID.Equals("http://podkast.nrk.no/fil/bergensbanen_minutt_for_minutt/nrk_bergensbanen_minutt_for_minutt_2009-1127-0755_41656.mp4"))
                {
                    Assert.AreEqual("Bergensbanen minutt for minutt - del 1", c.Title);
                    Assert.AreEqual("Bergen til Urdland. Bergensbanen gjennom fjordlandskapet til Voss. Mange tunneler  og dermed mange arkivklipp, m.a fr� togavsporing i 1948.", c.Description);
                    if (CultureInfo.CurrentCulture.ToString().Equals("nb-NO"))
                    {
                        Assert.AreEqual("7. desember 2009 16:13", c.Klokkeslett);
                    }
                    else
                    {
                        Assert.AreEqual("Monday, December 07, 2009 4:13 PM", c.Klokkeslett);
                    }
                    Assert.AreEqual("00:00:00", c.Duration);
                    Assert.AreEqual("http://fil.nrk.no/contentfile/file/1.6896395!img6896395.jpg", c.Bilde);
                    Assert.AreEqual(Clip.KlippType.PODCAST, c.Type);
                    funnetDel1 = true;
                }
                else if (c.ID.Equals("http://podkast.nrk.no/fil/bergensbanen_minutt_for_minutt/nrk_bergensbanen_minutt_for_minutt_2009-1127-0755_41655.mp4"))
                {
                    Assert.AreEqual("Bergensbanen minutt for minutt - del 2", c.Title);
                    Assert.AreEqual("Urdland til Ustaoset. Sj�lvaste h�gfjellsstrekninga p� Bergensbanen. Haustlandskapet blir gradvis meir og meir vinterleg. Her kjem utsikta ned Fl�msdalen! Vi ser m.a.klipp fr� sn�rydding i gamle dagar, h�yrer intervju med gammal rallar.", c.Description);
                    if (CultureInfo.CurrentCulture.ToString().Equals("nb-NO"))
                    {
                        Assert.AreEqual("7. desember 2009 16:05", c.Klokkeslett);
                    }
                    else
                    {
                        Assert.AreEqual("Monday, December 07, 2009 4:13 PM", c.Klokkeslett);
                    }
                    
                    Assert.AreEqual("00:00:00", c.Duration);
                    Assert.AreEqual("http://fil.nrk.no/contentfile/file/1.6896395!img6896395.jpg", c.Bilde);
                    Assert.AreEqual(Clip.KlippType.PODCAST, c.Type);
                    funnetDel2 = true;
                }
                else if (c.ID.Equals("http://podkast.nrk.no/fil/bergensbanen_minutt_for_minutt/nrk_bergensbanen_minutt_for_minutt_2009-1127-0755_41654.mp4"))
                {
                    Assert.AreEqual("Bergensbanen minutt for minutt - del 3", c.Title);
                    Assert.AreEqual("Ustaoset til Austvoll. Bergensbanen passerer Ustaoset og Geilo , og k�yrer gjennom heile Hallingdalen tilbakelagt. Vi forlet vinteren og k�yrer nedover mot hausten att. Her dukka rein kortversjon av den gamle stumfilmen �Bergenstoget plyndret i natt�. P� �l skifter vi lokomotivf�rar. ", c.Description);
                    if (CultureInfo.CurrentCulture.ToString().Equals("nb-NO"))
                    {
                        Assert.AreEqual("7. desember 2009 15:59", c.Klokkeslett);
                    }
                    else
                    {
                        Assert.AreEqual("Monday, December 07, 2009 3:59 PM", c.Klokkeslett);
                    }
                    
                    Assert.AreEqual("00:00:00", c.Duration);
                    Assert.AreEqual("http://fil.nrk.no/contentfile/file/1.6896395!img6896395.jpg", c.Bilde);
                    Assert.AreEqual(Clip.KlippType.PODCAST, c.Type);
                    funnetDel3 = true;
                }
                else if (c.ID.Equals("http://podkast.nrk.no/fil/bergensbanen_minutt_for_minutt/nrk_bergensbanen_minutt_for_minutt_2009-1127-0755_41653.mp4"))
                {
                    Assert.AreEqual("Bergensbanen minutt for minutt - del 4", c.Title);
                    Assert.AreEqual("Austvoll til Skotselv. Startar med det gamle vasst�rnet p� Austvoll og g�r gjennom roleg skogkledd landskap.. Fleire intervju med dagens passasjerar. Gjensyn med den siste postekspedisjonsvogna p� Bergensbanen.", c.Description);
                    if (CultureInfo.CurrentCulture.ToString().Equals("nb-NO"))
                    {
                        Assert.AreEqual("7. desember 2009 15:56", c.Klokkeslett);
                    }
                    else
                    {
                        Assert.AreEqual("Monday, December 07, 2009 3:56 PM", c.Klokkeslett);
                    }
                    
                    Assert.AreEqual("00:00:00", c.Duration);
                    Assert.AreEqual("http://fil.nrk.no/contentfile/file/1.6896395!img6896395.jpg", c.Bilde);
                    Assert.AreEqual(Clip.KlippType.PODCAST, c.Type);
                    funnetDel4 = true;
                }
                else if (c.ID.Equals("http://podkast.nrk.no/fil/bergensbanen_minutt_for_minutt/nrk_bergensbanen_minutt_for_minutt_2009-1130-0755_41652.mp4"))
                {
                    Assert.AreEqual("Bergensbanen minutt for minutt - del 5", c.Title);
                    Assert.AreEqual("Skotselv - Oslo. Ransfjordsbanen og Drammensbanen inn mot hovudstaden.", c.Description);
                    if (CultureInfo.CurrentCulture.ToString().Equals("nb-NO"))
                    {
                        Assert.AreEqual("7. desember 2009 15:46", c.Klokkeslett);
                    }
                    else
                    {
                        Assert.AreEqual("Monday, December 07, 2009 3:46 PM", c.Klokkeslett);
                    }
                    
                    Assert.AreEqual("00:00:00", c.Duration);
                    Assert.AreEqual("http://fil.nrk.no/contentfile/file/1.6896395!img6896395.jpg", c.Bilde);
                    Assert.AreEqual(Clip.KlippType.PODCAST, c.Type);
                    funnetDel5 = true;
                }
                else
                {
                    Assert.Fail("Skal aldri komme hit!");
                }
            }
            Assert.IsTrue(funnetDel1);
            Assert.IsTrue(funnetDel2);
            Assert.IsTrue(funnetDel3);
            Assert.IsTrue(funnetDel4);
            Assert.IsTrue(funnetDel5);

        }
    }
}