//-----------------------------------------------------------------------
// <copyright file="HideoutViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using Caliburn.Micro;
using PoeLurker.Core;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;
using ProcessLurker;

/// <summary>
/// Represents the Hideout overlay.
/// </summary>
/// <seealso cref="PoeLurker.UI.ViewModels.PoeOverlayBase" />
public class HideoutViewModel : PoeOverlayBase
{
    #region Fields

    private static readonly int DefaultSize = 60;
    private readonly PoeKeyboardHelper _keyboardHelper;
    private bool _isVisible;
    private bool _guildVisible;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="HideoutViewModel" /> class.
    /// </summary>
    /// <param name="windowManager">The window manager.</param>
    /// <param name="dockingHelper">The docking helper.</param>
    /// <param name="processLurker">The process lurker.</param>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="keyboardHelper">The keyboard helper.</param>
    /// <param name="clientLurker">The client lurker.</param>
    public HideoutViewModel(IWindowManager windowManager, DockingHelper dockingHelper, ProcessService processLurker, SettingsService settingsService, PoeKeyboardHelper keyboardHelper, ClientLurker clientLurker)
        : base(windowManager, dockingHelper, processLurker, settingsService)
    {
        _keyboardHelper = keyboardHelper;
        IsVisible = true;
        GuildVisible = SettingsService.GuildHideoutEnabled;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether this instance is visible.
    /// </summary>
    public bool IsVisible
    {
        get
        {
            return _isVisible;
        }

        private set
        {
            _isVisible = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance is visible.
    /// </summary>
    public bool GuildVisible
    {
        get
        {
            return _guildVisible;
        }

        private set
        {
            _guildVisible = value;
            NotifyOfPropertyChange();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Joins the Guild hideout.
    /// </summary>
    public async void JoinGuildHideout()
    {
        await _keyboardHelper.JoinGuildHideout();
    }

    /// <summary>
    /// Joins the hideout.
    /// </summary>
    public async void JoinHideout()
    {
        await _keyboardHelper.JoinHideout();
    }

    /// <summary>
    /// Sets the window position.
    /// </summary>
    /// <param name="windowInformation">The window information.</param>
    protected override void SetWindowPosition(PoeWindowInformation windowInformation)
    {
        var value = DefaultSize * windowInformation.Height / 1080;
        var margin = SettingsService.CenteredUI && windowInformation.WideScreen ? windowInformation.FlaskBarWidth * 0.8 : 0;
        Execute.OnUIThread(() =>
        {
            View.Height = ApplyAbsoluteScalingY(value);
            View.Width = ApplyAbsoluteScalingX(value);
            View.Left = ApplyScalingX(windowInformation.Position.Left + margin);
            View.Top = ApplyScalingY(windowInformation.Position.Bottom - value);
        });
    }

    /// <summary>
    /// On activate.
    /// </summary>
    protected override Task OnActivateAsync(CancellationToken token)
    {
        SettingsService.OnSave += SettingsService_OnSave;

        return base.OnActivateAsync(token);
    }

    /// <summary>
    /// On Deactivate.
    /// </summary>
    /// <param name="close">If closing.</param>
    protected override Task OnDeactivateAsync(bool close, CancellationToken token)
    {
        SettingsService.OnSave -= SettingsService_OnSave;

        return base.OnDeactivateAsync(close, token);
    }

    private void SettingsService_OnSave(object sender, System.EventArgs e)
    {
        GuildVisible = SettingsService.GuildHideoutEnabled;
    }

    #endregion
}