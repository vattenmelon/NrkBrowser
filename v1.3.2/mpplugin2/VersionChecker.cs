using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using MediaPortal.GUI.Library;


namespace NrkBrowser
{
    public static class VersionChecker
    {
        public static bool newVersionAvailable(ref String nyVer)
        {
            Log.Info("Checking for new version of plugin");
            String availableVersion = GetNewestAvailableVersion();
            nyVer = availableVersion;
            String thisVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            String[] splittedAvailable = availableVersion.Split('.');
            String[] splittedThis = thisVersion.Split('.');
            bool returnValue = false;
            if (Int32.Parse(splittedAvailable[0]) > Int32.Parse(splittedThis[0]))
            {
                returnValue = true;
            }
            else if (Int32.Parse(splittedAvailable[1]) > Int32.Parse(splittedThis[1]))
            {
                returnValue = true;
            }
            else if (Int32.Parse(splittedAvailable[2]) > Int32.Parse(splittedThis[2]))
            {
                returnValue = true;
            }
            return returnValue;
        }

        public static string GetNewestAvailableVersion()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(trustAllCertificates);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://forum.team-mediaportal.com/mediaportal-plugins-47/nrk-browser-vattenmelon-edition-45089/");
            request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.0)";
            // Set some reasonable limits on resources used by this request
            request.MaximumAutomaticRedirections = 4;
            request.MaximumResponseHeadersLength = 4;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            // Get the stream associated with the response.
            System.IO.Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            string ret = readStream.ReadToEnd();
            response.Close();
            readStream.Close();

            ret = GetVersionFromHtml(ret);
            Log.Info("Newest version of plugin is: " + ret);
            
            return ret;
        }
        

        private static string GetVersionFromHtml(string ret)
        {
            Regex query = new Regex("###(.*?)###");
            MatchCollection result = query.Matches(ret);
            foreach (Match x in result)
            {
                ret = x.Groups[1].Value;
            }
            return ret;
        }

        private static bool trustAllCertificates(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
