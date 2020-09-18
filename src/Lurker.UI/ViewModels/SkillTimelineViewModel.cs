//-----------------------------------------------------------------------
// <copyright file="SkillTimelineViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Patreon.Events;
    using Lurker.Services;
    using Lurker.UI.Models;

    /// <summary>
    /// Represent the overlay of the time line.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.PoeOverlayBase" />
    public class SkillTimelineViewModel : BuildViewModel, IHandle<SkillMessage>
    {
        #region Fields

        private static readonly int MaxLevel = 100;
        private List<Skill> _skills;
        private CharacterService _characterService;
        private string _characterName;
        private IEventAggregator _eventAggregator;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SkillTimelineViewModel" /> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="processLurker">The process lurker.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="characterService">The character service.</param>
        public SkillTimelineViewModel(IWindowManager windowManager, DockingHelper dockingHelper, ProcessLurker processLurker, SettingsService settingsService, CharacterService characterService)
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            this._characterService = characterService;
            var firstPlayer = this._characterService.FirstPlayer;

            var progress = 0;
            if (firstPlayer != null)
            {
                progress = firstPlayer.Levels.FirstOrDefault();
                this.CharacterName = firstPlayer.Name;
            }

            this.Timeline = new TimelineViewModel(progress, MaxLevel);
            this.CharacterName = firstPlayer != null ? firstPlayer.Name : string.Empty;
            this._characterService.PlayerChanged += this.PlayerChanged;

            this._eventAggregator = IoC.Get<IEventAggregator>();
            this._skills = new List<Skill>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the timeline.
        /// </summary>
        public TimelineViewModel Timeline { get; set; }

        /// <summary>
        /// Gets the name of the character.
        /// </summary>
        public string CharacterName
        {
            get
            {
                return this._characterName;
            }

            private set
            {
                this._characterName = value;
                this.NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(SkillMessage message)
        {
            if (message.Delete)
            {
                bool removed;
                do
                {
                    removed = this._skills.Remove(message.Skill);
                }
                while (removed);
            }
            else
            {
                var index = this._skills.IndexOf(message.Skill);
                if (index == -1)
                {
                    this._skills.Add(message.Skill);
                }
            }

            this.Timeline.Clear();
            this.AddSkillItems();
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            this._eventAggregator.Subscribe(this);
            base.OnActivate();
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected override void OnDeactivate(bool close)
        {
            this._eventAggregator.Unsubscribe(this);
            base.OnDeactivate(close);
        }

        /// <summary>
        /// Sets the window position.
        /// </summary>
        /// <param name="windowInformation">The window information.</param>
        protected override void SetWindowPosition(PoeWindowInformation windowInformation)
        {
            // When Poe Lurker is updated we save the settings before the view are loaded
            if (this.View == null)
            {
                return;
            }

            var overlayHeight = 30 * windowInformation.FlaskBarHeight / DefaultFlaskBarHeight * this.SettingsService.TradebarScaling;
            var overlayWidth = windowInformation.Width - (windowInformation.FlaskBarWidth * 2);
            var margin = PoeApplicationContext.WindowStyle == WindowStyle.Windowed ? 10 : 0;

            Execute.OnUIThread(() =>
            {
                this.View.Height = overlayHeight;
                this.View.Width = overlayWidth;
                this.View.Left = windowInformation.Position.Left + windowInformation.FlaskBarWidth + Margin - margin;
                this.View.Top = windowInformation.Position.Bottom - overlayHeight - windowInformation.ExpBarHeight - Margin;
            });
        }

        /// <summary>
        /// Players the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void PlayerChanged(object sender, PlayerLevelUpEvent e)
        {
            this.CharacterName = e.PlayerName;
            this.Timeline.SetProgess(e.Level);
        }

        /// <summary>
        /// Adds the skill items.
        /// </summary>
        private void AddSkillItems()
        {
            if (!this._skills.Any())
            {
                this.Timeline.SetMaxValue(MaxLevel);
                return;
            }

            var combineGems = this._skills.SelectMany(s => s.Gems);
            this.Timeline.SetMaxValue(combineGems.Max(g => g.Level) + 1);
            foreach (var gems in combineGems.GroupBy(g => g.Level))
            {
                var skill = new Skill();
                foreach (var gem in gems)
                {
                    skill.AddGem(gem);
                }

                var item = new TimelineItemViewModel(gems.Key)
                {
                    DetailedView = new SkillViewModel(skill),
                };

                this.Timeline.AddItem(item);
            }
        }

        #endregion
    }
}