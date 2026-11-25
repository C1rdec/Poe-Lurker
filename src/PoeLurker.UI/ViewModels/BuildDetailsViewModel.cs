//-----------------------------------------------------------------------
// <copyright file="BuildDetailsViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Linq;
using PoeLurker.Core.Models;

/// <summary>
/// Represents build details.
/// </summary>
public class BuildDetailsViewModel : Caliburn.Micro.PropertyChangedBase
{
    private static readonly string[] KnownAscendancies =
    [
        "Assassin", "Ascendant", "Berserker", "Champion", "Chieftain", "Deadeye", "Elementalist",
        "Gladiator", "Guardian", "Hierophant", "Inquisitor", "Juggernaut", "Necromancer", "Occultist",
        "Pathfinder", "Raider", "Saboteur", "Slayer", "Trickster",
    ];

    private readonly Action _showBuildSelector;
    private readonly Action<bool, bool, bool> _updatePlannerBuild;
    private bool _includeInventorySlots;
    private bool _includePassives;
    private bool _includeSkills;

    public BuildDetailsViewModel(
        Build build,
        Action showBuildSelector,
        Action<bool, bool, bool> updatePlannerBuild,
        bool includeInventorySlots,
        bool includePassives,
        bool includeSkills)
    {
        Build = build;
        _showBuildSelector = showBuildSelector;
        _updatePlannerBuild = updatePlannerBuild;
        _includeInventorySlots = includeInventorySlots;
        _includePassives = includePassives;
        _includeSkills = includeSkills;
    }

    public Build Build { get; }

    public string BuildName => Build?.Name;

    public bool HasBuildName => !string.IsNullOrEmpty(BuildName);

    public bool HasNoBuildName => !HasBuildName;

    public string DisplayName => $"{ClassName} ({Ascendancy})";

    public string ClassName => Build?.Class ?? "Unknown";

    public string Ascendancy => Build?.Ascendancy ?? "Unknown";

    public bool HasUnknownAscendancy => !KnownAscendancies.Contains(Ascendancy);

    public bool IncludeInventorySlots
    {
        get
        {
            return _includeInventorySlots;
        }

        set
        {
            if (_includeInventorySlots == value)
            {
                return;
            }

            _includeInventorySlots = value;
            NotifyOfPropertyChange();
            _updatePlannerBuild?.Invoke(IncludeInventorySlots, IncludePassives, IncludeSkills);
        }
    }

    public bool IncludePassives
    {
        get
        {
            return _includePassives;
        }

        set
        {
            if (_includePassives == value)
            {
                return;
            }

            _includePassives = value;
            NotifyOfPropertyChange();
            _updatePlannerBuild?.Invoke(IncludeInventorySlots, IncludePassives, IncludeSkills);
        }
    }

    public bool IncludeSkills
    {
        get
        {
            return _includeSkills;
        }

        set
        {
            if (_includeSkills == value)
            {
                return;
            }

            _includeSkills = value;
            NotifyOfPropertyChange();
            _updatePlannerBuild?.Invoke(IncludeInventorySlots, IncludePassives, IncludeSkills);
        }
    }

    public void BackToBuildSelector()
    {
        _showBuildSelector?.Invoke();
    }
}
