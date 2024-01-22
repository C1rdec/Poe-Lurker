//-----------------------------------------------------------------------
// <copyright file="GemLocation.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Core.Models;

using System.Collections.Generic;

/// <summary>
/// Represents where the player can buy the gem.
/// </summary>
public class GemLocation
{
    #region Properties

    /// <summary>
    /// Gets or sets the name of the quest.
    /// </summary>
    public string Quest { get; set; }

    /// <summary>
    /// Gets or sets the act.
    /// </summary>
    public string Act { get; set; }

    /// <summary>
    /// Gets or sets the NPC.
    /// </summary>
    public string Npc { get; set; }

    /// <summary>
    /// Gets or sets the classes.
    /// </summary>
    public IEnumerable<Class> Classes { get; set; }

    #endregion
}