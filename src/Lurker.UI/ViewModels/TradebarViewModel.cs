//-----------------------------------------------------------------------
// <copyright file="TradebarViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Patreon.Events;
    using Lurker.Patreon.Models;
    using Lurker.Patreon.Parsers;
    using Lurker.Patreon.Services;
    using Lurker.Services;
    using Lurker.UI.Models;

    /// <summary>
    /// Represent the trade bar.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.PoeOverlayBase" />
    public class TradebarViewModel : PoeOverlayBase
    {
        #region Fields

        private static readonly ItemClassParser ItemClassParser = new ItemClassParser();
        private static readonly int DefaultOverlayHeight = 60;
        private PoeKeyboardHelper _keyboardHelper;
        private ClientLurker _clientLurker;
        private KeyboardLurker _keyboardLurker;
        private TradebarContext _context;
        private List<OfferViewModel> _activeOffers = new List<OfferViewModel>();
        private IEventAggregator _eventAggregator;
        private System.Action _removeActive;
        private List<TradeEvent> _soldOffers;
        private SoundService _soundService;
        private PushBulletService _pushBulletService;

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
        public TradebarViewModel(
            IEventAggregator eventAggregator,
            ClientLurker clientLurker,
            ProcessLurker processLurker,
            KeyboardLurker keyboardLurker,
            DockingHelper dockingHelper,
            PoeKeyboardHelper keyboardHelper,
            SettingsService settingsService,
            IWindowManager windowManager,
            SoundService soundService,
            PushBulletService pushBulletService)
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            this._eventAggregator = eventAggregator;
            this._keyboardHelper = keyboardHelper;
            this._soundService = soundService;
            this._clientLurker = clientLurker;
            this._pushBulletService = pushBulletService;

            this._keyboardLurker = keyboardLurker;
            this.TradeOffers = new ObservableCollection<OfferViewModel>();
            this._soldOffers = new List<TradeEvent>();
            this._context = new TradebarContext(this.RemoveOffer, this.AddActiveOffer, this.AddToSoldOffer, this.SetActiveOffer, this.ClearAll);
            this.DisplayName = "Poe Lurker";
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
        private OfferViewModel ActiveOffer => this._activeOffers.FirstOrDefault();

        #endregion

        #region Methods

        /// <summary>
        /// Searches the item.
        /// </summary>
        public async void SearchItem()
        {
            var activeOffer = this.ActiveOffer;
            if (activeOffer != null)
            {
                await this._keyboardHelper.Search(activeOffer.BuildSearchItemName());
            }
        }

        /// <summary>
        /// Handles the MainActionPressed event of the KeyboardLurker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Winook.KeyboardMessageEventArgs"/> instance containing the event data.</param>
        private void KeyboardLurker_MainActionPressed(object sender, Winook.KeyboardMessageEventArgs e)
        {
            this.ExecuteOnRecentOffer((o) =>
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
            this.ExecuteOnRecentOffer(async (o) =>
            {
                await o.Invite();
            });
        }

        /// <summary>
        /// Handles the TradePressed event of the KeyboardLurker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Winook.KeyboardMessageEventArgs"/> instance containing the event data.</param>
        private void KeyboardLurker_TradePressed(object sender, Winook.KeyboardMessageEventArgs e)
        {
            this.ExecuteOnRecentOffer(async (o) =>
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
            this.ExecuteOnRecentOffer(
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
            this.ExecuteOnRecentOffer(async (o) =>
            {
                await o.StillInterested();
            });
        }

        private void KeyboardLurker_DismissPressed(object sender, Winook.KeyboardMessageEventArgs e)
        {
            this.ExecuteOnRecentOffer((o) =>
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
            var offer = this.ActiveOffer;
            if (offer == null)
            {
                offer = this.TradeOffers.FirstOrDefault();
            }

            if (offer == null)
            {
                return;
            }

            if (predicate != null)
            {
                var index = this.TradeOffers.IndexOf(offer);
                while (index < this.TradeOffers.Count)
                {
                    offer = this.TradeOffers.ElementAt(index);
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
                this.SetActiveOffer(offer);
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
            if (this.TradeOffers.Any(o => o.Event.Equals(e)))
            {
                return;
            }

            if (this.SettingsService.AlertEnabled)
            {
                this.PlayAlert();
            }

            if (PoeApplicationContext.IsAfk)
            {
                await this._pushBulletService.SendTradeMessageAsync(e);
            }

            Execute.OnUIThread(() =>
            {
                this.TradeOffers.Add(new OfferViewModel(e, this._keyboardHelper, this._context, this.SettingsService, this.CheckIfOfferIsAlreadySold(e), this.DockingHelper));
            });
        }

        /// <summary>
        /// Checks if offer is already sold.
        /// </summary>
        /// <param name="tradeEvent">The trade event.</param>
        private bool CheckIfOfferIsAlreadySold(TradeEvent tradeEvent)
        {
            if (!this.SettingsService.SoldDetection)
            {
                return false;
            }

            var location = tradeEvent.Location.ToString();
            var defaultLocation = new Location().ToString();
            if (location != defaultLocation)
            {
                foreach (var lastOffer in this._soldOffers)
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
            this._soundService.PlayTradeAlert(this.SettingsService.AlertVolume);
        }

        /// <summary>
        /// Lurkers the trade accepted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void Lurker_TradeAccepted(object sender, TradeAcceptedEvent e)
        {
            var offer = this.TradeOffers.Where(t => t.Status == OfferStatus.Traded).FirstOrDefault();
            if (offer == null)
            {
                var activeOffer = this.ActiveOffer;
                if (activeOffer != null && activeOffer.BuyerInSameInstance)
                {
                    offer = activeOffer;
                }
            }

            if (offer != null)
            {
                await this.ExecuteTradeAccepted(offer);
            }
        }

        /// <summary>
        /// Executes the trade.
        /// </summary>
        /// <param name="offer">The offer.</param>
        /// <returns>The task.</returns>
        private async Task ExecuteTradeAccepted(OfferViewModel offer)
        {
            this.InsertEvent(offer.Event);
            if (!string.IsNullOrEmpty(this.SettingsService.ThankYouMessage))
            {
                await offer.ThankYou();
            }

            if (this.SettingsService.AutoKickEnabled)
            {
                await this._keyboardHelper.Kick(offer.PlayerName);
            }

            var itemClass = offer.Event.ItemClass;
            if (itemClass != ItemClass.Map && itemClass != ItemClass.Currency && itemClass != ItemClass.DivinationCard)
            {
                var alreadySold = this.CheckIfOfferIsAlreadySold(offer.Event);
                if (!alreadySold)
                {
                    this.AddToSoldOffer(offer);
                }
            }

            // Listener: LeagueViewModel
            this._eventAggregator.PublishOnUIThread(new TradeCompletedEvent());
            this.RemoveOffer(offer);
        }

        /// <summary>
        /// Adds to sold offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        private void AddToSoldOffer(OfferViewModel offer)
        {
            this._soldOffers.Add(offer.Event);
            if (this._soldOffers.Count >= 5)
            {
                this._soldOffers.RemoveAt(0);
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
            foreach (var offer in this.TradeOffers.Where(o => o.PlayerName == e.PlayerName))
            {
                playNotification = true;
                offer.BuyerInSameInstance = true;
            }

            if (playNotification && this.SettingsService.JoinHideoutEnabled)
            {
                this._soundService.PlayJoinHideout(this.SettingsService.JoinHideoutVolume);
            }
        }

        /// <summary>
        /// Lurkers the player left.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Lurker_PlayerLeft(object sender, PlayerLeftEvent e)
        {
            foreach (var offer in this.TradeOffers.Where(o => o.PlayerName == e.PlayerName))
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
                        this._removeActive?.Invoke();
                    }

                    this.TradeOffers.Remove(offer);
                    this._activeOffers.Remove(offer);
                    var activeOffer = this.ActiveOffer;
                    if (activeOffer != null)
                    {
                        activeOffer.Active = true;
                        this.SendToLifeBulb(activeOffer.Event);
                    }
                    else
                    {
                        this._removeActive?.Invoke();
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
            if (!this._activeOffers.Contains(offer))
            {
                this._activeOffers.Add(offer);
            }

            this.ActiveOffer.Active = true;

            if (offer.Equals(this.ActiveOffer))
            {
                this.SendToLifeBulb(this.ActiveOffer.Event);
            }
        }

        /// <summary>
        /// Sends to life bulb.
        /// </summary>
        /// <param name="tradeEvent">The trade event.</param>
        private void SendToLifeBulb(TradeEvent tradeEvent)
        {
            this._eventAggregator.PublishOnUIThread(new LifeBulbMessage()
            {
                View = new PositionViewModel(tradeEvent, this.SettingsService),
                OnShow = (a) => { this._removeActive = a; },
                Action = this.SearchItem,
            });
        }

        /// <summary>
        /// Sets the active offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        private void SetActiveOffer(OfferViewModel offer)
        {
            this.DockingHelper.SetForeground();

            var currentActiveOffer = this.ActiveOffer;
            if (currentActiveOffer == null)
            {
                this.AddActiveOffer(offer);
                return;
            }

            if (currentActiveOffer == offer)
            {
                return;
            }

            currentActiveOffer.Active = false;

            var index = this._activeOffers.IndexOf(offer);
            if (index != -1)
            {
                this._activeOffers.RemoveAt(index);
            }

            this._activeOffers.Insert(0, offer);
            this.ActiveOffer.Active = true;

            this.SendToLifeBulb(offer.Event);
        }

        /// <summary>
        /// Clears the offers.
        /// </summary>
        private void ClearAll()
        {
            this.TradeOffers.Clear();
            this._activeOffers.Clear();
            this._removeActive?.Invoke();

            this.NotifyOfPropertyChange("ActiveOffer");
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                this.SettingsService.OnSave -= this.SettingsService_OnSave;

                this._keyboardLurker.InvitePressed -= this.KeyboardLurker_InvitePressed;
                this._keyboardLurker.TradePressed -= this.KeyboardLurker_TradePressed;
                this._keyboardLurker.BusyPressed -= this.KeyboardLurker_BusyPressed;
                this._keyboardLurker.DismissPressed -= this.KeyboardLurker_DismissPressed;

                this._clientLurker.IncomingOffer -= this.Lurker_IncomingOffer;
                this._clientLurker.TradeAccepted -= this.Lurker_TradeAccepted;
                this._clientLurker.PlayerJoined -= this.Lurker_PlayerJoined;
                this._clientLurker.PlayerLeft -= this.Lurker_PlayerLeft;
                this.TradeOffers.Clear();
            }

            base.OnDeactivate(close);
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected async override void OnActivate()
        {
            await this._pushBulletService.CheckPledgeStatus();

            this.SettingsService.OnSave += this.SettingsService_OnSave;

            this._keyboardLurker.MainActionPressed += this.KeyboardLurker_MainActionPressed;
            this._keyboardLurker.InvitePressed += this.KeyboardLurker_InvitePressed;
            this._keyboardLurker.TradePressed += this.KeyboardLurker_TradePressed;
            this._keyboardLurker.BusyPressed += this.KeyboardLurker_BusyPressed;
            this._keyboardLurker.DismissPressed += this.KeyboardLurker_DismissPressed;

            this._keyboardLurker.InstallHandlers();

            this._clientLurker.IncomingOffer += this.Lurker_IncomingOffer;
            this._clientLurker.TradeAccepted += this.Lurker_TradeAccepted;
            this._clientLurker.PlayerJoined += this.Lurker_PlayerJoined;
            this._clientLurker.PlayerLeft += this.Lurker_PlayerLeft;

            base.OnActivate();
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="windowInformation">The window information.</param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            // When Poe Lurker is updated we save the settings before the view are loaded
            if (this.View == null)
            {
                return;
            }

            var overlayHeight = DefaultOverlayHeight * windowInformation.FlaskBarHeight / DefaultFlaskBarHeight * this.SettingsService.TradebarScaling;
            var overlayWidth = (windowInformation.Width - (windowInformation.FlaskBarWidth * 2)) / 2;
            var margin = PoeApplicationContext.WindowStyle == WindowStyle.Windowed ? 10 : 0;

            Execute.OnUIThread(() =>
            {
                this.View.Height = this.ApplyScalingY(overlayHeight);
                this.View.Width = this.ApplyScalingX(overlayWidth);
                this.View.Left = this.ApplyScalingX(windowInformation.Position.Left + windowInformation.FlaskBarWidth + Margin - margin);
                this.View.Top = this.ApplyScalingY(windowInformation.Position.Bottom - overlayHeight - windowInformation.ExpBarHeight - Margin - margin);
            });
        }

        /// <summary>
        /// Handles the OnSave event of the _settingsService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SettingsService_OnSave(object sender, System.EventArgs e)
        {
            if (this.DockingHelper.WindowInformation != null)
            {
                this.SetWindowPosition(this.DockingHelper.WindowInformation);
            }
        }

        #endregion
    }
}