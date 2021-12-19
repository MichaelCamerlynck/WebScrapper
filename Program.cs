using System;
using WebScapper.Views;
using WebScapper.Sites;
using WebScapper.Sites.indeed;
using WebScapper.Sites.Booking;
using WebScapper.Programs;

namespace WebScapper
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                //try
                //{
                    IntroPage.Print();
                    char pickedOption = Convert.ToChar(Console.ReadLine().Substring(0, 1));

                    if (pickedOption == '1')
                    {
                        Console.Clear();
                        YouTubeProgram.Run();
                    }
                    else if (pickedOption == '2')
                    {
                        Console.Clear();
                        IndeedProgram.Run();
                    }
                    else if (pickedOption == '3')
                    {
                        Console.Clear();
                        BookingProgram.Run();
                    }
                    else
                    {
                        Console.WriteLine("Exiting Program...");
                        Environment.Exit(0);
                    }
                    Console.Write("\nPress enter to continue...");
                    Console.ReadLine();
                //}
                //catch
                //{
                //    Console.WriteLine("Error Occured, press enter to return to menu...");
                //    Console.ReadLine();
                //}
                

            } while (true);
        }
    }
}
