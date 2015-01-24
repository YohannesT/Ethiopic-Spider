using System;
using System.Linq;
using System.Threading;
using Spider.Data;
using Spider.Domain;
using Spider.Utility;

namespace Spider.Crowler
{
    public class SpiderCommand
    {
        private readonly DataContext _dc = new DataContext();

        private void ResumeCrawling(string url)
        {
            var spider = new Spider.Spider();

            var lastSite = _dc.WebPages.OrderByDescending(s => s.Date).First();
            var uri = new Uri(lastSite.Url);
            _dc.EthiopicWords.RemoveRange(_dc.EthiopicWords.Where(w => w.SourceWebPageID == lastSite.WebPageID).ToList());
            _dc.WebPages.Remove(lastSite);
            _dc.SaveChanges();
            Console.WriteLine("{0} Resuming from {1}", DateTime.Now.ToShortTimeString(), uri.AbsoluteUri);
            
            spider.CrawlRecursive(uri, lastSite.ParentSite != null ? new WebPage(lastSite.ParentSite) : null, null );
        }

        private void StartCrawling(Uri uri)
        {
            var spider = new Spider.Spider();
            spider.CrawlRecursive(uri, null, null);
        }

        public void StartCrawling(string[] urls)
        {
            foreach (var url in urls)
            {
                Uri uri;
                if (Uri.TryCreate(url, UriKind.Absolute, out uri))
                {
                    if(_dc.WebPages.Any(s => s.Url.Contains(uri.Authority)))
                        ResumeCrawling(uri.GetUnicodeAbsoluteUri());
                    else StartCrawling(uri);

                    Thread.Sleep(30 * 1000);
                }
                else
                {
                    Console.WriteLine("Unable to parse the {0}", url);
                }
            }
        }

        public void StartFromSeed()
        {
            foreach (var seed in _dc.SeedSites)
            {
                Uri uri;
                if (Uri.TryCreate(seed.URL, UriKind.Absolute, out uri))
                {
                    if (_dc.WebPages.Any(s => s.Url.Contains(uri.Authority)))
                        ResumeCrawling(uri.GetUnicodeAbsoluteUri());
                    else StartCrawling(uri);

                    Thread.Sleep(30 * 1000);
                }
                else
                {
                    Console.WriteLine("Unable to parse the {0}", seed.URL);
                }
            }
        }
    }
}
