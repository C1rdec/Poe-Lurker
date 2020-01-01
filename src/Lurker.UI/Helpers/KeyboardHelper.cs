//-----------------------------------------------------------------------
// <copyright file="KeyboardHelper.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Helpers
{
    using System;
    using WindowsInput;

    public class KeyboardHelper
    {
        #region Fields

        private readonly object CommandLock = new object();
        private static readonly string EnterKey = "{ENTER}";
        private IntPtr _windowHandle;
        private InputSimulator _simulator;
        

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardHelper"/> class.
        /// </summary>
        /// <param name="windowHandle">The window handle.</param>
        public KeyboardHelper(IntPtr windowHandle)
        {
            this._simulator = new InputSimulator();
            this._windowHandle = windowHandle;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends the command.
        /// </summary>
        /// <param name="command">The command.</param>
        protected void SendCommand(string command)
        {            
            lock (CommandLock)
            {                
                Native.SetForegroundWindow(this._windowHandle);
                System.Windows.Forms.SendKeys.SendWait(EnterKey);

                // We are using the interop since SendWait block mouse input.
                this._simulator.Keyboard.TextEntry(command);
                System.Windows.Forms.SendKeys.SendWait(EnterKey);
            }
        }

        #endregion
    }
}
