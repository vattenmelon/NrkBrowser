using System;
using System.Collections.Generic;
using Vattenmelon.Nrk.Parser;
using Vattenmelon.Nrk.Domain;

namespace Vattenmelon.Nrk.Parser
{
    public class NrkBrowser
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                printUsageAndExit();
            }
            else if (args[0] == "-categories")
            {
                printCategoriesAndExit();
            }
            else if (args[0] == "-recommended")
            {
                printRecommendedAndExit();
            }
            else if (args[0] == "-mostwatched")
            {
                printMostWatchedAndExit(args[1]);
            }
            else if (args[0] == "-version")
            {
                printVersionAndExit();
            }
            
            Console.WriteLine("Press enter to quit");
            Console.Read();
        }

        private static void printVersionAndExit()
        {
            Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            Console.WriteLine("nrkweb: " + v);
            v = System.Reflection.Assembly.GetAssembly(typeof(NrkParser)).GetName().Version;
            Console.WriteLine("nrkparser: " + v);
            v = System.Reflection.Assembly.GetAssembly(typeof (Item)).GetName().Version;
            Console.WriteLine("nrkparser: " + v);
        }

        private static void printMostWatchedAndExit(string days)
        {
            int dager = Int32.Parse(days);
            NrkParser parser = new NrkParser(2000, new NullLogger());
            
            List<Item> anbefalte = parser.GetMestSette(dager);
            Console.WriteLine("Most watched last " + days + " days");
            foreach (Item item in anbefalte)
            {
                Console.WriteLine(item.Title);
            }

        }

        private static void printRecommendedAndExit()
        {
            NrkParser parser = new NrkParser(2000, new NullLogger());
            List<Item> anbefalte = parser.GetAnbefaltePaaForsiden();
            Console.WriteLine("Recommended:");
            foreach (Item item in anbefalte)
            {
                Console.WriteLine(item.Title);
            }
        }

        private static void printCategoriesAndExit()
        {
            NrkParser parser = new NrkParser(2000, new NullLogger());
            List<Item> kategorier = parser.GetCategories();
            Console.WriteLine("Categories:");
            foreach(Item item in kategorier)
            {
                Category cat = (Category)item;
                Console.WriteLine(cat.Title);
            }
        }

        private static void printUsageAndExit()
        {
            Console.WriteLine("NRK Browser");
            Console.WriteLine("Usage: ");
            Console.WriteLine("-categories: print categories.");
            Console.WriteLine("-recommended: print recommended on the front page.");
            Console.WriteLine("-mostwatched x: print most watched last x days.");
            Console.WriteLine("-version: print version number.");
        }
    }
}