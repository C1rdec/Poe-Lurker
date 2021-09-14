//-----------------------------------------------------------------------
// <copyright file="CurrencyLine.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models.Ninja
{
    /// <summary>
    /// Represents a currency line.
    /// </summary>
    public class CurrencyLine
    {
        #region Properties

        /// <summary>
        /// Gets or sets the CurrencyTypeName.
        /// </summary>
        public string CurrencyTypeName { get; set; }

        /// <summary>
        /// Gets or sets the Chaos Equivalent.
        /// </summary>
        public double ChaosEquivalent { get; set; }

        #endregion
    }
}