// <copyright file="SkillTreeInformation.cs" company="Wohs Inc.">
// Copyright © Wohs Inc.
// </copyright>

namespace Lurker.Models
{
    /// <summary>
    /// Represent tree information.
    /// </summary>
    public class SkillTreeInformation
    {
        /// <summary>
        /// Gets or Sets the url.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or Sets the version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or Sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <returns>The display name.</returns>
        public string DisplayName => string.IsNullOrEmpty(this.Title) ? this.Version : this.Title;
    }
}