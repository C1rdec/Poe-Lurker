//-----------------------------------------------------------------------
// <copyright file="Price.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Models
{
    public class Price
    {
        #region Properties

        /// <summary>
        /// Gets or sets the number of currencies.
        /// </summary>
        public int NumberOfCurrencies { get; set; }

        /// <summary>
        /// Gets or sets the type of the currency.
        /// </summary>
        public CurrencyType CurrencyType { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Converts to string.
        /// </summary>
        public override string ToString()
        {
            return $"{this.NumberOfCurrencies} {this.CurrencyType}";
        }

        #endregion
    }
}
