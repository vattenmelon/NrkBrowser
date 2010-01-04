using System;
using System.Collections.Generic;
using System.Text;
using Vattenmelon.Nrk.Domain;

namespace NrkBrowser.Domain
{
    public class PodKast : Item
    {
        public PodKast(string id, string title) : base(id, title)
        {
        }
    }
}
