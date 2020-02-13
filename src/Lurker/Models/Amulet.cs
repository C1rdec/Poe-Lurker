//-----------------------------------------------------------------------
// <copyright file="Amulet.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    public class Amulet : PoeItem
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Belt"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Amulet(string value) 
            : base(value)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the item class.
        /// </summary>
        public override ItemClass ItemClass => ItemClass.Amulet;

        #endregion
    }
}
