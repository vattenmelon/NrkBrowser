using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Vattenmelon.Nrk.Parser.Http;

namespace Vattenmelon.Nrk.Parser
{
    public class StubHttpClient : IHttpClient
    {
        Dictionary<String, String> urls = new Dictionary<string, string>();

        public StubHttpClient()
        {
            registerUrls();
        }

        public string GetUrl(string url)
        {
            Console.WriteLine("StubHttpClient--->GetUrl: " + url);
            String fileName = urls[url];
            if (fileName == null)
            {
                throw new Exception("Finnes ikke stubb for denne url'en: " + url);
            }
            Console.WriteLine("                 -----> " +fileName);
            return readFile(fileName);
          
        }

        private void registerUrls()
        {     
            urls.Add("http://www1.nrk.no/nett-tv/", "../../stubfiler/allcategories.html");
            urls.Add("http://www1.nrk.no/nett-tv/ml/topp12.aspx?dager=31&_=", "../../stubfiler/topp12lastmonth.html");
            urls.Add("http://www1.nrk.no/nett-tv/bokstav/@", "../../stubfiler/allprograms.html");
            urls.Add("http://www1.nrk.no/nett-tv/direkte/", "../../stubfiler/topptabdirekte.html");
            urls.Add("http://www1.nrk.no/nett-tv/valg/", "../../stubfiler/valgside.html");
            urls.Add("http://www1.nrk.no/nett-tv/tema/2", "../../stubfiler/tema2.html");
            urls.Add("http://www1.nrk.no/nett-tv/DynamiskLaster.aspx?SearchResultList$search:Norge|sort:dato|page:1", "../../stubfiler/sokNorgeSide1.html");
            urls.Add("http://www1.nrk.no/nett-tv/DynamiskLaster.aspx?SearchResultList$search:Norge|sort:dato|page:2", "../../stubfiler/sokNorgeSide2.html");
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
