//-----------------------------------------------------------------------
// <copyright file="MouseLurker.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker
{
    using System;
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

        private SettingsService _settingsService;
        private MouseHook _mouseHook;
        private bool _disposed = false;
        private int _x;
        private int _y;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseLurker" /> class.
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        /// <param name="settingsService">The settings service.</param>
        public MouseLurker(int processId, SettingsService settingsService)
        {
            this._settingsService = settingsService;
            this._mouseHook = new MouseHook(processId, MouseMessageTypes.All);
            this._mouseHook.LeftButtonUp += this.MouseHook_LeftButtonUp;
            this._mouseHook.MouseMove += this.MouseHook_MouseMove;
            this._mouseHook.InstallAsync();
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a new item is in the clipboard.
        /// </summary>
        public event EventHandler<PoeItem> Newitem;

        /// <summary>
        /// Occurs when [mouse mouve].
        /// </summary>
        public event EventHandler<MouseMessageEventArgs> MouseMove;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the x.
        /// </summary>
        public int X => this._x;

        /// <summary>
        /// Gets the y.
        /// </summary>
        public int Y => this._y;

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
        /// Handles the MouseMove event of the MouseHook control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseMessageEventArgs"/> instance containing the event data.</param>
        private void MouseHook_MouseMove(object sender, MouseMessageEventArgs e)
        {
            this._x = e.X;
            this._y = e.Y;

            this.MouseMove?.Invoke(this, e);
        }

        /// <summary>
        /// Handles the MouseLeftButtonUp event of the MouseHookService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void MouseHook_LeftButtonUp(object sender, EventArgs e)
        {
            if (Native.IsKeyPressed(Native.VirtualKeyStates.VK_SHIFT))
            {
                await Task.Delay(100);
                await this.ParseItem();
            }
        }

        /// <summary>
        /// Parses the item.
        /// </summary>
        private async Task ParseItem()
        {
            PoeItem item = default;
            var retryCount = 2;
            for (int i = 0; i < retryCount; i++)
            {
                await Simulate.Events().ClickChord(WindowsInput.Events.KeyCode.LControlKey, WindowsInput.Events.KeyCode.C).Invoke();
                await Task.Delay(100);
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

            if (item.ItemClass == ItemClass.Unknown)
            {
                return;
            }

            this.Newitem?.Invoke(this, item);
            ClipboardHelper.ClearClipboard();
        }

        #endregion
    }
}