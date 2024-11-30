//-----------------------------------------------------------------------
// <copyright file="HotkeyService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Services;

using PoeLurker.Core.Models;
using Winook;

/// <summary>
/// Represents Hotkey service.
/// </summary>
/// <seealso cref="PoeLurker.Core.Services.ServiceBase" />
public class HotkeyService
{
    #region Fields

    private readonly HotkeySettingsFile _settings;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="HotkeyService"/> class.
    /// </summary>
    public HotkeyService()
    {
        _settings = new HotkeySettingsFile();
        _settings.Initialize();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the toggle build.
    /// </summary>
    public KeyCode ToggleBuild
    {
        get
        {
            return _settings.Entity.ToggleBuild;
        }

        set
        {
            _settings.Entity.ToggleBuild = value;
        }
    }

    /// <summary>
    /// Gets or sets the main.
    /// </summary>
    public Hotkey Main
    {
        get
        {
            return _settings.Entity.Main;
        }

        set
        {
            _settings.Entity.Main = value;
        }
    }

    /// <summary>
    /// Gets or sets the open wiki.
    /// </summary>
    public Hotkey OpenWiki
    {
        get
        {
            return _settings.Entity.OpenWiki;
        }

        set
        {
            _settings.Entity.OpenWiki = value;
        }
    }

    /// <summary>
    /// Gets or sets the join guild hideout.
    /// </summary>
    public Hotkey JoinGuildHideout
    {
        get
        {
            return _settings.Entity.JoinGuildHideout;
        }

        set
        {
            _settings.Entity.JoinGuildHideout = value;
        }
    }

    /// <summary>
    /// Gets or sets the join hideout.
    /// </summary>
    public Hotkey JoinHideout
    {
        get
        {
            return _settings.Entity.JoinHideout;
        }

        set
        {
            _settings.Entity.JoinHideout = value;
        }
    }

    /// <summary>
    /// Gets or sets the search item.
    /// </summary>
    public Hotkey SearchItem
    {
        get
        {
            return _settings.Entity.SearchItem;
        }

        set
        {
            _settings.Entity.SearchItem = value;
        }
    }

    /// <summary>
    /// Gets or sets the search item.
    /// </summary>
    public Hotkey RemainingMonster
    {
        get
        {
            return _settings.Entity.RemainingMonster;
        }

        set
        {
            _settings.Entity.RemainingMonster = value;
        }
    }

    /// <summary>
    /// Gets or sets the busy.
    /// </summary>
    public Hotkey Busy
    {
        get
        {
            return _settings.Entity.Busy;
        }

        set
        {
            _settings.Entity.Busy = value;
        }
    }

    /// <summary>
    /// Gets or sets the whisper.
    /// </summary>
    public Hotkey Whisper
    {
        get
        {
            return _settings.Entity.Whisper;
        }

        set
        {
            _settings.Entity.Whisper = value;
        }
    }

    /// <summary>
    /// Gets or sets the dismiss.
    /// </summary>
    public Hotkey Dismiss
    {
        get
        {
            return _settings.Entity.Dismiss;
        }

        set
        {
            _settings.Entity.Dismiss = value;
        }
    }

    /// <summary>
    /// Gets or sets the still interested.
    /// </summary>
    public Hotkey StillInterested
    {
        get
        {
            return _settings.Entity.StillInterested;
        }

        set
        {
            _settings.Entity.StillInterested = value;
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Saves the specified raise event.
    /// </summary>
    public void Save()
    {
        _settings.Save();
    }

    #endregion
}