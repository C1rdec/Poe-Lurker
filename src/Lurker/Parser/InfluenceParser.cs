//-----------------------------------------------------------------------
// <copyright file="InfluenceParser.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Parser
{
    using Lurker.Models;
    using System.Collections.Generic;

    class InfluenceParser : ExactEnumParserBase<Influence>
    {
        protected override Dictionary<Influence, string[]> Dictionary => new Dictionary<Influence, string[]>
        {
            { Influence.Shaper, new string[]{ "shaper"} },
            { Influence.Elder, new string[]{ "elder"} },
            { Influence.Hunter, new string[]{ "hunter"} },
            { Influence.Redeemer, new string[]{ "redeemer" } },
            { Influence.Warlord, new string[]{ "warlord"} },
            { Influence.Crusader, new string[]{ "crusader" } },

        };
    }
}
