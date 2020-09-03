using System;

namespace UIFT
{
    public static class Extensions
    {
        public static bool IsValidUrl(this string s)
        {
            Uri uriResult;
            return Uri.TryCreate(s, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}