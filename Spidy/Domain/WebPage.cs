using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Spider.Domain
{
    public class WebPage
    {
        public int WebPageID { get; set; }

        private HtmlDocument _htmlDocument;
        private List<Uri> _intraDomainlinks, _allLinks;
        private string _htmlText, _innerText;

        public HtmlDocument HtmlDocument { get; set; }
        public Website Website { get; set; }
        public Uri Uri { get; set; }

        public DateTime Date { get; set; }

        public WebPage(HtmlDocument htmlDocument)
        {
            _htmlDocument = htmlDocument;
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
    }
}
