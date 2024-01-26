//-----------------------------------------------------------------------
// <copyright file="OutgoingbarViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Caliburn.Micro;
using PoeLurker.Core;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;
using PoeLurker.Patreon.Events;
using PoeLurker.Patreon.Services;
using PoeLurker.UI.Models;
using ProcessLurker;

/// <summary>
/// Represents the outgoing bar view model.
/// </summary>
/// <seealso cref="PoeLurker.UI.ViewModels.Screen" />
/// <seealso cref="Caliburn.Micro.IViewAware" />
public class OutgoingbarViewModel : PoeOverlayBase
{
    #region Fields

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly ClipboardLurker _clipboardLurker;
    private readonly ClientLurker _clientLurker;
    private readonly PoeKeyboardHelper _keyboardHelper;
    private readonly Timer _timer;
    private readonly IEventAggregator _eventAggregator;
    private System.Action _removeActive;
    private OutgoingOfferViewModel _activeOffer;
    private string _lastOutgoingOfferText;
    private string _searchValue;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="OutgoingbarViewModel" /> class.
    /// </summary>
    /// <param name="eventAggregator">The event aggregator.</param>
    /// <param name="clipboardLurker">The clipboard lurker.</param>
    /// <param name="clientLurker">The client lurker.</param>
    /// <param name="processLurker">The process lurker.</param>
    /// <param name="dockingHelper">The docking helper.</param>
    /// <param name="keyboardHelper">The keyboard helper.</param>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="windowManager">The window manager.</param>
    public OutgoingbarViewModel(
        IEventAggregator eventAggregator,
        ClipboardLurker clipboardLurker,
        ClientLurker clientLurker,
        ProcessService processLurker,
        DockingHelper dockingHelper,
        PoeKeyboardHelper keyboardHelper,
        SettingsService settingsService,
        IWindowManager windowManager)
        : base(windowManager, dockingHelper, processLurker, settingsService)
    {
        Offers = new ObservableCollection<OutgoingOfferViewModel>();
        FilteredOffers = new ObservableCollection<OutgoingOfferViewModel>();
        _timer = new Timer(50);
        _timer.Elapsed += Timer_Elapsed;
        _keyboardHelper = keyboardHelper;
        _clipboardLurker = clipboardLurker;
        _eventAggregator = eventAggregator;
        _clientLurker = clientLurker;

        Offers.CollectionChanged += Offers_CollectionChanged;
        _clipboardLurker.NewOffer += ClipboardLurker_NewOffer;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the search value.
    /// </summary>
    public string SearchValue
    {
        get
        {
            return _searchValue;
        }

        set
        {
            _searchValue = value;
            OnSearchValueChange(value);
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the filtered offer.
    /// </summary>
    public ObservableCollection<OutgoingOfferViewModel> FilteredOffers { get; set; }

    /// <summary>
    /// Gets or sets the offers.
    /// </summary>
    public ObservableCollection<OutgoingOfferViewModel> Offers { get; set; }

    /// <summary>
    /// Gets a value indicating whether [any offer].
    /// </summary>
    public bool AnyOffer => Offers.Any();

    /// <summary>
    /// Gets the default width.
    /// </summary>
    protected static int DefaultWidth => 300;

    #endregion

    #region Methods

    /// <summary>
    /// Called when activating.
    /// </summary>
    protected override Task OnActivateAsync(CancellationToken token)
    {
        _clientLurker.OutgoingOffer += Lurker_OutgoingOffer;
        _clientLurker.TradeAccepted += Lurker_TradeAccepted;

        return base.OnActivateAsync(token);
    }

    /// <summary>
    /// Called when deactivating.
    /// </summary>
    /// <param name="close">Inidicates whether this instance will be closed.</param>
    protected override Task OnDeactivateAsync(bool close, CancellationToken token)
    {
        if (close)
        {
            _clientLurker.OutgoingOffer -= Lurker_OutgoingOffer;
            _clientLurker.TradeAccepted -= Lurker_TradeAccepted;
        }

        return base.OnDeactivateAsync(close, token);
    }

    /// <summary>
    /// Creates the context.
    /// </summary>
    /// <returns>The context.</returns>
    private OutgoingbarContext CreateContext()
    {
        return new OutgoingbarContext(RemoveOffer, SetActiveOffer, ClearAll);
    }

    /// <summary>
    /// Called when [search value change].
    /// </summary>
    /// <param name="value">The value.</param>
    private void OnSearchValueChange(string value)
    {
        Execute.OnUIThread(() =>
        {
            FilteredOffers.Clear();
            var offers = string.IsNullOrEmpty(value) ? Offers.ToArray() : Offers.Where(o => o.PlayerName.ToLower().Contains(value.ToLower())).OrderBy(o => o.PlayerName).ToArray();
            foreach (var offer in offers)
            {
                FilteredOffers.Add(offer);
            }
        });
    }

    /// <summary>
    /// Clipboards the lurker new offer.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="newOfferText">The new offer text.</param>
    private async void ClipboardLurker_NewOffer(object sender, string newOfferText)
    {
        if (!SettingsService.ClipboardEnabled || _lastOutgoingOfferText == newOfferText)
        {
            return;
        }

        _lastOutgoingOfferText = newOfferText;
        await _keyboardHelper.SendMessage(newOfferText);
    }

    /// <summary>
    /// Handles the Elapsed event of the Timer control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
        try
        {
            foreach (var offer in Offers.Where(o => !o.Waiting && !o.Active))
            {
                offer.Tick();

                if (offer.DelayToClose <= 0)
                {
                    return;
                }
            }
        }
        catch (System.InvalidOperationException)
        {
            // An offer has been deleted (Will need to handle this scenario)
            return;
        }
    }

    /// <summary>
    /// Lurkers the outgoing offer.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void Lurker_OutgoingOffer(object sender, PoeLurker.Patreon.Events.OutgoingTradeEvent e)
    {
        if (Offers.Any(o => o.Event.Equals(e)))
        {
            return;
        }

        Execute.OnUIThread(() =>
        {
            var index = 0;
            if (AnyOffer)
            {
                var value = e.Price.CalculateValue();
                var closestOffer = Offers.Aggregate((x, y) => Math.Abs(x.PriceValue - value) < Math.Abs(y.PriceValue - value) ? x : y);
                index = Offers.IndexOf(closestOffer);

                if (value >= closestOffer.PriceValue)
                {
                    index++;
                }
            }

            Offers.Insert(index, new OutgoingOfferViewModel(e, _keyboardHelper, CreateContext(), DockingHelper, SettingsService));
        });
    }

    /// <summary>
    /// Lurkers the trade accepted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private async void Lurker_TradeAccepted(object sender, PoeLurker.Patreon.Events.TradeAcceptedEvent e)
    {
        if (_activeOffer == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(SettingsService.ThankYouMessage))
        {
            var tradeEvent = _activeOffer.Event;
            await _keyboardHelper.Whisper(tradeEvent.PlayerName, TokenHelper.ReplaceToken(SettingsService.ThankYouMessage, tradeEvent));
        }

        if (SettingsService.AutoKickEnabled)
        {
            await _keyboardHelper.Leave();
        }

        await InsertEvent(_activeOffer.Event);
        RemoveOffer(_activeOffer);
        _activeOffer = null;
        _removeActive = null;
    }

    /// <summary>
    /// Inserts the event.
    /// </summary>
    /// <param name="tradeEvent">The trade event.</param>
    private async Task InsertEvent(TradeEvent tradeEvent)
    {
        if (tradeEvent == null)
        {
            return;
        }

        try
        {
            using (var service = new DatabaseService())
            {
                await service.InsertAsync(tradeEvent);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, ex.Message);
        }
    }

    /// <summary>
    /// Removes the offer.
    /// </summary>
    /// <param name="offer">The offer.</param>
    private void RemoveOffer(OutgoingOfferViewModel offer)
    {
        if (offer == _activeOffer)
        {
            _removeActive?.Invoke();
        }

        _timer.Stop();
        Execute.OnUIThread(() => Offers.Remove(offer));
        _timer.Start();
    }

    /// <summary>
    /// Clears all.
    /// </summary>
    private void ClearAll()
    {
        _removeActive?.Invoke();
        Offers.Clear();
    }

    /// <summary>
    /// Sets the window position.
    /// </summary>
    /// <param name="windowInformation">The window information.</param>
    protected override void SetWindowPosition(PoeWindowInformation windowInformation)
    {
        var yPosition = windowInformation.FlaskBarWidth * (238 / (double)DefaultFlaskBarWidth);
        var width = windowInformation.Height * DefaultWidth / 1080;
        var height = windowInformation.FlaskBarHeight + 25;

        Execute.OnUIThread(() =>
        {
            View.Height = ApplyAbsoluteScalingY(height < 0 ? 1 : height);
            View.Width = ApplyAbsoluteScalingX(width);
            View.Left = ApplyScalingX(windowInformation.Position.Left + yPosition);
            View.Top = ApplyScalingY(windowInformation.Position.Bottom - height - 12 + Margin);
        });
    }

    /// <summary>
    /// Handles the CollectionChanged event of the Offers control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
    private void Offers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (Offers.Any(o => !o.Waiting))
        {
            _timer.Start();
        }
        else
        {
            _timer.Stop();
        }

        if (e.NewItems != null)
        {
            foreach (var offer in e.NewItems)
            {
                var outgoingOffer = offer as OutgoingOfferViewModel;
                if (string.IsNullOrEmpty(SearchValue))
                {
                    FilteredOffers.Insert(Offers.IndexOf(outgoingOffer), outgoingOffer);
                }
                else if (outgoingOffer.PlayerName.ToLower().Contains(SearchValue.ToLower()))
                {
                    FilteredOffers.Insert(0, outgoingOffer);
                }
            }
        }

        if (e.OldItems != null)
        {
            foreach (var offer in e.OldItems)
            {
                FilteredOffers.Remove(offer as OutgoingOfferViewModel);
            }
        }

        if (!AnyOffer)
        {
            SearchValue = string.Empty;
        }

        NotifyOfPropertyChange("AnyOffer");
    }

    /// <summary>
    /// Sets the active offer.
    /// </summary>
    /// <param name="offer">The offer.</param>
    private void SetActiveOffer(OutgoingOfferViewModel offer)
    {
        DockingHelper.SetForeground();

        if (offer.Active)
        {
            return;
        }

        if (_activeOffer != null)
        {
            _activeOffer.Active = false;
        }

        _activeOffer = offer;
        _activeOffer.SetActive();

        _eventAggregator.PublishOnUIThreadAsync(new LifeBulbMessage()
        {
            View = new TradeValueViewModel(offer.Event, SettingsService),
            OnShow = (a) => { _removeActive = a; },
            Action = async () => { await _keyboardHelper.Trade(offer.Event.PlayerName); },
        });
    }

    #endregion
}