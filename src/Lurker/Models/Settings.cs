//-----------------------------------------------------------------------
// <copyright file="Settings.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using System.ComponentModel;
    using ConfOxide;

    /// <summary>
    /// Represents the settings.
    /// </summary>
    public sealed class Settings : SettingsBase<Settings>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the busy message.
        /// </summary>
        public string BusyMessage { get; set; }

        /// <summary>
        /// Gets or sets the sold message.
        /// </summary>
        public string SoldMessage { get; set; }

        /// <summary>
        /// Gets or sets the thank you message.
        /// </summary>
        public string ThankYouMessage { get; set; }

        /// <summary>
        /// Gets or sets the sill interested message.
        /// </summary>
        public string StillInterestedMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [first launch].
        /// </summary>
        public bool FirstLaunch { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [incoming trade enabled].
        /// </summary>
        [DefaultValue(true)]
        public bool IncomingTradeEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [outgoing trade enabled].
        /// </summary>
        [DefaultValue(true)]
        public bool OutgoingTradeEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [search enabled].
        /// </summary>
        public bool SearchEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [search enabled].
        /// </summary>
        public bool MapEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [debug enabled].
        /// </summary>
        public bool DebugEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [alert enabled].
        /// </summary>
        public bool AlertEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [join hideout enabled].
        /// </summary>
        public bool JoinHideoutEnabled { get; set; }

        /// <summary>
        /// Gets or sets the alert volume.
        /// </summary>
        [DefaultValue(1)]
        public float AlertVolume { get; set; }

        /// <summary>
        /// Gets or sets the join hideout volume.
        /// </summary>
        [DefaultValue(1)]
        public float JoinHideoutVolume { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [tool tip enabled].
        /// </summary>
        [DefaultValue(true)]
        public bool ToolTipEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [clipboard enabled].
        /// </summary>
        [DefaultValue(true)]
        public bool ClipboardEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [automatic kick].
        /// </summary>
        [DefaultValue(true)]
        public bool AutoKickEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [item highlight enabled].
        /// </summary>
        public bool ItemHighlightEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [item highlight enabled].
        /// </summary>
        public bool RemainingMonsterEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [dashboard enabled].
        /// </summary>
        public bool DashboardEnabled { get; set; }

        /// <summary>
        /// Gets or sets the tooltip delay.
        /// </summary>
        [DefaultValue(1000)]
        public int ToolTipDelay { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [hide in background].
        /// </summary>
        [DefaultValue(true)]
        public bool HideInBackground { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [hide in background].
        /// </summary>
        public bool DeleteItemEnabled { get; set; }

        /// <summary>
        /// Gets or sets the life foreground.
        /// </summary>
        [DefaultValue("#FFFFFFFF")]
        public string LifeForeground { get; set; }

        /// <summary>
        /// Gets or sets the trade bar scaling.
        /// </summary>
        [DefaultValue(1)]
        public double TradebarScaling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show startup animation].
        /// </summary>
        [DefaultValue(true)]
        public bool ShowStartupAnimation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Settings"/> is vulkan.
        /// </summary>
        public bool VulkanRenderer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [path of building].
        /// </summary>
        public bool BuildHelper { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [sold detection].
        /// </summary>
        [DefaultValue(true)]
        public bool SoldDetection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show release note].
        /// </summary>
        [DefaultValue(true)]
        public bool ShowReleaseNote { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [timeline enabled].
        /// </summary>
        public bool TimelineEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [group by skill].
        /// </summary>
        public bool GroupBySkill { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [synchronize build].
        /// </summary>
        [DefaultValue(true)]
        public bool SyncBuild { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [hideout enabled].
        /// </summary>
        [DefaultValue(true)]
        public bool HideoutEnabled { get; set; }

        #endregion
    }
}