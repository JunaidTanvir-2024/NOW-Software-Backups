using System;

namespace TalkHome.Extensions
{
    /// <summary>
    /// Extends methods for the Uri class
    /// </summary>
    public static class UriExtensions
    {
        /// <summary>
        /// Removes port number and returns the cleaned Uri
        /// </summary>
        /// <param name="uri">The input Uri</param>
        /// <returns>The cleaned Uri</returns>
        public static string ToCleanUri(this Uri uri)
        {
            return uri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Port, UriFormat.UriEscaped);
        }
    }
}
