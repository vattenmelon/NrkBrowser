using System;
using System.Collections.Generic;
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
    }
}
