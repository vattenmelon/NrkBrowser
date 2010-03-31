using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Vattenmelon.Nrk.Parser.Http;

namespace Vattenmelon.Nrk.Parser
{
    public class StubHttpClient : IHttpClient
    {
        private Dictionary<String, String> getUrls;
        private Dictionary<String, String> postUrls;

        public StubHttpClient()
        {
            registerUrls();
        }

        public string GetUrl(string url)
        {
            Console.WriteLine("StubHttpClient--->GetUrl: " + url);
            return getContent(getUrls[url]);
        }

        private string getContent(string fileName)
        {
            if (fileName == null)
            {
                throw new Exception("Finnes ikke denne stubben");
            }
            Console.WriteLine("                 -----> " +fileName);
            return readFile(fileName);
        }

        private void registerUrls()
        {   getUrls = new Dictionary<string, string>();
            postUrls = new Dictionary<string, string>();
            getUrls.Add("http://www1.nrk.no/nett-tv/", "../../stubfiler/allcategories.html");
            getUrls.Add("http://www1.nrk.no/nett-tv/ml/topp12.aspx?dager=31&_=", "../../stubfiler/topp12lastmonth.html");
            getUrls.Add("http://www1.nrk.no/nett-tv/bokstav/@", "../../stubfiler/allprograms.html");
            getUrls.Add("http://www1.nrk.no/nett-tv/direkte/", "../../stubfiler/topptabdirekte.html");
            getUrls.Add("http://www1.nrk.no/nett-tv/valg/", "../../stubfiler/valgside.html");
            getUrls.Add("http://www1.nrk.no/nett-tv/tema/2", "../../stubfiler/tema2.html");
            getUrls.Add("http://www1.nrk.no/nett-tv/DynamiskLaster.aspx?SearchResultList$search:Norge|sort:dato|page:1", "../../stubfiler/sokNorgeSide1.html");
            getUrls.Add("http://www1.nrk.no/nett-tv/DynamiskLaster.aspx?SearchResultList$search:Norge|sort:dato|page:2", "../../stubfiler/sokNorgeSide2.html");
            getUrls.Add(string.Format("{0}{1}",NrkParserConstants.NRK_BETA_FEEDS_KATEGORI_URL,NrkParserConstants.NRK_BETA_SECTION_TV_SERIES), "../../stubfiler/nrkbeta_tv-serier.xml");
            getUrls.Add("http://www.nrk.no/podkast/", "../../stubfiler/podkaster_nrk.htm");
        }

        public string PostUrl(string url, string postData)
        {
            Console.WriteLine("StubHttpClient--->PostUrl: " + url);
            return getContent(postUrls[url]);
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
