//-----------------------------------------------------------------------
// <copyright file="WikiHelper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Helpers
{
    using System;
    using System.Linq;
    using HtmlAgilityPack;

    /// <summary>
    /// Represents the WikiHelper.
    /// </summary>
    public static class WikiHelper
    {
        #region Fields

        private static readonly string WikiBaseUri = "https://pathofexile.gamepedia.com/";

        #endregion

        #region Methods

        /// <summary>
        /// Creates the gem URI.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The wiki url.</returns>
        public static Uri CreateItemUri(string name)
        {
            // replace space encodes with '_' to match the link layout of the poe wiki and then url encode it
            var itemLink = System.Net.WebUtility.UrlEncode(name.Replace(" ", "_"));
            return new Uri(WikiBaseUri + itemLink);
        }

        /// <summary>
        /// Gets the item image URL.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The item image url.</returns>
        public static Uri GetItemImageUrl(string name)
        {
            var webPage = new HtmlWeb();
            var fileUrl = $"https://pathofexile.gamepedia.com/File:{System.Net.WebUtility.UrlEncode(name.Replace(" ", "_"))}_inventory_icon.png";
            var document = webPage.Load(fileUrl);

            var mediaElement = document.DocumentNode.Descendants().Where(e => e.Name == "div" && e.GetAttributeValue("class", string.Empty) == "fullMedia").FirstOrDefault();
            if (mediaElement != null)
            {
                var hyperlink = mediaElement.Descendants().Where(e => e.Name == "a").FirstOrDefault();
                if (hyperlink != null)
                {
                    var href = hyperlink.Attributes.Where(a => a.Name == "href").FirstOrDefault();
                    if (href != null)
                    {
                        return new Uri(href.Value);
                    }
                }
            }

            throw new InvalidOperationException();
        }

        #endregion
    }
}