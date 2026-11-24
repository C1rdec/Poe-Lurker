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
    #region Methods

    public static List<Build> Get()
    {
        var builds = new List<Build>();
        using var service = new PathOfBuildingService();
        var folderName = "Path of Building";

        if (PoeApplicationContext.Poe2)
        {
            folderName += " (PoE2)";
        }

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

    public static Build Get(string buildPath)
    {
        using var service = new PathOfBuildingService();

        if (!File.Exists(buildPath))
        {
            return null;
        }

        var build = service.Decode(File.ReadAllText(buildPath));
        build.Name = Path.GetFileNameWithoutExtension(buildPath);

        return build;
    }

    #endregion
}