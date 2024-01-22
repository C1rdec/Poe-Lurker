//-----------------------------------------------------------------------
// <copyright file="TradeEventHelper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Helpers;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

/// <summary>
/// Represents the trade event helper.
/// </summary>
public static class TradeEventHelper
{
    #region Fields

    private static readonly List<string> _regexes = new List<string>
    {
        "@.+Hi, I would like to buy.+", // english
        "@.+안녕하세요,.+구매하고 싶습니다", // korean
        "@.+Здравствуйте, хочу купить у вас.+", // russian
        "@.+สวัสดี, เราต้องการจะชื้อของคุณ.+", // thai
        "@.+Bonjour, je souhaiterais t'acheter.+", // french
        "@.+Hi, ich möchte.+kaufen.+", // german
        "@.+Olá, eu gostaria de comprar o seu item.+", // portuguese
        "@.+Hola, quisiera comprar tu.+", // spanish
    };

    #endregion

    #region Properties

    /// <summary>
    /// Determines whether [is trade message] [the specified message].
    /// </summary>
    /// <param name="message">The message.</param>
    /// <returns>
    ///   <c>true</c> if [is trade message] [the specified message]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsTradeMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return false;
        }

        return _regexes.Any(pattern => Regex.Match(message, pattern).Success);
    }

    #endregion
}