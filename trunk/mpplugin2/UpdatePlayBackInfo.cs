/*
 * Created by: Vattenmelon
 * Created: 4. november 2009
 */

using System;
using System.Threading;
using MediaPortal.GUI.Library;
using Vattenmelon.Nrk.Domain;

namespace Vattenmelon.Nrk.Parser
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
            Thread.Sleep(sleepDurationInMilliSecs);
            Item item = (Item)parametere[1];
            GUIPropertyManager.SetProperty("#Play.Current.Title", item.Title);
            GUIPropertyManager.SetProperty("#Play.Current.Plot", item.Description);
            string bildeUrl = GetBildeUrl(item);
            GUIPropertyManager.SetProperty("#Play.Current.Thumb", bildeUrl);
            finished = true;
        }

        private static string GetBildeUrl(Item item)
        {
            String bildeUrl;
            if (item.Bilde.Equals(String.Empty))
            {
                bildeUrl = NrkConstants.DEFAULT_PICTURE;
            }
            else
            {
                bildeUrl = item.Bilde;
            }
            return bildeUrl;
        }
    }
}