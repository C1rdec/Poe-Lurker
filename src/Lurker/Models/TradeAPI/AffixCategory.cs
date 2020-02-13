//-----------------------------------------------------------------------
// <copyright file="AffixCategory.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models.TradeAPI
{
    using System.Collections.Generic;

    public class AffixCategory
    {
        #region Properties

        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets or sets the entries.
        /// </summary>
        public IEnumerable<AffixEntry> Entries { get; set; }

        #endregion
    }
}
