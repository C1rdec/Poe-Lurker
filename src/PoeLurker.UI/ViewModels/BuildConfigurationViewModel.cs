//-----------------------------------------------------------------------
// <copyright file="BuildConfigurationViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Caliburn.Micro;
using PoeLurker.Core.Extensions;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;

/// <summary>
/// Class BuildConfigurationViewModel.
/// Implements the <see cref="Caliburn.Micro.PropertyChangedBase" />.
/// </summary>
/// <seealso cref="Caliburn.Micro.PropertyChangedBase" />
public class BuildConfigurationViewModel : PropertyChangedBase
{
    #region Fields

    private static readonly PathOfBuildingService PathOfBuildingService = new ();
    private Build _build;
    private readonly BuildSettings _settings;
    private string _ascendency;
    private bool _isSkillTreeOpen;

    #endregion

    #region Constructors

    public BuildConfigurationViewModel(Build build)
    {
        Ascendancy = build.Ascendancy;
        _build = build;
    }

    #endregion

    #region Properties

    public Build Build => _build;

    /// <summary>
    /// Gets or sets skill trees.
    /// </summary>
    public ObservableCollection<SkillTreeInformation> SkillTreeInformation { get; set; }

    /// <summary>
    /// Gets or sets skill trees.
    /// </summary>
    public SkillTreeInformation SelectedSkillTreeInformation { get; set; }

    /// <summary>
    /// Gets the ascendancy.
    /// </summary>
    public string Ascendancy
    {
        get
        {
            return _ascendency;
        }

        private set
        {
            _ascendency = value;
            NotifyOfPropertyChange();
        }
    }

    public bool Selected
    {
        get => field;

        set
        {
            field = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the popup is open.
    /// </summary>
    public bool IsSkillTreeOpen
    {
        get
        {
            return _isSkillTreeOpen;
        }

        set
        {
            _isSkillTreeOpen = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the gem view model.
    /// </summary>
    /// <value>The gem view model.</value>
    public GemViewModel GemViewModel { get; set; }

    /// <summary>
    /// Gets the items.
    /// </summary>
    public ObservableCollection<UniqueItemViewModel> Items { get; private set; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string DisplayName => _build == null ? string.Empty : $"{_build.Class} ({_build.Ascendancy})";

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string BuildName
    {
        get
        {
            return _build.Name;
        }

        set
        {
            _build.Name = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange("HasBuildName");
            NotifyOfPropertyChange("HasNoBuildName");
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has build name.
    /// </summary>
    public bool HasBuildName => !string.IsNullOrEmpty(BuildName);

    /// <summary>
    /// Gets a value indicating whether this instance has youtube.
    /// </summary>
    public bool HasYoutube => !string.IsNullOrEmpty(Youtube);

    /// <summary>
    /// Gets a value indicating whether this instance has forum.
    /// </summary>
    public bool HasForum => !string.IsNullOrEmpty(Forum);

    /// <summary>
    /// Gets a value indicating whether this instance has no build name.
    /// </summary>
    public bool HasNoBuildName => !HasBuildName;

    /// <summary>
    /// Gets or sets the youtube.
    /// </summary>
    public string Youtube
    {
        get
        {
            return _settings.YoutubeUrl;
        }

        set
        {
            _settings.YoutubeUrl = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange("HasYoutube");
        }
    }

    /// <summary>
    /// Gets or sets the forum post.
    /// </summary>
    public string Forum
    {
        get
        {
            return _settings.ForumUrl;
        }

        set
        {
            _settings.ForumUrl = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange("HasForum");
        }
    }

    #endregion

    #region Methods

    public void Select()
    {
        Selected = true;
    }

    /// <summary>
    /// Opens the tree.
    /// </summary>
    public void OpenTree()
    {
        IsSkillTreeOpen = true;
    }

    /// <summary>
    /// Opens the tree.
    /// </summary>
    public void OpenSelectedTree()
    {
        if (SelectedSkillTreeInformation != null && !string.IsNullOrEmpty(SelectedSkillTreeInformation.Url))
        {
            OpenUrl(SelectedSkillTreeInformation.Url);
        }
    }

    /// <summary>
    /// Opens the youtube.
    /// </summary>
    public void OpenYoutube()
        => OpenUrl(Youtube);

    /// <summary>
    /// Opens the forum.
    /// </summary>
    public void OpenForum()
        => OpenUrl(Forum);

    /// <summary>
    /// Opens the URL.
    /// </summary>
    /// <param name="value">The value.</param>
    private static void OpenUrl(string value)
    {
        if (Uri.TryCreate(value, UriKind.Absolute, out var _))
        {
            ProcessExtensions.OpenUrl(value);
        }
    }

    #endregion
}