using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Spider.Domain;
using Spider.Utility;

namespace Redips.Crawler
{
    public class Spider
    {
        private readonly int _maxAsyncThreadCount = SpiderInfo.AsyncThreadCount;
        private readonly bool _intraDomainOnly = SpiderInfo.IntraDomainOnly;
        public readonly int StandardDelay = SpiderInfo.StandardDelay;

        private int _asyncThreadCount;

        public Spider()
        {

        }

        public Spider(int delayInMinutes)
        {
            StandardDelay = delayInMinutes * 60 * 1000;
        }

        public Spider(int maxAsyncThreadCount, int delayInMinutes, bool intraDomainOnly = true)
        {
            _maxAsyncThreadCount = maxAsyncThreadCount;
            _intraDomainOnly = intraDomainOnly;
            StandardDelay = delayInMinutes*60*1000;
        }

        public async Task CrawlRecursive(Uri uri, WebPage parentWebPage, Website website)
        {
            _asyncThreadCount++;

            while (_asyncThreadCount > _maxAsyncThreadCount)
            {
                Console.WriteLine("{0} Waiting for others to finish before starting {1}", DateTime.Now.ToShortTimeString(), uri.AbsoluteUri);
                Thread.Sleep(1000);
            }

            Console.WriteLine("{0} Crawling over {1}.", DateTime.Now.ToShortTimeString(), uri.GetUnicodeAbsoluteUri());

            if (WebPage.IsWebPageSaved(uri)) return;

            if (website == null || parentWebPage == null || uri.Authority != parentWebPage.Uri.Authority)
            {
                if (Website.IsWebsiteSaved(uri))
                    website = Website.GetWebsite(uri);
                else
                {
                  website =  new Website(uri);
                    if(!website.Save()) //if saving fails
                        if (!website.Save()) //try again
                            return; //if it fails again, exit the function
                }  
            }

            var webPage = new WebPage(website, uri, parentWebPage);
            if (!webPage.IsDataLloaded) return; //if for some reason the page wasn't loaded, return

            var siteLinks = _intraDomainOnly ? webPage.IntraDomainLinks : webPage.AllLinks;
            var allowedUris = siteLinks.Where(u => website.IsWebsiteAllowed(u));

            if (!webPage.Save())
                webPage.Save();

            if (webPage.IsSaved)
                if(!webPage.SaveEthiopicContent())
                    webPage.SaveEthiopicContent();

            _asyncThreadCount--;

            foreach (var childUri in allowedUris)
            {
                try
                {
                    if (uri.Authority == childUri.Authority)
                    {
                        var robotDelay =  website.Delay;
                        Console.WriteLine("Sleeping until {0} before crawling {1} again", DateTime.Now.AddMilliseconds(StandardDelay).ToShortDateString(), uri.GetUnicodeAbsoluteUri() );
                        Thread.Sleep(StandardDelay > robotDelay ? StandardDelay : robotDelay);
                    }

                    await CrawlRecursive(childUri, webPage, website);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Error crawling over {1}. {2}.", DateTime.Now.ToShortTimeString(), uri.AbsoluteUri, ex.Message);
                }
            }
        }
    }
}
