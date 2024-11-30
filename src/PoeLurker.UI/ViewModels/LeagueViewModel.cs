//-----------------------------------------------------------------------
// <copyright file="LeagueViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using PoeLurker.Patreon.Services;
using PoeLurker.UI.Models;

/// <summary>
/// Represents a league.
/// </summary>
public class LeagueViewModel : PropertyChangedBase, IDisposable, IHandle<TradeCompletedEvent>
{
    #region Fields

    private readonly IEventAggregator _eventAggregator;
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
        _eventAggregator = eventAggregator;
        _eventAggregator.SubscribeOnPublishedThread(this);
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
            return _animate;
        }

        set
        {
            _animate = value;
            NotifyOfPropertyChange();
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
        _eventAggregator.Unsubscribe(this);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle trade complemented event.
    /// </summary>
    /// <param name="message">The message.</param>
    public async Task HandleAsync(TradeCompletedEvent message, CancellationToken cancellationToken)
    {
        if (_currentTask != null)
        {
            await _currentTask;
            _currentTask = null;
        }

        _currentTask = SetValue();
    }

    private async Task SetValue()
    {
        using (var service = new DatabaseService())
        {
            var league = service.GetLatestLeague();
            Value = league.Trades.Where(t => !t.IsOutgoing).Select(t => t.Price.CalculateValue()).Sum();
        }

        NotifyOfPropertyChange(() => Value);
        Animate = true;
        await Task.Delay(4000);
        Animate = false;
    }

    #endregion
}