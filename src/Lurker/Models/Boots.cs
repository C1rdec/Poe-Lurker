//-----------------------------------------------------------------------
// <copyright file="Boots.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    public class Boots : SocketableItem
    {
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

        #endregion
    }
}
