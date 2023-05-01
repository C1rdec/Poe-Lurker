//-----------------------------------------------------------------------
// <copyright file="PoeDBHelper.cs" company="Wohs Inc.">
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
    public static class PoeDBHelper
    {
        #region Fields

        private static readonly string BaseUri = $"https://poedb.tw/us/";

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
            name = name.Replace("'", string.Empty).Trim();
            var itemLink = System.Net.WebUtility.UrlEncode(name.Replace(" ", "_"));

            return new Uri(BaseUri + itemLink);
        }

        /// <summary>
        /// Gets the item image URL.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The item image url.</returns>
        public static Uri GetItemImageUrl(string name)
        {
            var webPage = new HtmlWeb();
            name = name.Replace("'", string.Empty).Trim();
            var escapeName = System.Net.WebUtility.UrlEncode(name.Replace(" ", "_"));

            return ParseMedia($"{BaseUri}{escapeName}", webPage);
        }

        /// <summary>
        /// Gets the item level.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The item level.</returns>
        public static int GetItemLevelRequirement(string name)
        {
            var url = CreateItemUri(name);
            var webPage = new HtmlWeb();
            var document = webPage.Load(url);

            var requirementsDiv = document.DocumentNode.Descendants().FirstOrDefault(e => e.Name == "div" && e.GetAttributeValue("class", string.Empty) == "requirements");
            if (requirementsDiv == null)
            {
                return 1;
            }

            var firstSpan = requirementsDiv.Descendants().FirstOrDefault(e => e.Name == "span");

            return int.Parse(firstSpan.InnerText);
        }

        /// <summary>
        /// Parses the media.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="webPage">The web page.</param>
        /// <returns>The url.</returns>
        public static Uri ParseMedia(string url, HtmlWeb webPage)
        {
            var document = webPage.Load(url);
            var mediaElement = document.DocumentNode.Descendants().FirstOrDefault(e => e.Name == "div" && e.GetAttributeValue("class", string.Empty) == "itemboximage");
            if (mediaElement != null)
            {
                var imgElement = mediaElement.Descendants().FirstOrDefault(e => e.Name == "img");
                if (imgElement != null)
                {
                    var value = imgElement.GetAttributeValue("src", string.Empty);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return new Uri(value);
                    }
                }
            }

            throw new InvalidOperationException();
        }

        #endregion
    }
}