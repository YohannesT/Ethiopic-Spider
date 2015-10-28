using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Redips.Utility;

namespace Redips.Services
{
    public class NetworkService
    {
        public static IPAddress[] GetIpAddresses(Uri uri)
        {
            try
            {
                return Dns.GetHostAddresses(uri.Host);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<HtmlDocument> GetHtmlDocumentAsync(Uri uri, int retryCount = 3, int currentRetry = 0)
        {
            try
            {
                uri = uri.LocalPath.Contains("//") ? new Uri(uri.Scheme + "://" + uri.Authority + uri.AbsolutePath.Replace("//", "/")) : uri;

                var request = WebRequest.CreateHttp(uri);

                request.UserAgent = SpiderInfo.Useragent + " - " + SpiderInfo.UseragentDocumentation;
                request.Headers.Add(HttpRequestHeader.From, SpiderInfo.UseragentDocumentation);

                var response = await request.GetResponseAsync();
                var stream = response.GetResponseStream();

                if (stream == null) return null;
                var htmlDocument = new HtmlDocument();
                htmlDocument.Load(stream, Encoding.UTF8);

                return htmlDocument;
            }
            catch (WebException)
            {
                if (retryCount <= currentRetry + 1) return null;

                var task = GetHtmlDocumentAsync(uri, retryCount, currentRetry++);

                return task.Result;
            }
        }
    }
}
