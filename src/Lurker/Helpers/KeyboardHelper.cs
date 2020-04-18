//-----------------------------------------------------------------------
// <copyright file="KeyboardHelper.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Helpers
{
    using Lurker.Extensions;
    using System;
    using System.Diagnostics;
    using WindowsInput;

    public class KeyboardHelper
    {
        #region Fields

        private readonly object CommandLock = new object();
        private Process _process;
        private InputSimulator _simulator;
        private IntPtr _windowHandle;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardHelper"/> class.
        /// </summary>
        /// <param name="windowHandle">The window handle.</param>
        public KeyboardHelper(Process process)
            : this()
        {
            this._process = process;
            this._windowHandle = this._process.GetWindowHandle();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardHelper"/> class.
        /// </summary>
        public KeyboardHelper()
        {
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
        /// Simulates a search using Ctrl+F 
        /// </summary>
        /// <param name="searchTerm">The search term to use</param>
        public void Search(string searchTerm)
        {
            lock (CommandLock)
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
            lock (CommandLock)
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
