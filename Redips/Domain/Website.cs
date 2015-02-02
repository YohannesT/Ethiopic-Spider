using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Redips.Data;
using Redips.Services;
using Redips.Utility;
using RobotsTxt;

namespace Redips.Domain
{
    public class Website
    {
        public Website()
        {
            
        }

        public Website(Data.Models.Website website)
        {
            Uri = new Uri(website.Url);
            IpAddresses = website.IpAddress.Split(new [] {';'}, StringSplitOptions.RemoveEmptyEntries).Select(IPAddress.Parse).ToArray();
            Robots = new Robots(website.RobotsTxt);
            WebsiteID = website.WebsiteID;
        }

        public Website(Uri uri)
        {

            Uri = uri;
            IpAddresses = NetworkService.GetIpAddresses(uri);
            Robots = new Robots(GetRobotsTxtAsync(uri).Result);
        }

        public int WebsiteID { get; set; }
        public Uri Uri { get; set; }
        public IPAddress[] IpAddresses { get; set; }
        public Robots Robots { get; set; }
        public string Country { get; set; }
        public DateTime Date { get; set; }

        public bool Save()
        {
            try
            {
                var website = new Data.Models.Website
                {
                    Url = Uri.GetUnicodeUri()
                    ,
                    RobotsTxt = Robots.Raw
                    ,
                    IpAddress = IpAddresses != null ? string.Join(";", IpAddresses.Select(p => p.ToString())) : "-"
                    ,
                    Country = Country
                    ,
                    Date = DateTime.Now
                };
                using(var dc = new DataContext())
                {
                    dc.Websites.Add(website);
                    dc.SaveChanges();
                }

                WebsiteID = website.WebsiteID;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsWebsiteAllowed(Uri uri)
        {
            return (Robots.IsPathAllowed(SpiderInfo.Useragent, uri.GetUnicodeAbsoluteUri()) ||
                   Robots.IsPathAllowed(SpiderInfo.Useragent, uri.AbsolutePath)) && !WebPage.IsWebPageSaved(uri);
        }

        public int Delay
        {
            get { return (int)Robots.CrawlDelay(SpiderInfo.Useragent); }
        }

        private async Task<string> GetRobotsTxtAsync(Uri uri)
        {
            Console.WriteLine("...{0} Fetching robots.txt for {1}", DateTime.Now.ToShortTimeString(), uri.Authority);
            var robotsUri = String.Format("{0}://{1}/robots.txt", uri.Scheme, uri.Authority);//
            var request = WebRequest.Create(robotsUri);
            try
            {
                var response = await request.GetResponseAsync();

                var responseStream = response.GetResponseStream();
                if (responseStream == null) return String.Empty;

                var sr = new StreamReader(responseStream);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine("......{0} Error fetching robots.txt for {1}, {2}", DateTime.Now.ToShortTimeString(), uri.Authority, ex.Message);
                return String.Empty;
            }
        }

        public static bool IsWebsiteSaved(Uri uri)
        {
            var dc = new DataContext();
            return dc.Websites.Any(c => c.Url == uri.Scheme + "://" + uri.Authority);
        }

        public static Website GetWebsite(Uri uri)
        {
            var dc = new DataContext();
            var website = dc.Websites.First(w => w.Url == uri.Scheme + "://" + uri.Authority);
            return new Website
            {
                WebsiteID = website.WebsiteID,
                Uri = new Uri(website.Url),
                IpAddresses = website.IpAddress.Split(';').Select(IPAddress.Parse).ToArray(),
                Country = website.Country,
                Robots = new Robots(website.RobotsTxt),
                Date = website.Date
            };
        }
    }
}
