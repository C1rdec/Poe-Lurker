//-----------------------------------------------------------------------
// <copyright file="CurrencyTypeParser.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Helpers
{
    using Lurker.Models;
    using System.Collections.Generic;
    using System.Linq;

    public static class CurrencyTypeParser
    {
        private static readonly Dictionary<CurrencyType, string[]> Dictionary = new Dictionary<CurrencyType, string[]>
        {
            { CurrencyType.Chaos, new string[]{ "c", "chaos", "chaos orb"} },
            { CurrencyType.Exalted, new string[]{ "ex", "exa", "exalted", "exalted orb"} },
            { CurrencyType.Fusing, new string[]{ "fuse", "fusing", "orb of fusing"} },
            { CurrencyType.Mirror, new string[]{ "mirror",  "mirror of kalandra"} },
            { CurrencyType.Chromatic, new string[]{ "chrome",  "chromatic", "chromatic orb" } },
            { CurrencyType.Alchemy, new string[]{ "alch",  "alchemy", "orb of alchemy" } },
            { CurrencyType.Vaal, new string[]{ "vaal", "vaal orb" } },
        };

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The currency type</returns>
        public static CurrencyType Parse(string curencyTypeValue)
        {
            var value = curencyTypeValue.ToLower();
            foreach (var currencyType in Dictionary)
            {
                if (currencyType.Value.Contains(value))
                {
                    return currencyType.Key;
                }
            }

            return CurrencyType.Unknown;
        }
    }
}
