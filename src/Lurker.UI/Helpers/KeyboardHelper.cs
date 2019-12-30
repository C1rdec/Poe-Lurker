//-----------------------------------------------------------------------
// <copyright file="KeyboardHelper.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Helpers
{
    using System;

    public class KeyboardHelper
    {
        #region Fields

        private readonly object CommandLock = new object();
        private static readonly string EnterKey = "{ENTER}";
        private IntPtr _windowHandle;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardHelper"/> class.
        /// </summary>
        /// <param name="windowHandle">The window handle.</param>
        public KeyboardHelper(IntPtr windowHandle)
        {
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
                System.Windows.Forms.SendKeys.SendWait(command);
                System.Windows.Forms.SendKeys.SendWait(EnterKey);
            }
        }

        #endregion
    }
}
