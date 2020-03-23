//-----------------------------------------------------------------------
// <copyright file="DashboardViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using Caliburn.Micro;
    using Lurker.Patreon;
    using Lurker.Patreon.Events;
    using Lurker.Patreon.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DashboardViewModel : ScreenBase
    {
        #region Fields

        private PieChartViewModel _pieChart;
        private ColumnChartViewModel _columnChart;
        private PropertyChangedBase _activeChart;
        private IEnumerable<SimpleTradeModel> _trades;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardViewModel"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        public DashboardViewModel(IWindowManager windowManager) 
            : base(windowManager)
        {
            this.DisplayName = "Dashboard";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the active chart.
        /// </summary>
        public PropertyChangedBase ActiveChart
        {
            get
            {
                return this._activeChart;
            }

            private set
            {
                this._activeChart = value;
                this.NotifyOfPropertyChange();
            }
        }

        #endregion

        /// <summary>
        /// Backs this instance.
        /// </summary>
        public void Back()
        {
            if (ActiveChart is ColumnChartViewModel)
            {
                this._pieChart = this.GetPieChart();
                this._pieChart.OnPieClick += this.PieChartViewModel_OnPieClick;
                this.ActiveChart = this._pieChart;
            }
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override async void OnActivate()
        {
            using (var service = new DatabaseService())
            {
                await service.CheckPledgeStatus();
                this._trades = service.Get().Where(t => !t.IsOutgoing).ToArray();
            }

            this._pieChart = this.GetPieChart();
            this._pieChart.OnPieClick += this.PieChartViewModel_OnPieClick;
            this.ActiveChart = this._pieChart;
            base.OnActivate();
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
        }

        /// <summary>
        /// Gets the pie chart.
        /// </summary>
        /// <returns></returns>
        private PieChartViewModel GetPieChart()
        {
            var pieChart = new PieChartViewModel();

            var groups = this._trades.GroupBy(i => i.Price.CurrencyType).Select(g => new { Type = g.First().Price.CurrencyType, Sum = g.Sum(i => i.Price.NumberOfCurrencies) });
            foreach (var group in groups)
            {
                pieChart.Add(group.Type.ToString(), group.Sum);
            }

            return pieChart;
        }

        /// <summary>
        /// Pies the chart view model on pie click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void PieChartViewModel_OnPieClick(object sender, LiveCharts.ChartPoint e)
        {
            if (Enum.TryParse<CurrencyType>(e.SeriesView.Title, true, out var value))
            {
                var trades = this._trades.Where(t => t.Price.CurrencyType == value);
                var chart = new ColumnChartViewModel(trades.Select(t => t.ItemName).ToArray());
                chart.Add(e.SeriesView.Title, trades.Select(t => t.Price.NumberOfCurrencies));
                this._columnChart = chart;
                this.ActiveChart = this._columnChart;
                this._pieChart.OnPieClick -= this.PieChartViewModel_OnPieClick;
            }
        }
    }
}
