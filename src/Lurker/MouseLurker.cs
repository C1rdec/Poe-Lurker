//-----------------------------------------------------------------------
// <copyright file="MouseLurker.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Lurker.Helpers;
    using Lurker.Patreon.Models;
    using Lurker.Services;
    using WindowsInput;
    using Winook;

    /// <summary>
    /// Represents the mouse lurker.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class MouseLurker : IDisposable
    {
        #region Fields

        private InputSimulator _simulator;
        private SettingsService _settingsService;
        private MouseHook _mouseHook;
        private bool _disposed = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseLurker" /> class.
        /// </summary>
        /// <param name="process">The process.</param>
        /// <param name="settingsService">The settings service.</param>
        public MouseLurker(Process process, SettingsService settingsService)
        {
            var is64BitProcess = process.ProcessName.Contains("64");

            this._settingsService = settingsService;
            this._simulator = new InputSimulator();
            this._mouseHook = new MouseHook(process.Id, is64BitProcess ? ProcessBitness.Is64Bit : ProcessBitness.Is32Bit);
            this._mouseHook.LeftButtonUp += this.MouseHook_LeftButtonUp;
            this._mouseHook.Install();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a new item is in the clipboard.
        /// </summary>
        public event EventHandler<PoeItem> Newitem;

        #endregion

        #region Methods

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
                        this._mouseHook.LeftButtonUp -= this.MouseHook_LeftButtonUp;
                        this._mouseHook.Dispose();
                    }
                    catch
                    {
                    }
                }

                this._disposed = true;
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the MouseHookService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void MouseHook_LeftButtonUp(object sender, EventArgs e)
        {
            if (!this._settingsService.SearchEnabled)
            {
                return;
            }

            if (Native.IsKeyPressed(Native.VirtualKeyStates.VK_SHIFT) && Native.IsKeyPressed(Native.VirtualKeyStates.VK_CONTROL))
            {
                this.ParseItem();
            }
        }

        /// <summary>
        /// Parses the item.
        /// </summary>
        private async void ParseItem()
        {
            PoeItem item = default;
            var retryCount = 2;
            for (int i = 0; i < retryCount; i++)
            {
                this._simulator.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.VK_C);
                await Task.Delay(20);
                item = ClipboardHelper.GetItemInClipboard();

                if (item == null)
                {
                    return;
                }

                if (!item.Identified)
                {
                    await Task.Delay(50);
                    continue;
                }

                break;
            }

            if (item == null || !item.Identified)
            {
                return;
            }

            this.Newitem?.Invoke(this, item);
            ClipboardHelper.ClearClipboard();
        }

        #endregion
    }
}