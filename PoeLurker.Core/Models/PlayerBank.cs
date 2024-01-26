//-----------------------------------------------------------------------
// <copyright file="PlayerBank.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Models;

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The bank of players.
/// </summary>
public sealed class PlayerBank
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

    #endregion
}