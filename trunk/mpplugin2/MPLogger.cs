using System;
using MediaPortal.GUI.Library;
using Vattenmelon.Nrk.Domain;

namespace Vattenmelon.Nrk.Browser
{
    public class MPLogger : ILog
    {
        public void BackupLogFiles()
        {
        }
        public void Debug(string format, params object[] arg)
        {
            Log.Debug(format, arg);
        }
        //void Debug(LogType type, string format, params object[] arg);
        public void Error(Exception ex)
        {
            Log.Error(ex);
        }
        public void Error(string format, params object[] arg)
        {
            Log.Error(format, arg);
        }
        //void Error(LogType type, string format, params object[] arg);
        //[Obsolete("This method will disappear because the thread information is always logged now.", true)]
        public void ErrorThread(string format, params object[] arg)
        {
            //Log.ErrorThread(format, arg);
        }
        public void Info(string format, params object[] arg)
        {
            Log.Info(format, arg);
        }
        //void Info(LogType type, string format, params object[] arg);
       // [Obsolete("This method will disappear because the thread information is always logged now.", true)]
        public void InfoThread(string format, params object[] arg)
        {
            //Log.InfoThread(format, arg);
        }
        //public void SetConfigurationMode();
        //public void SetLogLevel(Level logLevel);
        public void Warn(string format, params object[] arg)
        {
            Log.Warn(format, arg);
        }
    }
}
