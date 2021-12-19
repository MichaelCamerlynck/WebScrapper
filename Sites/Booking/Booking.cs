using CsvHelper;
using CsvHelper.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace WebScapper.Sites.Booking
{
    class Booking
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string Trip { get; set; }
        public string DepartDate { get; set; }
        public string ReturnDate { get; set; }
        public string Sort { get; set; }
        public int Page { get; set; }
        private Dictionary<int, Ticket> Tickets;


        public Booking(string origin, string destination, string trip, string departDate, string returnDate, string sort)
        {
            Origin = origin;
            Destination = destination;
            Trip = trip;
            DepartDate = departDate;
            ReturnDate = returnDate;
            Sort = sort;
        }
        public void GetResultsMode1()
        {
            // set driver up
            ChromeOptions options = new ChromeOptions();
            var chromeDriverService = ChromeDriverService.CreateDefaultService(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;
            options.AddArgument("headless");
            options.AddArgument("--silent");
            options.AddArgument("log-level=3");
            IWebDriver driver = new ChromeDriver(chromeDriverService,options);
            driver.Manage().Window.Size = new Size(1920, 1080);

            // go to site
            string url = String.Format("https://flights.booking.com/flights/{0}-{1}/?type={2}&adults=1&cabinClass=ECONOMY&children=&from={0}&to={1}&depart={3}&return={4}&sort={5}", Origin, Destination, Trip, DepartDate, ReturnDate, Sort);
            driver.Navigate().GoToUrl(url);
            Console.Write("Going to Booking.com");

            // wait until all data is loaded in
            while (!IsContentAvailble(driver))
            {
                Console.Write('.');
                Thread.Sleep(100);
            }
            Console.WriteLine('.');
            Thread.Sleep(1000);
            AcceptCookies(driver);
            Tickets = new Dictionary<int, Ticket>();


            int page = 1;
            int i = 0;
            bool running = true;
            while (running)
            {
                var allFlight = driver.FindElements(By.XPath("//div[contains(@id, 'flightcard')]"));
                Console.Write("Page" + page);
                foreach (var flight in allFlight)
                {
                    Tickets.Add(i, new Ticket());
                    GetTicketInfo(i, flight); 
                    i++;
                    Console.Write('.');
                }
                Console.WriteLine('.');
                page++;
                running = IsNextPresent(driver, page);
                if (running)
                {
                    var Body = driver.FindElement(By.TagName("body"));
                    for (int x = 0; x < 9; x++)
                    {
                        Body.SendKeys(Keys.PageDown);
                    }
                    driver.FindElement(By.XPath(String.Format("//div[@class='css-177s61e' and text()='{0}']", page))).Click();
                    Thread.Sleep(1000);
                }
            }
            driver.Close();
        }
        public void GetResultsMode2()
        {
            // set driver up
            ChromeOptions options = new ChromeOptions();
            var chromeDriverService = ChromeDriverService.CreateDefaultService(".\\");
            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;
            options.AddArgument("headless");
            options.AddArgument("--silent");
            options.AddArgument("log-level=3");
            IWebDriver driver = new ChromeDriver(chromeDriverService, options);
            driver.Manage().Window.Size = new Size(1920, 1080);

            Tickets = new Dictionary<int, Ticket>();
            List<String> departDates = GetDates(DepartDate);
            int i = 0;
            Console.WriteLine(String.Format("\nEstimated time: {0} seconds\n", departDates.Count * GetDates(ReturnDate).Count * 8));
            foreach (var departDate in departDates)
            {
                string url;
                if (Trip != "ONEWAY")
                {
                    List<String> returnDates = GetDates(ReturnDate);
                    foreach (var returnDate in returnDates)
                    {
                        Tickets.Add(i, new Ticket());
                        url = String.Format("https://flights.booking.com/flights/{0}-{1}/?type={2}&adults=1&cabinClass=ECONOMY&children=&from={0}&to={1}&depart={3}&return={4}&sort={5}", Origin, Destination, Trip, departDate, returnDate, Sort);
                        driver.Navigate().GoToUrl(url);
                        Console.Write(String.Format("Getting flight on {0} returning on {1}", departDate, returnDate));
                        while (!IsContentAvailble(driver))
                        {
                            Console.Write('.');
                            Thread.Sleep(300);
                        }
                        Console.WriteLine('.');
                        var flight = driver.FindElement(By.XPath("//div[contains(@id, 'flightcard')]"));
                        GetTicketInfo(i, flight);
                        i++;
                    }
                }
                else
                {
                    Tickets.Add(i, new Ticket());
                    url = String.Format("https://flights.booking.com/flights/{0}-{1}/?type={2}&adults=1&cabinClass=ECONOMY&children=&from={0}&to={1}&depart={3}&sort={4}", Origin, Destination, Trip, departDate, Sort);
                    driver.Navigate().GoToUrl(url);
                    Console.Write(String.Format("Getting flight on {0}", departDate));
                    while (!IsContentAvailble(driver))
                    {
                        Console.Write('.');
                        Thread.Sleep(300);
                    }
                    Console.WriteLine('.');
                    var flight = driver.FindElement(By.XPath("//div[contains(@id, 'flightcard')]"));
                    GetTicketInfo(i, flight);
                    i++;
                }
            }
            driver.Close();
        }
        public void PrintResults()
        {
            foreach (KeyValuePair<int, Ticket> entry in Tickets)
            {
                Console.WriteLine();
                Console.WriteLine("==========Ticket " + (entry.Key + 1) + "==========");
                Console.WriteLine("Price : " + entry.Value.Price);
                Console.WriteLine("Departing Flight-------------");
                Console.WriteLine("Origin : " + entry.Value.Flights[0].Origin);
                Console.WriteLine("Destination : " + entry.Value.Flights[0].Destination);
                Console.WriteLine("Departure Time : " + entry.Value.Flights[0].DepartureTime);
                Console.WriteLine("Departure Date : " + entry.Value.Flights[0].DepartureDate);
                Console.WriteLine("Arrival Time : " + entry.Value.Flights[0].ArrivalTime);
                Console.WriteLine("Arrival Date : " + entry.Value.Flights[0].ArrivalDate);
                Console.WriteLine("Flight Duration : " + entry.Value.Flights[0].Duration);
                Console.WriteLine("Number of Stops : " + entry.Value.Flights[0].Stops);
                if (Trip != "ONEWAY")
                {
                    Console.WriteLine("Return Flight----------------");
                    Console.WriteLine("Origin : " + entry.Value.Flights[1].Origin);
                    Console.WriteLine("Destination : " + entry.Value.Flights[1].Destination);
                    Console.WriteLine("Departure Time : " + entry.Value.Flights[1].DepartureTime);
                    Console.WriteLine("Departure Date : " + entry.Value.Flights[1].DepartureDate);
                    Console.WriteLine("Arrival Time : " + entry.Value.Flights[1].ArrivalTime);
                    Console.WriteLine("Arrival Date : " + entry.Value.Flights[1].ArrivalDate);
                    Console.WriteLine("Flight Duration : " + entry.Value.Flights[1].Duration);
                    Console.WriteLine("Number of Stops : " + entry.Value.Flights[1].Stops);
                }
            }
            MakeCSV();
        }
        private bool IsNextPresent(IWebDriver driver, int page)
        {
            if (page > Page)
            {
                return false;
            }
            try
            {

                driver.FindElement(By.XPath(String.Format("//div[@class='css-177s61e' and text()='{0}']", page)));
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        private static bool IsContentAvailble(IWebDriver driver)
        {
            try
            {
                driver.FindElement(By.XPath("//div[contains(@id, 'flightcard')]"));
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
        private static void AcceptCookies(IWebDriver driver)
        {
            try
            {
                driver.FindElement(By.XPath("//button[@id='onetrust-accept-btn-handler']")).Click();
            }
            catch (NoSuchElementException) { }
        }
        public void MakeCSV()
        {
            string parentFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(parentFolderPath, "WebScapper");

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            try
            {
                DepartDate = String.Format("{0} to {1}", DepartDate.Split('/'));
                ReturnDate = String.Format("{0} to {1}", ReturnDate.Split('/'));
            }
            catch { }
            if (Trip != "ONEWAY")
            {
                filePath = Path.Combine(filePath, String.Format("{0}-{1} {2} {3} {4}.csv", Origin, Destination, DepartDate, ReturnDate, Sort));
            }
            else
            {
                filePath = Path.Combine(filePath, String.Format("{0}-{1} {2} {3}.csv", Origin, Destination, DepartDate, Sort));
            }
            
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                string title = "price;Origin;Destination;Departure Time;Departure Date;Arrival Time;Arrival Date;Flight Duration;Number of Stops;Airline";
                if (Trip != "ONEWAY")
                {
                    title += ";Return Origin;Return Destination;Return Departure Time;Return Departure Date;Return Arrival Time;Return Arrival Date;Return Flight Duration;Return Number of Stops;Return Airline";
                }
                writer.WriteLine(title);

                foreach (KeyValuePair<int, Ticket> entry in Tickets)
                {
                    string line = entry.Value.Price;
                    foreach(var flight in entry.Value.Flights)
                    {
                        line += String.Format(";{0};{1};{2};{3};{4};{5};{6};{7};{8}", flight.Origin, flight.Destination, flight.DepartureTime, flight.DepartureDate, flight.ArrivalTime, flight.ArrivalDate, flight.Duration, flight.Stops, flight.Airline);
                    }
                    writer.WriteLine(line);
                }
            }
            Console.WriteLine("\n" + filePath + " created");
        }
        private static List<String> GetDates(String dateRange)
        {
            var dates = new List<String>();
            try
            {
                DateTime start = DateTime.Parse(dateRange.Split('/')[0]);
                DateTime end = DateTime.Parse(dateRange.Split('/')[1]);

                for (var dt = start; dt <= end; dt = dt.AddDays(1))
                {
                    dates.Add(dt.ToString("yyyy-MM-dd"));
                }
            }
            catch { }
            return dates;
        }
        private void GetTicketInfo(int i, IWebElement data)
        {
            Tickets[i].Price = (data.FindElement(By.ClassName("css-1ltp57x")).FindElement(By.XPath("./div[2]/div/div/div")).Text);

            // departure flight
            Flight departFlight = new Flight();
            // airline
            var airlines = data.FindElement(By.ClassName("css-1dimx8f")).FindElement(By.XPath("./div")).FindElements(By.ClassName("css-17m9lv6"));
            foreach (var airline in airlines) { departFlight.AddAirline(airline.FindElement(By.XPath("./div")).Text); }

            // departure
            var ancor = data.FindElement(By.ClassName("css-1niqckn"));
            departFlight.DepartureTime = ancor.FindElement(By.XPath("./div/div/div/div")).Text;
            departFlight.Origin = ancor.FindElement(By.XPath("./div/div/div/div[2]/div[1]")).Text;
            departFlight.DepartureDate = ancor.FindElement(By.XPath("./div/div/div/div[2]/div[3]")).Text;
            departFlight.ArrivalTime = ancor.FindElement(By.XPath("./div/div/div[3]/div")).Text;
            departFlight.Destination = ancor.FindElement(By.XPath("./div/div/div[3]/div[2]/div[1]")).Text;
            departFlight.ArrivalDate = ancor.FindElement(By.XPath("./div/div/div[3]/div[2]/div[3]")).Text;
            departFlight.Duration = data.FindElement(By.ClassName("css-1wnqz2m")).FindElement(By.XPath("./div")).Text;
            departFlight.Stops = data.FindElement(By.ClassName("css-1wnqz2m")).FindElement(By.XPath("./div[3]")).Text;

            Tickets[i].AddFlight(departFlight);
            if (Trip != "ONEWAY")
            {
                ancor = data.FindElement(By.ClassName("css-4o3ibe"));
                // return flight
                Flight returnFlight = new Flight();
                // airlines
                var returnAirlines = ancor.FindElement(By.XPath("./div/div[6]/div")).FindElements(By.ClassName("css-17m9lv6"));
                foreach (var airline in returnAirlines) { returnFlight.AddAirline(airline.FindElement(By.XPath("./div")).Text); }

                returnFlight.DepartureTime = ancor.FindElement(By.XPath("./div/div[4]/div[2]/div/div/div/div")).Text;
                returnFlight.Origin = ancor.FindElement(By.XPath("./div/div[4]/div[2]/div/div/div/div[2]/div[1]")).Text;
                returnFlight.DepartureDate = ancor.FindElement(By.XPath("./div/div[4]/div[2]/div/div/div/div[2]/div[3]")).Text;
                returnFlight.ArrivalTime = ancor.FindElement(By.XPath("./div/div[4]/div[2]/div/div/div[3]/div")).Text;
                returnFlight.Destination = ancor.FindElement(By.XPath("./div/div[4]/div[2]/div/div/div[3]/div[2]/div[1]")).Text;
                returnFlight.ArrivalDate = ancor.FindElement(By.XPath("./div/div[4]/div[2]/div/div/div[3]/div[2]/div[3]")).Text;
                returnFlight.Duration = ancor.FindElement(By.XPath("./div/div[4]/div[2]/div/div/div[2]/div")).Text;
                returnFlight.Stops = ancor.FindElement(By.XPath("./div/div[4]/div[2]/div/div/div[2]/div[3]")).Text;

                Tickets[i].AddFlight(returnFlight);
            }
        }
    }

}
