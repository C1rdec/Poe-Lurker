//-----------------------------------------------------------------------
// <copyright file="KeyboardLurker.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Lurker.Helpers;
    using Lurker.Patreon.Parsers;
    using Lurker.Services;
    using WindowsInput;
    using Winook;
    using static Winook.KeyboardHook;

    /// <summary>
    /// Represents the keyboard lurker.
    /// </summary>
    public class KeyboardLurker
    {
        #region Fields

        private CancellationTokenSource _tokenSource;
        private KeyboardHook _keyboardHook;
        private ItemParser _itemParser;
        private SettingsService _settingsService;
        private HotkeyService _hotkeyService;
        private PoeKeyboardHelper _keyboardHelper;
        private Task _currentHoldTask;
        private bool _disposed;
        private ushort _toggleBuildCode;
        private int _processId;
        private bool _hooked;
        private bool _disabled;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardLurker" /> class.
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="hotkeyService">The key code service.</param>
        /// <param name="keyboardHelper">The keyboard helper.</param>
        public KeyboardLurker(int processId, SettingsService settingsService, HotkeyService hotkeyService, PoeKeyboardHelper keyboardHelper)
        {
            this._processId = processId;
            this._settingsService = settingsService;
            this._hotkeyService = hotkeyService;
            this._keyboardHelper = keyboardHelper;

            this._itemParser = new ItemParser();
            this._itemParser.CheckPledgeStatus();
            this._settingsService.OnSave += this.SettingsService_OnSave;
            this._keyboardHook = new KeyboardHook(this._processId);
            _ = this._keyboardHook.InstallAsync();
            this._hooked = true;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [build toggled].
        /// </summary>
        public event EventHandler BuildToggled;

        /// <summary>
        /// Occurs when [invite pressed].
        /// </summary>
        public event KeyboardEventHandler InvitePressed;

        /// <summary>
        /// Occurs when [busy pressed].
        /// </summary>
        public event KeyboardEventHandler BusyPressed;

        /// <summary>
        /// Occurs when [dismiss pressed].
        /// </summary>
        public event KeyboardEventHandler DismissPressed;

        /// <summary>
        /// Occurs when [trade pressed].
        /// </summary>
        public event KeyboardEventHandler TradePressed;

        /// <summary>
        /// Occurs when [main action pressed].
        /// </summary>
        public event KeyboardEventHandler MainActionPressed;

        /// <summary>
        /// Occurs when [wiki action pressed].
        /// </summary>
        public event KeyboardEventHandler OpenWikiPressed;

        #endregion

        #region Methods

        /// <summary>
        /// Waits for next key asynchronous.
        /// </summary>
        /// <returns>The next key press.</returns>
        public Task<ushort> WaitForNextKeyAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<ushort>();
            using (var hook = new KeyboardHook(ProcessLurker.CurrentProcessId))
            {
                EventHandler<KeyboardMessageEventArgs> handler = default;
                handler = (object s, KeyboardMessageEventArgs e) =>
                {
                    taskCompletionSource.SetResult(e.KeyValue);
                    hook.MessageReceived -= handler;
                };

                hook.MessageReceived += handler;
                _ = hook.InstallAsync();

                return taskCompletionSource.Task;
            }
        }

        /// <summary>
        /// Toggles the build.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyboardMessageEventArgs"/> instance containing the event data.</param>
        public void ToggleBuild(object sender, KeyboardMessageEventArgs e)
        {
            this.BuildToggled?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose() => this.Dispose(true);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this._keyboardHook.Dispose();
                }

                this._disposed = true;
            }
        }

        /// <summary>
        /// Mains the action toggled.
        /// </summary>
        /// <param name="e">The <see cref="KeyboardMessageEventArgs"/> instance containing the event data.</param>
        private async void MainActionToggled(KeyboardMessageEventArgs e)
        {
            if (this._disabled)
            {
                return;
            }

            if (e.Direction == KeyDirection.Down)
            {
                if (this._currentHoldTask != null)
                {
                    return;
                }

                try
                {
                    this._tokenSource = new CancellationTokenSource();
                    this._currentHoldTask = Task.Delay(500);
                    await this._currentHoldTask;
                    if (this._tokenSource.IsCancellationRequested)
                    {
                        this._currentHoldTask = null;
                        return;
                    }

                    this.InvitePressed?.Invoke(this, e);
                }
                finally
                {
                    this._tokenSource.Dispose();
                    this._tokenSource = null;
                }

                return;
            }

            if (e.Direction == KeyDirection.Up)
            {
                if (this._currentHoldTask == null)
                {
                    return;
                }

                if (this._currentHoldTask.IsCompleted)
                {
                    this._tokenSource = new CancellationTokenSource();
                    this._currentHoldTask = null;
                    return;
                }

                this._tokenSource.Cancel();
                this.MainActionPressed?.Invoke(this, e);

                return;
            }
        }

        /// <summary>
        /// Handles the OnSave event of the SettingsService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void SettingsService_OnSave(object sender, EventArgs e)
        {
            this.UninstallHandlers();
            this.InstallHandlers();
        }

        /// <summary>
        /// Installs the hook handlers.
        /// </summary>
        public void InstallHandlers()
        {
            if (this._settingsService.BuildHelper)
            {
                this._toggleBuildCode = this._hotkeyService.ToggleBuild;
                this._keyboardHook.AddHandler(this._toggleBuildCode, this.ToggleBuild);
            }

            this._keyboardHook.MessageReceived += this.KeyboardHook_MessageReceived;
            this._keyboardHook.AddHandler(KeyCode.Delete, this.DeleteItem);

            // Hotkeys
            this._hotkeyService.Main.Install(this._keyboardHook, this.MainActionToggled, true);
            this._hotkeyService.Trade.Install(this._keyboardHook, (e) => this.HandleKeyboardMessage(e, this.TradePressed));
            this._hotkeyService.Busy.Install(this._keyboardHook, (e) => this.HandleKeyboardMessage(e, this.BusyPressed));
            this._hotkeyService.Dismiss.Install(this._keyboardHook, (e) => this.HandleKeyboardMessage(e, this.DismissPressed));
            this._hotkeyService.Invite.Install(this._keyboardHook, (e) => this.HandleKeyboardMessage(e, this.InvitePressed));
            this._hotkeyService.OpenWiki.Install(this._keyboardHook, (e) => this.HandleKeyboardMessage(e, this.OpenWikiPressed));
            this._hotkeyService.JoinGuildHideout.Install(this._keyboardHook, (e) => this.HandleKeyboardMessage(e, this.JoinGuildHideout));
            this._hotkeyService.RemainingMonster.Install(this._keyboardHook, (e) => this.HandleKeyboardMessage(e, this.RemainingMonsters));
            this._hotkeyService.SearchItem.Install(this._keyboardHook, this.SearchItem);
        }

        private void HandleKeyboardMessage(KeyboardMessageEventArgs message, KeyboardEventHandler handler)
        {
            if (this._disabled)
            {
                return;
            }

            handler?.Invoke(this, message);
        }

        private async void KeyboardHook_MessageReceived(object sender, KeyboardMessageEventArgs e)
        {
            var keyCode = (KeyCode)e.KeyValue;
            if (!this._disabled && keyCode == KeyCode.Enter)
            {
                this._disabled = true;
                await Task.Delay(2500);
                this._disabled = false;
            }
        }

        /// <summary>
        /// Uninstalls the hook handlers.
        /// </summary>
        private void UninstallHandlers()
        {
            if (!this._hooked)
            {
                return;
            }

            this._keyboardHook.RemoveAllHandlers();

            this._hotkeyService.Main.Uninstall();
            this._hotkeyService.Trade.Uninstall();
            this._hotkeyService.Busy.Uninstall();
            this._hotkeyService.Dismiss.Uninstall();
            this._hotkeyService.Invite.Uninstall();
            this._hotkeyService.OpenWiki.Uninstall();
            this._hotkeyService.RemainingMonster.Uninstall();
            this._hotkeyService.JoinGuildHideout.Uninstall();
        }

        /// <summary>
        /// Searches the item.
        /// </summary>
        /// <param name="e">The <see cref="KeyboardMessageEventArgs"/> instance containing the event data.</param>
        private async void SearchItem(KeyboardMessageEventArgs e)
        {
            var baseType = await this.GetItemSearchValueInClipboard();
            if (string.IsNullOrEmpty(baseType))
            {
                return;
            }

            await this._keyboardHelper.Search(baseType);
        }

        /// <summary>
        /// Join the Guild Hideout.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private async void JoinGuildHideout(object sender, KeyboardMessageEventArgs e)
        {
            await this._keyboardHelper.JoinGuildHideout();
        }

        /// <summary>
        /// Deletes the item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyboardMessageEventArgs" /> instance containing the event data.</param>
        private async void DeleteItem(object sender, KeyboardMessageEventArgs e)
        {
            if (this._settingsService.DeleteItemEnabled)
            {
                await Simulate.Events().Click(WindowsInput.Events.ButtonCode.Left).Invoke();
                await this._keyboardHelper.Destroy();
            }
        }

        /// <summary>
        /// Remainings the monsters.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyboardMessageEventArgs"/> instance containing the event data.</param>
        private async void RemainingMonsters(object sender, KeyboardMessageEventArgs e)
        {
            await this._keyboardHelper.RemainingMonster();
        }

        /// <summary>
        /// Gets the item base type in clipboard.
        /// </summary>
        /// <returns>The item search value.</returns>
        private async Task<string> GetItemSearchValueInClipboard()
        {
            try
            {
                await Simulate.Events().ClickChord(WindowsInput.Events.KeyCode.Control, WindowsInput.Events.KeyCode.C).Invoke();
                await Task.Delay(50);
                var text = ClipboardHelper.GetClipboardText();
                ClipboardHelper.ClearClipboard();

                return this._itemParser.GetSearchValue(text);
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }
}