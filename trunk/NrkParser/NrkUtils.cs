using System;

/*
 * Created by: Vattenmelon 
 * Created: 15. november 2008
 */
namespace Vattenmelon.Nrk.Parser
{
   public class NrkUtils
    {
        /// <summary>
        /// Metode som gjør om string på formen 00:27:38 (hh:mm:ss) til double
        /// </summary>
        /// <param name="time">String på formen hh:mm:ss</param>
        /// <returns></returns>
        public static double convertToDouble(string time)
        {
            String[] array = time.Split(':');
            double hours = Double.Parse(array[0]);
            double minutes = Double.Parse(array[1]);
            double seconds = Double.Parse(array[2]);
            double totalSeconds = seconds + minutes * 60 + hours * 60 * 60;
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
               //if parsing of this fails..this is fine
               return "";
           }
       }
    }
}
