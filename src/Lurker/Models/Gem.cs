//-----------------------------------------------------------------------
// <copyright file="Gem.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using System;
    using System.Linq;
    using System.Xml.Linq;
    using HtmlAgilityPack;

    /// <summary>
    /// Represents a gem.
    /// </summary>
    public class Gem
    {
        #region Fields

        private static readonly string WikiBaseUri = "https://pathofexile.gamepedia.com/";
        private static readonly string SupportValue = "Support";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets the wiki URL.
        /// </summary>
        public Uri WikiUrl { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        public Uri ImageUrl { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Froms the XML.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The gem.</returns>
        public static Gem FromXml(XElement element)
        {
            var name = element.Attribute("nameSpec").Value;
            var isSupport = element.Attribute("skillId").Value.Contains(SupportValue);

            if (isSupport)
            {
                name += $" {SupportValue}";
            }

            return new Gem()
            {
                Name = name,
                WikiUrl = CreateGemUri(name),
                Id = element.Attribute("skillId").Value,
            };
        }

        /// <summary>
        /// Sets the URL.
        /// </summary>
        public void SetUrl()
        {
            this.WikiUrl = CreateGemUri(this.Name);

            var webPage = new HtmlWeb();
            var fileUrl = $"https://pathofexile.gamepedia.com/File:{System.Net.WebUtility.UrlEncode(this.Name.Replace(" ", "_"))}_inventory_icon.png";
            var document = webPage.Load(fileUrl);
            var mediaElement = document.DocumentNode.Descendants().Where(e => e.Name == "div" && e.GetAttributeValue("class", string.Empty) == "fullMedia").FirstOrDefault();
            if (mediaElement == null)
            {
                return;
            }

            var hyperlink = mediaElement.Descendants().Where(e => e.Name == "a").FirstOrDefault();
            if (hyperlink != null)
            {
                var href = hyperlink.Attributes.Where(a => a.Name == "href").FirstOrDefault();
                if (href != null)
                {
                    this.ImageUrl = new Uri(href.Value);
                }
            }
        }

        /// <summary>
        /// Creates the gem URI.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The wiki url.</returns>
        private static Uri CreateGemUri(string name)
        {
            // replace space encodes with '_' to match the link layout of the poe wiki and then url encode it
            var itemLink = System.Net.WebUtility.UrlEncode(name.Replace(" ", "_"));
            return new Uri(WikiBaseUri + itemLink);
        }

        #endregion
    }
}