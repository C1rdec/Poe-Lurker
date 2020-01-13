//-----------------------------------------------------------------------
// <copyright file="Gloves.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    public class Gloves : PoeItem
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

        #endregion
    }
}
