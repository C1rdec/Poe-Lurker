//-----------------------------------------------------------------------
// <copyright file="HotkeySettings.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Models;

using System.ComponentModel;
using Winook;

/// <summary>
/// Represents the key settings.
/// </summary>
public sealed class HotkeySettings
{
    public HotkeySettings()
    {
        Main = new();
        Busy = new();
        Whisper = new();
        Dismiss = new();
        StillInterested = new();
        OpenWiki = new();
        JoinGuildHideout = new();
        JoinHideout = new();
        SearchItem = new();
        RemainingMonster = new();
    }

    #region Properties

    /// <summary>
    /// Gets or sets the toggle build.
    /// </summary>
    [DefaultValue(KeyCode.OemQuotes)]
    public KeyCode ToggleBuild { get; set; }

    /// <summary>
    /// Gets or sets the main.
    /// </summary>
    public Hotkey Main { get; set; }

    /// <summary>
    /// Gets or sets the busy.
    /// </summary>
    public Hotkey Busy { get; set; }

    /// <summary>
    /// Gets or sets the whisper.
    /// </summary>
    public Hotkey Whisper { get; set; }

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
    /// Gets or sets the join guild hideout.
    /// </summary>
    public Hotkey JoinHideout { get; set; }

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