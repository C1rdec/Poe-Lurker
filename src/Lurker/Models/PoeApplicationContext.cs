//-----------------------------------------------------------------------
// <copyright file="PoeApplicationContext.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    /// <summary>
    /// Represents an applicaiton context.
    /// </summary>
    public static class PoeApplicationContext
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is running.
        /// </summary>
        public static bool IsRunning { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [in foreground].
        /// </summary>
        public static bool InForeground { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is afk.
        /// </summary>
        public static bool IsAfk { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        public static string Location { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the window style.
        /// </summary>
        public static WindowStyle WindowStyle { get; set; }

        #endregion
    }
}