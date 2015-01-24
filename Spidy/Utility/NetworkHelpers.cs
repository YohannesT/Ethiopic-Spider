using System;
using System.Net;

namespace Spider.Utility
{
    public class NetworkHelpers
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
    }
}
