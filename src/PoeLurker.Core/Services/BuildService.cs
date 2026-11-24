//-----------------------------------------------------------------------
// <copyright file="BuildService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PoeLurker.Core.Models;

/// <summary>
/// Represents the build service.
/// </summary>
/// <seealso cref="PoeLurker.Core.Services.ServiceBase" />
public class BuildService
{
    #region Fields

    private readonly PathOfBuildingService _pathOfBuildingService;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildService"/> class.
    /// </summary>
    /// <param name="pathOfBuildingService">The path of building service.</param>
    public BuildService(PathOfBuildingService pathOfBuildingService)
    {
        _pathOfBuildingService = pathOfBuildingService;
    }

    #endregion

    #region Methods

    public const int DefaultRecentCount = 100;

    public Task<List<Build>> GetAsync(int skip = 0, int count = DefaultRecentCount)
    {
        return Task.Run(() => Get(skip, count));
    }

    public List<Build> Get(int skip = 0, int count = DefaultRecentCount)
    {
        var builds = new List<Build>();
        var service = _pathOfBuildingService;
        var folderName = "Path of Building (PoE2)";
        var pathOfBuildingFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), folderName, "Builds");

        if (!Directory.Exists(pathOfBuildingFolder))
        {
            return builds;
        }

        var files = new DirectoryInfo(pathOfBuildingFolder)
            .EnumerateFiles("*.xml", SearchOption.AllDirectories)
            .Where(f => !f.Name.StartsWith("."))
            .OrderByDescending(f => f.LastWriteTimeUtc)
            .Skip(skip)
            .Take(count)
            .ToList();

        var results = new Build[files.Count];
        Parallel.For(0, files.Count, i =>
        {
            var file = files[i];
            var build = service.Decode(File.ReadAllText(file.FullName));
            build.Name = file.Name.Replace(".xml", string.Empty);
            build.FilePath = file.FullName;
            results[i] = build;
        });

        builds.AddRange(results);

        return builds;
    }

    public Build Get(string buildPath)
    {
        var service = _pathOfBuildingService;

        if (!File.Exists(buildPath))
        {
            return null;
        }

        var build = service.Decode(File.ReadAllText(buildPath));
        build.Name = Path.GetFileNameWithoutExtension(buildPath);

        return build;
    }

    public void AddToPlanner(Build build)
    {
        var service = _pathOfBuildingService;
        if (!File.Exists(build.FilePath))
        {
            return;
        }

        service.AddToPlanner(build);
    }

    #endregion
}