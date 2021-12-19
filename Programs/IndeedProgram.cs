using System;
using System.Collections.Generic;
using System.Text;
using WebScapper.Sites.indeed;
using WebScapper.Views;

namespace WebScapper.Programs
{
    class IndeedProgram
    {
        public static void Run()
        {
            Art.Print();
            // get keyword and location
            Console.Write("Please select a keyword: ");
            string job = Console.ReadLine();
            Console.Write("Please select a Location(Can be left empty): ");
            string location = Console.ReadLine();
            IndeedJobs search = new IndeedJobs(job, location);

            // print results
            search.PrintResults();
            Console.Write("Do you want a CSV file?(y/n)");
            char csv = 'n';
            try
            {
                csv = Convert.ToChar(Console.ReadLine().Substring(0, 1));
            }
            catch { }
            if (csv == 'y')
            {
                search.MakeCSV();
            }
        }
    }
}
