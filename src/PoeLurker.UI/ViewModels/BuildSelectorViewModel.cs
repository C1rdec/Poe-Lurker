//-----------------------------------------------------------------------
// <copyright file="BuildSelectorViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Collections.ObjectModel;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;

/// <summary>
/// Represents the build selector.
/// </summary>
/// <seealso cref="Caliburn.Micro.ActivationProcessedEventArgs" />
public class BuildSelectorViewModel : Caliburn.Micro.ActivationProcessedEventArgs
{
    #region Fields

    private readonly BuildService _buildService;

    #endregion

    #region Contructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildSelectorViewModel"/> class.
    /// </summary>
    /// <param name="buildService">The build service.</param>
    public BuildSelectorViewModel(BuildService buildService, string activeBuildPath)
    {
        _buildService = buildService;
        Builds = new ObservableCollection<BuildConfigurationViewModel>();

        foreach (var build in BuildService.Get())
        {
            var viewModel = new BuildConfigurationViewModel(build);

            if (!string.IsNullOrEmpty(activeBuildPath))
            {
                viewModel.Selected = build.FilePath == activeBuildPath;
            }

            Builds.Add(viewModel);
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when [build selected].
    /// </summary>
    public event EventHandler<Build> BuildSelected;

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets the builds.
    /// </summary>
    public ObservableCollection<BuildConfigurationViewModel> Builds { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Selects the specified build.
    /// </summary>
    /// <param name="build">The build.</param>
    public void Select(BuildConfigurationViewModel viewModel)
    {
        foreach (var build in Builds)
        {
            build.Selected = false;
        }

        viewModel.Selected = true;
        BuildSelected?.Invoke(this, viewModel.Build);
    }

    #endregion
}