//-----------------------------------------------------------------------
// <copyright file="OutgoingbarViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Timers;
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Patreon.Events;
    using Lurker.Patreon.Services;
    using Lurker.Services;
    using Lurker.UI.Models;

    /// <summary>
    /// Represents the outgoing bar view model.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.ScreenBase" />
    /// <seealso cref="Caliburn.Micro.IViewAware" />
    public class OutgoingbarViewModel : PoeOverlayBase
    {
        #region Fields

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private ClipboardLurker _clipboardLurker;
        private ClientLurker _clientLurker;
        private PoeKeyboardHelper _keyboardHelper;
        private Timer _timer;
        private IEventAggregator _eventAggregator;
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
            ProcessLurker processLurker,
            DockingHelper dockingHelper,
            PoeKeyboardHelper keyboardHelper,
            SettingsService settingsService,
            IWindowManager windowManager)
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            this.Offers = new ObservableCollection<OutgoingOfferViewModel>();
            this.FilteredOffers = new ObservableCollection<OutgoingOfferViewModel>();
            this._timer = new Timer(50);
            this._timer.Elapsed += this.Timer_Elapsed;
            this._keyboardHelper = keyboardHelper;
            this._clipboardLurker = clipboardLurker;
            this._eventAggregator = eventAggregator;
            this._clientLurker = clientLurker;

            this.Offers.CollectionChanged += this.Offers_CollectionChanged;
            this._clipboardLurker.NewOffer += this.ClipboardLurker_NewOffer;
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
                return this._searchValue;
            }

            set
            {
                this._searchValue = value;
                this.OnSearchValueChange(value);
                this.NotifyOfPropertyChange();
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
        public bool AnyOffer => this.Offers.Any();

        /// <summary>
        /// Gets the default width.
        /// </summary>
        protected static int DefaultWidth => 300;

        #endregion

        #region Methods

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            this._clientLurker.OutgoingOffer += this.Lurker_OutgoingOffer;
            this._clientLurker.TradeAccepted += this.Lurker_TradeAccepted;

            base.OnActivate();
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                this._clientLurker.OutgoingOffer -= this.Lurker_OutgoingOffer;
                this._clientLurker.TradeAccepted -= this.Lurker_TradeAccepted;
            }

            base.OnDeactivate(close);
        }

        /// <summary>
        /// Creates the context.
        /// </summary>
        /// <returns>The context.</returns>
        private OutgoingbarContext CreateContext()
        {
            return new OutgoingbarContext(this.RemoveOffer, this.SetActiveOffer, this.ClearAll);
        }

        /// <summary>
        /// Called when [search value change].
        /// </summary>
        /// <param name="value">The value.</param>
        private void OnSearchValueChange(string value)
        {
            Execute.OnUIThread(() =>
            {
                this.FilteredOffers.Clear();
                var offers = string.IsNullOrEmpty(value) ? this.Offers.ToArray() : this.Offers.Where(o => o.PlayerName.ToLower().Contains(value.ToLower())).OrderBy(o => o.PlayerName).ToArray();
                foreach (var offer in offers)
                {
                    this.FilteredOffers.Add(offer);
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
            if (!this.SettingsService.ClipboardEnabled || this._lastOutgoingOfferText == newOfferText)
            {
                return;
            }

            this._lastOutgoingOfferText = newOfferText;
            await this._keyboardHelper.SendMessage(newOfferText);
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
                foreach (var offer in this.Offers.Where(o => !o.Waiting && !o.Active))
                {
                    offer.DelayToClose = offer.DelayToClose - 0.15;

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
        private void Lurker_OutgoingOffer(object sender, Patreon.Events.OutgoingTradeEvent e)
        {
            if (this.Offers.Any(o => o.Event.Equals(e)))
            {
                return;
            }

            Execute.OnUIThread(() =>
            {
                var index = 0;
                if (this.AnyOffer)
                {
                    var value = e.Price.CalculateValue();
                    var closestOffer = this.Offers.Aggregate((x, y) => Math.Abs(x.PriceValue - value) < Math.Abs(y.PriceValue - value) ? x : y);
                    index = this.Offers.IndexOf(closestOffer);

                    if (value >= closestOffer.PriceValue)
                    {
                        index++;
                    }
                }

                this.Offers.Insert(index, new OutgoingOfferViewModel(e, this._keyboardHelper, this.CreateContext(), this.DockingHelper));
            });
        }

        /// <summary>
        /// Lurkers the trade accepted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private async void Lurker_TradeAccepted(object sender, Patreon.Events.TradeAcceptedEvent e)
        {
            if (this._activeOffer == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(this.SettingsService.ThankYouMessage))
            {
                var tradeEvent = this._activeOffer.Event;
                await this._keyboardHelper.Whisper(tradeEvent.PlayerName, TokenHelper.ReplaceToken(this.SettingsService.ThankYouMessage, tradeEvent));
            }

            if (this.SettingsService.AutoKickEnabled)
            {
                await this._keyboardHelper.Leave();
            }

            await this.InsertEvent(this._activeOffer.Event);
            this.RemoveOffer(this._activeOffer);
            this._activeOffer = null;
            this._removeActive = null;
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
            if (offer == this._activeOffer)
            {
                this._removeActive?.Invoke();
            }

            this._timer.Stop();
            Execute.OnUIThread(() => this.Offers.Remove(offer));
            this._timer.Start();
        }

        /// <summary>
        /// Clears all.
        /// </summary>
        private void ClearAll()
        {
            this._removeActive?.Invoke();
            this.Offers.Clear();
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
                this.View.Height = this.ApplyScalingY(height < 0 ? 1 : height);
                this.View.Width = this.ApplyScalingX(width);
                this.View.Left = this.ApplyScalingX(windowInformation.Position.Left + yPosition);
                this.View.Top = this.ApplyScalingY(windowInformation.Position.Bottom - height - 12 + Margin);
            });
        }

        /// <summary>
        /// Handles the CollectionChanged event of the Offers control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Collections.Specialized.NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void Offers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.Offers.Any(o => !o.Waiting))
            {
                this._timer.Start();
            }
            else
            {
                this._timer.Stop();
            }

            if (e.NewItems != null)
            {
                foreach (var offer in e.NewItems)
                {
                    var outgoingOffer = offer as OutgoingOfferViewModel;
                    if (string.IsNullOrEmpty(this.SearchValue))
                    {
                        this.FilteredOffers.Insert(this.Offers.IndexOf(outgoingOffer), outgoingOffer);
                    }
                    else if (outgoingOffer.PlayerName.ToLower().Contains(this.SearchValue.ToLower()))
                    {
                        this.FilteredOffers.Insert(0, outgoingOffer);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var offer in e.OldItems)
                {
                    this.FilteredOffers.Remove(offer as OutgoingOfferViewModel);
                }
            }

            if (!this.AnyOffer)
            {
                this.SearchValue = string.Empty;
            }

            this.NotifyOfPropertyChange("AnyOffer");
        }

        /// <summary>
        /// Sets the active offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        private void SetActiveOffer(OutgoingOfferViewModel offer)
        {
            this.DockingHelper.SetForeground();

            if (offer.Active)
            {
                return;
            }

            if (this._activeOffer != null)
            {
                this._activeOffer.Active = false;
            }

            this._activeOffer = offer;
            this._activeOffer.SetActive();

            this._eventAggregator.PublishOnUIThread(new LifeBulbMessage()
            {
                View = new TradeValueViewModel(offer.Event, this.SettingsService),
                OnShow = (a) => { this._removeActive = a; },
                Action = async () => { await this._keyboardHelper.Trade(offer.Event.PlayerName); },
            });
        }

        #endregion
    }
}