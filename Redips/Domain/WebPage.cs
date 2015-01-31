using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Spider.Data;
using Spider.Data.Models;
using Spider.Services;
using Spider.Utility;

namespace Spider.Domain
{
    public class WebPage
    {
        private readonly HtmlDocument _htmlDocument;
        private List<Uri> _intraDomainlinks, _allLinks;
        private string _htmlText, _innerText;

        public WebPage(Data.Models.WebPage webPage)
        {
            WebPageID = webPage.WebPageID;
            _htmlText = webPage.HtmlContent;
            _innerText = webPage.TextContent;
            Uri = new Uri(webPage.Url);
            Date = webPage.Date;
        }

        public WebPage(Website website, Uri uri, WebPage parent = null)
        {
            Website = website;
            Uri = uri;
            ParentPage = parent;
            _htmlDocument = NetworkService.GetHtmlDocumentAsync(uri).Result;
        }

        public int WebPageID { get; set; }

        public HtmlDocument HtmlDocument { get; set; }
        public Website Website { get; set; }

        public Uri Uri { get; set; }
        public WebPage ParentPage { get; set; }

        public bool IsDataLloaded
        {
            get { return _htmlDocument != null; }
        }

        public DateTime Date { get; set; }

        public string FullUriText
        {
            get { return Uri.GetUnicodeAbsoluteUri(); }
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

        public bool IsSaved
        {
            get
            {
                using (var dc = new DataContext())
                {
                    return dc.WebPages.ToList().Any(wp => wp.Url == Uri.GetUnicodeAbsoluteUri());
                }
            }
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

                using (var dc = new DataContext())
                {
                    dc.WebPages.Add(webpage);
                    dc.SaveChanges();
                }
                WebPageID = webpage.WebPageID;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsWebPageSaved(Uri uri)
        {
            return new DataContext().WebPages.ToList().Any(wp => wp.Url == uri.GetUnicodeAbsoluteUri());
        }

        public bool SaveEthiopicContent()
        {
            try
            {
                if (!InnerText.ContainsEthipic()) return true;

                var words = InnerText.GetEthipicWords();

                var eWords = words.Select(w => new EthiopicWord
                {
                    Name = w,
                    SourceWebPageID = WebPageID,
                    Date = DateTime.Now
                });
                using (var dc = new DataContext())
                {
                    dc.EthiopicWords.AddRange(eWords);
                    dc.SaveChanges();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SaveEthiopicContentAsync()
        {
            try
            {
                if (!InnerText.ContainsEthipic()) return true;

                var words = await InnerText.GetEthipicWordsAsync();

                var eWords = words.Select(w => new EthiopicWord
                {
                    Name = w,
                    SourceWebPageID = WebPageID,
                    Date = DateTime.Now
                });
                using (var dc = new DataContext())
                {
                    dc.EthiopicWords.AddRange(eWords);
                    dc.SaveChanges();       
                }
             
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string GetInnerText()
        {
            var documentText = Regex.Replace(_htmlDocument.DocumentNode.InnerText, @"<[^>]+>|&nbsp;", "").Trim();
            return Regex.Replace(documentText, @"\s{2,}", " ");
        }

        private List<Uri> GetChildUris()
        {
            IEnumerable<string> links;
            try
            {
                 links =
                    _htmlDocument.DocumentNode.SelectNodes("//a[@href]")
                        .Select(a => a.Attributes["href"])
                        .Select(a => a.Value).Distinct();
            }
            catch (Exception)
            {
                return new List<Uri>();
            }
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
