/*
 * Created by: Vattenmelon
 * Created: 4. november 2009
 */

using System;
using System.Threading;
using MediaPortal.GUI.Library;
using NrkBrowser.Domain;

namespace NrkBrowser
{
    public class UpdatePlayBackInfo
    {
        public bool finished = false;
        public UpdatePlayBackInfo(int delay, Item item)
        {
            StartUpdatePlayBackInfoDelayedThread(delay, item);
        }

        private void StartUpdatePlayBackInfoDelayedThread(int delay, Item item)
        {
            Object[] paramz = new object[] { delay, item };
            Thread updatePlaybackInfoThread = new Thread(new ParameterizedThreadStart(UpdatePlaybackInfoThreadMethod));
            updatePlaybackInfoThread.Start(paramz);
        }

        /// <summary>
        /// Ugly delayed method that should be started as a separate thread to avoid that mediaportal clears
        /// the Current.Title etc information. 
        /// </summary>
        /// <param name="paramArray"></param>
        private void UpdatePlaybackInfoThreadMethod(Object paramArray)
        {
            Object[] parametere = (Object[])paramArray;
            int sleepDurationInMilliSecs = (int)parametere[0];
            Log.Debug("Thread setting osd values starting, will sleep: " + sleepDurationInMilliSecs);
            Thread.Sleep(sleepDurationInMilliSecs);
            Item item = (Item)parametere[1];
            GUIPropertyManager.SetProperty("#Play.Current.Title", item.Title);
            GUIPropertyManager.SetProperty("#Play.Current.Plot", item.Description);
            GUIPropertyManager.SetProperty("#Play.Current.Thumb", item.Bilde);
            Log.Debug("Thread setting osd values stopping, have slept: " + sleepDurationInMilliSecs);
            finished = true;
        }
    }
}