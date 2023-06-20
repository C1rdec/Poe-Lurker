//-----------------------------------------------------------------------
// <copyright file="DivineRatioViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Linq;
    using Caliburn.Micro;
    using PoeLurker.Patreon.Models.Ninja;

    /// <summary>
    /// Represents the Divine Ratio.
    /// </summary>
    public class DivineRatioViewModel : PropertyChangedBase
    {
        #region Fields

        private CurrencyLine _line;
        private double _ratio;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DivineRatioViewModel"/> class.
        /// </summary>
        /// <param name="line">The line.</param>
        public DivineRatioViewModel(CurrencyLine line)
        {
            this._line = line;
            this._ratio = line.ChaosEquivalent;
            this.Chart = new LineChartViewModel(false, 0);

            var buyPoints = line.BuyLine.Data.Where(d => d.HasValue);
            if (buyPoints.Any())
            {
                var minimum = Math.Abs(buyPoints.Min().Value);
                this.Chart.Add("Buy", buyPoints.Select(p => Convert.ToDouble(p + minimum + 1)));
            }

            this.TotalChange = $"{line.BuyLine.TotalChange}%";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ratio.
        /// </summary>
        public double Ratio
            => this.Fraction != null ? Math.Round(this._ratio * this.Fraction.Value, 2) : this._ratio;

        /// <summary>
        /// Gets the fraction.
        /// </summary>
        public double? Fraction { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the Fraction is visible.
        /// </summary>
        public bool HasFraction => this.Fraction != null;

        /// <summary>
        /// Gets the chart.
        /// </summary>
        public LineChartViewModel Chart { get; private set; }

        /// <summary>
        /// Gets the total change.
        /// </summary>
        public string TotalChange { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the total change is negative.
        /// </summary>
        public bool Negative => this._line.BuyLine.TotalChange < 0;

        #endregion

        #region Methods

        /// <summary>
        /// Set the fraction.
        /// </summary>
        /// <param name="fraction">The fraction.</param>
        public void SetFraction(double? fraction)
        {
            if (fraction == 0)
            {
                fraction = null;
            }

            this.Fraction = fraction;

            this.NotifyOfPropertyChange(() => this.Fraction);
            this.NotifyOfPropertyChange(() => this.HasFraction);
            this.NotifyOfPropertyChange(() => this.Ratio);
        }

        #endregion
    }
}