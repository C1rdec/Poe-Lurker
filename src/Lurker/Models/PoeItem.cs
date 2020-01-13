//-----------------------------------------------------------------------
// <copyright file="PoeItem.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

using Lurker.Extensions;
using Lurker.Parsers;
using System.Linq;

namespace Lurker.Models
{
    public abstract class PoeItem
    {
        #region Fields

        private static readonly string ItemLevelMarker = "Item Level: ";
        private static readonly string RarityMarker = "Rarity: ";
        private static readonly string UnidentifiedMarker = "Unidentified";
        private static readonly string Seperator = "--------";
        private static readonly RarityParser RarityParser = new RarityParser();
        private static readonly ItemClassParser ItemClassParser = new ItemClassParser();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PoeItem"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public PoeItem(string value)
        {
            this.Identified = value.IndexOf(UnidentifiedMarker) == -1;
            this.BaseType = GetBaseType(value);
            this.ItemLevel = GetItemLevel(value);
            this.Rarity = GetRarity(value);

            // need to know if the item is identified first
            this.Name = this.GetName(value);
        }

        #endregion

        #region Properties

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
            var itemLevelIndex = value.IndexOf(ItemLevelMarker);
            if (itemLevelIndex == -1)
            {
                return default;
            }

            return int.Parse(value.GetLine(itemLevelIndex, ItemLevelMarker));
        }

        /// <summary>
        /// Gets the rarity.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The item rarity</returns>
        private static Rarity GetRarity(string value)
        {
            var rarityIndex = value.IndexOf(RarityMarker);
            if (rarityIndex == -1)
            {
                return default;
            }

            return RarityParser.Parse(value.GetLine(rarityIndex, RarityMarker));
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The item name.</returns>
        private string GetName(string value)
        {
            if (this.Identified)
            {
                var sections = value.Split(Seperator);
                var headerSection = sections[0];
                return headerSection.Split(System.Environment.NewLine).ElementAt(1);
            }

            return string.Empty;
        }

        #endregion
    }
}
