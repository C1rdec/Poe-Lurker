//-----------------------------------------------------------------------
// <copyright file="Collaboration.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Models;

using System;
using PoeLurker.Core.Extensions;

/// <summary>
/// Represents a collaboration.
/// </summary>
public class Collaboration
{
    #region Properties

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the expire date.
    /// </summary>
    public DateTime ExpireDate { get; set; }

    /// <summary>
    /// Gets or sets the URL.
    /// </summary>
    public Uri Url { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Determines whether this instance is expired.
    /// </summary>
    /// <returns>
    ///   <c>true</c> if this instance is expired; otherwise, <c>false</c>.
    /// </returns>
    public bool IsExpired()
    {
        return DateTime.Compare(DateTime.Now, ExpireDate) >= 0 ? true : false;
    }

    /// <summary>
    /// Opens this instance.
    /// </summary>
    public void Open()
    {
        ProcessExtensions.OpenUrl(Url.ToString());
    }

    #endregion
}