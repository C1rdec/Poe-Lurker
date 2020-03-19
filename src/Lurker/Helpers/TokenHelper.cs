//-----------------------------------------------------------------------
// <copyright file="TokenHelper.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Helpers
{
    using Lurker.Patreon.Events;
    using System.Text.RegularExpressions;

    public static class TokenHelper
    {
        #region Fields

        public static readonly string ItemName = "{ItemName}";
        public static readonly string BuyerName = "{BuyerName}";
        public static readonly string Price = "{Price}";

        #endregion

        #region Methods

        /// <summary>
        /// Replaces the token.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="offer">The offer.</param>
        /// <returns>The string with token replaced</returns>
        public static string ReplaceToken(string message, TradeEvent trade)
        {
            message = Regex.Replace(message, ItemName, trade.ItemName, RegexOptions.IgnoreCase);
            message = Regex.Replace(message, BuyerName, trade.PlayerName, RegexOptions.IgnoreCase);
            message = Regex.Replace(message, Price, trade.Price.ToString(), RegexOptions.IgnoreCase);
            return message;
        }

        #endregion
    }
}
