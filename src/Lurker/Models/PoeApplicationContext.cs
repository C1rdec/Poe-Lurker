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
        /// Gets or sets a value indicating whether [in foreground].
        /// </summary>
        public static bool InForeground { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is afk.
        /// </summary>
        public static bool IsAfk { get; set; }

        #endregion
    }
}