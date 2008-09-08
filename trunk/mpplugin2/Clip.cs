/*
 * Created by: 
 * Created: 5. september 2008
 */

using System;

namespace NrkBrowser
{
    public class Clip : Item
    {
        public enum KlippType
        {
            KLIPP = 0,
            VERDI = 1,
            RSS = 2,
            INDEX = 3,
        }

        private string verdiLink;
        private string antallGangerVist;
        private string klokkeslett;
        private Double startTime;
        // sier noe om hvilken m�te man finner klipplinken
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

        public Double StartTime
        {
            get { return startTime; }
            set { this.startTime = value; }
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