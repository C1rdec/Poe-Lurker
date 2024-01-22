//-----------------------------------------------------------------------
// <copyright file="GithubService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PoeLurker.Core.Models;

/// <summary>
/// Represents the Github Service.
/// </summary>
/// <seealso cref="PoeLurker.Core.Services.HttpServiceBase" />
public class GithubService : HttpServiceBase
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private IEnumerable<Gem> _gems;
    private IEnumerable<UniqueItem> _items;

    /// <summary>
    /// Gets all items.
    /// </summary>
    public IEnumerable<WikiItem> AllItems => _gems.Concat<WikiItem>(_items);

    /// <summary>
    /// Gemses this instance.
    /// </summary>
    /// <returns>List of Gems.</returns>
    public async Task<IEnumerable<Gem>> Gems()
    {
        try
        {
            if (_gems == null)
            {
                var gemInformation = await GetText($"https://raw.githubusercontent.com/C1rdec/Poe-Lurker/master/assets/Data/GemInfo.json?{Guid.NewGuid()}");
                _gems = JsonSerializer.Deserialize<IEnumerable<Gem>>(gemInformation);
            }

            return _gems;
        }
        catch (Exception exception)
        {
            Logger.Error(exception, exception.Message);

            return Enumerable.Empty<Gem>();
        }
    }

    /// <summary>
    /// Uniqueses this instance.
    /// </summary>
    /// <returns>List of uniques.</returns>
    public async Task<IEnumerable<UniqueItem>> Uniques()
    {
        try
        {
            if (_items == null)
            {
                var uniqueInformation = await GetText($"https://raw.githubusercontent.com/C1rdec/Poe-Lurker/master/assets/Data/UniqueInfo.json?{Guid.NewGuid()}");
                _items = JsonSerializer.Deserialize<List<UniqueItem>>(uniqueInformation);
            }

            return _items;
        }
        catch (Exception exception)
        {
            Logger.Error(exception, exception.Message);

            return Enumerable.Empty<UniqueItem>();
        }
    }

    /// <summary>
    /// Searches the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The list of items.</returns>
    public IEnumerable<WikiItem> Search(string value)
    {
        if (string.IsNullOrEmpty(value) || _items == null || _gems == null)
        {
            return Enumerable.Empty<WikiItem>();
        }

        value = value.ToLower();
        return AllItems.Where(i => i.Name.ToLower().Contains(value)).Take(10);
    }
}