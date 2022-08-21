//-----------------------------------------------------------------------
// <copyright file="OfferViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Patreon.Events;
    using Lurker.Patreon.Models;
    using Lurker.Services;
    using Lurker.UI.Models;
    using Lurker.UI.Views;

    /// <summary>
    /// Represents an Offer.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    /// <seealso cref="System.IDisposable" />
    public class OfferViewModel : Screen, IDisposable
    {
        #region Fields

        private TradeEvent _tradeEvent;
        private TimeSpan _elapsedTime;
        private PoeKeyboardHelper _keyboardHelper;
        private TradebarContext _tradebarContext;
        private SettingsService _settingsService;
        private OfferStatus _status;
        private bool _waiting;
        private bool _active;
        private bool _skipMainAction;
        private bool _buyerInSameInstance;
        private bool _alreadySold;
        private bool _showDetail;
        private CancellationTokenSource _tokenSource;
        private DockingHelper _dockingHelper;
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
            this._tradeEvent = tradeEvent;
            this._keyboardHelper = keyboardHelper;
            this._tradebarContext = tradebarContext;
            this._settingsService = settingsService;
            this._alreadySold = sold;
            this._dockingHelper = dockingHelper;

            this._settingsService.OnSave += this.SettingsService_OnSave;
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
        public bool AlreadySold => this._alreadySold;

        /// <summary>
        /// Gets a value indicating whether [not sold].
        /// </summary>
        public bool NotSold => !this._alreadySold;

        /// <summary>
        /// Gets the number off currency.
        /// </summary>
        public double NumberOffCurrency => this._tradeEvent.Price.NumberOfCurrencies;

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        public string ItemName => this._tradeEvent.ItemName;

        /// <summary>
        /// Gets the location.
        /// </summary>
        public Location Location => this._tradeEvent.Location;

        /// <summary>
        /// Gets the name of the player.
        /// </summary>
        public string PlayerName => this._tradeEvent.PlayerName;

        /// <summary>
        /// Gets the type of the currency.
        /// </summary>
        public CurrencyType CurrencyType => this._tradeEvent.Price.CurrencyType;

        /// <summary>
        /// Gets the price.
        /// </summary>
        public Price Price => this._tradeEvent.Price;

        /// <summary>
        /// Gets or sets the time span since the offer was received.
        /// </summary>
        public TimeSpan Elapsed
        {
            get
            {
                return this._elapsedTime;
            }

            set
            {
                this._elapsedTime = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is invite sended.
        /// </summary>
        public OfferStatus Status
        {
            get
            {
                return this._status;
            }

            set
            {
                this._status = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OfferViewModel"/> is ShowDetail.
        /// </summary>
        public bool ShowDetail
        {
            get
            {
                return this._showDetail;
            }

            set
            {
                this._showDetail = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OfferViewModel"/> is waiting.
        /// </summary>
        public bool Waiting
        {
            get
            {
                return this._waiting;
            }

            set
            {
                this._waiting = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OfferViewModel"/> is active.
        /// </summary>
        public bool Active
        {
            get
            {
                return this._active;
            }

            set
            {
                this._active = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [buyer in same instance].
        /// </summary>
        public bool BuyerInSameInstance
        {
            get
            {
                return this._buyerInSameInstance;
            }

            set
            {
                this._buyerInSameInstance = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the Details.
        /// </summary>
        public OfferDetailsViewModel Details
        {
            get
            {
                return this._details;
            }

            private set
            {
                this._details = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets a value indicating whether [tool tip enabled].
        /// </summary>
        public bool ToolTipEnabled => this._settingsService.ToolTipEnabled;

        /// <summary>
        /// Gets the tool tip delay.
        /// </summary>
        public int ToolTipDelay => this._settingsService.ToolTipDelay;

        /// <summary>
        /// Gets the event.
        /// </summary>
        public TradeEvent Event => this._tradeEvent;

        /// <summary>
        /// Gets the date.
        /// </summary>
        public string Date => this._tradeEvent.Date.ToString("h:mm tt");

        #endregion

        #region Methods

        /// <summary>
        /// Dismisses the cart.
        /// </summary>
        public void DismissCart()
        {
            this._skipMainAction = true;
            this._alreadySold = false;
            this.NotifyOfPropertyChange(() => this.AlreadySold);
            this.NotifyOfPropertyChange(() => this.NotSold);
        }

        /// <summary>
        /// Whoes the is.
        /// </summary>
        public async void RightClick()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                await this._keyboardHelper.WhoIs(this.PlayerName);
                return;
            }

            if (this._tradeEvent.Price.CurrencyType == CurrencyType.Divine)
            {
                var detail = new OfferDetailsViewModel(this._tradeEvent);
                await detail.Initialize();
                this.Details = detail;
                this.ShowDetail = !this.ShowDetail;
            }
            else
            {
                await this._keyboardHelper.WhoIs(this.PlayerName);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Removes this instance.
        /// </summary>
        public void Remove()
        {
            this._skipMainAction = true;
            Execute.OnUIThread(() =>
            {
                if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    this._tradebarContext.ClearAll();
                }
                else
                {
                    this.RemoveFromTradebar();
                }

                this._dockingHelper.SetForeground();
            });
        }

        /// <summary>
        /// Waits this instance.
        /// </summary>
        public async void Wait()
        {
            this._skipMainAction = true;
            this.Waiting = true;
            await this.Whisper(this._settingsService.BusyMessage);
        }

        /// <summary>
        /// Answers this instance.
        /// </summary>
        public async void MainAction()
        {
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                this._tradebarContext.SetActiveOffer(this);
                return;
            }

            if (this._skipMainAction)
            {
                this._skipMainAction = false;
                return;
            }

            if (this.AlreadySold)
            {
                await this.Sold();
                return;
            }

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    await this.StillInterested();
                    return;
                }

                this._tradebarContext.AddToSoldOffer(this);
                await this.Sold();
                return;
            }

            await this.MainActionCore();
        }

        /// <summary>
        /// Mains the action core.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task MainActionCore()
        {
            switch (this._status)
            {
                case OfferStatus.Pending:
                    await this.Invite();
                    break;
                case OfferStatus.Invited:
                case OfferStatus.Traded:
                    await this.Trade();
                    break;
            }
        }

        /// <summary>
        /// Builds the name of the search item.
        /// </summary>
        /// <returns>The item name to be placed in tab search.</returns>
        public string BuildSearchItemName()
        {
            return this._tradeEvent.BuildSearchItemName();
        }

        /// <summary>
        /// Thanks you.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task ThankYou()
        {
            await this.Whisper(this._settingsService.ThankYouMessage);
        }

        /// <summary>
        /// Called when [mouse down].
        /// </summary>
        public async void OnMouseDown()
        {
            if (this._tokenSource != null || this.Status == OfferStatus.Pending)
            {
                return;
            }

            try
            {
                this._tokenSource = new CancellationTokenSource();
                await Task.Delay(500);
                if (this._tokenSource.IsCancellationRequested)
                {
                    return;
                }

                await this._keyboardHelper.Invite(this.PlayerName);
            }
            finally
            {
                this._tokenSource.Dispose();
                this._tokenSource = null;
            }
        }

        /// <summary>
        /// Called when [mouse up].
        /// </summary>
        public void OnMouseUp()
        {
            if (this._tokenSource != null)
            {
                this._tokenSource.Cancel();
            }
        }

        /// <summary>
        /// Called when [mouse leave].
        /// </summary>
        public void OnMouseLeave()
        {
            this.ShowDetail = false;
        }

        /// <summary>
        /// Invites the buyer.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task Invite()
        {
            this.Status = OfferStatus.Invited;
            await this._keyboardHelper.Invite(this.PlayerName);
            this._tradebarContext.AddToActiveOffer(this);
        }

        /// <summary>
        /// Trades the Buyer.
        /// </summary>
        /// <returns>the task.</returns>
        public async Task Trade()
        {
            this.Status = OfferStatus.Traded;
            await this._keyboardHelper.Trade(this.PlayerName);
        }

        /// <summary>
        /// Stills the interested.
        /// </summary>
        /// <returns>The task.</returns>
        public async Task StillInterested()
        {
            await this.Whisper(this._settingsService.StillInterestedMessage);
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view">The view.</param>
        protected override void OnViewLoaded(object view)
        {
            var offerView = view as OfferView;
            this.ButtonHeight = offerView.ActualHeight / 3;
            this.FontSize = offerView.ActualHeight / 4;

            this.NotifyOfPropertyChange("ButtonHeight");
            this.NotifyOfPropertyChange("FontSize");
        }

        /// <summary>
        /// Tell the buyer that the item is sold.
        /// </summary>
        private async Task Sold()
        {
            await this.Whisper(this._settingsService.SoldMessage);
            this.RemoveFromTradebar();
        }

        /// <summary>
        /// Removes this instance.
        /// </summary>
        private void RemoveFromTradebar()
        {
            this._tradebarContext.RemoveOffer(this);
        }

        /// <summary>
        /// Whispers the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        private async Task Whisper(string message)
        {
            await this._keyboardHelper.Whisper(this.PlayerName, TokenHelper.ReplaceToken(message, this._tradeEvent));
        }

        /// <summary>
        /// Handles the OnSave event of the SettingsService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void SettingsService_OnSave(object sender, System.EventArgs e)
        {
            this.NotifyOfPropertyChange(nameof(this.ToolTipEnabled));
            this.NotifyOfPropertyChange(nameof(this.ToolTipDelay));
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._settingsService.OnSave -= this.SettingsService_OnSave;
            }
        }

        #endregion
    }
}