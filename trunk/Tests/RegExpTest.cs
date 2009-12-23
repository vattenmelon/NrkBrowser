using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Vattenmelon.Nrk.Parser;

namespace Tests
{
    [TestFixture]
    [Category("Test-Tests")]
    public class RegExpTest
    {
        [Test]
        public void TestRegExp()
        {
            string data;
            data = "<img src=\"abba\" width=\"100\" height=\"57\">";
            //?<title>[^<]*
            Regex query =
                new Regex("<img src=\"([a-zA-Z0-9]*)\" width=\"100\" height=\"57\">",
                    RegexOptions.Compiled | RegexOptions.Singleline);
            MatchCollection matches = query.Matches(data);

            String s = null;
            foreach (Match x in matches)
            {
               s = x.Groups[1].Value;
            }
            Assert.AreEqual("abba", s);
            
        }

        [Test]
        public void TestRegExp2()
        {
            string data;
            data = "<img src=\"abba\" width=\"100\" height=\"57\">";
            //?<title>[^<]*
            Regex query =
                new Regex("<img src=\"([a-zA-Z0-9]*)\" width=\"100\" height=\"57\">",
                    RegexOptions.Compiled | RegexOptions.Singleline);
            Match m = query.Match(data);

            String s = null;
            while(m.Success){
                s = m.Groups[1].Value;
                m = m.NextMatch();
            }

            Assert.AreEqual("abba", s);

        }

        [Test]
        public void TestRegExp3()
        {
            string data;
            data = "<img src=\"abba\" width=\"100\" height=\"57\">";
            //?<title>[^<]*
            Regex query =
                new Regex("<img src=\"(?<hent>.*)\" width=\".*?\" height=\"57\">",
                    RegexOptions.Compiled | RegexOptions.Singleline);
            MatchCollection matches = query.Matches(data);

            String s = null;
            foreach (Match x in matches)
            {
                s = x.Groups["hent"].Value;
            }
            Assert.AreEqual("abba", s);

        }
        [Test]
        public void DateFormatTest()
        {
            String klokkeslett = "Mon, 07 Dec 2009 14:46:00 GMT";
            DateTime dt = DateTime.Parse(klokkeslett);

            CultureInfo current = CultureInfo.CurrentCulture;
            String dtSomString = dt.ToString();
            if (current.ToString().Equals("en-US"))
            {
                Assert.AreEqual("12/7/2009 3:46:00 PM", dtSomString); 
            }
            else if (current.ToString().Equals("nb-NO"))
            {
                Assert.AreEqual("07.12.2009 15:46:00", dtSomString); 
            }
            else
            {
                Assert.Fail(string.Format("Skal ikke komme hit, var ingen av cultureinfoene, current cultureinfo er: {0}", CultureInfo.CurrentCulture.ToString()));
            }
 
            dtSomString = dt.ToShortDateString();
            if (current.ToString().Equals("en-US"))
            {
                Assert.AreEqual("12/7/2009", dtSomString);
            }
            else if (current.ToString().Equals("nb-NO"))
            {
                Assert.AreEqual("07.12.2009", dtSomString);
            }
            else
            {
                Assert.Fail(string.Format("Skal ikke komme hit, var ingen av cultureinfoene, current cultureinfo er: {0}", CultureInfo.CurrentCulture.ToString()));
            }
            
            
            
            CultureInfo cu = new CultureInfo("en-US");
            dtSomString = dt.ToString(cu);
            Assert.AreEqual("12/7/2009 3:46:00 PM", dtSomString);
            cu = new CultureInfo("nb-NO");
            dtSomString = dt.ToString(cu);
            //because of different formatting in mono and .net
            if (System.Environment.OSVersion.ToString().ToLower().Contains("unix"))
            {
                Assert.AreEqual("07.12.09 15:46:00 +1", dtSomString);
            }
            else
            {
                Assert.AreEqual("07.12.2009 15:46:00", dtSomString);
            }
            
            cu = new CultureInfo("nn-NO");
            dtSomString = dt.ToString(cu);
            Assert.AreEqual("07.12.2009 15:46:00", dtSomString);
            cu = new CultureInfo("nb-NO");
            dtSomString = dt.ToString("f", cu);
            Assert.AreEqual("7. desember 2009 15:46", dtSomString);
            
        }
    }
}
