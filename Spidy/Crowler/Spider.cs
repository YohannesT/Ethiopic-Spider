using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RobotsTxt;
using Spider.Data;
using Spider.Data.Models;
using Spider.Domain;

namespace Spider.Crowler
{
    public class Spider
    {
        private const string UserAgent = "EthRobot";
        private const string UserAgentEmailAddress = "eth.robot@gmail.com";
        private const string UserAgentDocumentation = "https://docs.google.com/document/d/1wzhmCvrz4kcR4jK4v5PCA2_vGPkoj1wd9ky0yyAFN2A/edit?usp=sharing";

        private readonly int _maxAsyncThreadCount = 10;
        private int _asyncThreadCount;
        private readonly bool _intraDomainOnly;
        public readonly int StandardDelay = 5 * 60 * 1000;
        private readonly DataContext _dataContext = new DataContext();

        public Spider()
        {
            _intraDomainOnly = true;
            _maxAsyncThreadCount = 10;
        }

        public Spider(int maxAsyncThreadCount, bool intraDomainOnly = true)
        {
            _maxAsyncThreadCount = maxAsyncThreadCount;
            _intraDomainOnly = intraDomainOnly;
        }

        public async Task CrawlRecursive(Uri uri, Uri parentUri, Robots robotsTxt = null)
        {
            _asyncThreadCount++;

            while (_asyncThreadCount > _maxAsyncThreadCount)
            {
                Console.WriteLine("{0} Waiting for others to finish before starting {1}", DateTime.Now.ToShortTimeString(), uri.AbsoluteUri);
                Thread.Sleep(1000);
            }

            Console.WriteLine("{0} Crawling over {1}.", DateTime.Now.ToShortTimeString(), uri);

            var siteUrl = String.Format("{0}://{1}{2}", uri.Scheme, uri.Authority, uri.LocalPath);
            if (_dataContext.Sites.Any(s => s.Url == siteUrl))
                return;

            if (robotsTxt == null || parentUri == null || uri.Authority != parentUri.Authority)
                robotsTxt = new Robots(await GetRobotsTxtAsync(uri));

            string ipAddresses = String.Empty;
            if (parentUri == null || uri.Authority != parentUri.Authority)
                ipAddresses = GetIpAddresses(uri);

            var website = await GetWebsiteAsync(uri);

            if (website == null || !website.Save(ipAddresses, parentUri)) return;

            if (!await CheckAndSaveAsync(website)) //if saving fails, try one more time
                await CheckAndSaveAsync(website);  //if it fails again, then leave it

            var siteLinks = _intraDomainOnly ? website.IntraDomainLinks : website.AllLinks;
            var allowedUris = siteLinks.Where(u => robotsTxt.IsPathAllowed(UserAgent, u.AbsoluteUri) && !_dataContext.Sites.Any(s => s.Url == u.AbsoluteUri)).ToList();

            _asyncThreadCount--;
            foreach (var childUri in allowedUris)
            {
                try
                {
                    if (uri.Authority == childUri.Authority)
                    {
                        var robotDelay = (int)robotsTxt.CrawlDelay(UserAgent);
                        Thread.Sleep(StandardDelay > robotDelay ? StandardDelay : robotDelay);
                    }

                    await CrawlRecursive(childUri, uri, robotsTxt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Error crawling over {1}. {2}.", DateTime.Now.ToShortTimeString(), uri.AbsoluteUri, ex.Message);
                }
            }
        }

        private static string GetIpAddresses(Uri uri)
        {
            Console.WriteLine("...{0} Getting IP of {1}.", DateTime.Now.ToShortTimeString(), uri.AbsoluteUri);
            var ipAddresses = String.Empty;
            try
            {
                var add = Dns.GetHostAddresses(uri.Host);
                ipAddresses = add != null && add.Length > 0
                    ? String.Join(";", add.Select(ip => ip.ToString()))
                    : String.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine("...{0} Error getting IP of {1}. {2}.", DateTime.Now.ToShortTimeString(), uri.AbsoluteUri, ex.Message);
            }
            return ipAddresses;
        }

        private static async Task<Website> GetWebsiteAsync(Uri uri)
        {
            try
            {
                var request = WebRequest.CreateHttp(uri);
                
                request.UserAgent = UserAgent + " - " + UserAgentDocumentation;
                request.Headers.Add(HttpRequestHeader.From, UserAgentEmailAddress);

                var response = await request.GetResponseAsync();
                var stream = response.GetResponseStream();

                if (stream == null) return null;
                var htmlDocument = new HtmlDocument();
                htmlDocument.Load(stream, Encoding.UTF8);

                return new Website
                {
                    Uri = response.ResponseUri,
                    HtmlDocument = htmlDocument,
                    HtmlCharacterLength = htmlDocument.DocumentNode.OuterHtml.Length,
                    StreamLength = stream.CanSeek ? stream.Length : -1
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<bool> CheckAndSaveAsync(Website sourceSite)
        {
            var innerText = sourceSite.InnerText;
            if (!await sourceSite.InnerText.ContainsEthipicAsync()) return true;

            try
            {
                var paragraphs = await innerText.GetEthipicParagraphsAsync();
                var words = await innerText.GetEthipicWordsAsync();

                var eParagraphs = paragraphs.Select(p => new Paragraph
                {
                    Value = p,
                    SourceSiteID = sourceSite.WebsiteID,
                    DateOfEntry = DateTime.Now
                });

                var eWords = words.Select(w => new EthiopicWord
                {
                    Name = w,
                    SourceSiteID = sourceSite.WebsiteID,
                    DateOfEntry = DateTime.Now
                });

                _dataContext.Words.AddRange(eWords);
                _dataContext.Paragraphs.AddRange(eParagraphs);
                _dataContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> GetRobotsTxtAsync(Uri uri)
        {
            Console.WriteLine("...{0} Fetching robots.txt for {1}", DateTime.Now.ToShortTimeString(), uri.Authority);
            var robotsUri = String.Format("{0}://{1}/robots.txt", uri.Scheme, uri.Authority);//
            var request = WebRequest.Create(robotsUri);
            try
            {
                var response = await request.GetResponseAsync();

                var responseStream = response.GetResponseStream();
                if (responseStream == null) return String.Empty;

                var sr = new StreamReader(responseStream);
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine("......{0} Error fetching robots.txt for {1}, {2}", DateTime.Now.ToShortTimeString(), uri.Authority, ex.Message);
                return String.Empty;
            }
        }
    }
}
