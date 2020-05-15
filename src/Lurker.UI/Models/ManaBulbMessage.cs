//-----------------------------------------------------------------------
// <copyright file="ManaBulbMessage.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Models
{
    /// <summary>
    /// Represents the manabulb message.
    /// </summary>
    /// <seealso cref="Lurker.UI.Models.BulbMessage" />
    public class ManaBulbMessage : BulbMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is update.
        /// </summary>
        public bool IsUpdate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance needs to be close.
        /// </summary>
        public bool NeedToHide { get; set; }

        #endregion
    }
}