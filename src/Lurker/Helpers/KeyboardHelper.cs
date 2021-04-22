//-----------------------------------------------------------------------
// <copyright file="KeyboardHelper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Helpers
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Lurker.Extensions;
    using WindowsInput;
    using WindowsInput.Events;

    /// <summary>
    /// Represents the keyboard helper.
    /// </summary>
    public class KeyboardHelper
    {
        #region Fields

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
            // For the settings instance
            if (processId != 0)
            {
                this._process = ProcessLurker.GetProcessById(processId);
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
        public Task<ushort> WaitForNextKeyAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<ushort>();

            var hook = new Winook.KeyboardHook(ProcessLurker.CurrentProcessId);
            EventHandler<Winook.KeyboardMessageEventArgs> handler = default;
            handler = (object s, Winook.KeyboardMessageEventArgs e) =>
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
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The task awaiter.</returns>
        public async Task Write(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            await Simulate.Events().Click(text).Invoke();
        }

        /// <summary>
        /// Simulates a search using Ctrl+F.
        /// </summary>
        /// <param name="searchTerm">The search term to use.</param>
        /// <returns>The task awaiter.</returns>
        public async Task Search(string searchTerm)
        {
            await Simulate.Events().Click(KeyCode.LMenu).Invoke();
            Native.SetForegroundWindow(this._windowHandle);

            var eventBuilder = Simulate.Events();
            eventBuilder.ClickChord(KeyCode.LControl, KeyCode.F);
            eventBuilder.Click(searchTerm);
            eventBuilder.Click(KeyCode.Return);
            await eventBuilder.Invoke();
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// The task awaiter.
        /// </returns>
        protected async Task SendCommand(string command)
        {
            // This is to fix the first SetForegroundWindow
            await Simulate.Events().Click(KeyCode.LMenu).Invoke();
            Native.SetForegroundWindow(this._windowHandle);
            await Task.Delay(10);
            var foregroundWindow = Native.GetForegroundWindow();
            if (this._windowHandle != foregroundWindow)
            {
                await Task.Delay(100);
            }

            var eventBuilder = Simulate.Events();
            eventBuilder.Click(KeyCode.Return);
            eventBuilder.ClickChord(KeyCode.LControl, KeyCode.A);
            eventBuilder.Click(command);
            eventBuilder.Click(KeyCode.Return);

            await eventBuilder.Invoke();
        }

        #endregion
    }
}