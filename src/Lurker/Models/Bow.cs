//-----------------------------------------------------------------------
// <copyright file="Bow.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    public class Bow : PoeItem
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Helmet"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Bow(string value)
            : base(value)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the item class.
        /// </summary>
        public override ItemClass ItemClass => ItemClass.Bow;

        #endregion
    }
}
