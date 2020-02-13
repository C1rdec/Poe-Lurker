//-----------------------------------------------------------------------
// <copyright file="ResistanceAffixes.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------


namespace Lurker.Services
{
    using System.Collections.Generic;

    public static class ResistanceAffixes
    {
        public static readonly string LightningResistanceText = "#% to Lightning Resistance";
        public static readonly string ColdResistanceText = "#% to Cold Resistance";
        public static readonly string FireResistanceText = "#% to Fire Resistance";
        public static readonly string AllResistanceText = "#% to all Elemental Resistances";

        public static readonly string FireColdResistanceText = "#% to Fire and Cold Resistances";
        public static readonly string ColdLightningResistanceText = "#% to Cold and Lightning Resistances";
        public static readonly string FireLightningResistanceText = "#% to Fire and Lightning Resistances";


        /// <summary>
        /// Gets the cold resistance texts.
        /// </summary>
        public static IEnumerable<string> ColdResistanceTexts => new string[] { ColdResistanceText, AllResistanceText, ColdLightningResistanceText };
        /// <summary>
        /// Gets the lightning resistance texts.
        /// </summary>
        public static IEnumerable<string> LightningResistanceTexts => new string[] { LightningResistanceText, AllResistanceText, ColdLightningResistanceText };

        /// <summary>
        /// Gets the fire resistance texts.
        /// </summary>
        public static IEnumerable<string> FireResistanceTexts => new string[] { FireResistanceText, AllResistanceText, FireLightningResistanceText };

    }
}
