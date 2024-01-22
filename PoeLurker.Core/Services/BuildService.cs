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
using System.Xml.Linq;
using PoeLurker.Core.Models;

/// <summary>
/// Represents the build service.
/// </summary>
/// <seealso cref="PoeLurker.Core.Services.ServiceBase" />
public class BuildService
{
    #region Fields

    private readonly BuildManager _buildManager;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildService"/> class.
    /// </summary>
    public BuildService()
    {
        _buildManager = new BuildManager();
        _buildManager.Initialize();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Synchronizes this instance.
    /// </summary>
    public void Sync()
    {
        var pathOfBuildingFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Path of Building", "Builds");
        if (Directory.Exists(pathOfBuildingFolder))
        {
            foreach (var file in Directory.GetFiles(pathOfBuildingFolder))
            {
                try
                {
                    var fileName = Path.GetFileName(file);
                    if (fileName.StartsWith("."))
                    {
                        continue;
                    }

                    var buildName = fileName.Replace(".xml", string.Empty);
                    var existingBuild = Builds.FirstOrDefault(b => b.Name == buildName);
                    if (existingBuild == null)
                    {
                        var xml = File.ReadAllText(file);
                        var notesElement = XDocument.Parse(xml).Root.Element("Notes");
                        var notes = string.Empty;
                        if (notesElement != null)
                        {
                            notes = notesElement.Value.Trim();
                        }

                        AddBuild(new SimpleBuild() { PathOfBuildingCode = xml, Name = buildName, Notes = notes });
                    }
                    else
                    {
                        existingBuild.PathOfBuildingCode = File.ReadAllText(file);
                    }
                }
                catch
                {
                }
            }

            Save();
        }
    }

    /// <summary>
    /// Saves the specified raise event.
    /// </summary>
    public void Save()
    {
        _buildManager.Save();
    }

    /// <summary>
    /// Adds the build.
    /// </summary>
    /// <param name="build">The build.</param>
    /// <returns>Simple Build.</returns>
    public SimpleBuild AddBuild(Build build)
    {
        var simpleBuild = new SimpleBuild()
        {
            PathOfBuildingCode = build.Xml,
        };

        AddBuild(simpleBuild);

        return simpleBuild;
    }

    /// <summary>
    /// Adds the build.
    /// </summary>
    /// <param name="build">The build.</param>
    public void AddBuild(SimpleBuild build)
    {
        _buildManager.Builds.Add(build);
    }

    /// <summary>
    /// Removes the build.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public void RemoveBuild(string id)
    {
        var build = _buildManager.Builds.FirstOrDefault(b => b.Id == id);
        if (build != null)
        {
            _buildManager.Builds.Remove(build);
        }
    }

    /// <summary>
    /// Gets the builds.
    /// </summary>
    public IEnumerable<SimpleBuild> Builds => _buildManager.Builds;

    #endregion
}