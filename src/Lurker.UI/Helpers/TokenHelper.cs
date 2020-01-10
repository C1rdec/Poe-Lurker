//-----------------------------------------------------------------------
// <copyright file="TokenHelper.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Helpers
{
    using Lurker.UI.ViewModels;
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
        public static string ReplaceToken(string message, OfferViewModel offer)
        {
            message = Regex.Replace(message, ItemName, offer.ItemName, RegexOptions.IgnoreCase);
            message = Regex.Replace(message, BuyerName, offer.PlayerName, RegexOptions.IgnoreCase);
            message = Regex.Replace(message, Price, offer.Price.ToString(), RegexOptions.IgnoreCase);
            return message;
        }

        #endregion
    }
}
