//-----------------------------------------------------------------------
// <copyright file="UpdateManager.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Helpers;

using System.Threading.Tasks;
using PoeLurker.Core;
using PoeLurker.Core.Services;
using Velopack;
using Velopack.Sources;

//using Squirrel;

/// <summary>
/// Represents the update manager.
/// </summary>
public class PoeLurkerUpdateManager
{
    #region Fields

    private static readonly string PoeLukerGithubUrl = "https://github.com/C1rdec/Poe-Lurker";

    private readonly UpdateManager _updateManager = new(new GithubSource(PoeLukerGithubUrl, string.Empty, false));
    private readonly SettingsService _settingsService;
    private readonly ClipboardLurker _clipboardLurker;
    private readonly ClientLurker _clientLurker;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateManager" /> class.
    /// </summary>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="clipboardLurker">The clipboard lurker.</param>
    /// <param name="clientLurker">The client lurker.</param>
    public PoeLurkerUpdateManager(SettingsService settingsService, ClipboardLurker clipboardLurker, ClientLurker clientLurker)
    {
        _clipboardLurker = clipboardLurker;
        _clientLurker = clientLurker;
        _settingsService = settingsService;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Updates this instance.
    /// </summary>
    /// <returns>The task awaiter.</returns>
    public async Task Update()
    {
        _settingsService.FirstLaunch = true;
        _settingsService.Save();

        if (_clipboardLurker != null)
        {
            _clipboardLurker.Dispose();
        }

        if (_clientLurker != null)
        {
            _clientLurker.Dispose();
        }

        var newVersion = await _updateManager.CheckForUpdatesAsync();
        if (newVersion == null)
        {
            return;
        }

        await _updateManager.DownloadUpdatesAsync(newVersion);
        _updateManager.ApplyUpdatesAndRestart(newVersion);
    }

    /// <summary>
    /// Updates this instance.
    /// </summary>
    /// <returns>True if needs update.</returns>
    public async Task<bool> CheckForUpdate()
    {
#if DEBUG
        return false;
#endif

#pragma warning disable CS0162
        try
        {
            var newVersion = await _updateManager.CheckForUpdatesAsync();

            return true;
        }
        catch
        {
            return false;
        }
#pragma warning restore CS0162
    }

    #endregion
}