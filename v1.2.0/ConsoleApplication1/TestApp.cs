using System;
using System.Collections.Generic;
using NrkBrowser;

namespace ConsoleApplication1
{
    internal class TestApp
    {
        private static void Main(string[] args)
        {
            NrkParser nrk = new NrkParser(900);

            List<Item> clips = nrk.GetAnbefaltePaaForsiden();
            Console.WriteLine("Antall: " + clips.Count);
            foreach (Item clip in clips)
            {
                Console.WriteLine("------------------------");
                Clip c = (Clip) clip;
//                Console.WriteLine("id: " + c.ID + ", title: " + c.Title + ", description: " + c.Description +
//                                  ", bilde: " + c.Bilde);
//                Console.WriteLine("Klokkeslett: " + c.Klokkeslett);
                Console.WriteLine("videoUrl: " + nrk.GetClipUrl(c));

            }
            
            //Console.WriteLine(nrk.GetClipUrl((Clip) clips[0]));
            Console.WriteLine("Press enter to quit");
            Console.Read();
        }
    }
}