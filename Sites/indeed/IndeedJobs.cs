using CsvHelper;
using CsvHelper.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace WebScapper.Sites.indeed
{
    class IndeedJobs
    {
        public string JobSearchTerm { get; set; }
        public string LocationSearchTerm { get; set; }
        private Dictionary<int, Job> Results;

        public IndeedJobs(string Job, string Location)
        {
            JobSearchTerm = Job;
            LocationSearchTerm = Location;
        }

        private void GetResult()
        {
            Results = new Dictionary<int, Job>();
            string url = String.Format("https://be.indeed.com/jobs?q={0}&l={1}&fromage=3&sort=date", JobSearchTerm, LocationSearchTerm);

            // create driver
            ChromeOptions options = new ChromeOptions();
            var chromeDriverService = ChromeDriverService.CreateDefaultService(".\\");
            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;
            options.AddArgument("headless");
            options.AddArgument("--silent");
            options.AddArgument("log-level=3");
            IWebDriver driver = new ChromeDriver(chromeDriverService, options);

            driver.Navigate().GoToUrl(url);
            HandlePopUp(driver);
            Thread.Sleep(2000);
            bool running = true;
            int i = 0;
            while (running)
            {
                var jobListings = driver.FindElements(By.XPath("//a[@data-hiring-event]"));
                foreach (var job in jobListings)
                {
                    Results.Add(i, new Job());
                    Results[i].Link = job.GetAttribute("href");
                    Results[i].Title = job.FindElement(By.ClassName("jobTitle-newJob")).FindElement(By.XPath("./span")).Text;
                    Results[i].Company = job.FindElement(By.ClassName("companyName")).Text;
                    Results[i].Location = job.FindElement(By.ClassName("companyLocation")).Text;
                    i++;
                    Console.Write('.');
                }
                running = IsNextPresent(driver);
                Thread.Sleep(1000);
                if (running)
                {
                    driver.Navigate().GoToUrl(driver.FindElement(By.XPath("//a[@aria-label='Next']")).GetAttribute("href"));
                    Thread.Sleep(1000);
                    HandlePopUp(driver);
                }
            }
            //close the browser  
            driver.Close();
        }

        static bool IsNextPresent(IWebDriver driver)
        {
            try
            {
                driver.FindElement(By.XPath("//a[@aria-label='Next']"));
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        static void HandlePopUp(IWebDriver driver)
        {
            try
            {
                driver.FindElement(By.XPath("//div[@id='popover-foreground']/div/button[@aria-label='Close']")).Click();
            }
            catch (NoSuchElementException) { }
        }

        public void PrintResults()
        {
            Console.Clear();
            Console.WriteLine("Looking for jobs posted in the last three days");
            GetResult();

            foreach (KeyValuePair<int, Job> entry in Results)
            {
                Console.WriteLine();
                Console.WriteLine("==========Job " + (entry.Key + 1) + "==========");
                Console.WriteLine("Link     : " + entry.Value.Link);
                Console.WriteLine("Title    : " + entry.Value.Title);
                Console.WriteLine("Company  : " + entry.Value.Company);
                Console.WriteLine("Location : " + entry.Value.Location);
            }
        }

        public void MakeCSV()
        {
            string parentFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(parentFolderPath, "WebScapper");

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            filePath = Path.Combine(filePath, String.Format("{0}-{1}.csv", JobSearchTerm, LocationSearchTerm));
            var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";", Encoding = Encoding.UTF8 };
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, config))
            {
                var records = new List<Job>();
                foreach (KeyValuePair<int, Job> entry in Results)
                {
                    records.Add(entry.Value);
                }
                csv.WriteRecords(records);
            }
        }
    }
}
