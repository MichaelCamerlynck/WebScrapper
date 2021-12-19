using System;
using System.Collections.Generic;
using System.Text;
using WebScapper.Sites;
using WebScapper.Views;

namespace WebScapper.Programs
{
    class YouTubeProgram
    {
        public static void Run()
        {
            int amount;
            Art.Print();
            // get search term
            Console.Write("Please Select a Search Term: ");
            YouTube search = new YouTube(Console.ReadLine());

            // get amount of result wanted
            Console.Write("How many searches do you want?(default 5, max 20): ");
            bool isParsable = Int32.TryParse(Console.ReadLine(), out amount);
            search.Amount = isParsable && amount <= 20 ? amount : 5;

            // print results and gets csv
            search.PrintResults();
            Console.Write("Do you want a CSV file?(y/n)");
            char csv = 'n';
            try
            {
                csv = Convert.ToChar(Console.ReadLine().Substring(0, 1));
            }
            catch {}
            if (csv == 'y')
            {
                search.MakeCSV();
            }
        }
    }
}
