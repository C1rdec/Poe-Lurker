//-----------------------------------------------------------------------
// <copyright file="DockingHelper.cs" company="Wohs">
//     Missing Copyright information from a valid stylecop.json file.
// </copyright>
//-----------------------------------------------------------------------

namespace Lurker.Helpers
{
    using Lurker.Models;
    using Lurker.Services;
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
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

        private Process _myProcess;
        private readonly uint _windowProcessId, _windowOwnerId;
        private readonly WinEventDelegate _winEventDelegate;
        private CancellationTokenSource _tokenSource;
        private SettingsService _settingsService;
        private IntPtr _hook;
        private IntPtr _hookEx;
        private IntPtr _windowHandle;
        private HookProc _procedure;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DockingHelper"/> class.
        /// </summary>
        /// <param name="process">The process.</param>
        public DockingHelper(IntPtr windowHandle, SettingsService settingsService)
        {
            this._myProcess = Process.GetCurrentProcess();
            this._tokenSource = new CancellationTokenSource();
            this._settingsService = settingsService;
            this._windowHandle = windowHandle;
            this._windowOwnerId = GetWindowThreadProcessId(this._windowHandle, out this._windowProcessId);
            this._winEventDelegate = WhenWindowMoveStartsOrEnds;
            this._hook = SetWinEventHook(0, MoveEnd, this._windowHandle, this._winEventDelegate, this._windowProcessId, this._windowOwnerId, 0);

            this._procedure = new HookProc(this.TestProc);
            this._hookEx = SetWindowsHookEx(HookType.WH_MOUSE_LL, this._procedure, IntPtr.Zero, 0);
            this.WindowInformation = this.GetWindowInformation();
            this.WatchForegound();
        }

        #endregion

        #region Events

        public event EventHandler<PoeWindowInformation> OnWindowMove;

        public event EventHandler OnLostMouseCapture;

        public event EventHandler OnMouseCapture;

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
        /// Tests the proc.
        /// </summary>
        /// <param name="nCode">The n code.</param>
        /// <param name="wParam">The w parameter.</param>
        /// <param name="lParam">The l parameter.</param>
        /// <returns></returns>
        public int TestProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            return CallNextHookEx(this._hookEx, nCode, wParam, lParam);
        }

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
                this._myProcess.Dispose();
                this._tokenSource.Cancel();
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
            }
        }

        /// <summary>
        /// Watches the foregound.
        /// </summary>
        private async void WatchForegound()
        {
            var token = this._tokenSource.Token;
            while(true)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                var inForeground = false;
                var foregroundWindow = Native.GetForegroundWindow();

                if (foregroundWindow == this._windowHandle)
                {
                    inForeground = true;
                }

                GetWindowThreadProcessId(foregroundWindow, out var processId);
                if (processId == this._myProcess.Id)
                {
                    inForeground = true;
                }

                if (PoeApplicationContext.InForeground != inForeground)
                {
                    PoeApplicationContext.InForeground = inForeground;

                    if (this._settingsService.HideInBackground)
                    {
                        this.OnForegroundChange?.Invoke(this, inForeground);
                    }
                }

                await Task.Delay(500);
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
            Rect poePosition = default;
            Native.GetWindowRect(this._windowHandle, ref poePosition);
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
