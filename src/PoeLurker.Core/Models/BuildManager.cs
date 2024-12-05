//-----------------------------------------------------------------------
// <copyright file="BuildManager.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Models;

using System.Collections.Generic;

/// <summary>
/// Represents the build manager.
/// </summary>
public sealed class BuildManager
{
    public BuildManager()
    {
        Builds = [];
    }

    #region Properties

    /// <summary>
    /// Gets or sets the build.
    /// </summary>
    public List<SimpleBuild> Builds { get; set; }

    #endregion
}