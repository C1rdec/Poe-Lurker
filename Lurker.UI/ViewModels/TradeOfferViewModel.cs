//-----------------------------------------------------------------------
// <copyright file="TradeOfferViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Lurker.Events;
    using Lurker.Models;

    public class TradeOfferViewModel
    {
        #region MyRegion

        private TradeEvent _tradeEvent;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeOfferViewModel"/> class.
        /// </summary>
        /// <param name="tradeEvent">The trade event.</param>
        public TradeOfferViewModel(TradeEvent tradeEvent)
        {
            this._tradeEvent = tradeEvent;
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
    }
}
