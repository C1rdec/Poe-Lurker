//-----------------------------------------------------------------------
// <copyright file="RarityParser.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Parsers
{
    using Lurker.Models.Items;
    using System.Collections.Generic;

    public class RarityParser: ExactEnumParserBase<Rarity>
    {
        protected override Dictionary<Rarity, string[]> Dictionary => new Dictionary<Rarity, string[]>
        {
            { Rarity.Unique, new string[]{ "unique"} },
            { Rarity.Rare, new string[]{ "rare"} },
            { Rarity.Magique, new string[]{ "magic"} },
            { Rarity.Normal, new string[]{ "normal"} },
            { Rarity.Gem, new string[]{ "gem"} },
            { Rarity.Currency, new string[]{ "currency"} },
            { Rarity.DivinationCard, new string[]{ "divination card"} },
        };
    }
}
