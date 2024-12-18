﻿//-----------------------------------------------------------------------
// <copyright file="GemLocationViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System.Collections.Generic;
using PoeLurker.Core.Models;

/// <summary>
/// Represents a gem location.
/// </summary>
public class GemLocationViewModel
{
    #region Fields

    private readonly GemLocation _location;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="GemLocationViewModel"/> class.
    /// </summary>
    /// <param name="location">The location.</param>
    public GemLocationViewModel(GemLocation location)
    {
        _location = location;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the act.
    /// </summary>
    public string Act => _location.Act;

    /// <summary>
    /// Gets the NPC.
    /// </summary>
    public string Npc => $"({_location.Npc})";

    /// <summary>
    /// Gets the quest.
    /// </summary>
    public string Quest => _location.Quest;

    /// <summary>
    /// Gets the classes.
    /// </summary>
    public IEnumerable<Class> Classes => _location.Classes;

    #endregion
}