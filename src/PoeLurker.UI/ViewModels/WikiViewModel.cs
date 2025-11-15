//-----------------------------------------------------------------------
// <copyright file="WikiViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using PoeLurker.Core;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;
using PoeLurker.Patreon.Models;
using PoeLurker.Patreon.Services;
using ProcessLurker;
using Winook;

/// <summary>
/// Represents the wiki overlay.
/// </summary>
/// <seealso cref="PoeOverlayBase" />
public class WikiViewModel : PoeOverlayBase
{
    #region Fields

    private readonly GithubService _githubService;
    private string _searchValue = string.Empty;
    private readonly MouseLurker _mouseLurker;
    private readonly KeyboardLurker _keyboardLurker;
    private readonly ClientLurker _clientLurker;
    private readonly PoeNinjaService _ninjaService;
    private bool _visible;
    private DivineRatioViewModel _divineRatioViewModel;
    private IEnumerable<UniqueItem> _uniques;
    private string _currentLeague;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="WikiViewModel" /> class.
    /// </summary>
    /// <param name="dockingHelper">The docking helper.</param>
    /// <param name="processLurker">The process lurker.</param>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="githubService">The github service.</param>
    /// <param name="mouseLurker">The mouse lurker.</param>
    /// <param name="keyboardLurker">The keyboard lurker.</param>
    /// <param name="ninjaService">The ninja service.</param>
    /// <param name="clientLurker">The client lurker.</param>
    public WikiViewModel(
        DockingHelper dockingHelper,
        ProcessService processLurker,
        SettingsService settingsService,
        GithubService githubService,
        MouseLurker mouseLurker,
        KeyboardLurker keyboardLurker,
        PoeNinjaService ninjaService,
        ClientLurker clientLurker)
        : base(dockingHelper, processLurker, settingsService)
    {
        _githubService = githubService;
        _mouseLurker = mouseLurker;
        _keyboardLurker = keyboardLurker;
        _ninjaService = ninjaService;
        _clientLurker = clientLurker;
        _currentLeague = SettingsService.RecentLeagueName;
        Items = [];

        _clientLurker.LeagueChanged += ClientLurker_LeagueChanged;
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the current league.
    /// </summary>
    public string CurrentLeague => _currentLeague;

    /// <summary>
    /// Gets or sets the search value.
    /// </summary>
    public string SearchValue
    {
        get
        {
            return _searchValue;
        }

        set
        {
            _searchValue = value;
            Execute.OnUIThread(() => Search(value));
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether is Visible.
    /// </summary>
    public bool Visible
    {
        get
        {
            return _visible;
        }

        set
        {
            _visible = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    public ObservableCollection<WikiItemBaseViewModel> Items { get; private set; }

    /// <summary>
    /// Gets or sets the ExaltedRatio.
    /// </summary>
    public PropertyChangedBase CurrentView { get; set; }

    #endregion

    #region Methods

    /// <summary>
    /// Closes this instance.
    /// </summary>
    public void CloseWindow()
    {
        SearchValue = string.Empty;
        CurrentView = null;
        NotifyOfPropertyChange(() => CurrentView);
        Visible = false;
        DockingHelper.SetForeground();
    }

    /// <summary>
    /// Show the instance.
    /// </summary>
    /// <returns>The task.</returns>
    public async Task Show()
    {
        var clipboardTask = ClipboardHelper.GetItemInClipboard();
        Visible = true;
        SetInForeground();
        var clipboardItem = await clipboardTask;
        ClipboardHelper.ClearClipboard();
        if (clipboardItem != null && clipboardItem.Rarity == Rarity.Unique)
        {
            var item = _uniques.FirstOrDefault(u => u.Name == clipboardItem.BaseType);
            if (item != null)
            {
                SearchValue = item.Name;
                OnItemClick(item);
            }
        }
        else
        {
            await SetDivineRatio();
        }
    }

    /// <summary>
    /// On keydown.
    /// </summary>
    /// <param name="e">The event.</param>
    public void OnKeyDown(KeyEventArgs e)
    {
        if (_keyboardLurker.OpenWikiHotkey == null)
        {
            return;
        }

        var keyValue = e.Key.ToString();
        if (Enum.TryParse<KeyCode>(keyValue, out var keyCode))
        {
            if (keyCode == _keyboardLurker.OpenWikiHotkey.KeyCode)
            {
                CloseWindow();
            }
        }
    }

    /// <summary>
    /// Closes this instance.
    /// </summary>
    /// <param name="view">The window information.</param>
    protected override void OnViewReady(object view)
    {
        base.OnViewReady(view);
        var window = view as System.Windows.Window;
        Execute.OnUIThread(() => window.Focus());
    }

    /// <summary>
    /// Closes this instance.
    /// </summary>
    protected async override Task OnActivatedAsync(CancellationToken token)
    {
        _uniques = await _githubService.Uniques();
        SetInForeground();
        _mouseLurker.MouseLeftButtonUp += MouseLurker_MouseLeftButtonUp;

        await base.OnActivatedAsync(token);
    }

    /// <summary>
    /// On Desactivate.
    /// </summary>
    /// <param name="close">if close.</param>
    protected override Task OnDeactivateAsync(bool close, CancellationToken token)
    {
        _mouseLurker.MouseLeftButtonUp -= MouseLurker_MouseLeftButtonUp;

        return base.OnDeactivateAsync(close, token);
    }

    /// <summary>
    /// Sets the window position.
    /// </summary>
    /// <param name="windowInformation">The window information.</param>
    protected override void SetWindowPosition(PoeWindowInformation windowInformation)
    {
        if (View == null)
        {
            return;
        }

        var height = windowInformation.Height / 4;
        var overlayWidth = windowInformation.Width - (windowInformation.FlaskBarWidth * 2);
        Execute.OnUIThread(() =>
        {
            View.Height = ApplyAbsoluteScalingY(height);
            View.Width = ApplyAbsoluteScalingX(overlayWidth);
            View.Left = ApplyScalingX(windowInformation.Position.Left + windowInformation.FlaskBarWidth + Margin);
            View.Top = ApplyScalingY(windowInformation.Position.Bottom - height - windowInformation.ExpBarHeight + Margin);
        });
    }

    private void ClientLurker_LeagueChanged(object sender, string leagueName)
    {
        _currentLeague = leagueName;
        NotifyOfPropertyChange(() => CurrentLeague);
    }

    private void MouseLurker_MouseLeftButtonUp(object sender, System.EventArgs e)
    {
        CloseWindow();
    }

    private void Search(string value)
    {
        CurrentView = _divineRatioViewModel;
        NotifyOfPropertyChange(() => CurrentView);
        Items.Clear();
        if (string.IsNullOrEmpty(value))
        {
            _divineRatioViewModel?.SetFraction(null);

            return;
        }

        if (double.TryParse(value, out var fraction))
        {
            _divineRatioViewModel?.SetFraction(fraction);

            return;
        }

        var items = _githubService.Search(value);

        foreach (var item in items)
        {
            switch (item)
            {
                case Gem gem:
                    Items.Add(new GemViewModel(gem));
                    break;
                case UniqueItem uniqueItem:
                    Items.Add(new UniqueItemViewModel(uniqueItem, OnItemClick));
                    break;
                default:
                    break;
            }
        }

        if (Items.Any())
        {
            CurrentView = null;
            NotifyOfPropertyChange(() => CurrentView);
        }

        if (items.Count() == 1)
        {
            var firstItem = Items.FirstOrDefault();
            if (firstItem is UniqueItemViewModel unique)
            {
                OnItemClick(unique.Item);
            }
        }
    }

    private async void OnItemClick(UniqueItem item)
    {
        var itemLine = await _ninjaService.GetItemAsync(item.Name, SettingsService.RecentLeagueName);
        if (itemLine == null)
        {
            return;
        }

        CurrentView = new ItemChartViewModel(itemLine, item);
        NotifyOfPropertyChange(() => CurrentView);
    }

    private async Task SetDivineRatio()
    {
        if (string.IsNullOrEmpty(SettingsService.RecentLeagueName))
        {
            return;
        }

        var line = await _ninjaService.GetDivineRationAsync(SettingsService.RecentLeagueName);
        if (line != null && line.ChaosEquivalent != 0)
        {
            _divineRatioViewModel = new DivineRatioViewModel(line);
            CurrentView = _divineRatioViewModel;
            NotifyOfPropertyChange(() => CurrentView);
        }
    }

    #endregion
}