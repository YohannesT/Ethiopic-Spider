using System;
using System.Linq;
using System.Threading;
using Spider.Data;

namespace Spider.Crowler
{
    public class SpiderCommand
    {
        private readonly DataContext _dc = new DataContext();

        private void ResumeCrawling(string url)
        {
            var spider = new Spider.Spider();

            var lastSite = _dc.Sites.OrderByDescending(s => s.VisitedDate).First();
            var uri = new Uri(lastSite.Url);
            var pUri = new Uri(lastSite.ParentSite.Url);
            _dc.Words.RemoveRange(_dc.Words.Where(w => w.SourceSiteID == lastSite.SiteID).ToList());
            _dc.Sites.Remove(lastSite);
            _dc.SaveChanges();
            Console.WriteLine("{0} Resuming from {1}", DateTime.Now.ToShortTimeString(), uri.AbsoluteUri);
            
            spider.CrawlRecursive(uri, pUri);
        }

        private void StartCrawling(Uri uri)
        {
            var spider = new Spider.Spider();
            spider.CrawlRecursive(uri, null);
        }

        public void StartCrawling(string[] urls)
        {
            foreach (var url in urls)
            {
                Uri uri;
                if (Uri.TryCreate(url, UriKind.Absolute, out uri))
                {
                    if(_dc.Sites.Any(s => s.Url.Contains(uri.Authority)))
                        ResumeCrawling(String.Format("{0}://{1}/{2}", uri.Scheme, uri.Authority, uri.LocalPath));
                    else StartCrawling(uri);

                    Thread.Sleep(30 * 1000);
                }
                else
                {
                    Console.WriteLine("Unable to parse the {0}", url);
                }
            }
        }
    }
}
