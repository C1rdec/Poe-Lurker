//-----------------------------------------------------------------------
// <copyright file="PathOfExileProcessLurker.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core;

using System.Collections.Generic;
using PoeLurker.Core.Models;
using ProcessLurker;

/// <summary>
/// Represents the Path of Exile process lurker.
/// </summary>
/// <seealso cref="Lurker.ProcessLurker" />
public class PathOfExileProcessLurker : ProcessService
{
    #region Fields

    private static readonly List<string> PossibleProcessNames =
    [
        "PathOfExile",
        "PathOfExile_x64",
        "PathOfExileSteam",
        "PathOfExile_x64Steam",
        "PathOfExile_x64_KG",
        "PathOfExile_KG",
        "PathOfExile_x64EGS",
        "PathOfExileEGS",
    ];

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PathOfExileProcessLurker"/> class.
    /// </summary>
    public PathOfExileProcessLurker()
        : base(PossibleProcessNames)
    {
    }

    #endregion

    #region Methods

    /// <summary>
    /// Called when [exit].
    /// </summary>
    protected override void OnExit()
    {
        PoeApplicationContext.IsRunning = false;
    }

    #endregion
}