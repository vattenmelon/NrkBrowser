using System;
using System.Collections.Generic;
using System.Text;
using MediaPortal.GUI.Library;

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
    }
}
