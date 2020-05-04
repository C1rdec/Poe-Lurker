//-----------------------------------------------------------------------
// <copyright file="TradeBarViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Patreon.Events;
    using Lurker.Patreon.Models;
    using Lurker.Patreon.Parsers;
    using Lurker.Services;
    using Lurker.UI.Models;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public class TradebarViewModel : PoeOverlayBase
    {
        #region Fields

        private static readonly ItemClassParser ItemClassParser = new ItemClassParser();
        private static int DefaultOverlayHeight = 60;
        private PoeKeyboardHelper _keyboardHelper;
        private ClientLurker _clientLurker;
        private TradebarContext _context;
        private List<OfferViewModel> _activeOffers = new List<OfferViewModel>();
        private IEventAggregator _eventAggregator;
        private System.Action _removeActive;
        private List<TradeEvent> _lastOffers;
        private SoundService _soundService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeBarViewModal"/> class.
        /// </summary>
        /// <param name="lurker">The lurker.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="keyboardHelper">The keyboard helper.</param>
        public TradebarViewModel(IEventAggregator eventAggregator, ClientLurker clientLurker, ProcessLurker processLurker, DockingHelper dockingHelper, PoeKeyboardHelper keyboardHelper, SettingsService settingsService, IWindowManager windowManager, SoundService soundService)
            : base (windowManager, dockingHelper, processLurker, settingsService)
        {
            this._eventAggregator = eventAggregator;
            this._keyboardHelper = keyboardHelper;
            this._settingsService = settingsService;
            this._soundService = soundService;
            this._clientLurker = clientLurker;
            this.TradeOffers = new ObservableCollection<OfferViewModel>();
            this._lastOffers = new List<TradeEvent>();

            this._clientLurker.IncomingOffer += this.Lurker_IncomingOffer;
            this._clientLurker.TradeAccepted += this.Lurker_TradeAccepted;
            this._clientLurker.PlayerJoined += this.Lurker_PlayerJoined;
            this._clientLurker.PlayerLeft += this.Lurker_PlayerLeft;

            this._context = new TradebarContext(this.RemoveOffer, this.AddActiveOffer, this.SetActiveOffer);
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
        public void SearchItem()
        {
            var activeOffer = this.ActiveOffer;
            if (activeOffer != null)
            {
                this._keyboardHelper.Search(activeOffer.BuildSearchItemName());
            }
        }

        /// <summary>
        /// Lurkers the new offer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The trade event.</param>
        private void Lurker_IncomingOffer(object sender, TradeEvent e)
        {
            if (this.TradeOffers.Any(o => o.Event.Equals(e)))
            {
                return;
            }

            if (this._settingsService.AlertEnabled)
            {
                this.PlayAlert();
            }

            Execute.OnUIThread(() => 
            {
                this.TradeOffers.Add(new OfferViewModel(e, this._keyboardHelper, this._context, this._settingsService, this.CheckIfOfferIsAlreadySold(e)));
            });
        }

        /// <summary>
        /// Checks if offer is already sold.
        /// </summary>
        /// <param name="tradeEvent">The trade event.</param>
        private bool CheckIfOfferIsAlreadySold(TradeEvent tradeEvent)
        {
            var location = tradeEvent.Location.ToString();
            var defaultLocation = new Location().ToString();
            if (location != defaultLocation)
            {
                foreach (var lastOffer in this._lastOffers)
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
            this._soundService.PlayTradeAlert(this._settingsService.AlertVolume);
        }

        /// <summary>
        /// Lurkers the trade accepted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Lurker_TradeAccepted(object sender, TradeAcceptedEvent e)
        {
            var offer = this.TradeOffers.Where(t => t.Status == OfferStatus.Traded).FirstOrDefault();
            if (offer != null)
            {
                this.InsertEvent(offer.Event);
                if (!string.IsNullOrEmpty(this._settingsService.ThankYouMessage))
                {
                    offer.ThankYou();
                }

                if (this._settingsService.AutoKickEnabled)
                {
                    this._keyboardHelper.Kick(offer.PlayerName);
                }

                var itemClass = offer.Event.ItemClass;
                if (itemClass != ItemClass.Map && itemClass != ItemClass.Currency && itemClass != ItemClass.DivinationCard)
                {
                    var alreadySold = this.CheckIfOfferIsAlreadySold(offer.Event);
                    if (!alreadySold)
                    {
                        this._lastOffers.Add(offer.Event);
                        if (this._lastOffers.Count >= 5)
                        {
                            this._lastOffers.RemoveAt(0);
                        }
                    }
                }

                this.RemoveOffer(offer);
            }
        }

        /// <summary>
        /// Lurkers the player joined.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The PLayerJoined Event.</param>
        private void Lurker_PlayerJoined(object sender, PlayerJoinedEvent e)
        {
            foreach (var offer in this.TradeOffers.Where(o => o.PlayerName == e.PlayerName))
            {
                offer.BuyerInSameInstance = true;
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
            using (var service = new Lurker.Patreon.DatabaseService())
            {
                service.Insert(tradeEvent);
            }
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
                    this.TradeOffers.Remove(offer);
                    this._activeOffers.Remove(offer);

                    var activeOffer = this.ActiveOffer;
                    if (this.ActiveOffer != null)
                    {
                        this.ActiveOffer.Active = true;
                        this.SendToLifeBulb(this.ActiveOffer.Event);
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
            this._activeOffers.Add(offer);
            this.ActiveOffer.Active = true;


            this.SendToLifeBulb(this.ActiveOffer.Event);
        }

        /// <summary>
        /// Sends to life bulb.
        /// </summary>
        /// <param name="tradeEvent">The trade event.</param>
        private void SendToLifeBulb(TradeEvent tradeEvent)
        {
            this._eventAggregator.PublishOnUIThread(new LifeBulbMessage()
            {
                View = new PositionViewModel(tradeEvent, this._settingsService),
                OnShow = (a) => { this._removeActive = a; },
                Action = this.SearchItem
            });
        }

        /// <summary>
        /// Sets the active offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        private void SetActiveOffer(OfferViewModel offer)
        {
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
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                this._clientLurker.IncomingOffer -= this.Lurker_IncomingOffer;
                this._clientLurker.TradeAccepted -= this.Lurker_TradeAccepted;
                this._clientLurker.PlayerJoined -= this.Lurker_PlayerJoined;
                this._clientLurker.PlayerLeft -= this.Lurker_PlayerLeft;
            }

            base.OnDeactivate(close);
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="windowInformation"></param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            var overlayHeight = DefaultOverlayHeight * windowInformation.FlaskBarHeight / DefaultFlaskBarHeight * this._settingsService.TradebarScaling;
            var overlayWidth = (windowInformation.Width - (windowInformation.FlaskBarWidth * 2)) / 2;

            Execute.OnUIThread(() =>
            {
                this._view.Height = overlayHeight;
                this._view.Width = overlayWidth;
                this._view.Left = windowInformation.Position.Left + windowInformation.FlaskBarWidth + Margin;
                this._view.Top = windowInformation.Position.Bottom - overlayHeight - windowInformation.ExpBarHeight - Margin;
            });
        }

        #endregion
    }
}
