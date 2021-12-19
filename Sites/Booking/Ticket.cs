using System;
using System.Collections.Generic;
using System.Text;
using WebScapper.Sites.Booking;

namespace WebScapper.Sites.Booking
{
    class Ticket
    {
        public string Price { get; set; }
        public List<Flight> Flights = new List<Flight>();

        public void AddFlight(Flight flight)
        {
            Flights.Add(flight);
        }

        public List<Flight> ReturnCSV()
        {
            var records = new List<Flight>();
            foreach (var flight in Flights)
            {
                records.Add(flight);
            }
            return records;
        }
    }
}
