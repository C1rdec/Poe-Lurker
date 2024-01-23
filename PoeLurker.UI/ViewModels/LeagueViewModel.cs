//-----------------------------------------------------------------------
// <copyright file="LeagueViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using PoeLurker.UI.Models;
    using PoeLurker.Patreon.Services;
    using System.Threading;

    /// <summary>
    /// Represents a league.
    /// </summary>
    public class LeagueViewModel : Caliburn.Micro.PropertyChangedBase, IDisposable, IHandle<TradeCompletedEvent>
    {
        #region Fields

        private IEventAggregator _eventAggregator;
        private bool _animate;
        private Task _currentTask;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LeagueViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public LeagueViewModel(IEventAggregator eventAggregator)
        {
            this._eventAggregator = eventAggregator;
            this._eventAggregator.Subscribe(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether animate.
        /// </summary>
        public bool Animate
        {
            get
            {
                return this._animate;
            }

            set
            {
                this._animate = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the trade value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            this._eventAggregator.Unsubscribe(this);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle trade complemented event.
        /// </summary>
        /// <param name="message">The message.</param>
        public async Task HandleAsync(TradeCompletedEvent message, CancellationToken cancellationToken)
        {
            if (this._currentTask != null)
            {
                await this._currentTask;
                this._currentTask = null;
            }

            this._currentTask = this.SetValue();
        }

        private async Task SetValue()
        {
            using (var service = new DatabaseService())
            {
                var league = service.GetLatestLeague();
                this.Value = league.Trades.Where(t => !t.IsOutgoing).Select(t => t.Price.CalculateValue()).Sum();
            }

            this.NotifyOfPropertyChange(() => this.Value);
            this.Animate = true;
            await Task.Delay(4000);
            this.Animate = false;
        }

        #endregion
    }
}