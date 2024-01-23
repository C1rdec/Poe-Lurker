//-----------------------------------------------------------------------
// <copyright file="ChartViewModelBase.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels
{
    using System;
    using Caliburn.Micro;
    using LiveCharts;
    using PoeLurker.UI.Models;

    /// <summary>
    /// Represent the chart.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class ChartViewModelBase : PropertyChangedBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartViewModelBase"/> class.
        /// </summary>
        /// <param name="labels">The labels.</param>
        public ChartViewModelBase(string[] labels)
            : this()
        {
            this.Labels = labels;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartViewModelBase"/> class.
        /// </summary>
        public ChartViewModelBase()
        {
            this.SeriesCollection = new SeriesCollection();
            this.LegendLocation = LegendLocation.Bottom;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the labels.
        /// </summary>
        public string[] Labels { get; set; }

        /// <summary>
        /// Gets or sets the series collection.
        /// </summary>
        public SeriesCollection SeriesCollection { get; set; }

        /// <summary>
        /// Creates new offer.
        /// </summary>
        public event EventHandler<ChartPoint> OnSerieClick;

        /// <summary>
        /// Gets the data click command.
        /// </summary>
        public MyCommand<ChartPoint> DataClickCommand => new MyCommand<ChartPoint>()
        {
            ExecuteDelegate = p => this.OnClick(p),
        };

        /// <summary>
        /// Gets or sets the legend location.
        /// </summary>
        public LegendLocation LegendLocation { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Called when [click].
        /// </summary>
        /// <param name="point">The point.</param>
        private void OnClick(ChartPoint point)
        {
            this.OnSerieClick?.Invoke(this, point);
        }

        #endregion
    }
}