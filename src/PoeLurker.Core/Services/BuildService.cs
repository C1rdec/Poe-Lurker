//-----------------------------------------------------------------------
// <copyright file="BuildService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Services;

using System;
using System.Collections.Generic;
using System.IO;
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

    public List<Build> Get()
    {
        var builds = new List<Build>();
        var service = _pathOfBuildingService;
        var folderName = "Path of Building (PoE2)";


        var pathOfBuildingFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), folderName, "Builds");
        foreach (var file in Directory.GetFiles(pathOfBuildingFolder, "*.xml", SearchOption.AllDirectories))
        {
            var fileName = Path.GetFileName(file);
            if (fileName.StartsWith("."))
            {
                continue;
            }

            var build = service.Decode(File.ReadAllText(file));
            build.Name = fileName.Replace(".xml", string.Empty);
            build.FilePath = file;

            builds.Add(build);
        }

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