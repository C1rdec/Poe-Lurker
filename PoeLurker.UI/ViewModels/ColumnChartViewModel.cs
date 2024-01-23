//-----------------------------------------------------------------------
// <copyright file="ColumnChartViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels
{
    using System.Collections.Generic;
    using LiveCharts;
    using LiveCharts.Wpf;

    /// <summary>
    /// Represents the ColumnChartViewModel.
    /// </summary>
    /// <seealso cref="PoeLurker.UI.ViewModels.ChartViewModelBase" />
    public class ColumnChartViewModel : ChartViewModelBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnChartViewModel" /> class.
        /// </summary>
        /// <param name="labels">The labels.</param>
        public ColumnChartViewModel(string[] labels)
            : base(labels)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified label.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="values">The values.</param>
        public void Add(string title, IEnumerable<double> values)
        {
            this.SeriesCollection.Add(new ColumnSeries
            {
                Title = title,
                Values = new ChartValues<double>(values),
            });
        }

        #endregion
    }
}