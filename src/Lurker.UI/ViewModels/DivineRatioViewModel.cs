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
    using Lurker.Patreon.Models.Ninja;

    /// <summary>
    /// Represents the Divine Ratio.
    /// </summary>
    public class DivineRatioViewModel : PropertyChangedBase
    {
        #region Fields

        private CurrencyLine _line;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DivineRatioViewModel"/> class.
        /// </summary>
        /// <param name="line">The line.</param>
        public DivineRatioViewModel(CurrencyLine line)
        {
            this._line = line;
            this.Ratio = line.ChaosEquivalent;
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
        public double Ratio { get; private set; }

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
    }
}