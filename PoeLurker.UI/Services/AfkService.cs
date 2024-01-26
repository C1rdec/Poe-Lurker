//-----------------------------------------------------------------------
// <copyright file="AfkService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using PoeLurker.Core;
using PoeLurker.Patreon.Events;

/// <summary>
/// Represents the afk service.
/// </summary>
/// <seealso cref="System.IDisposable" />
public class AfkService : IDisposable
{
    #region Fields

    private readonly ClientLurker _clientLurker;
    private bool _afkEnabled;
    private readonly List<TradeEvent> _trades;
    private readonly IEventAggregator _eventAggregator;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AfkService" /> class.
    /// </summary>
    /// <param name="clientLurker">The client lurker.</param>
    /// <param name="eventAggregator">The event aggregator.</param>
    public AfkService(ClientLurker clientLurker, IEventAggregator eventAggregator)
    {
        _trades = new List<TradeEvent>();
        _clientLurker = clientLurker;
        _eventAggregator = eventAggregator;

        _clientLurker.AfkChanged += AfkChanged;
        _clientLurker.IncomingOffer += IncomingOffer;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _clientLurker.AfkChanged -= AfkChanged;
            _clientLurker.IncomingOffer -= IncomingOffer;
            _clientLurker.Dispose();
        }
    }

    /// <summary>
    /// Afks the changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void AfkChanged(object sender, AfkEvent e)
    {
        _afkEnabled = e.AfkEnable;

        if (!_afkEnabled && _trades.Any())
        {
            _trades.Clear();
        }
    }

    /// <summary>
    /// Incomings the offer.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void IncomingOffer(object sender, TradeEvent e)
    {
        if (_afkEnabled)
        {
            _trades.Add(e);
        }
    }

    #endregion
}