//-----------------------------------------------------------------------
// <copyright file="MapViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Lurker.Models;
    using Lurker.Patreon.Models;
    using Lurker.Services;

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
        private bool _modsSelectionVisible;
        private PlayerService _playerService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MapViewModel" /> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="playerViewModel">The player view model.</param>
        /// <param name="playerService">The player service.</param>
        /// <param name="closeCallback">The close callback.</param>
        public MapViewModel(Map map, PlayerViewModel playerViewModel, PlayerService playerService, System.Action closeCallback)
        {
            this._map = map;
            this.CurrentPlayer = playerViewModel;
            this._closeCallBack = closeCallback;
            this._playerService = playerService;
            this._playerService.PlayerChanged += this.PlayerService_PlayerChanged;
            this.Affixes = new ObservableCollection<MapAffixViewModel>();
            this.ShowMapMods();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the affixes.
        /// </summary>
        public ObservableCollection<MapAffixViewModel> Affixes { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is safe.
        /// </summary>
        public bool Safe => !this.Affixes.Any();

        /// <summary>
        /// Gets a value indicating whether [not safe].
        /// </summary>
        public bool NotSafe => !this.Safe;

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

        /// <summary>
        /// Shows the mod selection.
        /// </summary>
        public void ToggleModSelection()
        {
            this.Affixes.Clear();

            if (!this._modsSelectionVisible)
            {
                this.Affixes.Add(new MapAffixViewModel(MapAffixViewModel.CannotRegenerateId, true, this.CurrentPlayer));
                this.Affixes.Add(new MapAffixViewModel(MapAffixViewModel.CannotLeechId, true, this.CurrentPlayer));
                this.Affixes.Add(new MapAffixViewModel(MapAffixViewModel.ReflectElementalId, true, this.CurrentPlayer));
                this.Affixes.Add(new MapAffixViewModel(MapAffixViewModel.ReflectPhysicalId, true, this.CurrentPlayer));
                this.Affixes.Add(new MapAffixViewModel(MapAffixViewModel.TemporalChainsId, true, this.CurrentPlayer));
                this.NotifyOfPropertyChange(() => this.Safe);
            }
            else
            {
                this.ShowMapMods();
            }

            this._modsSelectionVisible = !this._modsSelectionVisible;
        }

        /// <summary>
        /// Players the service player changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void PlayerService_PlayerChanged(object sender, Player e)
        {
            this._modsSelectionVisible = false;
            this.Affixes.Clear();
            this.ShowMapMods();
        }

        /// <summary>
        /// Shows the map mods.
        /// </summary>
        private void ShowMapMods()
        {
            foreach (var affix in this._map.DangerousAffixes.Where(d => d != null))
            {
                if (this.CurrentPlayer.IgnoredMadMods.Contains(affix.Id))
                {
                    continue;
                }

                this.Affixes.Add(new MapAffixViewModel(affix, this.CurrentPlayer));
            }

            this.NotifyOfPropertyChange(() => this.Safe);
            this.NotifyOfPropertyChange(() => this.NotSafe);
        }

        #endregion
    }
}