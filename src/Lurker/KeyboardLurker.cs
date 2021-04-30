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

        private static readonly ushort DeleteKeyCode = 46;
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
        /// Occurs when [still interested pressed].
        /// </summary>
        public event KeyboardEventHandler StillInterestedPressed;

        #endregion

        #region Methods

        /// <summary>
        /// Waits for next key asynchronous.
        /// </summary>
        /// <returns>The next key press.</returns>
        public Task<ushort> WaitForNextKeyAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<ushort>();

            var hook = new KeyboardHook(ProcessLurker.CurrentProcessId);
            EventHandler<KeyboardMessageEventArgs> handler = default;
            handler = (object s, KeyboardMessageEventArgs e) =>
            {
                taskCompletionSource.SetResult(e.KeyValue);
                hook.MessageReceived -= handler;
                hook.Dispose();
            };

            hook.MessageReceived += handler;
            hook.InstallAsync().Wait();

            return taskCompletionSource.Task;
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
                    try
                    {
                        this._keyboardHook.Dispose();
                    }
                    catch
                    {
                    }
                }

                this._disposed = true;
            }
        }

        /// <summary>
        /// Mains the action toggled.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyboardMessageEventArgs"/> instance containing the event data.</param>
        private async void MainActionToggled(object sender, KeyboardMessageEventArgs e)
        {
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

                    this.InvitePressed?.Invoke(sender, e);
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
                this.MainActionPressed?.Invoke(sender, e);

                return;
            }
        }

        /// <summary>
        /// Handles the OnSave event of the SettingsService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void SettingsService_OnSave(object sender, EventArgs e)
        {
            this.UninstallHook();
            await this.InstallHookAsync();
        }

        /// <summary>
        /// Creates the hook.
        /// </summary>
        /// <returns>The task awaiter.</returns>
        public async Task InstallHookAsync()
        {
            if (this._settingsService.BuildHelper)
            {
                this._toggleBuildCode = this._hotkeyService.ToggleBuild;
                this._keyboardHook.AddHandler(this._toggleBuildCode, this.ToggleBuild);
            }

            this._keyboardHook.AddHandler('F', Modifiers.Alt, this.SearchItem);
            this._keyboardHook.AddHandler('R', Modifiers.Control, this.RemainingMonsters);
            this._keyboardHook.AddHandler(DeleteKeyCode, this.DeleteItem);

            // Hotkeys
            this._hotkeyService.Main.Install(this._keyboardHook, this.MainActionToggled, true);
            this._hotkeyService.Trade.Install(this._keyboardHook, this.TradePressed);
            this._hotkeyService.Busy.Install(this._keyboardHook, this.BusyPressed);
            this._hotkeyService.Dismiss.Install(this._keyboardHook, this.DismissPressed);
            this._hotkeyService.Invite.Install(this._keyboardHook, this.InvitePressed);

            // this._hotkeyService.StillInterested.Install(this._keyboardHook, this.StillInterestedPressed);
            try
            {
                await this._keyboardHook.InstallAsync();
                this._hooked = true;
            }
            catch
            {
                this._hooked = false;
            }
        }

        /// <summary>
        /// Uninstalls the hook.
        /// </summary>
        private void UninstallHook()
        {
            if (!this._hooked)
            {
                return;
            }

            this._keyboardHook.RemoveHandler(this._toggleBuildCode, this.ToggleBuild);
            this._keyboardHook.RemoveHandler('F', Modifiers.Alt, KeyDirection.Up, this.SearchItem);
            this._keyboardHook.RemoveHandler('R', Modifiers.Control, KeyDirection.Up, this.RemainingMonsters);
            this._keyboardHook.RemoveHandler(DeleteKeyCode, this.DeleteItem);

            this._hotkeyService.Main.Uninstall();
            this._hotkeyService.Trade.Uninstall();
            this._hotkeyService.Busy.Uninstall();
            this._hotkeyService.Dismiss.Uninstall();
            this._hotkeyService.Invite.Uninstall();

            // this._hotkeyService.StillInterested.Uninstall(this._keyboardHook);
            this._keyboardHook.Uninstall();
        }

        /// <summary>
        /// Searches the item.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyboardMessageEventArgs"/> instance containing the event data.</param>
        private async void SearchItem(object sender, KeyboardMessageEventArgs e)
        {
            if (this._settingsService.ItemHighlightEnabled && Native.IsKeyPressed(Native.VirtualKeyStates.VK_MENU))
            {
                var baseType = await this.GetItemSearchValueInClipboard();
                if (string.IsNullOrEmpty(baseType))
                {
                    return;
                }

                await this._keyboardHelper.Search(baseType);
            }
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
            if (this._settingsService.RemainingMonsterEnabled)
            {
                await this._keyboardHelper.RemainingMonster();
            }
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