//-----------------------------------------------------------------------
// <copyright file="AffixService.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------


namespace Lurker.Services
{
    using Lurker.Models;
    using Lurker.Models.TradeAPI;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class AffixService
    {
        #region Fields
        
        public static readonly string ImplicitMarker = " (implicit)";
        public static readonly string CraftedMarker = " (crafted)";
        private IEnumerable<AffixEntry> _affixes;
        private static readonly TradeApiClient Client = new TradeApiClient();
        private static AffixService _instance;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AffixService"/> class.
        /// </summary>
        /// <param name="affixes">The affixes.</param>
        private AffixService(IEnumerable<AffixEntry> affixes)
        {
            this._affixes = affixes;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the all the affixes.
        /// </summary>
        private IEnumerable<AffixEntry> Affixes => this._affixes;

        /// <summary>
        /// Gets the explicits.
        /// </summary>
        private IEnumerable<AffixEntry> Explicits => this._affixes.Where(a => a.Type == AffixType.Explicit);

        /// <summary>
        /// Gets the implicits.
        /// </summary>
        private IEnumerable<AffixEntry> Implicits => this._affixes.Where(a => a.Type == AffixType.Implicit);

        /// <summary>
        /// Gets the pseudos.
        /// </summary>
        private IEnumerable<AffixEntry> Pseudos => this._affixes.Where(a => a.Type == AffixType.Pseudo);

        /// <summary>
        /// Gets the crafted.
        /// </summary>
        private IEnumerable<AffixEntry> Crafted => this._affixes.Where(a => a.Type == AffixType.Crafted);


        #endregion

        #region Methods

        /// <summary>
        /// Gets the total strength.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The sum of strength.</returns>
        public static double GetTotalStrength(PoeItem item)
        {
            return AffixSum(AttributeAffixes.StrengthTexts, item);
        }

        /// <summary>
        /// Gets the total dexterity.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The sum of dexterity.</returns>
        public static double GetTotalDexterity(PoeItem item)
        {
            return AffixSum(AttributeAffixes.DexterityTexts, item);

        }

        /// <summary>
        /// Gets the total intelligence.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The sum of Intelligence</returns>
        public static double GetTotalIntelligence(PoeItem item)
        {
            return AffixSum(AttributeAffixes.IntelligenceTexts, item);
        }

        /// <summary>
        /// Gets the total cold resistance.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The sum of cold resistances</returns>
        public static double GetTotalColdResistance(PoeItem item)
        {
            return AffixSum(ResistanceAffixes.ColdResistanceTexts, item);
        }

        /// <summary>
        /// Gets the total fire resistance.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The sum of fire resistance.</returns>
        public static double GetTotalFireResistance(PoeItem item)
        {
            return AffixSum(ResistanceAffixes.FireResistanceTexts, item);
        }

        /// <summary>
        /// Gets the total lightning resistance.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The sum of lightning resistance.</returns>
        public static double GetTotalLightningResistance(PoeItem item)
        {
            return AffixSum(ResistanceAffixes.LightningResistanceTexts, item);
        }

        /// <summary>
        /// Gets the total life.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The total life of the item.</returns>
        public static double GetTotalLife(PoeItem item)
        {
            double increasedLifeCount = 0;

            var strengthCount = GetTotalStrength(item);
            if (strengthCount > 0)
            {
                var strBonus = (int)(strengthCount / 10);
                increasedLifeCount = strBonus * 5;
            }

            var instance = GetInstance();
            var maximumLifeAffixes = instance.Affixes.Where(a => a.text == AttributeAffixes.MaximumLifeText);

            foreach (var maximumLifeAffix in maximumLifeAffixes)
            {
                var affix = item.Affixes.FirstOrDefault(a => a.Id == maximumLifeAffix.Id);
                if (affix != null)
                {
                    increasedLifeCount += affix.Value;
                }
            }

            return increasedLifeCount;
        }

        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <returns>The affix service.</returns>
        public static async Task<bool> InitializeAsync()
        {
            try
            {
                if (_instance == null)
                {
                    var affixes = await Client.GetAffixes();
                    _instance = new AffixService(affixes);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Finds the explicit identifier.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The explicit id.</returns>
        public static string FindExplicitId(string text)
        {
            var instance = GetInstance();
            var affix = instance.Explicits.FirstOrDefault(e => e.text == text);
            if (affix == null)
            {
                return null;
            }

            return affix.Id;
        }

        /// <summary>
        /// Finds the crafted identifier.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The affix id.</returns>
        public static string FindCraftedId(string text)
        {
            var newText = text.Replace(CraftedMarker, string.Empty);
            var instance = GetInstance();
            var affix = instance.Crafted.FirstOrDefault(e => e.text == newText);
            if (affix == null)
            {
                return null;
            }

            return affix.Id;
        }

        /// <summary>
        /// Finds the implicit identifier.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The implicit id</returns>
        public static string FindImplicitId(string text)
        {
            var newText = text.Replace(ImplicitMarker, string.Empty);
            var instance = GetInstance();
            var affix = instance.Implicits.FirstOrDefault(e => e.text == newText);
            if (affix == null)
            {
                return null;
            }

            return affix.Id;
        }

        /// <summary>
        /// Simplifies the affix text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The simplified text.</returns>
        public static string SimplifyText(string text)
        {
            text = text.Replace(ImplicitMarker, string.Empty);
            text = text.Replace(CraftedMarker, string.Empty);

            return text;
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>The singleton.</returns>
        private static AffixService GetInstance()
        {
            if (_instance == null)
            {
                throw new System.InvalidOperationException("Needs to be Initialize first");
            }

            return _instance;
        }

        /// <summary>
        /// Affixes the sum.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="item">The item.</param>
        /// <returns>The sum of the affixes</returns>
        private static double AffixSum(IEnumerable<string> values, PoeItem item)
        {
            return item.Affixes.Where(a => values.Contains(a.Text)).Sum(a => a.Value);

        }

        #endregion
    }
}
