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
public sealed class BuildManager : AppDataFileBase<BuildManager>
{
    #region Properties

    /// <summary>
    /// Gets or sets the build.
    /// </summary>
    public List<SimpleBuild> Builds { get; set; }

    protected override string FileName => "Builds.json";

    protected override string FolderName => "PoeLurker";

    #endregion
}