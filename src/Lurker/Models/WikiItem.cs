//-----------------------------------------------------------------------
// <copyright file="WikiItem.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using System;

    /// <summary>
    /// Represents a Wiki item.
    /// </summary>
    public abstract class WikiItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the wiki URL.
        /// </summary>
        public Uri WikiUrl { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        public Uri ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        public int Level { get; set; }

        #endregion
    }
}