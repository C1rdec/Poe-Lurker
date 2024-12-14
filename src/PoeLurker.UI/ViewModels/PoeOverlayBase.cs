//-----------------------------------------------------------------------
// <copyright file="PoeOverlayBase.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Caliburn.Micro;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;
using ProcessLurker;

/// <summary>
/// Represents a Poe Overlay.
/// </summary>
/// <seealso cref="PoeLurker.UI.ViewModels.Screen" />
/// <seealso cref="Caliburn.Micro.IViewAware" />
public abstract class PoeOverlayBase : Screen, IViewAware
{
    #region Fields

    private bool _manualHide;
    private double _scaleX;
    private double _scaleY;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PoeOverlayBase" /> class.
    /// </summary>
    /// <param name="windowManager">The window manager.</param>
    /// <param name="dockingHelper">The docking helper.</param>
    /// <param name="processLurker">The process lurker.</param>
    /// <param name="settingsService">The settings service.</param>
    public PoeOverlayBase(IWindowManager windowManager, DockingHelper dockingHelper, ProcessService processLurker, SettingsService settingsService)
    {
        DockingHelper = dockingHelper;
        ProcessLurker = processLurker;
        SettingsService = settingsService;

        // Default scale value
        _scaleY = 1;
        _scaleX = 1;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether [debug enabled].
    /// </summary>
    public bool DebugEnabled => SettingsService.DebugEnabled;

    /// <summary>
    /// Gets the margin.
    /// </summary>
    protected static int Margin => 4;

    /// <summary>
    /// Gets the default height of the flask bar.
    /// </summary>
    protected static int DefaultFlaskBarHeight => 122;

    /// <summary>
    /// Gets the default width of the flask bar.
    /// </summary>
    protected static int DefaultFlaskBarWidth => 550;

    /// <summary>
    /// Gets the default height of the exp bar.
    /// </summary>
    protected static int DefaultExpBarHeight => 24;

    /// <summary>
    /// Gets the default height.
    /// </summary>
    protected static int DefaultHeight => 1080;

    /// <summary>
    /// Gets the view.
    /// </summary>
    protected Window View { get; private set; }

    /// <summary>
    /// Gets the settings service.
    /// </summary>
    protected SettingsService SettingsService { get; private set; }

    /// <summary>
    /// Gets the process lurker.
    /// </summary>
    protected ProcessService ProcessLurker { get; private set; }

    /// <summary>
    /// Gets the docking helper.
    /// </summary>
    protected DockingHelper DockingHelper { get; private set; }

    #endregion

    #region Methods

    /// <summary>
    /// Applies the scaling x.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The scaled value.</returns>
    protected double ApplyAbsoluteScalingX(double value)
    {
        return Scale(value, _scaleX, true);
    }

    /// <summary>
    /// Applies the scaling x.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The scaled value.</returns>
    protected double ApplyScalingX(double value)
    {
        return Scale(value, _scaleX);
    }

    /// <summary>
    /// Applies the scalling y.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The scaled value.</returns>
    protected double ApplyAbsoluteScalingY(double value)
    {
        return Scale(value, _scaleY, true);
    }

    /// <summary>
    /// Applies the scalling y.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The scaled value.</returns>
    protected double ApplyScalingY(double value)
    {
        return Scale(value, _scaleY);
    }

    /// <summary>
    /// Hides the view.
    /// </summary>
    protected void HideView()
    {
        View?.Hide();
    }

    /// <summary>
    /// Shows the view.
    /// </summary>
    protected void ShowView()
    {
        if (_manualHide || View == null || !View.IsLoaded)
        {
            return;
        }

        View?.Show();
    }

    /// <summary>
    /// Shows the view.
    /// </summary>
    protected void SetInForeground()
    {
        if (View == null)
        {
            return;
        }

        Execute.OnUIThread(() =>
        {
            var handle = new WindowInteropHelper(View).Handle;
            DockingHelper.SetForeground(handle);
        });
    }

    /// <summary>
    /// Hides the view.
    /// </summary>
    /// <param name="time">The time.</param>
    protected async void HideView(int time)
    {
        HideView();
        _manualHide = true;
        await Task.Delay(time);
        _manualHide = false;
        ShowView();
    }

    /// <summary>
    /// Scale the value.
    /// </summary>
    /// <param name="value">The value to scale.</param>
    /// <param name="scale">The scale factor.</param>
    /// <returns>The scaled value.</returns>
    private static double Scale(double value, double scale)
    {
        return Scale(value, scale, false);
    }

    /// <summary>
    /// Scale the value.
    /// </summary>
    /// <param name="value">The value to scale.</param>
    /// <param name="scale">The scale factor.</param>
    /// <param name="absolute">If the value need to be positive.</param>
    /// <returns>The scaled value.</returns>
    private static double Scale(double value, double scale, bool absolute)
    {
        var current = absolute ? Math.Abs(value) : value;

        return current / scale;
    }

    /// <summary>
    /// Dockings the helper on foreground change.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">if set to <c>true</c> [e].</param>
    private void DockingHelper_OnForegroundChange(object sender, bool inForegound)
    {
        Execute.OnUIThread(() =>
        {
            if (inForegound)
            {
                ShowView();
            }
            else
            {
                HideView();
            }
        });
    }

    /// <summary>
    /// Handles the OnSave event of the SettingsService control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void SettingsService_OnSave(object sender, EventArgs e)
    {
        NotifyOfPropertyChange(nameof(DebugEnabled));
    }

    /// <summary>
    /// Handles the OnWindowMove event of the DockingHelper control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="information">The information.</param>
    private void DockingHelper_OnWindowMove(object sender, PoeWindowInformation information)
    {
        if (View != null)
        {
            SetWindowPosition(information);
        }
    }

    /// <summary>
    /// Handles the PoeEnded event of the Lurker control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void Lurker_PoeClosed(object sender, EventArgs e)
    {
        TryCloseAsync();
    }

    /// <summary>
    /// Called when an attached view's Loaded event fires.
    /// </summary>
    /// <param name="view">The view.</param>
    protected override void OnViewLoaded(object view)
    {
        View = view as Window;
        View.ShowActivated = false;

        var source = PresentationSource.FromVisual(View);
        if (source != null)
        {
            _scaleX = source.CompositionTarget.TransformToDevice.M11;
            _scaleY = source.CompositionTarget.TransformToDevice.M22;
        }

        SetWindowPosition(DockingHelper.WindowInformation);
    }

    /// <summary>
    /// Sets the window position.
    /// </summary>
    /// <param name="windowInformation">The window information.</param>
    protected abstract void SetWindowPosition(PoeWindowInformation windowInformation);

    /// <summary>
    /// Called when activating.
    /// </summary>
    protected override Task OnActivateAsync(CancellationToken token)
    {
        ProcessLurker.ProcessClosed += Lurker_PoeClosed;
        SettingsService.OnSave += SettingsService_OnSave;

        if (DockingHelper != null)
        {
            DockingHelper.OnWindowMove += DockingHelper_OnWindowMove;
            DockingHelper.OnForegroundChange += DockingHelper_OnForegroundChange;
        }

        return base.OnActivateAsync(token);
    }

    /// <summary>
    /// Called when deactivating.
    /// </summary>
    /// <param name="close">Inidicates whether this instance will be closed.</param>
    protected override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
    {
        if (close)
        {
            ProcessLurker.ProcessClosed -= Lurker_PoeClosed;
            SettingsService.OnSave -= SettingsService_OnSave;
            DockingHelper.OnWindowMove -= DockingHelper_OnWindowMove;

            if (DockingHelper != null)
            {
                DockingHelper.OnWindowMove -= DockingHelper_OnWindowMove;
                DockingHelper.OnForegroundChange -= DockingHelper_OnForegroundChange;
            }
        }

        return base.OnDeactivateAsync(close, cancellationToken);
    }

    #endregion
}