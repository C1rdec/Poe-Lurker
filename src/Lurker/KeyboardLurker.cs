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
        private PoeKeyboardHelper _keyboardHelper;
        private bool _disposed;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardLurker" /> class.
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        /// <param name="settingsService">The settings service.</param>
        /// <param name="keyboardHelper">The keyboard helper.</param>
        public KeyboardLurker(int processId, SettingsService settingsService, PoeKeyboardHelper keyboardHelper)
        {
            this._settingsService = settingsService;
            this._keyboardHelper = keyboardHelper;

            this._itemParser = new ItemParser();
            this._keyboardHook = new KeyboardHook(processId);
            this._keyboardHook.AddHandler('B', Modifiers.Control, this.ToggleBuild);
            this._keyboardHook.AddHandler('F', Modifiers.Alt, this.SearchItem);
            this._keyboardHook.AddHandler('R', Modifiers.Control, this.RemainingMonsters);
            this._keyboardHook.AddHandler(DeleteKeyCode, this.DeleteItem);

            this._itemParser.CheckPledgeStatus();
            this._keyboardHook.InstallAsync();
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