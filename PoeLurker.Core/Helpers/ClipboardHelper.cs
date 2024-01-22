//-----------------------------------------------------------------------
// <copyright file="ClipboardHelper.cs" company="Wohs Inc.">
//     Copyright © Wohs Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PoeLurker.Core.Helpers;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Desktop.Robot;
using Desktop.Robot.Extensions;
using PoeLurker.Patreon.Models;
using PoeLurker.Patreon.Parsers;

/// <summary>
/// Represents the Clipboard helper.
/// </summary>
public static class ClipboardHelper
{
    #region Fields

    private static readonly Robot Robot = new();
    private static readonly ItemParser ItemParser = new();

    #endregion

    #region Methods

    /// <summary>
    /// Gets the clipboard data.
    /// </summary>
    /// <returns>The clipboard text.</returns>
    public static string GetClipboardText()
    {
        return GetLegacy();
    }

    /// <summary>
    /// Clears the clipboard.
    /// </summary>
    public static void ClearClipboard()
    {
        RetryOnMainThread(() =>
        {
            Clipboard.Clear();
        });
    }

    /// <summary>
    /// Gets the item in clipboard.
    /// </summary>
    /// <returns>The item in the clipboard.</returns>
    public static async Task<PoeItem> GetItemInClipboard()
    {
        try
        {
            Robot.CombineKeys(Key.Control, Key.C);
            await Task.Delay(100);
            var text = GetClipboardText();
            return ItemParser.Parse(text);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Checks the pledge asynchronous.
    /// </summary>
    /// <returns>The task awaiter.</returns>
    public static Task CheckPledgeStatusAsync()
    {
        return ItemParser.CheckPledgeStatus();
    }

    /// <summary>
    /// Retries the on main thread.
    /// </summary>
    /// <param name="action">The action.</param>
    private static void RetryOnMainThread(Action action)
    {
        Thread thread = new(() =>
        {
            var retryCount = 3;
            while (retryCount != 0)
            {
                try
                {
                    action();
                    break;
                }
                catch
                {
                    retryCount--;
                    Thread.Sleep(200);
                }
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();
    }

    private static string GetLegacy()
    {
        var clipboardText = string.Empty;
        RetryOnMainThread(() =>
        {
            clipboardText = Clipboard.GetText();
        });

        if (string.IsNullOrEmpty(clipboardText))
        {
            return string.Empty;
        }

        return clipboardText;
    }

    #endregion
}