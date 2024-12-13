//-----------------------------------------------------------------------
// <copyright file="OfferViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Services;
using PoeLurker.Patreon.Events;
using PoeLurker.Patreon.Models;
using PoeLurker.UI.Models;
using PoeLurker.UI.Views;

/// <summary>
/// Represents an Offer.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
/// <seealso cref="System.IDisposable" />
public class OfferViewModel : Screen, IDisposable
{
    #region Fields

    private readonly TradeEvent _tradeEvent;
    private TimeSpan _elapsedTime;
    private readonly PoeKeyboardHelper _keyboardHelper;
    private readonly TradebarContext _tradebarContext;
    private readonly SettingsService _settingsService;
    private OfferStatus _status;
    private bool _waiting;
    private bool _active;
    private bool _skipMainAction;
    private bool _buyerInSameInstance;
    private bool _alreadySold;
    private bool _showDetail;
    private CancellationTokenSource _tokenSource;
    private readonly DockingHelper _dockingHelper;
    private OfferDetailsViewModel _details;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="OfferViewModel" /> class.
    /// </summary>
    /// <param name="tradeEvent">The trade event.</param>
    /// <param name="keyboardHelper">The keyboard helper.</param>
    /// <param name="tradebarContext">The tradebar context.</param>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="sold">if set to <c>true</c> [sold].</param>
    /// <param name="dockingHelper">The docking helper.</param>
    public OfferViewModel(TradeEvent tradeEvent, PoeKeyboardHelper keyboardHelper, TradebarContext tradebarContext, SettingsService settingsService, bool sold, DockingHelper dockingHelper)
    {
        _tradeEvent = tradeEvent;
        _keyboardHelper = keyboardHelper;
        _tradebarContext = tradebarContext;
        _settingsService = settingsService;
        _alreadySold = sold;
        _dockingHelper = dockingHelper;

        _settingsService.OnSave += SettingsService_OnSave;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the height of the button.
    /// </summary>
    public double ButtonHeight { get; private set; }

    /// <summary>
    /// Gets the size of the font.
    /// </summary>
    public double FontSize { get; private set; }

    /// <summary>
    /// Gets a value indicating whether [already sold].
    /// </summary>
    public bool AlreadySold => _alreadySold;

    /// <summary>
    /// Gets a value indicating whether [not sold].
    /// </summary>
    public bool NotSold => !_alreadySold;

    /// <summary>
    /// Gets the number off currency.
    /// </summary>
    public double NumberOffCurrency => _tradeEvent.Price.NumberOfCurrencies;

    /// <summary>
    /// Gets the name of the item.
    /// </summary>
    public string ItemName => _tradeEvent.ItemName;

    /// <summary>
    /// Gets the location.
    /// </summary>
    public Location Location => _tradeEvent.Location;

    /// <summary>
    /// Gets the name of the player.
    /// </summary>
    public string PlayerName => _tradeEvent.PlayerName;

    /// <summary>
    /// Gets the type of the currency.
    /// </summary>
    public CurrencyType CurrencyType => _tradeEvent.Price.CurrencyType;

    /// <summary>
    /// Gets the currency view model.
    /// </summary>
    public CurrencyViewModel Currency => new (_tradeEvent.Price.CurrencyType);

    /// <summary>
    /// Gets the price.
    /// </summary>
    public Price Price => _tradeEvent.Price;

    /// <summary>
    /// Gets or sets the time span since the offer was received.
    /// </summary>
    public TimeSpan Elapsed
    {
        get
        {
            return _elapsedTime;
        }

        set
        {
            _elapsedTime = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is invite sended.
    /// </summary>
    public OfferStatus Status
    {
        get
        {
            return _status;
        }

        set
        {
            _status = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="OfferViewModel"/> is ShowDetail.
    /// </summary>
    public bool ShowDetail
    {
        get
        {
            return _showDetail;
        }

        set
        {
            _showDetail = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="OfferViewModel"/> is waiting.
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
    /// Gets or sets a value indicating whether this <see cref="OfferViewModel"/> is active.
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
    /// Gets or sets a value indicating whether [buyer in same instance].
    /// </summary>
    public bool BuyerInSameInstance
    {
        get
        {
            return _buyerInSameInstance;
        }

        set
        {
            _buyerInSameInstance = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets the Details.
    /// </summary>
    public OfferDetailsViewModel Details
    {
        get
        {
            return _details;
        }

        private set
        {
            _details = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets a value indicating whether [tool tip enabled].
    /// </summary>
    public bool ToolTipEnabled => _settingsService.ToolTipEnabled;

    /// <summary>
    /// Gets the tool tip delay.
    /// </summary>
    public int ToolTipDelay => _settingsService.ToolTipDelay;

    /// <summary>
    /// Gets the event.
    /// </summary>
    public TradeEvent Event => _tradeEvent;

    /// <summary>
    /// Gets the date.
    /// </summary>
    public string Date => _tradeEvent.Date.ToString("h:mm tt");

    #endregion

    #region Methods

    /// <summary>
    /// Dismisses the cart.
    /// </summary>
    public void DismissCart()
    {
        _skipMainAction = true;
        _alreadySold = false;
        NotifyOfPropertyChange(() => AlreadySold);
        NotifyOfPropertyChange(() => NotSold);
    }

    /// <summary>
    /// Whoes the is.
    /// </summary>
    public async void RightClick()
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl))
        {
            await _keyboardHelper.WhoIs(PlayerName);
            return;
        }

        if (_tradeEvent.Price.CurrencyType == CurrencyType.Divine)
        {
            var detail = new OfferDetailsViewModel(_tradeEvent);
            await detail.Initialize();
            Details = detail;
            ShowDetail = !ShowDetail;
        }
        else
        {
            await _keyboardHelper.WhoIs(PlayerName);
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Removes this instance.
    /// </summary>
    public void Remove()
    {
        _skipMainAction = true;
        Execute.OnUIThread(() =>
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                _tradebarContext.ClearAll();
            }
            else
            {
                RemoveFromTradebar();
            }

            _dockingHelper.SetForeground();
        });
    }

    /// <summary>
    /// Waits this instance.
    /// </summary>
    public async void Wait()
    {
        _skipMainAction = true;
        Waiting = true;
        await Whisper(_settingsService.BusyMessage);
    }

    /// <summary>
    /// Answers this instance.
    /// </summary>
    public async void MainAction()
    {
        if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
        {
            _tradebarContext.SetActiveOffer(this);
            return;
        }

        if (_skipMainAction)
        {
            _skipMainAction = false;
            return;
        }

        if (AlreadySold)
        {
            await Sold();
            return;
        }

        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                await StillInterested();
                return;
            }

            _tradebarContext.AddToSoldOffer(this);
            await Sold();
            return;
        }

        await MainActionCore();
    }

    /// <summary>
    /// Mains the action core.
    /// </summary>
    /// <returns>The task.</returns>
    public async Task MainActionCore()
    {
        switch (_status)
        {
            case OfferStatus.Pending:
                await Invite();
                break;
            case OfferStatus.Invited:
            case OfferStatus.Traded:
                await Trade();
                break;
        }
    }

    /// <summary>
    /// Builds the name of the search item.
    /// </summary>
    /// <returns>The item name to be placed in tab search.</returns>
    public string BuildSearchItemName()
    {
        return _tradeEvent.BuildSearchItemName();
    }

    /// <summary>
    /// Thanks you.
    /// </summary>
    /// <returns>The task.</returns>
    public async Task ThankYou()
    {
        await Whisper(_settingsService.ThankYouMessage);
    }

    /// <summary>
    /// Called when [mouse down].
    /// </summary>
    public async void OnMouseDown()
    {
        if (_tokenSource != null || Status == OfferStatus.Pending)
        {
            return;
        }

        try
        {
            _tokenSource = new CancellationTokenSource();
            await Task.Delay(500);
            if (_tokenSource.IsCancellationRequested)
            {
                return;
            }

            await _keyboardHelper.Invite(PlayerName);
        }
        finally
        {
            _tokenSource.Dispose();
            _tokenSource = null;
        }
    }

    /// <summary>
    /// Called when [mouse up].
    /// </summary>
    public void OnMouseUp()
    {
        _tokenSource?.Cancel();
    }

    /// <summary>
    /// Called when [mouse leave].
    /// </summary>
    public void OnMouseLeave()
    {
        ShowDetail = false;
    }

    /// <summary>
    /// Invites the buyer.
    /// </summary>
    /// <returns>The task.</returns>
    public async Task Invite()
    {
        Status = OfferStatus.Invited;
        await _keyboardHelper.Invite(PlayerName);
        _tradebarContext.AddToActiveOffer(this);
    }

    /// <summary>
    /// Whisper the buyer.
    /// </summary>
    /// <returns>The task.</returns>
    public async Task Whisper()
    {
        await _keyboardHelper.Whisper(PlayerName);
        _tradebarContext.AddToActiveOffer(this);
    }

    /// <summary>
    /// Trades the Buyer.
    /// </summary>
    /// <returns>the task.</returns>
    public async Task Trade()
    {
        Status = OfferStatus.Traded;
        await _keyboardHelper.Trade(PlayerName);
    }

    /// <summary>
    /// Stills the interested.
    /// </summary>
    /// <returns>The task.</returns>
    public async Task StillInterested()
    {
        await Whisper(_settingsService.StillInterestedMessage);
    }

    /// <summary>
    /// Called when an attached view's Loaded event fires.
    /// </summary>
    /// <param name="view">The view.</param>
    protected override void OnViewLoaded(object view)
    {
        var offerView = view as OfferView;
        ButtonHeight = offerView.ActualHeight / 3;
        FontSize = offerView.ActualHeight / 4;

        NotifyOfPropertyChange("ButtonHeight");
        NotifyOfPropertyChange("FontSize");
    }

    /// <summary>
    /// Tell the buyer that the item is sold.
    /// </summary>
    private async Task Sold()
    {
        await Whisper(_settingsService.SoldMessage);
        RemoveFromTradebar();
    }

    /// <summary>
    /// Removes this instance.
    /// </summary>
    private void RemoveFromTradebar()
    {
        _tradebarContext.RemoveOffer(this);
    }

    /// <summary>
    /// Whispers the specified message.
    /// </summary>
    /// <param name="message">The message.</param>
    private async Task Whisper(string message)
    {
        await _keyboardHelper.Whisper(PlayerName, TokenHelper.ReplaceToken(message, _tradeEvent));
    }

    /// <summary>
    /// Handles the OnSave event of the SettingsService control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void SettingsService_OnSave(object sender, System.EventArgs e)
    {
        NotifyOfPropertyChange(nameof(ToolTipEnabled));
        NotifyOfPropertyChange(nameof(ToolTipDelay));
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _settingsService.OnSave -= SettingsService_OnSave;
        }
    }

    #endregion
}