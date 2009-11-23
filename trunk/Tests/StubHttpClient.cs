using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vattenmelon.Nrk.Parser.Http;

namespace Tests
{
    public class StubHttpClient : IHttpClient
    {
        public string GetUrl(string url)
        {
            if (url.Equals("http://www1.nrk.no/nett-tv/"))
            {
                return readFile("../../stubfiler/allcategories.html");
            }
            else
            {
                throw new Exception("Finnes ikke stubb for denne url'en: " + url);
            }
        }

        public string PostUrl(string url, string postData)
        {
            throw new NotImplementedException();
        }

        private string readFile(String fileName)
        {
            // create reader & open file
            TextReader tr = new StreamReader(fileName);

            // read a line of text
            string s = tr.ReadToEnd();

            // close the stream
            tr.Close();

            return s;
        }
    }
}
