//-----------------------------------------------------------------------
// <copyright file="ItemMessage.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Models
{
    using Lurker.Models;

    /// <summary>
    /// Represents a Item message.
    /// </summary>
    /// <seealso cref="Lurker.UI.Models.TimelineMessage" />
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