//-----------------------------------------------------------------------
// <copyright file="BuildViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Services;
    using Lurker.UI.Models;

    /// <summary>
    /// Represents a build viewmodel.
    /// </summary>
    /// <seealso cref="PoeOverlayBase" />
    public class BuildViewModel : PoeOverlayBase
    {
        #region Fields

        private Task _currentTask;
        private bool _isOpen;
        private bool _isVisible;
        private string _ascendancy;
        private bool _isOptionOpen;
        private bool _hasNoBuild;
        private bool _skillTimelineEnabled;
        private IEventAggregator _eventAggregator;
        private PlayerService _playerService;
        private Player _activePlayer;
        private BuildService _buildService;
        private SimpleBuild _currentBuild;
        private SettingsViewModel _settings;
        private GithubService _githubService;

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
        public BuildViewModel(IWindowManager windowManager, DockingHelper dockingHelper, ProcessLurker processLurker, SettingsService settingsService, BuildService buildService, PlayerService playerService, SettingsViewModel settingsViewModel, GithubService githubService)
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            this._githubService = githubService;
            this._activePlayer = playerService.FirstPlayer;
            if (this._activePlayer != null && !string.IsNullOrEmpty(this._activePlayer.Build.BuildId))
            {
                var build = buildService.Builds.FirstOrDefault(b => b.Id == this._activePlayer.Build.BuildId);
                if (build == null)
                {
                    this._hasNoBuild = true;
                }
                else
                {
                    this._currentBuild = build;
                    this._currentTask = this.Initialize(build.PathOfBuildingCode, false);
                }
            }
            else
            {
                this._hasNoBuild = true;
            }

            this.IsVisible = true;
            this._settings = settingsViewModel;
            this._eventAggregator = IoC.Get<IEventAggregator>();
            this.Skills = new ObservableCollection<SkillViewModel>();
            this._skillTimelineEnabled = this.SettingsService.TimelineEnabled;
            this._playerService = playerService;
            this.UniqueItems = new ObservableCollection<UniqueItemViewModel>();

            this.ActivePlayer = new PlayerViewModel(playerService);
            this._playerService.PlayerChanged += this.PlayerService_PlayerChanged;
            this._buildService = buildService;
            this.Builds = new ObservableCollection<SimpleBuild>();

            this.BuildSelector = new BuildSelectorViewModel(buildService);
            this.BuildSelector.BuildSelected += this.BuildSelector_BuildSelected;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the build selector.
        /// </summary>
        public BuildSelectorViewModel BuildSelector { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return this._isOpen;
            }

            set
            {
                this._isOpen = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is option open.
        /// </summary>
        public bool IsOptionOpen
        {
            get
            {
                return this._isOptionOpen;
            }

            set
            {
                this._isOptionOpen = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the ascendancy.
        /// </summary>
        public string Ascendancy
        {
            get
            {
                return this._ascendancy;
            }

            private set
            {
                this._ascendancy = value;
                this.NotifyOfPropertyChange();
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
                return this._isVisible;
            }

            set
            {
                this._isVisible = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [skill timeline enabled].
        /// </summary>
        public bool SkillTimelineEnabled
        {
            get
            {
                return this._skillTimelineEnabled;
            }

            set
            {
                this._skillTimelineEnabled = value;
                this.NotifyOfPropertyChange();
                this.SetTimelineSettings(this._skillTimelineEnabled);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [group by skill].
        /// </summary>
        public bool GroupBySkill
        {
            get
            {
                return this.SettingsService.GroupBySkill;
            }

            set
            {
                this.SettingsService.GroupBySkill = value;
                this.SettingsService.Save(false);
                this.NotifyOfPropertyChange();

                // Just to refresh the items
                this._eventAggregator.PublishOnUIThread(new SkillMessage());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has build.
        /// </summary>
        public bool HasNoBuild
        {
            get
            {
                return this._hasNoBuild;
            }

            set
            {
                this._hasNoBuild = value;
                this.NotifyOfPropertyChange();
                this.NotifyOfPropertyChange("HasBuild");
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has build.
        /// </summary>
        public bool HasBuild => !this.HasNoBuild;

        /// <summary>
        /// Gets or sets the build.
        /// </summary>
        public Build Build { get; set; }

        /// <summary>
        /// Gets the data click command.
        /// </summary>
        public MyCommand<SimpleBuild> DataClickCommand => new MyCommand<SimpleBuild>()
        {
            ExecuteDelegate = p => this.SelectBuild(p),
        };

        #endregion

        #region Methods

        /// <summary>
        /// Selects the build.
        /// </summary>
        /// <param name="build">The build.</param>
        public async void SelectBuild(SimpleBuild build)
        {
            this._currentBuild = build;
            if (this._activePlayer != null)
            {
                this._activePlayer.SetBuild(build.Id);
                this._playerService.Save();
            }

            this.ClearBuild();
            await this.Initialize(build.PathOfBuildingCode, true);
        }

        /// <summary>
        /// Opens the option.
        /// </summary>
        public void ShowOption()
        {
            this.IsOptionOpen = true;
        }

        /// <summary>
        /// Imports this instance.
        /// </summary>
        public async void Import()
        {
            try
            {
                if (this._currentTask != null)
                {
                    await this._currentTask;
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

                if (await this.Initialize(text, true))
                {
                    var selectedSKill = this.Skills.FirstOrDefault(s => s.Selected);
                    if (selectedSKill != null)
                    {
                        // Handled in BuildTimelineViewModel
                        this._eventAggregator.PublishOnUIThread(new SkillMessage() { Clear = true, Skill = selectedSKill.Skill });
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Trees this instance.
        /// </summary>
        public void Tree()
        {
            if (this.Build != null)
            {
                Process.Start(this.Build.SkillTreeUrl);
            }
        }

        /// <summary>
        /// Toggles this instance.
        /// </summary>
        public void Toggle()
        {
            this.IsVisible = !this.IsVisible;
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

            this.Build = null;

            try
            {
                using (var service = new PathOfBuildingService())
                {
                    await service.InitializeAsync(this._githubService);
                    this.Build = service.Decode(buildValue);
                    this.Ascendancy = this.Build.Ascendancy;
                    this.Skills.Clear();
                    foreach (var skill in this.Build.Skills.Select(s => new SkillViewModel(s, this.SettingsService.TimelineEnabled)))
                    {
                        skill.PropertyChanged += this.Skill_PropertyChanged;
                        this.Skills.Add(skill);
                    }

                    if (findMainSkill && this._activePlayer != null)
                    {
                        var settings = this._activePlayer.Build;
                        var mainSKill = this.Skills.OrderByDescending(s => s.Gems.Count(g => g.Support)).FirstOrDefault();
                        if (mainSKill != null)
                        {
                            var index = this.Skills.IndexOf(mainSKill);
                            settings.ItemsSelected.Clear();
                            settings.SkillsSelected.Clear();
                            settings.SkillsSelected.Add(index);
                        }

                        this._playerService.Save();
                    }

                    this.UniqueItems.Clear();
                    foreach (var item in this.Build.Items.Select(s => new UniqueItemViewModel(s, this.SettingsService.TimelineEnabled)))
                    {
                        item.PropertyChanged += this.Item_PropertyChanged;
                        this.UniqueItems.Add(item);
                    }

                    this.SelectItems(true);

                    // To notify that we are initialize.
                    this.NotifyOfPropertyChange("Skills");
                }
            }
            catch
            {
                return false;
            }

            this.HasNoBuild = false;
            return true;
        }

        /// <summary>
        /// Clears the build.
        /// </summary>
        public void ClearBuild()
        {
            this.Build = null;
            Execute.OnUIThread(() =>
            {
                this.Skills.Clear();
                this.UniqueItems.Clear();
            });
            this._eventAggregator.PublishOnUIThread(new SkillMessage() { Clear = true });
        }

        /// <summary>
        /// Creates new build.
        /// </summary>
        public void NewBuild()
        {
            // Set to build tab
            this._settings.SelectTabIndex = 0;
            this._eventAggregator.PublishOnUIThread(this._settings);
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            if (this.View != null)
            {
                this.View.Deactivated += this.View_Deactivated;
            }

            Execute.OnUIThread(() =>
            {
                if (this.Build != null)
                {
                    this.ClearEventHandlers();

                    // Gems
                    this.Skills.Clear();
                    foreach (var skill in this.Build.Skills.Select(s => new SkillViewModel(s, this.SettingsService.TimelineEnabled)))
                    {
                        skill.PropertyChanged += this.Skill_PropertyChanged;
                        this.Skills.Add(skill);
                    }

                    // Unique items
                    this.UniqueItems.Clear();
                    foreach (var item in this.Build.Items.Select(s => new UniqueItemViewModel(s, this.SettingsService.TimelineEnabled)))
                    {
                        item.PropertyChanged += this.Item_PropertyChanged;
                        this.UniqueItems.Add(item);
                    }

                    this.SelectItems();
                }

                this.Builds.Clear();
                foreach (var build in this._buildService.Builds)
                {
                    this.Builds.Add(build);
                }
            });

            base.OnActivate();
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="windowInformation">The window information.</param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            var value = 220 * windowInformation.Height / 1080;
            var margin = PoeApplicationContext.WindowStyle == WindowStyle.Windowed ? 10 : 0;
            Execute.OnUIThread(() =>
            {
                this.View.Height = this.ApplyScalingY(500);
                this.View.Width = this.ApplyScalingX(350);
                this.View.Left = this.ApplyScalingX(windowInformation.Position.Right - 350 - margin);
                this.View.Top = this.ApplyScalingY(windowInformation.Position.Bottom - value - 500 - margin);
            });
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected override void OnDeactivate(bool close)
        {
            this.ClearEventHandlers();
            this.View.Deactivated -= this.View_Deactivated;
            base.OnDeactivate(close);
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view">The view.</param>
        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            this.View.Deactivated += this.View_Deactivated;
        }

        /// <summary>
        /// Builds the selector build selected.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void BuildSelector_BuildSelected(object sender, SimpleBuild e)
        {
            this.SelectBuild(e);
        }

        /// <summary>
        /// Handles the Deactivated event of the View control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void View_Deactivated(object sender, System.EventArgs e)
        {
            if (this.ActivePlayer != null)
            {
                this.ActivePlayer.SelectionVisible = false;
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
            if (this._activePlayer == null)
            {
                if (this._currentBuild != null)
                {
                    e.Build.BuildId = this._currentBuild.Id;
                }

                return;
            }

            if (this._activePlayer.Name == e.Name)
            {
                return;
            }

            this.ClearBuild();
            if (string.IsNullOrEmpty(e.Build.BuildId))
            {
                return;
            }

            this._activePlayer = e;
            var build = this._buildService.Builds.FirstOrDefault(b => b.Id == e.Build.BuildId);
            if (build != null)
            {
                await this.Initialize(build.PathOfBuildingCode, false);
            }
            else
            {
                this.HasNoBuild = true;
            }
        }

        /// <summary>
        /// Clears the event handlers.
        /// </summary>
        private void ClearEventHandlers()
        {
            foreach (var skill in this.Skills)
            {
                skill.PropertyChanged -= this.Skill_PropertyChanged;
            }

            foreach (var item in this.UniqueItems)
            {
                item.PropertyChanged -= this.Item_PropertyChanged;
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

            if (this._activePlayer == null)
            {
                return;
            }

            var index = this.UniqueItems.IndexOf(item);
            var settings = this._activePlayer.Build;
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

            this._playerService.Save();
        }

        /// <summary>
        /// Selects the skills.
        /// </summary>
        /// <param name="raiseEvent">if set to <c>true</c> [raise event].</param>
        private void SelectItems(bool raiseEvent = false)
        {
            if (this._activePlayer == null)
            {
                return;
            }

            var settings = this._activePlayer.Build;
            foreach (var index in settings.SkillsSelected.ToArray())
            {
                if (index >= this.Skills.Count)
                {
                    continue;
                }

                var selectedSKill = this.Skills.ElementAt(index);
                if (selectedSKill != null)
                {
                    selectedSKill.Selected = true;

                    if (raiseEvent)
                    {
                        this._eventAggregator.PublishOnUIThread(new SkillMessage() { Skill = selectedSKill.Skill });
                    }
                }
            }

            foreach (var index in settings.ItemsSelected.ToArray())
            {
                if (index >= this.UniqueItems.Count)
                {
                    continue;
                }

                var selectedItem = this.UniqueItems.ElementAt(index);
                if (selectedItem != null)
                {
                    selectedItem.Selected = true;

                    if (raiseEvent)
                    {
                        this._eventAggregator.PublishOnUIThread(new ItemMessage() { Item = selectedItem.Item });
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

            if (this._activePlayer == null)
            {
                return;
            }

            var index = this.Skills.IndexOf(skill);
            var settings = this._activePlayer.Build;

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

            this._playerService.Save();
        }

        /// <summary>
        /// Sets the timeline settings.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        private void SetTimelineSettings(bool enabled)
        {
            if (!enabled)
            {
                this._eventAggregator.PublishOnUIThread(new SkillMessage() { Clear = true });
            }

            this.SettingsService.TimelineEnabled = enabled;
            this.SettingsService.Save(false);

            foreach (var skill in this.Skills)
            {
                skill.SetSelectable(enabled);
            }

            foreach (var item in this.UniqueItems)
            {
                item.SetSelectable(enabled);
            }

            // Handled in ShellviewModel
            this._eventAggregator.PublishOnUIThread(new SkillTimelineMessage() { IsVisible = enabled });
            foreach (var skillViewModel in this.Skills.Where(s => s.Selected))
            {
                this._eventAggregator.PublishOnUIThread(new SkillMessage() { Skill = skillViewModel.Skill });
            }
        }

        #endregion
    }
}