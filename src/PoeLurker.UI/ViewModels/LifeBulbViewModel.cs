//-----------------------------------------------------------------------
// <copyright file="LifeBulbViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using Caliburn.Micro;
using PoeLurker.Core;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;
using PoeLurker.UI.Models;
using PoeLurker.UI.Views;
using ProcessLurker;

/// <summary>
/// Represents the life bulb viewmodel.
/// </summary>
/// <seealso cref="PoeLurker.UI.ViewModels.BulbViewModelBase" />
public class LifeBulbViewModel : BulbViewModelBase, IHandle<LifeBulbMessage>
{
    #region Fields

    private readonly IEventAggregator _eventAggregator;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="LifeBulbViewModel" /> class.
    /// </summary>
    /// <param name="eventAggregator">The event aggregator.</param>
    /// <param name="windowManager">The window manager.</param>
    /// <param name="dockingHelper">The docking helper.</param>
    /// <param name="processLurker">The process lurker.</param>
    /// <param name="clientLurker">The client lurker.</param>
    /// <param name="settingsService">The settings service.</param>
    public LifeBulbViewModel(IEventAggregator eventAggregator, IWindowManager windowManager, DockingHelper dockingHelper, ProcessService processLurker, ClientLurker clientLurker, SettingsService settingsService)
        : base(windowManager, dockingHelper, processLurker, settingsService, clientLurker)
    {
        _eventAggregator = eventAggregator;
        _eventAggregator.SubscribeOnPublishedThread(this);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether HasSubAction.
    /// </summary>
    public bool HasSubAction => SubAction != null;

    #endregion

    #region Methods

    /// <summary>
    /// Handles the SubAction.
    /// </summary>
    public void OnSubAction()
    {
        SubAction?.Invoke();
    }

    /// <summary>
    /// Handles the message.
    /// </summary>
    /// <param name="message">The message.</param>
    public Task HandleAsync(LifeBulbMessage message, CancellationToken cancellationToken)
    {
        SetAction(message);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Sets the window position.
    /// </summary>
    /// <param name="windowInformation">The window information.</param>
    protected override void SetWindowPosition(PoeWindowInformation windowInformation)
    {
        var value = DefaultBulbHeight * windowInformation.Height / 1080;
        var margin = SettingsService.CentredUI ? windowInformation.FlaskBarWidth * 0.8 : 0;

        Execute.OnUIThread(() =>
        {
            View.Height = ApplyAbsoluteScalingY(value);
            View.Width = ApplyAbsoluteScalingX(value);
            View.Left = ApplyScalingX(windowInformation.Position.Left + 10 + margin);
            View.Top = ApplyScalingY(windowInformation.Position.Bottom - value - 10);
            var lifeView = View as LifeBulbView;
            lifeView.ResizeLifeBulb();
        });
    }

    #endregion
}