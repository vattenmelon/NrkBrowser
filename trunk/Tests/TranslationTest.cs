using System;
using System.Reflection;
using NrkBrowser;
using NUnit.Framework;

namespace Translation
{
    [TestFixture]
    public class TranslationTest
    {
        

        [Test]
        public void testGetStringInEnglish()
        {
            NrkConstants.InitWithParam("en-US", @"C:\Users\Erling Reizer\Documents\Visual Studio 2005\Projects\NRKBrowser\languages");
            String s = NrkConstants.FOR_UNIT_TESTING;
            Assert.AreEqual("Engelsk", s);
        }
        [Test]
        public void testGetStringInNorwegian()
        {
            NrkConstants.InitWithParam("no", @"C:\Users\Erling Reizer\Documents\Visual Studio 2005\Projects\NRKBrowser\languages");
            String s = NrkConstants.FOR_UNIT_TESTING;
            Assert.AreEqual("Norsk", s);
        }
  
        [Test]
        public void testLanguageNotFoundShouldFallBackToEnglish()
        {
            NrkConstants.InitWithParam("swe", @"C:\Users\Erling Reizer\Documents\Visual Studio 2005\Projects\NRKBrowser\languages");
            String s = NrkConstants.FOR_UNIT_TESTING;
            Assert.AreEqual("Default language", s);

        }
        [Test]
        public void CountTranslatedStrings()
        {
            ///Hvis det kommer flere språk så fyll på med tester her.
            NrkConstants.InitWithParam("no", @"C:\Users\Erling Reizer\Documents\Visual Studio 2005\Projects\NRKBrowser\languages");
            int antallNorske = NrkConstants.GetNumberOfTranslatedStrings();
            NrkConstants.InitWithParam("en-US", @"C:\Users\Erling Reizer\Documents\Visual Studio 2005\Projects\NRKBrowser\languages");
            int antallEngelske = NrkConstants.GetNumberOfTranslatedStrings();
            NrkConstants.InitWithParam("swe", @"C:\Users\Erling Reizer\Documents\Visual Studio 2005\Projects\NRKBrowser\languages");
            int antallswe = NrkConstants.GetNumberOfTranslatedStrings();
            Assert.IsTrue(antallNorske == antallEngelske);
            Assert.IsFalse(antallNorske == antallswe); //det er ingen svenske oversatt
            
        }

    }

}
