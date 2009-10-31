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
            NrkTranslatableStrings.InitWithParam("en-US", LANGUAGE_PATH);
            String s = NrkTranslatableStrings.FOR_UNIT_TESTING;
            Assert.AreEqual("Engelsk", s);
        }
        [Test]
        public void testGetStringInNorwegian()
        {
            NrkTranslatableStrings.InitWithParam("no", LANGUAGE_PATH);
            String s = NrkTranslatableStrings.FOR_UNIT_TESTING;
            Assert.AreEqual("Norsk", s);
        }
  
        [Test]
        public void testLanguageNotFoundShouldFallBackToEnglish()
        {
            NrkTranslatableStrings.InitWithParam("swe", LANGUAGE_PATH);
            String s = NrkTranslatableStrings.FOR_UNIT_TESTING;
            Assert.AreEqual("Default language", s);

        }
        [Test]
        public void CountTranslatedStrings()
        {
            ///Hvis det kommer flere spr�k s� fyll p� med tester her.
            NrkTranslatableStrings.InitWithParam("no", LANGUAGE_PATH);
            int antallNorske = NrkTranslatableStrings.GetNumberOfTranslatedStrings();
            NrkTranslatableStrings.InitWithParam("en-US", LANGUAGE_PATH);
            int antallEngelske = NrkTranslatableStrings.GetNumberOfTranslatedStrings();
            NrkTranslatableStrings.InitWithParam("swe", LANGUAGE_PATH);
            int antallswe = NrkTranslatableStrings.GetNumberOfTranslatedStrings();
            Assert.IsTrue(antallNorske == antallEngelske);
            Assert.IsFalse(antallNorske == antallswe); //det er ingen svenske oversatt
            
        }

    }

}
