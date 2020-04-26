//-----------------------------------------------------------------------
// <copyright file="PieChartViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using LiveCharts;
    using LiveCharts.Wpf;

    /// <summary>
    /// Represents the PieChart.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.ChartViewModelBase" />
    public class PieChartViewModel : ChartViewModelBase
    {
        #region Methods

        /// <summary>
        /// Adds the specified label.
        /// </summary>
        /// <param name="label">The label.</param>
        /// <param name="value">The value.</param>
        public void Add(string label, double value)
        {
            var serie = new PieSeries()
            {
                Title = label,
                Values = new ChartValues<double>(new double[] { value }),
                DataLabels = true,
                FontSize = this.FontSize,
                LabelPosition = this.LabelPosition,
            };

            this.SeriesCollection.Add(serie);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the inner radius.
        /// </summary>
        public int InnerRadius { get; set; }

        /// <summary>
        /// Gets or sets the label position.
        /// </summary>
        public PieLabelPosition LabelPosition { get; set; }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        public double FontSize { get; set; }

        #endregion
    }
}