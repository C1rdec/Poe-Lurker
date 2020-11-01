//-----------------------------------------------------------------------
// <copyright file="MapViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Lurker.Patreon.Models;

    /// <summary>
    /// Represents a map item.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class MapViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private const string ReflectPhysicalId = "stat_3464419871";
        private const string ReflectElementalId = "stat_2764017512";
        private const string CannotRegenerateId = "stat_1910157106";
        private const string CannotLeechId = "stat_526251910";
        private const string TemporalChainsId = "stat_2326202293";
        private System.Action _closeCallBack;
        private Map _map;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MapViewModel" /> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="playerViewModel">The player view model.</param>
        /// <param name="closeCallback">The close callback.</param>
        public MapViewModel(Map map, PlayerViewModel playerViewModel, System.Action closeCallback)
        {
            this._map = map;
            this.CurrentPlayer = playerViewModel;
            this._closeCallBack = closeCallback;

            this.Affixes = new ObservableCollection<MapAffixViewModel>();
            foreach (var affix in this._map.DangerousAffixes.Where(d => d != null))
            {
                this.Affixes.Add(new MapAffixViewModel(affix));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the affixes.
        /// </summary>
        public ObservableCollection<MapAffixViewModel> Affixes { get; set; }

        /// <summary>
        /// Gets the current player.
        /// </summary>
        public PlayerViewModel CurrentPlayer { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            this._closeCallBack?.Invoke();
        }

        #endregion
    }
}