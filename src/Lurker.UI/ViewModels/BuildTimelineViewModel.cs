//-----------------------------------------------------------------------
// <copyright file="BuildTimelineViewModel.cs" company="Wohs Inc.">
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
    using Lurker.Services;
    using Lurker.UI.Models;
    using NuGet;

    /// <summary>
    /// Represent the overlay of the time line.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.PoeOverlayBase" />
    public class BuildTimelineViewModel : PoeOverlayBase, IHandle<SkillMessage>, IHandle<ItemMessage>
    {
        #region Fields

        private static readonly int MaxLevel = 100;
        private List<Skill> _skills;
        private List<UniqueItem> _items;
        private PlayerService _playerService;
        private string _playerName;
        private string _playerLevel;
        private IEventAggregator _eventAggregator;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildTimelineViewModel" /> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="dockingHelper">The docking helper.</param>
        /// <param name="processLurker">The process lurker.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="playerService">The character service.</param>
        public BuildTimelineViewModel(IWindowManager windowManager, DockingHelper dockingHelper, ProcessLurker processLurker, SettingsService settingsService, PlayerService playerService)
            : base(windowManager, dockingHelper, processLurker, settingsService)
        {
            this._playerService = playerService;
            var firstPlayer = this._playerService.FirstPlayer;

            var progress = 0;
            if (firstPlayer != null)
            {
                progress = firstPlayer.Levels.FirstOrDefault();
                this.PlayerName = firstPlayer.Name;
            }

            this.Timeline = new TimelineViewModel(progress, MaxLevel);
            this.PlayerName = firstPlayer != null ? firstPlayer.Name : string.Empty;

            this.PlayerLevel = string.Empty;
            if (firstPlayer != null)
            {
                var level = firstPlayer.Levels.FirstOrDefault();
                if (level != 0)
                {
                    this.PlayerLevel = level.ToString();
                }
            }

            this._playerService.PlayerChanged += this.PlayerChanged;

            this._eventAggregator = IoC.Get<IEventAggregator>();
            this._skills = new List<Skill>();
            this._items = new List<UniqueItem>();
            this.ActivePlayer = new PlayerViewModel(playerService);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the active player.
        /// </summary>
        public PlayerViewModel ActivePlayer { get; set; }

        /// <summary>
        /// Gets or sets the timeline.
        /// </summary>
        public TimelineViewModel Timeline { get; set; }

        /// <summary>
        /// Gets the name of the character.
        /// </summary>
        public string PlayerName
        {
            get
            {
                return this._playerName;
            }

            private set
            {
                this._playerName = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets the player level.
        /// </summary>
        public string PlayerLevel
        {
            get
            {
                return this._playerLevel;
            }

            private set
            {
                this._playerLevel = value;
                this.NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

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
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ItemMessage message)
        {
            if (message.Clear)
            {
                this._items.Clear();
            }

            if (message.Delete)
            {
                bool removed;
                do
                {
                    removed = this._items.Remove(message.Item);
                }
                while (removed);
            }
            else
            {
                var index = this._items.IndexOf(message.Item);
                if (index == -1)
                {
                    this._items.Add(message.Item);
                }
            }

            this.Timeline.Clear();
            this.GenerateTimelineItems();
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(SkillMessage message)
        {
            if (message.Clear)
            {
                this._skills.Clear();
                this._items.Clear();
            }

            if (message.Delete)
            {
                bool removed;
                do
                {
                    removed = this._skills.Remove(message.Skill);
                }
                while (removed);
            }
            else if (message.Skill != null)
            {
                var index = this._skills.IndexOf(message.Skill);
                if (index == -1)
                {
                    this._skills.Add(message.Skill);
                }
            }

            this.Timeline.Clear();
            this.GenerateTimelineItems();
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            this._eventAggregator.Subscribe(this);
            if (this.View != null)
            {
                this.View.Deactivated += this.View_Deactivated;
            }

            base.OnActivate();
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected override void OnDeactivate(bool close)
        {
            this._eventAggregator.Unsubscribe(this);
            this.View.Deactivated -= this.View_Deactivated;
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

            var overlayHeight = 35 * windowInformation.FlaskBarHeight / DefaultFlaskBarHeight * this.SettingsService.TradebarScaling;
            var overlayWidth = windowInformation.Width - (windowInformation.FlaskBarWidth * 2);

            Execute.OnUIThread(() =>
            {
                this.View.Height = this.ApplyAbsoluteScalingY(overlayHeight);
                this.View.Width = this.ApplyAbsoluteScalingX(overlayWidth);
                this.View.Left = this.ApplyScalingX(windowInformation.Position.Left + windowInformation.FlaskBarWidth + Margin);
                this.View.Top = this.ApplyScalingY(windowInformation.Position.Bottom - overlayHeight - windowInformation.ExpBarHeight + Margin);
            });
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
        /// Players the changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void PlayerChanged(object sender, Player e)
        {
            this.PlayerName = e.Name;
            var level = e.GetCurrentLevel();
            this.PlayerLevel = level.ToString();
            this.Timeline.SetProgess(level);
        }

        /// <summary>
        /// Generates the timeline items.
        /// </summary>
        private void GenerateTimelineItems()
        {
            if (!this._skills.Any() && !this._items.Any())
            {
                this.Timeline.SetMaxValue(MaxLevel);
                return;
            }

            this.AddItems();
        }

        /// <summary>
        /// Adds the items.
        /// </summary>
        private void AddItems()
        {
            if (this.SettingsService.GroupBySkill)
            {
                this.GroupBySKill();
                return;
            }

            var combineGems = this._skills.SelectMany(s => s.Gems).AsEnumerable<WikiItem>();
            var combineItems = combineGems.Concat(this._items);

            foreach (var items in combineItems.GroupBy(g => g.Level))
            {
                var views = new List<WikiItemBaseViewModel>();
                foreach (var item in items)
                {
                    WikiItemBaseViewModel view = null;
                    if (item is Gem gem)
                    {
                        view = new GemViewModel(gem);
                    }

                    if (item is UniqueItem uniqueItem)
                    {
                        view = new UniqueItemViewModel(uniqueItem, false);
                    }

                    if (view == null)
                    {
                        continue;
                    }

                    views.Add(view);
                }

                if (views.Any())
                {
                    var timelineItem = new TimelineItemViewModel(items.Key)
                    {
                        DetailedView = new GroupItemViewModel(views),
                    };
                    this.Timeline.AddItem(timelineItem);
                }
            }
        }

        /// <summary>
        /// Groups the by s kill.
        /// </summary>
        private void GroupBySKill()
        {
            var skillGroups = this._skills.GroupBy(s => s.Gems.Max(g => g.Level));
            foreach (var group in skillGroups)
            {
                var timelineItem = new TimelineItemViewModel(group.Key);

                var wikiItems = new List<WikiItemBaseViewModel>();
                foreach (var entry in group.AsEnumerable())
                {
                    wikiItems.AddRange(entry.Gems.Select(g => new GemViewModel(g)));
                }

                foreach (var item in this._items.Where(i => i.Level == group.Key))
                {
                    wikiItems.Add(new UniqueItemViewModel(item, false));
                }

                timelineItem.DetailedView = new GroupItemViewModel(wikiItems);

                this.Timeline.AddItem(timelineItem);
            }

            foreach (var uniqueItem in this._items)
            {
                if (skillGroups.Any(s => s.Key == uniqueItem.Level))
                {
                    continue;
                }

                var timelineItem = new TimelineItemViewModel(uniqueItem.Level)
                {
                    DetailedView = new UniqueItemViewModel(uniqueItem, false),
                };
                this.Timeline.AddItem(timelineItem);
            }
        }

        #endregion
    }
}