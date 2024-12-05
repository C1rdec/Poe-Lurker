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
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PoeLurker.Core.Models;

/// <summary>
/// Represents the Github Service.
/// </summary>
/// <seealso cref="PoeLurker.Core.Services.HttpServiceBase" />
public class GithubService : HttpServiceBase
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
    private Task<List<Gem>> _gemsTask;
    private Task<List<UniqueItem>> _itemsTask;

    /// <summary>
    /// Gets all items.
    /// </summary>
    public IEnumerable<WikiItem> AllItems => _gemsTask.Result.Concat<WikiItem>(_itemsTask.Result);

    /// <summary>
    /// Gemses this instance.
    /// </summary>
    /// <returns>List of Gems.</returns>
    public Task<List<Gem>> Gems()
    {
        if (_gemsTask != null)
        {
            return _gemsTask;
        }
        else
        {
            _gemsTask = GetText($"https://raw.githubusercontent.com/C1rdec/Poe-Lurker/main/assets/Data/GemInfo.json?{Guid.NewGuid()}").ContinueWith(t =>
            {
                return JsonSerializer.Deserialize<List<Gem>>(t.Result, Options);
            });

            return _gemsTask;
        }
    }

    /// <summary>
    /// Uniqueses this instance.
    /// </summary>
    /// <returns>List of uniques.</returns>
    public Task<List<UniqueItem>> Uniques()
    {
        if (_itemsTask != null)
        {
            return _itemsTask;
        }
        else
        {
            _itemsTask = GetText($"https://raw.githubusercontent.com/C1rdec/Poe-Lurker/main/assets/Data/UniqueInfo.json?{Guid.NewGuid()}").ContinueWith(t =>
            {
                return JsonSerializer.Deserialize<List<UniqueItem>>(t.Result, Options);
            });

            return _itemsTask;
        }
    }

    /// <summary>
    /// Searches the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>The list of items.</returns>
    public IEnumerable<WikiItem> Search(string value)
    {
        if (string.IsNullOrEmpty(value) || _itemsTask == null || _gemsTask == null)
        {
            return Enumerable.Empty<WikiItem>();
        }

        value = value.ToLower();
        return AllItems.Where(i => i.Name.ToLower().Contains(value)).Take(10);
    }
}