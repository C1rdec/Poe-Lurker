//-----------------------------------------------------------------------
// <copyright file="ItemMessage.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Models
{
    using PoeLurker.Core.Models;

    /// <summary>
    /// Represents a Item message.
    /// </summary>
    /// <seealso cref="PoeLurker.UI.Models.TimelineMessage" />
    public class ItemMessage : TimelineMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the item.
        /// </summary>
        public UniqueItem Item { get; set; }

        #endregion
    }
}