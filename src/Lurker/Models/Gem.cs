//-----------------------------------------------------------------------
// <copyright file="Gem.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using HtmlAgilityPack;
    using Lurker.Helpers;

    /// <summary>
    /// Represents a gem.
    /// </summary>
    public class Gem : WikiItem
    {
        #region Fields

        private static readonly string SupportValue = "Support";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Gem"/> is support.
        /// </summary>
        public bool Support { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public GemLocation Location { get; set; }

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
                WikiUrl = WikiHelper.CreateItemUri(name),
                Id = element.Attribute("skillId").Value,
            };
        }

        /// <summary>
        /// Sets the URL.
        /// </summary>
        public void ParseWiki()
        {
            this.WikiUrl = PoeDBHelper.CreateItemUri(this.Name);

            this.SetImageUrl();
            this.SetLocation();
        }

        /// <summary>
        /// Sets the image URL.
        /// </summary>
        private void SetImageUrl()
        {
            try
            {
                this.ImageUrl = WikiHelper.GetItemImageUrl(this.Name);
                if (this.ImageUrl == null)
                {
                    this.ImageUrl = PoeDBHelper.GetItemImageUrl(this.Name);
                }
            }
            catch (InvalidOperationException)
            {
            }
        }

        /// <summary>
        /// Sets the location.
        /// </summary>
        private void SetLocation()
        {
            var webPage = new HtmlWeb();
            HtmlDocument document = default;
            try
            {
                document = webPage.Load(WikiHelper.CreateItemUri(this.Name));
            }
            catch
            {
                return;
            }

            this.SetWikiLocation(document);
        }

        private void SetWikiLocation(HtmlDocument document)
        {
            var vendorRewardSpan = document.DocumentNode.Descendants().FirstOrDefault(e => e.Name == "span" && e.GetAttributeValue("id", string.Empty) == "Vendor_reward");

            if (vendorRewardSpan == null)
            {
                return;
            }

            var h3Element = vendorRewardSpan.ParentNode;
            var sibling = h3Element.NextSibling;
            var siblingCount = 0;
            while (sibling.Name != "table")
            {
                if (siblingCount >= 10)
                {
                    return;
                }

                sibling = sibling.NextSibling;
                siblingCount++;
            }

            var tableBody = sibling.Descendants().FirstOrDefault(e => e.Name == "tbody");
            if (tableBody == null)
            {
                return;
            }

            var classRow = tableBody.ChildNodes.FirstOrDefault();
            if (classRow == null)
            {
                return;
            }

            var questRow = tableBody.ChildNodes.ElementAt(2);
            var questHeader = questRow.ChildNodes.FirstOrDefault(c => c.Name == "th");
            var gemLocation = new GemLocation();
            var texts = questHeader.Descendants().Where(e => e.Name == "#text").Select(e => e.InnerText);
            if (texts.Count() >= 3)
            {
                gemLocation.Quest = texts.ElementAt(0);
                gemLocation.Act = texts.ElementAt(1);
                gemLocation.Npc = texts.ElementAt(2);
            }

            // ✗; ✓
            var availabilities = questRow.Descendants().Where(e => e.Name == "td").Select(e => e.InnerText);
            var classValues = classRow.Descendants().Where(e => e.Name == "a").Select(e => e.InnerText);
            var classes = new List<Class>();

            // All Classes
            if (availabilities.Count() == 1)
            {
                classes.Add(Class.Witch);
                classes.Add(Class.Shadow);
                classes.Add(Class.Ranger);
                classes.Add(Class.Duelist);
                classes.Add(Class.Marauder);
                classes.Add(Class.Templar);
                classes.Add(Class.Scion);
            }
            else
            {
                for (int i = 0; i < availabilities.Count(); i++)
                {
                    if (availabilities.ElementAt(i) == "✓")
                    {
                        var classValue = classValues.ElementAt(i);
                        classes.Add((Class)Enum.Parse(typeof(Class), classValue));
                    }
                }
            }

            gemLocation.Classes = classes;
            this.Location = gemLocation;
        }

        #endregion
    }
}