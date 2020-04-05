//-----------------------------------------------------------------------
// <copyright file="ManaBulbMessage.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Models
{
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
