//-----------------------------------------------------------------------
// <copyright file="ItemLine.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models.Ninja
{
    /// <summary>
    /// Represents a Item line.
    /// </summary>
    public class ItemLine
    {
        #region Properties

        /// <summary>
        /// Gets or sets the CurrencyTypeName.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the ChaosValue.
        /// </summary>
        public double ChaosValue { get; set; }

        /// <summary>
        /// Gets or sets the ExaltedValue.
        /// </summary>
        public double ExaltedValue { get; set; }

        /// <summary>
        /// Gets or sets the Links.
        /// </summary>
        public int Links { get; set; }

        #endregion
    }
}