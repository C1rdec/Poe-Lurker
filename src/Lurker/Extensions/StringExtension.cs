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
        /// <summary>
        /// Splits the specified split value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="splitValue">The split value.</param>
        /// <returns>Split the current string using another string</returns>
        public static string[] Split(this string value, string splitValue)
        {
            return value.Split(new string[] { splitValue }, System.StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Gets the lines.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The lines.</returns>
        public static string[] GetLines(this string value)
        {
            return value.Split(System.Environment.NewLine);
        }

        /// <summary>
        /// Gets the line after.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="marker">The marker.</param>
        /// <returns>The line.</returns>
        public static string GetLineAfter(this string value, string marker)
        {
            var index = value.IndexOf(marker);
            if (index == -1)
            {
                return null;
            }

            var textAfter = value.Substring(index + marker.Length);
            return textAfter.Split(System.Environment.NewLine).First().Trim();
        }

        /// <summary>
        /// Gets the line before.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="marker">The marker.</param>
        /// <returns>The line</returns>
        public static string GetLineBefore(this string value, string marker)
        {
            var index = value.IndexOf(marker);
            if (index == -1)
            {
                return null;
            }

            var textBefore = value.Substring(0, index);
            return textBefore.Split(System.Environment.NewLine).Last().Trim();
        }

        /// <summary>
        /// Gets the line.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="marker">The marker.</param>
        /// <returns>The full line of the marker</returns>
        public static string GetLine(this string value, string marker)
        {
            var index = value.IndexOf(marker);
            if (index == -1)
            {
                return null;
            }

            var textBefore = value.Substring(0, index + marker.Length);
            return textBefore.Split(System.Environment.NewLine).Last().Trim();
        }
    }
}
