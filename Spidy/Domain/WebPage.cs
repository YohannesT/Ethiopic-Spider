using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Spider.Data;
using Spider.Utility;

namespace Spider.Domain
{
    public class WebPage
    {
        private readonly DataContext _dataContext;

        private readonly HtmlDocument _htmlDocument;
        private List<Uri> _intraDomainlinks, _allLinks;
        private string _htmlText, _innerText;

        public int WebPageID { get; set; }

        public HtmlDocument HtmlDocument { get; set; }
        public Website Website { get; set; }
        
        public Uri Uri { get; set; }
        public WebPage ParentPage { get; set; }

        public DateTime Date { get; set; }

        public WebPage(Website website, HtmlDocument htmlDocument, Uri uri, WebPage parent = null)
        {
            _dataContext = new DataContext();
            _htmlDocument = htmlDocument;
            Website = website;
            Uri = uri;
            ParentPage = parent;
        }

        public string InnerText
        {
            get { return String.IsNullOrEmpty(_innerText) ? _innerText = GetInnerText() : _innerText; }
        }

        public string HtmlText
        {
            get
            {
                return String.IsNullOrEmpty(_htmlText) ? _htmlText = _htmlDocument.DocumentNode.OuterHtml : _htmlText;
            }
        }

        public List<Uri> AllLinks
        {
            get { return _allLinks ?? (_allLinks = GetChildUris()); }
        }

        public List<Uri> IntraDomainLinks
        {
            get { return _intraDomainlinks ?? (_intraDomainlinks = GetChildUris().Where(c => c.Authority == Uri.Authority).ToList()); }
        } 

        private string GetInnerText()
        {
            var documentText = Regex.Replace(_htmlDocument.DocumentNode.InnerText, @"<[^>]+>|&nbsp;", "").Trim();
            return Regex.Replace(documentText, @"\s{2,}", " ");
        }

        private List<Uri> GetChildUris()
        {
            var links =
              _htmlDocument.DocumentNode.SelectNodes("//a[@href]")
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

        public bool Save()
        {
            try
            {
                var webpage = new Data.Models.WebPage
                {
                    Url = Uri.GetUnicodeAbsoluteUri()
                    ,
                    HtmlContent = HtmlText
                    ,
                    TextContent = InnerText
                    ,
                    WebsiteID = Website.WebsiteID
                    ,
                    Date = DateTime.Now
                    ,
                    NavigatedFromWebPageID = ParentPage != null ? ParentPage.WebPageID : new int?()
                };

                _dataContext.WebPages.Add(webpage);

                WebPageID = webpage.WebPageID;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
