/*
 * Created by: Erling Reizer
 * Created: 17. november 2008
 */

using NUnit.Framework;

namespace NrkBrowser
{
    [TestFixture]
    public class UtilsTest
    {
        [Test]
        public void TestConvertToDouble()
        {
            double d1 = NrkUtils.convertToDouble("00:00:01");
            Assert.AreEqual(1, d1, "Skal være likt");
            d1 = NrkUtils.convertToDouble("00:00:02");
            Assert.AreEqual(2, d1, "Skal være likt");
            d1 = NrkUtils.convertToDouble("00:00:20");
            Assert.AreEqual(20, d1, "Skal være likt");
            d1 = NrkUtils.convertToDouble("00:01:00");
            Assert.AreEqual(60, d1, "Skal være likt");
            d1 = NrkUtils.convertToDouble("00:10:10");
            Assert.AreEqual(610, d1, "Skal være likt");
            d1 = NrkUtils.convertToDouble("01:00:00");
            Assert.AreEqual(3600, d1, "Skal være likt");
            d1 = NrkUtils.convertToDouble("10:10:10");
            Assert.AreEqual(36610, d1, "Skal være likt");
        }

        [Test]
        public void TestParseKlokkeslettFraBilde()
        {
            string bildeFil = "http://fil.nrk.no/nett-tv/data/stillbilder/nsps_upload_2009_7_29_17_53_57_1390_loop.jpg";
            string klokkeslett = NrkUtils.parseKlokkeSlettFraBilde(bildeFil);
            Assert.AreEqual("17:53 29/7-2009", klokkeslett);

            bildeFil = "http://fil.nrk.no/nett-tv/data/stillbilder/nsps_upload_2009_9_12_11_2_21_1419_loop.jpg";
            klokkeslett = NrkUtils.parseKlokkeSlettFraBilde(bildeFil);
            Assert.AreEqual("11:02 12/9-2009", klokkeslett);
        }

        [Test]
        public void TestParseKlokkeslettFraBildeDersomDetIkkeGaarSkalDetReturneresBlank()
        {
            string bildeFil = "http://fil.nrk.no/nett-tv/data/stillbilder/nsps_upload_2009adsf_1390_loop.jpg";
            string klokkeslett = NrkUtils.parseKlokkeSlettFraBilde(bildeFil);
            Assert.AreEqual("", klokkeslett);
        }
    }
}