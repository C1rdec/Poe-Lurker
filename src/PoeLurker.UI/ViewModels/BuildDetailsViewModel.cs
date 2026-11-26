//-----------------------------------------------------------------------
// <copyright file="BuildDetailsViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Linq;
using System.Threading.Tasks;
using PoeLurker.Core.Models;
using PoeLurker.UI.Controls;
using PoeLurker.UI.Services;

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
                "Titan", "Warbringer", "Smith of Kitava", "Warden", "Amazon", "Ritualist", "Stormweaver", "Chronomancer",
                "Disciple of Varashta", "Infernalist", "Blood Mage", "Lich", "Abyssal Lich", "Invoker", "Acolyte of Chayula",
                "Witchhunter", "Gemling Legionnaire", "Tactician", "Oracle", "Shaman",
    ];

    private readonly Action _showBuildSelector;
    private readonly Action<bool, bool, bool> _updatePlannerBuild;
    private readonly ItemFilterService _itemFilterService;
    private bool _includeInventorySlots;
    private bool _includePassives;
    private bool _includeSkills;
    private bool _additionalFilterEditorOpen;
    private string _additionalFilterDraft;
    private string _lastAppliedBaseFilter;
    private string _lastAppliedAdditionalFilter;

    public BuildDetailsViewModel(
        Build build,
        Action showBuildSelector,
        Action<bool, bool, bool> updatePlannerBuild,
        ItemFilterService itemFilterService,
        bool includeInventorySlots,
        bool includePassives,
        bool includeSkills)
    {
        Build = build;
        _showBuildSelector = showBuildSelector;
        _updatePlannerBuild = updatePlannerBuild;
        _itemFilterService = itemFilterService;
        _includeInventorySlots = includeInventorySlots;
        _includePassives = includePassives;
        _includeSkills = includeSkills;
        _lastAppliedBaseFilter = _itemFilterService.SelectedBaseFilter;
        _lastAppliedAdditionalFilter = _itemFilterService.AdditionalFilter;
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

    public string[] BaseFilters => _itemFilterService.BaseFilters;

    public bool ItemFilterEnabled
    {
        get
        {
            return _itemFilterService.ItemFilterEnabled;
        }

        set
        {
            if (_itemFilterService.ItemFilterEnabled == value)
            {
                return;
            }

            _itemFilterService.UpdateItemFilterEnabled(value);
            NotifyOfPropertyChange();
        }
    }

    public string SelectedBaseFilter
    {
        get
        {
            return _itemFilterService.SelectedBaseFilter;
        }

        set
        {
            if (_itemFilterService.SelectedBaseFilter == value)
            {
                return;
            }

            _itemFilterService.SelectedBaseFilter = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(nameof(CanSaveFilter));
        }
    }

    public string AdditionalFilter
    {
        get
        {
            return _itemFilterService.AdditionalFilter;
        }

        set
        {
            if (_itemFilterService.AdditionalFilter == value)
            {
                return;
            }

            _itemFilterService.AdditionalFilter = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(nameof(HasAdditionalFilter));
            NotifyOfPropertyChange(nameof(CanSaveFilter));
        }
    }

    public bool HasAdditionalFilter => !string.IsNullOrWhiteSpace(AdditionalFilter);

    public bool AdditionalFilterEditorOpen
    {
        get
        {
            return _additionalFilterEditorOpen;
        }

        set
        {
            if (_additionalFilterEditorOpen == value)
            {
                return;
            }

            _additionalFilterEditorOpen = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(nameof(MainFilterViewVisible));
        }
    }

    public ICompletionProvider CompletionProvider { get; } = new ItemFilterCompletionProvider();

    public bool MainFilterViewVisible => !AdditionalFilterEditorOpen;

    public string AdditionalFilterDraft
    {
        get
        {
            return _additionalFilterDraft;
        }

        set
        {
            if (_additionalFilterDraft == value)
            {
                return;
            }

            _additionalFilterDraft = value;
            NotifyOfPropertyChange();
        }
    }

    public bool CanSaveFilter => !string.Equals(SelectedBaseFilter, _lastAppliedBaseFilter, StringComparison.Ordinal)
        || !string.Equals(AdditionalFilter ?? string.Empty, _lastAppliedAdditionalFilter ?? string.Empty, StringComparison.Ordinal);

    public async Task SaveFilter()
    {
        await _itemFilterService.SaveFilter(SelectedBaseFilter, AdditionalFilter);
        _lastAppliedBaseFilter = SelectedBaseFilter;
        _lastAppliedAdditionalFilter = AdditionalFilter;
        NotifyOfPropertyChange(nameof(CanSaveFilter));
    }

    public void OpenAdditionalFilterEditor()
    {
        AdditionalFilterDraft = AdditionalFilter;
        AdditionalFilterEditorOpen = true;
    }

    public void AddAdditionalFilter()
    {
        AdditionalFilter = AdditionalFilterDraft;
        AdditionalFilterEditorOpen = false;
    }

    public void BackToBuildSelector()
    {
        _showBuildSelector?.Invoke();
    }
}
