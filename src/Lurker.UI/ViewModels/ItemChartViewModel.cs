//-----------------------------------------------------------------------
// <copyright file="ItemChartViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Linq;
    using Caliburn.Micro;
    using Lurker.Models;
    using PoeLurker.Patreon.Models.Ninja;

    /// <summary>
    /// Represents ItemChartViewModel.
    /// </summary>
    public class ItemChartViewModel : PropertyChangedBase
    {
        #region Fields

        private ItemLine _line;
        private UniqueItem _item;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemChartViewModel"/> class.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="item">The item.</param>
        public ItemChartViewModel(ItemLine line, UniqueItem item)
        {
            this._line = line;
            this._item = item;

            this.Price = this.PriceInDivine ? line.DivineValue : line.ChaosValue;
            this.Chart = new LineChartViewModel(false, 0);

            var buyPoints = this._line.PriceLine.Data.Where(d => d.HasValue);
            if (buyPoints.Any())
            {
                var minimum = Math.Abs(buyPoints.Min().Value);
                this.Chart.Add("Buy", buyPoints.Select(p => Convert.ToDouble(p + minimum + 1)));
            }

            this.TotalChange = $"{this._line.PriceLine.TotalChange}%";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the price.
        /// </summary>
        public double Price { get; private set; }

        /// <summary>
        /// Gets the chart.
        /// </summary>
        public LineChartViewModel Chart { get; private set; }

        /// <summary>
        /// Gets the total change.
        /// </summary>
        public string TotalChange { get; private set; }

        /// <summary>
        /// Gets the image.
        /// </summary>
        public Uri ImageUrl => this._item.ImageUrl;

        /// <summary>
        /// Gets a value indicating whether the total change is negative.
        /// </summary>
        public bool Negative => this._line.PriceLine.TotalChange < 0;

        /// <summary>
        /// Gets a value indicating whether the total change is negative.
        /// </summary>
        public bool PriceInDivine => this._line.DivineValue >= 1;

        /// <summary>
        /// Gets a value indicating whether the total change is negative.
        /// </summary>
        public bool PriceInChaos => this._line.DivineValue < 1;

        #endregion
    }
}