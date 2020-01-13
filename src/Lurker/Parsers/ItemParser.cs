//-----------------------------------------------------------------------
// <copyright file="ItemParser.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Parsers
{
    using Lurker.Models;

    public class ItemParser
    {
        #region Methods

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The parsed item.</returns>
        public PoeItem Parse(string value)
        {
            if (!value.StartsWith("Rarity: "))
            {
                return null;
            }

            switch(PoeItem.GetItemClass(value))
            {
                case ItemClass.Boots:
                    return new Boots(value);
                case ItemClass.Gloves:
                    return new Gloves(value);
                case ItemClass.BodyArmour:
                    return new BodyArmour(value);
                default:
                    return null;
            }
        }

        #endregion
    }
}
