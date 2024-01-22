//-----------------------------------------------------------------------
// <copyright file="HotkeyService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Core.Services;

using Lurker.Core.Models;

/// <summary>
/// Represents Hotkey service.
/// </summary>
/// <seealso cref="Lurker.Core.Services.ServiceBase" />
public class HotkeyService
{
    #region Fields

    private readonly HotkeySettings _settings;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="HotkeyService"/> class.
    /// </summary>
    public HotkeyService()
    {
        _settings = new HotkeySettings();
        _settings.Initialize();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the toggle build.
    /// </summary>
    public ushort ToggleBuild
    {
        get
        {
            return _settings.ToggleBuild;
        }

        set
        {
            _settings.ToggleBuild = value;
        }
    }

    /// <summary>
    /// Gets or sets the main.
    /// </summary>
    public Hotkey Main
    {
        get
        {
            return _settings.Main;
        }

        set
        {
            _settings.Main = value;
        }
    }

    /// <summary>
    /// Gets or sets the open wiki.
    /// </summary>
    public Hotkey OpenWiki
    {
        get
        {
            return _settings.OpenWiki;
        }

        set
        {
            _settings.OpenWiki = value;
        }
    }

    /// <summary>
    /// Gets or sets the join guild hideout.
    /// </summary>
    public Hotkey JoinGuildHideout
    {
        get
        {
            return _settings.JoinGuildHideout;
        }

        set
        {
            _settings.JoinGuildHideout = value;
        }
    }

    /// <summary>
    /// Gets or sets the join hideout.
    /// </summary>
    public Hotkey JoinHideout
    {
        get
        {
            return _settings.JoinHideout;
        }

        set
        {
            _settings.JoinHideout = value;
        }
    }

    /// <summary>
    /// Gets or sets the search item.
    /// </summary>
    public Hotkey SearchItem
    {
        get
        {
            return _settings.SearchItem;
        }

        set
        {
            _settings.SearchItem = value;
        }
    }

    /// <summary>
    /// Gets or sets the search item.
    /// </summary>
    public Hotkey RemainingMonster
    {
        get
        {
            return _settings.RemainingMonster;
        }

        set
        {
            _settings.RemainingMonster = value;
        }
    }

    /// <summary>
    /// Gets or sets the busy.
    /// </summary>
    public Hotkey Busy
    {
        get
        {
            return _settings.Busy;
        }

        set
        {
            _settings.Busy = value;
        }
    }

    /// <summary>
    /// Gets or sets the whisper.
    /// </summary>
    public Hotkey Whisper
    {
        get
        {
            return _settings.Whisper;
        }

        set
        {
            _settings.Whisper = value;
        }
    }

    /// <summary>
    /// Gets or sets the dismiss.
    /// </summary>
    public Hotkey Dismiss
    {
        get
        {
            return _settings.Dismiss;
        }

        set
        {
            _settings.Dismiss = value;
        }
    }

    /// <summary>
    /// Gets or sets the still interested.
    /// </summary>
    public Hotkey StillInterested
    {
        get
        {
            return _settings.StillInterested;
        }

        set
        {
            _settings.StillInterested = value;
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