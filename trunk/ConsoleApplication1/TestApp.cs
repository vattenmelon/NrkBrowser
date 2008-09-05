using System;
using System.Collections.Generic;
using System.Text;
using Nrk;

namespace ConsoleApplication1
{
    class TestApp
    {
        static void Main(string[] args)
        {
            NrkParser nrk = new NrkParser(900);

            List<Item> clips = nrk.GetTopTabRSS("natur");
            System.Console.WriteLine("Antall: " + clips.Count);
            foreach (Item clip in clips)
            {
                Clip c = (Clip) clip;
                System.Console.WriteLine("id: " + c.ID + ", title: " + c.Title + ", description: " + c.Description + ", bilde: " + c.Bilde);
                System.Console.WriteLine("Klokkeslett: "+ c.Klokkeslett);
                System.Console.WriteLine("videoUrl: " + nrk.GetClipUrl(c));
                
               
            }
            System.Console.WriteLine(nrk.GetClipUrl((Clip)clips[0]));
            System.Console.WriteLine("Press enter to quit");
            System.Console.Read();
        }
    }
}
