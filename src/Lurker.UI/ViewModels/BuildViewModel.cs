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
    /// <seealso cref="Lurker.UI.ViewModels.PoeOverlayBase" />
    public class BuildViewModel : PoeOverlayBase
    {
        #region Fields

        private Task _currentTask;
        private bool _isVisible;
        private bool _hasNoBuild;
        private bool _skillTimelineEnabled;
        private IEventAggregator _eventAggregator;
        private PlayerService _playerService;
        private Player _activePlayer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildViewModel" /> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="processLurker">The process lurker.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="playerService">The player service.</param>
        public BuildViewModel(IWindowManager windowManager, DockingHelper dockingHelper, ProcessLurker processLurker, SettingsService settingsService, PlayerService playerService)
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            this._activePlayer = playerService.FirstPlayer;
            if (this._activePlayer != null && !string.IsNullOrEmpty(this._activePlayer.Build.Value))
            {
                this._currentTask = this.Initialize(this._activePlayer.Build.Value, false);
            }
            else
            {
                this._hasNoBuild = true;
            }

            this.IsVisible = true;
            this._eventAggregator = IoC.Get<IEventAggregator>();
            this.Skills = new ObservableCollection<SkillViewModel>();
            this._skillTimelineEnabled = this.SettingsService.TimelineEnabled;
            this._playerService = playerService;
            this.UniqueItems = new ObservableCollection<UniqueItemViewModel>();

            this.ActivePlayer = new PlayerViewModel(playerService);
            this._playerService.PlayerChanged += this.PlayerService_PlayerChanged;
        }

        #endregion

        #region Properties

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

        #endregion

        #region Methods

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
                    await service.InitializeAsync();
                    this.Build = service.Decode(buildValue);

                    this.Skills.Clear();
                    foreach (var skill in this.Build.Skills.Select(s => new SkillViewModel(s, this.SettingsService.ToolTipEnabled)))
                    {
                        skill.PropertyChanged += this.Skill_PropertyChanged;
                        this.Skills.Add(skill);
                    }

                    if (findMainSkill && this._activePlayer != null)
                    {
                        var settings = this._activePlayer.Build;
                        settings.Value = buildValue;
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
                    foreach (var item in this.Build.Items.Select(s => new UniqueItemViewModel(s, this.SettingsService.ToolTipEnabled)))
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
            this.Skills.Clear();
            this.UniqueItems.Clear();
            this.HasNoBuild = true;

            this._eventAggregator.PublishOnUIThread(new SkillMessage() { Clear = true });
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
                this.View.Height = 500;
                this.View.Width = 350;
                this.View.Left = windowInformation.Position.Right - 350 - margin;
                this.View.Top = windowInformation.Position.Bottom - value - 500 - margin;
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
                if (this.Build != null)
                {
                    e.Build.Value = this.Build.Value;
                }

                return;
            }

            this.ClearBuild();
            this._activePlayer = e;
            if (string.IsNullOrEmpty(e.Build.Value))
            {
                return;
            }

            await this.Initialize(e.Build.Value, false);
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
            this.SettingsService.TimelineEnabled = enabled;
            this.SettingsService.Save();

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
        }

        #endregion
    }
}