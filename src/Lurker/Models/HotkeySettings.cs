//-----------------------------------------------------------------------
// <copyright file="HotkeySettings.cs" company="Wohs Inc.">
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
    public sealed class HotkeySettings : SettingsBase<HotkeySettings>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the toggle build.
        /// </summary>
        [DefaultValue(222)]
        public ushort ToggleBuild { get; set; }

        /// <summary>
        /// Gets or sets the invite to party.
        /// </summary>
        public Hotkey Invite { get; set; }

        /// <summary>
        /// Gets or sets the main.
        /// </summary>
        public Hotkey Main { get; set; }

        /// <summary>
        /// Gets or sets the trade.
        /// </summary>
        public Hotkey Trade { get; set; }

        /// <summary>
        /// Gets or sets the busy.
        /// </summary>
        public Hotkey Busy { get; set; }

        /// <summary>
        /// Gets or sets the dismiss.
        /// </summary>
        public Hotkey Dismiss { get; set; }

        /// <summary>
        /// Gets or sets the still interested.
        /// </summary>
        public Hotkey StillInterested { get; set; }

        /// <summary>
        /// Gets or sets the open wiki.
        /// </summary>
        public Hotkey OpenWiki { get; set; }

        /// <summary>
        /// Gets or sets the join guild hideout.
        /// </summary>
        public Hotkey JoinGuildHideout { get; set; }

        /// <summary>
        /// Gets or sets the open wiki.
        /// </summary>
        public Hotkey SearchItem { get; set; }

        /// <summary>
        /// Gets or sets the open wiki.
        /// </summary>
        public Hotkey RemainingMonster { get; set; }

        #endregion
    }
}