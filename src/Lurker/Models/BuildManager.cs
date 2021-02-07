//-----------------------------------------------------------------------
// <copyright file="BuildManager.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using System.Collections.Generic;
    using ConfOxide;

    /// <summary>
    /// Represents the build manager.
    /// </summary>
    public sealed class BuildManager : SettingsBase<BuildManager>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the build.
        /// </summary>
        public List<SimpleBuild> Builds { get; set; }

        #endregion
    }
}