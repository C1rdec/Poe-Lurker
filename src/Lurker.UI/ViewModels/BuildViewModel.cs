//-----------------------------------------------------------------------
// <copyright file="BuildViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
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

        private static readonly string FileName = "build.pob";
        private Task _currentTask;
        private bool _isVisible;
        private bool _hasNoBuild;
        private bool _skillTimelineEnabled;
        private IEventAggregator _eventAggregator;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildViewModel"/> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="processLurker">The process lurker.</param>
        /// <param name="settingsService">The settings service.</param>
        public BuildViewModel(IWindowManager windowManager, DockingHelper dockingHelper, ProcessLurker processLurker, SettingsService settingsService)
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            if (AssetService.Exists(FileName))
            {
                this._currentTask = this.Initialize(File.ReadAllText(AssetService.GetFilePath(FileName)), false);
            }
            else
            {
                this._hasNoBuild = true;
            }

            this.IsVisible = true;
            this._eventAggregator = IoC.Get<IEventAggregator>();
            this.Skills = new ObservableCollection<SkillViewModel>();
            this._skillTimelineEnabled = this.SettingsService.BuildHelperSettings.TimelineEnabled;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the skills.
        /// </summary>
        public ObservableCollection<SkillViewModel> Skills { get; set; }

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
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            if (this.Build != null)
            {
                foreach (var skill in this.Skills)
                {
                    skill.PropertyChanged -= this.Skill_PropertyChanged;
                }

                this.Skills.Clear();

                var settings = this.SettingsService.BuildHelperSettings;
                foreach (var skill in this.Build.Skills.Select(s => new SkillViewModel(s, settings.TimelineEnabled)))
                {
                    skill.PropertyChanged += this.Skill_PropertyChanged;
                    this.Skills.Add(skill);
                }

                this.SelectSkills(settings);
            }

            base.OnActivate();
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
                    AssetService.Create(FileName, text);
                    var selectedSKill = this.Skills.FirstOrDefault(s => s.Selected);
                    if (selectedSKill != null)
                    {
                        // Handled in SkillTimelineViewModel
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
            this.Build = null;

            try
            {
                var settings = this.SettingsService.BuildHelperSettings;
                using (var service = new PathOfBuildingService())
                {
                    await service.InitializeAsync();
                    this.Build = service.Decode(buildValue);

                    this.Skills = new ObservableCollection<SkillViewModel>();
                    foreach (var skill in this.Build.Skills.Select(s => new SkillViewModel(s, settings.TimelineEnabled)))
                    {
                        skill.PropertyChanged += this.Skill_PropertyChanged;
                        this.Skills.Add(skill);
                    }

                    if (findMainSkill)
                    {
                        var mainSKill = this.Skills.OrderByDescending(s => s.Gems.Count(g => g.Support)).FirstOrDefault();
                        if (mainSKill != null)
                        {
                            var index = this.Skills.IndexOf(mainSKill);
                            settings.SkillsSelected.Clear();
                            settings.SkillsSelected.Add(index);
                            this.SettingsService.Save();
                        }
                    }

                    this.SelectSkills(settings, true);

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
        /// Selects the skills.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="raiseEvent">if set to <c>true</c> [raise event].</param>
        private void SelectSkills(BuildHelperSettings settings, bool raiseEvent = false)
        {
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
            if (skill.Selected)
            {
                var skillIndex = this.SettingsService.BuildHelperSettings.SkillsSelected.IndexOf(index);
                if (skillIndex == -1)
                {
                    this.SettingsService.BuildHelperSettings.SkillsSelected.Add(index);
                }
            }
            else
            {
                bool removed;
                do
                {
                    removed = this.SettingsService.BuildHelperSettings.SkillsSelected.Remove(index);
                }
                while (removed);
            }

            this.SettingsService.Save();
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
        /// Sets the timeline settings.
        /// </summary>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        private void SetTimelineSettings(bool enabled)
        {
            this.SettingsService.BuildHelperSettings.TimelineEnabled = enabled;
            this.SettingsService.Save();

            foreach (var skill in this.Skills)
            {
                skill.SetSelectable(enabled);
            }

            // Handled in ShellviewModel
            this._eventAggregator.PublishOnUIThread(new SkillTimelineMessage() { IsVisible = enabled });
        }

        #endregion
    }
}