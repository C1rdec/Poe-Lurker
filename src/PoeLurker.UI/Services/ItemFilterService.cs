//-----------------------------------------------------------------------
// <copyright file="ItemFilterService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Services;

/// <summary>
/// Represents item filter settings and actions.
/// </summary>
public class ItemFilterService
{
    private const string OutputFilterFileName = "Poe-Lurker.filter";
    private static readonly TimeSpan BaseFilterCacheDuration = TimeSpan.FromMinutes(5);
    private static readonly HttpClient HttpClient = new();

    private readonly PlayerService _playerService;
    private readonly object _cacheLock = new();
    private readonly Dictionary<string, (DateTimeOffset Timestamp, string Content)> _downloadedFilterCache = new();
    private readonly Dictionary<string, string> _baseFilterUrls = new()
    {
        ["0 - Soft"] = "https://raw.githubusercontent.com/NeverSinkDev/NeverSink-Filter-for-PoE2/main/NeverSink%27s%20filter%202%20-%200-SOFT.filter",
        ["1 - Regular"] = "https://raw.githubusercontent.com/NeverSinkDev/NeverSink-Filter-for-PoE2/main/NeverSink%27s%20filter%202%20-%201-REGULAR.filter",
        ["2 - Semi-Strict"] = "https://raw.githubusercontent.com/NeverSinkDev/NeverSink-Filter-for-PoE2/main/NeverSink%27s%20filter%202%20-%202-SEMI-STRICT.filter",
        ["3 - Strict"] = "https://raw.githubusercontent.com/NeverSinkDev/NeverSink-Filter-for-PoE2/main/NeverSink%27s%20filter%202%20-%203-STRICT.filter",
        ["4 - Very Strict"] = "https://raw.githubusercontent.com/NeverSinkDev/NeverSink-Filter-for-PoE2/main/NeverSink%27s%20filter%202%20-%204-VERY-STRICT.filter",
        ["5 - Uber Strict"] = "https://raw.githubusercontent.com/NeverSinkDev/NeverSink-Filter-for-PoE2/main/NeverSink%27s%20filter%202%20-%205-UBER-STRICT.filter",
        ["6 - Uber Plus Strict"] = "https://raw.githubusercontent.com/NeverSinkDev/NeverSink-Filter-for-PoE2/main/NeverSink%27s%20filter%202%20-%206-UBER-PLUS-STRICT.filter",
    };

    public string[] BaseFilters { get; } =
    [
        "0 - Soft",
        "1 - Regular",
        "2 - Semi-Strict",
        "3 - Strict",
        "4 - Very Strict",
        "5 - Uber Strict",
        "6 - Uber Plus Strict",
    ];

    public ItemFilterService(PlayerService playerService)
    {
        _playerService = playerService;
    }

    public bool ItemFilterEnabled { get; private set; } = true;

    public string SelectedBaseFilter
    {
        get
        {
            var player = _playerService.FirstPlayer;
            if (player == null || string.IsNullOrWhiteSpace(player.SelectedBaseFilter))
            {
                return BaseFilters[0];
            }

            return player.SelectedBaseFilter;
        }

        set
        {
            var player = _playerService.FirstPlayer;
            if (player == null)
            {
                return;
            }

            player.SelectedBaseFilter = value;
            _playerService.Save();
        }
    }

    public string AdditionalFilter
    {
        get
        {
            var player = _playerService.FirstPlayer;
            return player?.AdditionalFilter;
        }

        set
        {
            var player = _playerService.FirstPlayer;
            if (player == null)
            {
                return;
            }

            player.AdditionalFilter = value;
            _playerService.Save();
        }
    }

    public string SelectedBaseFilterUrl => GetBaseFilterUrl(SelectedBaseFilter);

    public void UpdateItemFilterEnabled(bool isEnabled)
    {
        ItemFilterEnabled = isEnabled;
    }

    public async Task SaveFilter(string baseFilter, string additionalFilter, bool applyInGame = true)
    {
        var player = _playerService.FirstPlayer;
        if (player == null)
        {
            return;
        }

        player.SelectedBaseFilter = baseFilter;
        player.AdditionalFilter = additionalFilter;
        _playerService.Save();

        var baseFilterUrl = GetBaseFilterUrl(baseFilter);
        if (string.IsNullOrWhiteSpace(baseFilterUrl))
        {
            return;
        }

        var downloadedBaseFilter = await GetBaseFilterContentAsync(baseFilterUrl);
        if (string.IsNullOrWhiteSpace(downloadedBaseFilter))
        {
            return;
        }

        var finalFilter = BuildFinalFilter(additionalFilter, downloadedBaseFilter);
        var outputDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "My Games",
            "Path of Exile 2");

        Directory.CreateDirectory(outputDirectory);
        var outputPath = Path.Combine(outputDirectory, OutputFilterFileName);
        await File.WriteAllTextAsync(outputPath, finalFilter, Encoding.UTF8);

        if (!applyInGame)
        {
            return;
        }

        try
        {
            var keyboardHelper = IoC.Get<PoeKeyboardHelper>();
            var filterName = Path.GetFileNameWithoutExtension(OutputFilterFileName);
            await keyboardHelper.ItemFilter(filterName);
        }
        catch
        {
            // Ignore when POE keyboard helper is not available.
        }
    }

    public string GetBaseFilterUrl(string baseFilter)
    {
        return _baseFilterUrls.TryGetValue(baseFilter, out var url)
            ? url
            : string.Empty;
    }

    private async Task<string> GetBaseFilterContentAsync(string baseFilterUrl)
    {
        lock (_cacheLock)
        {
            if (_downloadedFilterCache.TryGetValue(baseFilterUrl, out var cacheEntry)
                && DateTimeOffset.UtcNow - cacheEntry.Timestamp <= BaseFilterCacheDuration)
            {
                return cacheEntry.Content;
            }
        }

        var downloadedContent = await HttpClient.GetStringAsync(baseFilterUrl);

        lock (_cacheLock)
        {
            _downloadedFilterCache[baseFilterUrl] = (DateTimeOffset.UtcNow, downloadedContent);
        }

        return downloadedContent;
    }

    private static string BuildFinalFilter(string additionalFilter, string downloadedBaseFilter)
    {
        if (string.IsNullOrWhiteSpace(additionalFilter))
        {
            return downloadedBaseFilter;
        }

        return $"{additionalFilter.TrimEnd()}\n\n{downloadedBaseFilter}";
    }
}
