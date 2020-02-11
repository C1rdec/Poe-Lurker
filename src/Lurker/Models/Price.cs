//-----------------------------------------------------------------------
// <copyright file="Price.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

using Lurker.Items;
using Lurker.Items.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Lurker.Models
{
    public class Price
    {
        #region Fields

        private static readonly CurrencyTypeParser currencyTypeParser = new CurrencyTypeParser();

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the number of currencies.
        /// </summary>
        public double NumberOfCurrencies { get; set; }

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

        /// <summary>
        /// Factory method that creates Price from the given log line.
        /// </summary>
        /// <param name="logLine">A line of log.</param>
        /// <returns>Returns an instance of Price.</returns>
        public static Price FromLogLine(string logLine)
        {
            Match match = Regex.Match(logLine, @"(?:listed for|for my) (?<price>[0-9\.]+) (?<currency>.*?) in");
            if (match.Success)
            {
                return new Price()
                {
                    CurrencyType = currencyTypeParser.Parse(match.Groups["currency"].Value),
                    NumberOfCurrencies = double.Parse(match.Groups["price"].Value, CultureInfo.InvariantCulture)
                };
            }
            return new Price();
        }

        #endregion
    }
}
