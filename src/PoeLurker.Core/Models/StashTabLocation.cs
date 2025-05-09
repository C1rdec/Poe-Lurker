﻿//-----------------------------------------------------------------------
// <copyright file="StashTabLocation.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Models;

/// <summary>
/// Represents the stash tab location.
/// </summary>
public class StashTabLocation
{
    #region Properties

    /// <summary>
    /// Gets or sets Name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets Top.
    /// </summary>
    public int Top { get; set; }

    /// <summary>
    /// Gets or sets Left.
    /// </summary>
    public int Left { get; set; }

    /// <summary>
    /// Gets or sets StashTabType.
    /// </summary>
    public StashTabType StashTabType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the tab is in a folder.
    /// </summary>
    public bool InFolder { get; set; }

    #endregion
}