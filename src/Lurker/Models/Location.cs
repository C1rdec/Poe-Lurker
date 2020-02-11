//-----------------------------------------------------------------------
// <copyright file="Location.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Text.RegularExpressions;

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

        /// <summary>
        /// Factory method that creates a location of an item from the given log line.
        /// </summary>
        /// <param name="logLine">The log line.</param>
        /// <returns>The location of the requested item.</returns>
        public static Location FromLogLine(string logLine)
        {
            Match match = Regex.Match(logLine, @"(?:\(stash (?:tab )*\""(?<tab>.*)\""; (?:position: )*left (?<left>[0-9])+, top (?<top>[0-9]+)\))");
            if (match.Success)
            {
                return new Location()
                {
                    StashTabName = match.Groups["tab"].Value,
                    Left = Convert.ToInt32(match.Groups["left"].Value),
                    Top = Convert.ToInt32(match.Groups["top"].Value)
                };
            }
            return new Location();
        }

        #endregion
    }
}
