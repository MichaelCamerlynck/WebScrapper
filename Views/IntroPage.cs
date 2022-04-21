using System;
using System.Collections.Generic;
using System.Text;

namespace WebScapper.Views
{
    class IntroPage
    {
        public static void Print()
        {
            Console.Clear();
            Art.Print();
            Console.WriteLine("=======================");
            Console.WriteLine("1. YouTube Scraping");
            Console.WriteLine("2. Job Scrapping");
            Console.WriteLine("3. Flight Price Scrapping");
            Console.WriteLine("Any other key to exit");
            Console.WriteLine("=======================");
            Console.Write("Please pick an option: ");
        }
    }
}
