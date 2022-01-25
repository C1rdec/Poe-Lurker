//-----------------------------------------------------------------------
// <copyright file="StashTab.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using ConfOxide;

    /// <summary>
    /// Represetns a stash tab.
    /// </summary>
    public sealed class StashTab : SettingsBase<StashTab>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets The tabType.
        /// </summary>
        public StashTabType TabType { get; set; }

        #endregion
    }
}