//-----------------------------------------------------------------------
// <copyright file="BuildManager.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Models;

using System.Collections.Generic;
using Lurker.AppData;

/// <summary>
/// Represents the build manager.
/// </summary>
public class BuildManager : AppDataFileBase<BuildManager>
{
    public BuildManager()
    {
        Builds = [];
    }

    #region Properties

    /// <summary>
    /// Gets or sets the build.
    /// </summary>
    public List<BuildSettings> Builds { get; set; }

    protected override string FileName => "BuildSettings.json";

    protected override string FolderName => "PoeLurker";

    #endregion
}