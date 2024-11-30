//-----------------------------------------------------------------------
// <copyright file="PositionViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System.Windows.Media;
using PoeLurker.Core.Services;
using PoeLurker.Patreon.Events;

/// <summary>
/// The position view model.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class PositionViewModel : Caliburn.Micro.PropertyChangedBase
{
    #region Fields

    private readonly TradeEvent _tradeEvent;
    private readonly SettingsService _settingService;
    private readonly StashTabService _stashTabService;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PositionViewModel" /> class.
    /// </summary>
    /// <param name="tradeEvent">The trade event.</param>
    /// <param name="settingService">The setting service.</param>
    /// <param name="stashTabService">The stash tab service.</param>
    public PositionViewModel(TradeEvent tradeEvent, SettingsService settingService, StashTabService stashTabService)
    {
        _tradeEvent = tradeEvent;
        _settingService = settingService;
        _stashTabService = stashTabService;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the name of the stash.
    /// </summary>
    public string StashName => _tradeEvent.Location.StashTabName;

    /// <summary>
    /// Gets the name of the item.
    /// </summary>
    public string Location
    {
        get
        {
            if (_tradeEvent.Location.Left <= 0 || _tradeEvent.Location.Top <= 0)
            {
                return string.Empty;
            }

            return $"Left: {_tradeEvent.Location.Left}, Top: {_tradeEvent.Location.Top}";
        }
    }

    /// <summary>
    /// Gets the foreground.
    /// </summary>
    public SolidColorBrush Foreground => new SolidColorBrush((Color)ColorConverter.ConvertFromString(_settingService.LifeForeground));

    #endregion

    #region Methods

    /// <summary>
    /// Locate the item.
    /// </summary>
    public void Locate()
    {
        _stashTabService.PlaceMarker(_tradeEvent.Location);
    }

    #endregion
}