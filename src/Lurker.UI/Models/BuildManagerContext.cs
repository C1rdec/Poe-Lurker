//-----------------------------------------------------------------------
// <copyright file="BuildManagerContext.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Models
{
    using System;
    using Lurker.UI.ViewModels;

    /// <summary>
    /// Represents a build manager context.
    /// </summary>
    public class BuildManagerContext
    {
        #region Fields

        private Action<BuildConfigurationViewModel> _remove;
        private Action<BuildConfigurationViewModel> _open;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildManagerContext"/> class.
        /// </summary>
        /// <param name="remove">The remove.</param>
        /// <param name="open">The open.</param>
        public BuildManagerContext(Action<BuildConfigurationViewModel> remove, Action<BuildConfigurationViewModel> open)
        {
            this._remove = remove;
            this._open = open;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Removes the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public void Remove(BuildConfigurationViewModel configuration)
        {
            this._remove(configuration);
        }

        /// <summary>
        /// Opens the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public void Open(BuildConfigurationViewModel configuration)
        {
            this._open(configuration);
        }

        #endregion
    }
}