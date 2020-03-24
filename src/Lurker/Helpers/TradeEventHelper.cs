using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lurker.Helpers
{
    public static class TradeEventHelper
    {
        private static List<string> _regexes = new List<string>
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

        public static bool IsTradeMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;

            return _regexes.Any(pattern => Regex.Match(message, pattern).Success);
        }
    }
}
