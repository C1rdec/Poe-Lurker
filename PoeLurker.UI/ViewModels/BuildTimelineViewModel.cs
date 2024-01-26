//-----------------------------------------------------------------------
// <copyright file="BuildTimelineViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;
using PoeLurker.UI.Models;
using ProcessLurker;

/// <summary>
/// Represent the overlay of the time line.
/// </summary>
/// <seealso cref="PoeLurker.UI.ViewModels.PoeOverlayBase" />
public class BuildTimelineViewModel : PoeOverlayBase, IHandle<SkillMessage>, IHandle<ItemMessage>
{
    #region Fields

    private static readonly int MaxLevel = 100;
    private readonly List<Skill> _skills;
    private readonly List<UniqueItem> _items;
    private readonly PlayerService _playerService;
    private string _playerName;
    private string _playerLevel;
    private readonly IEventAggregator _eventAggregator;

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
    public BuildTimelineViewModel(IWindowManager windowManager, DockingHelper dockingHelper, ProcessService processLurker, SettingsService settingsService, PlayerService playerService)
        : base(windowManager, dockingHelper, processLurker, settingsService)
    {
        _playerService = playerService;
        var firstPlayer = _playerService.FirstPlayer;

        var progress = 0;
        if (firstPlayer != null)
        {
            progress = firstPlayer.Levels.FirstOrDefault();
            PlayerName = firstPlayer.Name;
        }

        Timeline = new TimelineViewModel(progress, MaxLevel);
        PlayerName = firstPlayer != null ? firstPlayer.Name : string.Empty;

        PlayerLevel = string.Empty;
        if (firstPlayer != null)
        {
            var level = firstPlayer.Levels.FirstOrDefault();
            if (level != 0)
            {
                PlayerLevel = level.ToString();
            }
        }

        _playerService.PlayerChanged += PlayerChanged;

        _eventAggregator = IoC.Get<IEventAggregator>();
        _skills = new List<Skill>();
        _items = new List<UniqueItem>();
        ActivePlayer = new PlayerViewModel(playerService);
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
            return _playerName;
        }

        private set
        {
            _playerName = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets the player level.
    /// </summary>
    public string PlayerLevel
    {
        get
        {
            return _playerLevel;
        }

        private set
        {
            _playerLevel = value;
            NotifyOfPropertyChange();
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
        View.Deactivated += View_Deactivated;
    }

    /// <summary>
    /// Handles the message.
    /// </summary>
    /// <param name="message">The message.</param>
    public Task HandleAsync(ItemMessage message, CancellationToken token)
    {
        if (message.Clear)
        {
            _items.Clear();
        }

        if (message.Delete)
        {
            bool removed;
            do
            {
                removed = _items.Remove(message.Item);
            }
            while (removed);
        }
        else
        {
            var index = _items.IndexOf(message.Item);
            if (index == -1)
            {
                _items.Add(message.Item);
            }
        }

        Timeline.Clear();
        GenerateTimelineItems();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles the message.
    /// </summary>
    /// <param name="message">The message.</param>
    public Task HandleAsync(SkillMessage message, CancellationToken token)
    {
        if (message.Clear)
        {
            _skills.Clear();
            _items.Clear();
        }

        if (message.Delete)
        {
            bool removed;
            do
            {
                removed = _skills.Remove(message.Skill);
            }
            while (removed);
        }
        else if (message.Skill != null)
        {
            var index = _skills.IndexOf(message.Skill);
            if (index == -1)
            {
                _skills.Add(message.Skill);
            }
        }

        Timeline.Clear();
        GenerateTimelineItems();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Called when activating.
    /// </summary>
    protected override Task OnActivateAsync(CancellationToken token)
    {
        _eventAggregator.SubscribeOnPublishedThread(this);
        if (View != null)
        {
            View.Deactivated += View_Deactivated;
        }

        return base.OnActivateAsync(token);
    }

    /// <summary>
    /// Called when deactivating.
    /// </summary>
    /// <param name="close">Inidicates whether this instance will be closed.</param>
    protected override Task OnDeactivateAsync(bool close, CancellationToken token)
    {
        _eventAggregator.Unsubscribe(this);
        View.Deactivated -= View_Deactivated;

        return base.OnDeactivateAsync(close, token);
    }

    /// <summary>
    /// Sets the window position.
    /// </summary>
    /// <param name="windowInformation">The window information.</param>
    protected override void SetWindowPosition(PoeWindowInformation windowInformation)
    {
        // When Poe Lurker is updated we save the settings before the view are loaded
        if (View == null)
        {
            return;
        }

        var overlayHeight = 35 * windowInformation.FlaskBarHeight / DefaultFlaskBarHeight * SettingsService.TradebarScaling;
        var overlayWidth = windowInformation.Width - (windowInformation.FlaskBarWidth * 2);

        Execute.OnUIThread(() =>
        {
            View.Height = ApplyAbsoluteScalingY(overlayHeight);
            View.Width = ApplyAbsoluteScalingX(overlayWidth);
            View.Left = ApplyScalingX(windowInformation.Position.Left + windowInformation.FlaskBarWidth + Margin);
            View.Top = ApplyScalingY(windowInformation.Position.Bottom - overlayHeight - windowInformation.ExpBarHeight + Margin);
        });
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
    /// Players the changed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The e.</param>
    private void PlayerChanged(object sender, Player e)
    {
        PlayerName = e.Name;
        var level = e.GetCurrentLevel();
        PlayerLevel = level.ToString();
        Timeline.SetProgess(level);
    }

    /// <summary>
    /// Generates the timeline items.
    /// </summary>
    private void GenerateTimelineItems()
    {
        if (!_skills.Any() && !_items.Any())
        {
            Timeline.SetMaxValue(MaxLevel);
            return;
        }

        AddItems();
    }

    /// <summary>
    /// Adds the items.
    /// </summary>
    private void AddItems()
    {
        if (SettingsService.GroupBySkill)
        {
            GroupBySKill();
            return;
        }

        var combineGems = _skills.SelectMany(s => s.Gems).AsEnumerable<WikiItem>();
        var combineItems = combineGems.Concat(_items);

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
                Timeline.AddItem(timelineItem);
            }
        }
    }

    /// <summary>
    /// Groups the by s kill.
    /// </summary>
    private void GroupBySKill()
    {
        var skillGroups = _skills.GroupBy(s => s.Gems.Max(g => g.Level));
        foreach (var group in skillGroups)
        {
            var timelineItem = new TimelineItemViewModel(group.Key);

            var wikiItems = new List<WikiItemBaseViewModel>();
            foreach (var entry in group.AsEnumerable())
            {
                wikiItems.AddRange(entry.Gems.Select(g => new GemViewModel(g)));
            }

            foreach (var item in _items.Where(i => i.Level == group.Key))
            {
                wikiItems.Add(new UniqueItemViewModel(item, false));
            }

            timelineItem.DetailedView = new GroupItemViewModel(wikiItems);

            Timeline.AddItem(timelineItem);
        }

        foreach (var uniqueItem in _items)
        {
            if (skillGroups.Any(s => s.Key == uniqueItem.Level))
            {
                continue;
            }

            var timelineItem = new TimelineItemViewModel(uniqueItem.Level)
            {
                DetailedView = new UniqueItemViewModel(uniqueItem, false),
            };
            Timeline.AddItem(timelineItem);
        }
    }

    #endregion
}