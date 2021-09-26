//-----------------------------------------------------------------------
// <copyright file="LineChartViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Collections.Generic;
    using LiveCharts;
    using LiveCharts.Wpf;

    /// <summary>
    /// Represents the line chart.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.ChartViewModelBase" />
    public class LineChartViewModel : ChartViewModelBase
    {
        #region Methods

        /// <summary>
        /// Adds the specified label.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="values">The values.</param>
        public void Add(string title, IEnumerable<double> values)
        {
            this.SeriesCollection.Add(new LineSeries
            {
                Title = title,
                Values = new ChartValues<double>(values),
                PointGeometry = null,
            });
        }

        #endregion
    }
}