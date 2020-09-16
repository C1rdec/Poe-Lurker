//-----------------------------------------------------------------------
// <copyright file="CharacterService.cs" company="Wohs Inc.">
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
    using Lurker.Patreon.Events;

    /// <summary>
    /// Represent the character Service.
    /// </summary>
    public class CharacterService : IDisposable
    {
        #region Fields

        private ClientLurker _clientLurker;
        private PlayerBank _playerBank;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterService"/> class.
        /// </summary>
        /// <param name="clientLurker">The lurker.</param>
        public CharacterService(ClientLurker clientLurker)
        {
            this._clientLurker = clientLurker;
            this._clientLurker.PlayerLevelUp += this.Lurker_PlayerLevelUp;
            this._clientLurker.PlayerJoined += this.AddExternalPlayer;
            this._clientLurker.PlayerLeft += this.AddExternalPlayer;

            this._playerBank = new PlayerBank();
            if (!Directory.Exists(this.SettingsFolderPath))
            {
                Directory.CreateDirectory(this.SettingsFolderPath);
            }

            if (!File.Exists(this.SettingsFilePath))
            {
                using (var file = File.Create(this.SettingsFilePath))
                {
                }

                this.Save();
            }
            else
            {
                try
                {
                    this._playerBank.ReadJsonFile(this.SettingsFilePath);
                }
                catch
                {
                    File.Delete(this.SettingsFilePath);
                }
            }
        }

        /// <summary>
        /// Adds the external player.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void AddExternalPlayer(object sender, Patreon.Events.PlayerEvent e)
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
                this._playerBank.Players.Remove(knownPlayer);
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
            try
            {
                var knownCharacter = this._playerBank.GetKnownPlayer(e.PlayerName);
                if (knownCharacter != null)
                {
                    knownCharacter.AddLevel(e.Level);
                    this._playerBank.Players.Remove(knownCharacter);
                    this._playerBank.Players.Insert(0, knownCharacter);
                    this.PlayerChanged?.Invoke(this, e);
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
                this._playerBank.Players.Add(newPlayer);
                this.PlayerChanged?.Invoke(this, e);
            }
            finally
            {
                this.Save();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the first player.
        /// </summary>
        public Player FirstPlayer => this._playerBank.Players.FirstOrDefault();

        /// <summary>
        /// Gets or sets the player updated.
        /// </summary>
        public EventHandler<PlayerLevelUpEvent> PlayerChanged { get; set; }

        /// <summary>
        /// Gets the name of the folder.
        /// </summary>
        private string FolderName => "PoeLurker";

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        private string FileName => "Players.json";

        /// <summary>
        /// Gets the application data folder path.
        /// </summary>
        private string AppDataFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// Gets the settings folder path.
        /// </summary>
        private string SettingsFolderPath => System.IO.Path.Combine(this.AppDataFolderPath, this.FolderName);

        /// <summary>
        /// Gets the settings file path.
        /// </summary>
        private string SettingsFilePath => System.IO.Path.Combine(this.SettingsFolderPath, this.FileName);

        #endregion

        #region Methods

        /// <summary>
        /// Saves the specified raise event.
        /// </summary>
        private void Save()
        {
            this._playerBank.WriteJsonFile(this.SettingsFilePath);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
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

        #endregion
    }
}