//-----------------------------------------------------------------------
// <copyright file="DamageValue.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    /// <summary>
    /// Represents the total damage.
    /// </summary>
    public class DamageValue
    {
        #region Properties

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is DOT.
        /// </summary>
        public bool IsDot { get; set; }

        #endregion
    }
}