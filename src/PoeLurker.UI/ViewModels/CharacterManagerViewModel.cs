//-----------------------------------------------------------------------
// <copyright file="CharacterManagerViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;

/// <summary>
/// Class BuildManagerViewModel.
/// Implements the <see cref="Caliburn.Micro.PropertyChangedBase" />.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class CharacterManagerViewModel : Caliburn.Micro.PropertyChangedBase
{
    #region Fields

    private ObservableCollection<Player> _characters;
    private readonly Func<string, string, MessageDialogStyle?, Task<MessageDialogResult>> _showMessage;
    private readonly PlayerService _playerService;
    private string _newCharacterName;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacterManagerViewModel" /> class.
    /// </summary>
    /// <param name="showMessage">The show message.</param>
    public CharacterManagerViewModel(Func<string, string, MessageDialogStyle?, Task<MessageDialogResult>> showMessage)
    {
        _playerService = new PlayerService();
        _showMessage = showMessage;

        _characters = new ObservableCollection<Player>();
        foreach (var player in _playerService.Players)
        {
            _characters.Add(player);
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
            return _characters;
        }

        private set
        {
            _characters = value;
        }
    }

    /// <summary>
    /// Gets or sets new character name.
    /// </summary>
    public string NewCharacterName
    {
        get
        {
            return _newCharacterName;
        }

        set
        {
            _newCharacterName = value;
            NotifyOfPropertyChange();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adds this instance.
    /// </summary>
    public void Add()
    {
        if (string.IsNullOrEmpty(NewCharacterName))
        {
            return;
        }

        var newCharacter = new Player() { Name = NewCharacterName };
        if (_playerService.Add(newCharacter))
        {
            Characters.Add(newCharacter);
        }

        NewCharacterName = string.Empty;
    }

    /// <summary>
    /// Removes the specified configuration.
    /// </summary>
    /// <param name="player">The player.</param>
    public async void Remove(Player player)
    {
        var result = await _showMessage("Are you sure?", $"You are about to delete {player.Name}", MessageDialogStyle.AffirmativeAndNegative);

        if (result == MessageDialogResult.Affirmative)
        {
            _playerService.Remove(player);
            Characters.Remove(player);
        }
    }

    #endregion
}