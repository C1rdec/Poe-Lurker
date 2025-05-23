﻿//-----------------------------------------------------------------------
// <copyright file="TradeValueViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System.Windows.Media;
using PoeLurker.Core.Services;
using PoeLurker.Patreon.Events;
using PoeLurker.Patreon.Models;

/// <summary>
/// Represetns the trade value.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class TradeValueViewModel : Caliburn.Micro.PropertyChangedBase
{
    #region Fields

    private readonly TradeEvent _tradeEvent;
    private readonly SettingsService _settingsService;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="TradeValueViewModel" /> class.
    /// </summary>
    /// <param name="tradeEvent">The trade event.</param>
    /// <param name="settingsService">The settings service.</param>
    public TradeValueViewModel(TradeEvent tradeEvent, SettingsService settingsService)
    {
        _tradeEvent = tradeEvent;
        _settingsService = settingsService;
    }

    #endregion

    #region Properties

    public CurrencyViewModel Currency => new(CurrencyType);

    /// <summary>
    /// Gets the type of the currency.
    /// </summary>
    public CurrencyType CurrencyType => _tradeEvent.Price.CurrencyType;

    /// <summary>
    /// Gets the number off currency.
    /// </summary>
    public double NumberOffCurrency => _tradeEvent.Price.NumberOfCurrencies;

    /// <summary>
    /// Gets the foreground.
    /// </summary>
    public SolidColorBrush Foreground => new SolidColorBrush((Color)ColorConverter.ConvertFromString(_settingsService.LifeForeground));

    #endregion
}