//-----------------------------------------------------------------------
// <copyright file="LineChartViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Collections.Generic;
    using Caliburn.Micro;
    using LiveCharts;
    using LiveCharts.Wpf;

    /// <summary>
    /// Represents the line chart.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.ChartViewModelBase" />
    public class LineChartViewModel : ChartViewModelBase
    {
        #region Fields

        private bool _showLabels;
        private int _minYAxis;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineChartViewModel"/> class.
        /// </summary>
        public LineChartViewModel()
            : this(true, 0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineChartViewModel"/> class.
        /// </summary>
        /// <param name="showLabels">If we show the labels.</param>
        /// <param name="minYAxis">Minimum Y Value.</param>
        public LineChartViewModel(bool showLabels, int minYAxis)
        {
            this._showLabels = showLabels;
            this._minYAxis = minYAxis;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether we need to show labels.
        /// </summary>
        public bool ShowLabels => this._showLabels;

        /// <summary>
        /// Gets a value indicating the min Y Axis.
        /// </summary>
        public int MinYAxis => this._minYAxis;

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified label.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="values">The values.</param>
        public void Add(string title, IEnumerable<double> values)
        {
            Execute.OnUIThread(() =>
            {
                this.SeriesCollection.Add(new LineSeries
                {
                    Title = title,
                    Values = new ChartValues<double>(values),
                    PointGeometry = null,
                });
            });
        }

        #endregion
    }
}