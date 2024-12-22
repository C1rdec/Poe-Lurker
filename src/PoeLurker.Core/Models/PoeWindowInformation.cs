//-----------------------------------------------------------------------
// <copyright file="PoeWindowInformation.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Models;

using System;

/// <summary>
/// Represents the Poe window information.
/// </summary>
/// <seealso cref="System.EventArgs" />
public class PoeWindowInformation : EventArgs
{
    #region Properties

    /// <summary>
    /// Gets or sets the width.
    /// </summary>
    public double Width { get; set; }

    /// <summary>
    /// Gets or sets the height.
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// Gets or sets the height of the exp bar.
    /// </summary>
    public double ExpBarHeight { get; set; }

    /// <summary>
    /// Gets or sets the width of the flask bar.
    /// </summary>
    public double FlaskBarWidth { get; set; }

    /// <summary>
    /// Gets or sets the height of the flask bar.
    /// </summary>
    public double FlaskBarHeight { get; set; }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    public Native.Rect Position { get; set; }

    public bool WideScreen { get; set; }

    #endregion
}