//-----------------------------------------------------------------------
// <copyright file="PoeApplicationContext.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using System;

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

        /// <summary>
        /// Gets or sets the window style.
        /// </summary>
        public static IntPtr WindowStyle { get; set; }

        #endregion
    }
}
