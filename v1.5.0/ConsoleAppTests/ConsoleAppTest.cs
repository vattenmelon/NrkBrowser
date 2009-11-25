using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Vattenmelon.Nrk.Parser;

namespace ConsoleAppTests
{
    [TestFixture]
    public class ConsoleAppTest
    {
        [Test]
        public void TestPrintVersion()
        {
           TestApp.printVersionAndExit();
            
        }
        [Test]
        public void TestPrintMostWatched()
        {
            TestApp.printMostWatchedAndExit("365");
        }
    }
}
