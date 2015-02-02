using System;
using System.Configuration;

namespace Redips.Utility
{
    public static class SpiderInfo
    {
        private static string _userAgent, _userAgentEmailAddress, _userAgentDocumentation;
        private static int _delay = 5 * 60 * 1000;
        private static int _asyncThreadCount = 10;

        public static string Useragent
        {
            get { return String.IsNullOrWhiteSpace(_userAgent) ? _userAgent = ConfigurationManager.AppSettings["userAgent"] : _userAgent; }
        }

        public static string UseragentEmailAddress
        {
            get { return String.IsNullOrWhiteSpace(_userAgentEmailAddress) ? _userAgentEmailAddress = ConfigurationManager.AppSettings["userAgentEmailAddress"] : _userAgentEmailAddress; }
        }

        public static string UseragentDocumentation
        {
            get { return  String.IsNullOrWhiteSpace(_userAgentDocumentation) ? _userAgentDocumentation =ConfigurationManager.AppSettings["userAgentDocumentation"] : _userAgentDocumentation; }
        }

        public static int StandardDelay
        {
            get
            {
                // ReSharper disable once RedundantAssignment
                int.TryParse(ConfigurationManager.AppSettings["standardDelay"], out _delay);
                return _delay;
            }
        }

        public static int AsyncThreadCount
        {
            get
            {
                int.TryParse(ConfigurationManager.AppSettings["asyncThreadCount"], out _asyncThreadCount);
                return _asyncThreadCount;
            }
        }

        public static bool IntraDomainOnly
        {
            get
            {
                return ConfigurationManager.AppSettings["intraDomainOnly"].ToLower().Trim() == "true";
            }
        }
    }
}
