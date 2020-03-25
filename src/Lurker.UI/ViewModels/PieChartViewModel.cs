//-----------------------------------------------------------------------
// <copyright file="PieChartViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using LiveCharts;
    using LiveCharts.Wpf;

    public class PieChartViewModel: ChartViewModelBase
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
