﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spider.Data;
using Spider.Domain;
using Spider.Utility;

namespace Spider.Redips
{
    public class SpiderCommand
    {
        private void ResumeCrawling(string url, int? delayInMinutes = null)
        {
            var dc = new DataContext();
            var spider = delayInMinutes.HasValue ? new Spider.Spider(delayInMinutes.Value) : new Spider.Spider();

            var lastSite = dc.WebPages.Where(u => u.Url == url).OrderByDescending(s => s.Date).First();
            var uri = new Uri(lastSite.Url);
            dc.EthiopicWords.RemoveRange(dc.EthiopicWords.Where(w => w.SourceWebPageID == lastSite.WebPageID).ToList());
            dc.WebPages.Remove(lastSite);
            dc.SaveChanges();
            Console.WriteLine("{0} Resuming from {1}", DateTime.Now.ToShortTimeString(), uri.AbsoluteUri);

            spider.CrawlRecursive(uri, lastSite.ParentSite != null ? new WebPage(lastSite.ParentSite) : null, null);
        }

        private void StartCrawling(Uri uri, int? delayInMinutes = null)
        {
            var spider = delayInMinutes.HasValue ? new Spider.Spider(delayInMinutes.Value) : new Spider.Spider();

            spider.CrawlRecursive(uri, null, null);
        }

        public void StartCrawling(string[] urls)
        {
            var tasks = new List<Task>();

            using (var dc = new DataContext())
            {
                tasks.AddRange(
                    urls.Select(url => new Task(() =>
                {
                    Uri uri;
                    if (Uri.TryCreate(url, UriKind.Absolute, out uri))
                    {
                        if (dc.WebPages.Any(s => s.Url.Contains(uri.Authority)))
                            ResumeCrawling(uri.GetUnicodeAbsoluteUri());
                        else StartCrawling(uri);
                    }
                    else
                        Console.WriteLine("Unable to parse the {0}", url);
                })));

                tasks.ForEach(t =>
                {
                    t.Start();
                    Thread.Sleep(5000);
                });
            }
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
    }
}
