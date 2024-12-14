//-----------------------------------------------------------------------
// <copyright file="TradebarViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using PoeLurker.Core;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;
using PoeLurker.Patreon.Events;
using PoeLurker.Patreon.Models;
using PoeLurker.Patreon.Parsers;
using PoeLurker.Patreon.Services;
using PoeLurker.UI.Models;
using ProcessLurker;

/// <summary>
/// Represent the trade bar.
/// </summary>
/// <seealso cref="PoeLurker.UI.ViewModels.PoeOverlayBase" />
public class TradebarViewModel : PoeOverlayBase, IDisposable
{
    #region Fields

    private static readonly ItemClassParser ItemClassParser = new ItemClassParser();
    private static readonly int DefaultOverlayHeight = 60;
    private readonly PoeKeyboardHelper _keyboardHelper;
    private readonly ClientLurker _clientLurker;
    private readonly KeyboardLurker _keyboardLurker;
    private readonly TradebarContext _context;
    private readonly List<OfferViewModel> _activeOffers = new List<OfferViewModel>();
    private readonly IEventAggregator _eventAggregator;
    private System.Action _removeActive;
    private readonly List<TradeEvent> _soldOffers;
    private readonly SoundService _soundService;
    private readonly PushBulletService _pushBulletService;
    private readonly PushHoverService _pushHoverService;
    private readonly StashTabService _stashTabService;
    private readonly ClipboardLurker _clipboardLurker;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TradebarViewModel" /> class.
    /// </summary>
    /// <param name="eventAggregator">The event aggregator.</param>
    /// <param name="clientLurker">The client lurker.</param>
    /// <param name="processLurker">The process lurker.</param>
    /// <param name="keyboardLurker">The keyboard lurker.</param>
    /// <param name="dockingHelper">The docking helper.</param>
    /// <param name="keyboardHelper">The keyboard helper.</param>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="windowManager">The window manager.</param>
    /// <param name="soundService">The sound service.</param>
    /// <param name="pushBulletService">The PushBullet service.</param>
    /// <param name="pushHoverService">The PushHover service.</param>
    /// <param name="stashTabService">The stash tab service.</param>
    public TradebarViewModel(
        IEventAggregator eventAggregator,
        ClientLurker clientLurker,
        ProcessService processLurker,
        KeyboardLurker keyboardLurker,
        DockingHelper dockingHelper,
        PoeKeyboardHelper keyboardHelper,
        ClipboardLurker clipboardLurker,
        SettingsService settingsService,
        IWindowManager windowManager,
        SoundService soundService,
        PushBulletService pushBulletService,
        PushHoverService pushHoverService,
        StashTabService stashTabService)
        : base(windowManager, dockingHelper, processLurker, settingsService)
    {
        _eventAggregator = eventAggregator;
        _keyboardHelper = keyboardHelper;
        _soundService = soundService;
        _clientLurker = clientLurker;
        _pushBulletService = pushBulletService;
        _pushHoverService = pushHoverService;
        _stashTabService = stashTabService;
        _clipboardLurker = clipboardLurker;

        _keyboardLurker = keyboardLurker;
        TradeOffers = new ObservableCollection<OfferViewModel>();
        _soldOffers = new List<TradeEvent>();
        _context = new TradebarContext(RemoveOffer, AddActiveOffer, AddToSoldOffer, SetActiveOffer, ClearAll);
        DisplayName = "Poe Lurker";
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the trade offers.
    /// </summary>
    public ObservableCollection<OfferViewModel> TradeOffers { get; set; }

    /// <summary>
    /// Gets the active offer.
    /// </summary>
    private OfferViewModel ActiveOffer => _activeOffers.FirstOrDefault();

    #endregion

    #region Methods

    /// <summary>
    /// Searches the item.
    /// </summary>
    public async void SearchItem()
    {
        var activeOffer = ActiveOffer;
        if (activeOffer != null)
        {
            await _keyboardHelper.Search(activeOffer.BuildSearchItemName());
        }
    }

    /// <summary>
    /// Locate the item.
    /// </summary>
    public async void LocateItem()
    {
        var activeOffer = ActiveOffer;
        if (activeOffer != null)
        {
            await _keyboardHelper.Search(activeOffer.BuildSearchItemName());
            _stashTabService.PlaceMarker(activeOffer.Event.Location);
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
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _pushBulletService?.Dispose();
            _pushHoverService?.Dispose();
        }
    }

    /// <summary>
    /// Handles the MainActionPressed event of the KeyboardLurker control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="Winook.KeyboardMessageEventArgs"/> instance containing the event data.</param>
    private void KeyboardLurker_MainActionPressed(object sender, Winook.KeyboardMessageEventArgs e)
    {
        ExecuteOnRecentOffer((o) =>
        {
            Execute.OnUIThread(async () => await o.MainActionCore());
        });
    }

    /// <summary>
    /// Handles the InvitePressed event of the KeyboardLurker control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void KeyboardLurker_InvitePressed(object sender, System.EventArgs e)
    {
        ExecuteOnRecentOffer(async (o) =>
        {
            await o.Invite();
        });
    }

    /// <summary>
    /// Handles the WhisperPressed event of the KeyboardLurker control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void KeyboardLurker_WhisperPressed(object sender, System.EventArgs e)
    {
        ExecuteOnRecentOffer(async (o) =>
        {
            await o.Whisper();
        });
    }

    /// <summary>
    /// Handles the TradePressed event of the KeyboardLurker control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="Winook.KeyboardMessageEventArgs"/> instance containing the event data.</param>
    private void KeyboardLurker_TradePressed(object sender, Winook.KeyboardMessageEventArgs e)
    {
        ExecuteOnRecentOffer(async (o) =>
        {
            await o.Trade();
        });
    }

    /// <summary>
    /// Handles the BusyPressed event of the KeyboardLurker control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="Winook.KeyboardMessageEventArgs"/> instance containing the event data.</param>
    private void KeyboardLurker_BusyPressed(object sender, Winook.KeyboardMessageEventArgs e)
    {
        ExecuteOnRecentOffer(
            (o) =>
            {
                o.Wait();
            },
            (o) => o.Waiting);
    }

    /// <summary>
    /// Handles the StillInterestedPressed event of the KeyboardLurker control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="Winook.KeyboardMessageEventArgs"/> instance containing the event data.</param>
    private void KeyboardLurker_StillInterestedPressed(object sender, Winook.KeyboardMessageEventArgs e)
    {
        ExecuteOnRecentOffer(async (o) =>
        {
            await o.StillInterested();
        });
    }

    private void KeyboardLurker_DismissPressed(object sender, Winook.KeyboardMessageEventArgs e)
    {
        ExecuteOnRecentOffer((o) =>
        {
            o.Remove();
        });
    }

    /// <summary>
    /// Executes the on recent offer.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <param name="predicate">The predicate.</param>
    private void ExecuteOnRecentOffer(Action<OfferViewModel> action, Func<OfferViewModel, bool> predicate = null)
    {
        var offer = ActiveOffer;
        if (offer == null)
        {
            offer = TradeOffers.FirstOrDefault();
        }

        if (offer == null)
        {
            return;
        }

        if (predicate != null)
        {
            var index = TradeOffers.IndexOf(offer);
            while (index < TradeOffers.Count)
            {
                offer = TradeOffers.ElementAt(index);
                if (predicate(offer))
                {
                    index++;
                    continue;
                }

                break;
            }
        }

        if (!offer.IsActive)
        {
            SetActiveOffer(offer);
        }

        if (predicate == null || !predicate(offer))
        {
            action(offer);
        }
    }

    /// <summary>
    /// Lurkers the new offer.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The trade event.</param>
    private async void Lurker_IncomingOffer(object sender, TradeEvent e)
    {
        if (!PoeApplicationContext.InForeground || _clipboardLurker.LastClipboardText.Contains(e.WhisperMessage))
        {
            return;
        }

        if (TradeOffers.Any(o => o.Event.Equals(e)))
        {
            return;
        }

        var alreadySold = CheckIfOfferIsAlreadySold(e);
        if (alreadySold && SettingsService.IgnoreAlreadySold)
        {
            return;
        }

        if (SettingsService.AlertEnabled)
        {
            PlayAlert();
        }

        if (PoeApplicationContext.IsAfk)
        {
            await _pushBulletService.SendTradeMessageAsync(e);
            await _pushHoverService.SendTradeMessageAsync(e);
        }

        Execute.OnUIThread(() =>
        {
            TradeOffers.Add(new OfferViewModel(e, _keyboardHelper, _context, SettingsService, alreadySold, DockingHelper));
        });
    }

    /// <summary>
    /// Checks if offer is already sold.
    /// </summary>
    /// <param name="tradeEvent">The trade event.</param>
    private bool CheckIfOfferIsAlreadySold(TradeEvent tradeEvent)
    {
        if (!SettingsService.SoldDetection)
        {
            return false;
        }

        var location = tradeEvent.Location.ToString();
        var defaultLocation = new Location().ToString();
        if (location != defaultLocation)
        {
            foreach (var lastOffer in _soldOffers)
            {
                if (tradeEvent.ItemName == lastOffer.ItemName)
                {
                    if (location == lastOffer.Location.ToString())
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Plays the alert.
    /// </summary>
    private void PlayAlert()
    {
        _soundService.PlayTradeAlert(SettingsService.AlertVolume);
    }

    /// <summary>
    /// Lurkers the trade accepted.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private async void Lurker_TradeAccepted(object sender, TradeAcceptedEvent e)
    {
        var offer = TradeOffers.Where(t => t.Status == OfferStatus.Traded).FirstOrDefault();
        if (offer == null)
        {
            var activeOffer = ActiveOffer;
            if (activeOffer != null && activeOffer.BuyerInSameInstance)
            {
                offer = activeOffer;
            }
        }

        if (offer != null)
        {
            await ExecuteTradeAccepted(offer);
        }
    }

    /// <summary>
    /// Executes the trade.
    /// </summary>
    /// <param name="offer">The offer.</param>
    /// <returns>The task.</returns>
    private async Task ExecuteTradeAccepted(OfferViewModel offer)
    {
        InsertEvent(offer.Event);
        if (!string.IsNullOrEmpty(SettingsService.ThankYouMessage))
        {
            await offer.ThankYou();
        }

        if (SettingsService.AutoKickEnabled)
        {
            await _keyboardHelper.Kick(offer.PlayerName);
        }

        var itemClass = offer.Event.ItemClass;
        if (itemClass != ItemClass.Map && itemClass != ItemClass.Currency && itemClass != ItemClass.DivinationCard)
        {
            var alreadySold = CheckIfOfferIsAlreadySold(offer.Event);
            if (!alreadySold)
            {
                AddToSoldOffer(offer);
            }
        }

        // Listener: LeagueViewModel
        // this._eventAggregator.PublishOnUIThread(new TradeCompletedEvent());
        RemoveOffer(offer);
    }

    /// <summary>
    /// Adds to sold offer.
    /// </summary>
    /// <param name="offer">The offer.</param>
    private void AddToSoldOffer(OfferViewModel offer)
    {
        _soldOffers.Add(offer.Event);
        if (_soldOffers.Count >= 5)
        {
            _soldOffers.RemoveAt(0);
        }
    }

    /// <summary>
    /// Lurkers the player joined.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The PLayerJoined Event.</param>
    private void Lurker_PlayerJoined(object sender, PlayerJoinedEvent e)
    {
        var playNotification = false;
        foreach (var offer in TradeOffers.Where(o => o.PlayerName == e.PlayerName))
        {
            playNotification = true;
            offer.BuyerInSameInstance = true;
        }

        if (playNotification && SettingsService.JoinHideoutEnabled)
        {
            _soundService.PlayJoinHideout(SettingsService.JoinHideoutVolume);
        }
    }

    /// <summary>
    /// Lurkers the player left.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void Lurker_PlayerLeft(object sender, PlayerLeftEvent e)
    {
        foreach (var offer in TradeOffers.Where(o => o.PlayerName == e.PlayerName))
        {
            offer.BuyerInSameInstance = false;
        }
    }

    /// <summary>
    /// Inserts the event.
    /// </summary>
    /// <param name="tradeEvent">The trade event.</param>
    private void InsertEvent(TradeEvent tradeEvent)
    {
        Execute.OnUIThread(async () =>
        {
            try
            {
                using (var service = new DatabaseService())
                {
                    await service.InsertAsync(tradeEvent);
                }
            }
            catch
            {
            }
        });
    }

    /// <summary>
    /// Removes the offer.
    /// </summary>
    /// <param name="offer">The offer.</param>
    private void RemoveOffer(OfferViewModel offer)
    {
        if (offer != null)
        {
            Execute.OnUIThread(() =>
            {
                if (offer.Active)
                {
                    _removeActive?.Invoke();
                    _stashTabService.Close();
                }

                TradeOffers.Remove(offer);
                _activeOffers.Remove(offer);
                var activeOffer = ActiveOffer;
                if (activeOffer != null)
                {
                    activeOffer.Active = true;
                    SendToLifeBulb(activeOffer.Event);
                }
                else
                {
                    _removeActive?.Invoke();
                    _stashTabService.Close();
                }

                offer.Dispose();
            });
        }
    }

    /// <summary>
    /// Adds the active offer.
    /// </summary>
    /// <param name="offer">The offer.</param>
    private void AddActiveOffer(OfferViewModel offer)
    {
        if (!_activeOffers.Contains(offer))
        {
            _activeOffers.Add(offer);
        }

        ActiveOffer.Active = true;

        if (offer.Equals(ActiveOffer))
        {
            SendToLifeBulb(ActiveOffer.Event);
        }
    }

    /// <summary>
    /// Sends to life bulb.
    /// </summary>
    /// <param name="tradeEvent">The trade event.</param>
    private void SendToLifeBulb(TradeEvent tradeEvent)
    {
        var viewModel = new PositionViewModel(tradeEvent, SettingsService, _stashTabService);
        _eventAggregator.PublishOnUIThreadAsync(new LifeBulbMessage()
        {
            View = viewModel,
            OnShow = (a) => { _removeActive = a; },
            Action = SearchItem,
            SubAction = LocateItem,
        });
    }

    /// <summary>
    /// Sets the active offer.
    /// </summary>
    /// <param name="offer">The offer.</param>
    private void SetActiveOffer(OfferViewModel offer)
    {
        DockingHelper.SetForeground();

        var currentActiveOffer = ActiveOffer;
        if (currentActiveOffer == null)
        {
            AddActiveOffer(offer);
            return;
        }

        if (currentActiveOffer == offer)
        {
            return;
        }

        currentActiveOffer.Active = false;

        var index = _activeOffers.IndexOf(offer);
        if (index != -1)
        {
            _activeOffers.RemoveAt(index);
        }

        _activeOffers.Insert(0, offer);
        ActiveOffer.Active = true;

        SendToLifeBulb(offer.Event);
    }

    /// <summary>
    /// Clears the offers.
    /// </summary>
    private void ClearAll()
    {
        TradeOffers.Clear();
        _activeOffers.Clear();
        _removeActive?.Invoke();

        NotifyOfPropertyChange("ActiveOffer");
    }

    /// <summary>
    /// Called when deactivating.
    /// </summary>
    /// <param name="close">Inidicates whether this instance will be closed.</param>
    protected override Task OnDeactivateAsync(bool close, CancellationToken token)
    {
        if (close)
        {
            SettingsService.OnSave -= SettingsService_OnSave;

            _keyboardLurker.InvitePressed -= KeyboardLurker_InvitePressed;
            _keyboardLurker.WhisperPressed -= KeyboardLurker_WhisperPressed;
            _keyboardLurker.BusyPressed -= KeyboardLurker_BusyPressed;
            _keyboardLurker.DismissPressed -= KeyboardLurker_DismissPressed;

            _clientLurker.IncomingOffer -= Lurker_IncomingOffer;
            _clientLurker.TradeAccepted -= Lurker_TradeAccepted;
            _clientLurker.PlayerJoined -= Lurker_PlayerJoined;
            _clientLurker.PlayerLeft -= Lurker_PlayerLeft;
            Execute.OnUIThread(() => TradeOffers.Clear());
        }

        return base.OnDeactivateAsync(close, token);
    }

    /// <summary>
    /// Called when activating.
    /// </summary>
    protected async override Task OnActivateAsync(CancellationToken token)
    {
        if (SettingsService.ConnectedToPatreon)
        {
            await _pushBulletService.CheckPledgeStatus();
            await _pushHoverService.CheckPledgeStatus();
        }

        SettingsService.OnSave += SettingsService_OnSave;

        _keyboardLurker.MainActionPressed += KeyboardLurker_MainActionPressed;
        _keyboardLurker.WhisperPressed += KeyboardLurker_WhisperPressed;
        _keyboardLurker.InvitePressed += KeyboardLurker_InvitePressed;
        _keyboardLurker.BusyPressed += KeyboardLurker_BusyPressed;
        _keyboardLurker.DismissPressed += KeyboardLurker_DismissPressed;

        _keyboardLurker.InstallHandlers();

        _clientLurker.IncomingOffer += Lurker_IncomingOffer;
        _clientLurker.TradeAccepted += Lurker_TradeAccepted;
        _clientLurker.PlayerJoined += Lurker_PlayerJoined;
        _clientLurker.PlayerLeft += Lurker_PlayerLeft;

        await base.OnActivateAsync(token);
    }

    /// <summary>
    /// Sets the window position.
    /// </summary>
    /// <param name="windowInformation">The window information.</param>
    protected override void SetWindowPosition(PoeWindowInformation windowInformation)
    {
        // When Poe Lurker is updated we save the settings before the view are loaded
        if (View == null)
        {
            return;
        }

        var overlayHeight = DefaultOverlayHeight * windowInformation.FlaskBarHeight / DefaultFlaskBarHeight * SettingsService.TradebarScaling;
        var overlayWidth = (windowInformation.Width - (windowInformation.FlaskBarWidth * 2)) / 2;

        Execute.OnUIThread(() =>
        {
            View.Height = ApplyAbsoluteScalingY(overlayHeight);
            View.Width = ApplyAbsoluteScalingX(overlayWidth);
            View.Left = ApplyScalingX(windowInformation.Position.Left + windowInformation.FlaskBarWidth + Margin);
            View.Top = ApplyScalingY(windowInformation.Position.Bottom - overlayHeight - windowInformation.ExpBarHeight - Margin);
        });
    }

    /// <summary>
    /// Handles the OnSave event of the _settingsService control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void SettingsService_OnSave(object sender, System.EventArgs e)
    {
        if (DockingHelper.WindowInformation != null)
        {
            SetWindowPosition(DockingHelper.WindowInformation);
        }
    }

    #endregion
}