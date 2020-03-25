//-----------------------------------------------------------------------
// <copyright file="LineChartViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using LiveCharts;
    using LiveCharts.Wpf;
    using System.Collections.Generic;

    public class LineChartViewModel : ChartViewModelBase
    {

        #region Methods

        /// <summary>
        /// Adds the specified label.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="value">The value.</param>
        public void Add(string title, IEnumerable<double> values)
        {
            this.SeriesCollection.Add(new LineSeries
            {
                Title = title,
                Values = new ChartValues<double>(values)
            });
        }

        #endregion
    }
}
