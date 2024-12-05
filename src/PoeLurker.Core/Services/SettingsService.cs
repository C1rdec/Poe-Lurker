//-----------------------------------------------------------------------
// <copyright file="SettingsService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Services;

using System;
using PoeLurker.Core.Models;

/// <summary>
/// The settings service.
/// </summary>
public class SettingsService
{
    #region Fields

    private readonly SettingsFile _settings;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsService"/> class.
    /// </summary>
    public SettingsService()
    {
        _settings = new SettingsFile();
        _settings.Initialize();

        if (string.IsNullOrEmpty(_settings.Entity.UserId))
        {
            _settings.Entity.UserId = Guid.NewGuid().ToString();
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
    public string UserId => _settings.Entity.UserId;

    /// <summary>
    /// Gets or sets the busy message.
    /// </summary>
    public string BusyMessage
    {
        get
        {
            return _settings.Entity.BusyMessage;
        }

        set
        {
            _settings.Entity.BusyMessage = value;
        }
    }

    /// <summary>
    /// Gets or sets the sold message.
    /// </summary>
    public string SoldMessage
    {
        get
        {
            return _settings.Entity.SoldMessage;
        }

        set
        {
            _settings.Entity.SoldMessage = value;
        }
    }

    /// <summary>
    /// Gets or sets the thank you message.
    /// </summary>
    public string ThankYouMessage
    {
        get
        {
            return _settings.Entity.ThankYouMessage;
        }

        set
        {
            _settings.Entity.ThankYouMessage = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [first launch].
    /// </summary>
    public bool FirstLaunch
    {
        get
        {
            return _settings.Entity.FirstLaunch;
        }

        set
        {
            _settings.Entity.FirstLaunch = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the welcome scrren needs to be shown.
    /// </summary>
    public bool ShowWelcome
    {
        get
        {
            return _settings.Entity.ShowWelcome;
        }

        set
        {
            _settings.Entity.ShowWelcome = value;
        }
    }

    /// <summary>
    /// Gets or sets the sill interested message.
    /// </summary>
    public string StillInterestedMessage
    {
        get
        {
            return _settings.Entity.StillInterestedMessage;
        }

        set
        {
            _settings.Entity.StillInterestedMessage = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [incoming trade enabled].
    /// </summary>
    public bool IncomingTradeEnabled
    {
        get
        {
            return _settings.Entity.IncomingTradeEnabled;
        }

        set
        {
            _settings.Entity.IncomingTradeEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [outgoing trade enabled].
    /// </summary>
    public bool OutgoingTradeEnabled
    {
        get
        {
            return _settings.Entity.OutgoingTradeEnabled;
        }

        set
        {
            _settings.Entity.OutgoingTradeEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [search enabled].
    /// </summary>
    public bool SearchEnabled
    {
        get
        {
            return _settings.Entity.SearchEnabled;
        }

        set
        {
            _settings.Entity.SearchEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [map enabled].
    /// </summary>
    public bool MapEnabled
    {
        get
        {
            return _settings.Entity.MapEnabled;
        }

        set
        {
            _settings.Entity.MapEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [debug enabled].
    /// </summary>
    public bool DebugEnabled
    {
        get
        {
            return _settings.Entity.DebugEnabled;
        }

        set
        {
            _settings.Entity.DebugEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [alert enabled].
    /// </summary>
    public bool AlertEnabled
    {
        get
        {
            return _settings.Entity.AlertEnabled;
        }

        set
        {
            _settings.Entity.AlertEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [alert enabled].
    /// </summary>
    public bool ItemAlertEnabled
    {
        get
        {
            return _settings.Entity.ItemAlertEnabled;
        }

        set
        {
            _settings.Entity.ItemAlertEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [join hideout enabled].
    /// </summary>
    public bool JoinHideoutEnabled
    {
        get
        {
            return _settings.Entity.JoinHideoutEnabled;
        }

        set
        {
            _settings.Entity.JoinHideoutEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [tool tip enabled].
    /// </summary>
    public bool ToolTipEnabled
    {
        get
        {
            return _settings.Entity.ToolTipEnabled;
        }

        set
        {
            _settings.Entity.ToolTipEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [clipboard enabled].
    /// </summary>
    public bool ClipboardEnabled
    {
        get
        {
            return _settings.Entity.ClipboardEnabled;
        }

        set
        {
            _settings.Entity.ClipboardEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [automatic kick enabled].
    /// </summary>
    public bool AutoKickEnabled
    {
        get
        {
            return _settings.Entity.AutoKickEnabled;
        }

        set
        {
            _settings.Entity.AutoKickEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [dashboard enabled].
    /// </summary>
    public bool DashboardEnabled
    {
        get
        {
            return _settings.Entity.DashboardEnabled;
        }

        set
        {
            _settings.Entity.DashboardEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets the alert volume.
    /// </summary>
    public float AlertVolume
    {
        get
        {
            return _settings.Entity.AlertVolume;
        }

        set
        {
            _settings.Entity.AlertVolume = value;
        }
    }

    /// <summary>
    /// Gets or sets the alert volume.
    /// </summary>
    public float ItemAlertVolume
    {
        get
        {
            return _settings.Entity.ItemAlertVolume;
        }

        set
        {
            _settings.Entity.ItemAlertVolume = value;
        }
    }

    /// <summary>
    /// Gets or sets the join hideout volume.
    /// </summary>
    public float JoinHideoutVolume
    {
        get
        {
            return _settings.Entity.JoinHideoutVolume;
        }

        set
        {
            _settings.Entity.JoinHideoutVolume = value;
        }
    }

    /// <summary>
    /// Gets or sets the tool tip delay.
    /// </summary>
    public int ToolTipDelay
    {
        get
        {
            return _settings.Entity.ToolTipDelay;
        }

        set
        {
            _settings.Entity.ToolTipDelay = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [hide in background].
    /// </summary>
    public bool HideInBackground
    {
        get
        {
            return _settings.Entity.HideInBackground;
        }

        set
        {
            _settings.Entity.HideInBackground = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [delete item enabled].
    /// </summary>
    public bool DeleteItemEnabled
    {
        get
        {
            return _settings.Entity.DeleteItemEnabled;
        }

        set
        {
            _settings.Entity.DeleteItemEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets the life foreground.
    /// </summary>
    public string LifeForeground
    {
        get
        {
            return _settings.Entity.LifeForeground;
        }

        set
        {
            _settings.Entity.LifeForeground = value;
        }
    }

    /// <summary>
    /// Gets or sets the tradebar scaling.
    /// </summary>
    public double TradebarScaling
    {
        get
        {
            return _settings.Entity.TradebarScaling;
        }

        set
        {
            _settings.Entity.TradebarScaling = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [show startup animation].
    /// </summary>
    public bool ShowStartupAnimation
    {
        get
        {
            return _settings.Entity.ShowStartupAnimation;
        }

        set
        {
            _settings.Entity.ShowStartupAnimation = value;
        }
    }

    public bool ConnectedToPatreon
    {
        get
        {
            return _settings.Entity.ConnectedToPatreon;
        }

        set
        {
            _settings.Entity.ConnectedToPatreon = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [vulkan renderer].
    /// </summary>
    public bool VulkanRenderer
    {
        get
        {
            return _settings.Entity.VulkanRenderer;
        }

        set
        {
            _settings.Entity.VulkanRenderer = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [build helper].
    /// </summary>
    public bool BuildHelper
    {
        get
        {
            return _settings.Entity.BuildHelper;
        }

        set
        {
            _settings.Entity.BuildHelper = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [sold detection].
    /// </summary>
    public bool SoldDetection
    {
        get
        {
            return _settings.Entity.SoldDetection;
        }

        set
        {
            _settings.Entity.SoldDetection = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [show release note].
    /// </summary>
    public bool ShowReleaseNote
    {
        get
        {
            return _settings.Entity.ShowReleaseNote;
        }

        set
        {
            _settings.Entity.ShowReleaseNote = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [time line enabled].
    /// </summary>
    public bool TimelineEnabled
    {
        get
        {
            return _settings.Entity.TimelineEnabled;
        }

        set
        {
            _settings.Entity.TimelineEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [build automatic close].
    /// </summary>
    public bool BuildAutoClose
    {
        get
        {
            return _settings.Entity.BuildAutoClose;
        }

        set
        {
            _settings.Entity.BuildAutoClose = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [group by skill].
    /// </summary>
    public bool GroupBySkill
    {
        get
        {
            return _settings.Entity.GroupBySkill;
        }

        set
        {
            _settings.Entity.GroupBySkill = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [synchronize build].
    /// </summary>
    public bool SyncBuild
    {
        get
        {
            return _settings.Entity.SyncBuild;
        }

        set
        {
            _settings.Entity.SyncBuild = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [hideout enabled].
    /// </summary>
    public bool HideoutEnabled
    {
        get
        {
            return _settings.Entity.HideoutEnabled;
        }

        set
        {
            _settings.Entity.HideoutEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [hideout enabled].
    /// </summary>
    public bool GuildHideoutEnabled
    {
        get
        {
            return _settings.Entity.GuildHideoutEnabled;
        }

        set
        {
            _settings.Entity.GuildHideoutEnabled = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating the recent league name.
    /// </summary>
    public string RecentLeagueName
    {
        get
        {
            return _settings.Entity.RecentLeagueName;
        }

        set
        {
            _settings.Entity.RecentLeagueName = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether we ignore already sold items.
    /// </summary>
    public bool IgnoreAlreadySold
    {
        get
        {
            return _settings.Entity.IgnoreAlreadySold;
        }

        set
        {
            _settings.Entity.IgnoreAlreadySold = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether we ignore already sold items.
    /// </summary>
    public double OutgoingDelayToClose
    {
        get
        {
            return _settings.Entity.OutgoingDelayToClose;
        }

        set
        {
            _settings.Entity.OutgoingDelayToClose = value;
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