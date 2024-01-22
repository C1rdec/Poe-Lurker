//-----------------------------------------------------------------------
// <copyright file="PlayerBank.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Core.Models;

using System.Collections.Generic;
using System.Linq;
using Lurker.AppData;

/// <summary>
/// The bank of players.
/// </summary>
public sealed class PlayerBank : AppDataFileBase<PlayerBank>
{
    #region Properties

    /// <summary>
    /// Gets or sets the players.
    /// </summary>
    public List<Player> Players { get; set; }

    /// <summary>
    /// Gets or sets the external players.
    /// </summary>
    public List<Player> ExternalPlayers { get; set; }

    protected override string FileName => "Players.json";

    protected override string FolderName => "PoeLurker";

    #endregion

    #region Methods

    /// <summary>
    /// Gets the known player.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>Known player or null.</returns>
    public Player GetKnownPlayer(string name)
    {
        return Players.FirstOrDefault(p => p.Name == name);
    }

    /// <summary>
    /// Gets the known player.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>Known player or null.</returns>
    public Player GetExternalPlayer(string name)
    {
        return ExternalPlayers.FirstOrDefault(p => p.Name == name);
    }

    #endregion
}