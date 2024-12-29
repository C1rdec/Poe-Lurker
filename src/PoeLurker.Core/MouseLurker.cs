//-----------------------------------------------------------------------
// <copyright file="MouseLurker.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core;

using System;
using System.Threading.Tasks;
using PoeLurker.Core.Helpers;
using PoeLurker.Core.Services;
using PoeLurker.Patreon.Models;
using Winook;

/// <summary>
/// Represents the mouse lurker.
/// </summary>
/// <seealso cref="System.IDisposable" />
public class MouseLurker : IDisposable
{
    #region Fields

    private readonly SettingsService _settingsService;
    private readonly MouseHook _mouseHook;
    private bool _disposed = false;
    private int _x;
    private int _y;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="MouseLurker" /> class.
    /// </summary>
    /// <param name="processId">The process identifier.</param>
    /// <param name="settingsService">The settings service.</param>
    public MouseLurker(int processId, SettingsService settingsService)
    {
        _settingsService = settingsService;
        _mouseHook = new MouseHook(processId, MouseMessageTypes.All);

        _mouseHook.RightButtonUp += MouseHook_RightButtonUp;
        _mouseHook.LeftButtonUp += MouseHook_LeftButtonUp;
        _mouseHook.MouseMove += MouseHook_MouseMove;
        _mouseHook.InstallAsync();
    }

    #endregion

    #region Events

    /// <summary>
    /// Occurs when [item identified].
    /// </summary>
    public event EventHandler<PoeItem> ItemIdentified;

    /// <summary>
    /// Occurs when a new item is in the clipboard.
    /// </summary>
    public event EventHandler<PoeItem> ItemDetails;

    /// <summary>
    /// Occurs when [mouse mouve].
    /// </summary>
    public event EventHandler<MouseMessageEventArgs> MouseMove;

    /// <summary>
    /// Occurs when [mouse mouve left button up].
    /// </summary>
    public event EventHandler MouseLeftButtonUp;

    #endregion

    #region Properties

    /// <summary>
    /// Gets the x.
    /// </summary>
    public int X => _x;

    /// <summary>
    /// Gets the y.
    /// </summary>
    public int Y => _y;

    #endregion

    #region Methods

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    public void Dispose() => Dispose(true);

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                try
                {
                    _mouseHook.LeftButtonUp -= MouseHook_LeftButtonUp;
                    _mouseHook.RemoveAllHandlers();
                    _mouseHook.Uninstall();
                    _mouseHook.Dispose();
                }
                catch
                {
                }
            }

            _disposed = true;
        }
    }

    /// <summary>
    /// Handles the MouseMove event of the MouseHook control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseMessageEventArgs"/> instance containing the event data.</param>
    private void MouseHook_MouseMove(object sender, MouseMessageEventArgs e)
    {
        _x = e.X;
        _y = e.Y;

        MouseMove?.Invoke(this, e);
    }

    /// <summary>
    /// Handles the MouseLeftButtonUp event of the MouseHookService control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private async void MouseHook_LeftButtonUp(object sender, MouseMessageEventArgs e)
    {
        MouseLeftButtonUp?.Invoke(this, e);

        if (e.Control || !e.Shift || (!_settingsService.MapEnabled && !_settingsService.SearchEnabled))
        {
            return;
        }

        await Task.Delay(100);
        var item = await ParseItem();

        if (item != null)
        {
            ItemIdentified?.Invoke(this, item);
        }

        ClipboardHelper.ClearClipboard();
    }

    /// <summary>
    /// Handles the RightButtonUp event of the MouseHook control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="MouseMessageEventArgs"/> instance containing the event data.</param>
    private async void MouseHook_RightButtonUp(object sender, MouseMessageEventArgs e)
    {
        if (e.Alt && _settingsService.SearchEnabled)
        {
            await Task.Delay(100);
            var item = await ParseItem();
            if (item != null)
            {
                ItemDetails?.Invoke(this, item);
            }

            ClipboardHelper.ClearClipboard();
        }
    }

    /// <summary>
    /// Parses the item.
    /// </summary>
    private async Task<PoeItem> ParseItem()
    {
        PoeItem item = default;
        var retryCount = 2;
        for (var i = 0; i < retryCount; i++)
        {
            item = await ClipboardHelper.GetItemInClipboard();
            if (item == null)
            {
                return null;
            }

            if (!item.Identified)
            {
                await Task.Delay(50);
                continue;
            }

            break;
        }

        if (item == null || !item.Identified)
        {
            return null;
        }

        if (item.ItemClass == ItemClass.Unknown)
        {
            return null;
        }

        return item;
    }

    #endregion
}