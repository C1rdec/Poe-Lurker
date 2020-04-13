//-----------------------------------------------------------------------
// <copyright file="PositionViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------


namespace Lurker.UI.ViewModels
{
    using Lurker.Patreon.Events;
    using Lurker.Services;
    using System.Windows.Media;

    public class PositionViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private TradeEvent _tradeEvent;
        private SettingsService _settingService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PositionViewModel"/> class.
        /// </summary>
        /// <param name="tradeEvent">The trade event.</param>
        public PositionViewModel(TradeEvent tradeEvent, SettingsService settingService)
        {
            this._tradeEvent = tradeEvent;
            this._settingService = settingService;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the stash.
        /// </summary>
        public string StashName => this._tradeEvent.Location.StashTabName;

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        public string Location => $"Left: {this._tradeEvent.Location.Left}, Top: {this._tradeEvent.Location.Top}";

        /// <summary>
        /// Gets the foreground.
        /// </summary>
        public SolidColorBrush Foreground => new SolidColorBrush((Color)ColorConverter.ConvertFromString(this._settingService.LifeForeground));

        #endregion
    }
}
