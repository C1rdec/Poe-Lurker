//-----------------------------------------------------------------------
// <copyright file="UniqueItem.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using Lurker.Patreon.Models;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents a unique item.
    /// </summary>
    /// <seealso cref="Lurker.Models.WikiItem" />
    public class UniqueItem : WikiItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets the item class.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemClass ItemClass { get; set; }

        #endregion
    }
}