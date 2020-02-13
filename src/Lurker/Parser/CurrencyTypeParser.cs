//-----------------------------------------------------------------------
// <copyright file="CurrencyTypeParser.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Parser
{
    using Lurker.Models;
    using System.Collections.Generic;

    public class CurrencyTypeParser: ExactEnumParserBase<CurrencyType>
    {
        protected override Dictionary<CurrencyType, string[]> Dictionary => new Dictionary<CurrencyType, string[]>
        {
            { CurrencyType.Chaos, new string[]{ "c", "chaos", "chaos orb"} },
            { CurrencyType.Exalted, new string[]{ "ex", "exa", "exalted", "exalted orb"} },
            { CurrencyType.Fusing, new string[]{ "fuse", "fusing", "orb of fusing"} },
            { CurrencyType.Mirror, new string[]{ "mirror",  "mirror of kalandra"} },
            { CurrencyType.Chromatic, new string[]{ "chrome",  "chromatic", "chromatic orb" } },
            { CurrencyType.Alchemy, new string[]{ "alch",  "alchemy", "orb of alchemy" } },
            { CurrencyType.Vaal, new string[]{ "vaal", "vaal orb" } },
            { CurrencyType.Regal, new string[]{ "regal", "regal orb" } },
            { CurrencyType.Divine, new string[]{ "divine", "divine orb" } },
            { CurrencyType.Alteration, new string[]{ "alt", "alteration", "orb of alteration" } },
            { CurrencyType.Jeweller, new string[]{ "jew", "jewellers", "jeweller's orb" } },
        };
    }
}
