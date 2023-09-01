//-----------------------------------------------------------------------
// <copyright file="DockingHelper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Helpers
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Lurker.Extensions;
    using Lurker.Models;
    using Lurker.Services;
    using static Lurker.Native;

    /// <summary>
    /// Represents DockingHelper.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class DockingHelper : IDisposable
    {
        #region Fields

        private const uint WideScreenHeightReference = 1440;
        private const uint WideScreenWidthReference = 3455;
        private const uint GainMouseCapture = 8;
        private const uint LostMouseCapture = 9;
        private const uint MoveEnd = 11;

        private static readonly int DefaultFlaskBarHeight = 122;
        private static readonly int DefaultFlaskBarWidth = 550;
        private static readonly int DefaultExpBarHeight = 24;
        private static readonly int DefaultHeight = 1080;

        private readonly uint _windowProcessId;
        private readonly uint _windowOwnerId;
        private readonly WinEventDelegate _winEventDelegate;

        private Process _myProcess;
        private CancellationTokenSource _tokenSource;
        private SettingsService _settingsService;
        private IntPtr _hook;
        private IntPtr _windowHandle;
        private IntPtr _currentWindowStyle;
        private Process _process;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DockingHelper" /> class.
        /// </summary>
        /// <param name="processId">The process identifier.</param>
        /// <param name="settingsService">The settings service.</param>
        public DockingHelper(int processId, SettingsService settingsService)
        {
            this._process = ProcessLurker.GetProcessById(processId);
            if (this._process != null)
            {
                this._myProcess = Process.GetCurrentProcess();
                this._tokenSource = new CancellationTokenSource();
                this._settingsService = settingsService;
                this._windowHandle = this._process.GetWindowHandle();

                this._windowOwnerId = GetWindowThreadProcessId(this._windowHandle, out this._windowProcessId);
                this._winEventDelegate = this.WhenWindowMoveStartsOrEnds;
                this._hook = SetWinEventHook(0, MoveEnd, this._windowHandle, this._winEventDelegate, this._windowProcessId, this._windowOwnerId, 0);
                this.WindowInformation = this.GetWindowInformation();
                this.WatchForegound();

                /*if (settingsService.VulkanRenderer)
                {
                    this.RemoveWindowBorder();
                }*/
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when [on window move].
        /// </summary>
        public event EventHandler<PoeWindowInformation> OnWindowMove;

        /// <summary>
        /// Occurs when [on lost mouse capture].
        /// </summary>
        public event EventHandler OnLostMouseCapture;

        /// <summary>
        /// Occurs when [on mouse capture].
        /// </summary>
        public event EventHandler OnMouseCapture;

        /// <summary>
        /// Occurs when [on foreground change].
        /// </summary>
        public event EventHandler<bool> OnForegroundChange;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is window in foreground.
        /// </summary>
        public bool IsWindowInForeground => this._windowHandle == Native.GetForegroundWindow();

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
        /// Sets the foreground.
        /// </summary>
        public void SetForeground()
        {
            this.SetForeground(this._windowHandle);
        }

        /// <summary>
        /// Sets the foreground.
        /// </summary>
        /// <param name="handle">The handle.</param>
        public void SetForeground(IntPtr handle)
        {
            Native.SetForegroundWindow(handle);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._myProcess.Dispose();
                this._tokenSource.Cancel();
                UnhookWinEvent(this._hook);
            }
        }

        /// <summary>
        /// Removes the window border.
        /// </summary>
        private void RemoveWindowBorder()
        {
            // Native.SetWindowLongPtr(new HandleRef(this, this._windowHandle), -16, (IntPtr)0x10000000);
            Native.SetWindowLong(this._windowHandle, -16, 0x10000000);
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

            switch (eventType)
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
            }
        }

        /// <summary>
        /// Watches the foregound.
        /// </summary>
        private async void WatchForegound()
        {
            var token = this._tokenSource.Token;
            while (true)
            {
                try
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }

                    var inForeground = false;
                    var foregroundWindow = Native.GetForegroundWindow();
                    GetWindowThreadProcessId(foregroundWindow, out var processId);

                    var style = Native.GetWindowLong(this._windowHandle, -16);
                    if (this._currentWindowStyle != style)
                    {
                        switch ((uint)style)
                        {
                            case 0x14cf0000:
                                PoeApplicationContext.WindowStyle = WindowStyle.Windowed;
                                break;
                            case 0x94000000:
                                PoeApplicationContext.WindowStyle = WindowStyle.WindowedFullScreen;
                                break;
                        }

                        this._currentWindowStyle = style;
                        this.InvokeWindowMove();
                    }

                    if (processId == this._myProcess.Id || foregroundWindow == this._windowHandle)
                    {
                        inForeground = true;
                    }

                    if (PoeApplicationContext.InForeground != inForeground)
                    {
                        if (inForeground)
                        {
                            this.InvokeWindowMove();
                        }

                        PoeApplicationContext.InForeground = inForeground;
                        if (this._settingsService.HideInBackground)
                        {
                            this.OnForegroundChange?.Invoke(this, inForeground);
                        }
                    }

                    await Task.Delay(500);
                }
                catch
                {
                }
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
            var information = this.GetWindowInformation();
            if (information != null) { }
            this.WindowInformation = this.GetWindowInformation();
            this.OnWindowMove?.Invoke(this, this.WindowInformation);
        }

        /// <summary>
        /// Gets the window information.
        /// </summary>
        /// <returns>The poe window information.</returns>
        private PoeWindowInformation GetWindowInformation()
        {
            var poePosition = this.HandleWideScreen();
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

        private Rect HandleWideScreen()
        {
            Rect poePosition = default;
            Native.GetWindowRect(this._windowHandle, ref poePosition);

            // Windows weird margins
            if (PoeApplicationContext.WindowStyle == WindowStyle.Windowed)
            {
                poePosition.Left += 8;
                poePosition.Right -= 8;
                poePosition.Top += 29;
                poePosition.Bottom -= 10;
            }

            double windowWidth = poePosition.Right - poePosition.Left;
            double windowHeight = poePosition.Bottom - poePosition.Top;

            var blackBarBreakPoint = windowHeight * WideScreenWidthReference / WideScreenHeightReference;

            var totalBlackBars = windowWidth - blackBarBreakPoint;
            if (totalBlackBars > 0)
            {
                var blackBarWidth = Convert.ToInt32(totalBlackBars / 2);
                return new Rect
                {
                    Right = poePosition.Right - blackBarWidth,
                    Left = poePosition.Left + blackBarWidth,
                    Bottom = poePosition.Bottom,
                    Top = poePosition.Top,
                };
            }

            return poePosition;
        }

        #endregion
    }
}