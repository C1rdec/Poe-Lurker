//-----------------------------------------------------------------------
// <copyright file="KeyCodeSettings.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using System.ComponentModel;
    using ConfOxide;

    /// <summary>
    /// Represents the key settings.
    /// </summary>
    public sealed class KeyCodeSettings : SettingsBase<KeyCodeSettings>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the toggle build.
        /// </summary>
        [DefaultValue(222)]
        public ushort ToggleBuild { get; set; }

        #endregion
    }
}