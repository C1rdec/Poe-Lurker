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
    using Lurker.UI.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DashboardViewModel : ScreenBase
    {
        #region Fields

        private PieChartViewModel _totalChart;
        private ColumnChartViewModel _tradesChart;
        private LineChartViewModel _networthChart;
        private ChartViewModelBase _activeChart;
        private PieChartViewModel _itemClassChart;
        private IEnumerable<SimpleTradeModel> _trades;
        private IEnumerable<SimpleTradeModel> _allTradres;

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
        /// Gets a value indicating whether this instance has active chart.
        /// </summary>
        public bool NoActiveChart => this._activeChart == null;

        /// <summary>
        /// Gets a value indicating whether this instance has active chart.
        /// </summary>
        public bool HasActiveChart => !this.NoActiveChart;

        /// <summary>
        /// Gets the active chart.
        /// </summary>
        public ChartViewModelBase ActiveChart
        {
            get
            {
                return this._activeChart;
            }

            private set
            {
                this._activeChart = value;
                this.NotifyOfPropertyChange();
                this.NotifyOfPropertyChange("NoActiveChart");
                this.NotifyOfPropertyChange("HasActiveChart");
            }
        }

        /// <summary>
        /// Gets the total chart.
        /// </summary>
        public PieChartViewModel TotalChart
        {
            get
            {
                return this._totalChart;
            }

            private set
            {
                this._totalChart = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the networth chart.
        /// </summary>
        public LineChartViewModel NetworthChart
        {
            get
            {
                return this._networthChart;
            }

            private set
            {
                this._networthChart = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the networth chart.
        /// </summary>
        public PieChartViewModel ItemClassChart
        {
            get
            {
                return this._itemClassChart;
            }

            private set
            {
                this._itemClassChart = value;
                this.NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Backs this instance.
        /// </summary>
        public void Back()
        {
            this.ActiveChart = null;
        }

        /// <summary>
        /// Days this instance.
        /// </summary>
        public void Day()
        {
            this._trades = this._allTradres.Where(t => DateTime.Compare(t.Date, DateTime.Now.AddDays(-1)) > 0);
            this.NetworthChart = this.GetNetworthChart();
        }

        /// <summary>
        /// Weeks this instance.
        /// </summary>
        public void Week()
        {
            this._trades = this._allTradres.Where(t => DateTime.Compare(t.Date, DateTime.Now.AddDays(-7)) > 0);
            this.NetworthChart = this.GetNetworthChart();
        }

        /// <summary>
        /// Alls this instance.
        /// </summary>
        public void All()
        {
            this._trades = this._allTradres;
            this.NetworthChart = this.GetNetworthChart();
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override async void OnActivate()
        {
            using (var service = new DatabaseService())
            {
                await service.CheckPledgeStatus();
                this._trades = service.Get().Where(t => !t.IsOutgoing).OrderBy(t => t.Date).ToArray();
                this._allTradres = this._trades;
            }

            this.ItemClassChart = this.CreateItemClassChart();
            this.ItemClassChart.OnSerieClick += this.ItemClassChart_OnSerieClick;
            this.NetworthChart = this.GetNetworthChart();
            this.TotalChart = this.CreateTotalChart();
            this.TotalChart.OnSerieClick += this.OnSerieClick;
            base.OnActivate();
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected override void OnDeactivate(bool close)
        {
            if (close)
            {
                this.ItemClassChart.OnSerieClick -= this.ItemClassChart_OnSerieClick;
                this.TotalChart.OnSerieClick -= this.OnSerieClick;
            }

            base.OnDeactivate(close);
        }

        /// <summary>
        /// Items the class chart on serie click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ItemClassChart_OnSerieClick(object sender, LiveCharts.ChartPoint e)
        {
            if (Enum.TryParse<ItemClass>(e.SeriesView.Title, true, out var value))
            {
                var trades = this._trades.Where(t => t.ItemClass == value);
                var chart = new ColumnChartViewModel(trades.Select(t => t.ItemName).ToArray());
                chart.Add(e.SeriesView.Title, trades.Select(t => t.Price.CalculateValue()));
                this._tradesChart = chart;
                this.ActiveChart = this._tradesChart;
            }
        }

        /// <summary>
        /// Gets the pie chart.
        /// </summary>
        /// <returns></returns>
        private PieChartViewModel CreateTotalChart()
        {
            var pieChart = new PieChartViewModel()
            {
                FontSize = 12,
                LabelPosition = LiveCharts.PieLabelPosition.InsideSlice
            };

            var groups = this._trades.GroupBy(i => i.Price.CurrencyType).Select(g => new { Type = g.First().Price.CurrencyType, Sum = g.Sum(i => i.Price.CalculateValue()) });
            foreach (var group in groups)
            {
                pieChart.Add(group.Type.ToString(), group.Sum);
            }

            return pieChart;
        }

        /// <summary>
        /// Gets the networth chart.
        /// </summary>
        /// <returns>The networth graph.</returns>
        private LineChartViewModel GetNetworthChart()
        {
            var lineChart = new LineChartViewModel();
            var points = new List<double>();
            double previousValue = 0;

            foreach (var trade in this._trades)
            {
                var value = trade.Price.CalculateValue();
                if (trade.IsOutgoing)
                {
                    previousValue -= value;
                }
                else
                {
                    previousValue += value;
                }

                points.Add(previousValue);
            }

            lineChart.Add("Networth", points);
            return lineChart;
        }

        /// <summary>
        /// Creates the item class chart.
        /// </summary>
        /// <returns></returns>
        private PieChartViewModel CreateItemClassChart()
        {
            var pieChart = new PieChartViewModel()
            {
                InnerRadius = 85,
                FontSize = 18,
            };

            var groups = this._trades.GroupBy(i => i.ItemClass).Select(g => new { Type = g.First().ItemClass, Sum = g.Sum(i => i.Price.CalculateValue()) });
            foreach (var group in groups.OrderByDescending(g => g.Sum).Take(5))
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
        private void OnSerieClick(object sender, LiveCharts.ChartPoint e)
        {
            if (Enum.TryParse<CurrencyType>(e.SeriesView.Title, true, out var value))
            {
                var trades = this._trades.Where(t => t.Price.CurrencyType == value);
                var chart = new ColumnChartViewModel(trades.Select(t => t.ItemName).ToArray()); ;
                chart.Add(e.SeriesView.Title, trades.Select(t => t.Price.NumberOfCurrencies));
                this._tradesChart = chart;
                this.ActiveChart = this._tradesChart;
            }
        }

        #endregion
    }
}
