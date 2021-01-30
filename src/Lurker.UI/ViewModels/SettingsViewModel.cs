//-----------------------------------------------------------------------
// <copyright file="SettingsViewModel.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Security.Authentication;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;
    using Caliburn.Micro;
    using Lurker.Helpers;
    using Lurker.Services;
    using Lurker.UI.Helpers;
    using MahApps.Metro.Controls;
    using MahApps.Metro.Controls.Dialogs;
    using Sentry;

    /// <summary>
    /// Represents the settings view model.
    /// </summary>
    /// <seealso cref="Lurker.UI.ViewModels.ScreenBase" />
    public class SettingsViewModel : ScreenBase
    {
        #region Fields

        private static readonly string LottieFileName = "LurckerIconSettings.json";
        private Task _activateTask;
        private bool _trialAvailable;
        private KeyboardHelper _keyboardHelper;
        private SettingsService _settingService;
        private bool _modified;
        private string _blessingtext;
        private bool _needsUpdate;
        private bool _pledging;
        private bool _activated;
        private int _alertVolume;
        private int _joinHideoutVolume;
        private Patreon.PatreonService _currentPatreonService;
        private SoundService _soundService;
        private CancellationTokenSource _currentTokenSource;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel" /> class.
        /// </summary>
        /// <param name="windowManager">The window manager.</param>
        /// <param name="keyboardHelper">The keyboard helper.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="soundService">The sound service.</param>
        public SettingsViewModel(IWindowManager windowManager, KeyboardHelper keyboardHelper, SettingsService settingsService, SoundService soundService)
            : base(windowManager)
        {
            this._keyboardHelper = keyboardHelper;
            this._settingService = settingsService;
            this._soundService = soundService;
            this.DisplayName = "Settings";

            this.PropertyChanged += this.SettingsViewModel_PropertyChanged;

            if (!AssetService.Exists(LottieFileName))
            {
                AssetService.Create(LottieFileName, GetResourceContent(LottieFileName));
            }

            this.BuildManager = new BuildManagerViewModel(this.ShowMessage);
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
        /// Gets or sets the build manager.
        /// </summary>
        /// <value>The build manager.</value>
        public BuildManagerViewModel BuildManager { get; set; }

        /// <summary>
        /// Gets or sets the index of the select teb.
        /// </summary>
        public int SelectTabIndex { get; set; }

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
                return !new Patreon.TokenService().Connected;
            }
        }

        /// <summary>
        /// Gets the patreon identifier.
        /// </summary>
        public string PatreonId => new Patreon.TokenService().PatreonId;

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
        /// Gets or sets a value indicating whether [remaining monster enabled].
        /// </summary>
        public bool RemainingMonsterEnabled
        {
            get
            {
                return this._settingService.RemainingMonsterEnabled;
            }

            set
            {
                this._settingService.RemainingMonsterEnabled = value;
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
        /// Gets or sets a value indicating whether [item highlight enabled].
        /// </summary>
        public bool ItemHighlightEnabled
        {
            get
            {
                return this._settingService.ItemHighlightEnabled;
            }

            set
            {
                this._settingService.ItemHighlightEnabled = value;
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

        #endregion

        #region Methods

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
                using (this._currentPatreonService = new Patreon.PatreonService())
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
                using (var service = new Patreon.PatreonService())
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

                    SentrySdk.CaptureMessage("New Trial", Sentry.Protocol.SentryLevel.Info);
                }
            });
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        public async void SaveSettings()
        {
            await this.ShowProgress("Hold on", "Saving setings...", () => this._settingService.Save());
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
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view">The view.</param>
        protected override void OnViewLoaded(object view)
        {
            var window = view as MetroWindow;
            window.Activate();
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
            this._activateTask = Task.Run(async () =>
            {
                using (var service = new Patreon.PatreonService())
                {
                    this.Pledging = await service.IsPledging();

                    if (!this.Pledging)
                    {
                        this.TrialAvailable = service.TrialAvailable;
                        this.SearchEnabled = false;
                        this.DashboardEnabled = false;
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

            var text = string.Empty;
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
            if (e.PropertyName != nameof(this.SelectTabIndex) && e.PropertyName != nameof(this.Modified) && e.PropertyName != nameof(this.IsActive))
            {
                if (!this._activated || !this._activateTask.IsCompleted)
                {
                    return;
                }

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
                this.PlaySoundTest(this._currentTokenSource.Token, () => this._soundService.PlayTradeAlert(this._settingService.AlertVolume));
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
        /// Shows the message.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        private async Task ShowMessage(string title, string message)
        {
            var coordinator = DialogCoordinator.Instance;
            await coordinator.ShowMessageAsync(this, title, message);
        }

        #endregion
    }
}