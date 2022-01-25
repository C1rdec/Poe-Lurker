//-----------------------------------------------------------------------
// <copyright file="SettingsViewModel.cs" company="Wohs Inc.">
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
    using System.Reflection;
    using System.Security.Authentication;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Patreon.Services;
    using Lurker.Services;
    using Lurker.UI.Helpers;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;
    using Microsoft.Win32;
    using NAudio.Wave;
    using Sentry;
    using Winook;

    /// <summary>
    /// Represents the settings view model.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.ScreenBase" />
    public class SettingsViewModel : ScreenBase
    {
        #region Fields

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly string LottieFileName = "LurckerIconSettings.json";
        private Task _activateTask;
        private bool _trialAvailable;
        private KeyboardHelper _keyboardHelper;
        private SettingsService _settingService;
        private HotkeyService _hotkeyService;
        private bool _modified;
        private string _blessingtext;
        private bool _needsUpdate;
        private bool _pledging;
        private bool _activated;
        private int _selectedTabIndex;
        private int _alertVolume;
        private int _itemAlertVolume;
        private int _joinHideoutVolume;
        private PatreonService _currentPatreonService;
        private SoundService _soundService;
        private CancellationTokenSource _currentTokenSource;
        private bool _hasCustomTradeSound;
        private bool _hasCustomItemSound;
        private WaveOutEvent _currentSound;
        private bool _isCharacterOpen;
        private bool _isPushBulletOpen;
        private CharacterManagerViewModel _characterManager;
        private PushBulletViewModel _pushBulletViewModel;
        private bool _keyboardWaiting;
        private MetroWindow _view;
        private IEnumerable<string> _excludePropertyNames;

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
        public SettingsViewModel(
            IWindowManager windowManager,
            KeyboardHelper keyboardHelper,
            SettingsService settingsService,
            HotkeyService hotkeyService,
            SoundService soundService,
            GithubService githubService,
            PushBulletService pushBulletService)
            : base(windowManager)
        {
            this._keyboardHelper = keyboardHelper;
            this._settingService = settingsService;
            this._hotkeyService = hotkeyService;
            this._soundService = soundService;
            this.DisplayName = "Settings";
            this._excludePropertyNames = this.GetExcludedPropertyNames();
            this.PropertyChanged += this.SettingsViewModel_PropertyChanged;

            if (!AssetService.Exists(LottieFileName))
            {
                AssetService.Create(LottieFileName, GetResourceContent(LottieFileName));
            }

            this.BuildManager = new BuildManagerViewModel(this.ShowMessage, githubService);
            this.PushBullet = new PushBulletViewModel(pushBulletService);
            this.SetupHotkeys();
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
        /// Gets the PushBullet.
        /// </summary>
        public PushBulletViewModel PushBullet
        {
            get
            {
                return this._pushBulletViewModel;
            }

            private set
            {
                this._pushBulletViewModel = value;
                this.NotifyOfPropertyChange();
            }
        }

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
        /// Gets the toggle build key value.
        /// </summary>
        public string ToggleBuildKeyValue => ConvertKeyCode(this._hotkeyService.ToggleBuild);

        /// <summary>
        /// Gets or sets a value indicating whether this instance is character open.
        /// </summary>
        public bool IsCharacterOpen
        {
            get
            {
                return this._isCharacterOpen;
            }

            set
            {
                this._isCharacterOpen = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is character open.
        /// </summary>
        public bool IsPushBulletOpen
        {
            get
            {
                return this._isPushBulletOpen;
            }

            set
            {
                this._isPushBulletOpen = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance has custom trade sound.
        /// </summary>
        public bool HasCustomTradeSound
        {
            get
            {
                return this._hasCustomTradeSound;
            }

            set
            {
                this._hasCustomTradeSound = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether HasCustom sound for Item.
        /// </summary>
        public bool HasCustomItemSound
        {
            get
            {
                return this._hasCustomItemSound;
            }

            set
            {
                this._hasCustomItemSound = value;
                this.NotifyOfPropertyChange();
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
                return this._characterManager;
            }

            private set
            {
                this._characterManager = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the index of the select teb.
        /// </summary>
        public int SelectTabIndex
        {
            get
            {
                return this._selectedTabIndex;
            }

            set
            {
                this._selectedTabIndex = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="SettingsViewModel"/> is saved.
        /// </summary>
        public bool Modified
        {
            get
            {
                return this._modified;
            }

            private set
            {
                this._modified = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [trial available].
        /// </summary>
        public bool TrialAvailable
        {
            get
            {
                return this._trialAvailable;
            }

            set
            {
                this._trialAvailable = value;
                this.NotifyOfPropertyChange();
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
                return this._blessingtext;
            }

            private set
            {
                this._blessingtext = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the color of the lifebar.
        /// </summary>
        public Color LifebarColor
        {
            get
            {
                return (Color)ColorConverter.ConvertFromString(this._settingService.LifeForeground);
            }

            set
            {
                this._settingService.LifeForeground = value.ToString();
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the tradebar scaling.
        /// </summary>
        public double TradebarScaling
        {
            get
            {
                return this._settingService.TradebarScaling;
            }

            set
            {
                this._settingService.TradebarScaling = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show startup animation].
        /// </summary>
        public bool ShowStartupAnimation
        {
            get
            {
                return this._settingService.ShowStartupAnimation;
            }

            set
            {
                this._settingService.ShowStartupAnimation = value;
                this.NotifyOfPropertyChange();
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
                return this._pledging;
            }

            set
            {
                this._pledging = value;
                this.NotifyOfPropertyChange();
                this.NotifyOfPropertyChange("NotPledging");
                this.NotifyOfPropertyChange("ConnectedWithoutPledging");
            }
        }

        /// <summary>
        /// Gets a value indicating whether [not pledgin].
        /// </summary>
        public bool NotPledging => !this.Pledging;

        /// <summary>
        /// Gets a value indicating whether [connected without pledge].
        /// </summary>
        public bool ConnectedWithoutPledging => !this.NotConnected && this.NotPledging;

        /// <summary>
        /// Gets or sets a value indicating whether [needs update].
        /// </summary>
        public bool NeedsUpdate
        {
            get
            {
                return this._needsUpdate;
            }

            set
            {
                this._needsUpdate = value;
                this.NotifyOfPropertyChange();
                this.NotifyOfPropertyChange("UpToDate");
            }
        }

        /// <summary>
        /// Gets a value indicating whether [up to date].
        /// </summary>
        public bool UpToDate => !this.NeedsUpdate;

        /// <summary>
        /// Gets or sets the busy message.
        /// </summary>
        public string BusyMessage
        {
            get
            {
                return this._settingService.BusyMessage;
            }

            set
            {
                this._settingService.BusyMessage = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [incoming trade enabled].
        /// </summary>
        public bool IncomingTradeEnabled
        {
            get
            {
                return this._settingService.IncomingTradeEnabled;
            }

            set
            {
                this._settingService.IncomingTradeEnabled = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [outgoing trade enabled].
        /// </summary>
        public bool OutgoingTradeEnabled
        {
            get
            {
                return this._settingService.OutgoingTradeEnabled;
            }

            set
            {
                this._settingService.OutgoingTradeEnabled = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [dashboard enabled].
        /// </summary>
        public bool DashboardEnabled
        {
            get
            {
                return this._settingService.DashboardEnabled;
            }

            set
            {
                this._settingService.DashboardEnabled = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the sold message.
        /// </summary>
        public string SoldMessage
        {
            get
            {
                return this._settingService.SoldMessage;
            }

            set
            {
                this._settingService.SoldMessage = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the thank you message.
        /// </summary>
        public string ThankYouMessage
        {
            get
            {
                return this._settingService.ThankYouMessage;
            }

            set
            {
                this._settingService.ThankYouMessage = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the still interested message.
        /// </summary>
        public string StillInterestedMessage
        {
            get
            {
                return this._settingService.StillInterestedMessage;
            }

            set
            {
                this._settingService.StillInterestedMessage = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [alert enabled].
        /// </summary>
        public bool AlertEnabled
        {
            get
            {
                return this._settingService.AlertEnabled;
            }

            set
            {
                this._settingService.AlertEnabled = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [alert enabled].
        /// </summary>
        public bool ItemAlertEnabled
        {
            get
            {
                return this._settingService.ItemAlertEnabled;
            }

            set
            {
                this._settingService.ItemAlertEnabled = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [join hideout enabled].
        /// </summary>
        public bool JoinHideoutEnabled
        {
            get
            {
                return this._settingService.JoinHideoutEnabled;
            }

            set
            {
                this._settingService.JoinHideoutEnabled = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [alert enabled].
        /// </summary>
        public bool SearchEnabled
        {
            get
            {
                return this._settingService.SearchEnabled;
            }

            set
            {
                this._settingService.SearchEnabled = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [map enabled].
        /// </summary>
        public bool MapEnabled
        {
            get
            {
                return this._settingService.MapEnabled;
            }

            set
            {
                this._settingService.MapEnabled = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [clipboard enabled].
        /// </summary>
        public bool ClipboardEnabled
        {
            get
            {
                return this._settingService.ClipboardEnabled;
            }

            set
            {
                this._settingService.ClipboardEnabled = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [automatic kick enabled].
        /// </summary>
        public bool AutoKickEnabled
        {
            get
            {
                return this._settingService.AutoKickEnabled;
            }

            set
            {
                this._settingService.AutoKickEnabled = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [debug enabled].
        /// </summary>
        public bool DebugEnabled
        {
            get
            {
                return this._settingService.DebugEnabled;
            }

            set
            {
                this._settingService.DebugEnabled = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the alert volume.
        /// </summary>
        public int AlertVolume
        {
            get
            {
                return this._alertVolume;
            }

            set
            {
                this._alertVolume = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the alert volume.
        /// </summary>
        public int ItemAlertVolume
        {
            get
            {
                return this._itemAlertVolume;
            }

            set
            {
                this._itemAlertVolume = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the join hideout volume.
        /// </summary>
        public int JoinHideoutVolume
        {
            get
            {
                return this._joinHideoutVolume;
            }

            set
            {
                this._joinHideoutVolume = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [tool tip enabled].
        /// </summary>
        public bool ToolTipEnabled
        {
            get
            {
                return this._settingService.ToolTipEnabled;
            }

            set
            {
                this._settingService.ToolTipEnabled = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets the tool tip delay.
        /// </summary>
        public int ToolTipDelay
        {
            get
            {
                return this._settingService.ToolTipDelay;
            }

            set
            {
                this._settingService.ToolTipDelay = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [always visible].
        /// </summary>
        public bool AlwaysVisible
        {
            get
            {
                return !this._settingService.HideInBackground;
            }

            set
            {
                this._settingService.HideInBackground = !value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [delete item enabled].
        /// </summary>
        public bool DeleteItemEnabled
        {
            get
            {
                return this._settingService.DeleteItemEnabled;
            }

            set
            {
                this._settingService.DeleteItemEnabled = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [vulkan renderer].
        /// </summary>
        public bool VulkanRenderer
        {
            get
            {
                return this._settingService.VulkanRenderer;
            }

            set
            {
                this._settingService.VulkanRenderer = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [build helper].
        /// </summary>
        public bool BuildHelper
        {
            get
            {
                return this._settingService.BuildHelper;
            }

            set
            {
                this._settingService.BuildHelper = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [sold detection].
        /// </summary>
        public bool SoldDetection
        {
            get
            {
                return this._settingService.SoldDetection;
            }

            set
            {
                this._settingService.SoldDetection = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show release note].
        /// </summary>
        public bool ShowReleaseNote
        {
            get
            {
                return this._settingService.ShowReleaseNote;
            }

            set
            {
                this._settingService.ShowReleaseNote = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [synchronize build].
        /// </summary>
        public bool SyncBuild
        {
            get
            {
                return this._settingService.SyncBuild;
            }

            set
            {
                this._settingService.SyncBuild = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hideout enabled].
        /// </summary>
        public bool HideoutEnabled
        {
            get
            {
                return this._settingService.HideoutEnabled;
            }

            set
            {
                this._settingService.HideoutEnabled = value;
                this.NotifyOfPropertyChange();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hideout enabled].
        /// </summary>
        public bool GuildHideoutEnabled
        {
            get
            {
                return this._settingService.GuildHideoutEnabled;
            }

            set
            {
                this._settingService.GuildHideoutEnabled = value;
                this.NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Set the tab index to lurker pro.
        /// </summary>
        public void OpenLurkerPro()
        {
            this.SelectTabIndex = 6;
        }

        /// <summary>
        /// Opens the logs.
        /// </summary>
        public void OpenLogs()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var folderName = Path.Combine(localAppData, "PoeLurker", $"app-{this.Version}/logs");

            if (Directory.Exists(folderName))
            {
                Process.Start(folderName);
            }
            else
            {
                this.ShowMessage("Oups!", "No logs folder found.", MessageDialogStyle.Affirmative);
            }
        }

        /// <summary>
        /// Opens the discord.
        /// </summary>
        public void OpenDiscord()
        {
            Process.Start("https://discord.com/invite/hQERv7K");
        }

        /// <summary>
        /// Opens the patreon.
        /// </summary>
        public void OpenPatreon()
        {
            Process.Start("https://www.patreon.com/poelurker");
        }

        /// <summary>
        /// Activates the window.
        /// </summary>
        public void ActivateWindow()
        {
            this._view.WindowState = WindowState.Normal;
            this._view.Activate();
        }

        /// <summary>
        /// Sets the toggle build key code.
        /// </summary>
        /// <returns>The task awaiter.</returns>
        public async Task SetToggleBuildKeyCode()
        {
            if (this._keyboardWaiting)
            {
                return;
            }

            this._keyboardWaiting = true;
            var task = this._keyboardHelper.WaitForNextKeyAsync();
            await this.ShowProgress("Waiting input for...", "Toggle build helper", task);

            var key = await task;
            this._hotkeyService.ToggleBuild = key.KeyValue;
            this.NotifyOfPropertyChange(() => this.ToggleBuildKeyValue);
            this._keyboardWaiting = false;
        }

        /// <summary>
        /// Gets the next key code.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>The key.</returns>
        private async Task<KeyboardMessageEventArgs> GetNextKeyCode(string description)
        {
            if (this._keyboardWaiting)
            {
                return null;
            }

            this._keyboardWaiting = true;
            var task = this._keyboardHelper.WaitForNextKeyAsync();
            await this.ShowProgress("Waiting input for...", description, task);

            var key = await task;
            this._keyboardWaiting = false;

            return key;
        }

        /// <summary>
        /// Opens the characters.
        /// </summary>
        public void OpenCharacters()
        {
            this.CharacterManager = new CharacterManagerViewModel(this.ShowMessage);
            this.IsCharacterOpen = true;
        }

        /// <summary>
        /// Selects the custom sound.
        /// </summary>
        public void SelectTradeSound()
        {
            if (!this.HasCustomTradeSound)
            {
                try
                {
                    if (this._currentSound != null)
                    {
                        this._currentSound.Stop();
                        this._currentSound = null;
                    }

                    AssetService.Delete(SoundService.TradeAlertFileName);
                }
                catch
                {
                    // Sound was playing.
                    this.HasCustomTradeSound = true;
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
                this.HasCustomTradeSound = false;
            }
        }

        /// <summary>
        /// Select the good item sound.
        /// </summary>
        public void SelectItemSound()
        {
            if (!this.HasCustomItemSound)
            {
                try
                {
                    if (this._currentSound != null)
                    {
                        this._currentSound.Stop();
                        this._currentSound = null;
                    }

                    AssetService.Delete(SoundService.ItemAlertFileName);
                }
                catch
                {
                    // Sound was playing.
                    this.HasCustomItemSound = true;
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
                this.HasCustomItemSound = false;
            }
        }

        /// <summary>
        /// Opens the dashboard.
        /// </summary>
        public void OpenDashboard()
        {
            var dashBoard = IoC.Get<DashboardViewModel>();
            var eventAggregator = IoC.Get<IEventAggregator>();
            eventAggregator.PublishOnUIThread(dashBoard);
        }

        /// <summary>
        /// Gets the patreon identifier.
        /// </summary>
        public void GetPatreonId()
        {
            if (string.IsNullOrEmpty(this.PatreonId))
            {
                return;
            }

            Clipboard.SetText(this.PatreonId);
        }

        /// <summary>
        /// Inserts the buyer name token.
        /// </summary>
        public async void InsertBuyerNameToken()
        {
            await this._keyboardHelper.Write(TokenHelper.BuyerName);
        }

        /// <summary>
        /// Inserts the buyer name token.
        /// </summary>
        public async void InsertLocationToken()
        {
            await this._keyboardHelper.Write(TokenHelper.Location);
        }

        /// <summary>
        /// Inserts the price token.
        /// </summary>
        public async void InsertPriceToken()
        {
            await this._keyboardHelper.Write(TokenHelper.Price);
        }

        /// <summary>
        /// Inserts the item name token.
        /// </summary>
        public async void InsertItemNameToken()
        {
            await this._keyboardHelper.Write(TokenHelper.ItemName);
        }

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
        {
            Process.Start(@"https://docs.google.com/presentation/d/1XhaSSNAFGxzouc5amzAW8c_6ifToNjnsQq5UmNgLXoo/present?slide=id.p");
        }

        /// <summary>
        /// Cheats the sheet.
        /// </summary>
        public void CheatSheet()
        {
            Process.Start(@"https://github.com/C1rdec/Poe-Lurker/blob/master/assets/CheatSheet.md");
        }

        /// <summary>
        /// Logins to patreon.
        /// </summary>
        public async void LoginToPatreon()
        {
            if (this._currentPatreonService != null)
            {
                this._currentPatreonService.Cancel();
                await Task.Delay(600);
            }

            try
            {
                using (this._currentPatreonService = new PatreonService())
                {
                    if (!this._currentPatreonService.IsConnected)
                    {
                        await this._currentPatreonService.Login();
                    }

                    this.Pledging = await this._currentPatreonService.IsPledging();
                    if (this.Pledging)
                    {
                        this.TrialAvailable = false;
                        var time = this._currentPatreonService.GetTrialRemainingTime();
                        this.BlessingText = GetBlessingText(time);

                        this.SearchEnabled = true;
                        this.MapEnabled = true;
                        this.DashboardEnabled = true;
                    }

                    this.NotifyOfPropertyChange("NotConnected");
                }
            }
            catch (AuthenticationException)
            {
            }
        }

        /// <summary>
        /// Starts the trial.
        /// </summary>
        public async void StartTrial()
        {
            await this.ShowProgress("Hold on", "Preparing the trial...", async () =>
            {
                using (var service = new PatreonService())
                {
                    service.StartTrial();
                    var pledging = await service.IsPledging();
                    if (pledging)
                    {
                        this.SearchEnabled = true;
                        this.DashboardEnabled = true;

                        var time = service.GetTrialRemainingTime();
                        this.BlessingText = GetBlessingText(time);
                    }

                    this.TrialAvailable = false;
                    this.Pledging = pledging;

                    SentrySdk.CaptureMessage("New Trial", SentryLevel.Info);
                }
            });
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        public async void SaveSettings()
        {
            await this.ShowProgress("Hold on", "Saving setings...", () =>
            {
                this._settingService.Save();
                this._hotkeyService.Save();
            });
            this.Modified = false;
        }

        /// <summary>
        /// Pledges this instance.
        /// </summary>
        public void Pledge()
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                this.GetPatreonId();
                return;
            }

            Process.Start("https://www.patreon.com/poelurker");
        }

        /// <summary>
        /// Open push bullet flyout.
        /// </summary>
        public void OpenPushBullet()
        {
            this.IsPushBulletOpen = true;
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view">The view.</param>
        protected override void OnViewLoaded(object view)
        {
            this._view = view as MetroWindow;
            this.ActivateWindow();
            base.OnViewLoaded(view);
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected override async void OnDeactivate(bool close)
        {
            if (close)
            {
                if (this._activateTask != null)
                {
                    await this._activateTask;
                }

                this._settingService.Save();
                this.OnClose?.Invoke(this, EventArgs.Empty);
                this._activated = false;
                this.SelectTabIndex = 0;
                this.Modified = false;
            }

            base.OnDeactivate(close);
        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected override void OnActivate()
        {
            this.HasCustomTradeSound = this._soundService.HasCustomTradeAlert();
            this.HasCustomItemSound = this._soundService.HasCustomItemAlert();
            this.BuildManager.PopulateBuilds(this.SyncBuild);
            this._activateTask = Task.Run(async () =>
            {
                using (var service = new PatreonService())
                {
                    this.Pledging = await service.IsPledging();

                    if (!this.Pledging)
                    {
                        this.TrialAvailable = service.TrialAvailable;
                        this.SearchEnabled = false;
                        this.DashboardEnabled = false;
                        this.MapEnabled = false;
                    }
                    else
                    {
                        if (service.IsTrialValid())
                        {
                            var time = service.GetTrialRemainingTime();
                            this.BlessingText = GetBlessingText(time);
                        }
                        else
                        {
                            this.BlessingText = "A blessing I can’t deny";
                        }
                    }
                }
            });

            this.AlertVolume = (int)(this._settingService.AlertVolume * 100);
            this.ItemAlertVolume = (int)(this._settingService.ItemAlertVolume * 100);
            this.JoinHideoutVolume = (int)(this._settingService.JoinHideoutVolume * 100);
            this.CheckForUpdate();
            base.OnActivate();

            this._activated = true;
        }

        /// <summary>
        /// Gets the blessing text.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>The blessing trial text.</returns>
        private static string GetBlessingText(TimeSpan time)
        {
            if (time == TimeSpan.Zero)
            {
                return "A blessing I can’t deny";
            }

            if (time.Days > 0)
            {
                return $"{time.Days} days, {time.Hours} hours, {time.Minutes} minutes";
            }

            if (time.Hours > 0)
            {
                return $"{time.Hours} hours, {time.Minutes} minutes";
            }

            return $"{time.Minutes} minutes";
        }

        /// <summary>
        /// Gets the content of the resource.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The resource content.</returns>
        private static string GetResourceContent(string fileName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Lurker.UI.Assets.{fileName}"))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Converts the key code.
        /// </summary>
        /// <param name="keyCode">The key code.</param>
        /// <returns>The key value.</returns>
        private static string ConvertKeyCode(uint keyCode)
        {
            return ((KeyCode)keyCode).ToString();
        }

        /// <summary>
        /// Get the excluded property name for PropertyChanged event.
        /// </summary>
        /// <returns>The list of property names.</returns>
        private IEnumerable<string> GetExcludedPropertyNames()
        {
            return new List<string>()
            {
                nameof(this.IsCharacterOpen),
                nameof(this.IsPushBulletOpen),
                nameof(this.Modified),
                nameof(this.CharacterManager),
                nameof(this.SelectTabIndex),
                nameof(this.IsActive),
                nameof(this.NeedsUpdate),
                nameof(this.UpToDate),
            };
        }

        /// <summary>
        /// Setups the hotkeys.
        /// </summary>
        private void SetupHotkeys()
        {
            this.MainHotkey = new HotkeyViewModel("Invite & Trade", this._hotkeyService.Main, this.GetNextKeyCode);
            this.OpenWikiHotkey = new HotkeyViewModel("Open Wiki", this._hotkeyService.OpenWiki, this.GetNextKeyCode);

            this.GuildHideoutHotkey = new HotkeyViewModel("Guild Hideout", this._hotkeyService.JoinGuildHideout, this.GetNextKeyCode);

            this.MonsterRemainingHotkey = new HotkeyViewModel("Remaining Monster", this._hotkeyService.RemainingMonster, this.GetNextKeyCode);
            this.SearchItemHotkey = new HotkeyViewModel("Item Highlight", this._hotkeyService.SearchItem, this.GetNextKeyCode);

            this.Hotkeys = new ObservableCollection<HotkeyViewModel>
            {
                new HotkeyViewModel("Invite", this._hotkeyService.Invite, this.GetNextKeyCode),
                new HotkeyViewModel("Trade", this._hotkeyService.Trade, this.GetNextKeyCode),
                new HotkeyViewModel("Busy", this._hotkeyService.Busy, this.GetNextKeyCode),
                new HotkeyViewModel("Dismiss", this._hotkeyService.Dismiss, this.GetNextKeyCode),
            };

            // this.Hotkeys.Add(new HotkeyViewModel("Still Interested", this._hotkeyService.StillInterested, this.GetNextKeyCode));
            foreach (var hotkey in this.Hotkeys)
            {
                hotkey.PropertyChanged += this.Hotkey_PropertyChanged;
            }

            this.MainHotkey.PropertyChanged += this.Hotkey_PropertyChanged;
            this.OpenWikiHotkey.PropertyChanged += this.Hotkey_PropertyChanged;
            this.MonsterRemainingHotkey.PropertyChanged += this.Hotkey_PropertyChanged;
            this.SearchItemHotkey.PropertyChanged += this.Hotkey_PropertyChanged;
            this.GuildHideoutHotkey.PropertyChanged += this.Hotkey_PropertyChanged;
        }

        /// <summary>
        /// Handles the PropertyChanged event of the Hotkey control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void Hotkey_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.NotifyOfPropertyChange("Dirty");
        }

        /// <summary>
        /// Checks for update.
        /// </summary>
        private async void CheckForUpdate()
        {
            var updateManager = IoC.Get<UpdateManager>();
            this.NeedsUpdate = await updateManager.CheckForUpdate();
        }

        /// <summary>
        /// Handles the PropertyChanged event of the SettingsViewModel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void SettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!this._excludePropertyNames.Contains(e.PropertyName))
            {
                if (!this._activated || !this._activateTask.IsCompleted)
                {
                    return;
                }

                Logger.Trace($"Modifed: {e.PropertyName}");
                this.Modified = true;
            }

            if (e.PropertyName == nameof(this.AlertVolume))
            {
                this._settingService.AlertVolume = (float)this.AlertVolume / 100;

                if (!this._activated || !this._activateTask.IsCompleted)
                {
                    return;
                }

                if (this._currentTokenSource != null)
                {
                    this._currentTokenSource.Cancel();
                    this._currentTokenSource.Dispose();
                    this._currentTokenSource = null;
                }

                this._currentTokenSource = new CancellationTokenSource();
                this.PlaySoundTest(this._currentTokenSource.Token, () =>
                {
                    this._currentSound = this._soundService.PlayTradeAlert(this._settingService.AlertVolume);
                });
            }
            else if (e.PropertyName == nameof(this.ItemAlertVolume))
            {
                this._settingService.ItemAlertVolume = (float)this.ItemAlertVolume / 100;

                if (!this._activated || !this._activateTask.IsCompleted)
                {
                    return;
                }

                if (this._currentTokenSource != null)
                {
                    this._currentTokenSource.Cancel();
                    this._currentTokenSource.Dispose();
                    this._currentTokenSource = null;
                }

                this._currentTokenSource = new CancellationTokenSource();
                this.PlaySoundTest(this._currentTokenSource.Token, () =>
                {
                    this._currentSound = this._soundService.PlayItemAlert(this._settingService.ItemAlertVolume);
                });
            }
            else if (e.PropertyName == nameof(this.JoinHideoutVolume))
            {
                this._settingService.JoinHideoutVolume = (float)this.JoinHideoutVolume / 100;

                if (!this._activated || !this._activateTask.IsCompleted)
                {
                    return;
                }

                if (this._currentTokenSource != null)
                {
                    this._currentTokenSource.Cancel();
                    this._currentTokenSource.Dispose();
                    this._currentTokenSource = null;
                }

                this._currentTokenSource = new CancellationTokenSource();
                this.PlaySoundTest(this._currentTokenSource.Token, () => this._soundService.PlayJoinHideout(this._settingService.JoinHideoutVolume));
            }
            else if (e.PropertyName == nameof(this.SyncBuild))
            {
                if (this.SyncBuild)
                {
                    this.BuildManager.PopulateBuilds(true);
                }
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

        #endregion
    }
}