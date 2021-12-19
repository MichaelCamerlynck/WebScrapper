using System;
using System.Collections.Generic;
using System.Text;
using WebScapper.Sites.Booking;
using WebScapper.Views;

namespace WebScapper.Programs
{
    class BookingProgram
    {
        public static void Run()
        {
            Art.Print();
            BookingModePick.Print();
            char pickedOption = Convert.ToChar(Console.ReadLine().Substring(0, 1));

            if (pickedOption == '1')
            {
                Mode1();
            }
            else if (pickedOption == '2')
            {
                Mode2();
            }
        }

        private static void Mode1()
        {
            // get keyword and location
            Console.Write("Please select an origin (Use airport name such as AMS for best result): ");
            string origin = Console.ReadLine();
            Console.Write("Please select an destination (Use airport name such as AMS for best result): ");
            string destination = Console.ReadLine();
            Console.Write("Please select departure date (yyyy-mm-dd): ");
            string departureDate = Console.ReadLine();
            Console.Write("Are you looking for Round Trip or One Way? (Default is OneWay) (R/O): ");
            char pickedOption = GetOption();
            string trip = "ONEWAY";
            string returnDate = "";
            if (pickedOption == 'r')
            {
                trip = "ROUNDTRIP";
                Console.Write("Please select return date (yyyy-mm-dd): ");
                returnDate = Console.ReadLine();
            }
            Console.Write("Do you want to sort by Best, Cheapest or Fastest? (Default is Best) (B/C/F): ");
            char pickedOption2 = GetOption();
            string sort = "BEST";
            if (pickedOption2 == 'c')
            {
                sort = "CHEAPEST";
            }
            else if (pickedOption2 == 'f')
            {
                sort = "FASTEST";
            }
            Booking search = new Booking(origin, destination, trip, departureDate, returnDate, sort);
            Console.Write("How many max results do you want? (One page takes around 5 seconds) (Default is 3): ");
            int page = 3;
            bool isParsable = Int32.TryParse(Console.ReadLine(), out page);
            search.Page = page;
            // print results
            search.GetResultsMode1();
            search.PrintResults();
        }

        private static void Mode2()
        {
            // get keyword and location
            Console.Write("Please select an origin (Use airport name such as AMS for best result): ");
            string origin = Console.ReadLine();
            Console.Write("Please select an destination (Use airport name such as AMS for best result): ");
            string destination = Console.ReadLine();

            Console.Write("Please select departure date range (yyyy-mm-dd/yyyy-mm-dd): ");
            string departureDateRange = Console.ReadLine();

            Console.Write("Are you looking for Round Trip or One Way? (Default is ONEWAY) (R/O): ");
            char pickedOption = GetOption();
            string trip = "ONEWAY";

            string returnDateRange = "";
            if (pickedOption == 'r')
            {
                trip = "ROUNDTRIP";
                Console.Write("Please select return date range (yyyy-mm-dd/yyyy-mm-dd): ");
                returnDateRange = Console.ReadLine();
            }
            Console.Write("Do you want to sort by Best, Cheapest or Fastest? (Default is Best) (B/C/F): ");
            char pickedOption2 = GetOption();
            string sort = "BEST";
            if (pickedOption2 == 'c')
            {
                sort = "CHEAPEST";
            }
            else if (pickedOption2 == 'f')
            {
                sort = "FASTEST";
            }
            Booking search = new Booking(origin, destination, trip, departureDateRange, returnDateRange, sort);
            // print results
            search.GetResultsMode2();
            search.PrintResults();
        }

        private static char GetOption()
        {
            try
            {
                char pickedOption = Convert.ToChar(Console.ReadLine().Substring(0, 1).ToLower());
                return pickedOption;
            }
            catch
            {
                return ' ';
            }
        }
    }
}
