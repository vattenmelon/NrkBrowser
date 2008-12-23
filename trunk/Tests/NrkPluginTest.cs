/*
 * Created by: 
 * Created: 17. november 2008
 */

using System;
using NUnit.Framework;

namespace NrkBrowser
{
    [TestFixture]
    public class NrkPluginTest
    {
        private NrkPlugin nrkPlugin;
        private static int pluginId = 40918376;

        [TestFixtureSetUp]
        public void setOpp()
        {
            nrkPlugin = new NrkPlugin();
        }

        [Test]
        public void testGetID()
        {
            Assert.AreEqual(pluginId, nrkPlugin.GetID);
        }

        [Test]
        public void testInit()
        {
            Assert.IsTrue(nrkPlugin.Init());
        }

//        [Test]
//        public void checkForNewVersionTest()
//        {
//            String nyVer = String.Empty;
//            Assert.IsFalse(NrkPlugin.newVersionAvailable(ref nyVer));
//        }
    }
}