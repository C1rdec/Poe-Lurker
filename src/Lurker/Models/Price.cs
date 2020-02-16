//-----------------------------------------------------------------------
// <copyright file="Price.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

using Lurker.Models;

namespace Lurker.Models
{
    public class Price
    {
        #region Properties

        /// <summary>
        /// Gets or sets the number of currencies.
        /// </summary>
        public double NumberOfCurrencies { get; set; }

        /// <summary>
        /// Gets or sets the type of the currency.
        /// </summary>
        public CurrencyType CurrencyType { get; set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return obj is Price price &&
                   this.NumberOfCurrencies == price.NumberOfCurrencies &&
                   this.CurrencyType == price.CurrencyType;
        }

        public override int GetHashCode()
        {
            var hashCode = -281760288;
            hashCode = hashCode * -1521134295 + this.NumberOfCurrencies.GetHashCode();
            hashCode = hashCode * -1521134295 + this.CurrencyType.GetHashCode();
            return hashCode;
        }

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
