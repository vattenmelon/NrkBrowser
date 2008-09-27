/*
 * Created by: 
 * Created: 5. september 2008
 */

namespace NrkBrowser
{
    public class Stream : Item
    {
        public Stream(string id, string title)
            : base(id, title)
        {
        }

        public override bool Playable
        {
            get { return true; }
        }
    }
}