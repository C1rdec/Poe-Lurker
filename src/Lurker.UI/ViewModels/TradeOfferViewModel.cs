//-----------------------------------------------------------------------
// <copyright file="TradeOfferViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;
    using Lurker.Events;
    using Lurker.Models;
    using Lurker.UI.Helpers;
    using Lurker.UI.Models;

    public class TradeOfferViewModel: PropertyChangedBase
    {
        #region Fields

        private bool _waiting;
        private TradeEvent _tradeEvent;
        private PoeKeyboardHelper _keyboardHelper;
        private OfferStatus _status;
        private TradebarContext _tradebarContext;
        private bool _skipMainAction;
        private bool _buyerInSameInstance;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeOfferViewModel"/> class.
        /// </summary>
        /// <param name="tradeEvent">The trade event.</param>
        public TradeOfferViewModel(TradeEvent tradeEvent, PoeKeyboardHelper keyboardHelper, TradebarContext tradebarContext)
        {
            this._tradeEvent = tradeEvent;
            this._keyboardHelper = keyboardHelper;
            this._tradebarContext = tradebarContext;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the number off currency.
        /// </summary>
        public double NumberOffCurrency => this._tradeEvent.Price.NumberOfCurrencies;

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        public string ItemName => this._tradeEvent.ItemName;

        /// <summary>
        /// Gets the location.
        /// </summary>
        public string Position => this._tradeEvent.Position;

        /// <summary>
        /// Gets the name of the player.
        /// </summary>
        public string PlayerName => this._tradeEvent.PlayerName;

        /// <summary>
        /// Gets the type of the currency.
        /// </summary>
        public CurrencyType CurrencyType => this._tradeEvent.Price.CurrencyType;

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
        /// Gets or sets a value indicating whether this <see cref="TradeOfferViewModel"/> is waiting.
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

        #endregion

        #region 

        /// <summary>
        /// Removes this instance.
        /// </summary>
        public void Remove()
        {
            this._skipMainAction = true;
            this._tradebarContext.RemoveOffer(this);
        }

        /// <summary>
        /// Waits this instance.
        /// </summary>
        public void Wait()
        {
            this._skipMainAction = true;
            this.Waiting = true;
            this._keyboardHelper.Whisper(this.PlayerName, "I'm busy right now I'll send you a party invite.");
        }

        /// <summary>
        /// Answers this instance.
        /// </summary>
        public void MainAction()
        {
            if (this._skipMainAction)
            {
                this._skipMainAction = false;
                return;
            }

            var playerName = this._tradeEvent.PlayerName;
            switch (this._status)
            {
                case OfferStatus.Pending:
                    this.Status = OfferStatus.Invited;
                    this._keyboardHelper.Invite(playerName);
                    this._tradebarContext.AddToActiveOffer(this);
                    break;
                case OfferStatus.Invited:
                case OfferStatus.Traded:
                    this.Status = OfferStatus.Traded;
                    this._keyboardHelper.Trade(playerName);
                    break;
            }
        }

        #endregion
    }
}
