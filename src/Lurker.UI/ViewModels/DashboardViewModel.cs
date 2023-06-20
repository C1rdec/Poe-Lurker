//-----------------------------------------------------------------------
// <copyright file="DashboardViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
    using Lurker.UI.Models;
    using PoeLurker.Patreon.Events;
    using PoeLurker.Patreon.Services;
    using Patreon = PoeLurker.Patreon.Models;

    /// <summary>
    /// Represents the DashboardViewModel.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.ScreenBase" />
    public class DashboardViewModel : ScreenBase
    {
        #region Fields

        private PieChartViewModel _totalChart;
        private ColumnChartViewModel _tradesChart;
        private LineChartViewModel _networthChart;
        private ChartViewModelBase _activeChart;
        private PieChartViewModel _itemClassChart;
        private IEnumerable<SimpleTradeModel> _networthTrades;
        private IEnumerable<SimpleTradeModel> _leagueTrades;
        private IEnumerable<SimpleTradeModel> _allTradres;
        private IEnumerable<League> _leagues;
        private League _selectedLeague;
        private uint _totalNetworth;

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

            this._networthTrades = Enumerable.Empty<SimpleTradeModel>();
            this._leagueTrades = Enumerable.Empty<SimpleTradeModel>();
            this._allTradres = Enumerable.Empty<SimpleTradeModel>();
            this._leagues = Enumerable.Empty<League>();
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
        /// Gets the leagues.
        /// </summary>
        public IEnumerable<League> Leagues
        {
            get
            {
                return this._leagues;
            }

            private set
            {
                this._leagues = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the selected league.
        /// </summary>
        /// <value>The selected league.</value>
        public League SelectedLeague
        {
            get
            {
                return this._selectedLeague;
            }

            set
            {
                if (this._selectedLeague != value)
                {
                    this._selectedLeague = value;
                    this.Filter(this._selectedLeague);
                }
            }
        }

        /// <summary>
        /// Gets the total networth.
        /// </summary>
        /// <value>The selected league.</value>
        public uint TotalNetworth
        {
            get
            {
                return this._totalNetworth;
            }

            private set
            {
                this._totalNetworth = value;
                this.NotifyOfPropertyChange();
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
            this._networthTrades = this._leagueTrades.Where(t => DateTime.Compare(t.Date, DateTime.Now.AddDays(-1)) > 0);
            this.NetworthChart = this.CreateNetworthChart();
        }

        /// <summary>
        /// Weeks this instance.
        /// </summary>
        public void Week()
        {
            this._networthTrades = this._leagueTrades.Where(t => DateTime.Compare(t.Date, DateTime.Now.AddDays(-7)) > 0);
            this.NetworthChart = this.CreateNetworthChart();
        }

        /// <summary>
        /// Alls this instance.
        /// </summary>
        public void All()
        {
            this._networthTrades = this._leagueTrades;
            this.NetworthChart = this.CreateNetworthChart();
        }

        /// <summary>
        /// Filters the specified league.
        /// </summary>
        /// <param name="league">The league.</param>
        public void Filter(League league)
        {
            this.SetLeagueTrades(league);

            if (this.ItemClassChart != null)
            {
                this.ItemClassChart.OnSerieClick -= this.ItemClassChart_OnSerieClick;
            }

            if (this.TotalChart != null)
            {
                this.TotalChart.OnSerieClick -= this.OnSerieClick;
            }

            this.ItemClassChart = this.CreateItemClassChart();
            this.ItemClassChart.OnSerieClick += this.ItemClassChart_OnSerieClick;
            this.NetworthChart = this.CreateNetworthChart();
            this.TotalChart = this.CreateTotalChart();
            this.TotalChart.OnSerieClick += this.OnSerieClick;
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override async void OnActivate()
        {
            using (var service = new DatabaseService())
            {
                await service.CheckPledgeStatus();
                this._allTradres = service.Get().Where(t => !t.IsOutgoing).OrderBy(t => t.Date).ToArray();

                var leagues = this._allTradres.GroupBy(t => t.LeagueName).Select(grp => grp.Key);
                this.Leagues = this.FilterLeagueNames(leagues);

                if (this._allTradres.Any())
                {
                    var lastTrade = this._allTradres.Last();
                    var lastLeague = this.Leagues.FirstOrDefault(l => l.PossibleNames.Contains(lastTrade.LeagueName));
                    this.SetLeagueTrades(lastLeague);
                }
            }

            this.ItemClassChart = this.CreateItemClassChart();
            this.ItemClassChart.OnSerieClick += this.ItemClassChart_OnSerieClick;
            this.NetworthChart = this.CreateNetworthChart();
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
        /// Sets the league trades.
        /// </summary>
        /// <param name="league">The league.</param>
        private void SetLeagueTrades(League league)
        {
            this._selectedLeague = league;

            // Needs to be notified manually
            this.NotifyOfPropertyChange(nameof(this.SelectedLeague));

            if (league == null)
            {
                this._leagueTrades = this._allTradres;
            }
            else
            {
                this._leagueTrades = this._allTradres.Where(t => !string.IsNullOrEmpty(t.LeagueName) && league.PossibleNames.Contains(t.LeagueName));
            }

            this._networthTrades = this._leagueTrades;
            this.TotalNetworth = (uint)this._leagueTrades.Select(t => t.GetChaosValue()).Sum();
        }

        /// <summary>
        /// Filters the league names.
        /// </summary>
        /// <param name="leagues">The leagues.</param>
        /// <returns>List of Leagues.</returns>
        private List<League> FilterLeagueNames(IEnumerable<string> leagues)
        {
            var filteredLeagues = new List<League>();
            foreach (var leagueName in leagues)
            {
                if (string.IsNullOrEmpty(leagueName))
                {
                    continue;
                }

                var trimmedLeague = leagueName.Trim('.');
                var league = filteredLeagues.FirstOrDefault(f => f.DisplayName == trimmedLeague);
                if (league == null)
                {
                    league = new League() { DisplayName = trimmedLeague };
                    filteredLeagues.Add(league);
                }

                if (!league.PossibleNames.Contains(leagueName))
                {
                    league.AddName(leagueName);
                }
            }

            return filteredLeagues;
        }

        /// <summary>
        /// Items the class chart on serie click.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void ItemClassChart_OnSerieClick(object sender, LiveCharts.ChartPoint e)
        {
            if (Enum.TryParse<Patreon.ItemClass>(e.SeriesView.Title, true, out var value))
            {
                var trades = this._leagueTrades.Where(t => t.ItemClass == value);
                var chart = new ColumnChartViewModel(trades.Select(t => t.ItemName).ToArray());
                chart.Add(e.SeriesView.Title, trades.Select(t => t.Price.CalculateValue()));
                this._tradesChart = chart;
                this.ActiveChart = this._tradesChart;
            }
        }

        /// <summary>
        /// Gets the pie chart.
        /// </summary>
        /// <returns>The PieChart.</returns>
        private PieChartViewModel CreateTotalChart()
        {
            var pieChart = new PieChartViewModel()
            {
                FontSize = 12,
                LabelPosition = LiveCharts.PieLabelPosition.InsideSlice,
            };

            var groups = this._leagueTrades.GroupBy(i => i.Price.CurrencyType).Select(g => new { Type = g.First().Price.CurrencyType, Sum = g.Sum(i => i.GetChaosValue()) });
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
        private LineChartViewModel CreateNetworthChart()
        {
            var lineChart = new LineChartViewModel();
            var points = new List<double>();

            if (this._networthTrades.Count() < 2)
            {
                return lineChart;
            }

            double previousValue = 0;
            var firstTrade = this._networthTrades.First();
            var lastTrade = this._networthTrades.Last();

            var days = (lastTrade.Date - firstTrade.Date).TotalDays;
            var tradeGroups = this._networthTrades.GroupBy(t => t.Date.Date);

            var firstDate = firstTrade.Date.Date;
            for (int i = 0; i < days + 1; i++)
            {
                var group = tradeGroups.FirstOrDefault(g => g.Key.Date == firstDate);
                if (group == null)
                {
                    points.Add(previousValue);
                    firstDate = firstDate.AddDays(1);
                    continue;
                }

                var value = group.Select(g => g.GetChaosValue()).Sum();
                previousValue += value;

                if (previousValue < 0)
                {
                    points.Add(0);
                }
                else
                {
                    points.Add(previousValue);
                }

                firstDate = firstDate.AddDays(1);
            }

            lineChart.Add("Networth", points);
            return lineChart;
        }

        /// <summary>
        /// Creates the item class chart.
        /// </summary>
        /// <returns>The PieChart.</returns>
        private PieChartViewModel CreateItemClassChart()
        {
            var pieChart = new PieChartViewModel()
            {
                InnerRadius = 85,
                FontSize = 18,
            };

            var groups = this._leagueTrades.GroupBy(i => i.ItemClass).Select(g => new { Type = g.First().ItemClass, Sum = g.Sum(i => i.GetChaosValue()) });
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
            if (Enum.TryParse<Patreon.CurrencyType>(e.SeriesView.Title, true, out var value))
            {
                var trades = this._leagueTrades.Where(t => t.Price.CurrencyType == value);
                var chart = new ColumnChartViewModel(trades.Select(t => t.ItemName).ToArray());
                chart.Add(e.SeriesView.Title, trades.Select(t => t.Price.NumberOfCurrencies));
                this._tradesChart = chart;
                this.ActiveChart = this._tradesChart;
            }
        }

        #endregion
    }
}