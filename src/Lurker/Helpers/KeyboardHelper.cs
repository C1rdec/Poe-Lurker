//-----------------------------------------------------------------------
// <copyright file="KeyboardHelper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Helpers
{
    using System;
    using System.Diagnostics;
    using Lurker.Extensions;
    using WindowsInput;

    /// <summary>
    /// Represents the keyboard helper.
    /// </summary>
    public class KeyboardHelper
    {
        #region Fields

        private readonly object _commandLock = new object();
        private Process _process;
        private InputSimulator _simulator;
        private IntPtr _windowHandle;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardHelper" /> class.
        /// </summary>
        /// <param name="process">The process.</param>
        public KeyboardHelper(Process process)
        {
            if (process != null)
            {
                this._process = process;
                this._windowHandle = this._process.GetWindowHandle();
            }

            this._simulator = new InputSimulator();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Writes the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        public void Write(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            this._simulator.Keyboard.TextEntry(text);
        }

        /// <summary>
        /// Simulates a search using Ctrl+F.
        /// </summary>
        /// <param name="searchTerm">The search term to use.</param>
        public void Search(string searchTerm)
        {
            lock (this._commandLock)
            {
                this._simulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.LMENU);
                Native.SetForegroundWindow(this._windowHandle);
                this._simulator.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.VK_F);

                // We are using the interop since SendWait block mouse input.
                this._simulator.Keyboard.TextEntry(searchTerm);
                this._simulator.Keyboard.Sleep(300);
                this._simulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);
            }
        }

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        protected void SendCommand(string command)
        {
            lock (this._commandLock)
            {
                // This is to fix the first SetForegroundWindow
                this._simulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.LMENU);
                Native.SetForegroundWindow(this._windowHandle);

                this._simulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);
                this._simulator.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.VK_A);

                // We are using the interop since SendWait block mouse input.
                this._simulator.Keyboard.TextEntry(command);
                this._simulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RETURN);
            }
        }

        #endregion
    }
}