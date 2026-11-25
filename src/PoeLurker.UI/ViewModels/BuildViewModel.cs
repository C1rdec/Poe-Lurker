//-----------------------------------------------------------------------
// <copyright file="BuildViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Caliburn.Micro;
using PoeLurker.Core;
using PoeLurker.Core.Extensions;
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
    private DateTime _lastBuildRefresh = DateTime.UtcNow;
    private bool _isVisible;
    private string _ascendancy;
    private bool _skillTimelineEnabled;
    private readonly IEventAggregator _eventAggregator;
    private readonly PlayerService _playerService;
    private Player _activePlayer;
    private readonly BuildService _buildService;
    private Build _currentBuild;
    private BuildDetailsViewModel _buildDetails;
    private bool _includeInventorySlots = true;
    private bool _includePassives = true;
    private bool _includeSkills = true;
    private readonly SettingsViewModel _settings;
    private readonly GithubService _githubService;
    private readonly PathOfBuildingService _pathOfBuildingService;
    private readonly MouseLurker _mouseLurker;
    private bool _isSkillTreeOpen;
    private System.IO.FileSystemWatcher _buildWatcher;
    private System.Threading.Timer _buildWatcherTimer;

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
    /// <param name="pathOfBuildingService">The path of building service.</param>
    public BuildViewModel(
        DockingHelper dockingHelper, 
        ProcessService processLurker, 
        SettingsService settingsService, 
        BuildService buildService, 
        PlayerService playerService, 
        SettingsViewModel settingsViewModel, 
        GithubService githubService, 
        MouseLurker mouseLurker, 
        PathOfBuildingService pathOfBuildingService)
        : base(dockingHelper, processLurker, settingsService)
    {
        _pathOfBuildingService = pathOfBuildingService;
        _settings = settingsViewModel;
        _playerService = playerService;
        _buildService = buildService;
        _githubService = githubService;
        _activePlayer = playerService.FirstPlayer;
        _skillTimelineEnabled = SettingsService.TimelineEnabled;
        _eventAggregator = IoC.Get<IEventAggregator>();

        Skills = [];
        UniqueItems = [];
        SkillTreeInformation = [];

        _mouseLurker = mouseLurker;

        // TODO : Set current build from player  

        IsVisible = true;

        ActivePlayer = new PlayerViewModel(playerService);

        BuildSelector = new BuildSelectorViewModel(buildService, _activePlayer?.BuildPath);
        WatchBuild(_activePlayer?.BuildPath);

        var launchPath = _activePlayer?.BuildPath;
        if (!string.IsNullOrEmpty(launchPath))
        {
            var launchBuild = _buildService.Get(launchPath);
            if (launchBuild != null)
            {
                launchBuild.FilePath = launchPath;
                _currentBuild = launchBuild;
                BuildDetails = CreateBuildDetails(launchBuild);
            }
        }
        if (!string.IsNullOrEmpty(launchPath) && System.IO.File.Exists(launchPath))
        {
            var plannerFile = _pathOfBuildingService.GetPlannerFilePath(System.IO.Path.GetFileNameWithoutExtension(launchPath));
            if (!System.IO.File.Exists(plannerFile) || System.IO.File.GetLastWriteTimeUtc(plannerFile) < System.IO.File.GetLastWriteTimeUtc(launchPath))
            {
                Task.Run(() => RefreshWatchedBuild(launchPath));
            }
        }
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
    /// Gets or sets the selected build details view model.
    /// </summary>
    public BuildDetailsViewModel BuildDetails
    {
        get
        {
            return _buildDetails;
        }

        private set
        {
            _buildDetails = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(() => ActiveBuildContent);
            NotifyOfPropertyChange(() => IsBuildSelectorVisible);
        }
    }

    /// <summary>
    /// Gets the active build content.
    /// </summary>
    public object ActiveBuildContent => (object)BuildDetails ?? BuildSelector;

    /// <summary>
    /// Gets a value indicating whether the build selector is visible.
    /// </summary>
    public bool IsBuildSelectorVisible => BuildDetails == null;

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

                if (_isOpen && DateTime.UtcNow - _lastBuildRefresh >= TimeSpan.FromSeconds(20))
                {
                    _lastBuildRefresh = DateTime.UtcNow;
                    BuildSelector?.Refresh();
                    NotifyOfPropertyChange(nameof(HasNoBuilds));
                    NotifyOfPropertyChange(nameof(HasNoBuildSelected));
                }
            }
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
    /// Gets the active player.
    /// </summary>
    public PlayerViewModel ActivePlayer { get; private set; }

    /// <summary>
    /// Gets a value indicating whether no build is selected.
    /// </summary>
    public bool HasNoBuildSelected => BuildSelector?.Builds.Count > 0 && !BuildSelector.Builds.Any(b => b.Selected);

    /// <summary>
    /// Gets a value indicating whether no build was detected.
    /// </summary>
    public bool HasNoBuilds => BuildSelector == null || BuildSelector.Builds.Count == 0;

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
    /// Gets or sets the build.
    /// </summary>
    public Build Build { get; set; }

    /// <summary>
    /// Gets the data click command.
    /// </summary>
    public MyCommand<Build> DataClickCommand => new()
    {
        ExecuteDelegate = p => SelectBuild(p),
    };

    #endregion

    #region Methods

    /// <summary>
    /// Selects the build.
    /// </summary>
    /// <param name="build">The build.</param>
    public void SelectBuild(Build build)
    {
        _currentBuild = build;
        if (build == null)
        {
            ShowBuildSelector();
            WatchBuild(null);
            return;
        }

        if (_activePlayer != null)
        {
            _activePlayer.BuildPath = build.FilePath;
            _playerService.Save();

            // Add the build to POE 2 Build Planner
            _pathOfBuildingService.AddToPlanner(build, _includeInventorySlots, _includePassives, _includeSkills);
        }

        WatchBuild(build.FilePath);
        BuildDetails = CreateBuildDetails(build);
    }

    private void WatchBuild(string buildPath)
    {
        StopWatchingBuild();

        if (string.IsNullOrEmpty(buildPath) || !System.IO.File.Exists(buildPath))
        {
            return;
        }

        try
        {
            var watcher = new System.IO.FileSystemWatcher(System.IO.Path.GetDirectoryName(buildPath), System.IO.Path.GetFileName(buildPath))
            {
                NotifyFilter = System.IO.NotifyFilters.LastWrite | System.IO.NotifyFilters.Size | System.IO.NotifyFilters.FileName,
            };

            watcher.Changed += BuildWatcher_Changed;
            watcher.Created += BuildWatcher_Changed;
            watcher.Renamed += BuildWatcher_Changed;
            watcher.EnableRaisingEvents = true;
            _buildWatcher = watcher;
        }
        catch (Exception ex) when (ex is ArgumentException or System.IO.IOException)
        {
        }
    }

    private void StopWatchingBuild()
    {
        if (_buildWatcher != null)
        {
            _buildWatcher.Changed -= BuildWatcher_Changed;
            _buildWatcher.Created -= BuildWatcher_Changed;
            _buildWatcher.Renamed -= BuildWatcher_Changed;
            _buildWatcher.Dispose();
            _buildWatcher = null;
        }

        _buildWatcherTimer?.Dispose();
        _buildWatcherTimer = null;
    }

    private void BuildWatcher_Changed(object sender, System.IO.FileSystemEventArgs e)
    {
        // Debounce: Path of Building can raise several events per save.
        _buildWatcherTimer?.Dispose();
        _buildWatcherTimer = new System.Threading.Timer(_ => RefreshWatchedBuild(e.FullPath), null, 500, System.Threading.Timeout.Infinite);
    }

    private void RefreshWatchedBuild(string path)
    {
        try
        {
            if (!IsSamePath(_activePlayer?.BuildPath, path) && !IsSamePath(_currentBuild?.FilePath, path))
            {
                return;
            }

            var build = _buildService.Get(path);
            if (build == null)
            {
                return;
            }

            build.FilePath = path;
            _currentBuild = build;

            if (BuildDetails != null && IsSamePath(BuildDetails.Build?.FilePath, build.FilePath))
            {
                Execute.OnUIThread(() => BuildDetails = CreateBuildDetails(build));
            }

            _pathOfBuildingService.AddToPlanner(build, _includeInventorySlots, _includePassives, _includeSkills);
        }
        catch (Exception ex) when (ex is System.IO.IOException or System.Xml.XmlException)
        {
        }
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
            if (Uri.TryCreate(text, UriKind.Absolute, out var url))
            {
                var rawUri = new Uri($"https://pastebin.com/raw{url.AbsolutePath}");
                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, rawUri);
                var response = await client.SendAsync(request);
                text = await response.Content.ReadAsStringAsync();
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
            await _pathOfBuildingService.InitializeAsync();
            Build = _pathOfBuildingService.Decode(buildValue);

            SkillTreeInformation.Clear();

            foreach (var tree in Build.SkillTrees.Reverse<SkillTreeInformation>())
            {
                SkillTreeInformation.Add(tree);
            }

            Ascendancy = Build.Ascendancy;

            // To notify that we are initialize.
            NotifyOfPropertyChange(nameof(Skills));
        }
        catch
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Opens the tree.
    /// </summary>
    public void OpenSelectedTree()
    {
        if (SelectedSkillTreeInformation != null && !string.IsNullOrEmpty(SelectedSkillTreeInformation.Url))
        {
            ProcessExtensions.OpenUrl(SelectedSkillTreeInformation.Url);
        }
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

    public void ShowBuildSelector()
    {
        BuildDetails = null;
    }

    private BuildDetailsViewModel CreateBuildDetails(Build build)
    {
        return new BuildDetailsViewModel(
            build,
            ShowBuildSelector,
            UpdatePlannerBuild,
            _includeInventorySlots,
            _includePassives,
            _includeSkills);
    }

    private void UpdatePlannerBuild(bool includeInventorySlots, bool includePassives, bool includeSkills)
    {
        _includeInventorySlots = includeInventorySlots;
        _includePassives = includePassives;
        _includeSkills = includeSkills;

        if (_currentBuild == null)
        {
            return;
        }

        _pathOfBuildingService.AddToPlanner(_currentBuild, _includeInventorySlots, _includePassives, _includeSkills);
    }

    private static bool IsSamePath(string leftPath, string rightPath)
    {
        if (string.IsNullOrWhiteSpace(leftPath) || string.IsNullOrWhiteSpace(rightPath))
        {
            return false;
        }

        try
        {
            leftPath = System.IO.Path.GetFullPath(leftPath);
            rightPath = System.IO.Path.GetFullPath(rightPath);
        }
        catch (Exception ex) when (ex is ArgumentException or System.IO.IOException)
        {
        }

        return string.Equals(leftPath, rightPath, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Called when activating.
    /// </summary>
    protected override Task OnActivatedAsync(CancellationToken token)
    {
        _mouseLurker.MouseLeftButtonUp += MouseLurker_MouseLeftButtonUp;
        BuildSelector.BuildSelected += BuildSelector_BuildSelected;
        _playerService.PlayerChanged += PlayerService_PlayerChanged;
        WatchBuild(_activePlayer?.BuildPath);
        if (View != null)
        {
            View.Deactivated += View_Deactivated;
        }

        return base.OnActivatedAsync(token);
    }

    /// <summary>
    /// Handles the MouseLeftButtonUp event of the MouseLurker control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void MouseLurker_MouseLeftButtonUp(object sender, EventArgs e)
    {
        IsOpen = false;
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
        StopWatchingBuild();

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
            View.Left = ApplyScalingX(windowInformation.Position.Right - 383);
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
    private void BuildSelector_BuildSelected(object sender, Build e)
    {
        SelectBuild(e);
        NotifyOfPropertyChange(() => HasNoBuildSelected);
        DockingHelper.SetForeground();
    }

    /// <summary>
    /// Handles the Deactivated event of the View control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private void View_Deactivated(object sender, System.EventArgs e)
    {
        ActivePlayer?.SelectionVisible = false;
    }

    /// <summary>
    /// Players the service player changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void PlayerService_PlayerChanged(object sender, Player e)
    {
        _activePlayer = e;

        Execute.OnUIThread(() =>
        {
            BuildSelector.SetSelected(e?.BuildPath);
            _currentBuild = _buildService.Get(e?.BuildPath);
            if (_currentBuild != null)
            {
                _currentBuild.FilePath = e.BuildPath;
            }

            ShowBuildSelector();
            WatchBuild(e?.BuildPath);
            NotifyOfPropertyChange(() => HasNoBuildSelected);
            DockingHelper.SetForeground();
        });
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