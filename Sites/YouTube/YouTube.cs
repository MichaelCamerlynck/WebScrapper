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

namespace WebScapper.Sites
{
    class YouTube
    {
        public string SearchTerm { get; }
        private string OriTerm;
        private Dictionary<int, Video> Results;
        public int Amount { get;  set; }

        public YouTube(string searchTerm)
        {
            OriTerm = searchTerm;
            foreach (char letter in searchTerm)
            {
                SearchTerm += letter != ' ' ? letter : '+';
            }
            Amount = 5;
        }

        private void GetResult()
        {
            Results = new Dictionary<int, Video>();
            string url = "https://www.youtube.com/results?search_query=" + SearchTerm + "&sp=CAISAhAB";

            // create driver
            ChromeOptions options = new ChromeOptions();
            var chromeDriverService = ChromeDriverService.CreateDefaultService(".\\");
            chromeDriverService.HideCommandPromptWindow = true;
            chromeDriverService.SuppressInitialDiagnosticInformation = true;
            options.AddArgument("headless");
            options.AddArgument("--silent");
            options.AddArgument("log-level=3");
            IWebDriver driver = new ChromeDriver(chromeDriverService, options);

            // navigate to URL
            driver.Navigate().GoToUrl(url);
            driver.FindElement(By.XPath("//tp-yt-paper-button[contains(@aria-label, 'Agree to')]")).Click();
            Thread.Sleep(1000);
            // puts all video in array
            var videos = driver.FindElements(By.TagName("ytd-video-renderer"));

            for (int i = 0; i < Amount; i++)
            {
                Results.Add(i, new Video());
                Results[i].Link = videos[i].FindElement(By.TagName("a")).GetAttribute("href");
                Results[i].Title = videos[i].FindElement(By.TagName("yt-formatted-string")).Text;
                Results[i].Uploader = videos[i].FindElement(By.Id("channel-info")).FindElement(By.Id("text")).FindElement(By.TagName("a")).Text;
                Results[i].Views = videos[i].FindElement(By.Id("metadata-line")).FindElement(By.TagName("span")).Text;
                Console.Write(".");
            }
            driver.Close();
        }

        public void PrintResults()
        {
            Console.Clear();
            Console.WriteLine("Looking For The " + Amount +  " Latest " + OriTerm + " Videos...");
            GetResult();

            foreach (KeyValuePair<int, Video> entry in Results)
            {
                Console.WriteLine();
                Console.WriteLine("==========Video " + (entry.Key+1) + "==========");
                Console.WriteLine("Link     : " + entry.Value.Link);
                Console.WriteLine("Title    : " + entry.Value.Title);
                Console.WriteLine("Uploader : " + entry.Value.Uploader);
                Console.WriteLine("Views    : " + entry.Value.Views);
            }
        }

        public void MakeCSV()
        {
            string parentFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(parentFolderPath,"WebScapper");
            
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            filePath = Path.Combine(filePath, String.Format("{0}.csv", OriTerm));
            var config = new CsvConfiguration(CultureInfo.CurrentCulture) { Delimiter = ";", Encoding = Encoding.UTF8 };
            using (var writer = new StreamWriter(filePath))
            using (var csv = new CsvWriter(writer, config))
            {
                var records = new List<Video>();
                foreach (KeyValuePair<int, Video> entry in Results)
                {
                    records.Add(entry.Value);
                }
                csv.WriteRecords(records);
            }
        }
    }
}
