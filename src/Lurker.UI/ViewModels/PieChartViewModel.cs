//-----------------------------------------------------------------------
// <copyright file="PieChartViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;
    using LiveCharts;
    using LiveCharts.Wpf;
    using Lurker.UI.Models;
    using System;

    public class PieChartViewModel: PropertyChangedBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PieChartViewModel"/> class.
        /// </summary>
        public PieChartViewModel()
        {
            this.SeriesCollection = new SeriesCollection();
            this.LegendLocation = LegendLocation.Bottom;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Creates new offer.
        /// </summary>
        public event EventHandler<ChartPoint> OnPieClick;

        /// <summary>
        /// Gets or sets the legend location.
        /// </summary>
        public LegendLocation LegendLocation { get; set; }

        /// <summary>
        /// Gets or sets the series collection.
        /// </summary>
        public SeriesCollection SeriesCollection { get; set; }

        public MyCommand<ChartPoint> DataClickCommand => new MyCommand<ChartPoint>()
        {
            ExecuteDelegate = p => this.OnClick(p)
        };

        #endregion

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
                FontSize = 20,
                LabelPosition = PieLabelPosition.OutsideSlice,
            };

            this.SeriesCollection.Add(serie);
        }

        /// <summary>
        /// Called when [click].
        /// </summary>
        /// <param name="point">The point.</param>
        private void OnClick(ChartPoint point)
        {
            this.OnPieClick?.Invoke(this, point);
        }

        #endregion
    }
}
