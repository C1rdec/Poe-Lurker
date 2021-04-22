//-----------------------------------------------------------------------
// <copyright file="KeyboardLurker.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker
{
    using System;
    using System.Threading.Tasks;
    using Lurker.Helpers;
    using Lurker.Patreon.Parsers;
    using Lurker.Services;
    using WindowsInput;
    using Winook;

    /// <summary>
    /// Represents the keyboard lurker.
    /// </summary>
    public class KeyboardLurker
    {
        #region Fields

        private static readonly ushort DeleteKeyCode = 46;
        private KeyboardHook _keyboardHook;
        private ItemParser _itemParser;
        private SettingsService _settingsService;
        private KeyCodeService _keyCodeService;
        private PoeKeyboardHelper _keyboardHelper;
        private bool _disposed;
        private ushort _toggleBuildCode;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardLurker" /> class.
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="keyCodeService">The key code service.</param>
        /// <param name="keyboardHelper">The keyboard helper.</param>
        public KeyboardLurker(int processId, SettingsService settingsService, KeyCodeService keyCodeService, PoeKeyboardHelper keyboardHelper)
        {
            this._settingsService = settingsService;
            this._keyCodeService = keyCodeService;
            this._keyboardHelper = keyboardHelper;

            this._itemParser = new ItemParser();
            this._itemParser.CheckPledgeStatus();
            this._settingsService.OnSave += this.SettingsService_OnSave;

            this._keyboardHook = new KeyboardHook(processId);
            this.InstallHook();
        }

        private void SettingsService_OnSave(object sender, EventArgs e)
        {
            this.UninstallHook();
            this.InstallHook();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [build toggled].
        /// </summary>
        public event EventHandler BuildToggled;

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
        /// Creates the hook.
        /// </summary>
        private void InstallHook()
        {
            if (this._settingsService.BuildHelper)
            {
                this._toggleBuildCode = this._keyCodeService.ToggleBuild;
                this._keyboardHook.AddHandler(this._toggleBuildCode, this.ToggleBuild);
            }

            this._keyboardHook.AddHandler('F', Modifiers.Alt, this.SearchItem);
            this._keyboardHook.AddHandler('R', Modifiers.Control, this.RemainingMonsters);
            this._keyboardHook.AddHandler(DeleteKeyCode, this.DeleteItem);

            this._keyboardHook.InstallAsync();
        }

        /// <summary>
        /// Uninstalls the hook.
        /// </summary>
        private void UninstallHook()
        {
            this._keyboardHook.RemoveHandler(this._toggleBuildCode, this.ToggleBuild);
            this._keyboardHook.RemoveHandler('F', Modifiers.Alt, KeyDirection.Down, this.SearchItem);
            this._keyboardHook.RemoveHandler('R', Modifiers.Control, KeyDirection.Down, this.RemainingMonsters);
            this._keyboardHook.RemoveHandler(DeleteKeyCode, this.DeleteItem);

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