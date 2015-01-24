using System;
using System.Linq;
using System.Net;
using RobotsTxt;
using Spider.Data;
using Spider.Utility;

namespace Spider.Domain
{
    public class Website
    {
        private readonly DataContext _dataContext;

        public Website(Uri uri)
        {
            _dataContext = new DataContext();
            Uri = uri;
            IpAddresses = NetworkHelpers.GetIpAddresses(uri);
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
                    Url = Uri.GetUnicodeAbsoluteUri()
                    ,
                    RobotsTxt = Robots.Raw
                    ,
                    IpAddress = IpAddresses != null ? string.Join(";", IpAddresses.Select(p => p.ToString())) : "-"
                    ,
                    Country = Country
                    ,
                    Date = DateTime.Now
                };
                _dataContext.Websites.Add(website);
                _dataContext.SaveChanges();
                WebsiteID = website.WebsiteID;

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
