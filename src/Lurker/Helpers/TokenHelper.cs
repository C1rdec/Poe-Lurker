//-----------------------------------------------------------------------
// <copyright file="TokenHelper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Helpers
{
    using System.Text.RegularExpressions;
    using Lurker.Patreon.Events;

    /// <summary>
    /// Represents token helper.
    /// </summary>
    public static class TokenHelper
    {
        #region Fields

        /// <summary>
        /// The item name.
        /// </summary>
        public static readonly string ItemName = "{ItemName}";

        /// <summary>
        /// The buyer name.
        /// </summary>
        public static readonly string BuyerName = "{BuyerName}";

        /// <summary>
        /// The price.
        /// </summary>
        public static readonly string Price = "{Price}";

        #endregion

        #region Methods

        /// <summary>
        /// Replaces the token.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="trade">The trade.</param>
        /// <returns>
        /// The string with token replaced.
        /// </returns>
        public static string ReplaceToken(string message, TradeEvent trade)
        {
            message = Regex.Replace(message, ItemName, trade.ItemName, RegexOptions.IgnoreCase);
            message = Regex.Replace(message, BuyerName, trade.PlayerName, RegexOptions.IgnoreCase);
            if (trade.Price.CurrencyType != Patreon.Models.CurrencyType.Unknown)
            {
                message = Regex.Replace(message, Price, trade.Price.ToString(), RegexOptions.IgnoreCase);
            }

            return message;
        }

        #endregion
    }
}