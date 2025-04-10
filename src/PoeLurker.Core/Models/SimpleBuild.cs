﻿//-----------------------------------------------------------------------
// <copyright file="SimpleBuild.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Models;

/// <summary>
/// Represents a serializable build.
/// </summary>
public sealed class SimpleBuild
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleBuild"/> class.
    /// </summary>
    public SimpleBuild()
    {
        Id = System.Guid.NewGuid().ToString();
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the path of building code.
    /// </summary>
    public string PathOfBuildingCode { get; set; }

    /// <summary>
    /// Gets or sets the youtube URL.
    /// </summary>
    public string YoutubeUrl { get; set; }

    /// <summary>
    /// Gets or sets the forum URL.
    /// </summary>
    public string ForumUrl { get; set; }

    /// <summary>
    /// Gets or sets the notes.
    /// </summary>
    public string Notes { get; set; }

    #endregion
}