//-----------------------------------------------------------------------
// <copyright file="BuildManagerContext.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Models;

using System;
using PoeLurker.UI.ViewModels;

/// <summary>
/// Represents a build manager context.
/// </summary>
public class BuildManagerContext
{
    #region Fields

    private readonly Action<BuildConfigurationViewModel> _remove;
    private readonly Action<BuildConfigurationViewModel> _open;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildManagerContext"/> class.
    /// </summary>
    /// <param name="remove">The remove.</param>
    /// <param name="open">The open.</param>
    public BuildManagerContext(Action<BuildConfigurationViewModel> remove, Action<BuildConfigurationViewModel> open)
    {
        _remove = remove;
        _open = open;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Removes the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public void Remove(BuildConfigurationViewModel configuration)
    {
        _remove(configuration);
    }

    /// <summary>
    /// Opens the specified configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public void Open(BuildConfigurationViewModel configuration)
    {
        _open(configuration);
    }

    #endregion
}