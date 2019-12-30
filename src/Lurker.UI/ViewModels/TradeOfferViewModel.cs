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
    using System;

    public class TradeOfferViewModel: PropertyChangedBase
    {
        #region Fields

        private TradeEvent _tradeEvent;
        private PoeKeyboardHelper _keyboardHelper;
        private OfferStatus _status;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeOfferViewModel"/> class.
        /// </summary>
        /// <param name="tradeEvent">The trade event.</param>
        public TradeOfferViewModel(TradeEvent tradeEvent, PoeKeyboardHelper keyboardHelper)
        {
            this._tradeEvent = tradeEvent;
            this._keyboardHelper = keyboardHelper;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the number off currency.
        /// </summary>
        public int NumberOffCurrency => this._tradeEvent.Price.NumberOfCurrencies;

        public string ItemName => this._tradeEvent.ItemName;

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

        #endregion

        #region 

        /// <summary>
        /// Answers this instance.
        /// </summary>
        public void Answer()
        {
            var playerName = this._tradeEvent.PlayerName;

            switch (this._status)
            {
                case OfferStatus.Pending:
                    this.Status = OfferStatus.Invited;
                    this._keyboardHelper.Invite(playerName);
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
