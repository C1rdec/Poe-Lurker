//-----------------------------------------------------------------------
// <copyright file="DockingHelper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Helpers;

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using PoeLurker.Core.Extensions;
using PoeLurker.Core.Models;
using PoeLurker.Core.Services;
using static PoeLurker.Core.Native;

/// <summary>
/// Represents DockingHelper.
/// </summary>
/// <seealso cref="System.IDisposable" />
public class DockingHelper : IDisposable
{
    #region Fields

    private Bitmap _referenceInventory;
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

    private readonly Process _myProcess;
    private readonly CancellationTokenSource _tokenSource;
    private readonly SettingsService _settingsService;
    private readonly IntPtr _hook;
    private readonly IntPtr _windowHandle;
    private IntPtr _currentWindowStyle;
    private readonly Process _process;
    private readonly ClientLurker _clientLurker;

    private PoeWindowInformation _latestWindowInformation;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DockingHelper" /> class.
    /// </summary>
    /// <param name="processId">The process identifier.</param>
    /// <param name="settingsService">The settings service.</param>
    public DockingHelper(int processId, SettingsService settingsService, ClientLurker clientLurker)
    {
        using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("PoeLurker.Core.Assets.InventoryReference.png"))
        {
            _referenceInventory = new Bitmap(stream);
        }

        _process = Process.GetProcessById(processId);
        _clientLurker = clientLurker;

        if (_process != null)
        {
            _clientLurker.Poe2 += ClientLurker_Poe2;
            _myProcess = Process.GetCurrentProcess();
            _tokenSource = new CancellationTokenSource();
            _settingsService = settingsService;
            _windowHandle = _process.GetWindowHandle();

            _windowOwnerId = GetWindowThreadProcessId(_windowHandle, out _windowProcessId);
            _winEventDelegate = WhenWindowMoveStartsOrEnds;
            _hook = SetWinEventHook(0, MoveEnd, _windowHandle, _winEventDelegate, _windowProcessId, _windowOwnerId, 0);
            WindowInformation = GetWindowInformation();
            Watch();

            /*if (settingsService.VulkanRenderer)
            {
                this.RemoveWindowBorder();
            }*/
        }
    }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when [Inventory State Change].
    /// </summary>
    public event EventHandler<bool> InventoryOpenChange;

    /// <summary>
    /// Occurs when [on window move].
    /// </summary>
    public event EventHandler<PoeWindowInformation> OnChanged;

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
    public bool IsWindowInForeground => _windowHandle == Native.GetForegroundWindow();

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
        Dispose(true);
    }

    /// <summary>
    /// Sets the foreground.
    /// </summary>
    public void SetForeground()
    {
        SetForeground(_windowHandle);
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
            _clientLurker.Poe2 -= ClientLurker_Poe2;
            _myProcess.Dispose();
            _tokenSource.Cancel();
            UnhookWinEvent(_hook);
        }
    }

    private void ClientLurker_Poe2(object sender, bool e)
    {
        Notify();
    }

    /// <summary>
    /// Removes the window border.
    /// </summary>
    private void RemoveWindowBorder()
    {
        // Native.SetWindowLongPtr(new HandleRef(this, this._windowHandle), -16, (IntPtr)0x10000000);
        Native.SetWindowLong(_windowHandle, -16, 0x10000000);
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
        if (_windowHandle != hwnd)
        {
            return;
        }

        switch (eventType)
        {
            case MoveEnd:
                Notify();
                break;
            case GainMouseCapture:
                Invoke(OnMouseCapture);
                break;
            case LostMouseCapture:
                Invoke(OnLostMouseCapture);
                break;
        }
    }

    /// <summary>
    /// Watches the foregound.
    /// </summary>
    private async void Watch()
    {
        var token = _tokenSource.Token;
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

                var style = Native.GetWindowLong(_windowHandle, -16);
                if (_currentWindowStyle != style)
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

                    _currentWindowStyle = style;
                    Notify();
                }

                if (processId == _myProcess.Id || foregroundWindow == _windowHandle)
                {
                    inForeground = true;
                }

                if (PoeApplicationContext.InForeground != inForeground)
                {
                    if (inForeground)
                    {
                        Notify();
                    }

                    PoeApplicationContext.InForeground = inForeground;
                    if (_settingsService.HideInBackground)
                    {
                        OnForegroundChange?.Invoke(this, inForeground);
                    }
                }

                if (inForeground)
                {
                    Capture();
                }

                await Task.Delay(800);
            }
            catch
            {
            }
        }
    }

    private void Capture()
    {
        var captureRegion = new Rectangle(_latestWindowInformation.Position.Right - 2, _latestWindowInformation.Position.Top + 1, 2, 2);

        var b = ScreenCapture.CaptureRegion(captureRegion);
        b.Save("screen.png", ImageFormat.Png);
        if (FuzzyMatch(b))
        {
            if (!_latestWindowInformation.InventoryOpen)
            {
                Notify(GetWindowInformation(true));
            }
        }
        else if (_latestWindowInformation.InventoryOpen)
        {
            Notify();
        }
    }

    private bool FuzzyMatch(Bitmap screen)
    {
        var tolerance = 80;
        var width = _referenceInventory.Width;
        var height = _referenceInventory.Height;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var screenPixel = screen.GetPixel(x, y);
                var refPixel = _referenceInventory.GetPixel(x, y);

                var diff = Math.Abs(screenPixel.R - refPixel.R)
                         + Math.Abs(screenPixel.G - refPixel.G)
                         + Math.Abs(screenPixel.B - refPixel.B);

                if (diff > tolerance)
                {
                    return false;
                }
            }
        }

        return true;
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
    private void Notify()
        => Notify(GetWindowInformation());

    private void Notify(PoeWindowInformation information)
    {
        if (information != null)
        {
            WindowInformation = information;
            OnChanged?.Invoke(this, WindowInformation);
        }
    }

    /// <summary>
    /// Gets the window information.
    /// </summary>
    /// <returns>The poe window information.</returns>
    private PoeWindowInformation GetWindowInformation(bool inventoryOpen = false)
    {
        var poePosition = HandleWideScreen(out var wideScreen);
        double poeWidth = poePosition.Right - poePosition.Left;
        double poeHeight = poePosition.Bottom - poePosition.Top;

        var expBarHeight = poeHeight * DefaultExpBarHeight / DefaultHeight;
        var flaskBarWidth = poeHeight * DefaultFlaskBarWidth / DefaultHeight;
        var flaskBarHeight = poeHeight * DefaultFlaskBarHeight / DefaultHeight;

        _latestWindowInformation = new PoeWindowInformation()
        {
            Height = poeHeight,
            Width = poeWidth,
            ExpBarHeight = expBarHeight,
            FlaskBarHeight = flaskBarHeight,
            FlaskBarWidth = flaskBarWidth,
            Position = poePosition,
            WideScreen = wideScreen,
            InventoryOpen = inventoryOpen,
        };

        return _latestWindowInformation;
    }

    private Rect HandleWideScreen(out bool wideScreen)
    {
        Rect poePosition = default;
        Native.GetWindowRect(_windowHandle, ref poePosition);

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
            wideScreen = true;

            return new Rect
            {
                Right = poePosition.Right - blackBarWidth,
                Left = poePosition.Left + blackBarWidth,
                Bottom = poePosition.Bottom,
                Top = poePosition.Top,
            };
        }

        wideScreen = false;

        return poePosition;
    }

    #endregion
}