//-----------------------------------------------------------------------
// <copyright file="TradeValueViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Lurker.Patreon.Events;
    using Lurker.Patreon.Models;

    /// <summary>
    /// Represetns the trade value.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class TradeValueViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private TradeEvent _tradeEvent;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeValueViewModel"/> class.
        /// </summary>
        /// <param name="tradeEvent">The trade event.</param>
        public TradeValueViewModel(TradeEvent tradeEvent)
        {
            this._tradeEvent = tradeEvent;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the type of the currency.
        /// </summary>
        public CurrencyType CurrencyType => this._tradeEvent.Price.CurrencyType;

        /// <summary>
        /// Gets the number off currency.
        /// </summary>
        public double NumberOffCurrency => this._tradeEvent.Price.NumberOfCurrencies;

        #endregion
    }
}