//-----------------------------------------------------------------------
// <copyright file="PositionViewModel.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------


namespace Lurker.UI.ViewModels
{
    using Lurker.Patreon.Events;
    using Lurker.Patreon.Models;
    using Lurker.Patreon.Parsers;

    public class PositionViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private static readonly ItemClassParser ItemClassParser = new ItemClassParser();
        private TradeEvent _tradeEvent;
        private ItemClass _ItemClass;

        #endregion

        #region Constructors

        public PositionViewModel(TradeEvent tradeEvent)
        {
            this._tradeEvent = tradeEvent;
            this._ItemClass = ItemClassParser.Parse(tradeEvent.ItemName);
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
        /// Gets or sets the item class.
        /// </summary>
        public ItemClass ItemClass => this._ItemClass;

        #endregion
    }
}
