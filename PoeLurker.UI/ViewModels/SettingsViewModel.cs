//-----------------------------------------------------------------------
// <copyright file="SettingsViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using NAudio.Wave;
using PoeLurker.Core.Extensions;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Services;
using PoeLurker.Patreon.Services;
using PoeLurker.UI.Helpers;
using PoeLurker.UI.Services;
using TextCopy;
using Winook;

/// <summary>
/// Represents the settings view model.
/// </summary>
/// <seealso cref="PoeLurker.UI.ViewModels.Screen" />
public class SettingsViewModel : Screen
{
    #region Fields

    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private static readonly string LottieFileName = "LurckerIconSettings.json";
    private readonly Task _activateTask;
    private bool _trialAvailable;
    private readonly KeyboardHelper _keyboardHelper;
    private readonly SettingsService _settingService;
    private readonly HotkeyService _hotkeyService;
    private bool _modified;
    private string _blessingtext;
    private bool _needsUpdate;
    private bool _pledging;
    private bool _activated;
    private int _selectedTabIndex;
    private int _alertVolume;
    private int _itemAlertVolume;
    private int _joinHideoutVolume;
    private readonly PoeLurkerPatreonService _currentPatreonService;
    private readonly SoundService _soundService;
    private CancellationTokenSource _currentTokenSource;
    private bool _hasCustomTradeSound;
    private bool _hasCustomItemSound;
    private WaveOutEvent _currentSound;
    private bool _isCharacterOpen;
    private bool _isPushBulletOpen;
    private CharacterManagerViewModel _characterManager;
    private PushProviderViewModel _selectedPushprovider;
    private bool _keyboardWaiting;
    private MetroWindow _view;
    private readonly IEnumerable<string> _excludePropertyNames;
    private readonly WinookService _winookService;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel" /> class.
    /// </summary>
    /// <param name="windowManager">The window manager.</param>
    /// <param name="keyboardHelper">The keyboard helper.</param>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="hotkeyService">The key code service.</param>
    /// <param name="soundService">The sound service.</param>
    /// <param name="githubService">The github service.</param>
    /// <param name="pushBulletService">The PushBullet service.</param>
    /// <param name="pushHoverService">The pushHover service.</param>
    public SettingsViewModel(
        KeyboardHelper keyboardHelper,
        SettingsService settingsService,
        SoundService soundService,
        HotkeyService hotkeyService,
        GithubService githubService,
        PushBulletService pushBulletService,
        PushHoverService pushHoverService,
        WinookService winookService,
        PoeLurkerPatreonService patreonService)
    {
        _currentPatreonService = patreonService;
        _winookService = winookService;
        _keyboardHelper = keyboardHelper;
        _settingService = settingsService;
        _hotkeyService = hotkeyService;
        _soundService = soundService;
        DisplayName = "Settings";
        _excludePropertyNames = GetExcludedPropertyNames();
        PropertyChanged += SettingsViewModel_PropertyChanged;

        BuildManager = new BuildManagerViewModel(ShowMessage, githubService);

        PushProviders = new ObservableCollection<PushProviderViewModel>();
        var pushBulletViewModel = new PushProviderViewModel("Pushbullet", pushBulletService);
        var pushHoverViewModel = new PushProviderViewModel("Pushover", pushHoverService);
        PushProviders.Add(pushBulletViewModel);
        PushProviders.Add(pushHoverViewModel);
        SelectedPushProvider = pushHoverService.Enable ? pushHoverViewModel : pushBulletViewModel;

        SetupHotkeys();
    }

    #endregion

    #region Events

    /// <summary>
    /// Raises the Close event.
    /// </summary>
    public event EventHandler OnClose;

    #endregion

    #region Properties

    /// <summary>
    /// Gets a value indicating whether HasHotkeys.
    /// </summary>
    public bool HasHotkeys => Hotkeys.Any(h => h.HasKeyCode) || MainHotkey.HasKeyCode;

    /// <summary>
    /// Gets the version.
    /// </summary>
    public string Version
    {
        get
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
    }

    /// <summary>
    /// Gets or Sets the PushBullet.
    /// </summary>
    public PushProviderViewModel SelectedPushProvider
    {
        get
        {
            return _selectedPushprovider;
        }

        set
        {
            _selectedPushprovider = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange(() => PushProviderViewModel);
        }
    }

    /// <summary>
    /// Gets the push provider.
    /// </summary>
    public PropertyChangedBase PushProviderViewModel => SelectedPushProvider.GetViewModel();

    /// <summary>
    /// Gets or sets the main hot key.
    /// </summary>
    public HotkeyViewModel MainHotkey { get; set; }

    /// <summary>
    /// Gets or sets the main hot key.
    /// </summary>
    public HotkeyViewModel OpenWikiHotkey { get; set; }

    /// <summary>
    /// Gets or sets the main hot key.
    /// </summary>
    public HotkeyViewModel GuildHideoutHotkey { get; set; }

    /// <summary>
    /// Gets or sets the main hot key.
    /// </summary>
    public HotkeyViewModel HideoutHotkey { get; set; }

    /// <summary>
    /// Gets or sets the main hot key.
    /// </summary>
    public HotkeyViewModel MonsterRemainingHotkey { get; set; }

    /// <summary>
    /// Gets or sets the search hot key.
    /// </summary>
    public HotkeyViewModel SearchItemHotkey { get; set; }

    /// <summary>
    /// Gets or sets the hotkeys.
    /// </summary>
    public ObservableCollection<HotkeyViewModel> Hotkeys { get; set; }

    /// <summary>
    /// Gets or sets the PushProviders.
    /// </summary>
    public ObservableCollection<PushProviderViewModel> PushProviders { get; set; }

    /// <summary>
    /// Gets the toggle build key value.
    /// </summary>
    public string ToggleBuildKeyValue => ConvertKeyCode(_hotkeyService.ToggleBuild);

    /// <summary>
    /// Gets or sets a value indicating whether this instance is character open.
    /// </summary>
    public bool IsCharacterOpen
    {
        get
        {
            return _isCharacterOpen;
        }

        set
        {
            _isCharacterOpen = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is character open.
    /// </summary>
    public bool IsPushBulletOpen
    {
        get
        {
            return _isPushBulletOpen;
        }

        set
        {
            _isPushBulletOpen = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance has custom trade sound.
    /// </summary>
    public bool HasCustomTradeSound
    {
        get
        {
            return _hasCustomTradeSound;
        }

        set
        {
            _hasCustomTradeSound = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether HasCustom sound for Item.
    /// </summary>
    public bool HasCustomItemSound
    {
        get
        {
            return _hasCustomItemSound;
        }

        set
        {
            _hasCustomItemSound = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the build manager.
    /// </summary>
    /// <value>The build manager.</value>
    public BuildManagerViewModel BuildManager { get; set; }

    /// <summary>
    /// Gets the character manager.
    /// </summary>
    public CharacterManagerViewModel CharacterManager
    {
        get
        {
            return _characterManager;
        }

        private set
        {
            _characterManager = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the index of the select teb.
    /// </summary>
    public int SelectTabIndex
    {
        get
        {
            return _selectedTabIndex;
        }

        set
        {
            _selectedTabIndex = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="SettingsViewModel"/> is saved.
    /// </summary>
    public bool Modified
    {
        get
        {
            return _modified;
        }

        private set
        {
            _modified = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets the animation file path.
    /// </summary>
    public string AnimationFilePath => AssetService.GetFilePath(LottieFileName);

    /// <summary>
    /// Gets the blessing text.
    /// </summary>
    public string BlessingText
    {
        get
        {
            return _blessingtext;
        }

        private set
        {
            _blessingtext = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the color of the lifebar.
    /// </summary>
    public Color LifebarColor
    {
        get
        {
            return (Color)ColorConverter.ConvertFromString(_settingService.LifeForeground);
        }

        set
        {
            _settingService.LifeForeground = value.ToString();
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the tradebar scaling.
    /// </summary>
    public double TradebarScaling
    {
        get
        {
            return _settingService.TradebarScaling;
        }

        set
        {
            _settingService.TradebarScaling = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the delay to close outgoing trades.
    /// </summary>
    public double DelayToClose
    {
        get
        {
            return _settingService.OutgoingDelayToClose;
        }

        set
        {
            _settingService.OutgoingDelayToClose = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [show startup animation].
    /// </summary>
    public bool ShowStartupAnimation
    {
        get
        {
            return _settingService.ShowStartupAnimation;
        }

        set
        {
            _settingService.ShowStartupAnimation = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="SettingsViewModel"/> is connected.
    /// </summary>
    public bool NotConnected
    {
        get
        {
            return !new TokenService().Connected;
        }
    }

    /// <summary>
    /// Gets the patreon identifier.
    /// </summary>
    public string PatreonId => new TokenService().PatreonId;

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="SettingsViewModel"/> is pledging.
    /// </summary>
    public bool Pledging
    {
        get
        {
            return _pledging;
        }

        set
        {
            _pledging = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange("NotPledging");
            NotifyOfPropertyChange("ConnectedWithoutPledging");
        }
    }

    /// <summary>
    /// Gets a value indicating whether [not pledgin].
    /// </summary>
    public bool NotPledging => !Pledging;

    /// <summary>
    /// Gets a value indicating whether [connected without pledge].
    /// </summary>
    public bool ConnectedWithoutPledging => !NotConnected && NotPledging;

    /// <summary>
    /// Gets or sets a value indicating whether [needs update].
    /// </summary>
    public bool NeedsUpdate
    {
        get
        {
            return _needsUpdate;
        }

        set
        {
            _needsUpdate = value;
            NotifyOfPropertyChange();
            NotifyOfPropertyChange("UpToDate");
        }
    }

    /// <summary>
    /// Gets a value indicating whether [up to date].
    /// </summary>
    public bool UpToDate => !NeedsUpdate;

    /// <summary>
    /// Gets or sets the busy message.
    /// </summary>
    public string BusyMessage
    {
        get
        {
            return _settingService.BusyMessage;
        }

        set
        {
            _settingService.BusyMessage = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [incoming trade enabled].
    /// </summary>
    public bool IncomingTradeEnabled
    {
        get
        {
            return _settingService.IncomingTradeEnabled;
        }

        set
        {
            _settingService.IncomingTradeEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [outgoing trade enabled].
    /// </summary>
    public bool OutgoingTradeEnabled
    {
        get
        {
            return _settingService.OutgoingTradeEnabled;
        }

        set
        {
            _settingService.OutgoingTradeEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [dashboard enabled].
    /// </summary>
    public bool DashboardEnabled
    {
        get
        {
            return _settingService.DashboardEnabled;
        }

        set
        {
            _settingService.DashboardEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the sold message.
    /// </summary>
    public string SoldMessage
    {
        get
        {
            return _settingService.SoldMessage;
        }

        set
        {
            _settingService.SoldMessage = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the thank you message.
    /// </summary>
    public string ThankYouMessage
    {
        get
        {
            return _settingService.ThankYouMessage;
        }

        set
        {
            _settingService.ThankYouMessage = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the still interested message.
    /// </summary>
    public string StillInterestedMessage
    {
        get
        {
            return _settingService.StillInterestedMessage;
        }

        set
        {
            _settingService.StillInterestedMessage = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [alert enabled].
    /// </summary>
    public bool AlertEnabled
    {
        get
        {
            return _settingService.AlertEnabled;
        }

        set
        {
            _settingService.AlertEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [alert enabled].
    /// </summary>
    public bool ItemAlertEnabled
    {
        get
        {
            return _settingService.ItemAlertEnabled;
        }

        set
        {
            _settingService.ItemAlertEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [join hideout enabled].
    /// </summary>
    public bool JoinHideoutEnabled
    {
        get
        {
            return _settingService.JoinHideoutEnabled;
        }

        set
        {
            _settingService.JoinHideoutEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [alert enabled].
    /// </summary>
    public bool SearchEnabled
    {
        get
        {
            return _settingService.SearchEnabled;
        }

        set
        {
            _settingService.SearchEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [map enabled].
    /// </summary>
    public bool MapEnabled
    {
        get
        {
            return _settingService.MapEnabled;
        }

        set
        {
            _settingService.MapEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [clipboard enabled].
    /// </summary>
    public bool ClipboardEnabled
    {
        get
        {
            return _settingService.ClipboardEnabled;
        }

        set
        {
            _settingService.ClipboardEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [automatic kick enabled].
    /// </summary>
    public bool AutoKickEnabled
    {
        get
        {
            return _settingService.AutoKickEnabled;
        }

        set
        {
            _settingService.AutoKickEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [debug enabled].
    /// </summary>
    public bool DebugEnabled
    {
        get
        {
            return _settingService.DebugEnabled;
        }

        set
        {
            _settingService.DebugEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the alert volume.
    /// </summary>
    public int AlertVolume
    {
        get
        {
            return _alertVolume;
        }

        set
        {
            _alertVolume = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the alert volume.
    /// </summary>
    public int ItemAlertVolume
    {
        get
        {
            return _itemAlertVolume;
        }

        set
        {
            _itemAlertVolume = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the join hideout volume.
    /// </summary>
    public int JoinHideoutVolume
    {
        get
        {
            return _joinHideoutVolume;
        }

        set
        {
            _joinHideoutVolume = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [tool tip enabled].
    /// </summary>
    public bool ToolTipEnabled
    {
        get
        {
            return _settingService.ToolTipEnabled;
        }

        set
        {
            _settingService.ToolTipEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets the tool tip delay.
    /// </summary>
    public int ToolTipDelay
    {
        get
        {
            return _settingService.ToolTipDelay;
        }

        set
        {
            _settingService.ToolTipDelay = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [always visible].
    /// </summary>
    public bool AlwaysVisible
    {
        get
        {
            return !_settingService.HideInBackground;
        }

        set
        {
            _settingService.HideInBackground = !value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [delete item enabled].
    /// </summary>
    public bool DeleteItemEnabled
    {
        get
        {
            return _settingService.DeleteItemEnabled;
        }

        set
        {
            _settingService.DeleteItemEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [vulkan renderer].
    /// </summary>
    public bool VulkanRenderer
    {
        get
        {
            return _settingService.VulkanRenderer;
        }

        set
        {
            _settingService.VulkanRenderer = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [build helper].
    /// </summary>
    public bool BuildHelper
    {
        get
        {
            return _settingService.BuildHelper;
        }

        set
        {
            _settingService.BuildHelper = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [sold detection].
    /// </summary>
    public bool SoldDetection
    {
        get
        {
            return _settingService.SoldDetection;
        }

        set
        {
            _settingService.SoldDetection = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [show release note].
    /// </summary>
    public bool ShowReleaseNote
    {
        get
        {
            return _settingService.ShowReleaseNote;
        }

        set
        {
            _settingService.ShowReleaseNote = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [synchronize build].
    /// </summary>
    public bool SyncBuild
    {
        get
        {
            return _settingService.SyncBuild;
        }

        set
        {
            _settingService.SyncBuild = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [hideout enabled].
    /// </summary>
    public bool HideoutEnabled
    {
        get
        {
            return _settingService.HideoutEnabled;
        }

        set
        {
            _settingService.HideoutEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [hideout enabled].
    /// </summary>
    public bool GuildHideoutEnabled
    {
        get
        {
            return _settingService.GuildHideoutEnabled;
        }

        set
        {
            _settingService.GuildHideoutEnabled = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [IgnoreAlreadySold].
    /// </summary>
    public bool IgnoreAlreadySold
    {
        get
        {
            return _settingService.IgnoreAlreadySold;
        }

        set
        {
            _settingService.IgnoreAlreadySold = value;
            NotifyOfPropertyChange();
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Reset all hotkeys.
    /// </summary>
    public async void ResetHotkeys()
    {
        var result = await ShowMessage("Wait!", "You are about to reset all hotkeys.", MessageDialogStyle.AffirmativeAndNegative);
        if (result == MessageDialogResult.Negative)
        {
            return;
        }

        MainHotkey.Remove();

        foreach (var hotkey in Hotkeys)
        {
            hotkey.Remove();
        }
    }

    /// <summary>
    /// Set the tab index to lurker pro.
    /// </summary>
    public void OpenLurkerPro()
    {
        SelectTabIndex = 6;
    }

    /// <summary>
    /// Opens the logs.
    /// </summary>
    public void OpenLogs()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var folderName = Path.Combine(localAppData, "PoeLurker", $"app-{Version}/logs");

        if (Directory.Exists(folderName))
        {
            Process.Start(folderName);
        }
    }

    /// <summary>
    /// Opens the discord.
    /// </summary>
    public void OpenDiscord()
        => OpenUrl("https://discord.com/invite/hQERv7K");

    /// <summary>
    /// Opens the patreon.
    /// </summary>
    public void OpenPatreon()
        => OpenUrl("https://www.patreon.com/poelurker");

    /// <summary>
    /// Activates the window.
    /// </summary>
    public void ActivateWindow()
    {
        if (_view == null)
        {
            return;
        }

        _view.WindowState = WindowState.Normal;
        _view.Activate();
    }

    /// <summary>
    /// Sets the toggle build key code.
    /// </summary>
    /// <returns>The task awaiter.</returns>
    public async Task SetToggleBuildKeyCode()
    {
        if (_keyboardWaiting)
        {
            return;
        }

        _keyboardWaiting = true;
        var task = _winookService.GetNextKeyAsync();
        await ShowProgress("Waiting input for...", "Toggle build helper", task);

        var value = await task;
        _hotkeyService.ToggleBuild = value.Key;
        NotifyOfPropertyChange(() => ToggleBuildKeyValue);
        _keyboardWaiting = false;
    }

    /// <summary>
    /// Gets the next key code.
    /// </summary>
    /// <param name="description">The description.</param>
    /// <returns>The key.</returns>
    private async Task<(KeyCode Key, Modifiers Modifier)> GetNextKeyCode(string description)
    {
        if (_keyboardWaiting)
        {
            return default;
        }

        _keyboardWaiting = true;
        var task = _winookService.GetNextKeyAsync();
        await ShowProgress("Waiting input for...", description, task);

        var key = await task;
        _keyboardWaiting = false;

        return key;
    }

    /// <summary>
    /// Opens the characters.
    /// </summary>
    public void OpenCharacters()
    {
        CharacterManager = new CharacterManagerViewModel(ShowMessage);
        IsCharacterOpen = true;
    }

    /// <summary>
    /// Selects the custom sound.
    /// </summary>
    public void SelectTradeSound()
    {
        if (!HasCustomTradeSound)
        {
            try
            {
                if (_currentSound != null)
                {
                    _currentSound.Stop();
                    _currentSound = null;
                }

                AssetService.Delete(SoundService.TradeAlertFileName);
            }
            catch
            {
                // Sound was playing.
                HasCustomTradeSound = true;
            }

            return;
        }

        var openFileDialog = new OpenFileDialog
        {
            Filter = "MP3 files (*.mp3)|*.mp3",
        };

        if (openFileDialog.ShowDialog() == true)
        {
            var fileName = openFileDialog.FileName;
            if (!File.Exists(fileName))
            {
                return;
            }

            var content = File.ReadAllBytes(fileName);
            AssetService.Create(SoundService.TradeAlertFileName, content);
        }
        else
        {
            HasCustomTradeSound = false;
        }
    }

    /// <summary>
    /// Select the good item sound.
    /// </summary>
    public void SelectItemSound()
    {
        if (!HasCustomItemSound)
        {
            try
            {
                if (_currentSound != null)
                {
                    _currentSound.Stop();
                    _currentSound = null;
                }

                AssetService.Delete(SoundService.ItemAlertFileName);
            }
            catch
            {
                // Sound was playing.
                HasCustomItemSound = true;
            }

            return;
        }

        var openFileDialog = new OpenFileDialog
        {
            Filter = "MP3 files (*.mp3)|*.mp3",
        };

        if (openFileDialog.ShowDialog() == true)
        {
            var fileName = openFileDialog.FileName;
            if (!File.Exists(fileName))
            {
                return;
            }

            var content = File.ReadAllBytes(fileName);
            AssetService.Create(SoundService.ItemAlertFileName, content);
        }
        else
        {
            HasCustomItemSound = false;
        }
    }

    /// <summary>
    /// Opens the dashboard.
    /// </summary>
    public void OpenDashboard()
    {
        //var dashBoard = IoC.Get<DashboardViewModel>();
        //var eventAggregator = IoC.Get<IEventAggregator>();
        //eventAggregator.PublishOnUIThreadAsync(dashBoard);
    }

    /// <summary>
    /// Gets the patreon identifier.
    /// </summary>
    public Task GetPatreonId()
    {
        if (string.IsNullOrEmpty(PatreonId))
        {
            return Task.CompletedTask;
        }

        return ClipboardService.SetTextAsync(PatreonId);
    }

    /// <summary>
    /// Inserts the buyer name token.
    /// </summary>
    public Task InsertBuyerNameToken()
        => _keyboardHelper.WriteAsync(TokenHelper.BuyerName);

    /// <summary>
    /// Inserts the buyer name token.
    /// </summary>
    public Task InsertLocationToken()
        => _keyboardHelper.WriteAsync(TokenHelper.Location);

    /// <summary>
    /// Inserts the price token.
    /// </summary>
    public Task InsertPriceToken()
        => _keyboardHelper.WriteAsync(TokenHelper.Price);

    /// <summary>
    /// Inserts the item name token.
    /// </summary>
    public Task InsertItemNameToken()
        => _keyboardHelper.WriteAsync(TokenHelper.ItemName);

    /// <summary>
    /// Updates this instance.
    /// </summary>
    public async void Update()
    {
        var updateManager = IoC.Get<UpdateManager>();
        await updateManager.Update();
    }

    /// <summary>
    /// Users the guide.
    /// </summary>
    public void UserGuide()
        => OpenUrl(@"https://docs.google.com/presentation/d/1XhaSSNAFGxzouc5amzAW8c_6ifToNjnsQq5UmNgLXoo/present?slide=id.p");

    /// <summary>
    /// Cheats the sheet.
    /// </summary>
    public void CheatSheet()
        => OpenUrl(@"https://github.com/C1rdec/Poe-Lurker/blob/master/assets/CheatSheet.md");

    /// <summary>
    /// Logins to patreon.
    /// </summary>
    public async Task LoginToPatreon()
    {
        await _currentPatreonService.CheckPledgeStatus();
        Pledging = _currentPatreonService.Pledging;
        if (Pledging)
        {
            BlessingText = "A blessing I can’t deny";
            SearchEnabled = true;
            MapEnabled = true;
            DashboardEnabled = true;
        }
    }

    /// <summary>
    /// Saves the settings.
    /// </summary>
    public async void SaveSettings()
    {
        await ShowProgress("Hold on", "Saving setings...", () =>
        {
            _settingService.Save();
            _hotkeyService.Save();
        });
        Modified = false;
    }

    /// <summary>
    /// Pledges this instance.
    /// </summary>
    public async void Pledge()
    {
        if (Keyboard.IsKeyDown(Key.LeftCtrl))
        {
            await GetPatreonId();

            return;
        }

        OpenUrl("https://www.patreon.com/poelurker");
    }

    /// <summary>
    /// Open push bullet flyout.
    /// </summary>
    public void OpenPushBullet()
    {
        IsPushBulletOpen = true;
    }

    /// <summary>
    /// Called when an attached view's Loaded event fires.
    /// </summary>
    /// <param name="view">The view.</param>
    protected override void OnViewLoaded(object view)
    {
        _view = view as MetroWindow;
        ActivateWindow();
        base.OnViewLoaded(view);
    }

    /// <summary>
    /// Called when deactivating.
    /// </summary>
    /// <param name="close">Inidicates whether this instance will be closed.</param>
    protected override async Task OnDeactivateAsync(bool close, CancellationToken token)
    {
        if (close)
        {
            if (_activateTask != null)
            {
                await _activateTask;
            }

            _settingService.Save();
            OnClose?.Invoke(this, EventArgs.Empty);
            _activated = false;
            SelectTabIndex = 0;
            Modified = false;
        }

        await base.OnDeactivateAsync(close, token);
    }

    /// <summary>
    /// Called when activating.
    /// </summary>
    protected override async Task OnActivateAsync(CancellationToken token)
    {
        HasCustomTradeSound = _soundService.HasCustomTradeAlert();
        HasCustomItemSound = _soundService.HasCustomItemAlert();

        await _currentPatreonService.CheckPledgeStatus();
        Pledging = _currentPatreonService.Pledging;
        if (Pledging)
        {
            BlessingText = "A blessing I can’t deny";
        }
        else
        {
            SearchEnabled = false;
            DashboardEnabled = false;
            MapEnabled = false;
        }

        AlertVolume = (int)(_settingService.AlertVolume * 100);
        ItemAlertVolume = (int)(_settingService.ItemAlertVolume * 100);
        JoinHideoutVolume = (int)(_settingService.JoinHideoutVolume * 100);
        CheckForUpdate();
        _activated = true;

        await base.OnActivateAsync(token);
    }

    /// <summary>
    /// Converts the key code.
    /// </summary>
    /// <param name="keyCode">The key code.</param>
    /// <returns>The key value.</returns>
    private static string ConvertKeyCode(KeyCode keyCode)
    {
        return keyCode.ToString();
    }

    /// <summary>
    /// Get the excluded property name for PropertyChanged event.
    /// </summary>
    /// <returns>The list of property names.</returns>
    private IEnumerable<string> GetExcludedPropertyNames()
    {
        return new List<string>()
        {
            nameof(PushProviderViewModel),
            nameof(SelectedPushProvider),
            nameof(IsCharacterOpen),
            nameof(IsPushBulletOpen),
            nameof(Modified),
            nameof(CharacterManager),
            nameof(SelectTabIndex),
            nameof(IsActive),
            nameof(NeedsUpdate),
            nameof(UpToDate),
        };
    }

    /// <summary>
    /// Setups the hotkeys.
    /// </summary>
    private void SetupHotkeys()
    {
        MainHotkey = new HotkeyViewModel("Invite & Trade", _hotkeyService.Main, GetNextKeyCode);
        OpenWikiHotkey = new HotkeyViewModel("Poe Ninja", _hotkeyService.OpenWiki, GetNextKeyCode);

        GuildHideoutHotkey = new HotkeyViewModel("Guild Hideout", _hotkeyService.JoinGuildHideout, GetNextKeyCode);
        HideoutHotkey = new HotkeyViewModel("Hideout", _hotkeyService.JoinHideout, GetNextKeyCode);

        MonsterRemainingHotkey = new HotkeyViewModel("Remaining Monster", _hotkeyService.RemainingMonster, GetNextKeyCode);
        SearchItemHotkey = new HotkeyViewModel("Item Highlight", _hotkeyService.SearchItem, GetNextKeyCode);

        Hotkeys = new ObservableCollection<HotkeyViewModel>
        {
            new HotkeyViewModel("Whisper", _hotkeyService.Whisper, GetNextKeyCode),
            new HotkeyViewModel("Busy", _hotkeyService.Busy, GetNextKeyCode),
            new HotkeyViewModel("Dismiss", _hotkeyService.Dismiss, GetNextKeyCode),
        };

        foreach (var hotkey in Hotkeys)
        {
            hotkey.PropertyChanged += Hotkey_PropertyChanged;
        }

        MainHotkey.PropertyChanged += Hotkey_PropertyChanged;
        OpenWikiHotkey.PropertyChanged += Hotkey_PropertyChanged;
        MonsterRemainingHotkey.PropertyChanged += Hotkey_PropertyChanged;
        SearchItemHotkey.PropertyChanged += Hotkey_PropertyChanged;
        GuildHideoutHotkey.PropertyChanged += Hotkey_PropertyChanged;
        HideoutHotkey.PropertyChanged += Hotkey_PropertyChanged;
    }

    /// <summary>
    /// Handles the PropertyChanged event of the Hotkey control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void Hotkey_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        NotifyOfPropertyChange(() => HasHotkeys);
        NotifyOfPropertyChange("Dirty");
    }

    /// <summary>
    /// Checks for update.
    /// </summary>
    private async void CheckForUpdate()
    {
        var updateManager = IoC.Get<UpdateManager>();
        //this.NeedsUpdate = await updateManager.CheckForUpdate();
    }

    /// <summary>
    /// Handles the PropertyChanged event of the SettingsViewModel control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
    private void SettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (!_excludePropertyNames.Contains(e.PropertyName))
        {
            if (!_activated)
            {
                return;
            }

            Logger.Trace($"Modifed: {e.PropertyName}");
            Modified = true;
        }

        if (e.PropertyName == nameof(AlertVolume))
        {
            _settingService.AlertVolume = (float)AlertVolume / 100;

            if (!_activated)
            {
                return;
            }

            if (_currentTokenSource != null)
            {
                _currentTokenSource.Cancel();
                _currentTokenSource.Dispose();
                _currentTokenSource = null;
            }

            _currentTokenSource = new CancellationTokenSource();
            PlaySoundTest(_currentTokenSource.Token, () =>
            {
                _currentSound = _soundService.PlayTradeAlert(_settingService.AlertVolume);
            });
        }
        else if (e.PropertyName == nameof(ItemAlertVolume))
        {
            _settingService.ItemAlertVolume = (float)ItemAlertVolume / 100;

            if (!_activated)
            {
                return;
            }

            if (_currentTokenSource != null)
            {
                _currentTokenSource.Cancel();
                _currentTokenSource.Dispose();
                _currentTokenSource = null;
            }

            _currentTokenSource = new CancellationTokenSource();
            PlaySoundTest(_currentTokenSource.Token, () =>
            {
                _currentSound = _soundService.PlayItemAlert(_settingService.ItemAlertVolume);
            });
        }
        else if (e.PropertyName == nameof(JoinHideoutVolume))
        {
            _settingService.JoinHideoutVolume = (float)JoinHideoutVolume / 100;

            if (!_activated)
            {
                return;
            }

            if (_currentTokenSource != null)
            {
                _currentTokenSource.Cancel();
                _currentTokenSource.Dispose();
                _currentTokenSource = null;
            }

            _currentTokenSource = new CancellationTokenSource();
            PlaySoundTest(_currentTokenSource.Token, () => _soundService.PlayJoinHideout(_settingService.JoinHideoutVolume));
        }
    }

    /// <summary>
    /// Plays the sound test.
    /// </summary>
    /// <param name="token">The token.</param>
    /// <param name="callback">The callback.</param>
    private async void PlaySoundTest(CancellationToken token, System.Action callback)
    {
        await Task.Delay(300);
        if (token.IsCancellationRequested)
        {
            return;
        }

        callback();
    }

    /// <summary>
    /// Shows the progress.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <param name="action">The action.</param>
    private async Task ShowProgress(string title, string message, System.Action action)
    {
        var coordinator = DialogCoordinator.Instance;
        var controller = await coordinator.ShowProgressAsync(this, title, message);
        controller.SetIndeterminate();

        await Task.Run(() =>
        {
            action?.Invoke();
        });

        await controller.CloseAsync();
    }

    /// <summary>
    /// Shows the progress.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <param name="task">The task.</param>
    private async Task ShowProgress(string title, string message, Task task)
    {
        var coordinator = DialogCoordinator.Instance;
        var controller = await coordinator.ShowProgressAsync(this, title, message);
        controller.SetIndeterminate();

        await task;

        await controller.CloseAsync();
    }

    /// <summary>
    /// Shows the message.
    /// </summary>
    /// <param name="title">The title.</param>
    /// <param name="message">The message.</param>
    /// <param name="style">The style.</param>
    /// <returns>The result.</returns>
    private Task<MessageDialogResult> ShowMessage(string title, string message, MessageDialogStyle? style)
    {
        var coordinator = DialogCoordinator.Instance;
        if (style.HasValue)
        {
            return coordinator.ShowMessageAsync(this, title, message, style.Value);
        }
        else
        {
            return coordinator.ShowMessageAsync(this, title, message);
        }
    }

    private void OpenUrl(string url)
        => ProcessExtensions.OpenUrl(url);

    #endregion
}