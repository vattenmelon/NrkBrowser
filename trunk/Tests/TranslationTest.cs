using System;
using System.Reflection;
using NrkBrowser;
using NUnit.Framework;

namespace Translation
{
    [TestFixture]
    public class TranslationTest
    {
        private static string LANGUAGE_PATH = @"C:\Documents and Settings\Erling\My Documents\Visual Studio 2008\nrkbrowser\languages";

        [Test]
        public void testGetStringInEnglish()
        {
            NrkConstants.InitWithParam("en-US", LANGUAGE_PATH);
            String s = NrkConstants.FOR_UNIT_TESTING;
            Assert.AreEqual("Engelsk", s);
        }
        [Test]
        public void testGetStringInNorwegian()
        {
            NrkConstants.InitWithParam("no", LANGUAGE_PATH);
            String s = NrkConstants.FOR_UNIT_TESTING;
            Assert.AreEqual("Norsk", s);
        }
  
        [Test]
        public void testLanguageNotFoundShouldFallBackToEnglish()
        {
            NrkConstants.InitWithParam("swe", LANGUAGE_PATH);
            String s = NrkConstants.FOR_UNIT_TESTING;
            Assert.AreEqual("Default language", s);

        }
        [Test]
        public void CountTranslatedStrings()
        {
            ///Hvis det kommer flere språk så fyll på med tester her.
            NrkConstants.InitWithParam("no", LANGUAGE_PATH);
            int antallNorske = NrkConstants.GetNumberOfTranslatedStrings();
            NrkConstants.InitWithParam("en-US", LANGUAGE_PATH);
            int antallEngelske = NrkConstants.GetNumberOfTranslatedStrings();
            NrkConstants.InitWithParam("swe", LANGUAGE_PATH);
            int antallswe = NrkConstants.GetNumberOfTranslatedStrings();
            Assert.IsTrue(antallNorske == antallEngelske);
            Assert.IsFalse(antallNorske == antallswe); //det er ingen svenske oversatt
            
        }

    }

}
