//-----------------------------------------------------------------------
// <copyright file="PriceExtension.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

using Lurker.Patreon.Models;

namespace Lurker.UI.Extensions
{
    public static class PriceExtension
    {
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
            return value.NumberOfCurrencies * ratio;;
        }
    }
}
