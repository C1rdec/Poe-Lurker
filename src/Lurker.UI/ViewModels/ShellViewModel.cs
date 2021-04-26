//-----------------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Text;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Models;
    using Lurker.Patreon;
    using Lurker.Patreon.Models;
    using Lurker.Services;
    using Lurker.UI.Helpers;
    using Lurker.UI.Models;
    using Lurker.UI.Services;
    using Lurker.UI.ViewModels;

    /// <summary>
    /// Represents the SHellViewModel.
    /// </summary>
    public class ShellViewModel : Conductor<Screen>.Collection.AllActive, IViewAware, IHandle<Screen>, IHandle<SkillTimelineMessage>
    {
        #region Fields

        private PlayerViewModel _activePlayer;
        private SimpleContainer _container;
        private ProcessLurker _processLurker;
        private ClientLurker _currentLurker;
        private PlayerService _currentCharacterService;
        private MouseLurker _mouseLurker;
        private KeyboardLurker _keyboardLurker;
        private DockingHelper _currentDockingHelper;
        private ClipboardLurker _clipboardLurker;
        private TradebarViewModel _incomingTradeBarOverlay;
        private BuildTimelineViewModel _skillTimelineOverlay;
        private OutgoingbarViewModel _outgoingTradeBarOverlay;
        private PopupViewModel _popup;
        private LifeBulbViewModel _lifeBulbOverlay;
        private ManaBulbViewModel _manaBulbOverlay;
        private HideoutViewModel _hideoutOverlay;
        private SettingsService _settingsService;
        private HotkeyService _keyCodeService;
        private AfkService _afkService;
        private BuildService _buildService;
        private SettingsViewModel _settingsViewModel;
        private BuildViewModel _buildViewModel;
        private HelpViewModel _helpOverlay;
        private IEventAggregator _eventAggregator;
        private bool _startWithWindows;
        private bool _needUpdate;
        private bool _showInTaskBar;
        private bool _showUpdateSuccess;
        private bool _closing;
        private Task _openingTask;

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
        /// <param name="eventAggregator">The event aggregator.</param>
        public ShellViewModel(SimpleContainer container, SettingsService settingsService, HotkeyService keyCodeService, BuildService buildService, SettingsViewModel settingsViewModel, IEventAggregator eventAggregator)
        {
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
            this._eventAggregator.Subscribe(this);
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
                return this._activePlayer;
            }

            private set
            {
                if (this._activePlayer != value)
                {
                    this._activePlayer = value;
                    this.NotifyOfPropertyChange();
                    this.NotifyOfPropertyChange("ActivePlayerVisible");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether [active player visible].
        /// </summary>
        public bool ActivePlayerVisible => this.ActivePlayer != null;

        /// <summary>
        /// Gets the command.
        /// </summary>
        public DoubleClickCommand ShowSettingsCommand => new DoubleClickCommand(this.ShowSettings);

        /// <summary>
        /// Gets or sets a value indicating whether [show in task bar].
        /// </summary>
        public bool ShowInTaskBar
        {
            get
            {
                return this._showInTaskBar;
            }

            set
            {
                this._showInTaskBar = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [start with windows].
        /// </summary>
        public bool StartWithWindows
        {
            get
            {
                return this._startWithWindows;
            }

            set
            {
                this._startWithWindows = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [need update].
        /// </summary>
        public bool NeedUpdate
        {
            get
            {
                return this._needUpdate;
            }

            set
            {
                this._needUpdate = value;
                this.NotifyOfPropertyChange();
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
        public string StartupFolderPath => Path.Combine(this.ApplicationDataFolderPath, @"Microsoft\Windows\Start Menu\Programs\Startup");

        /// <summary>
        /// Gets the shortcut file path.
        /// </summary>
        public string ShortcutFilePath => Path.Combine(this.StartupFolderPath, this.ShortcutName);

        /// <summary>
        /// Gets the version.
        /// </summary>
        public string Version => GetAssemblyVersion();

        #endregion

        #region Methods

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            this._eventAggregator.Unsubscribe(this);
            this.CleanUp();
            this.TryClose();
        }

        /// <summary>
        /// Creates the short cut.
        /// </summary>
        public void CreateShortCut()
        {
            if (File.Exists(this.ShortcutFilePath))
            {
                File.Delete(this.ShortcutFilePath);
            }
            else
            {
                this.CreateLink();
            }

            this.StartWithWindows = !this.StartWithWindows;
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

            this._eventAggregator.PublishOnUIThread(message);
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

            this._eventAggregator.PublishOnUIThread(message);
            this.CleanUp();

            this.ShowInTaskBar = false;
            var updateManager = IoC.Get<UpdateManager>();
            await updateManager.Update();
        }

        /// <summary>
        /// Shows the settings.
        /// </summary>
        public void ShowSettings()
        {
            if (this._settingsViewModel.IsActive)
            {
                this._settingsViewModel.ActivateWindow();
                return;
            }

            this.ActivateItem(this._settingsViewModel);
        }

        /// <summary>
        /// Desactivate the item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="close">if set to <c>true</c> [close].</param>
        public override void DeactivateItem(Screen item, bool close)
        {
            if (item.IsActive)
            {
                Execute.OnUIThread(() => { base.DeactivateItem(item, close); });
            }
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
            file.Save(this.ShortcutFilePath, false);
        }

        /// <summary>
        /// Handles the OnSave event of the SettingsService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private async void SettingsService_OnSave(object sender, EventArgs e)
        {
            await this.CheckPledgeStatus();

            if (this._settingsService.BuildHelper)
            {
                if (this._helpOverlay == null && PoeApplicationContext.IsRunning)
                {
                    this._helpOverlay = this._container.GetInstance<HelpViewModel>();
                    this._helpOverlay.Initialize(this.ToggleBuildHelper);
                    this.ActivateItem(this._helpOverlay);

                    if (this._skillTimelineOverlay != null && this._settingsService.TimelineEnabled)
                    {
                        this.ActivateItem(this._skillTimelineOverlay);
                    }
                }
            }
            else
            {
                if (this._helpOverlay != null)
                {
                    this.DeactivateItem(this._helpOverlay, true);
                    this._helpOverlay = null;
                }

                if (this._buildViewModel != null)
                {
                    this.DeactivateItem(this._buildViewModel, true);
                }

                if (this._skillTimelineOverlay != null)
                {
                    this.DeactivateItem(this._skillTimelineOverlay, true);
                }
            }

            if (this._settingsService.IncomingTradeEnabled)
            {
                this.ActivateItem(this._incomingTradeBarOverlay);
            }
            else
            {
                this.DeactivateItem(this._incomingTradeBarOverlay, true);
            }

            if (this._settingsService.OutgoingTradeEnabled)
            {
                this.ActivateItem(this._outgoingTradeBarOverlay);
            }
            else
            {
                this.DeactivateItem(this._outgoingTradeBarOverlay, true);
            }
        }

        /// <summary>
        /// Registers the instances.
        /// </summary>
        private void ShowOverlays(int processId)
        {
            Execute.OnUIThread(() =>
            {
                this._currentDockingHelper = new DockingHelper(processId, this._settingsService);

                // Keyboard
                var keyboarHelper = new PoeKeyboardHelper(processId);
                this._keyboardLurker = new KeyboardLurker(processId, this._settingsService, this._keyCodeService, keyboarHelper);
                this._keyboardLurker.BuildToggled += this.KeyboardLurker_BuildToggled;

                // Mouse
                this._mouseLurker = new MouseLurker(processId, this._settingsService);
                this._mouseLurker.Newitem += this.MouseLurker_Newitem;

                // Clipboard
                this._clipboardLurker = new ClipboardLurker();

                this._container.RegisterInstance(typeof(ProcessLurker), null, this._processLurker);
                this._container.RegisterInstance(typeof(MouseLurker), null, this._mouseLurker);
                this._container.RegisterInstance(typeof(ClientLurker), null, this._currentLurker);
                this._container.RegisterInstance(typeof(PlayerService), null, this._currentCharacterService);
                this._container.RegisterInstance(typeof(ClipboardLurker), null, this._clipboardLurker);
                this._container.RegisterInstance(typeof(DockingHelper), null, this._currentDockingHelper);
                this._container.RegisterInstance(typeof(PoeKeyboardHelper), null, keyboarHelper);
                this._container.RegisterInstance(typeof(KeyboardLurker), null, this._keyboardLurker);

                this._skillTimelineOverlay = this._container.GetInstance<BuildTimelineViewModel>();
                this._incomingTradeBarOverlay = this._container.GetInstance<TradebarViewModel>();
                this._outgoingTradeBarOverlay = this._container.GetInstance<OutgoingbarViewModel>();
                this._popup = this._container.GetInstance<PopupViewModel>();
                this._lifeBulbOverlay = this._container.GetInstance<LifeBulbViewModel>();
                this._manaBulbOverlay = this._container.GetInstance<ManaBulbViewModel>();
                this._afkService = this._container.GetInstance<AfkService>();
                this._hideoutOverlay = this._container.GetInstance<HideoutViewModel>();
                this._helpOverlay = this._container.GetInstance<HelpViewModel>();
                this._helpOverlay.Initialize(this.ToggleBuildHelper);
                this._buildViewModel = this._container.GetInstance<BuildViewModel>();

                if (this._settingsService.BuildHelper)
                {
                    this.ActivateItem(this._buildViewModel);
                }

                if (this._settingsService.BuildHelper)
                {
                    this.ActivateItem(this._helpOverlay);
                }

                if (this._settingsService.IncomingTradeEnabled)
                {
                    this.ActivateItem(this._incomingTradeBarOverlay);
                }

                if (this._settingsService.OutgoingTradeEnabled)
                {
                    this.ActivateItem(this._outgoingTradeBarOverlay);
                }

                if (this._settingsService.TimelineEnabled)
                {
                    this.ActivateItem(this._skillTimelineOverlay);
                }

                this.ActivateItem(this._lifeBulbOverlay);
                this.ActivateItem(this._manaBulbOverlay);
                this.ActivateItem(this._hideoutOverlay);

                // Needs to be done after
                var task = this._keyboardLurker.InstallHook();
            });
        }

        /// <summary>
        /// Handles the BuildToggled event of the KeyboardLurker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void KeyboardLurker_BuildToggled(object sender, EventArgs e)
        {
            this.ToggleBuildHelper();
        }

        /// <summary>
        /// Toggles the build helper.
        /// </summary>
        private void ToggleBuildHelper()
        {
            this._buildViewModel.IsOpen = !this._buildViewModel.IsOpen;
        }

        /// <summary>
        /// Handles the PoeClosed event of the CurrentLurker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private async void PoeClosed(object sender, EventArgs e)
        {
            if (this._openingTask != null)
            {
                await this._openingTask;
            }

            this.CleanUp();
            this._openingTask = this.WaitForPoe(true);
        }

        /// <summary>
        /// Cleans up.
        /// </summary>
        private void CleanUp()
        {
            this.ActivePlayer = null;
            this._container.UnregisterHandler<ClientLurker>();
            this._container.UnregisterHandler<PlayerService>();
            this._container.UnregisterHandler<ProcessLurker>();
            this._container.UnregisterHandler<DockingHelper>();
            this._container.UnregisterHandler<PoeKeyboardHelper>();
            this._container.UnregisterHandler<ClipboardLurker>();
            this._container.UnregisterHandler<MouseLurker>();
            this._container.UnregisterHandler<KeyboardLurker>();

            if (this._clipboardLurker != null)
            {
                this._clipboardLurker.Dispose();
                this._clipboardLurker = null;
            }

            if (this._currentLurker != null)
            {
                this._currentLurker.AdminRequested -= this.CurrentLurker_AdminRequested;
                this._currentLurker.Dispose();
                this._currentLurker = null;
            }

            if (this._currentCharacterService != null)
            {
                this._currentCharacterService.Dispose();
            }

            if (this._processLurker != null)
            {
                this._processLurker.ProcessClosed -= this.PoeClosed;
                this._processLurker.Dispose();
                this._processLurker = null;
            }

            if (this._currentDockingHelper != null)
            {
                this._currentDockingHelper.Dispose();
                this._currentDockingHelper = null;
            }

            if (this._afkService != null)
            {
                this._afkService.Dispose();
                this._afkService = null;
            }

            if (this._mouseLurker != null)
            {
                this._mouseLurker.Newitem -= this.MouseLurker_Newitem;
                this._mouseLurker.Dispose();
                this._mouseLurker = null;
            }

            if (this._keyboardLurker != null)
            {
                this._keyboardLurker.BuildToggled -= this.KeyboardLurker_BuildToggled;
                this._keyboardLurker.Dispose();
            }
        }

        /// <summary>
        /// Waits for poe.
        /// </summary>
        private async Task WaitForPoe(bool fromClosing)
        {
            var affixServiceTask = AffixService.InitializeAsync();

            // Process Lurker
            this._processLurker = new PathOfExileProcessLurker();
            this._processLurker.ProcessClosed += this.PoeClosed;
            var process = await this._processLurker.WaitForProcess();

            if (this._settingsService.SyncBuild)
            {
                this._buildService.Sync();
            }

            this._currentLurker = new ClientLurker(process);
            this._currentLurker.AdminRequested += this.CurrentLurker_AdminRequested;

            this._currentCharacterService = new PlayerService(this._currentLurker);
            this.ActivePlayer = new PlayerViewModel(this._currentCharacterService);
            this.NotifyOfPropertyChange("ActivePlayer");

            if (this._closing)
            {
                return;
            }

            this.ShowOverlays(process);
            await this.CheckForUpdate();
            await this.CheckPledgeStatus();

            await affixServiceTask;
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
                this._closing = true;
            }
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        private async Task CheckForUpdate()
        {
            var updateManager = IoC.Get<UpdateManager>();
            this.NeedUpdate = await updateManager.CheckForUpdate();
            if (this.NeedUpdate)
            {
                var message = new ManaBulbMessage()
                {
                    IsUpdate = true,
                    View = new UpdateViewModel(UpdateState.NeedUpdate),
                    Action = () => this.Update(),
                };

                this._eventAggregator.PublishOnUIThread(message);
                return;
            }
            else if (this._showUpdateSuccess)
            {
                this._showUpdateSuccess = false;
                this._eventAggregator.PublishOnUIThread(new ManaBulbMessage() { IsUpdate = true, View = new UpdateViewModel(UpdateState.Success), DisplayTime = TimeSpan.FromSeconds(5) });
            }
            else
            {
                Collaboration validCollaboration = null;
                using (var patreonService = new PatreonService())
                {
                    var isPledging = await patreonService.IsPledging();
                    if (!isPledging)
                    {
                        using (var service = new CollaborationService())
                        {
                            var collaboration = await service.GetCollaborationAsync();
                            if (!collaboration.IsExpired())
                            {
                                validCollaboration = collaboration;
                            }
                        }
                    }

                    if (validCollaboration != null)
                    {
                        this._eventAggregator.PublishOnUIThread(new ManaBulbMessage() { View = new CollaborationViewModel(validCollaboration), Action = validCollaboration.Open, DisplayTime = TimeSpan.FromSeconds(6) });
                    }
                    else if (this._settingsService.ShowStartupAnimation)
                    {
                        this._eventAggregator.PublishOnUIThread(new ManaBulbMessage() { View = new SplashscreenViewModel(this._settingsViewModel, this._eventAggregator), DisplayTime = TimeSpan.FromSeconds(10) });
                    }
                }
            }
        }

        /// <summary>
        /// Clipboards the lurker newitem.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="item">The item.</param>
        private void MouseLurker_Newitem(object sender, PoeItem item)
        {
            if (!this._popup.IsActive)
            {
                this.ActivateItem(this._popup);
            }

            if (item is Map map)
            {
                if (this._settingsService.MapEnabled)
                {
                    this._popup.Open(new MapViewModel(map, this.ActivePlayer, this._currentCharacterService));
                }
            }
            else if (!this._settingsService.SearchEnabled)
            {
                return;
            }
            else if (item is Weapon weapon)
            {
                this._popup.Open(new WeaponViewModel(weapon));
            }
            else if (item.Rarity != Rarity.Currency)
            {
                this._popup.Open(new ItemOverlayViewModel(item));
            }
        }

        /// <summary>
        /// Handles the specified screen.
        /// </summary>
        /// <param name="screen">The screen.</param>
        public void Handle(Screen screen)
        {
            if (screen.IsActive)
            {
                return;
            }

            this.ActivateItem(screen);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(SkillTimelineMessage message)
        {
            if (message.IsVisible)
            {
                this.ActivateItem(this._skillTimelineOverlay);
            }
            else
            {
                this.DeactivateItem(this._skillTimelineOverlay, true);
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
}