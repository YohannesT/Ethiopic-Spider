using System;

namespace Spider.Utility
{
    public static class UriExtension
    {
        public static string GetUnicodeAbsoluteUri(this Uri uri)
        {
            return String.Format("{0}://{1}{2}", uri.Scheme, uri.Authority, uri.LocalPath);
        }
    }
}
