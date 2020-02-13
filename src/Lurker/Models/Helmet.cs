//-----------------------------------------------------------------------
// <copyright file="Helmet.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    public class Helmet : SocketableItem
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Helmet"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Helmet(string value)
            : base(value)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the item class.
        /// </summary>
        public override ItemClass ItemClass => ItemClass.Helmet;

        #endregion
    }
}
