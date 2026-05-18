//-----------------------------------------------------------------------
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
using System.Text.Json;
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

    private List<Gem> _knownGems = [];
    private List<UniqueItem> _knownUniques = [];

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
        //if (_knownGems == null)
        //{
        //    throw new InvalidOperationException("Must be initialized");
        //}

        var build = new Build()
        {
            Value = buildValue,
        };

        var t = ConvertToOfficialBuild(buildValue);
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
                var skill = Skill.FromXml(element, _knownGems ?? []);
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
    /// Saves a Path of Building XML as an official PoE2 .build file to the Build Planner directory.
    /// </summary>
    /// <param name="pobXmlOrCode">The PoB XML content or encoded build code.</param>
    /// <param name="buildName">The name for the build.</param>
    /// <returns>The path to the saved .build file, or null if conversion failed.</returns>
    public static void SaveAsOfficialBuild(Build build)
    {
        if (!PoeApplicationContext.Poe2)
        {
            return;
        }

        var json = ConvertToOfficialBuild(build.Value, build.Name);
        if (json == null)
        {
            return;
        }

        var buildPlannerFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "My Games",
            "Path of Exile 2",
            "BuildPlanner");

        Directory.CreateDirectory(buildPlannerFolder);

        var sanitizedName = string.Join("_", build.Name.Split(Path.GetInvalidFileNameChars()));
        var filePath = Path.Combine(buildPlannerFolder, $"{sanitizedName}.build");
        File.WriteAllText(filePath, json);
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
            var compressed = Base64UrlDecode(build);

            using var input = new MemoryStream(compressed);
            using var zlib = new ZLibStream(input, CompressionMode.Decompress);
            using var output = new MemoryStream();
            zlib.CopyTo(output);

            return Encoding.UTF8.GetString(output.ToArray());
        }
        catch
        {
            return string.Empty;
        }
    }

    private static byte[] Base64UrlDecode(string s)
    {
        s = s.Trim();

        // handle URL-safe base64 (sometimes present)
        s = s.Replace('-', '+').Replace('_', '/');

        // padding
        switch (s.Length % 4)
        {
            case 2: s += "=="; break;
            case 3: s += "="; break;
        }

        return Convert.FromBase64String(s);
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

    /// <summary>
    /// Converts a Path of Building XML string to the official PoE2 Build Planner JSON format.
    /// </summary>
    /// <param name="pobXmlOrCode">The PoB XML content or encoded build code.</param>
    /// <param name="buildName">The name for the build.</param>
    /// <returns>The JSON string in the official .build format.</returns>
    private static string ConvertToOfficialBuild(string pobXmlOrCode, string buildName = "Imported Build")
    {
        var xml = GetXml(pobXmlOrCode);
        if (string.IsNullOrEmpty(xml))
        {
            return null;
        }

        var document = XDocument.Parse(xml);
        var officialBuild = new OfficialBuild
        {
            Name = buildName,
        };

        // Ascendancy
        var buildElement = document.Root.Element("Build");
        if (buildElement != null)
        {
            var ascendancy = buildElement.Attribute("ascendClassName")?.Value;
            if (!string.IsNullOrEmpty(ascendancy) && ascendancy != "None")
            {
                officialBuild.Ascendancy = MapAscendancy(buildElement.Attribute("className")?.Value, ascendancy);
            }

            // Description from notes
            var notesElement = document.Root.Element("Notes");
            if (notesElement != null)
            {
                var notes = notesElement.Value.Trim();
                if (!string.IsNullOrEmpty(notes))
                {
                    officialBuild.Description = notes;
                }
            }
        }

        // Passives
        var treeElement = document.Root.Element("Tree");
        if (treeElement != null)
        {
            var activeSpec = treeElement.Attribute("activeSpec")?.Value;
            var specs = treeElement.Elements("Spec").ToList();
            var spec = activeSpec != null && int.TryParse(activeSpec, out var specIndex) && specIndex > 0 && specIndex <= specs.Count
                ? specs[specIndex - 1]
                : specs.FirstOrDefault();

            if (spec != null)
            {
                officialBuild.Passives = ConvertPassives(spec);
            }
        }

        // Skills
        var skillsElement = document.Root.Element("Skills");
        if (skillsElement != null)
        {
            officialBuild.Skills = ConvertSkills(skillsElement);
        }

        // Items
        var itemsElement = document.Root.Element("Items");
        if (itemsElement != null)
        {
            officialBuild.Items = ConvertItems(itemsElement);
        }

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
        };

        return JsonSerializer.Serialize(officialBuild, options);
    }

    private static List<OfficialBuildPassive> ConvertPassives(XElement spec)
    {
        var passives = new List<OfficialBuildPassive>();
        var nodesAttr = spec.Attribute("nodes");
        if (nodesAttr == null)
        {
            return passives;
        }

        var allNodes = nodesAttr.Value.Split(',', StringSplitOptions.RemoveEmptyEntries);

        // Weapon set specific nodes
        var weaponSet1Nodes = new HashSet<string>(
            spec.Element("WeaponSet1")?.Attribute("nodes")?.Value.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? []);
        var weaponSet2Nodes = new HashSet<string>(
            spec.Element("WeaponSet2")?.Attribute("nodes")?.Value.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? []);

        foreach (var nodeId in allNodes)
        {
            var trimmed = nodeId.Trim();
            if (string.IsNullOrEmpty(trimmed))
            {
                continue;
            }

            var passive = new OfficialBuildPassive { Id = trimmed };

            if (weaponSet1Nodes.Contains(trimmed))
            {
                passive.WeaponSet = 1;
            }
            else if (weaponSet2Nodes.Contains(trimmed))
            {
                passive.WeaponSet = 2;
            }

            passives.Add(passive);
        }

        return passives;
    }

    private static List<OfficialBuildSkill> ConvertSkills(XElement skillsElement)
    {
        var skills = new List<OfficialBuildSkill>();

        var activeSkillSet = skillsElement.Attribute("activeSkillSet")?.Value;
        var skillSets = skillsElement.Elements("SkillSet").ToList();
        var skillSet = activeSkillSet != null && int.TryParse(activeSkillSet, out var setIndex) && setIndex > 0 && setIndex <= skillSets.Count
            ? skillSets[setIndex - 1]
            : skillSets.FirstOrDefault();

        var skillElements = skillSet?.Elements("Skill") ?? skillsElement.Elements("Skill");

        foreach (var skillElement in skillElements)
        {
            if (skillElement.Attribute("enabled")?.Value == "false")
            {
                continue;
            }

            var gems = skillElement.Elements("Gem").ToList();
            if (gems.Count == 0)
            {
                continue;
            }

            // Find the active skill gem (first non-support gem)
            var activeGem = gems.FirstOrDefault(g =>
            {
                var skillId = g.Attribute("skillId")?.Value;
                return skillId != null && !skillId.Contains("Support");
            });

            if (activeGem == null)
            {
                continue;
            }

            var gemId = activeGem.Attribute("gemId")?.Value;
            if (string.IsNullOrEmpty(gemId))
            {
                continue;
            }

            var officialSkill = new OfficialBuildSkill
            {
                Id = gemId,
            };

            // Support gems
            var supports = new List<OfficialBuildSupport>();
            foreach (var gem in gems)
            {
                if (gem == activeGem)
                {
                    continue;
                }

                var supportGemId = gem.Attribute("gemId")?.Value;
                if (string.IsNullOrEmpty(supportGemId))
                {
                    continue;
                }

                supports.Add(new OfficialBuildSupport
                {
                    Id = supportGemId,
                });
            }

            if (supports.Count > 0)
            {
                officialSkill.SupportSkills = supports;
            }

            skills.Add(officialSkill);
        }

        return skills;
    }

    private static List<OfficialBuildItem> ConvertItems(XElement itemsElement)
    {
        var items = new List<OfficialBuildItem>();

        // Map PoB slot names to official inventory IDs
        var slotMapping = new Dictionary<string, string>
        {
            ["Weapon 1"] = "Weapon1",
            ["Weapon 2"] = "Weapon2",
            ["Weapon 1 Swap"] = "Weapon1Swap",
            ["Weapon 2 Swap"] = "Weapon2Swap",
            ["Helmet"] = "Helm",
            ["Body Armour"] = "BodyArmour",
            ["Gloves"] = "Gloves",
            ["Boots"] = "Boots",
            ["Amulet"] = "Amulet",
            ["Ring 1"] = "Ring",
            ["Ring 2"] = "Ring2",
            ["Belt"] = "Belt",
            ["Flask 1"] = "Flask",
            ["Flask 2"] = "Flask2",
        };

        // Build item lookup by id
        var itemLookup = new Dictionary<string, XElement>();
        foreach (var itemElement in itemsElement.Elements("Item"))
        {
            var id = itemElement.Attribute("id")?.Value;
            if (id != null)
            {
                itemLookup[id] = itemElement;
            }
        }

        // Find the active item set
        var activeItemSet = itemsElement.Attribute("activeItemSet")?.Value;
        var itemSets = itemsElement.Elements("ItemSet").ToList();
        var itemSet = activeItemSet != null && int.TryParse(activeItemSet, out var setIdx) && setIdx > 0 && setIdx <= itemSets.Count
            ? itemSets[setIdx - 1]
            : itemSets.FirstOrDefault();

        if (itemSet == null)
        {
            return items;
        }

        foreach (var slot in itemSet.Elements("Slot"))
        {
            var slotName = slot.Attribute("name")?.Value;
            var itemId = slot.Attribute("itemId")?.Value;

            if (slotName == null || itemId == null || itemId == "0")
            {
                continue;
            }

            if (!slotMapping.TryGetValue(slotName, out var inventoryId))
            {
                continue;
            }

            // Only include unique items
            if (!itemLookup.TryGetValue(itemId, out var itemData))
            {
                continue;
            }

            var lines = itemData.Value.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var rarityLine = lines.FirstOrDefault(l => l.Trim().StartsWith("Rarity:"));
            if (rarityLine == null || !rarityLine.Contains("UNIQUE"))
            {
                continue;
            }

            var rarityIndex = Array.IndexOf(lines, rarityLine);
            if (rarityIndex < 0 || rarityIndex + 1 >= lines.Length)
            {
                continue;
            }

            var uniqueName = lines[rarityIndex + 1].Trim();

            items.Add(new OfficialBuildItem
            {
                InventoryId = inventoryId,
                SlotX = 0,
                SlotY = 0,
                UniqueName = uniqueName,
            });
        }

        return items;
    }

    private static string MapAscendancy(string className, string ascendancy)
    {
        // The official format uses internal IDs like "Ranger3" for Pathfinder
        // PoB stores ascendancyInternalId in the Spec element, fall back to className + ascendancy
        var mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Deadeye"] = "Ranger1",
            ["Warden"] = "Ranger2",
            ["Pathfinder"] = "Ranger3",
            ["Gladiator"] = "Warrior1",
            ["Titan"] = "Warrior2",
            ["Warbringer"] = "Warrior3",
            ["Chronomancer"] = "Sorceress1",
            ["Stormweaver"] = "Sorceress2",
            ["Infernalist"] = "Witch1",
            ["Blood Mage"] = "Witch2",
            ["Invoker"] = "Monk1",
            ["Acolyte of Chayula"] = "Monk2",
            ["Witchhunter"] = "Mercenary1",
            ["Gemling Legionnaire"] = "Mercenary2",
        };

        if (mapping.TryGetValue(ascendancy, out var mapped))
        {
            return mapped;
        }

        // Fallback: return as-is
        return ascendancy;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether this instance is initialize.
    /// </summary>
    public bool IsInitialize { get; private set; }

    #endregion
}