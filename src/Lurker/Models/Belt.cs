//-----------------------------------------------------------------------
// <copyright file="Belt.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    using Lurker.Services;
    using System.Collections.Generic;

    public class Belt : PoeItem
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Belt"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Belt(string value) 
            : base(value)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the item class.
        /// </summary>
        public override ItemClass ItemClass => ItemClass.Belt;

        /// <summary>
        /// Gets the important affixes.
        /// </summary>
        public override IEnumerable<Affix> ImportantAffixes => new Affix[]
        {
            AffixService.ElementalDamageWithAttackAffix(this),
        };

        #endregion
    }
}
