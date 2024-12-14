//-----------------------------------------------------------------------
// <copyright file="Settings.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Models;

using PoeLurker.Core.Helpers;

/// <summary>
/// Represents the settings.
/// </summary>
public sealed class Settings
{
    private static readonly string DefaultStillInterestedMessage = $"Are you still interested in my {TokenHelper.ItemName} listed for {TokenHelper.Price}?";
    private static readonly string DefaultSoldMessage = $"I'm sorry, my {TokenHelper.ItemName} has already been sold.";
    private static readonly string DefaultBusyMessage = "I'm busy right now I'll send you a party invite.";
    private static readonly string DefaultThankYouMessage = string.Empty;

    public Settings()
    {
        StillInterestedMessage = DefaultStillInterestedMessage;
        SoldMessage = DefaultSoldMessage;
        BusyMessage = DefaultBusyMessage;
        ThankYouMessage = DefaultThankYouMessage;

        ShowWelcome = true;
        IncomingTradeEnabled = true;
        OutgoingTradeEnabled = true;
        ToolTipEnabled = true;
        ClipboardEnabled = true;
        AutoKickEnabled = true;
        HideInBackground = true;
        HideoutEnabled = true;
        BuildAutoClose = true;
        ShowReleaseNote = true;
        ShowStartupAnimation = true;
        SoldDetection = true;
        OutgoingDelayToClose = 100;
        JoinHideoutVolume = 1;
        AlertVolume = 1;
        ItemAlertVolume = 0.5f;
        TradebarScaling = 1;
        LifeForeground = "#FFFFFFFF";
        ToolTipDelay = 1000;
    }

    #region Properties

    public bool PoeTrade { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the welcome screen should be shown.
    /// </summary>
    public bool ShowWelcome { get; set; }

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
    public bool IncomingTradeEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [outgoing trade enabled].
    /// </summary>
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
    /// Gets or sets a value indicating whether [alert enabled].
    /// </summary>
    public bool ItemAlertEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [join hideout enabled].
    /// </summary>
    public bool JoinHideoutEnabled { get; set; }

    /// <summary>
    /// Gets or sets the alert volume.
    /// </summary>
    public float AlertVolume { get; set; }

    /// <summary>
    /// Gets or sets the alert volume.
    /// </summary>
    public float ItemAlertVolume { get; set; }

    /// <summary>
    /// Gets or sets the join hideout volume.
    /// </summary>
    public float JoinHideoutVolume { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [tool tip enabled].
    /// </summary>
    public bool ToolTipEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [clipboard enabled].
    /// </summary>
    public bool ClipboardEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [automatic kick].
    /// </summary>
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
    public int ToolTipDelay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [hide in background].
    /// </summary>
    public bool HideInBackground { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [hide in background].
    /// </summary>
    public bool DeleteItemEnabled { get; set; }

    /// <summary>
    /// Gets or sets the life foreground.
    /// </summary>
    public string LifeForeground { get; set; }

    /// <summary>
    /// Gets or sets the trade bar scaling.
    /// </summary>
    public double TradebarScaling { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show startup animation].
    /// </summary>
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
    public bool SoldDetection { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [show release note].
    /// </summary>
    public bool ShowReleaseNote { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [timeline enabled].
    /// </summary>
    public bool TimelineEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [build automatic close].
    /// </summary>
    public bool BuildAutoClose { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [group by skill].
    /// </summary>
    public bool GroupBySkill { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [synchronize build].
    /// </summary>
    public bool SyncBuild { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [hideout enabled].
    /// </summary>
    public bool HideoutEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [guild hideout enabled].
    /// </summary>
    public bool GuildHideoutEnabled { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether [guild hideout enabled].
    /// </summary>
    public string RecentLeagueName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether we ignore already sold item.
    /// </summary>
    public bool IgnoreAlreadySold { get; set; }

    /// <summary>
    /// Gets or sets a value indicating the delay to close outgoing trades.
    /// </summary>
    public double OutgoingDelayToClose { get; set; }

    /// <summary>
    /// Gets or sets a value indicating if the user tried to connect with patreon.
    /// </summary>
    public bool ConnectedToPatreon { get; set; }

    #endregion
}