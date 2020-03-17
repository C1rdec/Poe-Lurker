//-----------------------------------------------------------------------
// <copyright file="Ring.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

using Lurker.Services;
using System.Collections.Generic;

namespace Lurker.Models
{
    public class Ring : PoeItem
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Helmet"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Ring(string value)
            : base(value)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the item class.
        /// </summary>
        public override ItemClass ItemClass => ItemClass.Ring;

        #endregion

        /// <summary>
        /// Gets the important affixes.
        /// </summary>
        public override IEnumerable<Affix> ImportantAffixes => new Affix[] 
        { 
            AffixService.AttackSpeedAffix(this),
            AffixService.CastSpeedAffix(this),
            AffixService.ElementalDamageWithAttackAffix(this),
            AffixService.CriticalStrikeChanceAffix(this),
            AffixService.CriticalStrikeMultiplierAffix(this),
        };
    }
}
