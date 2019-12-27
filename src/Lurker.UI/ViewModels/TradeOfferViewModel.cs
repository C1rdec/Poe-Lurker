//-----------------------------------------------------------------------
// <copyright file="TradeOfferViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Lurker.Events;
    using Lurker.Models;
    using System;

    public class TradeOfferViewModel
    {
        #region Fields

        private TradeEvent _tradeEvent;
        private Action<TradeOfferViewModel> _deleteAction;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeOfferViewModel"/> class.
        /// </summary>
        /// <param name="tradeEvent">The trade event.</param>
        public TradeOfferViewModel(TradeEvent tradeEvent, Action<TradeOfferViewModel> deleteAction)
        {
            this._tradeEvent = tradeEvent;
            this._deleteAction = deleteAction;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the number off currency.
        /// </summary>
        public int NumberOffCurrency => this._tradeEvent.Price.NumberOfCurrencies;

        /// <summary>
        /// Gets the type of the currency.
        /// </summary>
        public CurrencyType CurrencyType => this._tradeEvent.Price.CurrencyType;

        #endregion

        #region 

        /// <summary>
        /// Answers this instance.
        /// </summary>
        public void Answer()
        {
            this._deleteAction(this);
        }

        #endregion
    }
}
