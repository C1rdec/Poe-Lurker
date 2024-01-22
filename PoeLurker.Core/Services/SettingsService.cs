//-----------------------------------------------------------------------
// <copyright file="SettingsService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Services;

using System;
using System.IO;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Models;

/// <summary>
/// The settings service.
/// </summary>
public class SettingsService
{
    #region Fields

    private static readonly string DefaultStillInterestedMessage = $"Are you still interested in my {TokenHelper.ItemName} listed for {TokenHelper.Price}?";
    private static readonly string DefaultSoldMessage = $"I'm sorry, my {TokenHelper.ItemName} has already been sold.";
    private static readonly string DefaultBusyMessage = "I'm busy right now I'll send you a party invite.";
    private static readonly string DefaultThankYouMessage = string.Empty;

    private readonly Settings _settings;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsService"/> class.
    /// </summary>
    public SettingsService()
    {
        _settings = new Settings();
        _settings.Initialize();

        if (string.IsNullOrEmpty(_settings.UserId))
        {
            _settings.UserId = Guid.NewGuid().ToString();
            Save();
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when [on save].
    /// </summary>
    public event EventHandler OnSave;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the user identifier.
    /// </summary>
    public string UserId => _settings.UserId;

    /// <summary>
    /// Gets or sets the busy message.
    /// </summary>
    public string BusyMessage
    {
        get
        {
            return _settings.BusyMessage;
        }

        set
        {
            _settings.BusyMessage = value;
        }
    }

    /// <summary>
    /// Gets or sets the sold message.
    /// </summary>
    public string SoldMessage
    {
        get
        {
            return _settings.SoldMessage;
        }

        set
        {
            _settings.SoldMessage = value;
        }
    }

    /// <summary>
    /// Gets or sets the thank you message.
    /// </summary>
    public string ThankYouMessage
    {
        get
        {
            return _settings.ThankYouMessage;
        }

        set
        {
            _settings.ThankYouMessage = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [first launch].
    /// </summary>
    public bool FirstLaunch
    {
        get
        {
            return _settings.FirstLaunch;
        }

        set
        {
            _settings.FirstLaunch = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the welcome scrren needs to be shown.
    /// </summary>
    public bool ShowWelcome
    {
        get
        {
            return _settings.ShowWelcome;
        }

        set
        {
            _settings.ShowWelcome = value;
        }
    }

    /// <summary>
    /// Gets or sets the sill interested message.
    /// </summary>
    public string StillInterestedMessage
    {
        get
        {
            return _settings.StillInterestedMessage;
        }

        set
        {
            _settings.StillInterestedMessage = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [incoming trade enabled].
    /// </summary>
    public bool IncomingTradeEnabled
    {
        get
        {
            return _settings.IncomingTradeEnabled;
        }

        set
        {
            _settings.IncomingTradeEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [outgoing trade enabled].
    /// </summary>
    public bool OutgoingTradeEnabled
    {
        get
        {
            return _settings.OutgoingTradeEnabled;
        }

        set
        {
            _settings.OutgoingTradeEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [search enabled].
    /// </summary>
    public bool SearchEnabled
    {
        get
        {
            return _settings.SearchEnabled;
        }

        set
        {
            _settings.SearchEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [map enabled].
    /// </summary>
    public bool MapEnabled
    {
        get
        {
            return _settings.MapEnabled;
        }

        set
        {
            _settings.MapEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [debug enabled].
    /// </summary>
    public bool DebugEnabled
    {
        get
        {
            return _settings.DebugEnabled;
        }

        set
        {
            _settings.DebugEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [alert enabled].
    /// </summary>
    public bool AlertEnabled
    {
        get
        {
            return _settings.AlertEnabled;
        }

        set
        {
            _settings.AlertEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [alert enabled].
    /// </summary>
    public bool ItemAlertEnabled
    {
        get
        {
            return _settings.ItemAlertEnabled;
        }

        set
        {
            _settings.ItemAlertEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [join hideout enabled].
    /// </summary>
    public bool JoinHideoutEnabled
    {
        get
        {
            return _settings.JoinHideoutEnabled;
        }

        set
        {
            _settings.JoinHideoutEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [tool tip enabled].
    /// </summary>
    public bool ToolTipEnabled
    {
        get
        {
            return _settings.ToolTipEnabled;
        }

        set
        {
            _settings.ToolTipEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [clipboard enabled].
    /// </summary>
    public bool ClipboardEnabled
    {
        get
        {
            return _settings.ClipboardEnabled;
        }

        set
        {
            _settings.ClipboardEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [automatic kick enabled].
    /// </summary>
    public bool AutoKickEnabled
    {
        get
        {
            return _settings.AutoKickEnabled;
        }

        set
        {
            _settings.AutoKickEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [dashboard enabled].
    /// </summary>
    public bool DashboardEnabled
    {
        get
        {
            return _settings.DashboardEnabled;
        }

        set
        {
            _settings.DashboardEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets the alert volume.
    /// </summary>
    public float AlertVolume
    {
        get
        {
            return _settings.AlertVolume;
        }

        set
        {
            _settings.AlertVolume = value;
        }
    }

    /// <summary>
    /// Gets or sets the alert volume.
    /// </summary>
    public float ItemAlertVolume
    {
        get
        {
            return _settings.ItemAlertVolume;
        }

        set
        {
            _settings.ItemAlertVolume = value;
        }
    }

    /// <summary>
    /// Gets or sets the join hideout volume.
    /// </summary>
    public float JoinHideoutVolume
    {
        get
        {
            return _settings.JoinHideoutVolume;
        }

        set
        {
            _settings.JoinHideoutVolume = value;
        }
    }

    /// <summary>
    /// Gets or sets the tool tip delay.
    /// </summary>
    public int ToolTipDelay
    {
        get
        {
            return _settings.ToolTipDelay;
        }

        set
        {
            _settings.ToolTipDelay = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [hide in background].
    /// </summary>
    public bool HideInBackground
    {
        get
        {
            return _settings.HideInBackground;
        }

        set
        {
            _settings.HideInBackground = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [delete item enabled].
    /// </summary>
    public bool DeleteItemEnabled
    {
        get
        {
            return _settings.DeleteItemEnabled;
        }

        set
        {
            _settings.DeleteItemEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets the life foreground.
    /// </summary>
    public string LifeForeground
    {
        get
        {
            return _settings.LifeForeground;
        }

        set
        {
            _settings.LifeForeground = value;
        }
    }

    /// <summary>
    /// Gets or sets the tradebar scaling.
    /// </summary>
    public double TradebarScaling
    {
        get
        {
            return _settings.TradebarScaling;
        }

        set
        {
            _settings.TradebarScaling = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [show startup animation].
    /// </summary>
    public bool ShowStartupAnimation
    {
        get
        {
            return _settings.ShowStartupAnimation;
        }

        set
        {
            _settings.ShowStartupAnimation = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [vulkan renderer].
    /// </summary>
    public bool VulkanRenderer
    {
        get
        {
            return _settings.VulkanRenderer;
        }

        set
        {
            _settings.VulkanRenderer = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [build helper].
    /// </summary>
    public bool BuildHelper
    {
        get
        {
            return _settings.BuildHelper;
        }

        set
        {
            _settings.BuildHelper = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [sold detection].
    /// </summary>
    public bool SoldDetection
    {
        get
        {
            return _settings.SoldDetection;
        }

        set
        {
            _settings.SoldDetection = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [show release note].
    /// </summary>
    public bool ShowReleaseNote
    {
        get
        {
            return _settings.ShowReleaseNote;
        }

        set
        {
            _settings.ShowReleaseNote = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [time line enabled].
    /// </summary>
    public bool TimelineEnabled
    {
        get
        {
            return _settings.TimelineEnabled;
        }

        set
        {
            _settings.TimelineEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [build automatic close].
    /// </summary>
    public bool BuildAutoClose
    {
        get
        {
            return _settings.BuildAutoClose;
        }

        set
        {
            _settings.BuildAutoClose = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [group by skill].
    /// </summary>
    public bool GroupBySkill
    {
        get
        {
            return _settings.GroupBySkill;
        }

        set
        {
            _settings.GroupBySkill = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [synchronize build].
    /// </summary>
    public bool SyncBuild
    {
        get
        {
            return _settings.SyncBuild;
        }

        set
        {
            _settings.SyncBuild = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [hideout enabled].
    /// </summary>
    public bool HideoutEnabled
    {
        get
        {
            return _settings.HideoutEnabled;
        }

        set
        {
            _settings.HideoutEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [hideout enabled].
    /// </summary>
    public bool GuildHideoutEnabled
    {
        get
        {
            return _settings.GuildHideoutEnabled;
        }

        set
        {
            _settings.GuildHideoutEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the recent league name.
    /// </summary>
    public string RecentLeagueName
    {
        get
        {
            return _settings.RecentLeagueName;
        }

        set
        {
            _settings.RecentLeagueName = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether we ignore already sold items.
    /// </summary>
    public bool IgnoreAlreadySold
    {
        get
        {
            return _settings.IgnoreAlreadySold;
        }

        set
        {
            _settings.IgnoreAlreadySold = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether we ignore already sold items.
    /// </summary>
    public double OutgoingDelayToClose
    {
        get
        {
            return _settings.OutgoingDelayToClose;
        }

        set
        {
            _settings.OutgoingDelayToClose = value;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Saves this instance.
    /// </summary>
    /// <param name="raiseEvent">if set to <c>true</c> [raise event].</param>
    public void Save(bool raiseEvent = true)
    {
        _settings.Save();
        if (raiseEvent)
        {
            OnSave?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion
}