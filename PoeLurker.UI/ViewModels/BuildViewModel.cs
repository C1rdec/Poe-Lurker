//-----------------------------------------------------------------------
// <copyright file="BuildViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Caliburn.Micro;
using PoeLurker.Core;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;
using PoeLurker.UI.Models;
using ProcessLurker;

/// <summary>
/// Represents a build viewmodel.
/// </summary>
/// <seealso cref="PoeOverlayBase" />
public class BuildViewModel : PoeOverlayBase
{
    #region Fields

    private readonly Task _currentTask;
    private bool _isOpen;
    private bool _isVisible;
    private string _ascendancy;
    private bool _isOptionOpen;
    private bool _hasNoBuild;
    private bool _skillTimelineEnabled;
    private readonly IEventAggregator _eventAggregator;
    private readonly PlayerService _playerService;
    private Player _activePlayer;
    private readonly BuildService _buildService;
    private SimpleBuild _currentBuild;
    private readonly SettingsViewModel _settings;
    private readonly GithubService _githubService;
    private readonly MouseLurker _mouseLurker;
    private bool _isSkillTreeOpen;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BuildViewModel" /> class.
    /// </summary>
    /// <param name="windowManager">The window manager.</param>
    /// <param name="dockingHelper">The docking helper.</param>
    /// <param name="processLurker">The process lurker.</param>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="buildService">The build service.</param>
    /// <param name="playerService">The player service.</param>
    /// <param name="settingsViewModel">The settings view model.</param>
    /// <param name="githubService">The github service.</param>
    /// <param name="mouseLurker">The mouse lurker.</param>
    public BuildViewModel(IWindowManager windowManager, DockingHelper dockingHelper, ProcessService processLurker, SettingsService settingsService, BuildService buildService, PlayerService playerService, SettingsViewModel settingsViewModel, GithubService githubService, MouseLurker mouseLurker)
        : base(windowManager, dockingHelper, processLurker, settingsService)
    {
        _settings = settingsViewModel;
        _playerService = playerService;
        _buildService = buildService;
        _githubService = githubService;
        _activePlayer = playerService.FirstPlayer;
        _skillTimelineEnabled = SettingsService.TimelineEnabled;
        _eventAggregator = IoC.Get<IEventAggregator>();

        Skills = new ObservableCollection<SkillViewModel>();
        UniqueItems = new ObservableCollection<UniqueItemViewModel>();
        SkillTreeInformation = new ObservableCollection<SkillTreeInformation>();

        _mouseLurker = mouseLurker;

        if (_activePlayer != null && !string.IsNullOrEmpty(_activePlayer.Build.BuildId))
        {
            var build = buildService.Builds.FirstOrDefault(b => b.Id == _activePlayer.Build.BuildId);
            if (build == null)
            {
                _hasNoBuild = true;
            }
            else
            {
                _currentBuild = build;
                _currentTask = Initialize(build.PathOfBuildingCode, false);
            }
        }
        else
        {
            _hasNoBuild = true;
        }

        IsVisible = true;

        ActivePlayer = new PlayerViewModel(playerService);
        Builds = new ObservableCollection<SimpleBuild>();

        BuildSelector = new BuildSelectorViewModel(buildService);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets or sets skill trees.
    /// </summary>
    public ObservableCollection<SkillTreeInformation> SkillTreeInformation { get; set; }

    /// <summary>
    /// Gets or sets skill trees.
    /// </summary>
    public SkillTreeInformation SelectedSkillTreeInformation { get; set; }

    /// <summary>
    /// Gets or sets the build selector.
    /// </summary>
    public BuildSelectorViewModel BuildSelector { get; set; }

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
    /// Gets or sets a value indicating whether this instance is open.
    /// </summary>
    public bool IsOpen
    {
        get
        {
            return _isOpen;
        }

        set
        {
            if (_isOpen != value)
            {
                _isOpen = value;
                NotifyOfPropertyChange();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is option open.
    /// </summary>
    public bool IsOptionOpen
    {
        get
        {
            return _isOptionOpen;
        }

        set
        {
            _isOptionOpen = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets the ascendancy.
    /// </summary>
    public string Ascendancy
    {
        get
        {
            return _ascendancy;
        }

        private set
        {
            _ascendancy = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets the builds.
    /// </summary>
    public ObservableCollection<SimpleBuild> Builds { get; private set; }

    /// <summary>
    /// Gets the active player.
    /// </summary>
    public PlayerViewModel ActivePlayer { get; private set; }

    /// <summary>
    /// Gets or sets the skills.
    /// </summary>
    public ObservableCollection<SkillViewModel> Skills { get; set; }

    /// <summary>
    /// Gets or sets the unique items.
    /// </summary>
    public ObservableCollection<UniqueItemViewModel> UniqueItems { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is visible.
    /// </summary>
    public bool IsVisible
    {
        get
        {
            return _isVisible;
        }

        set
        {
            _isVisible = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [skill timeline enabled].
    /// </summary>
    public bool SkillTimelineEnabled
    {
        get
        {
            return _skillTimelineEnabled;
        }

        set
        {
            _skillTimelineEnabled = value;
            NotifyOfPropertyChange();
            SetTimelineSettings(_skillTimelineEnabled);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [automatic close enabled].
    /// </summary>
    public bool AutoCloseEnabled
    {
        get
        {
            return SettingsService.BuildAutoClose;
        }

        set
        {
            SettingsService.BuildAutoClose = value;
            SettingsService.Save();
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [group by skill].
    /// </summary>
    public bool GroupBySkill
    {
        get
        {
            return SettingsService.GroupBySkill;
        }

        set
        {
            SettingsService.GroupBySkill = value;
            SettingsService.Save(false);
            NotifyOfPropertyChange();

            // Just to refresh the items
            _eventAggregator.PublishOnUIThreadAsync(new SkillMessage());
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance has build.
    /// </summary>
    public bool HasNoBuild
    {
        get
        {
            return _hasNoBuild;
        }

        set
        {
            _hasNoBuild = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange("HasBuild");
        }
    }

    /// <summary>
    /// Gets a value indicating whether this instance has build.
    /// </summary>
    public bool HasBuild => !HasNoBuild;

    /// <summary>
    /// Gets or sets the build.
    /// </summary>
    public Build Build { get; set; }

    /// <summary>
    /// Gets the data click command.
    /// </summary>
    public MyCommand<SimpleBuild> DataClickCommand => new MyCommand<SimpleBuild>()
    {
        ExecuteDelegate = p => SelectBuild(p),
    };

    #endregion

    #region Methods

    /// <summary>
    /// Selects the build.
    /// </summary>
    /// <param name="build">The build.</param>
    public async void SelectBuild(SimpleBuild build)
    {
        _currentBuild = build;
        if (_activePlayer != null)
        {
            _activePlayer.SetBuild(build.Id);
            _playerService.Save();
        }

        ClearBuild();
        await Initialize(build.PathOfBuildingCode, true);
    }

    /// <summary>
    /// Opens the option.
    /// </summary>
    public void ShowOption()
    {
        IsOptionOpen = true;
    }

    /// <summary>
    /// Imports this instance.
    /// </summary>
    public async void Import()
    {
        try
        {
            if (_currentTask != null)
            {
                await _currentTask;
            }

            var text = ClipboardHelper.GetClipboardText();
            if (Uri.TryCreate(text, UriKind.Absolute, out Uri url))
            {
                var rawUri = new Uri($"https://pastebin.com/raw{url.AbsolutePath}");
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, rawUri);
                    var response = await client.SendAsync(request);
                    text = await response.Content.ReadAsStringAsync();
                }
            }

            if (await Initialize(text, true))
            {
                var selectedSKill = Skills.FirstOrDefault(s => s.Selected);
                if (selectedSKill != null)
                {
                    // Handled in BuildTimelineViewModel
                    await _eventAggregator.PublishOnUIThreadAsync(new SkillMessage() { Clear = true, Skill = selectedSKill.Skill });
                }
            }
        }
        catch
        {
        }
    }

    /// <summary>
    /// Opens the tree.
    /// </summary>
    public void OpenTree()
    {
        IsSkillTreeOpen = true;
    }

    /// <summary>
    /// Toggles this instance.
    /// </summary>
    public void Toggle()
    {
        IsVisible = !IsVisible;
    }

    /// <summary>
    /// Initializes the specified build value.
    /// </summary>
    /// <param name="buildValue">The build value.</param>
    /// <param name="findMainSkill">if set to <c>true</c> [find main skill].</param>
    /// <returns>
    /// Representing the asynchronous operation.
    /// </returns>
    public async Task<bool> Initialize(string buildValue, bool findMainSkill)
    {
        if (string.IsNullOrEmpty(buildValue))
        {
            return false;
        }

        Build = null;

        try
        {
            using (var service = new PathOfBuildingService())
            {
                await service.InitializeAsync(_githubService);
                Build = service.Decode(buildValue);

                SkillTreeInformation.Clear();

                foreach (var tree in Build.SkillTrees.Reverse<SkillTreeInformation>())
                {
                    SkillTreeInformation.Add(tree);
                }

                Ascendancy = Build.Ascendancy;
                Skills.Clear();
                foreach (var skill in Build.Skills.Select(s => new SkillViewModel(s, SettingsService.TimelineEnabled)))
                {
                    skill.PropertyChanged += Skill_PropertyChanged;
                    Skills.Add(skill);
                }

                if (findMainSkill && _activePlayer != null)
                {
                    var settings = _activePlayer.Build;
                    var mainSKill = Skills.OrderByDescending(s => s.Gems.Count(g => g.Support)).FirstOrDefault();
                    if (mainSKill != null)
                    {
                        var index = Skills.IndexOf(mainSKill);
                        settings.ItemsSelected.Clear();
                        settings.SkillsSelected.Clear();
                        settings.SkillsSelected.Add(index);
                    }

                    _playerService.Save();
                }

                UniqueItems.Clear();
                foreach (var item in Build.Items.Select(s => new UniqueItemViewModel(s, SettingsService.TimelineEnabled)))
                {
                    item.PropertyChanged += Item_PropertyChanged;
                    UniqueItems.Add(item);
                }

                SelectItems(true);

                // To notify that we are initialize.
                NotifyOfPropertyChange("Skills");
            }
        }
        catch
        {
            return false;
        }

        HasNoBuild = false;
        return true;
    }

    /// <summary>
    /// Opens the tree.
    /// </summary>
    public void OpenSelectedTree()
    {
        if (SelectedSkillTreeInformation != null && !string.IsNullOrEmpty(SelectedSkillTreeInformation.Url))
        {
            Process.Start(SelectedSkillTreeInformation.Url);
        }
    }

    /// <summary>
    /// Clears the build.
    /// </summary>
    public void ClearBuild()
    {
        Build = null;
        Execute.OnUIThread(() =>
        {
            ClearEventHandlers();
            Skills.Clear();
            UniqueItems.Clear();
        });

        // Handled in the Timeline
        _eventAggregator.PublishOnUIThreadAsync(new SkillMessage() { Clear = true });
    }

    /// <summary>
    /// Creates new build.
    /// </summary>
    public void NewBuild()
    {
        // Set to build tab
        _settings.SelectTabIndex = 0;
        _eventAggregator.PublishOnUIThreadAsync(_settings);
    }

    /// <summary>
    /// Called when activating.
    /// </summary>
    protected override Task OnActivateAsync(CancellationToken token)
    {
        _mouseLurker.MouseLeftButtonUp += MouseLurker_MouseLeftButtonUp;
        BuildSelector.BuildSelected += BuildSelector_BuildSelected;
        _playerService.PlayerChanged += PlayerService_PlayerChanged;
        if (View != null)
        {
            View.Deactivated += View_Deactivated;
        }

        Execute.OnUIThread(() =>
        {
            if (Build != null)
            {
                ClearEventHandlers();

                // Gems
                Skills.Clear();
                foreach (var skill in Build.Skills.Select(s => new SkillViewModel(s, SettingsService.TimelineEnabled)))
                {
                    skill.PropertyChanged += Skill_PropertyChanged;
                    Skills.Add(skill);
                }

                // Unique items
                UniqueItems.Clear();
                foreach (var item in Build.Items.Select(s => new UniqueItemViewModel(s, SettingsService.TimelineEnabled)))
                {
                    item.PropertyChanged += Item_PropertyChanged;
                    UniqueItems.Add(item);
                }

                SelectItems();
            }

            Builds.Clear();
            foreach (var build in _buildService.Builds)
            {
                Builds.Add(build);
            }
        });

        return base.OnActivateAsync(token);
    }

    /// <summary>
    /// Handles the MouseLeftButtonUp event of the MouseLurker control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void MouseLurker_MouseLeftButtonUp(object sender, EventArgs e)
    {
        if (SettingsService.BuildAutoClose)
        {
            IsOpen = false;
        }
    }

    /// <summary>
    /// Called when deactivating.
    /// </summary>
    /// <param name="close">Inidicates whether this instance will be closed.</param>
    protected override Task OnDeactivateAsync(bool close, CancellationToken token)
    {
        _mouseLurker.MouseLeftButtonUp -= MouseLurker_MouseLeftButtonUp;
        _playerService.PlayerChanged -= PlayerService_PlayerChanged;
        BuildSelector.BuildSelected -= BuildSelector_BuildSelected;

        ClearEventHandlers();
        View.Deactivated -= View_Deactivated;

        return base.OnDeactivateAsync(close, token);
    }

    /// <summary>
    /// Sets the window position.
    /// </summary>
    /// <param name="windowInformation">The window information.</param>
    protected override void SetWindowPosition(PoeWindowInformation windowInformation)
    {
        var value = 220 * windowInformation.Height / 1080;
        Execute.OnUIThread(() =>
        {
            View.Height = ApplyAbsoluteScalingY(500);
            View.Width = ApplyAbsoluteScalingX(350);
            View.Left = ApplyScalingX(windowInformation.Position.Right - 350);
            View.Top = ApplyScalingY(windowInformation.Position.Bottom - value - 500);
        });
    }

    /// <summary>
    /// Called when an attached view's Loaded event fires.
    /// </summary>
    /// <param name="view">The view.</param>
    protected override void OnViewLoaded(object view)
    {
        base.OnViewLoaded(view);
        View.Deactivated += View_Deactivated;
    }

    /// <summary>
    /// Builds the selector build selected.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void BuildSelector_BuildSelected(object sender, SimpleBuild e)
    {
        SelectBuild(e);
    }

    /// <summary>
    /// Handles the Deactivated event of the View control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void View_Deactivated(object sender, System.EventArgs e)
    {
        if (ActivePlayer != null)
        {
            ActivePlayer.SelectionVisible = false;
        }
    }

    /// <summary>
    /// Players the service player changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private async void PlayerService_PlayerChanged(object sender, Player e)
    {
        // First Character
        if (_activePlayer == null)
        {
            if (_currentBuild != null)
            {
                e.Build.BuildId = _currentBuild.Id;
            }

            return;
        }

        if (_activePlayer.Name == e.Name)
        {
            return;
        }

        ClearBuild();
        if (string.IsNullOrEmpty(e.Build.BuildId))
        {
            return;
        }

        _activePlayer = e;
        var build = _buildService.Builds.FirstOrDefault(b => b.Id == e.Build.BuildId);
        if (build != null)
        {
            await Initialize(build.PathOfBuildingCode, false);
        }
        else
        {
            HasNoBuild = true;
        }
    }

    /// <summary>
    /// Clears the event handlers.
    /// </summary>
    private void ClearEventHandlers()
    {
        foreach (var skill in Skills)
        {
            skill.PropertyChanged -= Skill_PropertyChanged;
        }

        foreach (var item in UniqueItems)
        {
            item.PropertyChanged -= Item_PropertyChanged;
        }
    }

    /// <summary>
    /// Handles the PropertyChanged event of the Item control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var item = sender as UniqueItemViewModel;
        if (item == null || e.PropertyName != "Selected")
        {
            return;
        }

        if (_activePlayer == null)
        {
            return;
        }

        var index = UniqueItems.IndexOf(item);
        var settings = _activePlayer.Build;
        if (item.Selected)
        {
            var itemIndex = settings.ItemsSelected.IndexOf(index);
            if (itemIndex == -1)
            {
                settings.ItemsSelected.Add(index);
            }
        }
        else
        {
            bool removed;
            do
            {
                removed = settings.ItemsSelected.Remove(index);
            }
            while (removed);
        }

        _playerService.Save();
    }

    /// <summary>
    /// Selects the skills.
    /// </summary>
    /// <param name="raiseEvent">if set to <c>true</c> [raise event].</param>
    private void SelectItems(bool raiseEvent = false)
    {
        if (_activePlayer == null)
        {
            return;
        }

        var settings = _activePlayer.Build;
        foreach (var index in settings.SkillsSelected.ToArray())
        {
            if (index >= Skills.Count)
            {
                continue;
            }

            var selectedSKill = Skills.ElementAt(index);
            if (selectedSKill != null)
            {
                selectedSKill.Selected = true;

                if (raiseEvent)
                {
                    _eventAggregator.PublishOnUIThreadAsync(new SkillMessage() { Skill = selectedSKill.Skill });
                }
            }
        }

        foreach (var index in settings.ItemsSelected.ToArray())
        {
            if (index >= UniqueItems.Count)
            {
                continue;
            }

            var selectedItem = UniqueItems.ElementAt(index);
            if (selectedItem != null)
            {
                selectedItem.Selected = true;

                if (raiseEvent)
                {
                    _eventAggregator.PublishOnUIThreadAsync(new ItemMessage() { Item = selectedItem.Item });
                }
            }
        }
    }

    /// <summary>
    /// Handles the PropertyChanged event of the Skill control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void Skill_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        var skill = sender as SkillViewModel;
        if (skill == null || e.PropertyName != "Selected")
        {
            return;
        }

        if (_activePlayer == null)
        {
            return;
        }

        var index = Skills.IndexOf(skill);
        var settings = _activePlayer.Build;

        if (skill.Selected)
        {
            var skillIndex = settings.SkillsSelected.IndexOf(index);
            if (skillIndex == -1)
            {
                settings.SkillsSelected.Add(index);
            }
        }
        else
        {
            bool removed;
            do
            {
                removed = settings.SkillsSelected.Remove(index);
            }
            while (removed);
        }

        _playerService.Save();
    }

    /// <summary>
    /// Sets the timeline settings.
    /// </summary>
    /// <param name="enabled">if set to <c>true</c> [enabled].</param>
    private void SetTimelineSettings(bool enabled)
    {
        if (!enabled)
        {
            _eventAggregator.PublishOnUIThreadAsync(new SkillMessage() { Clear = true });
        }

        SettingsService.TimelineEnabled = enabled;
        SettingsService.Save(false);

        foreach (var skill in Skills)
        {
            skill.SetSelectable(enabled);
        }

        foreach (var item in UniqueItems)
        {
            item.SetSelectable(enabled);
        }

        // Handled in ShellviewModel
        _eventAggregator.PublishOnUIThreadAsync(new SkillTimelineMessage() { IsVisible = enabled });
        foreach (var skillViewModel in Skills.Where(s => s.Selected))
        {
            _eventAggregator.PublishOnUIThreadAsync(new SkillMessage() { Skill = skillViewModel.Skill });
        }
    }

    #endregion
}