using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using RobotsTxt;
using Spider.Data;

namespace Spider.Domain
{
    public class Website
    {
        private string _innerText = String.Empty;
        private List<Uri> _links;
 
        public int WebsiteID { get; set; }
        public Uri Uri { get; set; }
        public long StreamLength { get; set; }
        public long HtmlCharacterLength { get; set; }

        public Robots Robot { get; set; }

        public HtmlDocument HtmlDocument { get; set; }

        public string InnerText
        {
            get
            {
                return  String.IsNullOrEmpty(_innerText) ? _innerText = GetInnerText() : _innerText;
            }
        }

        public List<Uri> AllLinks
        {
            get { return _links ?? (_links = GetChildUris()); }
        }

        public List<Uri> IntraDomainLinks
        {
            get { return AllLinks.Where(l => l.Authority == Uri.Authority).ToList(); }
        }

        public bool Save(string ipAddresses, Uri parentUri)
        {
            try
            {
                var dataContext = new DataContext();
                var site = new WebPage
                {
                    Url = String.Format("{0}://{1}{2}", Uri.Scheme, Uri.Authority, Uri.LocalPath),
                    IpAddress = ipAddresses,
                    VisitedDate = DateTime.Now,
                    StreamLength = StreamLength,
                    HtmlCharLength = HtmlCharacterLength,
                    ParentSite = parentUri != null ? dataContext.Sites.FirstOrDefault(s => s.Url == parentUri.AbsoluteUri) : null,
                };

                dataContext.Sites.Add(site);
                dataContext.SaveChanges();
                WebsiteID = site.SiteID;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private string GetInnerText()
        {
            var documentText = Regex.Replace(HtmlDocument.DocumentNode.InnerText, @"<[^>]+>|&nbsp;", "").Trim();
            return Regex.Replace(documentText, @"\s{2,}", " ");
        }

        private List<Uri> GetChildUris()
        {
            var links =
              HtmlDocument.DocumentNode.SelectNodes("//a[@href]")
               .Select(a => a.Attributes["href"])
               .Select(a => a.Value).Distinct();
           
            var childUris = new List<Uri>();
            links = links.Distinct().Where(l => l.Length > 1 && Uri.AbsoluteUri != l && !(l.Contains("#") || l.ToLower().Contains("javascript") || l.Contains("?share=")));
            
            foreach (var link in links)
            {
                Uri uri;
                if (Uri.TryCreate(link, UriKind.Absolute, out uri))
                    childUris.Add(uri);
                else
                {
                    if (link.StartsWith("/") || link.StartsWith("\\") && link.Length > 2)
                        if (Uri.TryCreate(Uri.AbsoluteUri + link, UriKind.Absolute, out uri))
                            childUris.Add(uri);
                }
            }

            return childUris;
        }

    }
}
