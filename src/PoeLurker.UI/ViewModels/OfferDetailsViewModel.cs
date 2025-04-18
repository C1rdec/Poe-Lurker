﻿//-----------------------------------------------------------------------
// <copyright file="OfferDetailsViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Threading.Tasks;
using PoeLurker.Patreon.Events;
using PoeLurker.Patreon.Services;

/// <summary>
/// Represents the offer details.
/// </summary>
public class OfferDetailsViewModel : Caliburn.Micro.PropertyChangedBase
{
    #region Fields

    private readonly TradeEvent _event;
    private double _divineRatio;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="OfferDetailsViewModel" /> class.
    /// </summary>
    /// <param name="tradeEvent">The event.</param>
    public OfferDetailsViewModel(TradeEvent tradeEvent)
    {
        _event = tradeEvent;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the DecimalChaosValue.
    /// </summary>
    public int DecimalChaosValue { get; private set; }

    /// <summary>
    /// Gets the Decimal.
    /// </summary>
    public double Decimal { get; private set; }

    /// <summary>
    /// Gets a value indicating whether has decimal.
    /// </summary>
    public bool HasDecimal => Decimal > 0;

    /// <summary>
    /// Gets the ExaltRatio.
    /// </summary>
    public double DivineRatio
    {
        get
        {
            return _divineRatio;
        }

        private set
        {
            _divineRatio = value;
            NotifyOfPropertyChange();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Initialize the viewmodel.
    /// </summary>
    /// <returns>
    /// The task.
    /// </returns>
    public async Task Initialize()
    {
        using (var service = new PoeNinjaService())
        {
            var line = await service.GetDivineRationAsync(_event.LeagueName);
            if (line == null)
            {
                return;
            }

            DivineRatio = Math.Round(line.ChaosEquivalent);
        }

        var value = _event.Price.NumberOfCurrencies % 1;
        Decimal = Math.Round(value, 2);
        DecimalChaosValue = Convert.ToInt32(Decimal * DivineRatio);
    }

    #endregion
}