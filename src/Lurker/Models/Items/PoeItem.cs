//-----------------------------------------------------------------------
// <copyright file="PoeItem.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models.Items
{
    using Lurker.Extensions;
    using Lurker.Parsers;
    using Lurker.Services;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class PoeItem
    {
        #region Fields

        private static readonly string CorruptedMarker = "Corrupted";
        private static readonly string ItemLevelMarker = "Item Level: ";
        private static readonly string NoteMarker = "Note: ";
        private static readonly string RarityMarker = "Rarity: ";
        private static readonly string UnidentifiedMarker = "Unidentified";
        private static readonly string Seperator = "--------";
        private static readonly RarityParser RarityParser = new RarityParser();
        private static readonly ItemClassParser ItemClassParser = new ItemClassParser();
        private static readonly InfluenceParser InfluenceParser = new InfluenceParser();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PoeItem"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PoeItem(string value)
        {
            this.Identified = value.IndexOf(UnidentifiedMarker) == -1;
            this.Corrupted = value.IndexOf(CorruptedMarker) != -1;
            this.BaseType = GetBaseType(value);
            this.ItemLevel = GetItemLevel(value);
            this.Rarity = GetRarity(value);
            this.Influence = GetInfluence(value);

            if (this.Identified)
            {
                this.Name = this.GetName(value);
                this.Affixes = this.GetAffixes(value);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the affixes.
        /// </summary>
        public IEnumerable<Affix> Affixes { get; private set; }

        /// <summary>
        /// Gets or sets the rarity.
        /// </summary>
        public Rarity Rarity { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type of the base.
        /// </summary>
        public string BaseType { get; set; }

        /// <summary>
        /// Gets or sets the item class.
        /// </summary>
        public abstract ItemClass ItemClass { get; }

        /// <summary>
        /// Gets or sets the item level.
        /// </summary>
        public int ItemLevel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PoeItem"/> is identified.
        /// </summary>
        public bool Identified { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PoeItem"/> is corrupted.
        /// </summary>
        public bool Corrupted { get; set; }

        /// <summary>
        /// Gets or sets the influence.
        /// </summary>
        public Influence Influence { get; set; }

        /// <summary>
        /// Gets the total life count.
        /// </summary>
        public double TotalLifeCount => AffixService.GetTotalLife(this);

        #endregion

        #region Methods

        /// <summary>
        /// Gets the item class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The item class</returns>
        public static ItemClass GetItemClass(string value)
        {
            var rarity = GetRarity(value);
            if (rarity == Rarity.Currency || rarity == Rarity.Gem || rarity == Rarity.DivinationCard)
            {
                return ItemClassParser.Parse(rarity.ToString());
            }

            return ItemClassParser.Parse(GetBaseType(value));
        }

        /// <summary>
        /// Gets the type of the base.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>the item base type</returns>
        private static string GetBaseType(string value)
        {
            var rarity = GetRarity(value);
            if (rarity == Rarity.DivinationCard)
            {
                return value.Split(System.Environment.NewLine).ElementAt(1);
            }

            var sections = value.Split(Seperator);
            var headerSection = sections[0];
            var headerValues = headerSection.Split(System.Environment.NewLine);

            return headerValues.Length == 2 ? headerValues[1] : headerValues[2];
        }

        /// <summary>
        /// Gets the item level.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The item level.</returns>
        private static int GetItemLevel(string value)
        {
            var itemLevelValue = value.GetLineAfter(ItemLevelMarker);
            if (string.IsNullOrEmpty(itemLevelValue))
            {
                return default;
            }

            return int.Parse(itemLevelValue);
        }

        /// <summary>
        /// Gets the rarity.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The item rarity</returns>
        private static Rarity GetRarity(string value)
        {
            var rarityValue = value.GetLineAfter(RarityMarker);
            if (string.IsNullOrEmpty(rarityValue))
            {
                return default;
            }

            return RarityParser.Parse(rarityValue);
        }

        /// <summary>
        /// Gets the influence.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The item influence.</returns>
        private static Influence GetInfluence(string value)
        {
            var sections = value.Split(Seperator);

            var count = 1;
            if (HasNote(value))
            {
                count++;
            }

            var influenceSection = sections.ElementAt(sections.Length - count);
            var firstLine = influenceSection.GetLines().First();
            var firstWord = firstLine.Split(' ').First();

            return InfluenceParser.Parse(firstWord);
        }

        /// <summary>
        /// Determines whether the specified value has note.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///   <c>true</c> if the specified value has note; otherwise, <c>false</c>.
        /// </returns>
        private static bool HasNote(string value)
        {
            return value.IndexOf(NoteMarker) != -1;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The item name.</returns>
        private string GetName(string value)
        {
            var sections = value.Split(Seperator);
            var headerSection = sections[0];
            return headerSection.Split(System.Environment.NewLine).ElementAt(1);
        }

        /// <summary>
        /// Gets the affixes.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The list of affixes.</returns>
        private IEnumerable<Affix> GetAffixes(string value)
        {
            var affixes = new List<Affix>();
            var implicitValue = value.GetLine(AffixService.ImplicitMarker);
            if (!string.IsNullOrEmpty(implicitValue))
            {
                affixes.Add(new Affix(implicitValue));
            }

            var affixSection = GetAffixSection(value);
            foreach (var line in affixSection.GetLines())
            {
                affixes.Add(new Affix(line));
            }

            return affixes;
        }

        /// <summary>
        /// Gets the affix section.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private string GetAffixSection(string value)
        {
            var sections = value.Split(Seperator);

            var counter = 1;
            if(this.Rarity == Rarity.Unique)
            {
                counter++;
            }

            if (HasNote(value))
            {
                counter++;
            }

            if (this.Influence != Influence.Normal)
            {
                counter++;
            }

            if (this.Corrupted)
            {
                counter++;
            }

            return sections.ElementAt(sections.Length - counter);
        }

        #endregion
    }
}
