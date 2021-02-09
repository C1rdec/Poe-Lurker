//-----------------------------------------------------------------------
// <copyright file="BuildSelectorViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using Lurker.Models;
    using Lurker.Services;

    /// <summary>
    /// Represents the build selector.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.ActivationProcessedEventArgs" />
    public class BuildSelectorViewModel : Caliburn.Micro.ActivationProcessedEventArgs
    {
        #region Fields

        private BuildService _buildService;

        #endregion

        #region Contructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSelectorViewModel"/> class.
        /// </summary>
        /// <param name="buildService">The build service.</param>
        public BuildSelectorViewModel(BuildService buildService)
        {
            this._buildService = buildService;
            this.Builds = new ObservableCollection<BuildConfigurationViewModel>();

            foreach (var build in this._buildService.Builds)
            {
                this.Builds.Add(new BuildConfigurationViewModel(build));
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [build selected].
        /// </summary>
        public event EventHandler<SimpleBuild> BuildSelected;

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
        public void Select(BuildConfigurationViewModel build)
        {
            this.BuildSelected?.Invoke(this, build.SimpleBuild);
        }

        #endregion
    }
}