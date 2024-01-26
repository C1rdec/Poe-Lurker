//-----------------------------------------------------------------------
// <copyright file="ManaBulbViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using Caliburn.Micro;
using PoeLurker.Core;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;
using PoeLurker.UI.Models;
using ProcessLurker;

/// <summary>
/// Represents the Manabulbviewmodel.
/// </summary>
public class ManaBulbViewModel : BulbViewModelBase, IHandle<ManaBulbMessage>
{
    #region Fields

    private readonly IEventAggregator _eventAggregator;
    private bool _updateRequired;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ManaBulbViewModel" /> class.
    /// </summary>
    /// <param name="eventAggregator">The event aggregator.</param>
    /// <param name="windowManager">The window manager.</param>
    /// <param name="dockingHelper">The docking helper.</param>
    /// <param name="clientLurker">The client lurker.</param>
    /// <param name="processLurker">The process lurker.</param>
    /// <param name="settingsService">The settings service.</param>
    /// H
    public ManaBulbViewModel(
        IEventAggregator eventAggregator,
        IWindowManager windowManager,
        DockingHelper dockingHelper,
        ClientLurker clientLurker,
        ProcessService processLurker,
        SettingsService settingsService)
        : base(windowManager, dockingHelper, processLurker, settingsService, clientLurker)
    {
        _eventAggregator = eventAggregator;
        _eventAggregator.SubscribeOnPublishedThread(this);

        ClientLurker.LocationChanged += Lurker_LocationChanged;
        SettingsService.OnSave += SettingsService_OnSave;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the default action.
    /// </summary>
    protected override System.Action DefaultAction
    {
        get
        {
            if (!SettingsService.DashboardEnabled)
            {
                return null;
            }

            return () => { };
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handles the message.
    /// </summary>
    /// <param name="message">The message.</param>
    public Task HandleAsync(ManaBulbMessage message, CancellationToken token)
    {
        if (message.NeedToHide)
        {
            HideView(8000);

            return Task.CompletedTask;
        }

        if (_updateRequired && message.IsUpdate)
        {
            base.SetAction(message);
            return Task.CompletedTask;
        }

        SetAction(message);

        if (message.IsUpdate)
        {
            _updateRequired = true;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Sets the window position.
    /// </summary>
    /// <param name="windowInformation">The wndow information.</param>
    protected override void SetWindowPosition(PoeWindowInformation windowInformation)
    {
        var value = DefaultBulbHeight * windowInformation.Height / 1080;
        Execute.OnUIThread(() =>
        {
            View.Height = ApplyAbsoluteScalingY(value);
            View.Width = ApplyAbsoluteScalingX(value);
            View.Left = ApplyScalingX(windowInformation.Position.Right - value);
            View.Top = ApplyScalingY(windowInformation.Position.Bottom - value);
        });
    }

    /// <summary>
    /// Called when deactivating.
    /// </summary>
    /// <param name="close">Inidicates whether this instance will be closed.</param>
    protected override Task OnDeactivateAsync(bool close, CancellationToken token)
    {
        if (close)
        {
            ClientLurker.LocationChanged -= Lurker_LocationChanged;
            SettingsService.OnSave -= SettingsService_OnSave;
            _eventAggregator.Unsubscribe(this);
        }

        return base.OnDeactivateAsync(close, token);
    }

    /// <summary>
    /// Sets the action.
    /// </summary>
    /// <param name="message">The message.</param>
    protected override void SetAction(BulbMessage message)
    {
        if (_updateRequired)
        {
            return;
        }

        base.SetAction(message);
    }

    /// <summary>
    /// Handles the OnSave event of the SettingsService control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private void SettingsService_OnSave(object sender, EventArgs e)
    {
        SetDefaultAction();
    }

    /// <summary>
    /// Lurkers the remaining monsters.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void Lurker_RemainingMonsters(object sender, PoeLurker.Patreon.Events.MonstersRemainEvent e)
    {
        SetAction(new ManaBulbMessage() { View = new RemainingMonsterViewModel(e), DisplayTime = TimeSpan.FromSeconds(3) });
    }

    /// <summary>
    /// Lurkers the location changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void Lurker_LocationChanged(object sender, PoeLurker.Patreon.Events.LocationChangedEvent e)
    {
        if (e.Location.EndsWith("Hideout"))
        {
            if (!HasAction)
            {
                var message = new ManaBulbMessage()
                {
                    Action = DefaultAction,
                    View = new LeagueViewModel(_eventAggregator),
                };
                SetAction(message);
            }
        }
        else
        {
            if (IsDefaultAction)
            {
                SetAction(new ManaBulbMessage());
            }
        }
    }

    #endregion
}