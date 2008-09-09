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

            List<Item> clips = nrk.GetSearchHits("norge", 1);
            Console.WriteLine("Antall: " + clips.Count);
            foreach (Item clip in clips)
            {
                Console.WriteLine("------------------------");
//                Clip c = (Clip) clip;
//                Console.WriteLine("id: " + c.ID + ", title: " + c.Title + ", description: " + c.Description +
//                                  ", bilde: " + c.Bilde);
//                Console.WriteLine("Klokkeslett: " + c.Klokkeslett);
//                Console.WriteLine("videoUrl: " + nrk.GetClipUrl(c));
                if(clip is Clip)
                {
                    Clip c = (Clip) clip;
                    if (c.Type == Clip.KlippType.INDEX)
                    {
                        Console.WriteLine(c.StartTime);
                        Console.WriteLine(nrk.GetClipUrl(c));
                        Console.WriteLine(c.StartTime);
                        
                    }
                }
                if (clip is Folder)
                {
                    Folder c = (Folder)clip;
                   // Console.WriteLine(nrk.GetFolders(c));
                }
                if (clip is Program)
                {
                    Program c = (Program)clip;
                    //Console.WriteLine(c);
                   
                }
                Console.WriteLine("------------------------");
            }
            
            //Console.WriteLine(nrk.GetClipUrl((Clip) clips[0]));
            Console.WriteLine("Press enter to quit");
            Console.Read();
        }
    }
}