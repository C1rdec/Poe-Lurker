﻿//-----------------------------------------------------------------------
// <copyright file="WikiHelper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Helpers;

using System;
using System.Linq;
using HtmlAgilityPack;

/// <summary>
/// Represents the WikiHelper.
/// </summary>
public static class WikiHelper
{
    #region Fields

    private static readonly string FandomBaseUri = "https://www.poewiki.net";
    private static readonly string WikiBaseUri = $"{FandomBaseUri}/wiki/";

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
    /// Creates the gem URI.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The wiki url.</returns>
    public static Uri GetItemImageUrl2(string name)
    {
        var webPage = new HtmlWeb();
        var escapeName = System.Net.WebUtility.UrlEncode(name.Replace(" ", "_"));
        var document = webPage.Load($"{WikiBaseUri}{escapeName}");

        var span = document.DocumentNode.Descendants().FirstOrDefault(e => e.Name == "span" && e.GetAttributeValue("class", string.Empty) == "images");
        if (span == null)
        {
            return new Uri("https://static.wikia.nocookie.net/pathofexile_gamepedia/images/7/72/Armour_item_icon.png/revision/latest/scale-to-width-down/80?cb=20141212074830");
        }

        var aElement = span.Descendants().FirstOrDefault(e => e.Name == "a" && e.GetAttributeValue("class", string.Empty) == "image");

        if (aElement == null)
        {
            return new Uri("https://static.wikia.nocookie.net/pathofexile_gamepedia/images/7/72/Armour_item_icon.png/revision/latest/scale-to-width-down/80?cb=20141212074830");
        }

        var image = aElement.Descendants().FirstOrDefault(e => e.Name == "img");

        var src = image.GetAttributeValue("src", string.Empty);
        if (string.IsNullOrEmpty(src))
        {
            return new Uri("https://static.wikia.nocookie.net/pathofexile_gamepedia/images/7/72/Armour_item_icon.png/revision/latest/scale-to-width-down/80?cb=20141212074830");
        }

        if (src.StartsWith("data:"))
        {
            src = image.GetAttributeValue("data-src", string.Empty);
        }

        if (src.StartsWith("http"))
        {
            return new Uri(src);
        }

        return new Uri($"{FandomBaseUri}{src}");
    }

    /// <summary>
    /// Gets the item image URL.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The item image url.</returns>
    public static Uri GetItemImageUrl(string name)
    {
        var webPage = new HtmlWeb();
        var escapeName = System.Net.WebUtility.UrlEncode(name.Replace(" ", "_"));

        return ParseMedia($"{WikiBaseUri}{escapeName}", webPage);
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
        var mediaElement = document.DocumentNode.Descendants().FirstOrDefault(e => e.Name == "span" && e.GetAttributeValue("class", string.Empty) == "images");
        if (mediaElement != null)
        {
            var hyperlink = mediaElement.Descendants().Where(e => e.Name == "a").FirstOrDefault();
            if (hyperlink != null)
            {
                var href = hyperlink.Attributes.Where(a => a.Name == "href").FirstOrDefault();
                if (href != null)
                {
                    try
                    {
                        return new Uri(href.Value);
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
        }
        else
        {
            var itemTable = document.DocumentNode.Descendants().FirstOrDefault(e => e.Name == "table" && e.GetAttributeValue("class", string.Empty).Contains(" item-table"));
            if (itemTable != null)
            {
                var firstTd = itemTable.Descendants("td").FirstOrDefault();
                var a = firstTd.Descendants("a").FirstOrDefault();
                var hrefValue = a.GetAttributeValue("href", string.Empty);
                if (!string.IsNullOrEmpty(hrefValue))
                {
                    return ParseMedia($"{FandomBaseUri}{hrefValue}", webPage);
                }
            }
        }

        return null;
    }

    #endregion
}