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
            TextReader textReader = new StreamReader(fileName);
            string s = textReader.ReadToEnd();
            textReader.Close();

            return s;
        }

        public static void WriteToFile(String text, String filename)
        {
            TextWriter textWriter = new StreamWriter(filename);
            textWriter.Write(text);
            textWriter.Close();
        }
    }
}
