//-----------------------------------------------------------------------
// <copyright file="StringExtension.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------


namespace Lurker.Extensions
{
    using System.Linq;

    public static class StringExtension
    {
        public static string[] Split(this string value, string splitValue)
        {
            return value.Split(new string[] { splitValue }, System.StringSplitOptions.RemoveEmptyEntries);
        }

        public static string GetLine(this string value, int index, string marker)
        {
            var textAfter = value.Substring(index + marker.Length);
            return textAfter.Split(System.Environment.NewLine).First();
        }
    }
}
