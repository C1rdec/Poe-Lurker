//-----------------------------------------------------------------------
// <copyright file="StashTabBank.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Models;

using System.Collections.Generic;

/// <summary>
/// Represetns the bank of tab.
/// </summary>
public sealed class StashTabBank
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StashTabBank"/> class.
    /// </summary>
    public StashTabBank()
    {
        Tabs = [];
    }

    #region Properties

    /// <summary>
    /// Gets or sets the tabs.
    /// </summary>
    public List<StashTab> Tabs { get; set; }

    #endregion
}