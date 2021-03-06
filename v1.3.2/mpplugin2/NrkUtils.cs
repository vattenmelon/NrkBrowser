using System;
using MediaPortal.GUI.Library;
/*
 * Created by: Vattenmelon 
 * Created: 15. november 2008
 */
namespace NrkBrowser
{
   public class NrkUtils
    {
        /// <summary>
        /// Metode som gj�r om string p� formen 00:27:38 (hh:mm:ss) til double
        /// </summary>
        /// <param name="time">String p� formen hh:mm:ss</param>
        /// <returns></returns>
        public static double convertToDouble(string time)
        {
            Log.Debug("convertTouDouble(String): " + time);
            String[] array = time.Split(':');
            double hours = Double.Parse(array[0]);
            double minutes = Double.Parse(array[1]);
            double seconds = Double.Parse(array[2]);
            double totalSeconds = seconds + minutes * 60 + hours * 60 * 60;
            Log.Debug("convertTouDouble(String): returns: " + totalSeconds + " seconds");
            return totalSeconds;
        }

       public static string parseKlokkeSlettFraBilde(string bildeUrl)
       {
           try
           {
               string temp;

               temp = bildeUrl.Substring(bildeUrl.LastIndexOf("/nsps_upload") + 13);
               string[] tab = temp.Split('_');
               string time = tab[3];
               if (time.Length == 1)
               {
                   time = "0" + time;
               }
               string minutt = tab[4];
               if (minutt.Length == 1)
               {
                   minutt = "0" + minutt;
               }
               return time + ":" + minutt + " " + tab[2] + "/" + tab[1] + "-" + tab[0];
           }
           catch (Exception)
           {
               Log.Info(NrkConstants.PLUGIN_NAME +
                        ": Could not parse date from image filename, but that is fine...just returning a blank string");
               return "";
           }
       }
    }
}
