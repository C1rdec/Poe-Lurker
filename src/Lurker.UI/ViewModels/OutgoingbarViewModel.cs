//-----------------------------------------------------------------------
// <copyright file="OutgoingbarViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Services;
    using Lurker.UI.Helpers;
    using Lurker.UI.Models;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Timers;

    /// <summary>
    /// Represents the outgoing bar view model.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.ScreenBase" />
    /// <seealso cref="Caliburn.Micro.IViewAware" />
    public class OutgoingbarViewModel : PoeOverlayBase
    {
        #region Fields

        protected static int DefaultWidth = 55;
        private ClientLurker _clientLurker;
        private ClipboardLurker _clipboardLurker;
        private PoeKeyboardHelper _keyboardHelper;
        private Timer _timer;
        private IEventAggregator _eventAggregator;
        private OutgoingbarContext _context;
        private System.Action _removeActive;
        private OutgoingOfferViewModel _activeOffer;
        private string _lastOutgoingOfferText;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OutgoingbarViewModel"/> class.
        /// </summary>
        /// <param name="lurker">The lurker.</param>
        /// <param name="windowManager">The window manager.</param>
        public OutgoingbarViewModel(IEventAggregator eventAggregator, ClipboardLurker clipboardLurker, ClientLurker clientLurker, DockingHelper dockingHelper, PoeKeyboardHelper keyboardHelper, SettingsService settingsService, IWindowManager windowManager) 
            : base(windowManager, dockingHelper, clientLurker, settingsService)
        {
            this.Offers = new ObservableCollection<OutgoingOfferViewModel>();
            this._timer = new Timer(50);
            this._timer.Elapsed += this.Timer_Elapsed;
            this._keyboardHelper = keyboardHelper;
            this._clientLurker = clientLurker;
            this._clipboardLurker = clipboardLurker;
            this._eventAggregator = eventAggregator;

            this._clipboardLurker.NewOffer += this.ClipboardLurker_NewOffer;
            this._clientLurker.OutgoingOffer += this.Lurker_OutgoingOffer;
            this._clientLurker.TradeAccepted += this.Lurker_TradeAccepted;
            this.Offers.CollectionChanged += this.Offers_CollectionChanged;
            this._context = new OutgoingbarContext(this.RemoveOffer, this.SetActiveOffer);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the offers.
        /// </summary>
        public ObservableCollection<OutgoingOfferViewModel> Offers { get; set; }

        /// <summary>
        /// Gets a value indicating whether [any offer].
        /// </summary>
        public bool AnyOffer => this.Offers.Any();

        #endregion

        #region Methods

        /// <summary>
        /// Clipboards the lurker new offer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="newOfferText">The new offer text.</param>
        private void ClipboardLurker_NewOffer(object sender, string newOfferText)
        {
            if (!this._settingsService.ClipboardEnabled || this._lastOutgoingOfferText == newOfferText)
            {
                return;
            }

            this._lastOutgoingOfferText = newOfferText;
            this._keyboardHelper.SendMessage(newOfferText);
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
        private void Lurker_OutgoingOffer(object sender, Events.OutgoingTradeEvent e)
        {
            if (this.Offers.Any(o => o.Event.Equals(e)))
            {
                return;
            }

            Execute.OnUIThread(() => this.Offers.Insert(0, new OutgoingOfferViewModel(e, this._keyboardHelper, this._context)));
        }

        /// <summary>
        /// Lurkers the trade accepted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void Lurker_TradeAccepted(object sender, Events.TradeAcceptedEvent e)
        {
            if (this._activeOffer == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(this._settingsService.ThankYouMessage))
            {
                this._keyboardHelper.Whisper(this._activeOffer.Event.PlayerName, this._settingsService.ThankYouMessage);
            }

            this.RemoveOffer(this._activeOffer);
            this._activeOffer = null;
            this._removeActive = null;
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
        /// Sets the window position.
        /// </summary>
        /// <param name="windowInformation"></param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            var yPosition = windowInformation.FlaskBarWidth * (238 / (double)DefaultFlaskBarWidth);
            var width = windowInformation.Height * DefaultWidth / 1080;
            var height = windowInformation.FlaskBarHeight - (Margin * 2);
            Execute.OnUIThread(() =>
            {
                this._view.Height = height < 0 ? 1 : height;
                this._view.Width = width;
                this._view.Left = windowInformation.Position.Left + yPosition;
                this._view.Top = windowInformation.Position.Bottom - windowInformation.FlaskBarHeight + Margin;
            });
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
                this.Offers.CollectionChanged -= this.Offers_CollectionChanged;
            }

            base.OnDeactivate(close);
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

            this.NotifyOfPropertyChange("AnyOffer");
        }

        /// <summary>
        /// Sets the active offer.
        /// </summary>
        /// <param name="offer">The offer.</param>
        private void SetActiveOffer(OutgoingOfferViewModel offer)
        {
            if (this._activeOffer != null)
            {
                this._activeOffer.Active = false;
            }

            this._activeOffer = offer;
            this._activeOffer.SetActive();

            this._eventAggregator.PublishOnUIThread(new LifeBulbMessage()
            {
                View = new TradeValueViewModel(offer.Event),
                OnShow = (a) => { this._removeActive = a; },
                Action = () => { this._keyboardHelper.Trade(offer.Event.PlayerName); }
            });
        }

        #endregion
    }
}
