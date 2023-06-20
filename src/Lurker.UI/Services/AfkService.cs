//-----------------------------------------------------------------------
// <copyright file="AfkService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
    using PoeLurker.Patreon.Events;

    /// <summary>
    /// Represents the afk service.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class AfkService : IDisposable
    {
        #region Fields

        private ClientLurker _clientLurker;
        private bool _afkEnabled;
        private List<TradeEvent> _trades;
        private IEventAggregator _eventAggregator;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AfkService" /> class.
        /// </summary>
        /// <param name="clientLurker">The client lurker.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public AfkService(ClientLurker clientLurker, IEventAggregator eventAggregator)
        {
            this._trades = new List<TradeEvent>();
            this._clientLurker = clientLurker;
            this._eventAggregator = eventAggregator;

            this._clientLurker.AfkChanged += this.AfkChanged;
            this._clientLurker.IncomingOffer += this.IncomingOffer;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._clientLurker.AfkChanged -= this.AfkChanged;
                this._clientLurker.IncomingOffer -= this.IncomingOffer;
                this._clientLurker.Dispose();
            }
        }

        /// <summary>
        /// Afks the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AfkChanged(object sender, AfkEvent e)
        {
            this._afkEnabled = e.AfkEnable;

            if (!this._afkEnabled && this._trades.Any())
            {
                this._trades.Clear();
            }
        }

        /// <summary>
        /// Incomings the offer.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void IncomingOffer(object sender, TradeEvent e)
        {
            if (this._afkEnabled)
            {
                this._trades.Add(e);
            }
        }

        #endregion
    }
}