//-----------------------------------------------------------------------
// <copyright file="DockingHelper.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Helpers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using static Lurker.UI.Native;

    public class DockingHelper: IDisposable
    {
        #region Fields

        private const uint MoveEnd = 11;
        private readonly uint _windowProcessId, _windowOwnerId;
        private readonly IntPtr _windowHandle;
        private readonly WinEventDelegate _winEventDelegate;
        private IntPtr _hook;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DockingHelper"/> class.
        /// </summary>
        /// <param name="process">The process.</param>
        public DockingHelper(Process process)
        {
            this._windowHandle = process.MainWindowHandle;
            this._windowOwnerId = GetWindowThreadProcessId(this._windowHandle, out this._windowProcessId);
            this._winEventDelegate = WhenWindowMoveStartsOrEnds;
            this._hook = SetWinEventHook(0, MoveEnd, this._windowHandle, this._winEventDelegate, this._windowProcessId, this._windowOwnerId, 0);
        }

        #endregion

        #region Events

        public event EventHandler OnWindowMove;

        #endregion

        #region Methods

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnhookWinEvent(this._hook);
            }
        }

        /// <summary>
        /// Notifies this instance.
        /// </summary>
        private void Notify()
        {
            this.OnWindowMove?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Whens the window move starts or ends.
        /// </summary>
        /// <param name="hWinEventHook">The h win event hook.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="hwnd">The HWND.</param>
        /// <param name="idObject">The identifier object.</param>
        /// <param name="idChild">The identifier child.</param>
        /// <param name="dwEventThread">The dw event thread.</param>
        /// <param name="dwmsEventTime">The DWMS event time.</param>
        private void WhenWindowMoveStartsOrEnds(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            File.AppendAllLines(@"C:\Temp\t.txt", new string[] { eventType.ToString() });
            if (this._windowHandle != hwnd)
            {
                return;
            }

            switch(eventType)
            {
                case MoveEnd:
                    this.Notify();
                    break;
            }
        }

        #endregion
    }
}
