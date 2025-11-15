//-----------------------------------------------------------------------
// <copyright file="HelpViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using Caliburn.Micro;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;
using ProcessLurker;

/// <summary>
/// Represents the HelpViewModel.
/// </summary>
/// <seealso cref="PoeLurker.UI.ViewModels.PoeOverlayBase" />
public class HelpViewModel : PoeOverlayBase
{
    #region Fields

    private static readonly int DefaultSize = 60;
    private System.Action _onClick;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="HelpViewModel"/> class.
    /// </summary>
    /// <param name="windowManager">The window manager.</param>
    /// <param name="dockingHelper">The docking helper.</param>
    /// <param name="processLurker">The process lurker.</param>
    /// <param name="settingsService">The settings service.</param>
    public HelpViewModel(DockingHelper dockingHelper, ProcessService processLurker, SettingsService settingsService)
        : base(dockingHelper, processLurker, settingsService)
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Helps this instance.
    /// </summary>
    public void Help()
    {
        _onClick?.Invoke();
    }

    /// <summary>
    /// Helps this instance.
    /// </summary>
    /// <param name="onClick">The on click.</param>
    public void Initialize(System.Action onClick)
    {
        _onClick = onClick;
    }

    /// <summary>
    /// Sets the window position.
    /// </summary>
    /// <param name="windowInformation">The window information.</param>
    protected override void SetWindowPosition(PoeWindowInformation windowInformation)
    {
        var value = DefaultSize * windowInformation.Height / 1080;

        Execute.OnUIThread(() =>
        {
            View.Height = ApplyAbsoluteScalingY(value);
            View.Width = ApplyAbsoluteScalingX(value);
            View.Left = ApplyScalingX(windowInformation.Position.Right - value);
            View.Top = ApplyScalingY(windowInformation.Position.Bottom - value);
        });
    }

    #endregion
}