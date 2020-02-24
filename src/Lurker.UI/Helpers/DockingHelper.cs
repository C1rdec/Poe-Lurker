//-----------------------------------------------------------------------
// <copyright file="DockingHelper.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.UI.Helpers
{
    using System;
    using System.Diagnostics;
    using Lurker.UI.Models;
    using static Lurker.Native;

    public class DockingHelper: IDisposable
    {
        #region Fields

        private static int DefaultFlaskBarHeight = 122;
        private static int DefaultFlaskBarWidth = 550;
        private static int DefaultExpBarHeight = 24;
        private static int DefaultHeight = 1080;

        private const uint LostFocus = 3;
        private const uint GainMouseCapture = 8;
        private const uint LostMouseCapture = 9;
        private const uint MoveEnd = 11;
        private const uint Minimized = 22;
        private const uint ObjectDestroy = 32769;
        private const uint ObjectHidden = 8003;

        private readonly uint _windowProcessId, _windowOwnerId;
        private readonly WinEventDelegate _winEventDelegate;
        private IntPtr _hook;
        private Process _process;
        private IntPtr _windowHandle;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DockingHelper"/> class.
        /// </summary>
        /// <param name="process">The process.</param>
        public DockingHelper(Process process)
        {
            this._process = process;
            this._process.WaitForInputIdle();

            this._windowHandle = this._process.MainWindowHandle;
            this._windowOwnerId = GetWindowThreadProcessId(this._windowHandle, out this._windowProcessId);
            this._winEventDelegate = WhenWindowMoveStartsOrEnds;
            this._hook = SetWinEventHook(0, MoveEnd, this._windowHandle, this._winEventDelegate, this._windowProcessId, this._windowOwnerId, 0);

            this.WindowInformation = this.GetWindowInformation();
        }

        #endregion

        #region Events

        public event EventHandler<PoeWindowInformation> OnWindowMove;

        public event EventHandler OnLostMouseCapture;

        public event EventHandler OnMouseCapture;

        public event EventHandler OnForegroundChange;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the window information.
        /// </summary>
        public PoeWindowInformation WindowInformation { get; private set; }

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
            if (this._windowHandle != hwnd)
            {
                return;
            }

            switch(eventType)
            {
                case MoveEnd:
                    this.InvokeWindowMove();
                    break;
                case GainMouseCapture:
                    this.Invoke(this.OnMouseCapture);
                    break;
                case LostMouseCapture:
                    this.Invoke(this.OnLostMouseCapture);
                    break;
                case LostFocus:
                    this.Invoke(this.OnForegroundChange);
                    break;
            }
        }

        /// <summary>
        /// Invokes the specified handler.
        /// </summary>
        /// <param name="handler">The handler.</param>
        private void Invoke(EventHandler handler)
        {
            handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Invokes the window move.
        /// </summary>
        private void InvokeWindowMove()
        {
            this.WindowInformation = this.GetWindowInformation();
            this.OnWindowMove?.Invoke(this, this.WindowInformation);
        }

        /// <summary>
        /// Gets the window information.
        /// </summary>
        /// <returns></returns>
        private PoeWindowInformation GetWindowInformation()
        {
            Native.GetWindowRect(this._windowHandle, out var poePosition);
            double poeWidth = poePosition.Right - poePosition.Left;
            double poeHeight = poePosition.Bottom - poePosition.Top;

            var expBarHeight = poeHeight * DefaultExpBarHeight / DefaultHeight;
            var flaskBarWidth = poeHeight * DefaultFlaskBarWidth / DefaultHeight;
            var flaskBarHeight = poeHeight * DefaultFlaskBarHeight / DefaultHeight;

            return new PoeWindowInformation()
            {
                Height = poeHeight,
                Width = poeWidth,
                ExpBarHeight = expBarHeight,
                FlaskBarHeight = flaskBarHeight,
                FlaskBarWidth = flaskBarWidth,
                Position = poePosition,
            };
        }

        #endregion
    }
}
