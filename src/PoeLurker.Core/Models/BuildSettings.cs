//-----------------------------------------------------------------------
// <copyright file="SimpleBuild.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Models;

/// <summary>
/// Represents a serializable build.
/// </summary>
public sealed class BuildSettings
{
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleBuild"/> class.
    /// </summary>
    public BuildSettings()
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// Gets or sets the youtube URL.
    /// </summary>
    public string YoutubeUrl { get; set; }

    /// <summary>
    /// Gets or sets the forum URL.
    /// </summary>
    public string ForumUrl { get; set; }

    public List<string> ItemBases { get; set; }

    #endregion
}