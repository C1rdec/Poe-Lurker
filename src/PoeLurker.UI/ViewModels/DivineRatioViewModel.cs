//-----------------------------------------------------------------------
// <copyright file="DivineRatioViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Linq;
using Caliburn.Micro;
using PoeLurker.Patreon.Models.Ninja;

/// <summary>
/// Represents the Divine Ratio.
/// </summary>
public class DivineRatioViewModel : PropertyChangedBase
{
    #region Fields

    private readonly CurrencyLine _line;
    private readonly double _ratio;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DivineRatioViewModel"/> class.
    /// </summary>
    /// <param name="line">The line.</param>
    public DivineRatioViewModel(CurrencyLine line)
    {
        _line = line;
        _ratio = line.ChaosEquivalent;
        //this.Chart = new LineChartViewModel(false, 0);

        var buyPoints = line.BuyLine.Data.Where(d => d.HasValue);
        if (buyPoints.Any())
        {
            var minimum = Math.Abs(buyPoints.Min().Value);
            //this.Chart.Add("Buy", buyPoints.Select(p => Convert.ToDouble(p + minimum + 1)));
        }

        TotalChange = $"{line.BuyLine.TotalChange}%";
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the ratio.
    /// </summary>
    public double Ratio
        => Fraction != null ? Math.Round(_ratio * Fraction.Value, 2) : _ratio;

    /// <summary>
    /// Gets the fraction.
    /// </summary>
    public double? Fraction { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the Fraction is visible.
    /// </summary>
    public bool HasFraction => Fraction != null;

    /// <summary>
    /// Gets the total change.
    /// </summary>
    public string TotalChange { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the total change is negative.
    /// </summary>
    public bool Negative => _line.BuyLine.TotalChange < 0;

    #endregion

    #region Methods

    /// <summary>
    /// Set the fraction.
    /// </summary>
    /// <param name="fraction">The fraction.</param>
    public void SetFraction(double? fraction)
    {
        if (fraction == 0)
        {
            fraction = null;
        }

        Fraction = fraction;

        NotifyOfPropertyChange(() => Fraction);
        NotifyOfPropertyChange(() => HasFraction);
        NotifyOfPropertyChange(() => Ratio);
    }

    #endregion
}