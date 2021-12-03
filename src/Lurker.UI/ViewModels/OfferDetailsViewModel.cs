//-----------------------------------------------------------------------
// <copyright file="OfferDetailsViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using Lurker.Patreon.Events;
    using Lurker.Patreon.Services;
    using Lurker.Services;

    /// <summary>
    /// Represents the offer details.
    /// </summary>
    public class OfferDetailsViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private TradeEvent _event;
        private double _exaltRatio;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferDetailsViewModel" /> class.
        /// </summary>
        /// <param name="tradeEvent">The event.</param>
        public OfferDetailsViewModel(TradeEvent tradeEvent)
        {
            this._event = tradeEvent;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the DecimalChaosValue.
        /// </summary>
        public int DecimalChaosValue { get; private set; }

        /// <summary>
        /// Gets the Decimal.
        /// </summary>
        public double Decimal { get; private set; }

        /// <summary>
        /// Gets a value indicating whether has decimal.
        /// </summary>
        public bool HasDecimal => this.Decimal > 0;

        /// <summary>
        /// Gets the ExaltRatio.
        /// </summary>
        public double ExaltRatio
        {
            get
            {
                return this._exaltRatio;
            }

            private set
            {
                this._exaltRatio = value;
                this.NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initialize the viewmodel.
        /// </summary>
        /// <returns>
        /// The task.
        /// </returns>
        public async Task Initialize()
        {
            using (var service = new PoeNinjaService())
            {
                var ratio = await service.GetExaltRationAsync(this._event.LeagueName);
                this.ExaltRatio = Math.Round(ratio);
            }

            var value = this._event.Price.NumberOfCurrencies % 1;
            this.Decimal = Math.Round(value, 2);
            this.DecimalChaosValue = Convert.ToInt32(this.Decimal * this.ExaltRatio);
        }

        #endregion
    }
}