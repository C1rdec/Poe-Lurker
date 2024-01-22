//-----------------------------------------------------------------------
// <copyright file="UniqueItem.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

using PoeLurker.Patreon.Models;

namespace Lurker.Core.Models;

/// <summary>
/// Represents a unique item.
/// </summary>
/// <seealso cref="Lurker.Core.Models.WikiItem" />
public class UniqueItem : WikiItem
{
    #region Properties

    /// <summary>
    /// Gets or sets the item class.
    /// </summary>
    public ItemClass ItemClass { get; set; }

    #endregion
}