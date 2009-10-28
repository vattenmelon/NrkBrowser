/*
 * Created by: Erling Reizer
 * Created: 17. november 2008
 */

using System;
using NUnit.Framework;

namespace NrkBrowser
{
    [TestFixture]
    public class VersionCheckerTest
    {
        [Test]
        public void TestGetNewestAvailableVersion()
        {

            String version = VersionChecker.GetNewestAvailableVersion();
            Assert.AreEqual("1.3.2", version);
        }

        [Test]
        public void TestNewVersionAvailable()
        {
            //NB!! Denne testen er tvilsom, må endres på etter nye versjoner
            String v = String.Empty;
            bool version = VersionChecker.newVersionAvailable(ref v);
            Assert.AreEqual("1.3.2", v);
            Assert.IsFalse(version);
        }
    }
}