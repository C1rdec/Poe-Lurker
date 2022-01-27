//-----------------------------------------------------------------------
// <copyright file="CharacterManagerViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Lurker.Models;
    using Lurker.Services;
    using MahApps.Metro.Controls.Dialogs;

    /// <summary>
    /// Class BuildManagerViewModel.
    /// Implements the <see cref="Caliburn.Micro.PropertyChangedBase" />.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
    public class CharacterManagerViewModel : Caliburn.Micro.PropertyChangedBase
    {
        #region Fields

        private ObservableCollection<Player> _characters;
        private Func<string, string, MessageDialogStyle?, Task<MessageDialogResult>> _showMessage;
        private PlayerService _playerService;
        private string _newCharacterName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterManagerViewModel" /> class.
        /// </summary>
        /// <param name="showMessage">The show message.</param>
        public CharacterManagerViewModel(Func<string, string, MessageDialogStyle?, Task<MessageDialogResult>> showMessage)
        {
            this._playerService = new PlayerService();
            this._showMessage = showMessage;

            this._characters = new ObservableCollection<Player>();
            foreach (var player in this._playerService.Players)
            {
                this._characters.Add(player);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the path of building code.
        /// </summary>
        /// <value>The path of building code.</value>
        public string PathOfBuildingCode { get; set; }

        /// <summary>
        /// Gets the configurations.
        /// </summary>
        /// <value>The configurations.</value>
        public ObservableCollection<Player> Characters
        {
            get
            {
                return this._characters;
            }

            private set
            {
                this._characters = value;
            }
        }

        /// <summary>
        /// Gets or sets new character name.
        /// </summary>
        public string NewCharacterName
        {
            get
            {
                return this._newCharacterName;
            }

            set
            {
                this._newCharacterName = value;
                this.NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds this instance.
        /// </summary>
        public void Add()
        {
            if (string.IsNullOrEmpty(this.NewCharacterName))
            {
                return;
            }

            var newCharacter = new Player() { Name = this.NewCharacterName };
            if (this._playerService.Add(newCharacter))
            {
                this.Characters.Add(newCharacter);
            }

            this.NewCharacterName = string.Empty;
        }

        /// <summary>
        /// Removes the specified configuration.
        /// </summary>
        /// <param name="player">The player.</param>
        public async void Remove(Player player)
        {
            var result = await this._showMessage("Are you sure?", $"You are about to delete {player.Name}", MessageDialogStyle.AffirmativeAndNegative);

            if (result == MessageDialogResult.Affirmative)
            {
                this._playerService.Remove(player);
                this.Characters.Remove(player);
            }
        }

        #endregion
    }
}