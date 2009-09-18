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
        /// Metode som gjør om string på formen 00:27:38 (hh:mm:ss) til double
        /// </summary>
        /// <param name="time">String på formen hh:mm:ss</param>
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
    }
}
