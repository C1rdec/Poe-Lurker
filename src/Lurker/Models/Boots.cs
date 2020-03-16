//-----------------------------------------------------------------------
// <copyright file="Boots.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using Lurker.Services;
    using System.Collections.Generic;

    public class Boots : SocketableItem
    {
        #region Fields

        public static readonly string MovementSpeedText = "#% increased Movement Speed";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Boots"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Boots(string value) 
            : base(value)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the item class.
        /// </summary>
        public override ItemClass ItemClass => ItemClass.Boots;

        /// <summary>
        /// Gets the important affixes.
        /// </summary>
        public override IEnumerable<Affix> ImportantAffixes => AffixService.GetAffixeByNames(new string[] { MovementSpeedText }, this);

        #endregion
    }
}
