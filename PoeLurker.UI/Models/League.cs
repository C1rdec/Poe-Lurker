//-----------------------------------------------------------------------
// <copyright file="League.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a league.
    /// </summary>
    public class League
    {
        #region Fields

        private List<string> _possibleNames = new List<string>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets the possible names.
        /// </summary>
        public IEnumerable<string> PossibleNames => this._possibleNames;

        #endregion

        #region Methods

        /// <summary>
        /// Adds the name.
        /// </summary>
        /// <param name="name">The name.</param>
        public void AddName(string name)
        {
            this._possibleNames.Add(name);
        }

        #endregion
    }
}