//-----------------------------------------------------------------------
// <copyright file="KeyboardHelper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Helpers;

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Desktop.Robot;
using Desktop.Robot.Extensions;
using PoeLurker.Core.Extensions;
using TextCopy;

/// <summary>
/// Represents the keyboard helper.
/// </summary>
public class KeyboardHelper
{
    #region Fields

    private readonly Robot _robot;
    private readonly Process _process;
    private readonly IntPtr _windowHandle;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardHelper" /> class.
    /// </summary>
    /// <param name="processId">The process identifier.</param>
    public KeyboardHelper(int processId)
    {
        _robot = new Robot();

        // For the settings instance
        if (processId != 0)
        {
            _process = Process.GetProcessById(processId);
            if (_process != null)
            {
                _windowHandle = _process.GetWindowHandle();
            }
        }
    }

    #endregion

    #region Methods

    /// <summary>
    /// Waits for next key asynchronous.
    /// </summary>
    /// <returns>The next key press.</returns>
    public Task<Winook.KeyboardMessageEventArgs> WaitForNextKeyAsync()
    {
        var taskCompletionSource = new TaskCompletionSource<Winook.KeyboardMessageEventArgs>();

        var hook = new Winook.KeyboardHook(Environment.ProcessId);
        void handler(object s, Winook.KeyboardMessageEventArgs e)
        {
            if (e.Direction == Winook.KeyDirection.Up)
            {
                taskCompletionSource.SetResult(e);
                hook.MessageReceived -= handler;
                hook.Dispose();
            }
        }

        hook.MessageReceived += handler;
        hook.InstallAsync().Wait();

        return taskCompletionSource.Task;
    }

    /// <summary>
    /// Writes the specified text.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <returns>The task awaiter.</returns>
    public async Task WriteAsync(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        await ClipboardService.SetTextAsync(text);
        _robot.CombineKeys(Key.Control, Key.V);
    }

    /// <summary>
    /// Simulates a search using Ctrl+F.
    /// </summary>
    /// <param name="searchTerm">The search term to use.</param>
    /// <returns>The task awaiter.</returns>
    public async Task Search(string searchTerm)
    {
        _robot.KeyPress(Key.Alt);
        _ = Native.SetForegroundWindow(_windowHandle);

        _robot.CombineKeys(Key.Control, Key.F);
        await Task.Delay(10);
        await ClipboardService.SetTextAsync(searchTerm);
        _robot.CombineKeys(Key.Control, Key.V);
        await Task.Delay(10);
        _robot.KeyPress(Key.Enter);
    }

    /// <summary>
    /// Sends the command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="skipLastReturn">Skip the last return.</param>
    /// <returns>
    /// The task awaiter.
    /// </returns>
    protected async Task SendCommand(string command, bool skipLastReturn = false)
    {
        // This is to fix the first SetForegroundWindow
        _robot.KeyPress(Key.Alt);
        _ = Native.SetForegroundWindow(_windowHandle);
        await Task.Delay(10);
        var foregroundWindow = Native.GetForegroundWindow();
        if (_windowHandle != foregroundWindow)
        {
            await Task.Delay(100);
        }

        _robot.KeyPress(Key.Enter);
        _robot.CombineKeys(Key.Control, Key.A);
        await ClipboardService.SetTextAsync(command);
        _robot.CombineKeys(Key.Control, Key.V);

        if (!skipLastReturn)
        {
            _robot.KeyPress(Key.Enter);
        }
    }

    #endregion
}