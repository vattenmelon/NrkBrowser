/*
 * Created by: Erling Reizer
 * Created: 17. november 2008
 */

using System;
using NUnit.Framework;
using Vattenmelon.Nrk.Browser;
using Vattenmelon.Nrk.Parser;

namespace Vattenmelon.Nrk.Browser
{
    [TestFixture]
    [Category("Integration Tests")]
    public class VersionCheckerTest
    {
        [SetUp]
        public void setOpp()
        {
            VersionChecker.SetLog(new NullLogger());
        }
        [Test]
        public void TestGetNewestAvailableVersion()
        {

            String version = VersionChecker.GetNewestAvailableVersion();
            Assert.AreEqual("1.5.0", version);
        }

        [Test]
        public void TestNewVersionAvailable()
        {
            //NB!! Denne testen er tvilsom, må endres på etter nye versjoner
            String v = String.Empty;
            bool version = VersionChecker.newVersionAvailable(ref v);
            Assert.AreEqual("1.5.0", v);
            Assert.IsFalse(version);
        }
    }
}