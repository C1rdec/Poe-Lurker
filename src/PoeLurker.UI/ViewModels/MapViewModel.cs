//-----------------------------------------------------------------------
// <copyright file="MapViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;
using PoeLurker.Patreon.Models;

/// <summary>
/// Represents a map item.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class MapViewModel : Caliburn.Micro.PropertyChangedBase, IDisposable
{
    #region Fields

    private readonly Map _map;
    private bool _modsSelectionVisible;
    private readonly PlayerService _playerService;
    private int _ignoredModCount;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MapViewModel" /> class.
    /// </summary>
    /// <param name="map">The map.</param>
    /// <param name="playerViewModel">The player view model.</param>
    /// <param name="playerService">The player service.</param>
    public MapViewModel(Map map, PlayerViewModel playerViewModel, PlayerService playerService)
    {
        _map = map;
        CurrentPlayer = playerViewModel;
        _playerService = playerService;
        _playerService.PlayerChanged += PlayerService_PlayerChanged;
        Affixes = new ObservableCollection<MapAffixViewModel>();
        ShowMapMods();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the mod count.
    /// </summary>
    public int IgnoredModCount
    {
        get
        {
            return _ignoredModCount;
        }

        private set
        {
            _ignoredModCount = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets a value indicating whether [any ignored mod].
    /// </summary>
    public bool AnyIgnoredMod => IgnoredModCount != 0;

    /// <summary>
    /// Gets a value indicating whether [mods selection visible].
    /// </summary>
    public bool ModsSelectionVisible => _modsSelectionVisible;

    /// <summary>
    /// Gets or sets the affixes.
    /// </summary>
    public ObservableCollection<MapAffixViewModel> Affixes { get; set; }

    /// <summary>
    /// Gets a value indicating whether this instance is safe.
    /// </summary>
    public bool Safe => !Affixes.Any();

    /// <summary>
    /// Gets a value indicating whether [not safe].
    /// </summary>
    public bool NotSafe => !Safe;

    /// <summary>
    /// Gets the current player.
    /// </summary>
    public PlayerViewModel CurrentPlayer { get; private set; }

    #endregion

    #region Methods

    /// <summary>
    /// Shows the mod selection.
    /// </summary>
    public void ToggleModSelection()
    {
        Affixes.Clear();

        if (!_modsSelectionVisible)
        {
            AddAffix(MapAffixViewModel.CannotRegenerateId);
            AddAffix(MapAffixViewModel.CannotLeechId);
            AddAffix(MapAffixViewModel.ReflectElementalId);
            AddAffix(MapAffixViewModel.ReflectPhysicalId);
            AddAffix(MapAffixViewModel.TemporalChainsId);
            AddAffix(MapAffixViewModel.AvoidAilmentsId);
            NotifyOfPropertyChange(() => Safe);
        }
        else
        {
            ShowMapMods();
        }

        _modsSelectionVisible = !_modsSelectionVisible;
        NotifyOfPropertyChange(() => ModsSelectionVisible);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _playerService.PlayerChanged -= PlayerService_PlayerChanged;
        }
    }

    /// <summary>
    /// Adds the affix.
    /// </summary>
    /// <param name="id">The identifier.</param>
    private void AddAffix(string id)
    {
        var ignored = false;
        if (_map.Affixes.Any(a => a.Id == id))
        {
            ignored = CurrentPlayer.IgnoredMadMods.Contains(id);
        }

        Affixes.Add(new MapAffixViewModel(id, true, CurrentPlayer, ignored));
    }

    /// <summary>
    /// Players the service player changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void PlayerService_PlayerChanged(object sender, Player e)
    {
        _modsSelectionVisible = false;
        Execute.OnUIThread(() =>
        {
            Affixes.Clear();
            ShowMapMods();
        });
        NotifyOfPropertyChange(() => ModsSelectionVisible);
    }

    /// <summary>
    /// Shows the map mods.
    /// </summary>
    private void ShowMapMods()
    {
        IgnoredModCount = 0;
        foreach (var affix in _map.DangerousAffixes.Where(d => d != null))
        {
            if (CurrentPlayer.IgnoredMadMods.Contains(affix.Id))
            {
                IgnoredModCount++;
                continue;
            }

            Affixes.Add(new MapAffixViewModel(affix.Id, false, CurrentPlayer));
        }

        NotifyOfPropertyChange(() => Safe);
        NotifyOfPropertyChange(() => NotSafe);
        NotifyOfPropertyChange(() => AnyIgnoredMod);
    }

    #endregion
}