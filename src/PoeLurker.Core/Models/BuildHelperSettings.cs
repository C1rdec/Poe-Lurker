﻿//-----------------------------------------------------------------------
// <copyright file="BuildHelperSettings.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Models;

using System.Collections.Generic;

/// <summary>
/// Represents build helper settings.
/// </summary>
public sealed class BuildHelperSettings
{
    #region Properties

    /// <summary>
    /// Gets or sets the build identifier.
    /// </summary>
    public string BuildId { get; set; }

    /// <summary>
    /// Gets or sets the skill selected.
    /// </summary>
    public List<int> SkillsSelected { get; set; }

    /// <summary>
    /// Gets or sets the items selected.
    /// </summary>
    public List<int> ItemsSelected { get; set; }

    #endregion
}