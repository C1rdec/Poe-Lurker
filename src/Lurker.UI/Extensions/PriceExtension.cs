//-----------------------------------------------------------------------
// <copyright file="PriceExtension.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Extensions
{
    using Lurker.Patreon.Models;

    /// <summary>
    /// Represents the price extensions.
    /// </summary>
    public static class PriceExtension
    {
        #region Methods

        /// <summary>
        /// Calculates the value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The value in chaos.</returns>
        public static double CalculateValue(this Price value)
        {
            double ratio = 1;
            switch (value.CurrencyType)
            {
                case CurrencyType.Exalted:
                    ratio = 100;
                    break;
                case CurrencyType.Divine:
                    ratio = 10;
                    break;
                case CurrencyType.Fusing:
                    ratio = 2;
                    break;
                case CurrencyType.Alteration:
                    ratio = 0.25;
                    break;
                case CurrencyType.Jeweller:
                    ratio = 0.1;
                    break;
                case CurrencyType.Chromatic:
                    ratio = 0.15;
                    break;
            }

            return value.NumberOfCurrencies * ratio;
        }

        #endregion
    }
}