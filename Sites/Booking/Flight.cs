using System;
using System.Collections.Generic;
using System.Text;

namespace WebScapper.Sites.Booking
{
    class Flight
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string DepartureTime { get; set; }
        public string DepartureDate { get; set; }
        public string ArrivalTime { get; set; }
        public string ArrivalDate { get; set; }
        public string Duration { get; set; }
        public string Stops { get; set; }
        public string Airline = "";

        public void AddAirline(string airline)
        {
            if (Airline == "")
            {
                Airline += airline;
            } 
            else
            {
                Airline += String.Format(", {0}", airline);
            }
            
        }
    }

}
