/*
 * Created by: Vattenmelon
 * Created: 4. november 2009
 */


using System.Threading;
using Vattenmelon.Nrk.Domain;
using NUnit.Framework;

namespace Vattenmelon.Nrk.Browser
{
    [TestFixture]
    public class UpdatePlayBackInfoTest
    {
        [Test]
        public void SmokeTest()
        {
           Clip item = new Clip("Id", "Tittel");
           UpdatePlayBackInfo updater = new UpdatePlayBackInfo(2000, item);    
           Assert.IsFalse(updater.finished);
           Thread.Sleep(2500);
           Assert.IsTrue(updater.finished);
        }

    }
}