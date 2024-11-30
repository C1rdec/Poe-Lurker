//-----------------------------------------------------------------------
// <copyright file="PlayerViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;

/// <summary>
/// Represent a player.
/// </summary>
public class PlayerViewModel : Caliburn.Micro.PropertyChangedBase
{
    #region Fields

    private readonly PlayerService _service;
    private Player _activePlayer;
    private Player _selectedPlayer;
    private bool _selectionVisible;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerViewModel"/> class.
    /// </summary>
    /// <param name="service">The service.</param>
    public PlayerViewModel(PlayerService service)
    {
        _service = service;
        _activePlayer = service.FirstPlayer;
        Players = new ObservableCollection<Player>();

        foreach (var player in service.Players)
        {
            Players.Add(player);
        }

        _service.PlayerChanged += Service_PlayerChanged;
        _service.PlayerListChanged += Service_PlayerListChanged;

        PropertyChanged += PlayerViewModel_PropertyChanged;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the ignored mad mods.
    /// </summary>
    public IEnumerable<string> IgnoredMadMods => _activePlayer == null ? Enumerable.Empty<string>() : _activePlayer.IgnoredMapMods;

    /// <summary>
    /// Gets the display name.
    /// </summary>
    public string DisplayName => _activePlayer == null ? string.Empty : $"{Name} ({Level})";

    /// <summary>
    /// Gets or sets the players.
    /// </summary>
    public ObservableCollection<Player> Players { get; set; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name => _activePlayer?.Name;

    /// <summary>
    /// Gets the level.
    /// </summary>
    public int Level => _activePlayer == null ? 0 : _activePlayer.GetCurrentLevel();

    /// <summary>
    /// Gets or sets the selected player.
    /// </summary>
    public Player SelectedPlayer
    {
        get
        {
            return _selectedPlayer;
        }

        set
        {
            if (_selectedPlayer != value)
            {
                _selectedPlayer = value;
                NotifyOfPropertyChange();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [selection visible].
    /// </summary>
    public bool SelectionVisible
    {
        get
        {
            return _selectionVisible;
        }

        set
        {
            if (_selectionVisible != value)
            {
                _selectionVisible = value;
                NotifyOfPropertyChange();
            }
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Toggles the selection.
    /// </summary>
    public void ToggleSelection()
    {
        SelectionVisible = !SelectionVisible;
    }

    /// <summary>
    /// Adds the ignored map mod.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public void AddIgnoredMapMod(string id)
    {
        if (_activePlayer != null && !_activePlayer.IgnoredMapMods.Contains(id))
        {
            _activePlayer.IgnoredMapMods.Add(id);
            _service.Save();
        }
    }

    /// <summary>
    /// Removes the ignored map mod.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public void RemoveIgnoredMapMod(string id)
    {
        if (_activePlayer == null)
        {
            return;
        }

        _activePlayer.IgnoredMapMods.Remove(id);
        _service.Save();
    }

    /// <summary>
    /// Handles the PropertyChanged event of the PlayerViewModel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void PlayerViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SelectedPlayer) && SelectedPlayer != null)
        {
            ToggleSelection();
            _service.ChangePlayer(SelectedPlayer);
        }
    }

    /// <summary>
    /// Services the player changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void Service_PlayerChanged(object sender, Player e)
    {
        _activePlayer = e;
        NotifyOfPropertyChange(() => DisplayName);
    }

    /// <summary>
    /// Services the player list changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void Service_PlayerListChanged(object sender, System.Collections.Generic.IEnumerable<Player> e)
    {
        Caliburn.Micro.Execute.OnUIThread(() =>
        {
            Players.Clear();

            foreach (var player in e)
            {
                Players.Add(player);
            }
        });
    }

    #endregion
}