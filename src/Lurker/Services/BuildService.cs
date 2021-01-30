//-----------------------------------------------------------------------
// <copyright file="BuildService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using ConfOxide;
    using Lurker.Models;

    /// <summary>
    /// Represents the build service.
    /// </summary>
    /// <seealso cref="Lurker.Services.ServiceBase" />
    public class BuildService : ServiceBase
    {
        #region Fields

        private BuildManager _buildManager;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildService"/> class.
        /// </summary>
        public BuildService()
        {
            this._buildManager = new BuildManager();

            if (!File.Exists(this.FilePath))
            {
                using (var file = File.Create(this.FilePath))
                {
                }

                this.Save();
            }
            else
            {
                try
                {
                    this._buildManager.ReadJsonFile(this.FilePath);
                }
                catch
                {
                    File.Delete(this.FilePath);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        protected override string FileName => "Build.json";

        #endregion

        #region Methods

        /// <summary>
        /// Saves the specified raise event.
        /// </summary>
        public void Save()
        {
            this._buildManager.WriteJsonFile(this.FilePath);
        }

        /// <summary>
        /// Adds the build.
        /// </summary>
        /// <param name="build">The build.</param>
        /// <returns>Simple Build.</returns>
        public SimpleBuild AddBuild(Build build)
        {
            var simpleBuild = new SimpleBuild()
            {
                PathOfBuildingCode = build.Value,
            };

            this._buildManager.Builds.Add(simpleBuild);

            return simpleBuild;
        }

        /// <summary>
        /// Removes the build.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public void RemoveBuild(string id)
        {
            var build = this._buildManager.Builds.FirstOrDefault(b => b.Id == id);
            if (build != null)
            {
                this._buildManager.Builds.Remove(build);
            }
        }

        /// <summary>
        /// Gets the builds.
        /// </summary>
        public IEnumerable<SimpleBuild> Builds => this._buildManager.Builds;

        #endregion
    }
}