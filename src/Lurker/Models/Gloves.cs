//-----------------------------------------------------------------------
// <copyright file="Gloves.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using Lurker.Services;
    using System.Collections.Generic;

    public class Gloves : SocketableItem
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Gloves"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Gloves(string value)
            : base(value)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the item class.
        /// </summary>
        public override ItemClass ItemClass => ItemClass.Gloves;

        /// <summary>
        /// Gets the important affixes.
        /// </summary>
        public override IEnumerable<Affix> ImportantAffixes => new Affix[]
        {
            AffixService.AttackSpeedAffix(this),
        };

        #endregion
    }
}
