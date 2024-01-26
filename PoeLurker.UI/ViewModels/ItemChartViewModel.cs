//-----------------------------------------------------------------------
// <copyright file="ItemChartViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Linq;
using Caliburn.Micro;
using PoeLurker.Core.Models;
using PoeLurker.Patreon.Models.Ninja;

/// <summary>
/// Represents ItemChartViewModel.
/// </summary>
public class ItemChartViewModel : PropertyChangedBase
{
    #region Fields

    private readonly ItemLine _line;
    private readonly UniqueItem _item;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemChartViewModel"/> class.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="item">The item.</param>
    public ItemChartViewModel(ItemLine line, UniqueItem item)
    {
        _line = line;
        _item = item;

        Price = PriceInDivine ? line.DivineValue : line.ChaosValue;
        //this.Chart = new LineChartViewModel(false, 0);

        var buyPoints = _line.PriceLine.Data.Where(d => d.HasValue);
        if (buyPoints.Any())
        {
            var minimum = Math.Abs(buyPoints.Min().Value);
            //this.Chart.Add("Buy", buyPoints.Select(p => Convert.ToDouble(p + minimum + 1)));
        }

        TotalChange = $"{_line.PriceLine.TotalChange}%";
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the price.
    /// </summary>
    public double Price { get; private set; }

    /// <summary>
    /// Gets the total change.
    /// </summary>
    public string TotalChange { get; private set; }

    /// <summary>
    /// Gets the image.
    /// </summary>
    public Uri ImageUrl => _item.ImageUrl;

    /// <summary>
    /// Gets a value indicating whether the total change is negative.
    /// </summary>
    public bool Negative => _line.PriceLine.TotalChange < 0;

    /// <summary>
    /// Gets a value indicating whether the total change is negative.
    /// </summary>
    public bool PriceInDivine => _line.DivineValue >= 1;

    /// <summary>
    /// Gets a value indicating whether the total change is negative.
    /// </summary>
    public bool PriceInChaos => _line.DivineValue < 1;

    #endregion
}