//-----------------------------------------------------------------------
// <copyright file="PlayerService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using ConfOxide;
    using Lurker.Models;
    using PoeLurker.Patreon.Events;

    /// <summary>
    /// Represent the player Service.
    /// </summary>
    public class PlayerService : ServiceBase, IDisposable
    {
        #region Fields

        private ClientLurker _clientLurker;
        private PlayerBank _playerBank;

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
                this._clientLurker = clientLurker;
                this._clientLurker.PlayerLevelUp += this.Lurker_PlayerLevelUp;
                this._clientLurker.PlayerJoined += this.AddExternalPlayer;
                this._clientLurker.PlayerLeft += this.AddExternalPlayer;
            }

            this._playerBank = new PlayerBank();

            if (!File.Exists(this.FilePath))
            {
                using (var file = File.Create(this.FilePath))
                {
                }

                this.Save();
            }
            else
            {
                try
                {
                    this._playerBank.ReadJsonFile(this.FilePath);
                    this.CleanExternalPlayers();
                }
                catch
                {
                    File.Delete(this.FilePath);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the first player.
        /// </summary>
        public Player FirstPlayer => this._playerBank.Players.FirstOrDefault();

        /// <summary>
        /// Gets the players.
        /// </summary>
        public IEnumerable<Player> Players => this._playerBank.Players;

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        protected override string FileName => "Players.json";

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
            this.MoveFirst(player);
            this.Save();

            this.PlayerChanged?.Invoke(this, player);
        }

        /// <summary>
        /// Removes the specified player.
        /// </summary>
        /// <param name="player">The player.</param>
        public void Remove(Player player)
        {
            var knownPlayer = this._playerBank.GetKnownPlayer(player.Name);
            if (knownPlayer != null)
            {
                var index = this._playerBank.Players.IndexOf(knownPlayer);
                this._playerBank.Players.Remove(knownPlayer);
                this.Save();

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
            var knownPlayer = this._playerBank.GetKnownPlayer(player.Name);
            if (knownPlayer == null)
            {
                this._playerBank.Players.Add(player);
                this.Save();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Saves the specified raise event.
        /// </summary>
        public void Save()
        {
            this._playerBank.WriteJsonFile(this.FilePath);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this._clientLurker.PlayerLevelUp -= this.Lurker_PlayerLevelUp;
                this._clientLurker.PlayerJoined -= this.AddExternalPlayer;
                this._clientLurker.PlayerLeft -= this.AddExternalPlayer;
            }
        }

        /// <summary>
        /// Cleans the external players.
        /// </summary>
        private void CleanExternalPlayers()
        {
            var playersToRemove = this._playerBank.ExternalPlayers.Where(p => p.Levels.FirstOrDefault() == 0).ToArray();
            foreach (var player in playersToRemove)
            {
                this._playerBank.ExternalPlayers.Remove(player);
            }

            this.Save();
        }

        /// <summary>
        /// Adds the external player.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AddExternalPlayer(object sender, PlayerEvent e)
        {
            var player = this._playerBank.GetExternalPlayer(e.PlayerName);
            if (player == null)
            {
                this._playerBank.ExternalPlayers.Add(new Player() { Name = e.PlayerName, Levels = new List<int> { 0 } });
                this.Save();
            }

            var knownPlayer = this._playerBank.GetKnownPlayer(e.PlayerName);
            if (knownPlayer != null)
            {
                this.RemovePlayer(knownPlayer);
                if (this.FirstPlayer != null)
                {
                    this.PlayerChanged?.Invoke(this, this.FirstPlayer);
                }

                this.Save();
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
                var knownPlayer = this._playerBank.GetKnownPlayer(e.PlayerName);
                if (knownPlayer != null)
                {
                    knownPlayer.AddLevel(e.Level);
                    this._playerBank.Players.Remove(knownPlayer);
                    this._playerBank.Players.Insert(0, knownPlayer);
                    this.PlayerChanged?.Invoke(this, knownPlayer);
                    return;
                }

                var externalPlayer = this._playerBank.GetExternalPlayer(e.PlayerName);
                if (externalPlayer != null)
                {
                    externalPlayer.AddLevel(e.Level);
                    return;
                }

                // Wait for location changed event to confirm the new player
                var newPlayer = new Player() { Name = e.PlayerName, Levels = new List<int>() { e.Level } };
                this.InsertPlayer(newPlayer);
                this.PlayerChanged?.Invoke(this, newPlayer);
            }
            finally
            {
                this.Save();
            }
        }

        /// <summary>
        /// Removes the player.
        /// </summary>
        /// <param name="player">The player.</param>
        private void RemovePlayer(Player player)
        {
            this._playerBank.Players.Remove(player);
            this.PlayerListChanged?.Invoke(this, this._playerBank.Players);
        }

        /// <summary>
        /// Inserts the player.
        /// </summary>
        /// <param name="player">The player.</param>
        private void InsertPlayer(Player player)
        {
            this._playerBank.Players.Insert(0, player);
            this.PlayerListChanged?.Invoke(this, this._playerBank.Players);
        }

        private void MoveFirst(Player player)
        {
            var knownPlayer = this._playerBank.GetKnownPlayer(player.Name);
            if (knownPlayer != null)
            {
                this._playerBank.Players.Remove(knownPlayer);
                this._playerBank.Players.Insert(0, knownPlayer);
                return;
            }
        }

        #endregion
    }
}