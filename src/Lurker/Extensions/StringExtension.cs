//-----------------------------------------------------------------------
// <copyright file="StringExtension.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------


namespace Lurker.Extensions
{
    using System.Linq;
    using System.Text.RegularExpressions;

    public static class StringExtension
    {
        public static string[] Split(this string value, string splitValue)
        {
            return value.Split(new string[] { splitValue }, System.StringSplitOptions.RemoveEmptyEntries);
        }

        public static string[] GetLines(this string value)
        {
            return value.Split(System.Environment.NewLine);
        }

        public static string GetLine(this string value, string marker)
        {
            var index = value.IndexOf(marker);
            if (index == -1)
            {
                return null;
            }

            var textAfter = value.Substring(index + marker.Length);
            return textAfter.Split(System.Environment.NewLine).First().Trim();
        }

        public static string ReplaceDigit(this string value, string replaceValue)
        {
            return Regex.Replace(value, @"[\d-]+", replaceValue).Trim();
        }
    }
}
