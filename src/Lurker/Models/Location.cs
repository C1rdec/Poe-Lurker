//-----------------------------------------------------------------------
// <copyright file="Location.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    public class Location
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the stash tab.
        /// </summary>
        public string StashTabName { get; set; }

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        public int Left { get; set; }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        public int Top { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Converts to string.
        /// </summary>
        public override string ToString()
        {
            return $"Tab: {this.StashTabName}, Left {this.Left}, Top {this.Top}";
        }

        #endregion
    }
}
