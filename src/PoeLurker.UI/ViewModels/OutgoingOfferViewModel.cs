//-----------------------------------------------------------------------
// <copyright file="OutgoingOfferViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System.Windows.Input;
using Caliburn.Micro;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Services;
using PoeLurker.Patreon.Events;
using PoeLurker.UI.Models;

/// <summary>
/// Represents the outgoing offer.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class OutgoingOfferViewModel : PropertyChangedBase
{
    #region Fields

    private readonly OutgoingTradeEvent _event;
    private readonly PoeKeyboardHelper _keyboardHelper;
    private bool _skipMainAction;
    private bool _waiting;
    private bool _active;
    private double _delayToClose;
    private readonly OutgoingbarContext _barContext;
    private readonly DockingHelper _dockingHelper;
    private readonly SettingsService _settingsService;
    private double _times;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="OutgoingOfferViewModel" /> class.
    /// </summary>
    /// <param name="tradeEvent">The trade event.</param>
    /// <param name="keyboardHelper">The keyboard helper.</param>
    /// <param name="context">The context.</param>
    /// <param name="dockingHelper">The docking helper.</param>
    /// <param name="settingsService">The settings service.</param>
    public OutgoingOfferViewModel(OutgoingTradeEvent tradeEvent, PoeKeyboardHelper keyboardHelper, OutgoingbarContext context, DockingHelper dockingHelper, SettingsService settingsService)
    {
        _event = tradeEvent;
        _keyboardHelper = keyboardHelper;
        _barContext = context;
        _dockingHelper = dockingHelper;
        _settingsService = settingsService;
        _times = settingsService.OutgoingDelayToClose;
        DelayToClose = 100;
        PriceValue = tradeEvent.Price.CalculateValue();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the price value.
    /// </summary>
    public double PriceValue { get; }

    /// <summary>
    /// Gets the name of the player.
    /// </summary>
    public string PlayerName => _event.PlayerName;

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="OutgoingOfferViewModel"/> is waiting.
    /// </summary>
    public bool Waiting
    {
        get
        {
            return _waiting;
        }

        set
        {
            _waiting = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="OutgoingOfferViewModel"/> is active.
    /// </summary>
    public bool Active
    {
        get
        {
            return _active;
        }

        set
        {
            _active = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets the event.
    /// </summary>
    public TradeEvent Event => _event;

    /// <summary>
    /// Gets or sets the delay to close.
    /// </summary>
    public double DelayToClose
    {
        get
        {
            return _delayToClose;
        }

        set
        {
            if (value <= 0)
            {
                RemoveCore(false);
            }

            _delayToClose = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets the trade value.
    /// </summary>
    public TradeValueViewModel TradeValue => new(_event, _settingsService);

    #endregion

    #region Methods

    /// <summary>
    /// Tick the delay to close.
    /// </summary>
    public void Tick()
    {
        _times -= 0.15;
        DelayToClose = _times * 100 / _settingsService.OutgoingDelayToClose;
    }

    /// <summary>
    /// Whoes the is.
    /// </summary>
    public async void WhoIs()
    {
        await _keyboardHelper.WhoIs(_event.PlayerName);
    }

    /// <summary>
    /// Mains the action.
    /// </summary>
    public async void MainAction()
    {
        if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
        {
            _barContext.SetActiveOffer(this);
            return;
        }

        if (_skipMainAction)
        {
            _skipMainAction = false;
            return;
        }

        await _keyboardHelper.JoinHideout(_event.PlayerName);
        _barContext.SetActiveOffer(this);
    }

    /// <summary>
    /// Res the send the offer.
    /// </summary>
    public async void ReSend()
    {
        _skipMainAction = true;
        await _keyboardHelper.Whisper(_event.PlayerName, _event.WhisperMessage);
        Waiting = true;
        DelayToClose = 100;
        _times = _settingsService.OutgoingDelayToClose;
    }

    /// <summary>
    /// Removes this instance.
    /// </summary>
    public void Remove()
    {
        RemoveCore(true);
    }

    /// <summary>
    /// Removes the core.
    /// </summary>
    /// <param name="setForeground">if set to <c>true</c> [set foreground].</param>
    public void RemoveCore(bool setForeground)
    {
        _skipMainAction = true;
        var controlPressed = false;
        Execute.OnUIThread(() => controlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
        if (controlPressed)
        {
            _barContext.ClearAll();
        }
        else
        {
            _barContext.RemoveOffer(this);
        }

        if (setForeground)
        {
            _dockingHelper.SetForeground();
        }
    }

    /// <summary>
    /// Sets the active.
    /// </summary>
    public void SetActive()
    {
        DelayToClose = 100;
        _times = _settingsService.OutgoingDelayToClose;
        Active = true;
    }

    #endregion
}