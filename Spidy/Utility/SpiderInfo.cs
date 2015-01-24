
using System.Configuration;

namespace Spider.Utility
{
    public static class SpiderInfo
    {
        public static string Useragent
        {
            get { return ConfigurationManager.AppSettings["userAgent"]; }
        }

        public static string UseragentEmailAddress
        {
            get { return ConfigurationManager.AppSettings["userAgentEmailAddress"]; }
        }

        public static string UseragentDocumentation
        {
            get { return ConfigurationManager.AppSettings["userAgentDocumentation"]; }
        }

    }
}
