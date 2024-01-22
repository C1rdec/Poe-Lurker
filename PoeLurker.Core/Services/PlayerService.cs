//-----------------------------------------------------------------------
// <copyright file="PlayerService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using Lurker.Core.Models;
using PoeLurker.Patreon.Events;

/// <summary>
/// Represent the player Service.
/// </summary>
public class PlayerService: IDisposable
{
    #region Fields

    private readonly ClientLurker _clientLurker;
    private readonly PlayerBank _playerBank;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerService"/> class.
    /// </summary>
    public PlayerService()
        : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerService"/> class.
    /// </summary>
    /// <param name="clientLurker">The lurker.</param>
    public PlayerService(ClientLurker clientLurker)
    {
        if (clientLurker != null)
        {
            _clientLurker = clientLurker;
            _clientLurker.PlayerLevelUp += Lurker_PlayerLevelUp;
            _clientLurker.PlayerJoined += AddExternalPlayer;
            _clientLurker.PlayerLeft += AddExternalPlayer;
        }

        _playerBank = new PlayerBank();
        _playerBank.Initialize();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the first player.
    /// </summary>
    public Player FirstPlayer => _playerBank.Players.FirstOrDefault();

    /// <summary>
    /// Gets the players.
    /// </summary>
    public IEnumerable<Player> Players => _playerBank.Players;

    #endregion

    #region Events

    /// <summary>
    /// Gets or sets the player updated.
    /// </summary>
    public event EventHandler<Player> PlayerChanged;

    /// <summary>
    /// Gets or sets the player list changed.
    /// </summary>
    public event EventHandler<IEnumerable<Player>> PlayerListChanged;

    #endregion

    #region Methods

    /// <summary>
    /// Changes the player.
    /// </summary>
    /// <param name="player">The player.</param>
    public void ChangePlayer(Player player)
    {
        MoveFirst(player);
        Save();

        PlayerChanged?.Invoke(this, player);
    }

    /// <summary>
    /// Removes the specified player.
    /// </summary>
    /// <param name="player">The player.</param>
    public void Remove(Player player)
    {
        var knownPlayer = _playerBank.GetKnownPlayer(player.Name);
        if (knownPlayer != null)
        {
            var index = _playerBank.Players.IndexOf(knownPlayer);
            _playerBank.Players.Remove(knownPlayer);
            Save();

            if (index == 0)
            {
                // invoke change
            }
        }
    }

    /// <summary>
    /// Adds the specified player.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>if succeed.</returns>
    public bool Add(Player player)
    {
        var knownPlayer = _playerBank.GetKnownPlayer(player.Name);
        if (knownPlayer == null)
        {
            _playerBank.Players.Add(player);
            Save();

            return true;
        }

        return false;
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Saves the specified raise event.
    /// </summary>
    public void Save()
    {
        _playerBank.Save();
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool isDisposing)
    {
        if (isDisposing)
        {
            _clientLurker.PlayerLevelUp -= Lurker_PlayerLevelUp;
            _clientLurker.PlayerJoined -= AddExternalPlayer;
            _clientLurker.PlayerLeft -= AddExternalPlayer;
        }
    }

    /// <summary>
    /// Cleans the external players.
    /// </summary>
    private void CleanExternalPlayers()
    {
        var playersToRemove = _playerBank.ExternalPlayers.Where(p => p.Levels.FirstOrDefault() == 0).ToArray();
        foreach (var player in playersToRemove)
        {
            _playerBank.ExternalPlayers.Remove(player);
        }

        Save();
    }

    /// <summary>
    /// Adds the external player.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void AddExternalPlayer(object sender, PlayerEvent e)
    {
        var player = _playerBank.GetExternalPlayer(e.PlayerName);
        if (player == null)
        {
            _playerBank.ExternalPlayers.Add(new Player() { Name = e.PlayerName, Levels = new List<int> { 0 } });
            Save();
        }

        var knownPlayer = _playerBank.GetKnownPlayer(e.PlayerName);
        if (knownPlayer != null)
        {
            RemovePlayer(knownPlayer);
            if (FirstPlayer != null)
            {
                PlayerChanged?.Invoke(this, FirstPlayer);
            }

            Save();
        }
    }

    /// <summary>
    /// Lurkers the player level up.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event.</param>
    private void Lurker_PlayerLevelUp(object sender, PlayerLevelUpEvent e)
    {
        if (PoeApplicationContext.Location.Contains("The Rogue Harbour"))
        {
            return;
        }

        try
        {
            var knownPlayer = _playerBank.GetKnownPlayer(e.PlayerName);
            if (knownPlayer != null)
            {
                knownPlayer.AddLevel(e.Level);
                _playerBank.Players.Remove(knownPlayer);
                _playerBank.Players.Insert(0, knownPlayer);
                PlayerChanged?.Invoke(this, knownPlayer);
                return;
            }

            var externalPlayer = _playerBank.GetExternalPlayer(e.PlayerName);
            if (externalPlayer != null)
            {
                externalPlayer.AddLevel(e.Level);
                return;
            }

            // Wait for location changed event to confirm the new player
            var newPlayer = new Player() { Name = e.PlayerName, Levels = new List<int>() { e.Level } };
            InsertPlayer(newPlayer);
            PlayerChanged?.Invoke(this, newPlayer);
        }
        finally
        {
            Save();
        }
    }

    /// <summary>
    /// Removes the player.
    /// </summary>
    /// <param name="player">The player.</param>
    private void RemovePlayer(Player player)
    {
        _playerBank.Players.Remove(player);
        PlayerListChanged?.Invoke(this, _playerBank.Players);
    }

    /// <summary>
    /// Inserts the player.
    /// </summary>
    /// <param name="player">The player.</param>
    private void InsertPlayer(Player player)
    {
        _playerBank.Players.Insert(0, player);
        PlayerListChanged?.Invoke(this, _playerBank.Players);
    }

    private void MoveFirst(Player player)
    {
        var knownPlayer = _playerBank.GetKnownPlayer(player.Name);
        if (knownPlayer != null)
        {
            _playerBank.Players.Remove(knownPlayer);
            _playerBank.Players.Insert(0, knownPlayer);

            return;
        }
    }

    #endregion
}