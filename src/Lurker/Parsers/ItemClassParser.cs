//-----------------------------------------------------------------------
// <copyright file="ItemClassParser.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Parsers
{
    using Lurker.Models;
    using System.Collections.Generic;

    public class ItemClassParser: EnumParserBase<ItemClass>
    {
        protected override Dictionary<ItemClass, string[]> Dictionary => new Dictionary<ItemClass, string[]>()
        {
            { ItemClass.Amulet, new string[]{ " amulet", " talisman" } },
            { ItemClass.Ring, new string[]{ " ring" } },
            { ItemClass.Helmet, new string[]{ " helmet", " hat", " burgonet", " cap", "tricorne", " hood", " pelt", " circlet", " cage", " heml", "sallet", " bascinet", " coif", " crown", " mask" } },
            { ItemClass.BodyArmour, new string[]{ " vest", " plate", "chestplate",  " jerkin", " leather", " tunic", " garb", " robe", " vest", " vestment", " regalia", " wrap", " silks", " brigandine", " doublet", " armour", " lamellar", " wyrmscale", " dragonscale", " coat", " ringmail", " chainmail", " hauberk", " jacket", " raiment" } },
            { ItemClass.Boots, new string[]{ " boots", " slippers", " shoes", " greaves" } },
            { ItemClass.Gloves, new string[]{ " gauntlets", " gloves", " mitts", " greaves" } },
            { ItemClass.Belt, new string[]{ " belt", " sash", " vise"} },
            { ItemClass.Bow, new string[]{ " bow"} },
            { ItemClass.Quiver, new string[]{ " quiver"} },
            { ItemClass.Map, new string[]{ " map"} },
            { ItemClass.Currency, new string[]{ "currency"} },
            { ItemClass.Gem, new string[]{ "gem"} },
            { ItemClass.DivinationCard, new string[]{ "divinationcard"} },
            { ItemClass.Flask, new string[]{ " flask"} },
            { ItemClass.Staff, new string[]{ " branch", " staff", "quarterstaff", "lathi"} },
            { ItemClass.Jewel, new string[]{ " jewel"} },
            { ItemClass.Shield, new string[]{ " shield", " buckler", " bundle"} },
            { ItemClass.Wand, new string[]{ " wand", " horn"} },
            { ItemClass.Dagger, new string[]{ " shank", " knife", "stiletto", " dagger", "porgnard", "trisula", "ambusher", "sai", " kris", "skean", " blade"} },
            { ItemClass.Claw, new string[]{ " fist", " claw", "awl", " paw", "blinder", "gouger", "ripper", "stabber"} },
        };
    }
}
