//-----------------------------------------------------------------------
// <copyright file="KeyboardHelper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Core.Helpers
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Desktop.Robot;
    using Desktop.Robot.Extensions;
    using Lurker.Core.Extensions;

    /// <summary>
    /// Represents the keyboard helper.
    /// </summary>
    public class KeyboardHelper
    {
        #region Fields

        private Robot _robot;
        private Process _process;
        private IntPtr _windowHandle;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardHelper" /> class.
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        public KeyboardHelper(int processId)
        {
            _robot = new Robot();

            // For the settings instance
            if (processId != 0)
            {
                this._process = Process.GetProcessById(processId);
                if (this._process != null)
                {
                    this._windowHandle = this._process.GetWindowHandle();
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Waits for next key asynchronous.
        /// </summary>
        /// <returns>The next key press.</returns>
        public Task<Winook.KeyboardMessageEventArgs> WaitForNextKeyAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<Winook.KeyboardMessageEventArgs>();

            var hook = new Winook.KeyboardHook(Process.GetCurrentProcess().Id);
            EventHandler<Winook.KeyboardMessageEventArgs> handler = default;
            handler = (object s, Winook.KeyboardMessageEventArgs e) =>
            {
                if (e.Direction == Winook.KeyDirection.Up)
                {
                    taskCompletionSource.SetResult(e);
                    hook.MessageReceived -= handler;
                    hook.Dispose();
                }
            };

            hook.MessageReceived += handler;
            hook.InstallAsync().Wait();

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The task awaiter.</returns>
        public void Write(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            _robot.Type(text, 10);
        }

        /// <summary>
        /// Simulates a search using Ctrl+F.
        /// </summary>
        /// <param name="searchTerm">The search term to use.</param>
        /// <returns>The task awaiter.</returns>
        public async Task Search(string searchTerm)
        {
            _robot.KeyPress(Key.Alt);
            Native.SetForegroundWindow(_windowHandle);

            _robot.CombineKeys(Key.Control, Key.F);
            await Task.Delay(10);
            _robot.Type(searchTerm, 10);
            await Task.Delay(10);
            _robot.KeyPress(Key.Enter);
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="skipLastReturn">Skip the last return.</param>
        /// <returns>
        /// The task awaiter.
        /// </returns>
        protected async Task SendCommand(string command, bool skipLastReturn = false)
        {
            // This is to fix the first SetForegroundWindow
            _robot.KeyPress(Key.Alt);
            Native.SetForegroundWindow(this._windowHandle);
            await Task.Delay(10);
            var foregroundWindow = Native.GetForegroundWindow();
            if (this._windowHandle != foregroundWindow)
            {
                await Task.Delay(100);
            }

            _robot.KeyPress(Key.Enter);
            _robot.CombineKeys(Key.Control, Key.A);
            _robot.Type(command, 10);

            if (!skipLastReturn)
            {
                _robot.KeyPress(Key.Enter);
            }
        }

        #endregion
    }
}