//-----------------------------------------------------------------------
// <copyright file="AttributeAffixes.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------


namespace Lurker.Services
{
    using System.Collections.Generic;

    public class AttributeAffixes
    {
        public static readonly string MaximumLifeText = "# to maximum Life";
        public static readonly string StrengthAttributeText = "# to Strength";
        public static readonly string DexterityAttributeText = "# to Dexterity";
        public static readonly string IntelligenceAttributeText = "# to Intelligence";
        public static readonly string AllAttributeText = "# to all Attributes";

        public static readonly string DexterityIntelligenceAttributeText = "# to Dexterity and Intelligence";
        public static readonly string StrengthDexterityAttributeText = "# to Strength and Dexterity";
        public static readonly string StrengthIntelligenceAttributeText = "# to Strength and Intelligence";

        /// <summary>
        /// Gets the strength texts.
        /// </summary>
        public static IEnumerable<string> StrengthTexts => new string[] { StrengthAttributeText, AllAttributeText, StrengthDexterityAttributeText, StrengthIntelligenceAttributeText };

        /// <summary>
        /// Gets the dexterity texts.
        /// </summary>
        public static IEnumerable<string> DexterityTexts => new string[] { DexterityAttributeText, AllAttributeText, StrengthDexterityAttributeText, DexterityIntelligenceAttributeText };

        /// <summary>
        /// Gets the intelligence texts.
        /// </summary>
        public static IEnumerable<string> IntelligenceTexts => new string[] { IntelligenceAttributeText, AllAttributeText, StrengthIntelligenceAttributeText, DexterityIntelligenceAttributeText };
    }
}
