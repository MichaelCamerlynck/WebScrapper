using System;
using System.Collections.Generic;
using System.Text;

namespace WebScapper.Views
{
    class BookingModePick
    {
        public static void Print()
        {
            Console.Clear();
            Art.Print();
            Console.WriteLine("===============================================================");
            Console.WriteLine("Pick A Mode\n");
            Console.WriteLine("1. Get all prices for a one date");
            Console.WriteLine("2. Get cheapest price of tickets from a certain date to another");
            Console.WriteLine("3. Return to previous Menu");
            Console.WriteLine("===============================================================");
            Console.Write("Please pick an option: ");
        }
    }
}
