//-----------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Core.Extensions;

using System.Linq;

/// <summary>
/// The string extensions.
/// </summary>
public static class StringExtensions
{
    #region Fiels

    /// <summary>
    /// The wild card.
    /// </summary>
    private static readonly char WildCard = '*';

    #endregion

    /// <summary>
    /// Splits the specified split value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="splitValue">The split value.</param>
    /// <returns>Split the current string using another string.</returns>
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
    /// <returns>The line.</returns>
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
    /// <returns>The full line of the marker.</returns>
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

    /// <summary>
    /// Matches the specified criteria.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <param name="criteria">The criteria.</param>
    /// <returns>If match.</returns>
    public static bool Match(this string value, string criteria)
    {
        if (string.IsNullOrEmpty(criteria))
        {
            return false;
        }

        var results = criteria.Split(WildCard);
        var firstResult = results.First();
        if (!string.IsNullOrEmpty(firstResult))
        {
            if (results.Length == 1)
            {
                return value == firstResult;
            }

            if (!value.StartsWith(firstResult))
            {
                return false;
            }
        }

        var index = value.IndexOf(firstResult);
        value = value.Substring(index + firstResult.Length);

        results = results.Skip(1).ToArray();

        foreach (var result in results)
        {
            var resultIndex = value.IndexOf(result);
            if (resultIndex == -1)
            {
                return false;
            }

            value = value.Substring(resultIndex + result.Length);
        }

        return true;
    }
}