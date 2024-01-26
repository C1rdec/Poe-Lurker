//-----------------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.UI.ViewModels;

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using PoeLurker.Core;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;
using PoeLurker.Patreon.Events;
using PoeLurker.Patreon.Models;
using PoeLurker.Patreon.Services;
using PoeLurker.UI.Helpers;
using PoeLurker.UI.Models;
using PoeLurker.UI.Services;
using PoeLurker.UI.Views;
using ProcessLurker;

/// <summary>
/// Represents the SHellViewModel.
/// </summary>
public class ShellViewModel : Conductor<Screen>.Collection.AllActive, IViewAware, IHandle<Screen>, IHandle<SkillTimelineMessage>
{
    #region Fields

    private Window _parent;
    private readonly WinookService _winookService;
    private readonly SoundService _soundService;
    private PlayerViewModel _activePlayer;
    private readonly SimpleContainer _container;
    private ProcessService _processLurker;
    private ClientLurker _currentLurker;
    private PlayerService _currentCharacterService;
    private MouseLurker _mouseLurker;
    private KeyboardLurker _keyboardLurker;
    private DockingHelper _currentDockingHelper;
    private ClipboardLurker _clipboardLurker;
    private TradebarViewModel _incomingTradeBarOverlay;
    private StashTabGridViewModel _stashTabGrid;
    private BuildTimelineViewModel _skillTimelineOverlay;
    private OutgoingbarViewModel _outgoingTradeBarOverlay;
    private PopupViewModel _popup;
    private LifeBulbViewModel _lifeBulbOverlay;
    private ManaBulbViewModel _manaBulbOverlay;
    private HideoutViewModel _hideoutOverlay;
    private WikiViewModel _wikiViewModel;
    private readonly SettingsService _settingsService;
    private readonly HotkeyService _keyCodeService;
    private AfkService _afkService;
    private readonly BuildService _buildService;
    private readonly SettingsViewModel _settingsViewModel;
    private BuildViewModel _buildViewModel;
    private HelpViewModel _helpOverlay;
    private readonly IEventAggregator _eventAggregator;
    private bool _startWithWindows;
    private bool _needUpdate;
    private bool _showInTaskBar;
    private bool _showUpdateSuccess;
    private bool _closing;
    private Task _openingTask;
    private IWindowManager _windowManager;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel" /> class.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="settingsService">The settings service.</param>
    /// <param name="keyCodeService">The key code service.</param>
    /// <param name="buildService">The build service.</param>
    /// <param name="settingsViewModel">The settings view model.</param>
    /// <param name="soundService">The sound service.</param>
    /// <param name="eventAggregator">The event aggregator.</param>
    public ShellViewModel(
        SimpleContainer container, 
        SettingsService settingsService, 
        HotkeyService keyCodeService, 
        BuildService buildService, 
        SoundService soundService,
        WinookService winookService,
        SettingsViewModel settingsViewModel,
        IEventAggregator eventAggregator,
        IWindowManager windowManager)
    {
        _windowManager = windowManager;
        _winookService = winookService;
        this._soundService = soundService;
        this._eventAggregator = eventAggregator;
        this._settingsService = settingsService;
        this._keyCodeService = keyCodeService;
        this._buildService = buildService;
        this._container = container;
        this._settingsViewModel = settingsViewModel;

        this._openingTask = this.WaitForPoe(false);
        this.StartWithWindows = File.Exists(this.ShortcutFilePath);
        this.ShowInTaskBar = true;
        this._settingsService.OnSave += this.SettingsService_OnSave;
        if (settingsService.FirstLaunch)
        {
            if (this.StartWithWindows)
            {
                // RefreshShortcut
                File.Delete(this.ShortcutFilePath);
                this.CreateLink();
            }

            settingsService.FirstLaunch = false;
            this._showUpdateSuccess = true;
            settingsService.Save(false);

            if (settingsService.ShowReleaseNote)
            {
                Process.Start("https://github.com/C1rdec/Poe-Lurker/releases/latest");
            }
        }

        // this.ActivateItem(IoC.Get<TutorialViewModel>());
        this._eventAggregator.SubscribeOnUIThread(this);
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets the active player.
    /// </summary>
    public PlayerViewModel ActivePlayer
    {
        get
        {
            return _activePlayer;
        }

        private set
        {
            if (_activePlayer != value)
            {
                _activePlayer = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange("ActivePlayerVisible");
            }
        }
    }

    /// <summary>
    /// Gets a value indicating whether [active player visible].
    /// </summary>
    public bool ActivePlayerVisible => ActivePlayer != null;

    /// <summary>
    /// Gets the command.
    /// </summary>
    public DoubleClickCommand ShowSettingsCommand => new DoubleClickCommand(ShowSettings);

    /// <summary>
    /// Gets or sets a value indicating whether [show in task bar].
    /// </summary>
    public bool ShowInTaskBar
    {
        get
        {
            return _showInTaskBar;
        }

        set
        {
            _showInTaskBar = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [start with windows].
    /// </summary>
    public bool StartWithWindows
    {
        get
        {
            return _startWithWindows;
        }

        set
        {
            _startWithWindows = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [need update].
    /// </summary>
    public bool NeedUpdate
    {
        get
        {
            return _needUpdate;
        }

        set
        {
            _needUpdate = value;
            NotifyOfPropertyChange();
        }
    }

    /// <summary>
    /// Gets the name of the shortcut.
    /// </summary>
    public string ShortcutName => "PoeLurker.lnk";

    /// <summary>
    /// Gets the application data folder path.
    /// </summary>
    public string ApplicationDataFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    /// <summary>
    /// Gets the startup folder path.
    /// </summary>
    public string StartupFolderPath => Path.Combine(ApplicationDataFolderPath, @"Microsoft\Windows\Start Menu\Programs\Startup");

    /// <summary>
    /// Gets the shortcut file path.
    /// </summary>
    public string ShortcutFilePath => Path.Combine(StartupFolderPath, ShortcutName);

    /// <summary>
    /// Gets the version.
    /// </summary>
    public string Version => GetAssemblyVersion();

    #endregion

    #region Methods

    protected override async void OnViewLoaded(object view)
    {
        await Task.Delay(200);
        await _winookService.InstallAsync();

        ShowInTaskBar = false;
        HideFromAltTab(view as ShellView);
    }

    private void HideFromAltTab(Window view)
    {
        _parent = new Window
        {
            Top = -100,
            Left = -100,
            Width = 1,
            Height = 1,

            // Set window style as ToolWindow to avoid its icon in AltTab
            WindowStyle = System.Windows.WindowStyle.ToolWindow,
            ShowInTaskbar = false,
        };

        _parent.Show();
        view.Owner = _parent;
        _parent.Hide();
    }

    /// <summary>
    /// Closes this instance.
    /// </summary>
    public void Close()
    {
        _eventAggregator.Unsubscribe(this);
        CleanUp();
        TryCloseAsync();
    }

    /// <summary>
    /// Creates the short cut.
    /// </summary>
    public void CreateShortCut()
    {
        if (File.Exists(ShortcutFilePath))
        {
            File.Delete(ShortcutFilePath);
        }
        else
        {
            CreateLink();
        }

        StartWithWindows = !StartWithWindows;
    }

    /// <summary>
    /// When the system tray opens.
    /// </summary>
    public void OnTrayOpen()
    {
        // Hide mana overlay for 10s
        var message = new ManaBulbMessage()
        {
            NeedToHide = true,
        };

        _eventAggregator.PublishOnUIThreadAsync(message);
    }

    /// <summary>
    /// Gets the assembly version.
    /// </summary>
    /// <returns>The assembly version.</returns>
    private static string GetAssemblyVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var information = FileVersionInfo.GetVersionInfo(assembly.Location);
        var version = information.FileVersion.Remove(information.FileVersion.Length - 2);
        return version;
    }

    /// <summary>
    /// Updates this instance.
    /// </summary>
    public async void Update()
    {
        var message = new ManaBulbMessage()
        {
            IsUpdate = true,
            View = new UpdateViewModel(UpdateState.Working),
        };

        await _eventAggregator.PublishOnUIThreadAsync(message);
        CleanUp();

        ShowInTaskBar = false;
        var updateManager = IoC.Get<UpdateManager>();
        await updateManager.Update();
    }

    /// <summary>
    /// Shows the settings.
    /// </summary>
    public async void ShowSettings()
    {
        if (_settingsViewModel.IsActive)
        {
            _settingsViewModel.ActivateWindow();
            return;
        }

        //await ActivateItemAsync(_settingsViewModel);
        await _windowManager.ShowDialogAsync(_settingsViewModel);
    }

    /// <summary>
    /// Desactivate the item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="close">if set to <c>true</c> [close].</param>
    public override Task DeactivateItemAsync(Screen item, bool close, CancellationToken token)
    {
        if (item != null && item.IsActive)
        {
            Execute.OnUIThread(() => { base.DeactivateItemAsync(item, close); });
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Creates the link.
    /// </summary>
    private void CreateLink()
    {
        var link = (IShellLink)new ShellLink();
        link.SetDescription("PoeLurker");
        link.SetPath(System.Reflection.Assembly.GetExecutingAssembly().Location);
        var file = (IPersistFile)link;
        file.Save(ShortcutFilePath, false);
    }

    /// <summary>
    /// Handles the OnSave event of the SettingsService control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
    private async void SettingsService_OnSave(object sender, EventArgs e)
    {
        await CheckPledgeStatus();

        if (_settingsService.BuildHelper)
        {
            if (_helpOverlay == null && PoeApplicationContext.IsRunning)
            {
                _helpOverlay = _container.GetInstance<HelpViewModel>();
                _helpOverlay.Initialize(ToggleBuildHelper);
                await ActivateItemAsync(_helpOverlay);
                await ActivateItemAsync(_buildViewModel);

                if (_skillTimelineOverlay != null && _settingsService.TimelineEnabled)
                {
                    await ActivateItemAsync(_skillTimelineOverlay);
                }
            }
        }
        else
        {
            if (_helpOverlay != null)
            {
                await DeactivateItemAsync(_helpOverlay, true, CancellationToken.None);
                _helpOverlay = null;
            }

            if (_buildViewModel != null)
            {
                await DeactivateItemAsync(_buildViewModel, true, CancellationToken.None);
            }

            if (_skillTimelineOverlay != null)
            {
                await DeactivateItemAsync(_skillTimelineOverlay, true, CancellationToken.None);
            }
        }

        if (_settingsService.IncomingTradeEnabled)
        {
            await ActivateItemAsync(_incomingTradeBarOverlay);
        }
        else
        {
            await DeactivateItemAsync(_incomingTradeBarOverlay, true, CancellationToken.None);
        }

        if (_settingsService.OutgoingTradeEnabled)
        {
            await ActivateItemAsync(_outgoingTradeBarOverlay);
        }
        else
        {
            await DeactivateItemAsync(_outgoingTradeBarOverlay, true, CancellationToken.None);
        }

        if (_settingsService.HideoutEnabled)
        {
            await ActivateItemAsync(_hideoutOverlay);
        }
        else
        {
            await DeactivateItemAsync(_hideoutOverlay, true, CancellationToken.None);
        }
    }

    /// <summary>
    /// Registers the instances.
    /// </summary>
    private void ShowOverlays(int processId)
    {
        Execute.OnUIThread(() =>
        {
            _currentDockingHelper = new DockingHelper(processId, _settingsService);

            // Keyboard
            var keyboarHelper = new PoeKeyboardHelper(processId);
            _keyboardLurker = new KeyboardLurker(processId, _settingsService, _keyCodeService, keyboarHelper);
            _keyboardLurker.BuildToggled += KeyboardLurker_BuildToggled;
            _keyboardLurker.OpenWikiPressed += KeyboardLurker_OpenWikiPressed;

            // Mouse
            _mouseLurker = new MouseLurker(processId, _settingsService);
            _mouseLurker.ItemDetails += ShowItemDetails;
            _mouseLurker.ItemIdentified += ItemIdentified;

            // Clipboard
            _clipboardLurker = new ClipboardLurker();

            _container.RegisterInstance(typeof(ProcessService), null, _processLurker);
            _container.RegisterInstance(typeof(MouseLurker), null, _mouseLurker);
            _container.RegisterInstance(typeof(ClientLurker), null, _currentLurker);
            _container.RegisterInstance(typeof(PlayerService), null, _currentCharacterService);
            _container.RegisterInstance(typeof(ClipboardLurker), null, _clipboardLurker);
            _container.RegisterInstance(typeof(DockingHelper), null, _currentDockingHelper);
            _container.RegisterInstance(typeof(PoeKeyboardHelper), null, keyboarHelper);
            _container.RegisterInstance(typeof(KeyboardLurker), null, _keyboardLurker);

            _stashTabGrid = _container.GetInstance<StashTabGridViewModel>();
            _skillTimelineOverlay = _container.GetInstance<BuildTimelineViewModel>();
            _incomingTradeBarOverlay = _container.GetInstance<TradebarViewModel>();
            _outgoingTradeBarOverlay = _container.GetInstance<OutgoingbarViewModel>();
            _popup = _container.GetInstance<PopupViewModel>();
            _lifeBulbOverlay = _container.GetInstance<LifeBulbViewModel>();
            _manaBulbOverlay = _container.GetInstance<ManaBulbViewModel>();
            _afkService = _container.GetInstance<AfkService>();
            _hideoutOverlay = _container.GetInstance<HideoutViewModel>();
            _helpOverlay = _container.GetInstance<HelpViewModel>();
            _helpOverlay.Initialize(ToggleBuildHelper);
            _buildViewModel = _container.GetInstance<BuildViewModel>();
            _wikiViewModel = _container.GetInstance<WikiViewModel>();

            if (_settingsService.BuildHelper)
            {
                ActivateItemAsync(_buildViewModel);
            }

            if (_settingsService.BuildHelper)
            {
                if (_settingsService.TimelineEnabled)
                {
                    ActivateItemAsync(_skillTimelineOverlay);
                }

                ActivateItemAsync(_helpOverlay);
            }

            if (_settingsService.IncomingTradeEnabled)
            {
                ActivateItemAsync(_incomingTradeBarOverlay);
            }

            if (_settingsService.OutgoingTradeEnabled)
            {
                ActivateItemAsync(_outgoingTradeBarOverlay);
            }

            if (_settingsService.HideoutEnabled)
            {
                ActivateItemAsync(_hideoutOverlay);
            }

            ActivateItemAsync(_lifeBulbOverlay);
            ActivateItemAsync(_manaBulbOverlay);
            ActivateItemAsync(_stashTabGrid);
            ActivateItemAsync(_wikiViewModel);
        });
    }

    /// <summary>
    /// Handles the BuildToggled event of the KeyboardLurker control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private async void KeyboardLurker_OpenWikiPressed(object sender, Winook.KeyboardMessageEventArgs e)
    {
        if (_wikiViewModel != null)
        {
            await _wikiViewModel.Show();
        }
    }

    /// <summary>
    /// Handles the BuildToggled event of the KeyboardLurker control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void KeyboardLurker_BuildToggled(object sender, EventArgs e)
    {
        ToggleBuildHelper();
    }

    /// <summary>
    /// Toggles the build helper.
    /// </summary>
    private void ToggleBuildHelper()
    {
        _buildViewModel.IsOpen = !_buildViewModel.IsOpen;
    }

    /// <summary>
    /// Handles the PoeClosed event of the CurrentLurker control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    private async void PoeClosed(object sender, EventArgs e)
    {
        if (_openingTask != null)
        {
            await _openingTask;
        }

        CleanUp();
        _openingTask = WaitForPoe(true);
    }

    /// <summary>
    /// Cleans up.
    /// </summary>
    private void CleanUp()
    {
        ActivePlayer = null;
        _container.UnregisterHandler<ClientLurker>();
        _container.UnregisterHandler<PlayerService>();
        _container.UnregisterHandler<ProcessService>();
        _container.UnregisterHandler<DockingHelper>();
        _container.UnregisterHandler<PoeKeyboardHelper>();
        _container.UnregisterHandler<ClipboardLurker>();
        _container.UnregisterHandler<MouseLurker>();
        _container.UnregisterHandler<KeyboardLurker>();

        if (_clipboardLurker != null)
        {
            _clipboardLurker.Dispose();
            _clipboardLurker = null;
        }

        if (_currentLurker != null)
        {
            _currentLurker.AdminRequested -= CurrentLurker_AdminRequested;
            _currentLurker.LeagueChanged -= CurrentLurker_LeagueChanged;
            _currentLurker.Dispose();
            _currentLurker = null;
        }

        if (_currentCharacterService != null)
        {
            _currentCharacterService.Dispose();
        }

        if (_processLurker != null)
        {
            _processLurker.ProcessClosed -= PoeClosed;
            _processLurker.Dispose();
            _processLurker = null;
        }

        if (_currentDockingHelper != null)
        {
            _currentDockingHelper.Dispose();
            _currentDockingHelper = null;
        }

        if (_afkService != null)
        {
            _afkService.Dispose();
            _afkService = null;
        }

        if (_mouseLurker != null)
        {
            _mouseLurker.ItemDetails -= ShowItemDetails;
            _mouseLurker.ItemIdentified -= ItemIdentified;
            _mouseLurker.Dispose();
            _mouseLurker = null;
        }

        if (_keyboardLurker != null)
        {
            _keyboardLurker.OpenWikiPressed -= KeyboardLurker_OpenWikiPressed;
            _keyboardLurker.BuildToggled -= KeyboardLurker_BuildToggled;
            _keyboardLurker.Dispose();
        }

        if (_stashTabGrid != null)
        {
            _stashTabGrid.Dispose();
        }

        if (_incomingTradeBarOverlay != null)
        {
            _incomingTradeBarOverlay.Dispose();
        }
    }

    /// <summary>
    /// Waits for poe.
    /// </summary>
    private async Task WaitForPoe(bool fromClosing)
    {
        // Process Lurker
        _processLurker = new PathOfExileProcessLurker();
        var existingProcess = _processLurker.GetProcess();
        _processLurker.ProcessClosed += PoeClosed;
        var windowHandle = await _processLurker.WaitForProcess();

        _currentLurker = new ClientLurker(windowHandle);
        _currentLurker.AdminRequested += CurrentLurker_AdminRequested;
        _currentLurker.LeagueChanged += CurrentLurker_LeagueChanged;

        if (existingProcess != null)
        {
            Start(windowHandle);
        }
        else
        {
            EventHandler<LocationChangedEvent> handler = default;
            handler = (object s, LocationChangedEvent e) =>
            {
                Start(windowHandle);
                _currentLurker.LocationChanged -= handler;
            };

            _currentLurker.LocationChanged += handler;
        }
    }

    private async void Start(int process)
    {
        var affixServiceTask = AffixService.InitializeAsync();
        _currentCharacterService = new PlayerService(_currentLurker);
        ActivePlayer = new PlayerViewModel(_currentCharacterService);
        NotifyOfPropertyChange("ActivePlayer");

        if (_closing)
        {
            return;
        }

        ShowOverlays(process);

        // Initialize Github
        var githubService = IoC.Get<GithubService>();
        await githubService.Gems();
        await githubService.Uniques();

        await CheckForUpdate();
        await CheckPledgeStatus();

        await affixServiceTask;
    }

    /// <summary>
    /// Set the league name.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="leagueName">The league name.</param>
    private void CurrentLurker_LeagueChanged(object sender, string leagueName)
    {
        if (_settingsService.RecentLeagueName != leagueName)
        {
            _settingsService.RecentLeagueName = leagueName;
            _settingsService.Save();
        }
    }

    /// <summary>
    /// Handles the AdminRequested event of the CurrentLurker control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void CurrentLurker_AdminRequested(object sender, EventArgs e)
    {
        if (AdminRequestHelper.RequestAdmin())
        {
            _closing = true;
        }
    }

    /// <summary>
    /// Updates this instance.
    /// </summary>
    private async Task CheckForUpdate()
    {
        var updateManager = IoC.Get<UpdateManager>();
        //this.NeedUpdate = await updateManager.CheckForUpdate();
        if (NeedUpdate)
        {
            var message = new ManaBulbMessage()
            {
                IsUpdate = true,
                View = new UpdateViewModel(UpdateState.NeedUpdate),
                Action = () => Update(),
            };

            await _eventAggregator.PublishOnUIThreadAsync(message);

            return;
        }
        else if (_showUpdateSuccess)
        {
            _showUpdateSuccess = false;
            await _eventAggregator.PublishOnUIThreadAsync(new ManaBulbMessage() { IsUpdate = true, View = new UpdateViewModel(UpdateState.Success), DisplayTime = TimeSpan.FromSeconds(5) });
        }
        else
        {
            //using (var patreonService = new PatreonService())
            //{
            //    var isPledging = await patreonService.IsPledging();
            //    if (!isPledging)
            //    {
            //        using (var service = new CollaborationService())
            //        {
            //            var collaboration = await service.GetCollaborationAsync();
            //            if (!collaboration.IsExpired())
            //            {
            //                validCollaboration = collaboration;
            //            }
            //        }
            //    }

            //    if (validCollaboration != null)
            //    {
            //        await this._eventAggregator.PublishOnUIThreadAsync(new ManaBulbMessage() { View = new CollaborationViewModel(validCollaboration), Action = validCollaboration.Open, DisplayTime = TimeSpan.FromSeconds(6) });
            //    }
            //    else if (this._settingsService.ShowStartupAnimation || !isPledging)
            //    {
            //        await this._eventAggregator.PublishOnUIThreadAsync(new ManaBulbMessage() { View = new SplashscreenViewModel(this._settingsViewModel, this._eventAggregator), DisplayTime = TimeSpan.FromSeconds(10) });
            //    }
            //}
        }
    }

    private void ItemIdentified(object sender, PoeItem item)
    {
        if (_settingsService.MapEnabled && item is Map map)
        {
            ShowMap(map);
        }
        else if (_settingsService.ItemAlertEnabled && item.IsGood())
        {
            _soundService.PlayItemAlert(_settingsService.ItemAlertVolume);
        }
    }

    /// <summary>
    /// Shows the map.
    /// </summary>
    /// <param name="item">The item.</param>
    private void ShowMap(Map item)
    {
        if (!_popup.IsActive)
        {
            ActivateItemAsync(_popup);
        }

        if (_settingsService.MapEnabled)
        {
            if (item.Rarity == Rarity.Normal)
            {
                return;
            }

            _popup.Open(new MapViewModel(item, ActivePlayer, _currentCharacterService));
        }
    }

    /// <summary>
    /// Clipboards the lurker newitem.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="item">The item.</param>
    private void ShowItemDetails(object sender, PoeItem item)
    {
        if (!_popup.IsActive)
        {
            ActivateItemAsync(_popup);
        }

        if (item is Map || item.Rarity == Rarity.Currency || item.Rarity == Rarity.Unique)
        {
            return;
        }
        else if (item is Weapon weapon)
        {
            _popup.Open(new WeaponViewModel(weapon));
        }
        else
        {
            _popup.Open(new ItemOverlayViewModel(item));
        }
    }

    /// <summary>
    /// Handles the specified screen.
    /// </summary>
    /// <param name="screen">The screen.</param>
    public Task HandleAsync(Screen screen, CancellationToken token)
    {
        if (screen.IsActive)
        {
            return Task.CompletedTask;
        }

        return ActivateItemAsync(screen);
    }

    /// <summary>
    /// Handles the message.
    /// </summary>
    /// <param name="message">The message.</param>
    public Task HandleAsync(SkillTimelineMessage message, CancellationToken token)
    {
        if (message.IsVisible)
        {
            return ActivateItemAsync(_skillTimelineOverlay);
        }
        else
        {
            return DeactivateItemAsync(_skillTimelineOverlay, true, CancellationToken.None);
        }
    }

    /// <summary>
    /// Checks the pledge status.
    /// </summary>
    private async Task CheckPledgeStatus()
    {
        await ClipboardHelper.CheckPledgeStatusAsync();
    }

    #endregion
}

#pragma warning disable
[ComImport]
[Guid("00021401-0000-0000-C000-000000000046")]
internal class ShellLink
{
}

[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
[Guid("000214F9-0000-0000-C000-000000000046")]
internal interface IShellLink
{
    void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, out IntPtr pfd, int fFlags);
    void GetIDList(out IntPtr ppidl);
    void SetIDList(IntPtr pidl);
    void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszName, int cchMaxName);
    void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
    void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
    void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
    void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
    void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
    void GetHotkey(out short pwHotkey);
    void SetHotkey(short wHotkey);
    void GetShowCmd(out int piShowCmd);
    void SetShowCmd(int iShowCmd);
    void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszIconPath, int cchIconPath, out int piIcon);
    void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
    void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, int dwReserved);
    void Resolve(IntPtr hwnd, int fFlags);
    void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
}