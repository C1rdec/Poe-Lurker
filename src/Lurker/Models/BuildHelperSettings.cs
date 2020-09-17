//-----------------------------------------------------------------------
// <copyright file="BuildHelperSettings.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using ConfOxide;

    /// <summary>
    /// Represents build helper settings.
    /// </summary>
    public sealed class BuildHelperSettings : SettingsBase<BuildHelperSettings>
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [timeline enabled].
        /// </summary>
        public bool TimelineEnabled { get; set; }

        /// <summary>
        /// Gets or sets the skill selected.
        /// </summary>
        public int SkillSelected { get; set; }

        #endregion
    }
}