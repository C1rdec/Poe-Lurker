//-----------------------------------------------------------------------
// <copyright file="OfficialBuild.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Models;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>
/// Represents the official PoE2 Build Planner format.
/// </summary>
public class OfficialBuild
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Description { get; set; }

    [JsonPropertyName("ascendancy")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Ascendancy { get; set; }

    [JsonPropertyName("passives")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<OfficialBuildPassive> Passives { get; set; }

    [JsonPropertyName("skills")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<OfficialBuildSkill> Skills { get; set; }

    [JsonPropertyName("items")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<OfficialBuildItem> Items { get; set; }
}

/// <summary>
/// Represents a passive node in the official build format.
/// </summary>
public class OfficialBuildPassive
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("level_interval")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<uint> LevelInterval { get; set; }

    [JsonPropertyName("weapon_set")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public uint? WeaponSet { get; set; }

    [JsonPropertyName("additional_text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string AdditionalText { get; set; }
}

/// <summary>
/// Represents a skill in the official build format.
/// </summary>
public class OfficialBuildSkill
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("level_interval")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<uint> LevelInterval { get; set; }

    [JsonPropertyName("additional_text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string AdditionalText { get; set; }

    [JsonPropertyName("support_skills")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<OfficialBuildSupport> SupportSkills { get; set; }
}

/// <summary>
/// Represents a support skill in the official build format.
/// </summary>
public class OfficialBuildSupport
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("level_interval")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<uint> LevelInterval { get; set; }

    [JsonPropertyName("additional_text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string AdditionalText { get; set; }
}

/// <summary>
/// Represents an item hint in the official build format.
/// </summary>
public class OfficialBuildItem
{
    [JsonPropertyName("inventory_id")]
    public string InventoryId { get; set; }

    [JsonPropertyName("slot_x")]
    public uint SlotX { get; set; }

    [JsonPropertyName("slot_y")]
    public uint SlotY { get; set; }

    [JsonPropertyName("level_interval")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<uint> LevelInterval { get; set; }

    [JsonPropertyName("unique_name")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string UniqueName { get; set; }

    [JsonPropertyName("additional_text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string AdditionalText { get; set; }
}
