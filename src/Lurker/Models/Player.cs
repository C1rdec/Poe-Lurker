//-----------------------------------------------------------------------
// <copyright file="Player.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using ConfOxide;

    /// <summary>
    /// Represents a player.
    /// </summary>
    public sealed class Player : SettingsBase<Player>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the build.
        /// </summary>
        public BuildHelperSettings Build { get; set; }

        /// <summary>
        /// Gets or sets the ignored map mods.
        /// </summary>
        public List<string> IgnoredMapMods { get; set; }

        /// <summary>
        /// Gets or sets the levels.
        /// </summary>
        public List<int> Levels { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the current level.
        /// </summary>
        /// <returns>The current known level.</returns>
        public int GetCurrentLevel()
        {
            return this.Levels.FirstOrDefault();
        }

        /// <summary>
        /// Adds the level.
        /// </summary>
        /// <param name="level">The level.</param>
        public void AddLevel(int level)
        {
            this.Levels.Insert(0, level);
        }

        #endregion
    }
}