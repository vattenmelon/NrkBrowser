/*
 * Created by: 
 * Created: 5. september 2008
 */

namespace NrkBrowser
{
    public class Clip : Item
    {
        public enum KlippType
        {
            KLIPP = 0,
            VERDI = 1,
            RSS = 2,
        }

        private string verdiLink;
        private string antallGangerVist;
        private string klokkeslett;
        // sier noe om hvilken måte man finner klipplinken
        private KlippType type = KlippType.KLIPP;

        public Clip(string id, string title)
            : base(id, title)
        {
        }

        public override bool Playable
        {
            get { return true; }
        }

        public string AntallGangerVist
        {
            get { return antallGangerVist; }
            set { this.antallGangerVist = value; }
        }

        public string VerdiLink
        {
            get { return verdiLink; }
            set { this.verdiLink = value; }
        }

        public string Klokkeslett
        {
            get { return klokkeslett; }
            set { this.klokkeslett = value; }
        }

        public KlippType Type
        {
            get { return type; }
            set { this.type = value; }
        }
    }
}