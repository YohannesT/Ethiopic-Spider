using System;

namespace Redips.Utility
{
    public static class UriExtension
    {
        public static string GetUnicodeUri(this Uri uri)
        {
            return String.Format("{0}://{1}", uri.Scheme, uri.Authority);
        }

        public static string GetUnicodeAbsoluteUri(this Uri uri)
        {
            return String.Format("{0}://{1}{2}", uri.Scheme, uri.Authority, uri.LocalPath.Contains("//") ? uri.LocalPath.Replace("//", "/") : uri.LocalPath);
        }
    }
}
