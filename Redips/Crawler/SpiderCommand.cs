using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Redips.Data;
using Redips.Utility;
using WebPage = Redips.Domain.WebPage;

namespace Redips.Crawler
{
    public class SpiderCommand
    {

        private void ResumeCrawling(string url, int? delayInMinutes = null)
        {
            var rUri = new Uri(url);
            using (var dc = new DataContext())
            {
                var spider = delayInMinutes.HasValue ? new Spider(delayInMinutes.Value) : new Spider();

                var pagesIndexed = dc.WebPages.Where(u => u.Url.Contains(rUri.Authority)).OrderByDescending(s => s.Date);
                foreach (var page in pagesIndexed)
                {
                    var webPage = new WebPage(page);
                    var website = webPage.Website;
                    var allowedUri = webPage.IntraDomainLinks.FirstOrDefault(website.IsPathAllowed);

                    if (allowedUri == null) continue;

                    Console.WriteLine("{0} Resuming from {1}", DateTime.Now.ToShortTimeString(), allowedUri.GetUnicodeAbsoluteUri());
                    spider.CrawlRecursive(allowedUri, page.ParentSite != null ? new WebPage(page.ParentSite) : null, null);
                    break;
                }
            }
        }

        private void StartCrawling(Uri uri, int? delayInMinutes = null)
        {
            var spider = delayInMinutes.HasValue ? new Spider(delayInMinutes.Value) : new Spider();
            spider.CrawlRecursive(uri, null, null);
        }

        public void StartCrawling(string[] urls)
        {
            var tasks = new List<Task>();
            tasks.AddRange(
                urls.Select(url => new Task(() =>
            {
                using (var dc = new DataContext())
                {
                    Uri uri;
                    if (Uri.TryCreate(url, UriKind.Absolute, out uri))
                    {
                        //foreach (var u in dc.WebPages.Select(p => new Uri(p.Url)).Where(u => u.Authority.ToLower() == uri.Authority.ToLower()))
                        //{
                        //    Console.Write("Fuck");
                        //}
                        //if (dc.WebPages.ToList().Select(u => new Uri(u.Url)).Any(s => s.Authority.ToLower().Equals(uri.Authority.ToLower())))
                        //    ResumeCrawling(uri.GetUnicodeAbsoluteUri());
                        //else
                        //    StartCrawling(uri);

                        if (dc.WebPages.ToList().Any(s => s.Url.ToLower().Contains(uri.Authority.ToLower())))
                            ResumeCrawling(uri.GetUnicodeAbsoluteUri());
                        else
                            StartCrawling(uri);
                    }
                    else
                        Console.WriteLine("Unable to parse the {0}", url);
                }
            })));

            tasks.ForEach(t =>
            {
                t.Start();
                Thread.Sleep(10 * 1000);
            });
        }

        public void StartFromSeed()
        {
            using (var dc = new DataContext())
            {
                foreach (var seed in dc.SeedSites)
                {
                    Uri uri;
                    if (Uri.TryCreate(seed.URL, UriKind.Absolute, out uri))
                    {
                        if (dc.WebPages.Any(s => s.Url.Contains(uri.Authority)))
                            ResumeCrawling(uri.GetUnicodeAbsoluteUri(), seed.CrawlDelayInMinutes);
                        else StartCrawling(uri, seed.CrawlDelayInMinutes);

                        Thread.Sleep(5 * 1000);
                    }
                    else
                        Console.WriteLine("Unable to parse the {0}", seed.URL);
                }
            }
        }

        private List<Uri> FindUnindexedLinks (bool intraDomainLinksOnly = true, bool withAmharicUrls = true)//aka gouge
        {
            var links = new List<Uri>();
            using (var dataContext = new DataContext())
            {
                foreach (var dWp in dataContext.WebPages)
                {
                    var wp = new WebPage(dWp);
                    var linksInPage = intraDomainLinksOnly ? wp.IntraDomainLinks : wp.AllLinks;

                    foreach (var l in linksInPage)
                    {
                        if(!dataContext.WebPages.Any(u => u.Url == l.AbsoluteUri))
                            links.Add(l);
                    }

                }
            }
            return links;
        }
    }
}
