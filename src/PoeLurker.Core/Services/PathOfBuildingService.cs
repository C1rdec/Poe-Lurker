﻿//-----------------------------------------------------------------------
// <copyright file="PathOfBuildingService.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PoeLurker.Core.Extensions;
using PoeLurker.Core.Models;

/// <summary>
/// Represents the service for Path ofBuilding.
/// </summary>
public class PathOfBuildingService : HttpServiceBase
{
    #region Fields

    private IEnumerable<Gem> _knownGems;
    private IEnumerable<UniqueItem> _knownUniques;

    #endregion

    #region Methods

    /// <summary>
    /// Validate the build.
    /// </summary>
    /// <param name="value">the value.</param>
    /// <returns>If the build is valid.</returns>
    public static bool IsValid(string value)
    {
        var xml = GetXml(value);
        if (string.IsNullOrEmpty(xml))
        {
            return false;
        }

        var document = XDocument.Parse(xml);

        var buildElement = document.Root.Element("Build");
        if (buildElement == null)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Initializes the asynchronous.
    /// </summary>
    /// <param name="service">The service.</param>
    /// <returns>
    /// The task awaiter.
    /// </returns>
    public async Task InitializeAsync(GithubService service)
    {
        _knownGems = await service.Gems();
        _knownUniques = await service.Uniques();
        IsInitialize = true;
    }

    /// <summary>
    /// Decodes the specified build.
    /// </summary>
    /// <param name="buildValue">The build value.</param>
    /// <returns>
    /// The xml structure.
    /// </returns>
    public Build Decode(string buildValue)
    {
        if (_knownGems == null)
        {
            throw new InvalidOperationException("Must be initialized");
        }

        var build = new Build()
        {
            Value = buildValue,
        };

        var xml = GetXml(buildValue);
        if (string.IsNullOrEmpty(xml))
        {
            return build;
        }

        build.Xml = xml;
        var document = XDocument.Parse(build.Xml);
        var buildElement = document.Root.Element("Build");
        if (buildElement != null)
        {
            double totalDps = 0;
            double totalDotDps = 0;

            var totalDpsElement = buildElement.Elements("PlayerStat").FirstOrDefault(e => e.Attribute("stat").Value == "TotalDPS");
            if (totalDpsElement != null)
            {
                totalDps = GetDoubleValue(totalDpsElement.Attribute("value"));
            }

            var totalDotDpsElement = buildElement.Elements("PlayerStat").FirstOrDefault(e => e.Attribute("stat").Value == "TotalDotDPS");
            if (totalDotDpsElement != null)
            {
                totalDotDps = GetDoubleValue(totalDotDpsElement.Attribute("value"));
            }

            build.Damage = new DamageValue()
            {
                IsDot = totalDotDps > totalDps,
                Value = totalDotDps > totalDps ? totalDotDps : totalDps,
            };

            var classAttribute = buildElement.Attribute("className");
            if (classAttribute != null)
            {
                build.Class = classAttribute.Value;
            }

            var ascendancyAttribute = buildElement.Attribute("ascendClassName");
            if (ascendancyAttribute != null)
            {
                build.Ascendancy = ascendancyAttribute.Value;
            }
        }

        var notesElement = document.Root.Element("Notes");
        if (notesElement != null)
        {
            build.Notes = notesElement.Value.Trim();
        }

        var skillsElement = document.Root.Element("Skills");
        if (skillsElement != null)
        {
            foreach (var element in skillsElement.Descendants("Skill"))
            {
                var skill = Skill.FromXml(element, _knownGems);
                if (skill.Gems.Any())
                {
                    build.AddSkill(skill);
                }
            }
        }

        var treeElement = document.Root.Element("Tree");
        if (treeElement != null)
        {
            foreach (var element in treeElement.Elements("Spec"))
            {
                var urlElement = element.Element("URL");
                var information = new SkillTreeInformation
                {
                    Url = urlElement?.Value.Trim().Replace("passive-skill-tree", "fullscreen-passive-skill-tree"),
                    Version = element?.Attribute("treeVersion")?.Value.Replace('_', '.'),
                    Title = element?.Attribute("title")?.Value,
                };

                build.SkillTrees.Add(information);
            }
        }

        var itemsElement = document.Root.Element("Items");
        if (itemsElement != null)
        {
            foreach (var element in itemsElement.Elements())
            {
                var value = element.Value.GetLineAfter("Rarity: ");
                if (value != null)
                {
                    var lines = value.Split('\n');
                    var rarity = lines.FirstOrDefault();
                    if (rarity == "UNIQUE" && lines.Length > 2)
                    {
                        var name = lines[1];
                        var uniqueItem = _knownUniques.FirstOrDefault(u => u.Name == name);
                        if (uniqueItem != null)
                        {
                            build.AddItem(uniqueItem);
                        }
                    }
                }
            }
        }

        return build;
    }

    /// <summary>
    /// Encodes the specified build.
    /// </summary>
    /// <param name="build">The build.</param>
    /// <returns>The Path of Building code.</returns>
    public string Encode(string build)
    {
        using var output = new MemoryStream();
        using var input = new MemoryStream(Encoding.ASCII.GetBytes(build));
        using var decompressor = new GZipStream(output, CompressionMode.Compress);
        input.CopyTo(decompressor);

        return Convert.ToBase64String(output.ToArray());
    }

    private static double GetDoubleValue(XAttribute attribute)
    {
        if (attribute != null)
        {
            var attributeValue = attribute.Value;
            if (double.TryParse(attributeValue, out var result))
            {
                return result;
            }
        }

        return 0;
    }

    /// <summary>
    /// Gets the XML.
    /// </summary>
    /// <param name="build">The build.</param>
    /// <returns>System.String.</returns>
    private static string GetXml(string build)
    {
        if (IsValidXml(build))
        {
            return build;
        }

        try
        {
            using var output = new MemoryStream();
            using var input = new MemoryStream(Convert.FromBase64String(build.Replace("_", "/").Replace("-", "+")));
            using var decompressor = new GZipStream(input, CompressionMode.Decompress);
            decompressor.CopyTo(output);

            return Encoding.UTF8.GetString(output.ToArray());
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Determines whether [is valid XML].
    /// </summary>
    /// <param name="xml">The XML.</param>
    /// <returns>
    ///   <c>true</c> if [is valid XML] [the specified XML]; otherwise, <c>false</c>.
    /// </returns>
    private static bool IsValidXml(string xml)
    {
        try
        {
            XDocument.Parse(xml);
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether this instance is initialize.
    /// </summary>
    public bool IsInitialize { get; private set; }

    #endregion
}